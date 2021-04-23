using blendnet.cms.repository.Interfaces;
using blendnet.common.dto;
using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
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
    public class ContentProviderController : ControllerBase
    {

        private readonly ILogger _logger;

        private IEventBus _eventBus;

        private IContentProviderRepository _contentProviderRepository;

        public ContentProviderController(IContentProviderRepository contentProviderRepository,
                                            ILogger<ContentProviderController> logger,
                                            IEventBus eventBus)
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
        public async Task<ActionResult<List<ContentProviderDto>>> GetContentProviders()
        {
            var contentProviders = await _contentProviderRepository.GetContentProviders();

            return Ok(contentProviders);
        }

        /// <summary>
        /// Get Content Provider
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        [HttpGet("{contentProviderId:guid}", Name = nameof(GetContentProvider))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<ContentProviderDto>> GetContentProvider(Guid contentProviderId)
        {
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
        public async Task<ActionResult<string>> CreateContentProvider(ContentProviderDto contentProvider)
        {
            //generate ids for content provider and administrator
            contentProvider.SetIdentifiers();
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
        public async Task<ActionResult> UpdateContentProvider(Guid contentProviderId, ContentProviderDto contentProvider)
        {
            contentProvider.Id = contentProviderId;
            contentProvider.Type = ContentProviderContainerType.ContentProvider;

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
        public async Task<ActionResult> DeactivateContentProvider(Guid contentProviderId)
        {
            return await ActivateDeactivateContentProvider(contentProviderId, false);
        }

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
                    contentProvider.ActivationDate = DateTime.Now;
                    contentProvider.IsActive = true;
                }
                else
                {
                    contentProvider.DeactivationDate = DateTime.Now;
                    contentProvider.IsActive = false;
                }

                await _contentProviderRepository.UpdateContentProvider(contentProvider);

                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Generates SAS token for the content provider
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        [HttpGet("{contentProviderId:guid}/generateSaS")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
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
    }
}
