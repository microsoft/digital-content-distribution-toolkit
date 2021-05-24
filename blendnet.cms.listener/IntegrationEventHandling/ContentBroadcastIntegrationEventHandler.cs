using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using blendnet.cms.listener.Common;
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
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.listener.IntegrationEventHandling
{
    /// <summary>
    /// Responsible for creating final TAR file.
    /// Moves the final TAR file to processed folder.
    /// Moves the final TAR file to SES folder.
    /// </summary>
    public class ContentBroadcastIntegrationEventHandler : IIntegrationEventHandler<ContentBroadcastIntegrationEvent>
    {
        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        private readonly AppSettings _appSettings;

        BlobServiceClient _cmsBlobServiceClient;

        BlobServiceClient _broadcastServiceClient;

        IContentRepository _contentRepository;

        SegmentDowloader _segmentDowloader;

        TarGenerator _tarGenerator;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="tc"></param>
        /// <param name="blobClientFactory"></param>
        /// <param name="optionsMonitor"></param>
        /// <param name="contentRepository"></param>

        public ContentBroadcastIntegrationEventHandler(ILogger<ContentBroadcastIntegrationEventHandler> logger,
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

            _broadcastServiceClient = blobClientFactory.CreateClient(ApplicationConstants.StorageInstanceNames.BroadcastStorage);

            _appSettings = optionsMonitor.CurrentValue;

            _contentRepository = contentRepository;

            _segmentDowloader = segmentDowloader;

            _tarGenerator = tarGenerator;
        }

        /// <summary>
        /// Handles the Broadcast event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(ContentBroadcastIntegrationEvent integrationEvent)
        {
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>("ContentBroadcastIntegrationEventHandler.Handle"))
                {
                    if (integrationEvent.ContentBroadcastCommand == null ||
                        integrationEvent.ContentBroadcastCommand.ContentId == Guid.Empty ||
                        integrationEvent.ContentBroadcastCommand.BroadcastRequest == null)
                    {
                        _logger.LogInformation($"No content details or Broadcast details found in integration event. Pass correct data to integation event");

                        return;
                    }

                    _logger.LogInformation($"Message Recieved for content id: {integrationEvent.ContentBroadcastCommand.ContentId}");

                    ContentCommand broadcastCommand = integrationEvent.ContentBroadcastCommand;

                    Content content = await _contentRepository.GetContentById(broadcastCommand.ContentId);

                    if (content == null)
                    {
                        _logger.LogInformation($"No content details found in database for content id: {integrationEvent.ContentBroadcastCommand.ContentId.ToString()}");

                        return;
                    }

                    DateTime currentTime = DateTime.UtcNow;

                    PopulateContentCommand(broadcastCommand, currentTime);

                    _logger.LogInformation($"Broadcasting for content id: {integrationEvent.ContentBroadcastCommand.ContentId} Command Id {broadcastCommand.Id.Value}");

                    //Create a command record with in progress status. It will use the command id as ID and Content Id and partition key
                    Guid commandId = await _contentRepository.CreateContentCommand(broadcastCommand);

                    content.ContentBroadcastStatus = ContentBroadcastStatus.BroadcastInProgress;

                    content.ModifiedDate = currentTime;

                    content.ContentBroadcastStatusUpdatedBy = commandId;

                    await _contentRepository.UpdateContent(content);

                    //Perform the content transformation
                    await PerformBroadcast(content, broadcastCommand);

                    //Update the command status. In case of any error, mark it to failure state.
                    if (broadcastCommand.FailureDetails.Count > 0)
                    {
                        await UpdateFailedStatus(content, broadcastCommand);
                    }
                    else
                    {
                        await UpdateInProgressStatus(content, broadcastCommand);
                    }

                    _logger.LogInformation($"Broadcast TAR pushed for content id: {integrationEvent.ContentBroadcastCommand.ContentId} command id : {broadcastCommand.Id.Value}");

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        /// <summary>
        /// Responsible for updating the XML File.
        /// Move the XML to fnl folder
        /// Generate the TAR File
        /// Move the TAR to processed folder
        /// Move the TAR to SES Storage Account!
        /// </summary>
        /// <param name="content"></param>
        /// <param name="transformCommand"></param>
        /// <returns></returns>
        private async Task PerformBroadcast(Content content, ContentCommand broadcastCommand)
        {
            string errorMessage = string.Empty;

            try
            {
                var baseName = content.ContentProviderId.ToString().Trim().ToLower();

                string mezzContainerName = $"{baseName}{ApplicationConstants.StorageContainerSuffix.Mezzanine}";

                string processedContainerName = $"{baseName}{ApplicationConstants.StorageContainerSuffix.Processed}";

                BlobContainerClient mezzContainer = this._cmsBlobServiceClient.GetBlobContainerClient(mezzContainerName);

                BlobContainerClient processedContainer = this._cmsBlobServiceClient.GetBlobContainerClient(processedContainerName);

                MpdInfo mpdInfo = _segmentDowloader.GetSegmentsMetadata(content.DashUrl, content.ContentTransformStatusUpdatedBy.Value.ToString());

                string workingDirectory = ($"{content.Id.Value}/{content.ContentTransformStatusUpdatedBy.Value}/{ApplicationConstants.DownloadDirectoryNames.Working}");

                _logger.LogInformation($"Broadcast Blob Working Directory Path {workingDirectory} for content id: {content.Id.Value} command id {broadcastCommand.Id.Value}");

                string finalDirectory = ($"{content.Id.Value}/{broadcastCommand.Id.Value}/{ApplicationConstants.DownloadDirectoryNames.Final}");

                _logger.LogInformation($"Broadcast Blob Final Directory Path {finalDirectory} for content id: {content.Id.Value} command id {broadcastCommand.Id.Value}");

                await MoveTarAndMpdToFinal(mezzContainer, processedContainer, content, broadcastCommand, mpdInfo, workingDirectory, finalDirectory);

                _logger.LogInformation($"Moved child tars and mpd for content id: {content.Id.Value} command id {broadcastCommand.Id.Value}");

                await UpdateAndMoveIngestXmlToFinal(mezzContainer, processedContainer, content, broadcastCommand, mpdInfo, workingDirectory, finalDirectory);

                _logger.LogInformation($"Geneated Ingest Xml for content id: {content.Id.Value} command id {broadcastCommand.Id.Value}");

                string tarfileName = $"{broadcastCommand.Id.Value}.tar";

                string tarfilePath = $"{content.Id.Value}/{broadcastCommand.Id.Value}/{tarfileName}";

                Tuple<long,string> infoData = await _tarGenerator.CreateTar(processedContainer, tarfilePath, tarfileName, finalDirectory, false);

                _logger.LogInformation($"Broadcast TAR file generated at {tarfilePath} - {infoData.Item1} for content id: {content.Id.Value} command id {broadcastCommand.Id.Value}");
                
                if(_appSettings.PerformCopyToBroadcastStorage)
                {
                    long timeElapsed = await CopyTarToBroadcastStorage(content.Id.Value, broadcastCommand.Id.Value,processedContainer, tarfilePath, tarfileName);

                    _logger.LogInformation($"Broadcast TAR file Copied to broadcast storage for content id: {content.Id.Value} command id {broadcastCommand.Id.Value}. Time elapsed {timeElapsed}");
                }
                else
                {
                    _logger.LogInformation($"Not copying on Broadcast storage as it is configured to false.");
                }

            }
            catch (Exception ex)
            {
                errorMessage = $"Failed to broadcast for content {content.ContentId.Value} Command {broadcastCommand.Id.Value} Exception {ex.Message}";

                broadcastCommand.FailureDetails.Add(errorMessage);

                _logger.LogError(ex, $"{errorMessage}");
            }
        }

        /// <summary>
        /// Copies the final TAR file to broadcast partner's storage
        /// </summary>
        /// <param name="processedContainer"></param>
        /// <param name="tarfilePath"></param>
        /// <returns></returns>
        private async Task<long> CopyTarToBroadcastStorage(Guid contentId, Guid commandId, BlobContainerClient processedContainer, string tarfilePath, string tarFileName)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            BlobContainerClient broadcastContainer = this._broadcastServiceClient.GetBlobContainerClient(_appSettings.BroadcastStorageContainerName);

            BlockBlobClient sourceBlob = processedContainer.GetBlockBlobClient(tarfilePath);

            BlockBlobClient targetBlob = broadcastContainer.GetBlockBlobClient(tarFileName);

            string blobSasUrl = EventHandlingUtilities.GetServiceSasUriForBlob(processedContainer.GetBlobClient(tarfilePath),
                                                             ApplicationConstants.StorageContainerPolicyNames.ProcessedReadOnly,
                                                             _appSettings.SASTokenExpiryToCopyContentInMts);

            await EventHandlingUtilities.CopyLargeBlob(_logger, sourceBlob, targetBlob,contentId,commandId, blobSasUrl);

            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;

        }

        /// <summary>
        /// Upla
        /// </summary>
        /// <param name="content"></param>
        /// <param name="broadcastCommand"></param>
        /// <returns></returns>
        private async Task UpdateAndMoveIngestXmlToFinal(   BlobContainerClient mezzContainer,
                                                            BlobContainerClient processedContainer,
                                                            Content content, 
                                                            ContentCommand broadcastCommand,
                                                            MpdInfo mpdInfo,
                                                            string workingDirectory,
                                                            string finalDirectory)
        {

            string workingXmlFilePath = $"{workingDirectory}/{string.Format(_appSettings.IngestFileName, content.ContentTransformStatusUpdatedBy.Value)}";

            string workingXmlFileContent = await EventHandlingUtilities.GetBlobContent(mezzContainer, workingXmlFilePath);
            
            string finalXmlFileContent = ReplaceTokenInXmlString(workingXmlFileContent, mpdInfo, content, broadcastCommand);

            string finalXmlFilePath = $"{finalDirectory}/{string.Format(_appSettings.IngestFileName, broadcastCommand.Id.Value)}";

            await EventHandlingUtilities.UploadBlob(processedContainer, finalXmlFilePath, finalXmlFileContent);

            _logger.LogInformation($"Moved Final Xml at {finalXmlFilePath} for content id: {content.Id.Value} command id {broadcastCommand.Id.Value}");



        }

        /// <summary>
        /// Move child tars and mpd to processed
        /// </summary>
        /// <param name="mezzContainer"></param>
        /// <param name="processedContainer"></param>
        /// <param name="content"></param>
        /// <param name="broadcastCommand"></param>
        /// <param name="mpdInfo"></param>
        /// <param name="workingDirectory"></param>
        /// <param name="finalDirectory"></param>
        /// <returns></returns>
        private async Task MoveTarAndMpdToFinal(BlobContainerClient mezzContainer,
                                                BlobContainerClient processedContainer,
                                                Content content,
                                                ContentCommand broadcastCommand,
                                                MpdInfo mpdInfo, 
                                                string workingDirectory,
                                                string finalDirectory)
        {
            BlockBlobClient sourceBlob;

            BlockBlobClient targetBlob;

            foreach (AdaptiveSetInfo adaptiveSet in mpdInfo.AdaptiveSets)
            {
                string sourceTar = $"{workingDirectory}/{adaptiveSet.DirectoryName}.tar";

                sourceBlob = mezzContainer.GetBlockBlobClient(sourceTar);

                string finalTar = $"{finalDirectory}/{broadcastCommand.Id.Value}_{adaptiveSet.DirectoryName}.tar";

                adaptiveSet.FinalPath = finalTar;

                targetBlob = processedContainer.GetBlockBlobClient(adaptiveSet.FinalPath);

                await EventHandlingUtilities.CopyBlob(_logger, sourceBlob, targetBlob,content.Id.Value,broadcastCommand.Id.Value);

                _logger.LogInformation($"Moved file from {sourceTar} to {adaptiveSet.FinalPath} for content id: {content.Id.Value} command id {broadcastCommand.Id.Value}");
            }

            string mpdPath = $"{workingDirectory}/{mpdInfo.MpdName}";

            mpdInfo.FinalMpdPath = $"{finalDirectory}/{broadcastCommand.Id.Value}.mpd";

            sourceBlob = mezzContainer.GetBlockBlobClient(mpdPath);

            targetBlob = processedContainer.GetBlockBlobClient(mpdInfo.FinalMpdPath);

            await EventHandlingUtilities.CopyBlob(_logger, sourceBlob, targetBlob,content.Id.Value,broadcastCommand.Id.Value);

            _logger.LogInformation($"Copied file from {mpdPath} to {mpdInfo.FinalMpdPath} for content id: {content.Id.Value} command id {broadcastCommand.Id.Value}");

        }


        /// <summary>
        /// Replaces Left Token
        /// </summary>
        /// <param name="xmlContent"></param>
        /// <param name="segmentInfo"></param>
        /// <param name="content"></param>
        /// <param name="broadcastCommand"></param>
        /// <returns></returns>
        private string ReplaceTokenInXmlString( string xmlContent,
                                                MpdInfo segmentInfo,
                                                Content content,
                                                ContentCommand broadcastCommand)
        {
            AdaptiveSetInfo audioSet = segmentInfo.AdaptiveSets.Where(audio => (audio.Type == ApplicationConstants.AdaptiveSetTypes.Audio)).FirstOrDefault();

            xmlContent = xmlContent.Replace(ApplicationConstants.XMLTokens.AUDIO_TAR, audioSet.FinalPath.Split('/').Last());

            xmlContent = xmlContent.Replace(ApplicationConstants.XMLTokens.AUDIO_TAR_FOLDER_NAME, audioSet.DirectoryName);

            AdaptiveSetInfo videoSet = segmentInfo.AdaptiveSets.Where(audio => (audio.Type == ApplicationConstants.AdaptiveSetTypes.Video)).FirstOrDefault();

            xmlContent = xmlContent.Replace(ApplicationConstants.XMLTokens.VIDEO_TAR, videoSet.FinalPath.Split('/').Last());

            xmlContent = xmlContent.Replace(ApplicationConstants.XMLTokens.VIDEO_TAR_FOLDER_NAME, videoSet.DirectoryName);

            xmlContent = xmlContent.Replace(ApplicationConstants.XMLTokens.MPD_FILE, segmentInfo.FinalMpdPath.Split('/').Last());

            xmlContent = xmlContent.Replace(ApplicationConstants.XMLTokens.COMMAND_ID, broadcastCommand.Id.Value.ToString());

            xmlContent = xmlContent.Replace(ApplicationConstants.XMLTokens.START_DATE, broadcastCommand.BroadcastRequest.StartDate.ToString(ApplicationConstants.BroadcastDateFormat));

            xmlContent = xmlContent.Replace(ApplicationConstants.XMLTokens.END_DATE, broadcastCommand.BroadcastRequest.EndDate.ToString(ApplicationConstants.BroadcastDateFormat));

            string filters = string.Join(string.Empty, broadcastCommand.BroadcastRequest.Filters.Select(item => "<filter>" + item + "</filter>"));

            xmlContent = xmlContent.Replace(ApplicationConstants.XMLTokens.FILTERS, filters);

            xmlContent = xmlContent.Replace(ApplicationConstants.XMLTokens.UNIQUE_ID, $"{broadcastCommand.Id.Value}");

            return xmlContent;
        }


        /// <summary>
        /// Update Failed Status
        /// </summary>
        /// <param name="content"></param>
        /// <param name="broadcastCommand"></param>
        /// <returns></returns>

        private async Task UpdateFailedStatus(Content content, ContentCommand broadcastCommand)
        {
            content.ContentBroadcastStatus = ContentBroadcastStatus.BroadcastFailed;

            broadcastCommand.CommandStatus = CommandStatus.Failed;

            await UpdateStatus(content, broadcastCommand);
        }

        /// <summary>
        /// Update In Progress String
        /// </summary>
        /// <param name="content"></param>
        /// <param name="broadcastCommand"></param>
        /// <returns></returns>
        private async Task UpdateInProgressStatus(Content content, ContentCommand broadcastCommand)
        {
            content.ContentBroadcastStatus = ContentBroadcastStatus.BroadcastTarPushed;

            broadcastCommand.CommandStatus = CommandStatus.InProgress;

            await UpdateStatus(content, broadcastCommand);
        }

        /// <summary>
        /// Update Status
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

        /// <summary>
        /// Populate command object with required attributes
        /// </summary>
        /// <param name="contentCommand"></param>
        private void PopulateContentCommand(ContentCommand contentCommand, DateTime currentDateTime)
        {
            contentCommand.Id = Guid.NewGuid();
            contentCommand.CommandType = CommandType.BroadcastContent;
            contentCommand.Type = ContentContainerType.Command;
            contentCommand.CommandStatus = CommandStatus.InProgress;
            contentCommand.CreatedDate = currentDateTime;
            contentCommand.ModifiedDate = currentDateTime;
            contentCommand.FailureDetails = new List<string>();

            CommandExecutionDetails commandExecutionDetails = new CommandExecutionDetails();
            
            commandExecutionDetails.ContentBroadcastStatus = ContentBroadcastStatus.BroadcastInProgress;
            
            commandExecutionDetails.EventDateTime = currentDateTime;

            contentCommand.ExecutionDetails.Add(commandExecutionDetails);
        }
    }
}
