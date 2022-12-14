// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
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
using System.Threading.Tasks;

namespace blendnet.cms.listener.IntegrationEventHandling
{
    /// <summary>
    /// Responsible to hand content upload event.
    /// Copy the Media content from Raw Container to Mezzanine Container.
    /// Copy the Attachments from Raw Container to CDN Enabled Storage
    /// </summary>
    public class ContentUploadedIntegrationEventHandler : IIntegrationEventHandler<ContentUploadedIntegrationEvent>
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
        public ContentUploadedIntegrationEventHandler(ILogger<ContentUploadedIntegrationEventHandler> logger,
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
        public async Task Handle(ContentUploadedIntegrationEvent integrationEvent)
        {
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>("ContentUploadedIntegrationEventHandler.Handle"))
                {
                    _logger.LogInformation($"Message Recieved for content id: {integrationEvent.ContentUploadCommand.ContentId.ToString()}");

                    if (integrationEvent.ContentUploadCommand == null || 
                        integrationEvent.ContentUploadCommand.ContentId == Guid.Empty)
                    {
                        _logger.LogInformation($"No content details found in integration event. Pass correct data to integation event");

                        return;
                    }

                    ContentCommand uploadCommand = integrationEvent.ContentUploadCommand;

                    Content content = await _contentRepository.GetContentById(uploadCommand.ContentId);

                    if (content == null)
                    {
                        _logger.LogInformation($"No content details found in database for content id: {integrationEvent.ContentUploadCommand.ContentId.ToString()}");

                        return;
                    }

                    DateTime currentTime = DateTime.UtcNow;

                    PopulateContentCommand(uploadCommand, currentTime);
                            
                    //Create a command record with in progress status. It will use the command id as ID and Content Id and partition key
                    Guid commandId = await _contentRepository.CreateContentCommand(uploadCommand);

                    content.ContentUploadStatus = ContentUploadStatus.UploadInProgress;

                    content.ModifiedDate = currentTime;

                    content.ContentUploadStatusUpdatedBy = commandId;
                            
                    await _contentRepository.UpdateContent(content);

                    _logger.LogInformation($"Copying media file for content id: {integrationEvent.ContentUploadCommand.ContentId}");

                    await CopyMediaContentToMezzanine(content, uploadCommand);

                    _logger.LogInformation($"Copying attachment file(s) for content id: {integrationEvent.ContentUploadCommand.ContentId}");

                    await CopyAttachmentContentToCdn(content, uploadCommand);

                    content = await _contentRepository.GetContentById(uploadCommand.ContentId);

                    //Update the command status. In case of any error, mark it to failure state.
                    if (uploadCommand.FailureDetails.Count > 0)
                    {
                        uploadCommand.CommandStatus = CommandStatus.Failed;

                        content.ContentUploadStatus = ContentUploadStatus.UploadFailed;
                    }
                    else
                    {
                        uploadCommand.CommandStatus = CommandStatus.Complete;

                        content.ContentUploadStatus = ContentUploadStatus.UploadComplete;
                    }

                    currentTime = DateTime.UtcNow;

                    uploadCommand.ModifiedDate = currentTime;

                    content.ModifiedDate = currentTime;

                    content.ContentUploadStatusUpdatedBy = uploadCommand.Id;

                    await _contentRepository.UpdateContentCommand(uploadCommand);

                    await _contentRepository.UpdateContent(content);

                    _logger.LogInformation($"Message Process Completed for content id: {integrationEvent.ContentUploadCommand.ContentId.ToString()}");

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        /// <summary>
        /// Populates the media content to Mezzanine Container
        /// </summary>
        /// <param name="content"></param>
        /// <param name="uploadCommand"></param>
        /// <returns></returns>
        private async Task CopyMediaContentToMezzanine(Content content, ContentCommand uploadCommand)
        {
            string errorMessage = string.Empty;

            try
            {
                var baseName = content.ContentProviderId.ToString().Trim().ToLower();

                string rawContainerName = $"{baseName}{ApplicationConstants.StorageContainerSuffix.Raw}";

                string mezzContainerName = $"{baseName}{ApplicationConstants.StorageContainerSuffix.Mezzanine}";

                BlobContainerClient sourceContainer = this._cmsBlobServiceClient.GetBlobContainerClient(rawContainerName);

                BlobContainerClient destinationContainer = this._cmsBlobServiceClient.GetBlobContainerClient(mezzContainerName);

                BlockBlobClient sourceBlob = sourceContainer.GetBlockBlobClient(content.MediaFileName);

                if (await sourceBlob.ExistsAsync())
                {
                    BlockBlobClient targetBlob = destinationContainer.GetBlockBlobClient($"{content.ContentId.Value.ToString()}/{content.MediaFileName}");

                    await EventHandlingUtilities.CopyBlob(_logger,sourceBlob, targetBlob, content.Id.Value,uploadCommand.Id.Value);
                }
                else
                {
                    errorMessage = $"For content {content.ContentId.Value.ToString()} Source File {content.MediaFileName} does not exist in source container";

                    uploadCommand.FailureDetails.Add(errorMessage);

                    _logger.LogError($"{errorMessage}");
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Failed to copy media for content {content.ContentId.Value.ToString()} Media File {content.MediaFileName} Message {ex.Message}";

                uploadCommand.FailureDetails.Add(errorMessage);

                _logger.LogError(ex, $"{errorMessage}");
            }
        }

        /// <summary>
        /// Populates the media content to CDN storage Container
        /// </summary>
        /// <param name="content"></param>
        /// <param name="uploadCommand"></param>
        /// <returns></returns>
        private async Task CopyAttachmentContentToCdn(Content content, ContentCommand uploadCommand)
        {
            string errorMessage = string.Empty;

            if (content.Attachments != null && content.Attachments.Count > 0)
            {
                var baseName = content.ContentProviderId.ToString().Trim().ToLower();

                string rawContainerName = $"{baseName}{ApplicationConstants.StorageContainerSuffix.Raw}";

                string cdnContainerName = $"{baseName}{ApplicationConstants.StorageContainerSuffix.Cdn}";

                BlobContainerClient sourceContainer = this._cmsBlobServiceClient.GetBlobContainerClient(rawContainerName);

                BlobContainerClient destinationContainer = this._cmsCdnBlobServiceClient.GetBlobContainerClient(cdnContainerName);

                foreach (Attachment attachment in content.Attachments)
                {
                    try
                    {
                        BlockBlobClient sourceBlob = sourceContainer.GetBlockBlobClient(attachment.Name);

                        string blobSasUrl = EventHandlingUtilities.GetServiceSasUriForBlob(sourceContainer.GetBlobClient(attachment.Name), 
                                                                         ApplicationConstants.StorageContainerPolicyNames.RawReadOnly, 
                                                                         _appSettings.SASTokenExpiryToCopyContentInMts);

                        if (await sourceBlob.ExistsAsync())
                        {
                            BlockBlobClient targetBlob = destinationContainer.GetBlockBlobClient($"{content.ContentId.Value.ToString()}/{attachment.Name}");

                            await EventHandlingUtilities.CopyBlob(_logger, sourceBlob, targetBlob,content.Id.Value, uploadCommand.Id.Value ,blobSasUrl);
                        }
                        else
                        {
                            errorMessage = $"For content {content.ContentId.Value.ToString()} Source Attachment File {attachment.Name} does not exist in source container";

                            uploadCommand.FailureDetails.Add(errorMessage);

                            _logger.LogError($"{errorMessage}");
                        }

                    }
                    catch(Exception ex)
                    {
                        errorMessage = $"Failed to copy attachment for content {content.ContentId.Value.ToString()} attachment {attachment.Name} ";

                        uploadCommand.FailureDetails.Add(errorMessage);

                        _logger.LogError(ex, errorMessage);
                    }
                }
            }else
            {
                _logger.LogInformation($"CopyAttachmentContentToCdn for content id: {content.ContentId.ToString()}. No Attachments found.");
            }
        }

        /// <summary>
        /// Populate command object with required attributes
        /// </summary>
        /// <param name="contentCommand"></param>
        private void PopulateContentCommand(ContentCommand contentCommand, DateTime currentDateTime)
        {
            contentCommand.Id = Guid.NewGuid();
            contentCommand.CommandType = CommandType.UploadContent;
            contentCommand.Type = ContentContainerType.Command;
            contentCommand.CommandStatus = CommandStatus.InProgress;
            contentCommand.CreatedDate = currentDateTime;
            contentCommand.ModifiedDate = currentDateTime;
            contentCommand.FailureDetails = new List<string>();
        }

    }
}
