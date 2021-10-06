using blendnet.cms.repository.Interfaces;
using blendnet.common.dto;
using blendnet.common.dto.Events;
using blendnet.common.dto.User;
using blendnet.common.infrastructure;
using blendnet.common.infrastructure.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.cms.api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin, ApplicationConstants.KaizalaIdentityRoles.ContentAdmin)]
    public class ContentProviderController : BaseController
    {
        private readonly ILogger _logger;

        private IEventBus _eventBus;

        private IContentProviderRepository _contentProviderRepository;

        IStringLocalizer<SharedResource> _stringLocalizer;

        public ContentProviderController(IContentProviderRepository contentProviderRepository,
                                            ILogger<ContentProviderController> logger,
                                            IEventBus eventBus,
                                            IStringLocalizer<SharedResource> stringLocalizer)
            : base(contentProviderRepository)
        {
            _contentProviderRepository = contentProviderRepository;

            _logger = logger;

            _eventBus = eventBus;

            _stringLocalizer = stringLocalizer;
        }


        #region Content Providers Methods

        /// <summary>
        /// List all content providers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin, ApplicationConstants.KaizalaIdentityRoles.ContentAdmin)]
        public async Task<ActionResult<List<ContentProviderDto>>> GetContentProviders()
        {
            var contentProviders = await _contentProviderRepository.GetContentProviders();

            List<ContentProviderDto> contentProviderToReturn = new List<ContentProviderDto>();

            //if the user is not super admin filter the list
            if (!this.User.IsInRole(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin))
            {
                if (contentProviders != null && contentProviders.Count() > 0)
                {
                    foreach (ContentProviderDto contentProvider in contentProviders)
                    {
                        if (base.IsValidContentAdmin(contentProvider))
                        {
                            contentProviderToReturn.Add(contentProvider);
                        }
                    }
                }

                return Ok(contentProviderToReturn);
            }

            return Ok(contentProviders);
        }

        /// <summary>
        /// Get Content Provider
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        [HttpGet("{contentProviderId:guid}", Name = nameof(GetContentProvider))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin, ApplicationConstants.KaizalaIdentityRoles.ContentAdmin)]
        public async Task<ActionResult<ContentProviderDto>> GetContentProvider(Guid contentProviderId)
        {
            ActionResult actionResult = await CheckAccess(contentProviderId);

            if (!(actionResult is OkResult))
            {
                return actionResult;
            }

            var contentProvider = await _contentProviderRepository.GetContentProviderById(contentProviderId);

            if (contentProvider != default(ContentProviderDto))
            {
                return Ok(contentProvider);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Create Content Provider
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<string>> CreateContentProvider(ContentProviderDto contentProvider)
        {
            //Check for duplicate phone number
            if (DoesDuplicateAdminPhoneNumberExists(contentProvider))
            {
                return BadRequest(_stringLocalizer["CMS_ERR_0032"]);
            }

            //Check if admin is already allocated to some other content provider
            if (await IsContentAdminAllocatedToAnotherContentProvider(contentProvider))
            {
                return BadRequest(_stringLocalizer["CMS_ERR_0033"]);
            }

            //generate ids for content provider and administrator
            contentProvider.SetIdentifiers();
            
            contentProvider.CreatedByUserId = UserClaimData.GetUserId(this.User.Claims);

            contentProvider.CreatedDate = DateTime.UtcNow;

            contentProvider.ModifiedByByUserId = null;

            contentProvider.ModifiedDate = null;

            contentProvider.Type = ContentProviderContainerType.ContentProvider;

            var contentProviderId = await _contentProviderRepository.CreateContentProvider(contentProvider);

            //publish the event
            ContentProviderCreatedIntegrationEvent contentProviderCreatedIntegrationEvent = new ContentProviderCreatedIntegrationEvent()
            {
                ContentProvider = contentProvider,
            };

            await _eventBus.Publish(contentProviderCreatedIntegrationEvent);

            return CreatedAtAction(nameof(GetContentProvider),
                                    new { contentProviderId = contentProviderId, Version = ApiVersion.Default.MajorVersion.ToString() },
                                    contentProviderId);
        }

        /// <summary>
        /// Updates the content provider id
        /// </summary>  
        /// <param name="contentProviderId"></param>
        /// <param name="contentProvider"></param>
        /// <returns></returns>
        [HttpPost("{contentProviderId:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult> UpdateContentProvider(Guid contentProviderId, ContentProviderDto contentProvider)
        {
            //Check for duplicate phone number
            if (DoesDuplicateAdminPhoneNumberExists(contentProvider))
            {
                return BadRequest(_stringLocalizer["CMS_ERR_0032"]);
            }

            if (await IsContentAdminAllocatedToAnotherContentProvider(contentProvider))
            {
                return BadRequest(_stringLocalizer["CMS_ERR_0033"]);
            }

            contentProvider.Id = contentProviderId;

            contentProvider.Type = ContentProviderContainerType.ContentProvider;

            contentProvider.ModifiedDate = DateTime.UtcNow;

            contentProvider.ModifiedByByUserId = UserClaimData.GetUserId(this.User.Claims);

            ContentProviderDto beforeContentProvider = await _contentProviderRepository.GetContentProviderById(contentProvider.Id.Value);

            if (beforeContentProvider == null)
            {
                return NotFound();
            }

            int statusCode = await _contentProviderRepository.UpdateContentProvider(contentProvider);

            if (statusCode == (int)System.Net.HttpStatusCode.OK)
            {
                //publish the update content provider event
                ContentProviderUpdatedIntegrationEvent contentProviderUpdatedIntegrationEvent = new ContentProviderUpdatedIntegrationEvent()
                {
                    BeforeUpdateContentProvider = beforeContentProvider,
                    AfterUpdateContentProvider = contentProvider,
                };

                await _eventBus.Publish(contentProviderUpdatedIntegrationEvent);

                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Deletes the content provider id
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        [HttpDelete("{contentProviderId:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult> DeleteContentProvider(Guid contentProviderId)
        {
            int statusCode = await _contentProviderRepository.DeleteContentProvider(contentProviderId);

            if (statusCode == (int)System.Net.HttpStatusCode.NoContent)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Activate Content Provider
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        [HttpPost("{contentProviderId:guid}/activate")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult> ActivateContentProvider(Guid contentProviderId)
        {
            return await ActivateDeactivateContentProvider(contentProviderId, true);
        }


        /// <summary>
        /// Deactivate content provider
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        [HttpPost("{contentProviderId:guid}/deactivate")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult> DeactivateContentProvider(Guid contentProviderId)
        {
            return await ActivateDeactivateContentProvider(contentProviderId, false);
        }

        /// <summary>
        /// Generates SAS token for the content provider
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        [HttpGet("{contentProviderId:guid}/generateSaS")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin, ApplicationConstants.KaizalaIdentityRoles.ContentAdmin)]
        public async Task<ActionResult<SasTokenDto>> GenerateToken(Guid contentProviderId)
        {
            ActionResult actionResult = await CheckAccess(contentProviderId);

            if (!(actionResult is OkResult))
            {
                return actionResult;
            }

            SasTokenDto sasUri = _contentProviderRepository.GenerateSaSToken(contentProviderId);

            if (sasUri != null)
            {
                return Ok(sasUri);
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Private method to support activate and deactivate content provider
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <param name="activate"></param>
        /// <returns></returns>
        private async Task<ActionResult> ActivateDeactivateContentProvider(Guid contentProviderId, bool activate)
        {
            var contentProvider = await _contentProviderRepository.GetContentProviderById(contentProviderId);

            DateTime currentDateTime = DateTime.UtcNow;

            if (contentProvider != null)
            {
                if (activate)
                {
                    contentProvider.ActivationDate = currentDateTime;
                }
                else
                {
                    contentProvider.DeactivationDate = currentDateTime;
                }

                contentProvider.ModifiedByByUserId = UserClaimData.GetUserId(this.User.Claims);

                contentProvider.ModifiedDate = currentDateTime;

                await _contentProviderRepository.UpdateContentProvider(contentProvider);

                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Check if duplicate phone number exists
        /// </summary>
        /// <param name="contentProvider"></param>
        /// <returns></returns>
        private bool DoesDuplicateAdminPhoneNumberExists(ContentProviderDto contentProvider)
        {
            if (contentProvider.ContentAdministrators != null && 
                contentProvider.ContentAdministrators.Count > 0)
            {
                var duplicateCount = contentProvider.ContentAdministrators
                                                    .GroupBy(ca => ca.PhoneNumber)
                                                    .Where(gr => gr.Count() > 1)
                                                    .Count();

                if (duplicateCount > 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Is Content Admin Allocated To Another Content Provider
        /// </summary>
        /// <param name="contentProvider"></param>
        /// <returns></returns>
        private async Task<bool> IsContentAdminAllocatedToAnotherContentProvider(ContentProviderDto contentProvider)
        {
            if (contentProvider.ContentAdministrators != null &&
                contentProvider.ContentAdministrators.Count > 0)
            {
                List<string> adminPhoneNumbers = contentProvider.ContentAdministrators.Select(ca => ca.PhoneNumber).ToList();

                List<ContentProviderDto> contentProviders = await _contentProviderRepository.GetContentProvidersByAdmin(adminPhoneNumbers);

                if (contentProviders == null || contentProviders.Count() <= 0)
                {
                    return false;
                }

                int otherCps = contentProviders.Where(cp => (cp.Id.Value != contentProvider.Id)).Count();

                if (otherCps > 0)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
