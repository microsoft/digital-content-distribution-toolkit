using blendnet.cms.repository.Interfaces;
using blendnet.common.dto;
using blendnet.common.dto.cms;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Linq;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Azure.Storage.Blobs.Models;
using Azure;

namespace blendnet.cms.repository.CosmosRepository
{
    /// <summary>
    /// Content Provider Repository
    /// </summary>
    public class ContentProviderRepository : IContentProviderRepository
    {
        private Container _container;
        
        AppSettings _appSettings;

        BlobServiceClient _blobServiceClient;
        
        public ContentProviderRepository(   CosmosClient dbClient, 
                                            IOptionsMonitor<AppSettings> optionsMonitor,
                                            BlobServiceClient blobServiceClient)
        {
            _appSettings = optionsMonitor.CurrentValue;

            _blobServiceClient = blobServiceClient;

            this._container = dbClient.GetContainer(_appSettings.DatabaseName,ApplicationConstants.CosmosContainers.ContentProvider);       
            
        }

        /// <summary>
        /// Creates the Content Provider
        /// </summary>
        /// <param name="contentProvider"></param>
        /// <returns></returns>
        public async Task<Guid> CreateContentProvider(ContentProviderDto contentProvider)
        {
            await this._container.CreateItemAsync<ContentProviderDto>(  contentProvider, 
                                                                        new PartitionKey(contentProvider.Id.Value.ToString()));

            return contentProvider.Id.Value;
        }

        /// <summary>
        /// Deletes content provider
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        public async Task<int> DeleteContentProvider(Guid contentProviderId)
        {
            try
            {
                var response = await this._container.DeleteItemAsync<ContentProviderDto>(contentProviderId.ToString(), new PartitionKey(contentProviderId.ToString()));

                return (int)response.StatusCode;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (int)ex.StatusCode;
            }
        }

        /// <summary>
        /// Get ContentProvider By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ContentProviderDto> GetContentProviderById(Guid id)
        {
            try
            {
                ItemResponse<ContentProviderDto> response = await this._container.ReadItemAsync<ContentProviderDto>(id.ToString(), new PartitionKey(id.ToString()));
                
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        /// <summary>
        /// Returns all the content providers
        /// </summary>
        /// <returns></returns>
        public async Task<List<ContentProviderDto>> GetContentProviders()
        {
            var query = this._container
                            .GetItemLinqQueryable<ContentProviderDto>(allowSynchronousQueryExecution: true)
                            .ToList<ContentProviderDto>();

            return query;
        }

        /// <summary>
        /// Update Content Provider
        /// </summary>
        /// <param name="updatedContentProvider"></param>
        /// <returns></returns>
        public async Task<int> UpdateContentProvider(ContentProviderDto updatedContentProvider)
        {
            try
            {
                var response = await this._container.ReplaceItemAsync<ContentProviderDto>(updatedContentProvider,
                                                                                        updatedContentProvider.Id.Value.ToString(),
                                                                                        new PartitionKey(updatedContentProvider.Id.Value.ToString()));

                return (int)response.StatusCode;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (int)ex.StatusCode;
            }

        }
        /// <summary>
        /// Generate Sas Token
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        public async Task<SasTokenDto> GenerateSaSToken(Guid contentProviderId)
        {    
            var containerName = contentProviderId+ApplicationConstants.ContainerConstants.ContainerSuffix;

            // BlobContainerClient _client = _blobServiceClient.GetBlobContainerClient(containerName);

            BlobContainerClient client = new BlobContainerClient(_appSettings.CMSStorageConnectionString,containerName);
            if(client.Exists())
            {
                await CreateStoredAccessPolicyAsync(containerName);

                SasTokenDto sasUri = GetServiceSasUriForContainer(client,_appSettings.PolicyName);

                return sasUri;
            }
            return null;

        }


        private SasTokenDto GetServiceSasUriForContainer(BlobContainerClient containerClient,
                                                string storedPolicyName = null)
        {
            // Check whether this BlobContainerClient object has been authorized with Shared Key.
            if (containerClient.CanGenerateSasUri)
            {
                // Create a SAS token that's valid for one hour.
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = containerClient.Name,
                    Resource = "c"
                };
                sasBuilder.Identifier = storedPolicyName;
                // Console.WriteLine(containerClient.CanGenerateSasUri); 
                Uri sasUri = containerClient.GenerateSasUri(sasBuilder);
                

                SasTokenDto token = new SasTokenDto();
                token.storageAccount = _appSettings.StorageAccount;
                token.containerName = containerClient.Name;
                token.policyName =  _appSettings.PolicyName;
                token.sasUri = sasUri;
                token.expiryInHours = ApplicationConstants.SaSToken.expiryInHours;

                return token;
            }
            else
            {
                return null;
            }
        }

        private async Task CreateStoredAccessPolicyAsync(string containerName)
        {
            // string connectionString = _appSettings.CMSStorageConnectionString;
            // Use the connection string to authorize the operation to create the access policy.
            // Azure AD does not support the Set Container ACL operation that creates the policy.
            BlobContainerClient containerClient =  _blobServiceClient.GetBlobContainerClient(containerName);

            // BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);
 
            // Create one or more stored access policies.
            List<BlobSignedIdentifier> signedIdentifiers = new List<BlobSignedIdentifier>
            {
                new BlobSignedIdentifier
                {
                    Id = _appSettings.PolicyName,
                    AccessPolicy = new BlobAccessPolicy
                    {
                        // StartsOn = DateTimeOffset.UtcNow.AddHours(-1),
                        // ExpiresOn = DateTimeOffset.UtcNow.AddDays(1),
                        Permissions = ApplicationConstants.Policy.PolicyPermissions
                    }      
                }
            };
            // Set the container's access policy.
            await containerClient.SetAccessPolicyAsync(permissions: signedIdentifiers);
        }

    }
}
