using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using blendnet.common.dto;
using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using blendnet.crm.contentprovider.api.Model;
using blendnet.crm.contentprovider.repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace blendnet.crm.contentprovider.api.Controllers
{
    /// <summary>
    /// Class to support content provider and administrator operations.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ContentProvidersController : ControllerBase
    {
        private readonly ILogger _logger;

        private IEventBus _eventBus;

        private IContentProviderRepository _contentProviderRepository;

        public ContentProvidersController(  IContentProviderRepository contentProviderRepository, 
                                            ILogger<ContentProvidersController> logger,
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
        [HttpGet("{contentProviderId:guid}",Name = nameof(GetContentProvider))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<ContentProviderDto>> GetContentProvider(Guid contentProviderId)
        {
            var contentProvider = await _contentProviderRepository.GetContentProviderById(contentProviderId);

            if (contentProvider != default(ContentProviderDto))
            {
                return Ok(contentProvider);
            }else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Create Content Provider
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ApiConventionMethod(typeof(DefaultApiConventions),nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult<string>> CreateContentProvider(ContentProviderDto contentProvider)
        {
            var contentProviderId = await _contentProviderRepository.CreateContentProvider(contentProvider);

            //publish the event
            ContentProviderCreatedIntegrationEvent contentProviderCreatedIntegrationEvent = new ContentProviderCreatedIntegrationEvent()
            {
                ContentProviderId = contentProviderId.ToString(),
                ContainerBaseName = "eroscms"
            };

            await _eventBus.Publish(contentProviderCreatedIntegrationEvent);

            return CreatedAtAction( nameof(GetContentProvider), 
                                    new { contentProviderId = contentProviderId, Version = ApiVersion.Default.MajorVersion.ToString() },
                                    contentProviderId.ToString());
        }

        /// <summary>
        /// Updates the content provider id
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <param name="contentProvider"></param>
        /// <returns></returns>
        [HttpPost("{contentProviderId:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> UpdateContentProvider(Guid contentProviderId , ContentProviderDto contentProvider)
        {
            contentProvider.Id = contentProviderId;

            int recordsAffected = await _contentProviderRepository.UpdateContentProvider(contentProvider);

            if (recordsAffected > 0)
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
            int recordsAffected = await _contentProviderRepository.DeleteContentProvider(contentProviderId);

            if (recordsAffected > 0)
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
            return  await ActivateDeactivateContentProvider(contentProviderId, true);
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
        #endregion

        #region Content Administrator Methods

        /// <summary>
        /// List all content admins
        /// </summary>
        /// <returns></returns>
        [HttpGet("{contentProviderId:guid}/ContentAdministrators")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<ContentAdministratorDto>>> GetContentAdministrators(Guid contentProviderId)
        {
            var contentProvider = await _contentProviderRepository.GetContentProviderById(contentProviderId);

            if (contentProvider != default(ContentProviderDto) 
                && contentProvider.ContentAdministrators != null 
                && contentProvider.ContentAdministrators.Count > 0)
            {
                return Ok(contentProvider.ContentAdministrators);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// This is actually an update on content provider only
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <param name="contentAdministrator"></param>
        /// <returns></returns>
        [HttpPost("{contentProviderId:guid}/ContentAdministrators")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> CreateContentAdministrator(Guid contentProviderId,ContentAdministratorDto contentAdministrator)
        {
            var contentProvider = await _contentProviderRepository.GetContentProviderById(contentProviderId);

            if (contentProvider != null)
            {
                contentAdministrator.ResetIdentifiers();

                if (contentProvider.ContentAdministrators == null)
                {
                    contentProvider.ContentAdministrators = new List<ContentAdministratorDto>();
                }

                contentProvider.ContentAdministrators.Add(contentAdministrator);

                await _contentProviderRepository.UpdateContentProvider(contentProvider);

                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// DeActivate Content Administrator
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <param name="contentAdministratorId"></param>
        /// <param name="contentAdministrator"></param>
        /// <returns></returns>
        [HttpPost("{contentProviderId:guid}/ContentAdministrators/{contentAdministratorId:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> UpdateContentAdministrator(Guid contentProviderId, Guid contentAdministratorId,ContentAdministratorDto contentAdministrator)
        {
            //set the id on the object
            contentAdministrator.Id = contentAdministratorId;

            var contentProvider = await _contentProviderRepository.GetContentProviderById(contentProviderId);

            if (contentProvider != null)
            {
                //Get the existing adminstrator
                var contentAdmistrator = contentProvider.ContentAdministrators.Where(ca => ca.Id == contentAdministratorId).FirstOrDefault();

                if (contentAdmistrator == null)
                {
                    return NotFound();

                }else
                {
                    contentProvider.ContentAdministrators.Remove(contentAdmistrator);

                    contentProvider.ContentAdministrators.Add(contentAdministrator);

                    await _contentProviderRepository.UpdateContentProvider(contentProvider);

                    return NoContent();
                }
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Activate Content Administrator
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <param name="contentAdministratorId"></param>
        /// <returns></returns>
        [HttpPost("{contentProviderId:guid}/ContentAdministrators/{contentAdministratorId:guid}/activate")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> ActivateContentAdministrator(Guid contentProviderId, Guid contentAdministratorId)
        {
            return await ActivateDeActivateContentAdministrator(contentProviderId, contentAdministratorId, true);
        }

        /// <summary>
        /// Deactivate Content Administrator
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <param name="contentAdministratorId"></param>
        /// <returns></returns>
        [HttpPost("{contentProviderId:guid}/ContentAdministrators/{contentAdministratorId:guid}/deactivate")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> DeactivateContentAdministrator(Guid contentProviderId, Guid contentAdministratorId)
        {
            return await ActivateDeActivateContentAdministrator(contentProviderId, contentAdministratorId, false);
        }


        /// <summary>
        /// Activates or Deactivates content administrator
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <param name="contentAdministratorId"></param>
        /// <param name="activate"></param>
        /// <returns></returns>
        private async Task<ActionResult> ActivateDeActivateContentAdministrator(Guid contentProviderId, Guid contentAdministratorId, bool activate)
        {
            var contentProvider = await _contentProviderRepository.GetContentProviderById(contentProviderId);

            if (contentProvider != null)
            {
                //Get the existing adminstrator
                var contentAdmistrator = contentProvider.ContentAdministrators.Where(ca => ca.Id == contentAdministratorId).FirstOrDefault();

                if (contentAdmistrator == null)
                {
                    return NotFound();
                }
                else
                {
                    if (activate)
                    {
                        contentAdmistrator.ActivationDate = DateTime.Now;

                        contentAdmistrator.IsActive = true;
                    }else
                    {
                        contentAdmistrator.DeactivationDate = DateTime.Now;

                        contentAdmistrator.IsActive = false;
                    }

                    await _contentProviderRepository.UpdateContentProvider(contentProvider);

                    return NoContent();
                }
            }
            else
            {
                return NotFound();
            }
        }

        #endregion
    }
}
