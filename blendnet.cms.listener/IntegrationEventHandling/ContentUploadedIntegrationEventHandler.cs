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
                    if (integrationEvent.ContentUploadCommand != null && integrationEvent.ContentUploadCommand.ContentId != null)
                    {
                        _logger.LogInformation($"Message Recieved for content id: {integrationEvent.ContentUploadCommand.ContentId.ToString()}");

                        ContentCommand uploadCommand = integrationEvent.ContentUploadCommand;

                        Content content = await _contentRepository.GetContentById(uploadCommand.ContentId);

                        if (content != null)
                        {
                            DateTime currentTime = DateTime.UtcNow;

                            PopulateContentCommand(uploadCommand, currentTime);
                            
                            //Create a command record with in progress status. It will use the command id as ID and Content Id and partition key
                            Guid commandId = await _contentRepository.CreateContentCommand(uploadCommand);

                            content.ContentUploadStatus = ContentUploadStatus.UploadInProgress;

                            content.ModifiedDate = currentTime;
                            
                            await _contentRepository.UpdateContent(content);

                            _logger.LogInformation($"Moving media file for content id: {integrationEvent.ContentUploadCommand.ContentId}");

                            await CopyMediaContentToMezzanine(content, uploadCommand);

                            _logger.LogInformation($"Moving media file for content id: {integrationEvent.ContentUploadCommand.ContentId}");

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

                            await _contentRepository.UpdateContentCommand(uploadCommand);

                            await _contentRepository.UpdateContent(content);

                            _logger.LogInformation($"Message Process Completed for content id: {integrationEvent.ContentUploadCommand.ContentId.ToString()}");
                        }
                        else
                        {
                            _logger.LogInformation($"No content details found in database for content id: {integrationEvent.ContentUploadCommand.ContentId.ToString()}");
                        }
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

                    await CopyBlob(sourceBlob, targetBlob);
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
                errorMessage = $"Failed to copy media for content {content.ContentId.Value.ToString()} Media File {content.MediaFileName}";

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

                        string blobSasUrl = GetServiceSasUriForBlob(sourceContainer.GetBlobClient(attachment.Name), 
                                                                         ApplicationConstants.StorageContainerPolicyNames.RawReadOnly, 
                                                                         _appSettings.SASTokenExpiryToCopyContentInMts);

                        if (await sourceBlob.ExistsAsync())
                        {
                            BlockBlobClient targetBlob = destinationContainer.GetBlockBlobClient($"{content.ContentId.Value.ToString()}/{attachment.Name}");

                            await CopyBlob(sourceBlob, targetBlob, blobSasUrl);
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


        /// <summary>
        /// https://docs.microsoft.com/en-us/azure/storage/common/storage-stored-access-policy-define-dotnet?tabs=dotnet
        /// </summary>
        /// <param name="blobClient"></param>
        /// <param name="identifier"></param>
        /// <param name="expiryMinutes"></param>
        /// <returns></returns>
        private string GetServiceSasUriForBlob(BlobClient blobClient, string identifier, int expiryMinutes)
        {
            // Create a SAS token that's valid for one hour.
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                BlobName = blobClient.Name,
                Identifier = identifier,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes)
            };

            Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

            return sasUri.AbsoluteUri;
        }

        /// <summary>
        /// Copy Blob
        /// </summary>
        /// <param name="sourceBlob"></param>
        /// <param name="targetBlob"></param>
        /// <returns></returns>
        private async Task CopyBlob(BlockBlobClient sourceBlob, BlockBlobClient targetBlob, string sourceBlobUrl="")
        {
            BlobLeaseClient lease = null;

            try
            {
                // Lease the source blob for the copy operation to prevent another client from modifying it.
                lease = sourceBlob.GetBlobLeaseClient();

                // Specifying -1 for the lease interval creates an infinite lease.
                await lease.AcquireAsync(TimeSpan.FromSeconds(-1));

                CopyFromUriOperation copyFromUriOperation;

                if (string.IsNullOrEmpty(sourceBlobUrl))
                {
                    // Start the copy operation.
                    copyFromUriOperation = await targetBlob.StartCopyFromUriAsync(sourceBlob.Uri);
                }
                else
                {
                    copyFromUriOperation = await targetBlob.StartCopyFromUriAsync(new Uri(sourceBlobUrl));
                }

                //wait for the operation to complete
                await copyFromUriOperation.WaitForCompletionAsync();

            }
            finally
            {
                // Update the source blob's properties.
                var sourceProperties = await sourceBlob.GetPropertiesAsync();

                if (sourceProperties.Value.LeaseState == LeaseState.Leased)
                {
                    // Break the lease on the source blob.
                    await lease.BreakAsync();
                }
            }
        }

    }
}
