using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using blendnet.common.dto.cms;
using Microsoft.Azure.Management.Media;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// Creates the AzureMediaServicesClient object based on the credentials
        /// supplied in local configuration file.
        /// </summary>
        /// <param name="config">The param is of type ConfigWrapper. This class reads values from local configuration file.</param>
        /// <returns></returns>
        public static async Task<IAzureMediaServicesClient> CreateMediaServicesClientAsync(AppSettings appSettings)
        {
            var credentials = await GetCredentialsAsync(appSettings);

            return new AzureMediaServicesClient(new Uri(appSettings.AmsArmEndPoint), credentials)
            {
                SubscriptionId = appSettings.AmsSubscriptionId
            };
        }

        /// <summary>
        /// Create the ServiceClientCredentials object based on the credentials supplied in local configuration file.
        /// </summary>
        private static async Task<ServiceClientCredentials> GetCredentialsAsync(AppSettings appSettings)
        {
            ClientCredential clientCredential = new ClientCredential(appSettings.AmsClientId, appSettings.AmsClientSecret);

            return await ApplicationTokenProvider.LoginSilentAsync(appSettings.AmsTenantId, clientCredential, ActiveDirectoryServiceSettings.Azure);
        }
    }
}
