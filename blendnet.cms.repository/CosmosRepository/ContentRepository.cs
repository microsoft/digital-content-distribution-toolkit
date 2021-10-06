using Azure.Storage.Blobs;
using blendnet.cms.repository.Interfaces;
using blendnet.common.dto;
using blendnet.common.dto.cms;
using blendnet.common.dto.Cms;
using blendnet.common.dto.Common;
using blendnet.common.infrastructure.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.cms.repository.CosmosRepository
{
    /// <summary>
    /// Content Repository
    /// </summary>
    public class ContentRepository : IContentRepository
    {
        private Container _container;
        private readonly ILogger _logger;
        AppSettings _appSettings;

        public ContentRepository(CosmosClient dbClient,
                                IOptionsMonitor<AppSettings> optionsMonitor,
                                ILogger<ContentProviderRepository> logger)
        {
            _appSettings = optionsMonitor.CurrentValue;

            _logger = logger;

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

        /// <summary>
        /// Returns the Content Command By Id
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="contentId"></param>
        /// <returns></returns>
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
        /// Get the content command by command id. 
        /// Fan out query. Use rarely
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns></returns>
        public async Task<ContentCommand> GetContentCommandById(Guid commandId)
        {
            var queryString = "select * from c where c.type = @type and c.id = @id";

            var queryDef = new QueryDefinition(queryString)
                                .WithParameter("@type", ContentContainerType.Command)
                                .WithParameter("@id", commandId);

            var commands = await this._container.ExtractDataFromQueryIterator<ContentCommand>(queryDef);
            
            var command = commands.FirstOrDefault();
            
            return command;
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
        /// Return the list of all the contents for the content provider id
        /// FAN OUT Query. Will look the performance and if reqd look for alternate
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        public async Task<List<Content>> GetContentByContentProviderId(Guid contentProviderId, ContentStatusFilter contentStatusFilter,bool activeOnly)
        {
            QueryDefinition queryDef = GetQueryDefinitionWithStatusFilter(contentProviderId, contentStatusFilter,activeOnly);

            List<Content> contentList = await this._container.ExtractDataFromQueryIterator<Content>(queryDef);

            return contentList;
        }


       


        /// <summary>
        /// Get content by provider id, status filter and continuation token
        /// </summary>
        /// <param name="contentProviderId"> Provider id</param>
        /// <param name="contentStatusFilter">Lists of Upload status, Transform status and broadcast status</param>
        /// <param name="continuationToken">continuation token to query</param>
        /// <returns>Content result which holds list of results and continuation token</returns>
        public async Task<ResultData<Content>> GetContentByContentProviderId(Guid contentProviderId, ContentStatusFilter contentStatusFilter, string continuationToken, bool activeOnly)
        {
            ResultData<Content> contentResult;
            
            QueryDefinition queryDef = GetQueryDefinitionWithStatusFilter(contentProviderId, contentStatusFilter,activeOnly);

            contentResult = await this._container.ExtractDataFromQueryIteratorWithToken<Content>(queryDef, continuationToken);

            return contentResult;
        }

        /// <summary>
        /// Returns the query based on given filter condition
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <param name="contentStatusFilter"></param>
        /// <param name="activeOnly"></param>
        /// <returns></returns>
        private QueryDefinition GetQueryDefinitionWithStatusFilter( Guid contentProviderId, 
                                                                    ContentStatusFilter contentStatusFilter, 
                                                                    bool activeOnly)
        {
            string queryString = $"SELECT * FROM c WHERE c.type = @type AND c.contentProviderId = @contentProviderId ";

            if (contentStatusFilter != null)
            {
                if (contentStatusFilter.IsContentUploadStatusesPresent)
                {
                    queryString = $" {queryString} AND ARRAY_CONTAINS(@uploadStatuses, c.contentUploadStatus)";
                }

                if (contentStatusFilter.IsContentTransformStatusesPresent)
                {
                    queryString = $" {queryString} AND ARRAY_CONTAINS(@transformStatuses, c.contentTransformStatus)";
                }

                if (contentStatusFilter.IsContentBroadcastStatusesPresent)
                {
                    queryString = $" {queryString} AND ARRAY_CONTAINS(@broadcastStatuses, c.contentBroadcastStatus)";
                }
            }

            if (activeOnly)
            {
                queryString = $" {queryString} AND c.isActive = true";
            }

            queryString = $" {queryString} ORDER BY c.modifiedDate desc";

            var queryDef = new QueryDefinition(queryString);

            queryDef.WithParameter("@type", ContentContainerType.Content);

            queryDef.WithParameter("@contentProviderId", contentProviderId);

            if (contentStatusFilter != null)
            {
                if (contentStatusFilter.IsContentUploadStatusesPresent)
                {
                    queryDef.WithParameter("@uploadStatuses", contentStatusFilter.ContentUploadStatuses);
                }

                if (contentStatusFilter.IsContentTransformStatusesPresent)
                {
                    queryDef.WithParameter("@transformStatuses", contentStatusFilter.ContentTransformStatuses);
                }

                if (contentStatusFilter.IsContentBroadcastStatusesPresent)
                {
                    queryDef.WithParameter("@broadcastStatuses", contentStatusFilter.ContentBroadcastStatuses);
                }
            }

            return queryDef;
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

            var queryString = $"SELECT * FROM c WHERE c.type = @type AND c.contentId = @contentId AND c.commandType = @commandType";

            var queryDef = new QueryDefinition(queryString);

            queryDef.WithParameter("@type", ContentContainerType.Content);

            queryDef.WithParameter("@contentId", contentId);

            queryDef.WithParameter("@commandType", commandType);

            contentList = await this._container.ExtractDataFromQueryIterator<ContentCommand>(queryDef);

            return contentList;
        }

        /// <summary>
        /// Get Content By Ids
        /// </summary>
        /// <param name="contentIds"></param>
        /// <returns></returns>
        public async Task<List<Content>> GetContentByIds(List<Guid> contentIds)
        {
            List<Content> contentList = new List<Content>();

            var queryString = $"SELECT * FROM c WHERE ARRAY_CONTAINS(@contentIds, c.contentId) AND c.type = @type";

            var queryDef = new QueryDefinition(queryString);

            queryDef.WithParameter("@type", ContentContainerType.Content);

            queryDef.WithParameter("@contentIds", contentIds);

            contentList = await this._container.ExtractDataFromQueryIterator<Content>(queryDef);

            return contentList;
        }

        /// <summary>
        /// Create Content
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<Guid> CreateContent(Content content)
        {
            content.ContentId = content.Id;

            await this._container.CreateItemAsync<Content>(content,new PartitionKey(content.ContentId.ToString()));

            return content.Id.Value;
        }
    }
}