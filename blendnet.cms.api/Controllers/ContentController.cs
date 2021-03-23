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
            if(! ValidateFileExtension(file)){
                    string info = "File not found or not in Json format";
                    return BadRequest(info);
            }

            // Convert Json to list of Contents.
            var contents = ValidateJson(file);
            Type type = typeof(List<Content>); 
            // using GetType method 
            Type _contentType = contents.GetType();


            if( type != _contentType){
                return BadRequest(contents);
            }

            // Check for File Extensions on the raw container.
            var extErrorList = await ValidateFileExtensions(contents,contentProviderId);
            if(extErrorList.Count>0){
                    return BadRequest(extErrorList);
            }     

            // Check for duplicate Ids
            var dupIdList = await ValidateDuplicateIdCheck(contents,contentProviderId);
            if(dupIdList.Count>0 ){       
                    return BadRequest(dupIdList);
            }

            // Check for File Existence on the raw container.
            var fileErrorList = await ValidateBlobExistence(contents,contentProviderId);
            if(fileErrorList.Count>0){
                    return BadRequest(fileErrorList);
            }

            // Perform CreateContent and Push it to Service Bus
            await UploadContent(contents,contentProviderId);
            return  Ok();
        }

        /// <summary>
        /// Deletes the content id
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        [HttpDelete("{contentId:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<ActionResult> DeleteContentProvider(List<Guid> contentIds)
        {
            foreach(Guid contentId in contentIds)
            {
            int statusCode = await _contentRepository.DeleteContent(contentId);

            if (statusCode == (int)System.Net.HttpStatusCode.NoContent)
            {
                continue;  
            }
            else
            {
                return NotFound();
            }
            }
            return NoContent();
        }

        private async Task UploadContent(List<Content> contents, Guid contentProviderId)
        {
            foreach(Content content in contents)
            {
                content.SetIdentifiers();
                content.ContentProviderId = contentProviderId;
                // Update the ContentUpload Status to UploadInProgress
                content.ContentUploadStatus = ContentUploadStatus.UploadSubmitted;

                Guid contentId = await _contentRepository.CreateContent(content);

                    ContentCommand contentCommand = new ContentCommand();
                    contentCommand.ContentId = contentId;
                    //publish the event
                    ContentUploadedIntegrationEvent contentUploadedIntegrationEvent = new ContentUploadedIntegrationEvent()
                    {
                        ContentUploadCommand = contentCommand,
                    };
                    await _eventBus.Publish(contentUploadedIntegrationEvent); 
            }
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

        private dynamic ValidateJson(IFormFile file){

            string error  = string.Empty;
            try
            {
                StreamReader reader = new StreamReader(file.OpenReadStream());

                string text = reader.ReadToEnd();

                reader.Close();
                reader.Dispose();

                List<Content> contents = JsonConvert.DeserializeObject<List<Content>>(text);

                return contents;
            }
            catch (Exception ex)
            {
                error  = ex.Message;
                return error;
            }
        }

        private async Task<List<string>> ValidateDuplicateIdCheck(List<Content> contents, Guid contentProviderId)
        {
            List<string> errorList = new List<string>();

            HashSet<string> contentIds = new HashSet<string>();

            foreach(Content content in contents)
            {
                // contentIds.Add(content.ContentProviderContentId);
                if(!contentIds.Add(content.ContentProviderContentId))
                {
                    // Duplicate ContentIds in the file
                    string info = $"Content with Id {content.ContentProviderContentId} and Title{content.Title} is found multiple times in the uploaded file.";
                    errorList.Add(info);
                }
                
            }

            List<Content> dbcontents = await _contentRepository.GetContentByContentProviderId(contentProviderId);

            HashSet<string> dbcontentIds = new HashSet<string>();

            foreach(Content content in dbcontents)
            {
                dbcontentIds.Add(content.ContentProviderContentId);
            }
            
            HashSet<string> commonIds = new HashSet<string>(dbcontentIds) ;

            commonIds.IntersectWith(contentIds);
            foreach(string Id in commonIds)
            {
                
                string info = $"Content with Id {Id} and is already uploaded to the Database.";
                errorList.Add(info);

            }
            return errorList;
        }

        private async Task<List<string>> ValidateFileExtensions(List<Content> contents,Guid contentProviderId)
        {
            List<string> errorList = new List<string>();

            List<String> Mediatypes = ApplicationConstants.SupportedFileFormats.mediaFormats;
                
            List<String> Thumbnailtypes = ApplicationConstants.SupportedFileFormats.ThumbnailFormats;

            foreach(Content content in contents)
            {
                string mediatype = FileFormat(content.MediaFileName);

                if(! Mediatypes.Contains(mediatype)){

                    string info = $"File format of type Media for the Blob {content.MediaFileName} from the content {content.Title} is not supported.";
                    errorList.Add(info);
                }
                foreach(Attachment attachment in content.Attachments)
                {         
                    if(attachment.Type is AttachmentType.Thumbnail){

                        string imagetype = FileFormat(attachment.Name); 
                        if(! Thumbnailtypes.Contains(imagetype))
                        {
                            string info = $"File format of type {attachment.Type} for the Blob {attachment.Name} from the content {content.Title} is not supported.";
                            errorList.Add(info);
                        }   
                    }
                    else{
                        string teasertype = FileFormat(attachment.Name); 
                        if(! Mediatypes.Contains(teasertype))
                        {
                            string info =$"File format of type {attachment.Type} for the Blob {attachment.Name} from the content {content.Title} is not supported.";
                            errorList.Add(info);
                        } 
                    }
                }
            }
            return  errorList; 
        }

        private async Task<List<string>> ValidateBlobExistence(List<Content> contents,Guid contentProviderId)
        {
            var containerName = contentProviderId+ApplicationConstants.StorageContainerSuffix.Cdn;

            var rawcontainerName = contentProviderId+ApplicationConstants.StorageContainerSuffix.Raw;

            BlobContainerClient rawClient = _cmsBlobServiceClient.GetBlobContainerClient(rawcontainerName);

            List<String> Mediatypes = ApplicationConstants.SupportedFileFormats.mediaFormats;
                
            List<String> Thumbnailtypes = ApplicationConstants.SupportedFileFormats.ThumbnailFormats;

            List<string> errorList = new List<string>();
            
            foreach(Content content in contents)
            {         
                if(! rawClient.GetBlobClient(content.MediaFileName).Exists())
                {
                    string info = $"Blob file of type Media with name {content.MediaFileName} from the content {content.Title} is not found.";
                    errorList.Add(info);
                }
                foreach(Attachment attachment in content.Attachments)
                {         
                    if(attachment.Type is AttachmentType.Thumbnail){

                        if(! rawClient.GetBlobClient(attachment.Name).Exists()){
                            string info = $"Blob file of type {attachment.Type} with name {attachment.Name} from the content {content.Title} is not found";
                            errorList.Add(info);
                        }
                    }
                    else{
                        if(! rawClient.GetBlobClient(attachment.Name).Exists()){
                            string info =$"Blob file of type {attachment.Type} with name {attachment.Name} from the content {content.Title} is not found";
                            errorList.Add(info);
                        } 
                    }
                }
            }
            return errorList;
        } 

        private static string FileFormat(string Name){
            string format = Name.Split('.')[1]; 
            return format;
        }


        #endregion
    }
}

