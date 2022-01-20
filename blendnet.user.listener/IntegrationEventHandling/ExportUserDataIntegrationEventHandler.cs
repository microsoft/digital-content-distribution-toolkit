using Azure.Storage.Blobs;
using blendnet.common.dto;
using blendnet.common.dto.Events;
using blendnet.common.dto.Extensions;
using blendnet.common.dto.User;
using blendnet.common.infrastructure;
using blendnet.common.infrastructure.Storage;
using blendnet.user.repository.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace blendnet.user.listener.IntegrationEventHandling
{
    /// <summary>
    /// Responsible for exporting user data
    /// </summary>
    internal class ExportUserDataIntegrationEventHandler : IIntegrationEventHandler<ExportUserDataIntegrationEvent>
    {
        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        private readonly UserAppSettings _appSettings;

        BlobServiceClient _userDataBlobServiceClient;

        IUserRepository _userRepository;

        private IEventBus _eventBus;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="tc"></param>
        /// <param name="blobClientFactory"></param>
        /// <param name="optionsMonitor"></param>
        /// <param name="eventsRepository"></param>
        /// <param name="eventBus"></param>
        public ExportUserDataIntegrationEventHandler(ILogger<ExportUserDataIntegrationEventHandler> logger,
                                                     TelemetryClient tc,
                                                     IAzureClientFactory<BlobServiceClient> blobClientFactory,
                                                     IOptionsMonitor<UserAppSettings> optionsMonitor,
                                                     IUserRepository userRepository,
                                                     IEventBus eventBus)
        {
            _logger = logger;

            _telemetryClient = tc;

            _userDataBlobServiceClient = blobClientFactory.CreateClient(ApplicationConstants.StorageInstanceNames.UserDataStorage);

            _appSettings = optionsMonitor.CurrentValue;

            _userRepository = userRepository;

            _eventBus = eventBus;

        }

        /// <summary>
        /// Handler to export the data to a blob storage
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(ExportUserDataIntegrationEvent integrationEvent)
        {
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>("ExportUserDataIntegrationEventHandler.User.Handle"))
                {
                    if (integrationEvent.CommandId == Guid.Empty ||
                        integrationEvent.UserId == Guid.Empty ||
                        string.IsNullOrEmpty(integrationEvent.UserPhoneNumber))
                    {
                        _logger.LogWarning($"No user details found in integration event. Pass correct data to integation event");

                        return;
                    }

                    _logger.LogInformation($"Message Recieved for user id: {integrationEvent.UserId} command id: {integrationEvent.CommandId}");

                    User user = await _userRepository.GetUserByPhoneNumber(integrationEvent.UserPhoneNumber);

                    //in  case no data found, raise the completion event notifying no data found to export
                    //although this should never happen for user
                    if (user == null)
                    {
                        _logger.LogInformation($"No user details found in database for user id: {integrationEvent.UserId}. command id: {integrationEvent.CommandId}");

                        await RaiseCompletionEvent(integrationEvent, true);

                        return;
                    }

                    UserToExport userToExport = new UserToExport() { PhoneNumber = user.PhoneNumber };

                    if (user.ReferralInfo != null)
                    {
                        userToExport.ReferralDateTime = user.ReferralInfo.ReferralDateTime;
                        userToExport.ReferredBy = user.ReferralInfo.RetailerPartnerId;
                        userToExport.RetailerReferralCode = user.ReferralInfo.RetailerReferralCode;
                    }

                    //upload the data to blob
                    await UploadUserDataToBlob(integrationEvent, userToExport);

                    //raise service bus completion event so that request status could be updated by calling listener
                    await RaiseCompletionEvent(integrationEvent, false);

                    _logger.LogInformation($"User export is complete for user id: {integrationEvent.UserId} command id : {integrationEvent.CommandId}.");

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        /// <summary>
        /// Uploads User Order data to blob
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        private async Task UploadUserDataToBlob(ExportUserDataIntegrationEvent integrationEvent,
                                                    UserToExport userToExport)
        {
            //user id will be container name
            var containerName = integrationEvent.UserId.ToString().Trim().ToLower();

            //file name for the content
            string blobUploadPath = $"{integrationEvent.CommandId}/{ApplicationConstants.BlendNetServices.UserService}.json";

            //serialize the data as json
            string dataJson = JsonSerializer.Serialize(userToExport, Utilties.GetJsonSerializerOptions(ignoreNull: true));

            await StorageUtilities.UploadUserDataToBlob(_userDataBlobServiceClient, containerName, blobUploadPath, dataJson, _logger);

        }

        /// <summary>
        /// Raise Completion Event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <param name="noDataToExport"></param>
        /// <returns></returns>
        private async Task RaiseCompletionEvent(ExportUserDataIntegrationEvent integrationEvent, bool noDataToExport)
        {
            //publish the complted event
            UserDataExportCompletedIntegrationEvent userDataExportCompletedIntegrationEvent =
                                    new UserDataExportCompletedIntegrationEvent()
                                    {
                                        UserId = integrationEvent.UserId,
                                        UserPhoneNumber = integrationEvent.UserPhoneNumber,
                                        CommandId = integrationEvent.CommandId,
                                        CompletionDateTime = DateTime.UtcNow,
                                        NoDataToExport = noDataToExport
                                    };

            await _eventBus.Publish(userDataExportCompletedIntegrationEvent);

        }
    }
}
