using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.Azure.Management.Media;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.testutility
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


        public static async Task UploadBlob(BlobContainerClient containerClient, string fileName, MemoryStream stream)
        {
            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            // Open the file and upload its data
            await blobClient.UploadAsync(stream, true);

        }

        /// <summary>
        /// Copy Blob
        /// </summary>
        /// <param name="sourceBlob"></param>
        /// <param name="targetBlob"></param>
        /// <returns></returns>
        public static async Task CopyBlob(BlockBlobClient sourceBlob, BlockBlobClient targetBlob, string sourceBlobUrl = "")
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

        /// <summary>
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

        public static string GetChecksum(MemoryStream stream)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            string checksum = string.Empty;

            using (var sha = SHA256.Create())
            {
                byte[] checksumBytes = sha.ComputeHash(stream);

                checksum = BitConverter.ToString(checksumBytes).Replace("-", String.Empty).ToLowerInvariant();
            }

            stopwatch.Stop();

            return checksum;
        }


    }
}
