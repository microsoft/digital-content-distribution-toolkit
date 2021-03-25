using blendnet.cms.repository.Interfaces;
using blendnet.common.dto;
using blendnet.common.dto.cms;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using System.Linq;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Azure.Storage.Blobs.Models;
using Azure;
using Microsoft.Extensions.Logging;

namespace blendnet.cms.repository.CosmosRepository
{
    /// <summary>
    /// Content Provider Repository
    /// </summary>
    public class ContentProviderRepository : IContentProviderRepository
    {
        private Container _container;
        private readonly ILogger _logger;
        
        AppSettings _appSettings;
        BlobServiceClient _cmsBlobServiceClient;
        
        public ContentProviderRepository(   CosmosClient dbClient, 
                                            IOptionsMonitor<AppSettings> optionsMonitor,
                                            ILogger<ContentProviderRepository> logger,
                                            IAzureClientFactory<BlobServiceClient> blobClientFactory)
        {
            _appSettings = optionsMonitor.CurrentValue;

            _logger = logger;

            _cmsBlobServiceClient = blobClientFactory.CreateClient(ApplicationConstants.StorageInstanceNames.CMSStorage);

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
            var containerName = $"{contentProviderId}{ApplicationConstants.StorageContainerSuffix.Raw}";

            BlobContainerClient client = _cmsBlobServiceClient.GetBlobContainerClient(containerName);
                
            SasTokenDto sasUri = GetServiceSasUriForContainer(  client,
                                                                ApplicationConstants.StorageContainerPolicyNames.RawReadWriteAll,
                                                                _appSettings.SasTokenExpiryForContentProviderInMts);

            return sasUri;
        }


        /// <summary>
        /// Get the SAS token based on the policy which was created while creating the container
        /// </summary>
        /// <param name="containerClient"></param>
        /// <param name="contentPolicyName"></param>
        /// <param name="expiryMinutes"></param>
        /// <returns></returns>
        private SasTokenDto GetServiceSasUriForContainer(   BlobContainerClient containerClient,
                                                            string contentPolicyName,
                                                            int expiryMinutes)
        {
            // Check whether this BlobContainerClient object has been authorized with Shared Key.
            if (containerClient.CanGenerateSasUri)
            {
                // Create a SAS token that's valid for one hour.
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = containerClient.Name,
                    Resource = "c",
                    Identifier = contentPolicyName,
                    ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes)
                };
                                               
                // Console.WriteLine(containerClient.CanGenerateSasUri); 
                Uri sasUri = containerClient.GenerateSasUri(sasBuilder);
                
                SasTokenDto token = new SasTokenDto();

                token.StorageAccount = containerClient.AccountName;
                
                token.ContainerName = containerClient.Name;
                
                token.PolicyName = contentPolicyName;
                
                token.SasUri = sasUri;
                
                token.ExpiryInMinutes = expiryMinutes;

                return token;
            }
            else
            {
                return null;
            }
        }
    }
}
