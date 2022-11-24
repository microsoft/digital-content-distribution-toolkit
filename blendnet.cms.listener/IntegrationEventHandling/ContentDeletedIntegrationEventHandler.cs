// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using blendnet.cms.repository.Interfaces;
using blendnet.common.dto;
using blendnet.common.dto.cms;
using blendnet.common.dto.Cms;
using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.listener.IntegrationEventHandling
{
    /// <summary>
    /// Responsible to handle content delete event.
    /// Delete the Media content from Mezzanine Container.
    /// Delete the Media content CDN Container.
    /// </summary>
    public class ContentDeletedIntegrationEventHandler : IIntegrationEventHandler<ContentDeletedIntegrationEvent>
    {
        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        private readonly AppSettings _appSettings;

        BlobServiceClient _cmsBlobServiceClient;

        BlobServiceClient _cmsCdnBlobServiceClient;

        IContentRepository _contentRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="tc"></param>
        /// <param name="blobClientFactory"></param>
        /// <param name="optionsMonitor"></param>
        public ContentDeletedIntegrationEventHandler(ILogger<ContentDeletedIntegrationEventHandler> logger,
                                                             TelemetryClient tc,
                                                             IAzureClientFactory<BlobServiceClient> blobClientFactory,
                                                             IOptionsMonitor<AppSettings> optionsMonitor,
                                                             IContentRepository contentRepository)
        {
            _logger = logger;

            _telemetryClient = tc;

            _cmsBlobServiceClient = blobClientFactory.CreateClient(ApplicationConstants.StorageInstanceNames.CMSStorage);

            _cmsCdnBlobServiceClient = blobClientFactory.CreateClient(ApplicationConstants.StorageInstanceNames.CMSCDNStorage);

            _appSettings = optionsMonitor.CurrentValue;

            _contentRepository = contentRepository;
        }


        /// <summary>
        /// Handle the content uploaded integration event handler
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(ContentDeletedIntegrationEvent integrationEvent)
        {
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>("ContentDeletedIntegrationEventHandler.Handle"))
                {
                    if (integrationEvent.ContentDeleteCommand != null && integrationEvent.ContentDeleteCommand.Content != null)
                    {
                        _logger.LogInformation($"Message Recieved for content id: {integrationEvent.ContentDeleteCommand.Content.ContentId.Value.ToString()}");

                        ContentCommand deleteCommand = integrationEvent.ContentDeleteCommand;

                        DateTime currentTime = DateTime.UtcNow;

                        PopulateContentCommand(deleteCommand.Content.ContentId.Value, deleteCommand, currentTime);

                        //Create a command record with in progress status. It will use the command id as ID and Content Id and partition key
                        Guid commandId = await _contentRepository.CreateContentCommand(deleteCommand);

                        _logger.LogInformation($"Deleting media file for content id: {integrationEvent.ContentDeleteCommand.Content.ContentId.Value}");

                        await DeleteMediaContentFromMezzanine(deleteCommand);

                        _logger.LogInformation($"Deleting attachment file(s) for content id: {integrationEvent.ContentDeleteCommand.Content.ContentId.Value}");

                        await DeleteAttachmentContentFromCdn(deleteCommand);

                        //Update the command status. In case of any error, mark it to failure state.
                        if (deleteCommand.FailureDetails.Count > 0)
                        {
                            deleteCommand.CommandStatus = CommandStatus.Failed;
                        }
                        else
                        {
                            deleteCommand.CommandStatus = CommandStatus.Complete;
                        }

                        currentTime = DateTime.UtcNow;

                        deleteCommand.ModifiedDate = currentTime;

                        await _contentRepository.UpdateContentCommand(deleteCommand);

                        _logger.LogInformation($"Message Process Completed for content id: {integrationEvent.ContentDeleteCommand.Content.ContentId.Value.ToString()}");
                    }
                    else
                    {
                        _logger.LogInformation($"No content details found in integration event. Pass correct data to integation event");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

        }

        /// <summary>
        /// Deletes the Media content from Mezzanine
        /// </summary>
        /// <param name="deleteCommand"></param>
        /// <returns></returns>
        private async Task DeleteMediaContentFromMezzanine(ContentCommand deleteCommand)
        {
            string errorMessage = string.Empty;

            try
            {
                var baseName = deleteCommand.Content.ContentProviderId.ToString().Trim().ToLower();

                string mezzContainerName = $"{baseName}{ApplicationConstants.StorageContainerSuffix.Mezzanine}";

                BlobContainerClient mezzContainer = this._cmsBlobServiceClient.GetBlobContainerClient(mezzContainerName);

                string blobNameToDelete = $"{deleteCommand.Content.ContentId.Value.ToString()}/{deleteCommand.Content.MediaFileName}";

                BlockBlobClient blobToDelete = mezzContainer.GetBlockBlobClient(blobNameToDelete);

                await blobToDelete.DeleteIfExistsAsync();

            }
            catch (Exception ex)
            {
                errorMessage = $"Failed to delete media for content {deleteCommand.Content.ContentId.Value.ToString()} Media File {deleteCommand.Content.MediaFileName}";

                deleteCommand.FailureDetails.Add(errorMessage);

                _logger.LogError(ex, $"{errorMessage}");
            }
        }


        /// <summary>
        /// Deletes the content from CDN folder
        /// </summary>
        /// <param name="deleteCommand"></param>
        /// <returns></returns>
        private async Task DeleteAttachmentContentFromCdn(ContentCommand deleteCommand)
        {
            string errorMessage = string.Empty;

            if (deleteCommand.Content.Attachments != null && deleteCommand.Content.Attachments.Count > 0)
            {
                var baseName = deleteCommand.Content.ContentProviderId.ToString().Trim().ToLower();

                string cdnContainerName = $"{baseName}{ApplicationConstants.StorageContainerSuffix.Cdn}";

                BlobContainerClient cdnContainer = this._cmsCdnBlobServiceClient.GetBlobContainerClient(cdnContainerName);

                foreach (Attachment attachment in deleteCommand.Content.Attachments)
                {
                    try
                    {
                        string blobToDeleteFileName = $"{deleteCommand.Content.ContentId.Value.ToString()}/{attachment.Name}";

                        BlockBlobClient blobToDelete = cdnContainer.GetBlockBlobClient(blobToDeleteFileName);

                        await blobToDelete.DeleteIfExistsAsync();

                    }
                    catch (Exception ex)
                    {
                        errorMessage = $"Failed to delete attachment for content {deleteCommand.Content.ContentId.Value.ToString()} attachment {attachment.Name} ";

                        deleteCommand.FailureDetails.Add(errorMessage);

                        _logger.LogError(ex, errorMessage);
                    }
                }
            }
            else
            {
                _logger.LogInformation($"DeleteAttachmentContentFromCdn for content id: {deleteCommand.Content.ContentId.Value.ToString()}. No Attachments found.");
            }
        }

        /// <summary>
        /// Populate command object with required attributes
        /// </summary>
        /// <param name="contentCommand"></param>
        private void PopulateContentCommand(Guid contentId, ContentCommand contentCommand, DateTime currentDateTime)
        {
            contentCommand.Id = Guid.NewGuid();
            contentCommand.ContentId = contentId;
            contentCommand.CommandType = CommandType.DeleteContent;
            contentCommand.Type = ContentContainerType.Command;
            contentCommand.CommandStatus = CommandStatus.InProgress;
            contentCommand.CreatedDate = currentDateTime;
            contentCommand.ModifiedDate = currentDateTime;
            contentCommand.FailureDetails = new List<string>();
        }
       
    }
}
