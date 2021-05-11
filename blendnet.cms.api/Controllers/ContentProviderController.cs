using blendnet.cms.repository.Interfaces;
using blendnet.common.dto;
using blendnet.common.dto.Events;
using blendnet.common.dto.User;
using blendnet.common.infrastructure;
using blendnet.common.infrastructure.Authentication;
using Microsoft.AspNetCore.Mvc;
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

        public ContentProviderController(IContentProviderRepository contentProviderRepository,
                                            ILogger<ContentProviderController> logger,
                                            IEventBus eventBus)
            : base(contentProviderRepository)
        {
            _contentProviderRepository = contentProviderRepository;

            _logger = logger;

            _eventBus = eventBus;
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
            contentProvider.Id = contentProviderId;

            contentProvider.Type = ContentProviderContainerType.ContentProvider;

            contentProvider.ModifiedDate = DateTime.UtcNow;

            contentProvider.ModifiedByByUserId = UserClaimData.GetUserId(this.User.Claims);

            int statusCode = await _contentProviderRepository.UpdateContentProvider(contentProvider);

            if (statusCode == (int)System.Net.HttpStatusCode.OK)
            {
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
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<SasTokenDto>> GenerateToken(Guid contentProviderId)
        {
            SasTokenDto sasUri = await _contentProviderRepository.GenerateSaSToken(contentProviderId);

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

            if (contentProvider != null)
            {
                if (activate)
                {
                    contentProvider.ActivationDate = DateTime.UtcNow;
                }
                else
                {
                    contentProvider.DeactivationDate = DateTime.UtcNow;
                }

                contentProvider.ModifiedByByUserId = UserClaimData.GetUserId(this.User.Claims);

                contentProvider.ModifiedDate = DateTime.UtcNow;

                await _contentProviderRepository.UpdateContentProvider(contentProvider);

                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
        #endregion
    }
}
