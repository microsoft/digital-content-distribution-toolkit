using blendnet.cms.repository.Interfaces;
using blendnet.common.dto;
using blendnet.common.dto.Cms;
using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Net;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;

namespace blendnet.cms.api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ContentController  : ControllerBase
    {
        private readonly ILogger _logger;

        private IEventBus _eventBus;

        BlobServiceClient _cmsBlobServiceClient;

        private IContentRepository _contentRepository;

        public ContentController(IContentRepository contentRepository,
                                            ILogger<ContentController> logger,
                                            IEventBus eventBus,
                                            IAzureClientFactory<BlobServiceClient> blobClientFactory)
        {
            _contentRepository = contentRepository;

            _cmsBlobServiceClient = blobClientFactory.CreateClient(ApplicationConstants.StorageInstanceNames.CMSStorage);

            _logger = logger;

            _eventBus = eventBus;
        }

        #region Content Management Methods


        /// <summary>
        /// Get Content 
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        [HttpGet("{contentId:guid}", Name = nameof(GetContent))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<Content>> GetContent(Guid contentId)
        {
            var content = await _contentRepository.GetContentById(contentId);

            if (content != default(Content))
            {
                return Ok(content);
            }
            else
            {
                return NotFound();
            }
        }


        /// <summary>
        /// Get Content By ContentProvider
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        [HttpGet("{contentProviderId:guid}/getcontents", Name = nameof(GetContentByContentProviderId))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<Content>> GetContentByContentProviderId(Guid contentProviderId)
        {
            List<Content> contentlist = await _contentRepository.GetContentByContentProviderId(contentProviderId);

            if (contentlist.Count()>0)
            {
                return Ok(contentlist);
            }
            else
            {
                return NoContent();
            }
        }


        /// <summary>
        /// Upload Contents
        /// </summary>
        /// <returns></returns>
        [HttpPost("{contentProviderId:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult> UploadContent(IFormFile file, Guid contentProviderId)
        {
            // Check for Json Format
            if(ValidateFileExtension(file) is false){
                    string ex = "File not found or not in Json format";
                    _logger.LogError(ex);                    
                    return BadRequest(ex);
            }

            // Convert Json to list of Contents.
            List<Content> contents = DeSerializeObjects(file);

            if(contents is null){
                string ex = "Improper JSON File";
                _logger.LogError(ex);   
                return BadRequest(ex);
            }

            // Check for duplicate Ids
            if(await ValidateDuplicateIdCheck(contents,contentProviderId) is false){
                    string ex = "Duplicate IDs found";
                    _logger.LogError(ex);                    
                    return BadRequest(ex);
            }

            // Check for File Existence on the raw container.
            if(await ValidateBlobExistence(contents,contentProviderId) is false){
                    string ex = "Blob doesn't exist";
                    _logger.LogError(ex);                    
                    return BadRequest(ex);
            }

            // Perform CreateContent and Push it to Service Bus
            if(await UploadContent(contents,contentProviderId) is false){
                string ex = "Upload failed";
                _logger.LogError(ex); 
                return BadRequest(ex);
            }
            return  Ok();
        }

        private async Task<bool> UploadContent(List<Content> contents, Guid contentProviderId)
        {
            foreach(Content content in contents){
                content.SetIdentifiers();
                content.ContentProviderId = contentProviderId;
                // Update the ContentUpload Status to UploadInProgress
                content.ContentUploadStatus = ContentUploadStatus.UploadSubmitted;

                Guid contentId = await _contentRepository.CreateContent(content);

                if(contentId !=  null){
                    ContentCommand contentCommand = new ContentCommand();
                    contentCommand.ContentId = contentId;
                    //publish the event
                    ContentUploadedIntegrationEvent contentUploadedIntegrationEvent = new ContentUploadedIntegrationEvent()
                    {
                        ContentUploadCommand = contentCommand,
                    };
                    await _eventBus.Publish(contentUploadedIntegrationEvent); 
                }
                else{
                    return false;
                }
            }
            return true;
        }


        private static bool ValidateFileExtension(IFormFile file)
        {
            string fileExt = System.IO.Path.GetExtension(file.FileName);
            if(fileExt.Equals(".json")){
                return true;
            }
            else
            {
                return false;
            }
        }

        private static List<Content> DeSerializeObjects(IFormFile file){

            using(StreamReader reader = new StreamReader(file.OpenReadStream()))
            {
                string text = reader.ReadToEnd();

                reader.Close();
                reader.Dispose();
                
                List<Content> contents = JsonConvert.DeserializeObject<List<Content>>(text);

                return contents;
            }
        }
        private async Task<bool> ValidateDuplicateIdCheck(List<Content> contents, Guid contentProviderId)
        {

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

            List<Content> dbcontents = await _contentRepository.GetContentByContentProviderId(contentProviderId);

            HashSet<string> dbcontentIds = new HashSet<string>();

            foreach(Content content in dbcontents)
            {
                dbcontentIds.Add(content.ContentProviderContentId);
            }
            
            HashSet<string> commonIds = new HashSet<string>(dbcontentIds) ;

            commonIds.IntersectWith(contentIds);
            if(commonIds.Count() == 0)
            {
                return true;
            }
            return false;
        } 

        private async Task<bool> ValidateBlobExistence(List<Content> contents,Guid contentProviderId)
        {
            var containerName = contentProviderId+ApplicationConstants.StorageContainerSuffix.Cdn;

            var rawcontainerName = contentProviderId+ApplicationConstants.StorageContainerSuffix.Raw;

            BlobContainerClient rawClient = _cmsBlobServiceClient.GetBlobContainerClient(rawcontainerName);

            List<String> Mediatypes = ApplicationConstants.SupportedFileFormats.mediaFormats;
                
            List<String> Thumbnailtypes = ApplicationConstants.SupportedFileFormats.ThumbnailFormats;
            
            foreach(Content content in contents)
            {
                
                BlobClient mediaClient = rawClient.GetBlobClient(content.MediaFileName);

                
                // string type = mediaClient.GetProperties()
                if(rawClient.GetBlobClient(content.MediaFileName).Exists() == false)
                {
                    return false;
                }

                string mediatype = FileFormat(content.MediaFileName);
                if(Mediatypes.Contains(mediatype)== false){
                    return false;
                }
                foreach(Attachment attachment in content.Attachments)
                {         
                    if(attachment.Type is AttachmentType.Thumbnail){

                        if(rawClient.GetBlobClient(attachment.Name).Exists() == false){
                            return false;
                        }
                        string imagetype = FileFormat(attachment.Name); 

                        if(Thumbnailtypes.Contains(imagetype)== false){
                            return false;
                        }   
                    }
                    else{
                        if(rawClient.GetBlobClient(attachment.Name).Exists() == false){
                            return false;
                        }
                        string imagetype = FileFormat(attachment.Name); 
                        if(Thumbnailtypes.Contains(imagetype)== false){
                            return false;
                        } 
                    }
                }
            }
            return true;
        } 

        private static string FileFormat(string Name){
            string format = Name.Split('.')[1]; 
            return format;
        }


        #endregion
    }
}

