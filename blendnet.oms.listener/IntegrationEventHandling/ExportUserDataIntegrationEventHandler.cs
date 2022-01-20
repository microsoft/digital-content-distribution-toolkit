using Azure.Storage.Blobs;
using blendnet.common.dto;
using blendnet.common.dto.Events;
using blendnet.common.dto.Extensions;
using blendnet.common.dto.Oms;
using blendnet.common.infrastructure;
using blendnet.common.infrastructure.Storage;
using blendnet.oms.repository.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace blendnet.oms.listener.IntegrationEventHandling
{
    /// <summary>
    /// Export User Data Integration EventHandler
    /// </summary>
    public class ExportUserDataIntegrationEventHandler : IIntegrationEventHandler<ExportUserDataIntegrationEvent>
    {
        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        private readonly OmsAppSettings _appSettings;

        BlobServiceClient _userDataBlobServiceClient;

        IOMSRepository _omsRepository;

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
                                                     IOptionsMonitor<OmsAppSettings> optionsMonitor,
                                                     IOMSRepository omsRepository,
                                                     IEventBus eventBus)
        {
            _logger = logger;

            _telemetryClient = tc;

            _userDataBlobServiceClient = blobClientFactory.CreateClient(ApplicationConstants.StorageInstanceNames.UserDataStorage);

            _appSettings = optionsMonitor.CurrentValue;

            _omsRepository = omsRepository;

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
                using (_telemetryClient.StartOperation<RequestTelemetry>("ExportUserDataIntegrationEventHandler.OMS.Handle"))
                {
                    if (integrationEvent.CommandId == Guid.Empty ||
                        integrationEvent.UserId == Guid.Empty ||
                        string.IsNullOrEmpty(integrationEvent.UserPhoneNumber))
                    {
                        _logger.LogWarning($"No user details found in integration event. Pass correct data to integation event");

                        return;
                    }

                    _logger.LogInformation($"Message Recieved for user id: {integrationEvent.UserId} command id: {integrationEvent.CommandId}");

                    List<OrderToExport> userOrders = await _omsRepository.GetUserOrders(integrationEvent.UserPhoneNumber);

                    //in  case no data found, raise the completion event notifying no data found to export
                    if (userOrders == null || userOrders.Count <= 0)
                    {
                        _logger.LogInformation($"No order details found in database for user id: {integrationEvent.UserId}. command id: {integrationEvent.CommandId}");

                        await RaiseCompletionEvent(integrationEvent, true);

                        return;
                    }

                    //upload the data to blob
                    await UploadUserDataToBlob(integrationEvent, userOrders);

                    //raise service bus completion event so that request status could be updated by calling listener
                    await RaiseCompletionEvent(integrationEvent, false);

                    _logger.LogInformation($"OMS event export is complete for user id: {integrationEvent.UserId} command id : {integrationEvent.CommandId}.");

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
                                                    List<OrderToExport> userOrders)
        {
            //user id will be container name
            var containerName = integrationEvent.UserId.ToString().Trim().ToLower();

            //file name for the content
            string blobUploadPath = $"{integrationEvent.CommandId}/{ApplicationConstants.BlendNetServices.OMSService}.json";

            //serialize the data as json
            string dataJson = JsonSerializer.Serialize(userOrders, Utilties.GetJsonSerializerOptions(ignoreNull:true));

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
            OrderDataExportCompletedIntegrationEvent orderDataExportCompletedIntegrationEvent =
                                    new OrderDataExportCompletedIntegrationEvent()
                                    {
                                        UserId = integrationEvent.UserId,
                                        UserPhoneNumber = integrationEvent.UserPhoneNumber,
                                        CommandId = integrationEvent.CommandId,
                                        CompletionDateTime = DateTime.UtcNow,
                                        NoDataToExport = noDataToExport
                                    };

            await _eventBus.Publish(orderDataExportCompletedIntegrationEvent);

        }
    }
}
