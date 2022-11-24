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
using blendnet.common.infrastructure.Ams;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.listener.IntegrationEventHandling
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/media-services/latest/media-services-apis-overview#sdks
    /// Use a new AzureMediaServicesClient object per thread. 
    /// </summary>
    public class ContentTransformIntegrationEventHandler : IIntegrationEventHandler<ContentTransformIntegrationEvent>
    {
        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        private readonly AppSettings _appSettings;

        BlobServiceClient _cmsBlobServiceClient;

        IContentRepository _contentRepository;

        /// <summary>
        /// Content Transform Integration Event
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="tc"></param>
        /// <param name="blobClientFactory"></param>
        /// <param name="optionsMonitor"></param>
        /// <param name="contentRepository"></param>
        public ContentTransformIntegrationEventHandler(ILogger<ContentTransformIntegrationEventHandler> logger,
                                                       TelemetryClient tc,
                                                       IAzureClientFactory<BlobServiceClient> blobClientFactory,
                                                       IOptionsMonitor<AppSettings> optionsMonitor,
                                                       IContentRepository contentRepository)
        {
            _logger = logger;

            _telemetryClient = tc;

            _cmsBlobServiceClient = blobClientFactory.CreateClient(ApplicationConstants.StorageInstanceNames.CMSStorage);

            _appSettings = optionsMonitor.CurrentValue;

            _contentRepository = contentRepository;
        }

        /// <summary>
        /// Responsible of submitting job to AMS
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>

        public async Task Handle(ContentTransformIntegrationEvent integrationEvent)
        {
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>("ContentTransformIntegrationEventHandler.Handle"))
                {
                    if (integrationEvent.ContentTransformCommand == null || 
                        integrationEvent.ContentTransformCommand.ContentId == Guid.Empty)
                    {
                        _logger.LogInformation($"No content details found in integration event. Pass correct data to integation event");

                        return;
                    }
                        
                    _logger.LogInformation($"Message Recieved for content id: {integrationEvent.ContentTransformCommand.ContentId.ToString()}");

                    ContentCommand transformCommand = integrationEvent.ContentTransformCommand;

                    Content content = await _contentRepository.GetContentById(transformCommand.ContentId);

                    if (content == null)
                    {
                        _logger.LogInformation($"No content details found in database for content id: {integrationEvent.ContentTransformCommand.ContentId.ToString()}");

                        return;
                    }
                        
                    DateTime currentTime = DateTime.UtcNow;

                    PopulateContentCommand(transformCommand, currentTime);

                    _logger.LogInformation($"Transforming for content id: {integrationEvent.ContentTransformCommand.ContentId} Command Id {transformCommand.Id.Value}");

                    //Create a command record with in progress status. It will use the command id as ID and Content Id and partition key
                    Guid commandId = await _contentRepository.CreateContentCommand(transformCommand);

                    content.ContentTransformStatus = ContentTransformStatus.TransformInProgress;

                    content.ModifiedDate = currentTime;

                    content.ContentTransformStatusUpdatedBy = commandId;

                    await _contentRepository.UpdateContent(content);

                    //Perform the content transformation
                    await SubmitContentTransformationJob(content, transformCommand);

                    //Update the command status. In case of any error, mark it to failure state.
                    if (transformCommand.FailureDetails.Count > 0)
                    {
                        await UpdateFailedStatus(content, transformCommand);
                    }
                    else
                    {
                        await UpdateInProgressStatus(content, transformCommand);
                    }

                    _logger.LogInformation($"Request submitted to AMS for content id: {integrationEvent.ContentTransformCommand.ContentId} command id : {transformCommand.Id.Value}");
                        
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        /// <summary>
        /// Update Failed Status
        /// </summary>
        /// <param name="content"></param>
        /// <param name="transformCommand"></param>
        /// <returns></returns>

        private async Task UpdateFailedStatus(Content content, ContentCommand transformCommand)
        {
            content.ContentTransformStatus = ContentTransformStatus.TransformFailed;

            transformCommand.CommandStatus = CommandStatus.Failed;

            await UpdateStatus(content, transformCommand);
        }

       /// <summary>
       /// Update In Progress String
       /// </summary>
       /// <param name="content"></param>
       /// <param name="transformCommand"></param>
       /// <returns></returns>
        private async Task UpdateInProgressStatus(Content content, ContentCommand transformCommand)
        {
            content.ContentTransformStatus = ContentTransformStatus.TransformAMSJobInProgress;

            transformCommand.CommandStatus = CommandStatus.InProgress;

            await UpdateStatus(content, transformCommand);
        }

        /// <summary>
        /// Update Status
        /// </summary>
        /// <param name="content"></param>
        /// <param name="transformCommand"></param>
        /// <returns></returns>
        private async Task UpdateStatus(Content content, ContentCommand transformCommand)
        {
            DateTime currentTime = DateTime.UtcNow;

            content.ModifiedDate = currentTime;

            content.ContentTransformStatusUpdatedBy = transformCommand.Id;

            transformCommand.ModifiedDate = currentTime;

            await _contentRepository.UpdateContent(content);

            await _contentRepository.UpdateContentCommand(transformCommand);
        }

        /// <summary>
        /// Populate command object with required attributes
        /// </summary>
        /// <param name="contentCommand"></param>
        private void PopulateContentCommand(ContentCommand contentCommand, DateTime currentDateTime)
        {
            contentCommand.Id = Guid.NewGuid();
            contentCommand.CommandType = CommandType.TransformContent;
            contentCommand.Type = ContentContainerType.Command;
            contentCommand.CommandStatus = CommandStatus.InProgress;
            contentCommand.CreatedDate = currentDateTime;
            contentCommand.ModifiedDate = currentDateTime;
            contentCommand.FailureDetails = new List<string>();
        }

        /// <summary>
        /// Submits the tranformation job
        /// </summary>
        /// <param name="content"></param>
        /// <param name="tranformCommand"></param>
        /// <returns></returns>
        private async Task SubmitContentTransformationJob(Content content, ContentCommand transformCommand)
        {
            string errorMessage = string.Empty;

            try
            {
                string uniqueName = $"{content.Id.Value}|{transformCommand.Id.Value}";

                IAzureMediaServicesClient amsclient = await  AmsUtilities.CreateMediaServicesClientAsync(_appSettings.AmsArmEndPoint,
                                                                                                         _appSettings.AmsClientId,
                                                                                                         _appSettings.AmsClientSecret,
                                                                                                         _appSettings.AmsTenantId,
                                                                                                        _appSettings.AmsSubscriptionId);
                await EnsureTransformExists(amsclient);

                _logger.LogInformation($"Ensure Transform Exists executed for content id: {transformCommand.ContentId.ToString()}. Unique Name : {uniqueName}");

                await CreateOutputAsset(amsclient, uniqueName);

                _logger.LogInformation($"Output Assest Created for content id: {transformCommand.ContentId.ToString()}. Unique Name : {uniqueName}");

                string injestUrl = GetIngestAssetUrl(content);

                _logger.LogInformation($"Injest URL generated for content id: {transformCommand.ContentId.ToString()}. Unique Name : {uniqueName} . Url {injestUrl}");

                await SubmitJob(amsclient, injestUrl, uniqueName, uniqueName);
            }
            catch(Exception ex)
            {
                errorMessage = $"Failed to submit job with AMS for content {content.ContentId.Value} Command {transformCommand.Id.Value} Exception {ex.Message}";

                transformCommand.FailureDetails.Add(errorMessage);

                _logger.LogError(ex, $"{errorMessage}");
            }
        }

        /// <summary>
        /// Returns the Injest Url
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private string GetIngestAssetUrl(Content content)
        {
            string mezzContainerName = $"{content.ContentProviderId}{ApplicationConstants.StorageContainerSuffix.Mezzanine}";

            BlobContainerClient sourceContainer = this._cmsBlobServiceClient.GetBlobContainerClient(mezzContainerName);

            string blobName = $"{content.ContentId.Value.ToString()}/{content.MediaFileName}";

            string blobSasUrl = EventHandlingUtilities.GetServiceSasUriForBlob(sourceContainer.GetBlobClient(blobName),
                                                             ApplicationConstants.StorageContainerPolicyNames.MezzanineReadOnly,
                                                             _appSettings.SASTokenExpiryForAmsJobInMts);

            return blobSasUrl;

        }


        /// <summary>
        /// Ensures that Blendnet Transformation Exists at AMS Instance
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private async Task<Transform> EnsureTransformExists(IAzureMediaServicesClient client)
        {
            Transform transform = null;

            try
            {
                transform = await client.Transforms.GetAsync(_appSettings.AmsResourceGroupName,
                                                        _appSettings.AmsAccountName,
                                                        _appSettings.AmsTransformationName);
            }
            catch (ErrorResponseException ex) when (ex.Response.StatusCode == System.Net.HttpStatusCode.NotFound) 
            {}
            

            if (transform == null)
            {
                // Create a new Transform Outputs array - this defines the set of outputs for the Transform
                TransformOutput[] outputs = new TransformOutput[]
                {
                   new TransformOutput(
                    new StandardEncoderPreset(
                    codecs: new Codec[]
                    {
                        // Add an AAC Audio layer for the audio encoding
                        new AacAudio(
                            channels: _appSettings.AmsAacAudioChannels,
                            samplingRate: _appSettings.AmsAacAudioSamplingRate,
                            bitrate: _appSettings.AmsAacAudioBitRate,
                            profile: AacAudioProfile.AacLc
                        ),
                        // Next, add a H264Video for the video encoding
                       new H264Video (
                            // Set the GOP interval to 2 seconds for both H264Layers
                            keyFrameInterval:TimeSpan.FromSeconds(_appSettings.AmsH264VideoKeyFrameworkIntervalInSec),
                            //  Add H264Layers, one at HD and the other at SD. Assign a label that you can use for the output filename
                            layers:  new H264Layer[]
                            {
                                new H264Layer (
                                    bitrate: _appSettings.AmsH264LayerBitRate,
                                    width: $"{_appSettings.AmsH264LayerWidth}",
                                    height: $"{_appSettings.AmsH264LayerHeight}",
                                    label: $"{_appSettings.AmsH264LayerLabel}"
                                )
                            }
                        ),
                    },
                    // Specify the format for the output files - one for video+audio, and another for the thumbnails
                    formats: new Format[]
                    {
                        // Mux the H.264 video and AAC audio into MP4 files, using basename, label, bitrate and extension macros
                        // Note that since you have multiple H264Layers defined above, you have to use a macro that produces unique names per H264Layer
                        // Either {Label} or {Bitrate} should suffice
                        new Mp4Format(
                            filenamePattern:"Video-{Basename}-{Label}-{Bitrate}{Extension}"
                        )
                    }),
                    onError: OnErrorType.StopProcessingJob,
                    relativePriority: Priority.Normal)
                };

                string description = "A custom encoding transform for blendnet";

                // Create the custom Transform with the outputs defined above
                transform = client.Transforms.CreateOrUpdate(   _appSettings.AmsResourceGroupName,
                                                                _appSettings.AmsAccountName,
                                                                _appSettings.AmsTransformationName, 
                                                                outputs, 
                                                                description);
            }

            return transform;
        }

        /// <summary>
        /// Create Output Asset
        /// </summary>
        /// <param name="client"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        private async Task<Asset> CreateOutputAsset(IAzureMediaServicesClient client, string assetName)
        {
            Asset asset = new Asset();

            return await client.Assets.CreateOrUpdateAsync(_appSettings.AmsResourceGroupName,
                                                            _appSettings.AmsAccountName, 
                                                            assetName, asset);
        }

        /// <summary>
        /// Submit Job to AMS
        /// </summary>
        /// <param name="client"></param>
        /// <param name="inputAssetUrl"></param>
        /// <param name="outPutAssetName"></param>
        /// <param name="jobName"></param>
        /// <returns></returns>
        private async Task<Job> SubmitJob(IAzureMediaServicesClient client, 
                                                        string inputAssetUrl, 
                                                        string outPutAssetName,
                                                        string jobName)
        {
            var trackList = new List<TrackDescriptor>
                {
                       new SelectAudioTrackByAttribute()
                       {
                           Attribute = TrackAttribute.Bitrate,
                           Filter = AttributeFilter.Top
                        },
                        new SelectVideoTrackByAttribute()
                        {
                            Attribute = TrackAttribute.Bitrate,
                            Filter = AttributeFilter.Top
                        }
                };

            var inputDefinitions = new List<InputDefinition>()
                {
                    new FromAllInputFile()
                    {
                        IncludedTracks = trackList
                    }
                };


            JobInputHttp jobInput = new JobInputHttp(files: new[] { inputAssetUrl }, inputDefinitions: inputDefinitions);

            JobOutput[] jobOutputs =
            {
                new JobOutputAsset(outPutAssetName),
            };

            Job job = await client.Jobs.CreateAsync(_appSettings.AmsResourceGroupName,
                                                    _appSettings.AmsAccountName,
                                                    _appSettings.AmsTransformationName,
                                                     jobName,
                new Job
                {
                    Input = jobInput,
                    Outputs = jobOutputs,
                });

            return job;
        }

    }
}
