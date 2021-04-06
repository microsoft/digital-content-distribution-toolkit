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
using System.Net;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using System.ComponentModel.DataAnnotations;
using blendnet.cms.api.Common;
using blendnet.cms.api.Model;

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

        private AmsHelper _amsHelper;

        public ContentController(IContentRepository contentRepository,
                                            ILogger<ContentController> logger,
                                            IEventBus eventBus,
                                            IAzureClientFactory<BlobServiceClient> blobClientFactory,
                                            AmsHelper amshelper)
        {
            _contentRepository = contentRepository;

            _cmsBlobServiceClient = blobClientFactory.CreateClient(ApplicationConstants.StorageInstanceNames.CMSStorage);

            _logger = logger;

            _eventBus = eventBus;

            _amsHelper = amshelper;
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
        /// Upload Contents
        /// </summary>
        /// <returns></returns>
        [HttpPost("{contentProviderId:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult> UploadContent(IFormFile file, Guid contentProviderId)
        {
            // Check for Json Format
            if (! ValidateFileExtension(file))
            {
                string info = "File not found or not in Json format";
                return BadRequest(info);
            }

            List<string> errorDetails = new List<string>();

            // Convert Json to list of Contents.
            List<Content> contents = ValidateJsonSchema(file, errorDetails);

            if (contents == null && errorDetails.Count > 0)
            {
                return BadRequest(errorDetails);
            }

            //perform data annotation validats to ensure all required attibutes are present
            var annotationErrorList = ValidateDataAnnotationCheck(contents);

            if (annotationErrorList.Count > 0)
            {
                return BadRequest(annotationErrorList);
            }

            // Check for File Extensions on the raw container.
            var extErrorList = ValidateFileExtensions(contents,contentProviderId);

            if(extErrorList.Count > 0)
            {
                return BadRequest(extErrorList);
            }     

            // Check for duplicate Ids
            var dupIdList = await ValidateDuplicateIdCheck(contents,contentProviderId);

            if(dupIdList.Count>0 )
            {       
                return BadRequest(dupIdList);
            }

            // Check for File Existence on the raw container.
            var fileErrorList = await ValidateBlobExistence(contents,contentProviderId);

            if(fileErrorList.Count>0)
            {
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
        public async Task<ActionResult> DeleteContent(List<Guid> contentIds)
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

        /// <summary>
        /// Returns the Token to view the content
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        [HttpGet("{contentId:guid}/token", Name = nameof(GetContentToken))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<string>> GetContentToken(Guid contentId)
        {
            Content content = await _contentRepository.GetContentById(contentId);

            if (content == null)
            {
                return BadRequest($"No valid details found for givent content id {contentId}");
            }

            if (content.ContentTransformStatus != ContentTransformStatus.TransformComplete)
            {
                return BadRequest($"The content tranform status should be complete. Current status is {content.ContentTransformStatus.ToString()}");
            }

            string token = await _amsHelper.GetContentToken(content.Id.Value, content.ContentTransformStatusUpdatedBy.Value);

            return token;
        }


        [HttpPost("{contentProviderId:guid}/contentlist")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<Content>> GetContentByContentProviderId(Guid contentProviderId,ContentStatusFilter statusFilter)
        {
            List<Content> contentlist = await _contentRepository.GetContentByContentProviderId(contentProviderId,statusFilter);

            if (contentlist.Count() > 0)
            {
                return Ok(contentlist);
            }
            else
            {
                return NoContent();
            }
        }

        /// <summary>
        /// Submits the Transformation Request
        /// </summary>
        /// <param name="transformContentRequest"></param>
        /// <returns></returns>
        [HttpPost("transform", Name = nameof(TransformContent))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult> TransformContent(TransformContentRequest transformContentRequest)
        {
            if (transformContentRequest == null || 
                transformContentRequest.ContentIds == null || 
                transformContentRequest.ContentIds.Count() <= 0)
            {
                return BadRequest($"No content id(s) found in the input request. Please provide valid content id(s)");
            }

            List<Content> contentlist = await _contentRepository.GetContentByIds(transformContentRequest.ContentIds);

            List<string> errorList = new List<string>();

            //adds the invalid id details to the error list
            ValidateContentIds(transformContentRequest.ContentIds, contentlist, errorList);

            if(contentlist != null && contentlist.Count > 0)
            {
                foreach (Content content in contentlist)
                {
                    if (content.ContentUploadStatus == ContentUploadStatus.UploadComplete &&
                        (content.ContentTransformStatus == ContentTransformStatus.TransformNotInitialized ||
                         content.ContentTransformStatus == ContentTransformStatus.TransformFailed ||
                         content.ContentTransformStatus == ContentTransformStatus.TransformCancelled))
                    {
                        content.ContentTransformStatus = ContentTransformStatus.TransformSubmitted;
                        
                        content.ModifiedDate = DateTime.UtcNow;

                        await _contentRepository.UpdateContent(content);

                        ContentCommand contentTransformCommand = new ContentCommand()
                        {
                            CommandType = CommandType.TransformContent,
                            ContentId = content.Id.Value
                        };

                        //publish the event
                        ContentTransformIntegrationEvent contentTransIntegrationEvent = new ContentTransformIntegrationEvent()
                        {
                            ContentTransformCommand = contentTransformCommand
                        };

                        await _eventBus.Publish(contentTransIntegrationEvent);
                    }
                    else
                    {
                        errorList.Add($"{content.Id.Value} - Upload Status should be {ContentUploadStatus.UploadComplete} and Tranform Status should be in {ContentTransformStatus.TransformNotInitialized},{ContentTransformStatus.TransformFailed},{ContentTransformStatus.TransformCancelled}");
                    }
                }
            }

            return Ok(errorList);
          
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Validate Content Ids
        /// </summary>
        /// <param name="parentIds"></param>
        /// <param name="retrievedContents"></param>
        /// <param name="errorList"></param>
        private void ValidateContentIds(List<Guid> parentIds, List<Content> retrievedContents,  List<string> errorList)
        {
            List<Guid> invalidIds = GetInvalidContentIds(parentIds, retrievedContents);

            if (invalidIds != null && invalidIds.Count > 0)
            {
                foreach (Guid invalidId in invalidIds)
                {
                    errorList.Add($"For { invalidId} no valid details found in application database.");
                }
            }
        }

        /// <summary>
        /// Uploads the content and pushes a message to event bus
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        private async Task UploadContent(List<Content> contents, Guid contentProviderId)
        {
            foreach (Content content in contents)
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

        /// <summary>
        /// Validates File Extension
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool ValidateFileExtension(IFormFile file)
        {
            string fileExt = System.IO.Path.GetExtension(file.FileName);

            if (fileExt.Equals(".json"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Validates if the json in the list of content format or not
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private List<Content> ValidateJsonSchema(IFormFile file, List<string> errorDetails)
        {

            string error = string.Empty;

            string text;

            try
            {
                using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                {
                    text = reader.ReadToEnd();

                    reader.Close();
                }

                JsonSerializerOptions serializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                serializerOptions.Converters.Add(new JsonStringEnumConverter());

                List<Content> contents = JsonSerializer.Deserialize<List<Content>>(text, serializerOptions);

                return contents;
            }
            catch (Exception ex)
            {
                errorDetails.Add(ex.Message);

                return null;
            }
        }

        /// <summary>
        /// Performs the data annotation check on each object.
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        private List<string> ValidateDataAnnotationCheck(List<Content> contents)
        {
            List<string> errorList = new List<string>();

            ValidationContext validationContext;

            List<ValidationResult> validationResults;

            bool validationResult;

            int index = 0;

            foreach (Content content in contents)
            {
                validationContext = new ValidationContext(content, null, null);

                validationResults = new List<ValidationResult>();

                validationResult = Validator.TryValidateObject(content, validationContext, validationResults, true);

                if (!validationResult)
                {
                    foreach (ValidationResult result in validationResults)
                    {
                        errorList.Add($"Item {index} - {result.ErrorMessage}");
                    }
                }

                index++;
            }

            return errorList;
        }

        /// <summary>
        /// Duplicate Content Id Check
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        private async Task<List<string>> ValidateDuplicateIdCheck(List<Content> contents, Guid contentProviderId)
        {
            List<string> errorList = new List<string>();

            HashSet<string> contentIds = new HashSet<string>();

            foreach (Content content in contents)
            {
                // contentIds.Add(content.ContentProviderContentId);
                if (!contentIds.Add(content.ContentProviderContentId))
                {
                    // Duplicate ContentIds in the file
                    string info = $"Content with Id {content.ContentProviderContentId} and Title{content.Title} is found multiple times in the uploaded file.";
                    errorList.Add(info);
                }

            }

            List<Content> dbcontents = await _contentRepository.GetContentByContentProviderId(contentProviderId,null);

            HashSet<string> dbcontentIds = new HashSet<string>();

            foreach (Content content in dbcontents)
            {
                dbcontentIds.Add(content.ContentProviderContentId);
            }

            HashSet<string> commonIds = new HashSet<string>(dbcontentIds);

            commonIds.IntersectWith(contentIds);

            foreach (string Id in commonIds)
            {
                string info = $"Content with Id {Id} and is already uploaded to the Database.";
                errorList.Add(info);

            }
            return errorList;
        }

        /// <summary>
        /// Validates files extensions
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        private List<string> ValidateFileExtensions(List<Content> contents, Guid contentProviderId)
        {
            List<string> errorList = new List<string>();

            List<String> Mediatypes = ApplicationConstants.SupportedFileFormats.mediaFormats;

            List<String> Thumbnailtypes = ApplicationConstants.SupportedFileFormats.ThumbnailFormats;

            foreach (Content content in contents)
            {
                string mediatype = FileFormat(content.MediaFileName);

                if (!Mediatypes.Contains(mediatype))
                {

                    string info = $"File format of type Media for the Blob {content.MediaFileName} from the content {content.Title} is not supported.";

                    errorList.Add(info);
                }
                foreach (Attachment attachment in content.Attachments)
                {
                    if (attachment.Type is AttachmentType.Thumbnail)
                    {

                        string imagetype = FileFormat(attachment.Name);

                        if (!Thumbnailtypes.Contains(imagetype))
                        {
                            string info = $"File format of type {attachment.Type} for the Blob {attachment.Name} from the content {content.Title} is not supported.";
                            errorList.Add(info);
                        }
                    }
                    else
                    {

                        string teasertype = FileFormat(attachment.Name);

                        if (!Mediatypes.Contains(teasertype))
                        {
                            string info = $"File format of type {attachment.Type} for the Blob {attachment.Name} from the content {content.Title} is not supported.";
                            errorList.Add(info);
                        }
                    }
                }
            }
            return errorList;
        }

        /// <summary>
        /// Validates if the given path exists in the raw foldr or now
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        private async Task<List<string>> ValidateBlobExistence(List<Content> contents, Guid contentProviderId)
        {
            var rawcontainerName = $"{contentProviderId}{ApplicationConstants.StorageContainerSuffix.Raw}";

            BlobContainerClient rawClient = _cmsBlobServiceClient.GetBlobContainerClient(rawcontainerName);

            List<String> Mediatypes = ApplicationConstants.SupportedFileFormats.mediaFormats;

            List<String> Thumbnailtypes = ApplicationConstants.SupportedFileFormats.ThumbnailFormats;

            List<string> errorList = new List<string>();

            foreach (Content content in contents)
            {
                if (!await rawClient.GetBlobClient(content.MediaFileName).ExistsAsync())
                {
                    string info = $"Blob file of type Media with name {content.MediaFileName} from the content {content.Title} is not found.";

                    errorList.Add(info);
                }
                foreach (Attachment attachment in content.Attachments)
                {
                    if (attachment.Type is AttachmentType.Thumbnail)
                    {

                        if (!await rawClient.GetBlobClient(attachment.Name).ExistsAsync())
                        {

                            string info = $"Blob file of type {attachment.Type} with name {attachment.Name} from the content {content.Title} is not found";

                            errorList.Add(info);
                        }
                    }
                    else
                    {
                        if (!await rawClient.GetBlobClient(attachment.Name).ExistsAsync())
                        {

                            string info = $"Blob file of type {attachment.Type} with name {attachment.Name} from the content {content.Title} is not found";

                            errorList.Add(info);
                        }
                    }
                }
            }
            return errorList;
        }

        /// <summary>
        /// Returns the file extension
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        private static string FileFormat(string Name)
        {
            string format = Name.Split('.')[1];
            return format;
        }

        /// <summary>
        /// List of Invalid Content Ids
        /// </summary>
        /// <param name="parentList"></param>
        /// <param name="retrievedContents"></param>
        /// <returns></returns>
        private List<Guid> GetInvalidContentIds(List<Guid> parentList, List<Content> retrievedContents)
        {
            List<Guid> invlidContentIds = new List<Guid>();

            foreach (Guid contentId in parentList)
            {
                if (!retrievedContents.Exists(c=>c.Id.Value.ToString().Equals(contentId.ToString())))
                {
                    invlidContentIds.Add(contentId);
                }
            }

            return invlidContentIds;
        }

        #endregion
    }
}

