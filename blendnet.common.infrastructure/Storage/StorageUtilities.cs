using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using blendnet.common.dto;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace blendnet.common.infrastructure.Storage
{
    public static class StorageUtilities
    {
        /// <summary>
        /// Create Container Policy
        /// </summary>
        /// <param name="blobServiceClient"></param>
        /// <param name="containerName"></param>
        /// <param name="policyName"></param>
        /// <param name="policyPermissions"></param>
        /// <returns></returns>
        public static async Task CreateContainerPolicy(BlobServiceClient blobServiceClient,
                                                string containerName, 
                                                string policyName, 
                                                string policyPermissions)
        {
            Dictionary<string, string> policies = new Dictionary<string, string>();

            policies.Add(policyName, policyPermissions);

            await CreateContainerPolicy(blobServiceClient,containerName, policies);
        }

        /// <summary>
        /// Create Container Policy
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="policyDetails"></param>
        /// <returns></returns>
        public static async Task CreateContainerPolicy( BlobServiceClient blobServiceClient, 
                                                        string containerName, 
                                                        Dictionary<string, string> policyDetails)
        {
            List<BlobSignedIdentifier> signedIdentifiers = new List<BlobSignedIdentifier>();

            BlobSignedIdentifier blobSignedIdentifier;

            foreach (KeyValuePair<string, string> entry in policyDetails)
            {
                blobSignedIdentifier = new BlobSignedIdentifier()
                {
                    Id = entry.Key,
                    AccessPolicy = new BlobAccessPolicy
                    {
                        Permissions = entry.Value
                    }
                };

                signedIdentifiers.Add(blobSignedIdentifier);
            }

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Set the container's access policy.
            await containerClient.SetAccessPolicyAsync(permissions: signedIdentifiers);
        }

        /// <summary>
        /// Uploads user data to blob
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <param name="userIncentiveEvents"></param>
        /// <returns></returns>
        public static async Task UploadUserDataToBlob(BlobServiceClient blobServiceClient,
                                                string containerName,
                                                string filePath,
                                                string fileContent,
                                                ILogger logger)
        {
            
            //check if user container exists or not. If not create one and also create an access policy
            bool containerExists = await blobServiceClient.GetBlobContainerClient(containerName).ExistsAsync();

            if (!containerExists)
            {
                try
                {
                    BlobContainerInfo containerInfo = await blobServiceClient.GetBlobContainerClient(containerName).CreateAsync(PublicAccessType.None);

                    //Create ReadOnly User Policy
                    await CreateContainerPolicy(blobServiceClient,
                                                                containerName,
                                                                ApplicationConstants.StorageContainerPolicyNames.UserDataReadOnly,
                                                                ApplicationConstants.Policy.ReadOnlyPolicyPermissions);
                }
                catch (RequestFailedException ex) when (ex.Status == (int)System.Net.HttpStatusCode.Conflict)
                {
                    logger.LogWarning($"Conflict occurred while creating container. Container Name {containerName}. StackTrace : {ex}");
                }
            }

            BlobContainerClient userDataContainer = blobServiceClient.GetBlobContainerClient(containerName);

            BlobClient blobClient = userDataContainer.GetBlobClient(filePath);

            //upload the data to blob
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent)))
            {
                await blobClient.UploadAsync(stream,overwrite:true);
            }
        }

        /// <summary>
        /// Get SAS URL
        /// </summary>
        /// <param name="blobClient"></param>
        /// <param name="identifier"></param>
        /// <param name="expiryMinutes"></param>
        /// <returns></returns>
        public static string GetServiceSasUriForBlob(BlobClient blobClient, 
                                                    string identifier, 
                                                    DateTimeOffset expiresOn)
        {
            // Create a SAS token that's valid till the given expiry.
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                BlobName = blobClient.Name,
                Identifier = identifier,
                Resource = "b",
                ExpiresOn = expiresOn
            };

            Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

            return sasUri.AbsoluteUri;
        }
    }
}

