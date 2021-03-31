using Azure.Storage.Blobs;
using blendnet.cms.listener.Common;
using blendnet.cms.repository.Interfaces;
using blendnet.common.dto;
using blendnet.common.dto.cms;
using blendnet.common.dto.Cms;
using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.listener.IntegrationEventHandling
{
    /// <summary>
    /// Handles the events from AMS.
    /// Subscription should be configured to handle the 3 events
    /// JobFinished, JobCanceled, JobErrored
    /// </summary>
    public class MediaServiceJobIntegrationEventHandler : IIntegrationEventHandler<MediaServiceJobIntegrationEvent>
    {
        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        private readonly AppSettings _appSettings;

        BlobServiceClient _cmsBlobServiceClient;

        IContentRepository _contentRepository;

        SegmentDowloader _segmentDowloader;

        TarGenerator _tarGenerator;

        public MediaServiceJobIntegrationEventHandler(ILogger<ContentUploadedIntegrationEventHandler> logger,
                                                            TelemetryClient tc,
                                                            IAzureClientFactory<BlobServiceClient> blobClientFactory,
                                                            IOptionsMonitor<AppSettings> optionsMonitor,
                                                            IContentRepository contentRepository,
                                                            SegmentDowloader segmentDowloader,
                                                            TarGenerator tarGenerator)
        {
            _logger = logger;

            _telemetryClient = tc;

            _cmsBlobServiceClient = blobClientFactory.CreateClient(ApplicationConstants.StorageInstanceNames.CMSStorage);

            _appSettings = optionsMonitor.CurrentValue;

            _contentRepository = contentRepository;

            _segmentDowloader = segmentDowloader;

            _tarGenerator = tarGenerator;
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/azure/media-services/latest/protect-with-drm
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(MediaServiceJobIntegrationEvent integrationEvent)
        {
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>("MediaServiceJobIntegrationEventHandler.Handle"))
                {
                    string request = System.Text.Json.JsonSerializer.Serialize(integrationEvent);

                    _logger.LogInformation($"Message Recieved from AMS: {request}");

                    string contentId, commandId;

                    //extract the command id and content id from topic string
                    GetContentandCommandId(integrationEvent.subject, out contentId, out commandId);

                    if(string.IsNullOrEmpty(contentId) || string.IsNullOrEmpty(commandId))
                    {
                        _logger.LogInformation($"Failed to extract content id and command id from topic: {integrationEvent.topic}");

                        return;
                    }

                    _logger.LogInformation($"Extracted content id: {contentId} Command Id {commandId} from topic");

                    ContentCommand transformCommand = await _contentRepository.GetContentCommandById(Guid.Parse(commandId), Guid.Parse(contentId));

                    Content content = await _contentRepository.GetContentById(Guid.Parse(contentId));

                    if (content == null || transformCommand == null)
                    {
                        _logger.LogInformation($"No content or command details found in database for content id: {contentId} command id {commandId}");

                        return;
                    }

                    if ( integrationEvent.data.state.Equals(ApplicationConstants.AMSJobStatuses.JobFinished,
                         StringComparison.InvariantCultureIgnoreCase))
                    {
                        await ProcessAMSCompletedJob(content, transformCommand);

                        _logger.LogInformation($"DASH URL generated for content id: {contentId} command id {commandId}");

                        await DownloadSegements(content, transformCommand);

                        _logger.LogInformation($"Segments downloaded and moved to final for content id: {contentId} command id {commandId}");

                        //Update the command status. In case of any error, mark it to failure state.
                        if (transformCommand.FailureDetails.Count > 0)
                        {
                            await UpdateFailedStatus(content, transformCommand);
                        }
                        else
                        {
                            await UpdateSuccessStatus(content, transformCommand);
                        }
                    }
                    else
                    {
                        transformCommand.FailureDetails.Add($"AMS status recieved for Content Id {content.Id.Value} Command Id {transformCommand.Id.Value} is {integrationEvent.data.state}.");

                        await UpdateFailedStatus(content, transformCommand);
                    }

                    _logger.LogInformation($"Completed Media Service Job Completed Handling for content id: {contentId} command id {commandId}");
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
        /// Update Sucess Status
        /// </summary>
        /// <param name="content"></param>
        /// <param name="transformCommand"></param>
        /// <returns></returns>
        private async Task UpdateSuccessStatus(Content content, ContentCommand transformCommand)
        {
            content.ContentTransformStatus = ContentTransformStatus.TransformComplete;
                        
            transformCommand.CommandStatus = CommandStatus.Complete;

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

            transformCommand.ModifiedDate = currentTime;

            await _contentRepository.UpdateContent(content);

            await _contentRepository.UpdateContentCommand(transformCommand);
        }



        /// <summary>
        /// Creates Content Policy
        /// Create Streaming locator
        /// </summary>
        /// <param name="content"></param>
        /// <param name="contentCommand"></param>
        /// <returns></returns>
        private async Task ProcessAMSCompletedJob(Content content, ContentCommand transformCommand)
        {
            string errorMessage;

            try
            {
                IAzureMediaServicesClient amsclient = await EventHandlingUtilities.CreateMediaServicesClientAsync(_appSettings);

                _logger.LogInformation($"Process AMS Job - AMS Client Created for content id: {content.Id.Value} command id {transformCommand.Id.Value}");

                string uniqueName = $"{content.Id.Value}|{transformCommand.Id.Value}";

                //Create the Content Policy if does not exists
                await GetOrCreateContentKeyPolicyAsync(amsclient);

                _logger.LogInformation($"Process AMS Job - Content key Policy Created for content id: {content.Id.Value} command id {transformCommand.Id.Value}");

                //Create streaming locator. One stream locator for each assest.
                StreamingLocator streamingLocator = await CreateStreamingLocatorAsync(amsclient, uniqueName, uniqueName, _appSettings.AmsContentPolicyName);

                _logger.LogInformation($"Process AMS Job - Streaming Locator Created for content id: {content.Id.Value} command id {transformCommand.Id.Value}");

                //Start the streaming locator if not started
                StreamingEndpoint streamingEndpoint = await StartStreamingLocator(amsclient);

                _logger.LogInformation($"Process AMS Job - Streaming Endpoint Started for content id: {content.Id.Value} command id {transformCommand.Id.Value}");

                // Get the Dash Url
                string dashUrl = await GetDASHStreamingUrlAsync(amsclient, streamingEndpoint, uniqueName);

                _logger.LogInformation($"Process AMS Job - Dash Url {dashUrl} generated for content id: {content.Id.Value} command id {transformCommand.Id.Value}");

                content.DashUrl = dashUrl;

                await DeleteAmsJob(amsclient,uniqueName);

                _logger.LogInformation($"Process AMS Job - Deleted AMS Job for content id: {content.Id.Value} command id {transformCommand.Id.Value}");
            }
            catch (Exception ex)
            {
                errorMessage = $"Failed to processs completed job from AMS for content {content.ContentId.Value} Command {transformCommand.Id.Value} Exception {ex.Message}";

                transformCommand.FailureDetails.Add(errorMessage);

                _logger.LogError(ex, $"{errorMessage}");
            }
        }

        /// <summary>
        /// Downloads the Segments using the DASH URL
        /// </summary>
        /// <param name="content"></param>
        /// <param name="transformCommand"></param>
        /// <returns></returns>
        private async Task DownloadSegements(Content content, ContentCommand transformCommand)
        {
            string errorMessage;

            try
            {
                string rootDirectory = Path.Combine(Directory.GetCurrentDirectory(), $"{transformCommand.Id.Value}");

                _logger.LogInformation($"Root Directory Path {rootDirectory} for content id: {content.Id.Value} command id {transformCommand.Id.Value}");

                string workingDirectory = Path.Combine(Directory.GetCurrentDirectory(),
                                                        $"{transformCommand.Id.Value}",
                                                        ApplicationConstants.DownloadDirectoryNames.Working);

                _logger.LogInformation($"Working Directory Path {workingDirectory} for content id: {content.Id.Value} command id {transformCommand.Id.Value}");

                string finalDirectory = Path.Combine(Directory.GetCurrentDirectory(),
                                                        $"{transformCommand.Id.Value}",
                                                        ApplicationConstants.DownloadDirectoryNames.Final);

                //Create Final Directory If does not exists
                if (!Directory.Exists(finalDirectory))
                {
                    Directory.CreateDirectory(finalDirectory);
                }

                _logger.LogInformation($"Finals Directory Path {finalDirectory} for content id: {content.Id.Value} command id {transformCommand.Id.Value}");

                MpdInfo mpdInfo = await _segmentDowloader.DownloadSegments(content.DashUrl, workingDirectory, transformCommand.Id.Value.ToString());

                _logger.LogInformation($"Download Segment Complete for content id: {content.Id.Value} command id {transformCommand.Id.Value}");

                MoveContentToFinal(content, transformCommand, mpdInfo, rootDirectory, workingDirectory, finalDirectory);

                _logger.LogInformation($"Moved the content to final directory for : {content.Id.Value} command id {transformCommand.Id.Value}");
            }
            catch(Exception ex)
            {
                errorMessage = $"Failed to download segments for content {content.ContentId.Value} Command {transformCommand.Id.Value} Exception {ex.Message}";

                transformCommand.FailureDetails.Add(errorMessage);

                _logger.LogError(ex, $"{errorMessage}");
            }
        }


        /// <summary>
        /// Move the content to final directory
        /// </summary>
        /// <param name="content"></param>
        /// <param name="transformCommand"></param>
        /// <param name="mpdInfo"></param>
        /// <param name="rootDirectory"></param>
        /// <param name="workingDirectory"></param>
        /// <param name="finalDirectory"></param>
        private void MoveContentToFinal(
                                        Content content,
                                        ContentCommand transformCommand,
                                        MpdInfo mpdInfo, 
                                        string rootDirectory,
                                        string workingDirectory, 
                                        string finalDirectory)
        {
            string tarPath;

            foreach (AdaptiveSetInfo adaptiveSet in mpdInfo.AdaptiveSets)
            {
                tarPath = Path.Combine(workingDirectory, $"{adaptiveSet.DirectoryName}.tar");

                adaptiveSet.FinalPath = Path.Combine(finalDirectory, $"{transformCommand.Id.Value.ToString()}_{adaptiveSet.DirectoryName}.tar");

                _tarGenerator.TarCreateFromStream(tarPath, Path.Combine(workingDirectory, adaptiveSet.DirectoryName));

                File.Move(tarPath, adaptiveSet.FinalPath);
            }

            string mpdPath = Path.Combine(workingDirectory, $"{mpdInfo.MpdName}");

            mpdInfo.FinalMpdPath = Path.Combine(finalDirectory, $"{mpdInfo.MpdName}");

            File.Copy(mpdPath, mpdInfo.FinalMpdPath);

        }

        /// <summary>
        /// Extracts the content id and command id from the topic string
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="contentId"></param>
        /// <param name="commandId"></param>
        private void GetContentandCommandId(string topic, out string contentId, out string commandId)
        {
            try
            {
                string id = topic.Split('/').Last();

                contentId = id.Split('|')[0];

                commandId = id.Split('|')[1];
            }
            catch (Exception ex)
            {
                contentId = string.Empty;

                commandId = string.Empty;

                _logger.LogError(ex, ex.Message);
            }
            
        }


        /// <summary>
        /// Delete the sucessfull AMS job
        /// </summary>
        /// <param name="amsclient"></param>
        /// <param name="jobName"></param>
        /// <returns></returns>
        private async Task DeleteAmsJob(IAzureMediaServicesClient amsclient, string jobName)
        {
            await amsclient.Jobs.DeleteAsync(_appSettings.AmsResourceGroupName,
                                                   _appSettings.AmsAccountName,
                                                   _appSettings.AmsTransformationName,
                                                    jobName);
        }


        /// <summary>
        /// Create or Get the Content Policy
        /// </summary>
        /// <param name="amsclient"></param>
        /// <returns></returns>
        private async Task<ContentKeyPolicy> GetOrCreateContentKeyPolicyAsync(IAzureMediaServicesClient amsclient)
        {
            ContentKeyPolicy policy = await amsclient.ContentKeyPolicies.GetAsync(  _appSettings.AmsResourceGroupName,
                                                                                    _appSettings.AmsAccountName,
                                                                                    _appSettings.AmsContentPolicyName);

            if (policy == null)
            {
                byte[] tokenSigningKey = Convert.FromBase64String(_appSettings.AmsTokenSigningKey);

                ContentKeyPolicySymmetricTokenKey primaryKey = new ContentKeyPolicySymmetricTokenKey(tokenSigningKey);

                List<ContentKeyPolicyTokenClaim> requiredClaims = new List<ContentKeyPolicyTokenClaim>()
                {
                    ContentKeyPolicyTokenClaim.ContentKeyIdentifierClaim
                };

                ContentKeyPolicyTokenRestriction restriction
                    = new ContentKeyPolicyTokenRestriction( _appSettings.AmsTokenIssuer, 
                                                            _appSettings.AmsTokenAudience, 
                                                            primaryKey, 
                                                            ContentKeyPolicyRestrictionTokenType.Jwt, 
                                                            null, 
                                                            requiredClaims);

                ContentKeyPolicyWidevineConfiguration widevineConfig = ConfigureWidevineLicenseTempate();

                List<ContentKeyPolicyOption> options = new List<ContentKeyPolicyOption>
                {
                    new ContentKeyPolicyOption()
                    {
                        Configuration = widevineConfig,
                        Restriction = restriction
                    }
                };

                policy = await amsclient.ContentKeyPolicies.CreateOrUpdateAsync(_appSettings.AmsResourceGroupName,
                                                                                _appSettings.AmsAccountName,
                                                                                _appSettings.AmsContentPolicyName, 
                                                                                options);
            }
            else
            {
                // Get the signing key from the existing policy.
                //var policyProperties = await amsclient.ContentKeyPolicies.GetPolicyPropertiesWithSecretsAsync(
                //                                                                _appSettings.AmsResourceGroupName,
                //                                                                _appSettings.AmsAccountName,
                //                                                                _appSettings.AmsContentPolicyName);

                //if (policyProperties.Options[0].Restriction is ContentKeyPolicyTokenRestriction restriction)
                //{
                //    if (restriction.PrimaryVerificationKey is ContentKeyPolicySymmetricTokenKey signingKey)
                //    {
                //        TokenSigningKey = signingKey.KeyValue;
                //    }
                //}
            }

            return policy;
        }

        /// <summary>
        /// Start Stream Locator in case its not started
        /// </summary>
        /// <param name="amsclient"></param>
        /// <returns></returns>
        private async Task<StreamingEndpoint> StartStreamingLocator(IAzureMediaServicesClient amsclient)
        {
            StreamingEndpoint streamingEndpoint = await amsclient.StreamingEndpoints.GetAsync(_appSettings.AmsResourceGroupName,
                                                                                              _appSettings.AmsAccountName,
                                                                                              _appSettings.AmsStreamingEndpointName);
            if (streamingEndpoint != null)
            {
                if (streamingEndpoint.ResourceState != StreamingEndpointResourceState.Running)
                {

                    await amsclient.StreamingEndpoints.StartAsync(  _appSettings.AmsResourceGroupName, 
                                                                    _appSettings.AmsAccountName,
                                                                    _appSettings.AmsStreamingEndpointName);
                }
            }

            return streamingEndpoint;
        }
            
        /// <summary>
        /// Create Streaming Locator
        /// </summary>
        /// <param name="client"></param>
        /// <param name="assetName"></param>
        /// <param name="locatorName"></param>
        /// <param name="contentPolicyName"></param>
        /// <returns></returns>
        private async Task<StreamingLocator> CreateStreamingLocatorAsync(IAzureMediaServicesClient client,
                                                                                string assetName,
                                                                                string locatorName,
                                                                                string contentPolicyName)
        {
            StreamingLocator locator = await client.StreamingLocators.CreateAsync(
                _appSettings.AmsResourceGroupName,
                _appSettings.AmsAccountName,
                locatorName,
                new StreamingLocator
                {
                    AssetName = assetName,
                    StreamingPolicyName = "Predefined_MultiDrmCencStreaming",
                    DefaultContentKeyPolicyName = contentPolicyName
                });

            return locator;
        }
        /// <summary>
        /// Get WidevineLicenseTemplate
        /// </summary>
        /// <returns></returns>
        private ContentKeyPolicyWidevineConfiguration ConfigureWidevineLicenseTempate()
        {
            WidevineTemplate template = new WidevineTemplate()
            {
                AllowedTrackTypes = "SD_HD",
                ContentKeySpecs = new ContentKeySpec[]
                {
                    new ContentKeySpec()
                    {
                        TrackType = "SD",
                        SecurityLevel = 1,
                        RequiredOutputProtection = new OutputProtection()
                        {
                            HDCP = "HDCP_NONE"
                        }
                    }
                },
                PolicyOverrides = new PolicyOverrides()
                {
                    CanPlay = _appSettings.AmsWidevineCanPlay,
                    CanPersist = _appSettings.AmsWidevineCanPersist,
                    CanRenew = _appSettings.AmsWidevineCanRenew,
                    RentalDurationSeconds = _appSettings.AmsWidevineRentalDurationSeconds,
                    PlaybackDurationSeconds = _appSettings.AmsWidevinePlaybackDurationSeconds,
                    LicenseDurationSeconds = _appSettings.AmsWidevineLicenseDurationSeconds,
                }
            };

            ContentKeyPolicyWidevineConfiguration objContentKeyPolicyWidevineConfiguration = new ContentKeyPolicyWidevineConfiguration
            {
                WidevineTemplate = Newtonsoft.Json.JsonConvert.SerializeObject(template)
            };

            return objContentKeyPolicyWidevineConfiguration;
        }

        /// <summary>
        /// Get Dash Streaming Url
        /// </summary>
        /// <param name="client"></param>
        /// <param name="streamingEndpoint"></param>
        /// <param name="locatorName"></param>
        /// <returns></returns>
        private async Task<string> GetDASHStreamingUrlAsync(
           IAzureMediaServicesClient client,
           StreamingEndpoint streamingEndpoint,
           string locatorName)
        {
            string dashPath = string.Empty;

            ListPathsResponse paths = await client.StreamingLocators.ListPathsAsync(_appSettings.AmsResourceGroupName,
                                                                                    _appSettings.AmsAccountName, 
                                                                                    locatorName);
            foreach (StreamingPath path in paths.StreamingPaths)
            {
                UriBuilder uriBuilder = new UriBuilder
                {
                    Scheme = "https",
                    Host = streamingEndpoint.HostName
                };

                if (path.StreamingProtocol == StreamingPolicyStreamingProtocol.Dash)
                {
                    uriBuilder.Path = path.Paths[0];
                    dashPath = uriBuilder.ToString();
                }
            }

            return dashPath;
        }
    }
}

