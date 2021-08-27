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
using Microsoft.AspNetCore.Authorization;
using blendnet.common.infrastructure.Authentication;
using blendnet.common.dto.User;
using Microsoft.Extensions.Localization;
using blendnet.cms.api;
using System.Reflection;
using AutoMapper;

namespace blendnet.cms.api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin, ApplicationConstants.KaizalaIdentityRoles.ContentAdmin)]
    public class ContentController  : BaseController
    {
        private readonly ILogger _logger;

        private IEventBus _eventBus;

        BlobServiceClient _cmsBlobServiceClient;

        private IContentRepository _contentRepository;

        private IContentProviderRepository _contentProviderRepository;

        private AmsHelper _amsHelper;

        IStringLocalizer<SharedResource> _stringLocalizer;

        private readonly IMapper _mapper;

        public ContentController(   IContentRepository contentRepository,
                                    IContentProviderRepository contentProviderRepository,
                                    ILogger<ContentController> logger,
                                    IEventBus eventBus,
                                    IAzureClientFactory<BlobServiceClient> blobClientFactory,
                                    AmsHelper amshelper,
                                    IMapper mapper,
                                    IStringLocalizer<SharedResource> stringLocalizer)
            :base(contentProviderRepository)
        {
            _contentRepository = contentRepository;

            _contentProviderRepository = contentProviderRepository;

            _cmsBlobServiceClient = blobClientFactory.CreateClient(ApplicationConstants.StorageInstanceNames.CMSStorage);

            _logger = logger;

            _eventBus = eventBus;

            _amsHelper = amshelper;

            _stringLocalizer = stringLocalizer;

            _mapper = mapper;

        }

        #region Content Management Methods

        /// <summary>
        /// Get Content 
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        [HttpGet("{contentId:guid}", Name = nameof(GetContent))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin, ApplicationConstants.KaizalaIdentityRoles.ContentAdmin)]
        public async Task<ActionResult<Content>> GetContent(Guid contentId)
        {
            var content = await _contentRepository.GetContentById(contentId);

            if (content != default(Content))
            {
                ActionResult actionResult = await CheckAccess(content.ContentProviderId);

                if (!(actionResult is OkResult))
                {
                    return actionResult;
                }

                return Ok(content);
            }
            else
            {
                return NotFound();
            }

        }

        /// <summary>
        /// Get Contents by content ids 
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        [HttpPost("contentIds", Name = nameof(GetContentProviderIds))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<List<ContentInfo>>> GetContentProviderIds(List<Guid> contentIds)
        {
            if(contentIds == null || contentIds.Count == 0)
            {
                return BadRequest(_stringLocalizer["CMS_ERR_0030"]);
            }

            var contents = await _contentRepository.GetContentByIds(contentIds);

            if(contents == null || contents.Count == 0)
            {
                return NotFound();
            }

            var contentInfos = _mapper.Map<List<Content>, List<ContentInfo>>(contents);

            return Ok(contentInfos);

        }


        /// <summary>
        /// Get Content 
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        [HttpGet("{contentId:guid}/command/{commandId:guid}", Name = nameof(GetCommand))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<ContentCommand>> GetCommand(Guid contentId, Guid commandId)
        {
            var contentCommand = await _contentRepository.GetContentCommandById(commandId,contentId);

            if (contentCommand != default(ContentCommand))
            {
                return Ok(contentCommand);
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
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin, ApplicationConstants.KaizalaIdentityRoles.ContentAdmin)]
        public async Task<ActionResult> UploadContent(IFormFile file, Guid contentProviderId)
        {
            ActionResult actionResult = await CheckAccess(contentProviderId);

            if (!(actionResult is OkResult))
            {
                return actionResult;
            }

            
            List<string> errorDetails = new List<string>();

            //Null check for file
            if (file == null)
            {
                errorDetails.Add(_stringLocalizer["CMS_ERR_0025"]);

                return BadRequest(errorDetails);
            }

            // Check for Json Format
            if (!ValidateFileExtension(file))
            {
                errorDetails.Add(_stringLocalizer["CMS_ERR_0001"]);
                
                return BadRequest(errorDetails);
            }

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
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin, ApplicationConstants.KaizalaIdentityRoles.ContentAdmin)]
        public async Task<ActionResult> DeleteContent(Guid contentId)
        {
            List<string> errorInfo = new List<string>();

            Content contentToDelete = await _contentRepository.GetContentById(contentId);

            if (contentToDelete == null)
            {
                return NotFound();
            }

            ActionResult actionResult = await CheckAccess(contentToDelete.ContentProviderId);

            if (!(actionResult is OkResult))
            {
                return actionResult;
            }

            if ((   contentToDelete.ContentUploadStatus != ContentUploadStatus.UploadFailed &&
                    contentToDelete.ContentUploadStatus != ContentUploadStatus.UploadComplete ) )
            {
                errorInfo.Add(String.Format(_stringLocalizer["CMS_ERR_0002"], contentToDelete.Id.Value, ContentUploadStatus.UploadComplete,
                    ContentUploadStatus.UploadFailed, ContentTransformStatus.TransformNotInitialized, ContentBroadcastStatus.BroadcastNotInitialized));

                return BadRequest(errorInfo);
            }

            if ((contentToDelete.ContentTransformStatus != ContentTransformStatus.TransformNotInitialized ||
                    contentToDelete.ContentBroadcastStatus != ContentBroadcastStatus.BroadcastNotInitialized))
            {
                errorInfo.Add(String.Format(_stringLocalizer["CMS_ERR_0002"], contentToDelete.Id.Value, ContentUploadStatus.UploadComplete,
                    ContentUploadStatus.UploadFailed, ContentTransformStatus.TransformNotInitialized, ContentBroadcastStatus.BroadcastNotInitialized));

                return BadRequest(errorInfo);
            }


            int statusCode = await _contentRepository.DeleteContent(contentId);

            if (statusCode == (int)System.Net.HttpStatusCode.NoContent)
            {
                ContentCommand contentDeleteCommand = new ContentCommand()
                {
                    CommandType = CommandType.DeleteContent,
                    Content = contentToDelete,
                    CreatedByUserId = UserClaimData.GetUserId(this.User.Claims)
                };

                //publish the event
                ContentDeletedIntegrationEvent contentDeletedIntegrationEvent = new ContentDeletedIntegrationEvent()
                {
                    ContentDeleteCommand = contentDeleteCommand
                };

                await _eventBus.Publish(contentDeletedIntegrationEvent);

                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Returns the Token to view the content
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        [HttpGet("{contentId:guid}/token", Name = nameof(GetContentToken))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<string>> GetContentToken(Guid contentId)
        {
            List<string> errorInfo = new List<string>();

            Content content = await _contentRepository.GetContentById(contentId);

            if (content == null)
            {
                errorInfo.Add(String.Format(_stringLocalizer["CMS_ERR_0003"], contentId));

                return BadRequest(errorInfo);
            }

            if (content.ContentTransformStatus != ContentTransformStatus.TransformComplete)
            {
                errorInfo.Add(String.Format(_stringLocalizer["CMS_ERR_0004"], content.ContentTransformStatus.ToString()));

                return BadRequest(errorInfo);
            }

            string token = await _amsHelper.GetContentToken(content.Id.Value, content.ContentTransformStatusUpdatedBy.Value);

            return Ok(token);
        }


        [HttpPost("{contentProviderId:guid}/contentlist")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin, ApplicationConstants.KaizalaIdentityRoles.ContentAdmin)]
        public async Task<ActionResult<Content>> GetContentByContentProviderId(Guid contentProviderId,ContentStatusFilter statusFilter)
        {
            ActionResult actionResult = await CheckAccess(contentProviderId);

            if (!(actionResult is OkResult))
            {
                return actionResult;
            }

            List<Content> contentlist = await _contentRepository.GetContentByContentProviderId(contentProviderId,statusFilter,false);

            if (contentlist.Count() > 0)
            {
                return Ok(contentlist);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Submits the Transformation Request
        /// </summary>
        /// <param name="transformContentRequest"></param>
        /// <returns></returns>
        [HttpPost("transform", Name = nameof(TransformContent))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult> TransformContent(TransformContentRequest transformContentRequest)
        {
            List<string> errorInfo = new List<string>();

            if (transformContentRequest == null || 
                transformContentRequest.ContentIds == null || 
                transformContentRequest.ContentIds.Count() <= 0)
            {
                errorInfo.Add(_stringLocalizer["CMS_ERR_0005"]);

                return BadRequest(errorInfo);
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

                        content.ModifiedByByUserId = UserClaimData.GetUserId(this.User.Claims);

                        content.ModifiedDate = DateTime.UtcNow;

                        await _contentRepository.UpdateContent(content);

                        ContentCommand contentTransformCommand = new ContentCommand()
                        {
                            CommandType = CommandType.TransformContent,
                            ContentId = content.Id.Value,
                            CreatedByUserId = UserClaimData.GetUserId(this.User.Claims)
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
                        errorList.Add(String.Format(_stringLocalizer["CMS_ERR_0006"], content.Id.Value, ContentUploadStatus.UploadComplete,
                            ContentTransformStatus.TransformNotInitialized, ContentTransformStatus.TransformFailed, ContentTransformStatus.TransformCancelled));
                    }
                }
            }

            return Ok(errorList);
          
        }

        /// <summary>
        /// Submits the Broadcast Request
        /// </summary>
        /// <param name="broadcastContentRequest"></param>
        /// <returns></returns>
        [HttpPost("broadcast", Name = nameof(BroadcastContent))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult> BroadcastContent(BroadcastContentRequest broadcastContentRequest)
        {
            List<string> errorInfo = new List<string>();

            if (broadcastContentRequest == null ||
                broadcastContentRequest.ContentIds == null ||
                broadcastContentRequest.ContentIds.Count() <= 0 ||
                broadcastContentRequest.Filters == null ||
                broadcastContentRequest.Filters.Count() <= 0 ||
                broadcastContentRequest.StartDate.Equals(DateTime.MinValue) ||
                broadcastContentRequest.EndDate.Equals(DateTime.MinValue))
            {
                errorInfo.Add(_stringLocalizer["CMS_ERR_0007"]);

                return BadRequest(errorInfo);
            }

            if (broadcastContentRequest.EndDate <= broadcastContentRequest.StartDate)
            {
                errorInfo.Add(_stringLocalizer["CMS_ERR_0008"]);

                return BadRequest(errorInfo);
            }

            List<Content> contentlist = await _contentRepository.GetContentByIds(broadcastContentRequest.ContentIds);

            List<string> errorList = new List<string>();

            //adds the invalid id details to the error list
            ValidateContentIds(broadcastContentRequest.ContentIds, contentlist, errorList);

            if (contentlist != null && contentlist.Count > 0)
            {
                foreach (Content content in contentlist)
                {
                    if (content.ContentUploadStatus == ContentUploadStatus.UploadComplete &&
                        content.ContentTransformStatus == ContentTransformStatus.TransformComplete && 
                         (content.ContentBroadcastStatus == ContentBroadcastStatus.BroadcastNotInitialized ||
                         content.ContentBroadcastStatus == ContentBroadcastStatus.BroadcastOrderCancelled ||
                         content.ContentBroadcastStatus == ContentBroadcastStatus.BroadcastOrderFailed ||
                         content.ContentBroadcastStatus == ContentBroadcastStatus.BroadcastOrderRejected ||
                         content.ContentBroadcastStatus == ContentBroadcastStatus.BroadcastFailed ||
                         content.ContentBroadcastStatus == ContentBroadcastStatus.BroadcastCancelComplete))
                    {
                        content.ContentBroadcastStatus = ContentBroadcastStatus.BroadcastSubmitted;

                        content.ModifiedByByUserId = UserClaimData.GetUserId(this.User.Claims);

                        content.ModifiedDate = DateTime.UtcNow;

                        await _contentRepository.UpdateContent(content);

                        ContentCommand contentBroadCommand = new ContentCommand()
                        {
                            CommandType = CommandType.BroadcastContent,
                            ContentId = content.Id.Value,
                            CreatedByUserId = UserClaimData.GetUserId(this.User.Claims),
                            BroadcastRequest = new BroadcastRequest() 
                            { 
                                Filters = broadcastContentRequest.Filters,
                                StartDate = broadcastContentRequest.StartDate,
                                EndDate = broadcastContentRequest.EndDate
                            }
                        };

                        //publish the event
                        ContentBroadcastIntegrationEvent contentBroadcastIntegrationEvent = new ContentBroadcastIntegrationEvent()
                        {
                            ContentBroadcastCommand = contentBroadCommand
                        };

                        await _eventBus.Publish(contentBroadcastIntegrationEvent);
                    }
                    else
                    {
                        errorList.Add(String.Format(_stringLocalizer["CMS_ERR_0009"], content.Id.Value, ContentUploadStatus.UploadComplete,
                            ContentTransformStatus.TransformComplete, ContentBroadcastStatus.BroadcastNotInitialized, ContentBroadcastStatus.BroadcastFailed,
                            ContentBroadcastStatus.BroadcastOrderCancelled, ContentBroadcastStatus.BroadcastOrderFailed,
                            ContentBroadcastStatus.BroadcastOrderRejected, ContentBroadcastStatus.BroadcastCancelComplete));
                    }
                }
            }

            return Ok(errorList);

        }


        /// <summary>
        /// Returns the Token to view the content
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        [HttpPost("{contentId:guid}/cancelbroadcast", Name = nameof(CancelBroadcast))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<string>> CancelBroadcast(Guid contentId)
        {
            List<string> errorInfo = new List<string>();

            Content content = await _contentRepository.GetContentById(contentId);

            if (content == null)
            {
                errorInfo.Add(String.Format(_stringLocalizer["CMS_ERR_0003"], contentId));

                return BadRequest(errorInfo);
            }

            //allow only if broadcasted or broadcast cancellation has failed
            if (content.ContentBroadcastStatus != ContentBroadcastStatus.BroadcastOrderComplete &&
                content.ContentBroadcastStatus != ContentBroadcastStatus.BroadcastCancelFailed)
            {
                errorInfo.Add(String.Format(_stringLocalizer["CMS_ERR_0027"], ContentBroadcastStatus.BroadcastOrderComplete, ContentBroadcastStatus.BroadcastCancelFailed));

                return BadRequest(errorInfo);
            }

            content.ContentBroadcastStatus = ContentBroadcastStatus.BroadcastCancelSubmitted;

            content.ContentBroadcastStatusUpdatedBy = null;

            content.ModifiedByByUserId = UserClaimData.GetUserId(this.User.Claims);

            content.ModifiedDate = DateTime.UtcNow;

            await _contentRepository.UpdateContent(content);

            ContentCommand contentBroadcastCancelCommand = new ContentCommand()
            {
                CommandType = CommandType.CancelBroadcastContent,
                ContentId = content.Id.Value,
                CreatedByUserId = UserClaimData.GetUserId(this.User.Claims)
            };

            //publish the event
            ContentBroadcastCancellationIntegrationEvent contentBroadcastIntegrationEvent = new ContentBroadcastCancellationIntegrationEvent()
            {
                ContentBroadcastCancellationCommand = contentBroadcastCancelCommand
            };

            await _eventBus.Publish(contentBroadcastIntegrationEvent);

            return Ok();
        }


        /// <summary>
        /// Activate / DeActivate content id
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        [HttpPost("{contentId:guid}/changeactivestatus")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult> ActivateDeactivateContent(Guid contentId, ChangeActiveStatusRequest statusRequest)
        {
            List<string> errorInfo = new List<string>();

            Content existingContent = await _contentRepository.GetContentById(contentId);

            if (existingContent == null)
            {
                return NotFound();
            }

            if (existingContent.ContentTransformStatus != ContentTransformStatus.TransformComplete)
            {
                errorInfo.Add(String.Format(_stringLocalizer["CMS_ERR_0028"], ContentTransformStatus.TransformComplete,
                     ContentBroadcastStatus.BroadcastOrderComplete));

                return BadRequest(errorInfo);
            }

            if (existingContent.ContentBroadcastStatus != ContentBroadcastStatus.BroadcastNotInitialized &&
                existingContent.ContentBroadcastStatus != ContentBroadcastStatus.BroadcastOrderComplete)
            {
                errorInfo.Add(String.Format(_stringLocalizer["CMS_ERR_0028"], ContentTransformStatus.TransformComplete,
                     ContentBroadcastStatus.BroadcastOrderComplete));

                return BadRequest(errorInfo);
            }

            //if it's already in same state, reject it.
            if (existingContent.IsActive == statusRequest.Status)
            {
                errorInfo.Add(String.Format(_stringLocalizer["CMS_ERR_0029"], statusRequest.Status ));

                return BadRequest(errorInfo);
            }

            
            existingContent.IsActive = statusRequest.Status;

            existingContent.ModifiedByByUserId = UserClaimData.GetUserId(this.User.Claims);

            existingContent.ModifiedDate = DateTime.UtcNow;

            await _contentRepository.UpdateContent(existingContent);

            return NoContent();
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
                    errorList.Add(String.Format(_stringLocalizer["CMS_ERR_0010"], invalidId));
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

                content.CreatedDate = DateTime.UtcNow;

                content.CreatedByUserId = UserClaimData.GetUserId(this.User.Claims);

                content.ModifiedByByUserId = null;

                content.ContentProviderId = contentProviderId;

                // Update the ContentUpload Status to UploadInProgress

                content.ContentUploadStatus = ContentUploadStatus.UploadSubmitted;

                Guid contentId = await _contentRepository.CreateContent(content);

                ContentCommand contentCommand = new ContentCommand();

                contentCommand.ContentId = contentId;

                contentCommand.CreatedByUserId = UserClaimData.GetUserId(this.User.Claims);

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
                errorDetails.Add(string.Format(_stringLocalizer["CMS_ERR_0024"], ex.Message));

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

            System.ComponentModel.DataAnnotations.ValidationContext validationContext;

            List<ValidationResult> validationResults;

            bool validationResult;

            int index = 0;

            foreach (Content content in contents)
            {
                validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(content, null, null);

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
                if (!contentIds.Add(content.ContentProviderContentId))
                {
                    // Duplicate ContentIds in the file
                    string info = String.Format(_stringLocalizer["CMS_ERR_0011"], content.ContentProviderContentId, content.Title);
                    errorList.Add(info);
                }

            }

            List<Content> dbcontents = await _contentRepository.GetContentByContentProviderId(contentProviderId,null,false);

            HashSet<string> dbcontentIds = new HashSet<string>();

            foreach (Content content in dbcontents)
            {
                dbcontentIds.Add(content.ContentProviderContentId);
            }

            HashSet<string> commonIds = new HashSet<string>(dbcontentIds);

            commonIds.IntersectWith(contentIds);

            foreach (string Id in commonIds)
            {
                string info = String.Format(_stringLocalizer["CMS_ERR_0012"], Id);
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

                    string info = String.Format(_stringLocalizer["CMS_ERR_0013"], content.MediaFileName, content.Title);

                    errorList.Add(info);
                }
                foreach (Attachment attachment in content.Attachments)
                {
                    if (attachment.Type is AttachmentType.Thumbnail)
                    {

                        string imagetype = FileFormat(attachment.Name);

                        if (!Thumbnailtypes.Contains(imagetype))
                        {
                            string info = String.Format(_stringLocalizer["CMS_ERR_0014"], attachment.Type, attachment.Name, content.Title);
                            errorList.Add(info);
                        }
                    }
                    else
                    {

                        string teasertype = FileFormat(attachment.Name);

                        if (!Mediatypes.Contains(teasertype))
                        {
                            string info = String.Format(_stringLocalizer["CMS_ERR_0014"], attachment.Type, attachment.Name, content.Title);
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

                    string info = String.Format(_stringLocalizer["CMS_ERR_0015"], content.MediaFileName, content.Title);

                    errorList.Add(info);
                }
                foreach (Attachment attachment in content.Attachments)
                {
                    if (attachment.Type is AttachmentType.Thumbnail)
                    {

                        if (!await rawClient.GetBlobClient(attachment.Name).ExistsAsync())
                        {

                            string info = String.Format(_stringLocalizer["CMS_ERR_0016"], attachment.Type, attachment.Name, content.Title);

                            errorList.Add(info);
                        }
                    }
                    else
                    {
                        if (!await rawClient.GetBlobClient(attachment.Name).ExistsAsync())
                        {

                            string info = String.Format(_stringLocalizer["CMS_ERR_0016"], attachment.Type, attachment.Name, content.Title);

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

