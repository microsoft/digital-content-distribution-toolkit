using blendnet.cms.listener.Model;
using blendnet.cms.repository.Interfaces;
using blendnet.common.dto;
using blendnet.common.dto.cms;
using blendnet.common.dto.Cms;
using blendnet.common.dto.Integration;
using blendnet.common.infrastructure.Extensions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.listener.Common
{
    /// <summary>
    /// To handle all the SES events to update the broadcast command status.
    /// </summary>
    public class MomOrderIntegrationEventHandler
    {
        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        private readonly AppSettings _appSettings;

        IContentRepository _contentRepository;

        /// <summary>
        /// MOM Order Integration Event Handler
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="tc"></param>
        /// <param name="optionsMonitor"></param>
        /// <param name="contentRepository"></param>
        public MomOrderIntegrationEventHandler(ILogger<MomOrderIntegrationEventHandler> logger,
                                                      TelemetryClient tc,
                                                      IOptionsMonitor<AppSettings> optionsMonitor,
                                                      IContentRepository contentRepository)
        {
            _logger = logger;

            _telemetryClient = tc;

            _appSettings = optionsMonitor.CurrentValue;

            _contentRepository = contentRepository;
        }

        /// <summary>
        /// Responsible of submitting job to AMS
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>

        public async Task Handle(IntegrationEvent integrationEvent, 
                                string operationName,
                                string eventName,
                                ContentBroadcastStatus contentBroadcastStatus)
        {
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>(operationName))
                {
                    if (string.IsNullOrEmpty(integrationEvent.CorrelationId))
                    {
                        _logger.LogInformation($"No correlation details found in integration event. Pass correct data to integation event. Event Body : {integrationEvent.Body}");

                        return;
                    }

                    Guid commandId;

                    if (!Guid.TryParse(integrationEvent.CorrelationId, out commandId))
                    {
                        _logger.LogInformation($"Invalid correlation id recieved. This is not meant for our application as it has to be a valid guid. Event Correlation Id {integrationEvent.CorrelationId} Event Body : {integrationEvent.Body}");

                        return;
                    }

                    _logger.LogInformation($"Broadcast related message recieved for command id: {integrationEvent.CorrelationId} Message Body : {integrationEvent.Body}");

                    ContentCommand broadcastCommand = await _contentRepository.GetContentCommandById(commandId);

                    Content content = await _contentRepository.GetContentById(broadcastCommand.ContentId);

                    if (content == null || broadcastCommand == null)
                    {
                        _logger.LogInformation($"No content or command details found in database for content id: {broadcastCommand.ContentId} command id {commandId}");

                        return;
                    }

                    await UpdateStatus(content, broadcastCommand, contentBroadcastStatus, eventName);

                    _logger.LogInformation($"Completed {operationName} - Event {eventName} Handling for content id: {broadcastCommand.ContentId} command id {commandId}");

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }



        
        /// <summary>
        /// Update Status
        /// </summary>
        /// <param name="content"></param>
        /// <param name="transformCommand"></param>
        /// <returns></returns>
        private async Task UpdateStatus(Content content, 
                                        ContentCommand broadcastCommand,
                                        ContentBroadcastStatus contentBroadcastStatus,
                                        string eventName)
        {
            CompleteBroadcastAIEvent completeBroadcastAIEvent = null;

            content.ContentBroadcastStatus = contentBroadcastStatus;

            if (contentBroadcastStatus == ContentBroadcastStatus.BroadcastOrderCreated ||
                contentBroadcastStatus == ContentBroadcastStatus.BroadcastOrderActive)
            {
                broadcastCommand.CommandStatus = CommandStatus.InProgress;
            }
            else if (contentBroadcastStatus == ContentBroadcastStatus.BroadcastOrderComplete)
            {
                broadcastCommand.CommandStatus = CommandStatus.Complete;

                ContentBroadcastedBy contentBroadcastedBy = new ContentBroadcastedBy() {
                    CommandId = broadcastCommand.Id.Value,
                    BroadcastRequest = broadcastCommand.BroadcastRequest
                };

                //keep track of command id which broadcasted the content
                content.ContentBroadcastedBy = contentBroadcastedBy;

                //populate completed AI event
                completeBroadcastAIEvent = new CompleteBroadcastAIEvent()
                {
                    Title = content.Title, ShortDescription = content.ShortDescription, ContentId = content.Id.Value,
                    ContentProviderId = content.ContentProviderId,CommandId = content.ContentBroadcastedBy.CommandId, 
                    BroadcastStartDate = content.ContentBroadcastedBy.BroadcastRequest.StartDate, 
                    BroadcastEndDate = content.ContentBroadcastedBy.BroadcastRequest.EndDate,
                    Filters = string.Join(",", content.ContentBroadcastedBy.BroadcastRequest.Filters)
                };
            }
            else if (contentBroadcastStatus == ContentBroadcastStatus.BroadcastOrderCancelled ||
                      contentBroadcastStatus == ContentBroadcastStatus.BroadcastOrderFailed ||
                      contentBroadcastStatus == ContentBroadcastStatus.BroadcastOrderRejected)
            {
                broadcastCommand.CommandStatus = CommandStatus.Failed;
            }
            else
            {
                _logger.LogInformation($"Invalid {contentBroadcastStatus} - Event {eventName} for content id: {broadcastCommand.ContentId} command id {broadcastCommand.Id.Value}");
            }

            DateTime currentTime = DateTime.UtcNow;

            content.ModifiedDate = currentTime;

            content.ContentBroadcastStatusUpdatedBy = broadcastCommand.Id;

            broadcastCommand.ModifiedDate = currentTime;

            CommandExecutionDetails executionDetails = new CommandExecutionDetails()
            {
                ContentBroadcastStatus = content.ContentBroadcastStatus.Value,
                EventName = eventName,
                EventDateTime = currentTime
            };

            broadcastCommand.ExecutionDetails.Add(executionDetails);

            await _contentRepository.UpdateContent(content);

            await _contentRepository.UpdateContentCommand(broadcastCommand);

            //Send telemetry event on broadcast complete
            if (completeBroadcastAIEvent != null)
            {
                _telemetryClient.TrackEvent(completeBroadcastAIEvent);
            }
        }
    }
}
