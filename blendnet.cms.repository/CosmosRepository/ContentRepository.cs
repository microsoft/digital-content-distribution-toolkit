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
using blendnet.common.dto.Cms;
using System.Net;

namespace blendnet.cms.repository.CosmosRepository
{
    /// <summary>
    /// Content Repository
    /// </summary>
    public class ContentRepository : IContentRepository
    {
        private Container _container;
        AppSettings _appSettings;
        BlobServiceClient _cmsBlobServiceClient;
        BlobServiceClient _cmsCdnBlobServiceClient;

        public ContentRepository(CosmosClient dbClient,
                                IOptionsMonitor<AppSettings> optionsMonitor,
                                IAzureClientFactory<BlobServiceClient> blobClientFactory)
        {
            _appSettings = optionsMonitor.CurrentValue;

            _cmsBlobServiceClient = blobClientFactory.CreateClient(ApplicationConstants.StorageInstanceNames.CMSStorage);

            _cmsCdnBlobServiceClient = blobClientFactory.CreateClient(ApplicationConstants.StorageInstanceNames.CMSCDNStorage);

            this._container = dbClient.GetContainer(_appSettings.DatabaseName,ApplicationConstants.CosmosContainers.Content);    
        }
        /// <summary>
        /// Delete Content
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        public async Task<int> DeleteContent(Guid contentId)
        {
            try
            {
                var response = await this._container.DeleteItemAsync<Content>(contentId.ToString(), new PartitionKey(contentId.ToString()));

                return (int)response.StatusCode;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (int)ex.StatusCode;
            }
        }

        /// <summary>
        /// Get Content by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Content> GetContentById(Guid contentId)
        {
            try
            {
                ItemResponse<Content> response = await this._container.ReadItemAsync<Content>(contentId.ToString(), new PartitionKey(contentId.ToString()));

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<ContentCommand> GetContentCommandById(Guid commandId , Guid contentId)
        {
            try
            {
                ItemResponse<ContentCommand> response = await this._container.ReadItemAsync<ContentCommand>(commandId.ToString(), new PartitionKey(contentId.ToString()));

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        /// <summary>
        /// Create content commad.
        /// Partition key is content id
        /// </summary>
        /// <param name="contentCommand"></param>
        /// <returns></returns>
        public async Task<Guid> CreateContentCommand(ContentCommand contentCommand)
        {
            await this._container.CreateItemAsync<ContentCommand>(contentCommand, new PartitionKey(contentCommand.ContentId.ToString()));

            return contentCommand.Id.Value;
        }

        /// <summary>
        /// Update Content by command id and content id
        /// </summary>
        /// <param name="updatedContent"></param>
        /// <returns></returns>
        public async Task<int> UpdateContentCommand(ContentCommand updatedContent)
        {
            try
            {
                var response = await this._container.ReplaceItemAsync<ContentCommand>(updatedContent,
                                                                                        updatedContent.Id.Value.ToString(),
                                                                                        new PartitionKey(updatedContent.ContentId.ToString()));

                return (int)response.StatusCode;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (int)ex.StatusCode;
            }

        }

        /// <summary>
        /// Update Content
        /// </summary>
        /// <param name="updatedContent"></param>
        /// <returns></returns>
        public async Task<int> UpdateContent(Content updatedContent)
        {
            try
            {
                var response = await this._container.ReplaceItemAsync<Content>(updatedContent,
                                                                                        updatedContent.Id.ToString(),
                                                                                        new PartitionKey(updatedContent.ContentId.ToString()));

                return (int)response.StatusCode;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (int)ex.StatusCode;
            }

        }

        /// <summary>
        /// Return the list of all the contents for the content provider id
        /// FAN OUT Query. Will look the performance and if reqd look for alternate
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        public async Task<List<Content>> GetContentByContentProviderId(Guid contentProviderId)
        {
            List<Content> contentList = new List<Content>();

            var queryString = $"SELECT * FROM c WHERE c.contentContainerType = @type AND c.contentProviderId = @contentProviderId";

            var queryDef = new QueryDefinition(queryString);

            queryDef.WithParameter("@type", ContentContainerType.Content);

            queryDef.WithParameter("@contentProviderId", contentProviderId);

            contentList = await ExtractDataFromQueryIterator<Content>(queryDef);

            return contentList;
        }

                /// <summary>
        /// Get Command by Content Id
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<List<ContentCommand>> GetCommandByContentId(Guid contentId, common.dto.Cms.CommandType commandType)
        {
            List<ContentCommand> contentList = new List<ContentCommand>();

            var queryString = $"SELECT * FROM c WHERE c.contentContainerType = @type AND c.contentId = @contentId AND c.commandType = @commandType";

            var queryDef = new QueryDefinition(queryString);

            queryDef.WithParameter("@type", ContentContainerType.Content);

            queryDef.WithParameter("@contentId", contentId);

            queryDef.WithParameter("@commandType", commandType);

            contentList = await ExtractDataFromQueryIterator<ContentCommand>(queryDef);

            return contentList;
        }

        /// <summary>
        /// Extract Data from Query Iterator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryDef"></param>
        /// <returns></returns>
        private async Task<List<T>> ExtractDataFromQueryIterator<T>(QueryDefinition queryDef)
        {
            List<T> returnList = new List<T>();

            var query = this._container.GetItemQueryIterator<T>(queryDef);

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                returnList.AddRange(response.ToList());
            }

            return returnList;
        }

        public async Task<bool> CreateContent(List<Content> contents,Guid contentProviderId)
        {
            // Condition to validate the input jsonfile
            if(await ValidateContents(contents,contentProviderId) == true)
            {
                foreach(Content content in contents)
                {
                    // Generate the required Ids
                    content.SetIdentifiers();

                    // Update the ContentUpload Status to UploadInProgress
                    content.ContentProviderId = contentProviderId;
                    content.ContentUploadStatus = ContentUploadStatus.UploadInProgress;

                    await this._container.CreateItemAsync<Content>(content,
                                                            new PartitionKey(content.ContentId.ToString()));
                }
                return true;      
            }
            else
            {
                return false;
            }
        }

        private  async Task<bool> ValidateContents(List<Content> contents,Guid contentProviderId)
        {
            if(await ContentIdCheck(contents) == true)
            {
                if (await FileExists(contents,contentProviderId) == true)
                {
                    return true;
                } 
            }
            return false;

        }

        private async Task<bool> FileExists(List<Content> contents,Guid contentProviderId)
        {
            var containerName = contentProviderId+ApplicationConstants.StorageContainerSuffix.Cdn;

            var rawcontainerName = contentProviderId+ApplicationConstants.StorageContainerSuffix.Raw;

            // BlobContainerClient client = _cmsCdnBlobServiceClient.GetBlobContainerClient(containerName);

            BlobContainerClient rawClient = _cmsBlobServiceClient.GetBlobContainerClient(rawcontainerName);
            
            foreach(Content content in contents)
            {
                if(rawClient.GetBlobClient(content.MediaFileName).Exists()){
                    foreach(Attachment attachment in content.Attachments)
                        {         
                            if(attachment.Type is AttachmentType.Thumbnail){
                                var name = content.Title+"/"+attachment.Name;
                                if(rawClient.GetBlobClient(name).Exists()){
                                    continue;
                                }
                                else{
                                    return false;
                                }
                            }
                            else{
                                if(rawClient.GetBlobClient(attachment.Name).Exists()){
                                    continue;
                                }
                                else{
                                    return false;
                                } 
                            }
                        }
                        }
                else{
                    return false;
                }
            }
            return true;
        }


        private async Task<bool> ContentIdCheck(List<Content> contents)
        {
            List<Content> dbcontents = new List<Content>();

            var queryString = $"SELECT * FROM content c";

            var queryDef = new QueryDefinition(queryString);

            dbcontents = await ExtractDataFromQueryIterator<Content>(queryDef);

            HashSet<string> dbcontentIds = new HashSet<string>();

            foreach(Content content in dbcontents)
            {
                dbcontentIds.Add(content.ContentProviderContentId);
            }

            HashSet<string> contentIds = new HashSet<string>();

            foreach(Content content in contents)
            {
                contentIds.Add(content.ContentProviderContentId);
            }
            
            // Duplicate ContentIds in the file
            if (contents.Count() != contentIds.Count())
            {
                return false;
            }
            
            HashSet<string> commonIds = new HashSet<string>(dbcontentIds) ;

            commonIds.IntersectWith(contentIds);
            if(commonIds.Count() == 0)
            {
                return true;
            }
            return false;
        }  
    }
}