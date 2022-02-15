using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using blendnet.api.proxy.Notification;
using blendnet.common.dto;
using blendnet.common.dto.Events;
using blendnet.common.dto.Exceptions;
using blendnet.common.dto.Notification;
using blendnet.common.dto.User;
using blendnet.common.infrastructure.Storage;
using blendnet.user.repository.Interfaces;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using System.Diagnostics;

namespace blendnet.user.listener.Common
{
    /// <summary>
    /// Commmon data export completed event handler
    /// </summary>
    public class DataExportCompletedIntegrationEventHandler
    {
        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        private readonly UserAppSettings _appSettings;

        private readonly NotificationProxy _notificationProxy;

        private readonly IUserRepository _userRepository;

        private readonly BlobServiceClient _userDataBlobServiceClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="tc"></param>
        /// <param name="optionsMonitor"></param>
        /// <param name="userRepository"></param>
        public DataExportCompletedIntegrationEventHandler(ILogger<DataExportCompletedIntegrationEventHandler> logger,
                                                            TelemetryClient tc,
                                                            IAzureClientFactory<BlobServiceClient> blobClientFactory,
                                                            IOptionsMonitor<UserAppSettings> optionsMonitor,
                                                            IUserRepository userRepository,
                                                            NotificationProxy notificationProxy)
        {
            _logger = logger;

            _telemetryClient = tc;

            _userDataBlobServiceClient = blobClientFactory.CreateClient(ApplicationConstants.StorageInstanceNames.UserDataStorage);

            _appSettings = optionsMonitor.CurrentValue;

            _userRepository = userRepository;

            _notificationProxy = notificationProxy;
        }

        /// <summary>
        /// Handle the data export complete for all services.
        /// Step 1 : Once the data export is complete from all 3 service, change the status to export complete
        /// Step 2 : Generat the zip file and send notification with SAS URL to end user
        /// </summary>
        /// <param name="dataExportCompletedIntegrationEvent"></param>
        /// <param name="serviceName"></param>
        /// <param name="operationName"></param>
        /// <returns></returns>

        public async Task Handle(BaseDataOperationCompletedIntegrationEvent dataExportCompletedIntegrationEvent,
                                    string operationName)
        {
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>(operationName))
                {
                    if (dataExportCompletedIntegrationEvent.CommandId == Guid.Empty ||
                        dataExportCompletedIntegrationEvent.UserId == Guid.Empty ||
                        string.IsNullOrEmpty(dataExportCompletedIntegrationEvent.UserPhoneNumber) ||
                        string.IsNullOrEmpty(dataExportCompletedIntegrationEvent.ServiceName))
                    {
                        _logger.LogWarning($"No user details found in data export integration event. Pass correct data to integation event");

                        return;
                    }

                    _logger.LogInformation($"Message Recieved for user id: {dataExportCompletedIntegrationEvent.UserId} command id: {dataExportCompletedIntegrationEvent.CommandId}");

                    common.dto.User.User user = await _userRepository.GetUserByPhoneNumber(dataExportCompletedIntegrationEvent.UserPhoneNumber);

                    UserCommand dataExportCommand = await  _userRepository.GetCommand(dataExportCompletedIntegrationEvent.UserPhoneNumber, dataExportCompletedIntegrationEvent.CommandId);

                    if (user == null || dataExportCommand == null)
                    {
                        _logger.LogWarning($"No user details or command details found in database. UserID {dataExportCompletedIntegrationEvent.UserId} CommandId {dataExportCompletedIntegrationEvent.CommandId}");

                        return;

                    }

                    //step 1 : Once the data export is complete from all 3 service, change the status to export complete
                    //Since multiple listeners are doing the export, high posibility that User or Command object is changed by other thread.

                    AsyncRetryPolicy policyBuilder = GetRetryPolicy<BlendNetCosmosTransactionalBatchException>();

                    await policyBuilder.ExecuteAsync(async () => 
                    {
                        await UpdateStatus(dataExportCompletedIntegrationEvent);
                    });

                    //step 2 : Generate the zip file and send notification with SAS URL to end user
                    //         in case all the processes are done exporting the data. Zip the files
                    await GeneratZipAndNotify(dataExportCompletedIntegrationEvent);
                    

                   _logger.LogInformation($"Handled export complete for user id: {dataExportCompletedIntegrationEvent.UserId} command id : {dataExportCompletedIntegrationEvent.CommandId} serviceName {dataExportCompletedIntegrationEvent.ServiceName}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        /// <summary>
        /// Generates the Zip , Notify and Change Status
        /// </summary>
        /// <param name="dataExportCompletedIntegrationEvent"></param>
        /// <returns></returns>
        private async Task GeneratZipAndNotify(BaseDataOperationCompletedIntegrationEvent dataExportCompletedIntegrationEvent)
        {
            common.dto.User.User user = await _userRepository.GetUserByPhoneNumber(dataExportCompletedIntegrationEvent.UserPhoneNumber);

            UserCommand dataExportCommand = await _userRepository.GetCommand(dataExportCompletedIntegrationEvent.UserPhoneNumber,
                                                                           dataExportCompletedIntegrationEvent.CommandId);

            if (dataExportCommand.DataExportRequestStatus == DataExportRequestStatus.Exported)
            {
                //zip the files
                string generatedFilePath = await GenerateZipFile(dataExportCompletedIntegrationEvent, dataExportCommand);

                //Get SAS URL
                string containerName = dataExportCompletedIntegrationEvent.UserId.ToString().Trim().ToLower();

                BlobClient blobClient = _userDataBlobServiceClient.GetBlobContainerClient(containerName).GetBlobClient(generatedFilePath);

                DateTimeOffset sasExpiry = DateTimeOffset.UtcNow.AddMinutes(_appSettings.ExportDataSASExpiryInMts);

                string sasURL = StorageUtilities.GetServiceSasUriForBlob(blobClient,
                                                         ApplicationConstants.StorageContainerPolicyNames.UserDataReadOnly,
                                                         sasExpiry);

                //Send Notification.
                bool isSuccess = await SendNotification(user,dataExportCommand);

                //set the right properties
                DataExportResult exportResult = new DataExportResult() {    ExportedDataUrl = sasURL, 
                                                                            ExpiresOn = sasExpiry.DateTime ,
                                                                            NotificationSent = isSuccess};

                user.DataExportedBy = new DataExportedBy() {    CommandId = dataExportCommand.Id, 
                                                                DataExportResult = exportResult };

                dataExportCommand.DataExportResult = exportResult;

                //set the status to complete
                await UpdateStatusToComplete(user, dataExportCommand);
            }
            
        }

        /// <summary>
        /// Update the command and user status
        /// </summary>
        /// <param name="userPhoneNumber"></param>
        /// <param name="commandId"></param>
        /// <returns></returns>
        private async Task UpdateStatus(BaseDataOperationCompletedIntegrationEvent dataExportCompletedIntegrationEvent)
        {
            common.dto.User.User user = await _userRepository.GetUserByPhoneNumber(dataExportCompletedIntegrationEvent.UserPhoneNumber);

            UserCommand dataExportCommand = await _userRepository.GetCommand(dataExportCompletedIntegrationEvent.UserPhoneNumber,
                                                                            dataExportCompletedIntegrationEvent.CommandId);

            //add the existing service completion
            StatusByEachServiceDetails dataExportByEachServiceDetail = new StatusByEachServiceDetails()
                                                                                {
                                                                                    CompletedByService = dataExportCompletedIntegrationEvent.ServiceName, 
                                                                                    CompletionDateTime = dataExportCompletedIntegrationEvent.CompletionDateTime ,
                                                                                    NoDataToOperate = dataExportCompletedIntegrationEvent.NoDataToOperate
                                                                                };

            dataExportCommand.StatusByEachServiceDetails.Add(dataExportByEachServiceDetail);

            DateTime currentTime = DateTime.UtcNow;

            //check if notification is recieved from all services
            if (dataExportCommand.IsCommandComplete())
            {
                dataExportCommand.DataExportRequestStatus = DataExportRequestStatus.Exported;

                user.DataExportRequestStatus = DataExportRequestStatus.Exported;

                CommandExecutionDetails executionDetails = new CommandExecutionDetails()
                {
                    EventName = DataExportRequestStatus.Exported.ToString(),
                    EventDateTime = currentTime
                };

                dataExportCommand.ExecutionDetails.Add(executionDetails);
            }

            dataExportCommand.ModifiedDate = currentTime;

            user.ModifiedDate = currentTime;

            await _userRepository.UpdateCommandBatch(dataExportCommand, user, true);

        }

        /// <summary>
        /// Responsible for Generating the Zip File
        /// </summary>
        /// <param name="dataExportCompletedIntegrationEvent"></param>
        /// <param name="dataExportCommand"></param>
        /// <returns></returns>
        private async Task<string> GenerateZipFile(BaseDataOperationCompletedIntegrationEvent dataExportCompletedIntegrationEvent,
                                                     UserCommand dataExportCommand)
        {
            long contentLength;

            string containerName = dataExportCompletedIntegrationEvent.UserId.ToString().Trim().ToLower();

            string zipFileName = $"{dataExportCompletedIntegrationEvent.CommandId}.zip";

            string zipFilePath = $"{dataExportCompletedIntegrationEvent.CommandId}/{dataExportCompletedIntegrationEvent.CommandId}.zip";

            BlobContainerClient userDataContainer = _userDataBlobServiceClient.GetBlobContainerClient(containerName);

            Stopwatch stopwatch = Stopwatch.StartNew();

            // Create an output stream. Does not have to be disk, could be MemoryStream etc.
            using (var ms = new MemoryStream())
            {
                using (ZipOutputStream zipOutputStream = new ZipOutputStream(ms))
                {
                    zipOutputStream.IsStreamOwner = false;

                    List<StatusByEachServiceDetails> serviceDetails = dataExportCommand.StatusByEachServiceDetails;

                    foreach (StatusByEachServiceDetails dataExportDetail in serviceDetails)
                    {
                        if (!dataExportDetail.NoDataToOperate)
                        {
                            ZipEntry entry = new ZipEntry($"{dataExportDetail.CompletedByService}.json");

                            zipOutputStream.PutNextEntry(entry);

                            string filePathToDownload = $"{dataExportCompletedIntegrationEvent.CommandId}/{dataExportDetail.CompletedByService}.json";

                            BlockBlobClient blockBlobClient = userDataContainer.GetBlockBlobClient(filePathToDownload);

                            await blockBlobClient.DownloadToAsync(zipOutputStream);

                            await zipOutputStream.FlushAsync();

                            await ms.FlushAsync();

                            zipOutputStream.CloseEntry();

                        }
                    }

                    zipOutputStream.Close();
                }

                contentLength = ms.Position;

                ms.Position = 0;

                BlobClient blobClient  = userDataContainer.GetBlobClient(zipFilePath);

                await blobClient.UploadAsync(ms, true);
                
            }

            stopwatch.Stop();

            _logger.LogInformation($"Generated Zip File {zipFileName}. Duration Milisecond : {stopwatch.ElapsedMilliseconds} Size {contentLength}");

            return zipFilePath;
        }

        /// <summary>
        /// Updates the user and command status to complete
        /// </summary>
        /// <param name="user"></param>
        /// <param name="dataExportCommand"></param>
        /// <returns></returns>
        private async Task UpdateStatusToComplete(  common.dto.User.User user, 
                                                    UserCommand dataExportCommand)
        {
            DateTime currentTime = DateTime.UtcNow;

            dataExportCommand.DataExportRequestStatus = DataExportRequestStatus.ExportedDataNotified;

            user.DataExportRequestStatus = DataExportRequestStatus.ExportedDataNotified;

            CommandExecutionDetails executionDetails = new CommandExecutionDetails()
            {
                EventName = DataExportRequestStatus.ExportedDataNotified.ToString(),
                EventDateTime = currentTime
            };

            dataExportCommand.ExecutionDetails.Add(executionDetails);

            dataExportCommand.ModifiedDate = currentTime;

            user.ModifiedDate = currentTime;

            await _userRepository.UpdateCommandBatch(dataExportCommand, user);

        }

        /// <summary>
        /// Send Push Notification
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<bool> SendNotification(common.dto.User.User user, UserCommand dataExportCommand)
        {
            // send notification
            NotificationRequest notificationRequest = new NotificationRequest()
            {
                Title = "Download data",
                Body = "Your data is ready to download",
                Type = PushNotificationType.UserDataExportComplete,
                UserData = new List<UserData>()
                        {
                            new UserData()
                            {
                                PhoneNumber = user.PhoneNumber,
                                UserId = user.UserId,
                            }
                        },
            };

            try
            {
                await _notificationProxy.SendNotification(notificationRequest);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send notification for Data Export Complete for user id {user.UserId} command id {dataExportCommand.Id}");

                return false;
            }
        }

        /// <summary>
        /// Polly Retry Policy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private AsyncRetryPolicy GetRetryPolicy<T>() where T : System.Exception
        {
            var builder = Policy
            .Handle<T>()
            .WaitAndRetryAsync(5,
              retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
              onRetry: (exception, delay, retryCount, context) =>
              {
                  _logger.LogError(exception, $"Attempt : {retryCount} - Exception Message : {exception.Message} - ");
              }
            );

            return builder;
        }
    }
}
