using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Azure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using blendnet.incentive.repository.Interfaces;
using blendnet.common.dto.Incentive;
using Azure.Storage.Blobs;
using blendnet.common.dto;
using Microsoft.ApplicationInsights.DataContracts;
using blendnet.common.infrastructure.Storage;
using System.Text.Json;
using blendnet.common.dto.Extensions;

namespace blendnet.incentive.listener.IntegrationEventHandling
{
    /// <summary>
    /// Event handler to export User Incentive Data
    /// </summary>
    public class ExportUserDataIntegrationEventHandler : IIntegrationEventHandler<ExportUserDataIntegrationEvent>
    {
        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        private readonly IncentiveAppSettings _appSettings;

        BlobServiceClient _userDataBlobServiceClient;

        IEventRepository _eventsRepository;

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
                                                     IOptionsMonitor<IncentiveAppSettings> optionsMonitor,
                                                     IEventRepository eventsRepository,
                                                     IEventBus eventBus)
        {
            _logger = logger;

            _telemetryClient = tc;

            _userDataBlobServiceClient = blobClientFactory.CreateClient(ApplicationConstants.StorageInstanceNames.UserDataStorage);

            _appSettings = optionsMonitor.CurrentValue;

            _eventsRepository = eventsRepository;

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
                using (_telemetryClient.StartOperation<RequestTelemetry>("ExportUserDataIntegrationEventHandler.Incentive.Handle"))
                {
                    if (integrationEvent.CommandId == Guid.Empty ||
                        integrationEvent.UserId == Guid.Empty ||
                        string.IsNullOrEmpty(integrationEvent.UserPhoneNumber))
                    {
                        _logger.LogWarning($"No user details found in integration event. Pass correct data to integation event");

                        return;
                    }

                    _logger.LogInformation($"Message Recieved for user id: {integrationEvent.UserId} command id: {integrationEvent.CommandId}");

                    List<IncentiveEventToExport> userIncentiveEvents = await _eventsRepository.GetUserEvents(integrationEvent.UserPhoneNumber);

                    //in  case no data found, raise the completion event notifying no data found to export
                    if (userIncentiveEvents == null || userIncentiveEvents.Count <= 0)
                    {
                        _logger.LogInformation($"No incentive event details found in database for user id: {integrationEvent.UserId}. command id: {integrationEvent.CommandId}");

                        await RaiseCompletionEvent(integrationEvent, true);

                        return;
                    }

                    //upload the data to blob
                    await UploadUserDataToBlob(integrationEvent, userIncentiveEvents);

                    //raise service bus completion event so that request status could be updated by calling listener
                    await RaiseCompletionEvent(integrationEvent, false);

                    _logger.LogInformation($"Incentive event export is complete for user id: {integrationEvent.UserId} command id : {integrationEvent.CommandId}.");

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        /// <summary>
        /// Uploads User Incentive data to blob
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        private async Task UploadUserDataToBlob(    ExportUserDataIntegrationEvent integrationEvent, 
                                                    List<IncentiveEventToExport> userIncentiveEvents)
        {
            //user id will be container name
            var containerName = integrationEvent.UserId.ToString().Trim().ToLower();

            //file name for the content
            string blobUploadPath = $"{integrationEvent.CommandId}/{ApplicationConstants.BlendNetServices.IncentiveService}.json";

            //serialize the data as json
            string dataJson = JsonSerializer.Serialize(userIncentiveEvents, Utilties.GetJsonSerializerOptions(ignoreNull: true, prettyPrint: true));

            await StorageUtilities.UploadUserDataToBlob(_userDataBlobServiceClient, containerName, blobUploadPath, dataJson,_logger);
           
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
            IncentiveDataExportCompletedIntegrationEvent incentiveDataExportCompletedIntegrationEvent =
                                    new IncentiveDataExportCompletedIntegrationEvent()
                                    {
                                        UserId = integrationEvent.UserId,
                                        UserPhoneNumber = integrationEvent.UserPhoneNumber,
                                        CommandId = integrationEvent.CommandId,
                                        CompletionDateTime = DateTime.UtcNow,
                                        NoDataToExport = noDataToExport
                                    };

            await _eventBus.Publish(incentiveDataExportCompletedIntegrationEvent);

        }
    }
}
