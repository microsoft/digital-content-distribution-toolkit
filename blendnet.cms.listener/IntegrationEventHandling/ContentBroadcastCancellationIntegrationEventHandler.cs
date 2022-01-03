using blendnet.api.proxy.Ses;
using blendnet.cms.listener.Model;
using blendnet.cms.repository.Interfaces;
using blendnet.common.dto.cms;
using blendnet.common.dto.Cms;
using blendnet.common.dto.Events;
using blendnet.common.dto.Ses;
using blendnet.common.infrastructure;
using blendnet.common.infrastructure.Extensions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace blendnet.cms.listener.IntegrationEventHandling
{
    /// <summary>
    /// Responsible for handling content broadcast cancellation events
    /// Invokes the SES API to login, cancel and logout
    /// </summary>
    public class ContentBroadcastCancellationIntegrationEventHandler : IIntegrationEventHandler<ContentBroadcastCancellationIntegrationEvent>
    {
        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        private readonly AppSettings _appSettings;

        IContentRepository _contentRepository;

        private VODEProxy _vodeProxy;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="tc"></param>
        /// <param name="optionsMonitor"></param>
        /// <param name="contentRepository"></param>
        public ContentBroadcastCancellationIntegrationEventHandler(ILogger<ContentBroadcastCancellationIntegrationEventHandler> logger,
                                                       TelemetryClient tc,
                                                       IOptionsMonitor<AppSettings> optionsMonitor,
                                                       IContentRepository contentRepository,
                                                       VODEProxy vodeProxy)
        {
            _logger = logger;

            _telemetryClient = tc;

            _appSettings = optionsMonitor.CurrentValue;

            _contentRepository = contentRepository;

            _vodeProxy = vodeProxy;
        }

        /// <summary>
        /// Handles the cancellation
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(ContentBroadcastCancellationIntegrationEvent integrationEvent)
        {
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>("ContentBroadcastCancellationIntegrationEventHandler.Handle"))
                {
                    if (integrationEvent.ContentBroadcastCancellationCommand == null ||
                        integrationEvent.ContentBroadcastCancellationCommand.ContentId == Guid.Empty)
                    {
                        _logger.LogInformation($"No content details or Broadcast Cancellation details found in integration event. Pass correct data to integation event");

                        return;
                    }

                    _logger.LogInformation($"Message Recieved for broadcast cancellation for content id: {integrationEvent.ContentBroadcastCancellationCommand.ContentId}");

                    ContentCommand broadcastCancellationCommand = integrationEvent.ContentBroadcastCancellationCommand;

                    Content content = await _contentRepository.GetContentById(broadcastCancellationCommand.ContentId);

                    if (content == null)
                    {
                        _logger.LogInformation($"No content details found in database for content id: {integrationEvent.ContentBroadcastCancellationCommand.ContentId.ToString()}");

                        return;
                    }

                    DateTime currentTime = DateTime.UtcNow;

                    PopulateContentCommand(broadcastCancellationCommand, currentTime);

                    _logger.LogInformation($"Cancelling Broadcasting for content id: {integrationEvent.ContentBroadcastCancellationCommand.ContentId} Command Id {broadcastCancellationCommand.Id.Value}");

                    //Create a command record with in progress status. It will use the command id as ID and Content Id and partition key
                    Guid commandId = await _contentRepository.CreateContentCommand(broadcastCancellationCommand);

                    content.ContentBroadcastStatus = ContentBroadcastStatus.BroadcastCancelInProgress;

                    content.ModifiedDate = currentTime;

                    content.ContentBroadcastStatusUpdatedBy = commandId;

                    await _contentRepository.UpdateContent(content);

                    //Perform the content transformation
                    await PerformBroadcastCancellation(content, broadcastCancellationCommand);

                    //Update the command status. In case of any error, mark it to failure state.
                    if (broadcastCancellationCommand.FailureDetails.Count > 0)
                    {
                        await UpdateFailedStatus(content, broadcastCancellationCommand);
                    }
                    else
                    {
                        await UpdateSucessStatus(content, broadcastCancellationCommand);

                        //populate completed AI event
                        CancelBroadcastAIEvent cancelBroadcastAIEvent = new CancelBroadcastAIEvent()
                        {
                            Title = content.Title,
                            ShortDescription = content.ShortDescription,
                            ContentId = content.Id.Value,
                            ContentProviderId = content.ContentProviderId,
                            CommandId = broadcastCancellationCommand.Id.Value,
                            CancelationDate = content.ModifiedDate.Value
                        };

                        _telemetryClient.TrackEvent(cancelBroadcastAIEvent);

                    }

                    _logger.LogInformation($"Broadcast Cancellation for content id: {integrationEvent.ContentBroadcastCancellationCommand.ContentId} command id : {broadcastCancellationCommand.Id.Value}");

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        /// <summary>
        /// Populate command object with required attributes
        /// </summary>
        /// <param name="contentCommand"></param>
        private void PopulateContentCommand(ContentCommand contentCommand, DateTime currentDateTime)
        {
            contentCommand.Id = Guid.NewGuid();
            contentCommand.CommandType = CommandType.CancelBroadcastContent;
            contentCommand.Type = ContentContainerType.Command;
            contentCommand.CommandStatus = CommandStatus.InProgress;
            contentCommand.CreatedDate = currentDateTime;
            contentCommand.ModifiedDate = currentDateTime;
            contentCommand.FailureDetails = new List<string>();

            CommandExecutionDetails commandExecutionDetails = new CommandExecutionDetails();

            commandExecutionDetails.ContentBroadcastStatus = ContentBroadcastStatus.BroadcastCancelInProgress;

            commandExecutionDetails.EventDateTime = currentDateTime;

            contentCommand.ExecutionDetails.Add(commandExecutionDetails);
        }

        /// <summary>
        /// Performs the cancellation for the broadcasted content
        /// </summary>
        /// <param name="content"></param>
        /// <param name="broadcastCancelCommand"></param>
        /// <returns></returns>
        private async Task PerformBroadcastCancellation(Content content, ContentCommand broadcastCancelCommand)
        {
            string errorMessage = string.Empty;

            try
            {
                BroadcastCancellationResult result = await _vodeProxy.CancelBroadcast(content.ContentBroadcastedBy.CommandId);

                string resultString = System.Text.Json.JsonSerializer.Serialize(result);

                _logger.LogInformation($"Successfully invoked the cancellation end point for Content {content.ContentId.Value} and Command {content.ContentBroadcastedBy.CommandId}. Response recieved : {resultString}");

            }
            catch (Exception ex)
            {
                errorMessage = $"Failed to cancel broadcast for content {content.ContentId.Value} Command {broadcastCancelCommand.Id.Value} Exception {ex.Message}";

                broadcastCancelCommand.FailureDetails.Add(errorMessage);

                _logger.LogError(ex, $"{errorMessage}");
            }
        }

        /// <summary>
        /// Updates the command and content status to failed
        /// </summary>
        /// <param name="content"></param>
        /// <param name="broadcastCancellationCommand"></param>
        /// <returns></returns>
        private async Task UpdateFailedStatus(Content content, ContentCommand broadcastCancellationCommand)
        {
            content.ContentBroadcastStatus = ContentBroadcastStatus.BroadcastCancelFailed;

            broadcastCancellationCommand.CommandStatus = CommandStatus.Failed;

            await UpdateStatus(content, broadcastCancellationCommand);
        }

        /// <summary>
        /// Update the command and content status to success
        /// </summary>
        /// <param name="content"></param>
        /// <param name="broadcastCancellationCommand"></param>
        /// <returns></returns>
        private async Task UpdateSucessStatus(Content content, ContentCommand broadcastCancellationCommand)
        {
            content.ContentBroadcastStatus = ContentBroadcastStatus.BroadcastCancelComplete;

            broadcastCancellationCommand.CommandStatus = CommandStatus.Complete;

            await UpdateStatus(content, broadcastCancellationCommand);
        }

        /// <summary>
        /// Updates the given command and content status
        /// </summary>
        /// <param name="content"></param>
        /// <param name="broadcastCommand"></param>
        /// <returns></returns>
        private async Task UpdateStatus(Content content, ContentCommand broadcastCommand)
        {
            DateTime currentTime = DateTime.UtcNow;

            content.ModifiedDate = currentTime;

            content.ContentBroadcastStatusUpdatedBy = broadcastCommand.Id;

            broadcastCommand.ModifiedDate = currentTime;

            CommandExecutionDetails commandExecutionDetails = new CommandExecutionDetails();

            commandExecutionDetails.ContentBroadcastStatus = content.ContentBroadcastStatus.Value;

            commandExecutionDetails.EventDateTime = currentTime;

            broadcastCommand.ExecutionDetails.Add(commandExecutionDetails);

            await _contentRepository.UpdateContent(content);

            await _contentRepository.UpdateContentCommand(broadcastCommand);
        }
    }
}
