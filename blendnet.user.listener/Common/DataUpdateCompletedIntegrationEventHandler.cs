// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Events;
using blendnet.common.dto.Exceptions;
using blendnet.common.dto.User;
using blendnet.user.repository.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace blendnet.user.listener.Common
{
    /// <summary>
    /// Commmon data update completed event handler
    /// </summary>
    public class DataUpdateCompletedIntegrationEventHandler
    {
        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        private readonly UserAppSettings _appSettings;

        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="tc"></param>
        /// <param name="optionsMonitor"></param>
        /// <param name="userRepository"></param>
        public DataUpdateCompletedIntegrationEventHandler(ILogger<DataExportCompletedIntegrationEventHandler> logger,
                                                            TelemetryClient tc,
                                                            IOptionsMonitor<UserAppSettings> optionsMonitor,
                                                            IUserRepository userRepository)
        {
            _logger = logger;

            _telemetryClient = tc;

            _appSettings = optionsMonitor.CurrentValue;

            _userRepository = userRepository;

        }

        /// <summary>
        /// Handle the data update complete for all services.
        /// Step 1 : Once the data update is complete from all 3 service, change the status to Update complete
        ///        : In case one of the process has failed, mark the status as failed
        /// </summary>
        /// <param name="dataUpdateCompletedIntegrationEvent"></param>
        /// <param name="serviceName"></param>
        /// <param name="operationName"></param>
        /// <returns></returns>

        public async Task Handle(BaseDataOperationCompletedIntegrationEvent dataUpdateCompletedIntegrationEvent,string operationName)
        {
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>(operationName))
                {
                    if (dataUpdateCompletedIntegrationEvent.CommandId == Guid.Empty ||
                        dataUpdateCompletedIntegrationEvent.UserId == Guid.Empty ||
                        string.IsNullOrEmpty(dataUpdateCompletedIntegrationEvent.UserPhoneNumber) ||
                        string.IsNullOrEmpty(dataUpdateCompletedIntegrationEvent.ServiceName))
                    {
                        _logger.LogWarning($"No user details found in data update integration event. Pass correct data to integation event");

                        return;
                    }

                    _logger.LogInformation($"Message Recieved for user id: {dataUpdateCompletedIntegrationEvent.UserId} command id: {dataUpdateCompletedIntegrationEvent.CommandId}");

                    common.dto.User.User user = await _userRepository.GetUserByPhoneNumber(dataUpdateCompletedIntegrationEvent.UserPhoneNumber);

                    UserCommand dataExportCommand = await _userRepository.GetCommand(dataUpdateCompletedIntegrationEvent.UserPhoneNumber, dataUpdateCompletedIntegrationEvent.CommandId);

                    if (user == null || dataExportCommand == null)
                    {
                        _logger.LogWarning($"No user details or command details found in database. UserID {dataUpdateCompletedIntegrationEvent.UserId} CommandId {dataUpdateCompletedIntegrationEvent.CommandId}");

                        return;

                    }

                    //step 1 : Once the data update is complete from all 3 service, change the status to export complete
                    //Since multiple listeners are doing the export, high posibility that User or Command object is changed by other thread.

                    AsyncRetryPolicy policyBuilder = GetRetryPolicy<BlendNetCosmosTransactionalBatchException>();

                    await policyBuilder.ExecuteAsync(async () =>
                    {
                        await UpdateStatus(dataUpdateCompletedIntegrationEvent);
                    });

                    _logger.LogInformation($"Step 1 status updated for user id: {dataUpdateCompletedIntegrationEvent.UserId} command id : {dataUpdateCompletedIntegrationEvent.CommandId} serviceName {dataUpdateCompletedIntegrationEvent.ServiceName}.");

                    //step 2: UpdateUserData - if all are complete, perform the change of user phone number with user id

                    await UpdateUserData(dataUpdateCompletedIntegrationEvent);

                    _logger.LogInformation($"Handled update complete for user id: {dataUpdateCompletedIntegrationEvent.UserId} command id : {dataUpdateCompletedIntegrationEvent.CommandId} serviceName {dataUpdateCompletedIntegrationEvent.ServiceName}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        

        /// <summary>
        /// Update the command and user status
        /// </summary>
        /// <param name="userPhoneNumber"></param>
        /// <param name="commandId"></param>
        /// <returns></returns>
        private async Task UpdateStatus(BaseDataOperationCompletedIntegrationEvent dataUpdateCompletedIntegrationEvent)
        {
            common.dto.User.User user = await _userRepository.GetUserByPhoneNumber(dataUpdateCompletedIntegrationEvent.UserPhoneNumber);

            UserCommand dataExportCommand = await _userRepository.GetCommand(dataUpdateCompletedIntegrationEvent.UserPhoneNumber,
                                                                            dataUpdateCompletedIntegrationEvent.CommandId);

            //add the existing service completion
            StatusByEachServiceDetails dataExportByEachServiceDetail = new StatusByEachServiceDetails()
            {
                CompletedByService = dataUpdateCompletedIntegrationEvent.ServiceName,
                CompletionDateTime = dataUpdateCompletedIntegrationEvent.CompletionDateTime,
                NoDataToOperate = dataUpdateCompletedIntegrationEvent.NoDataToOperate,
                IsFailed = dataUpdateCompletedIntegrationEvent.IsFailed,
                FailureReason = dataUpdateCompletedIntegrationEvent.FailureReason
            };

            dataExportCommand.StatusByEachServiceDetails.Add(dataExportByEachServiceDetail);

            DateTime currentTime = DateTime.UtcNow;

            //check if notification is recieved from all services
            if (dataExportCommand.IsCommandComplete())
            {
                if (dataExportCommand.IsAnyCommandFailed())
                {
                    dataExportCommand.DataUpdateRequestStatus = DataUpdateRequestStatus.Failed;

                    user.DataUpdateRequestStatus = DataUpdateRequestStatus.Failed;
                }
                else
                {
                    dataExportCommand.DataUpdateRequestStatus = DataUpdateRequestStatus.Updated;

                    user.DataUpdateRequestStatus = DataUpdateRequestStatus.Updated;
                }

                CommandExecutionDetails executionDetails = new CommandExecutionDetails()
                {
                    EventName = DataUpdateRequestStatus.Updated.ToString(),
                    EventDateTime = currentTime
                };

                dataExportCommand.ExecutionDetails.Add(executionDetails);
            }

            dataExportCommand.ModifiedDate = currentTime;

            user.ModifiedDate = currentTime;

            await _userRepository.UpdateCommandBatch(dataExportCommand, user, true);

        }

        /// <summary>
        /// Update the user phone number with user id only if all child process are sucessfull
        /// </summary>
        /// <param name="dataUpdateCompletedIntegrationEvent"></param>
        /// <returns></returns>
        private async Task UpdateUserData(BaseDataOperationCompletedIntegrationEvent dataUpdateCompletedIntegrationEvent)
        {
            common.dto.User.User user = await _userRepository.GetUserByPhoneNumber(dataUpdateCompletedIntegrationEvent.UserPhoneNumber);

            UserCommand dataUpdateCommand = await _userRepository.GetCommand(   dataUpdateCompletedIntegrationEvent.UserPhoneNumber,
                                                                                dataUpdateCompletedIntegrationEvent.CommandId);

            //if all the updates are done. We can finally delete and insert User and Current Command
            if (dataUpdateCommand.DataUpdateRequestStatus == DataUpdateRequestStatus.Updated)
            {
                List<Guid> idsToDelete = new List<Guid> { user.UserId, dataUpdateCommand.Id };

                user.PhoneNumber = user.UserId.ToString();
                user.IsUserDeleted = true;
                
                dataUpdateCommand.PhoneNumber = user.UserId.ToString();
                dataUpdateCommand.IsUserDeleted = true;

                //insert the user and command with new partition key
                await _userRepository.InsertBatch(user.UserId.ToString(),user, dataUpdateCommand);

                //delete the existing user and user command record.
                await _userRepository.DeleteBatch(dataUpdateCompletedIntegrationEvent.UserPhoneNumber, idsToDelete);

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
