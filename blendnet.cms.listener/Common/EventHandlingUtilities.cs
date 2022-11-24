// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Azure;

namespace blendnet.cms.listener.IntegrationEventHandling
{
    public static class EventHandlingUtilities
    {
        /// <summary>
        /// Uploads blob
        /// </summary>
        /// <param name="containerClient"></param>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task UploadBlob(BlobContainerClient containerClient, string fileName, string content)
        {
            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            
            var byteContent = Encoding.UTF8.GetBytes(content);

            // Open the file and upload its data
            using (MemoryStream uploadFileStream = new MemoryStream(byteContent))
            {
                await blobClient.UploadAsync(uploadFileStream, true);

                uploadFileStream.Close();
            }
                
        }

        /// <summary>
        /// Uploads Blob
        /// </summary>
        /// <param name="containerClient"></param>
        /// <param name="fileName"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task UploadBlob(BlobContainerClient containerClient, string fileName, MemoryStream stream)
        {
            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            // Open the file and upload its data
            await blobClient.UploadAsync(stream, true);

        }

        /// <summary>
        /// Returns the file content from BLOB
        /// </summary>
        /// <param name="containerClient"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<string> GetBlobContent(BlobContainerClient containerClient, string fileName)
        {
            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            BlobDownloadInfo downloadInfo = await blobClient.DownloadAsync();

            var content = downloadInfo.Content;

            string fileContent = string.Empty;

            using (var streamReader = new StreamReader(content))
            {

                fileContent = streamReader.ReadToEnd();
            }

            return fileContent;

        }

        /// <summary>
        /// Returns the check sum
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string GetChecksum(ILogger logger, string fileName, MemoryStream stream)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            string checksum = string.Empty;

            using (var sha = SHA256.Create())
            {
                byte[] checksumBytes = sha.ComputeHash(stream);

                checksum = BitConverter.ToString(checksumBytes).Replace("-", String.Empty).ToLowerInvariant();
            }

            stopwatch.Stop();

            logger.LogInformation($"Generated Checksum for {fileName}. Duration Milisecond : {stopwatch.ElapsedMilliseconds}");

            return checksum;
        }

        /// <summary>
        /// Copy Blob
        /// </summary>
        /// <param name="sourceBlob"></param>
        /// <param name="targetBlob"></param>
        /// <returns></returns>
        public static async Task CopyBlob(  ILogger logger,
                                            BlockBlobClient sourceBlob, 
                                            BlockBlobClient targetBlob,
                                            Guid contentId,
                                            Guid commandId,
                                            string sourceBlobUrl = "")
        {
            BlobLeaseClient lease = null;

            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                // Lease the source blob for the copy operation to prevent another client from modifying it.
                lease = sourceBlob.GetBlobLeaseClient();

                // Specifying -1 for the lease interval creates an infinite lease.
                await lease.AcquireAsync(TimeSpan.FromSeconds(-1));

                CopyFromUriOperation copyFromUriOperation;

                Uri copyUri;

                if (string.IsNullOrEmpty(sourceBlobUrl))
                {
                    copyUri = sourceBlob.Uri;

                    // Start the copy operation.
                    copyFromUriOperation = await targetBlob.StartCopyFromUriAsync(copyUri);
                }
                else
                {
                    copyUri = new Uri(sourceBlobUrl);

                    copyFromUriOperation = await targetBlob.StartCopyFromUriAsync(copyUri);
                }

                //wait for the operation to complete
                await copyFromUriOperation.WaitForCompletionAsync();

                stopwatch.Stop();

                logger.LogInformation($"Copied file {copyUri} to {targetBlob.Uri}. Content Id : {contentId} Command Id {commandId} Duration Milisecond : {stopwatch.ElapsedMilliseconds}");

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

        /// <summary>
        /// For large blob, uploading blocks on different threads is more efficient.
        /// Regular copy for 1.2 GB across private link is taking more than 20 mts.
        /// Using the block copy it comes down to 4 mts.
        /// The same is followed by AzCopy/Azure Storage Explorer internally
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="sourceBlob"></param>
        /// <param name="targetBlob"></param>
        /// <param name="contentId"></param>
        /// <param name="commandId"></param>
        /// <param name="sourceBlobUrl"></param>
        /// <returns></returns>
        public static async Task CopyLargeBlob(ILogger logger,
                                          BlockBlobClient sourceBlob,
                                          BlockBlobClient targetBlob,
                                          Guid contentId,
                                          Guid commandId,
                                          string sourceBlobUrl)
        {

            BlockList blockListResponse = await sourceBlob.GetBlockListAsync(BlockListTypes.Committed);

            if (blockListResponse.CommittedBlocks == null ||
                blockListResponse.CommittedBlocks.Count() <= 0)
            {
                logger.LogInformation($"No Committed blobs found for Content Id : {contentId} Command Id {commandId}. Hence, TAR to be copied by regular copy.");

                await CopyBlob(logger, sourceBlob, targetBlob, contentId, commandId, sourceBlobUrl);

            }
            else
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                // A list of tasks that are currently executing
                List<Task> runningTasks = new List<Task>();

                List<string> committedBlocks = new List<string>();

                long offset = 0;

                int maxWorkers = 10;

                foreach (BlobBlock block in blockListResponse.CommittedBlocks)
                {
                    string newBlockName = $"block-{Guid.NewGuid()}";

                    var bytes = Encoding.UTF8.GetBytes(newBlockName);

                    string blockId = Convert.ToBase64String(bytes);

                    committedBlocks.Add(blockId);

                    StageBlockFromUriOptions stageBlockFromUriOptions = new StageBlockFromUriOptions();

                    stageBlockFromUriOptions.SourceRange = new HttpRange(offset, block.Size);

                    Task task = targetBlob.StageBlockFromUriAsync(new Uri(sourceBlobUrl), blockId, stageBlockFromUriOptions);

                    offset += block.Size;

                    runningTasks.Add(task);

                    // Check if any of our tasks are still busy
                    if (runningTasks.Count >= maxWorkers)
                    {
                        await Task.WhenAny(runningTasks).ConfigureAwait(false);

                        // Clear any completed blocks from the task list
                        for (int i = 0; i < runningTasks.Count; i++)
                        {
                            Task runningTask = runningTasks[i];

                            if (!runningTask.IsCompleted)
                            {
                                continue;
                            }

                            await runningTask.ConfigureAwait(false);

                            runningTasks.RemoveAt(i);

                            i--;
                        }
                    }
                }

                // Wait for all our tasks
                await Task.WhenAll(runningTasks).ConfigureAwait(false);

                // Commit block list
                await targetBlob.CommitBlockListAsync(committedBlocks);

                stopwatch.Stop();

                logger.LogInformation($"Copied Large file {sourceBlob.Uri} to {targetBlob.Uri}. Content Id : {contentId} Command Id {commandId} Duration Milisecond : {stopwatch.ElapsedMilliseconds}");
            }
            
        }

        /// <summary>
        /// Generates SAS URI for Blob
        /// https://docs.microsoft.com/en-us/azure/storage/common/storage-stored-access-policy-define-dotnet?tabs=dotnet
        /// </summary>
        /// <param name="blobClient"></param>
        /// <param name="identifier"></param>
        /// <param name="expiryMinutes"></param>
        /// <returns></returns>
        public static string GetServiceSasUriForBlob(BlobClient blobClient, string identifier, int expiryMinutes)
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
    }
}
