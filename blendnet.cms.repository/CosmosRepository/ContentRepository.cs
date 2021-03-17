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

namespace blendnet.cms.repository.CosmosRepository
{
    /// <summary>
    /// Content Repository
    /// </summary>
    public class ContentRepository : IContentRepository
    {
        private Container _contentcontainer;
        
        AppSettings _appSettings;

        BlobServiceClient _blobServiceClient;

        public ContentRepository(CosmosClient dbClient,
                                IOptionsMonitor<AppSettings> optionsMonitor,
                                BlobServiceClient blobServiceClient)
        {
            _appSettings = optionsMonitor.CurrentValue;

            _blobServiceClient = blobServiceClient;

            this._contentcontainer = dbClient.GetContainer(_appSettings.DatabaseName,ApplicationConstants.CosmosContainers.Content);    
        }
        public Task<int> DeleteContent(Guid contentId)
        {
            throw new NotImplementedException();
        }

        public Task<Content> GetContentById(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Content>> GetContents()
        {
            var query = this._contentcontainer
                            .GetItemLinqQueryable<Content>(allowSynchronousQueryExecution:true)
                            .ToList<Content>();
            
            return query;
        }

        public Task<int> UpdateContent(Content updatedContent)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UploadContent(List<Content> contents)
        {
            // Condition to validate the input jsonfile
            if(ValidateJson(contents))
            {
                foreach(Content content in contents)
                {
                    // Generate the required Ids
                    content.SetIdentifiers();

                    // Update the ContentUpload Status to UploadInProgress
                    content.ContentUploadStatus = ContentUploadStatus.UploadInProgress;

                    Console.WriteLine(content.Id);
                    await this._contentcontainer.CreateItemAsync<Content>(content,
                                                            new PartitionKey(content.Id.ToString()));
                    
                }
            return true;            
            }
            else
            {
                return false;
            }

            
        }

        private static bool ValidateJson(List<Content> contents)
        {
            foreach(Content content in contents)
            {
                
            }   
            return true;
        }

    }
}