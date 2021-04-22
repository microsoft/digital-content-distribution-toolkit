using blendnet.cms.repository.Interfaces;
using blendnet.common.dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.cms.api.Controllers
{
    /// <summary>
    /// Controller for managing Subscription metadata
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class SubscriptionMetadataController : Controller
    {
        private readonly ILogger _logger;

        private IContentProviderRepository _contentProviderRepository;

        public SubscriptionMetadataController(ILogger<SubscriptionMetadataController> logger, IContentProviderRepository contentProviderRepository)
        {
            this._logger = logger;
            this._contentProviderRepository = contentProviderRepository;
        }

        #region methods

        /// <summary>
        /// Gets all subscriptions for a content provider
        /// </summary>
        /// <param name="contentProviderId">Content Provider ID</param>
        /// <returns></returns>
        [HttpGet("{contentProviderId:guid}")]
        public async Task<ActionResult<List<ContentProviderSubscriptionMetadataDto>>> GetSubscriptions(Guid contentProviderId)
        {
            if (!await ValidContentProvider(contentProviderId))
            {
                return NotFound();
            }

            var result = await this._contentProviderRepository.GetSubscriptions(contentProviderId);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new subscription
        /// </summary>
        /// <param name="subscriptionMetadata">subscription data</param>
        /// <returns>ID of the created subscription</returns>
        [HttpPost]
        public async Task<ActionResult<String>> CreateSubscription(ContentProviderSubscriptionMetadataDto subscriptionMetadata)
        {
            var contentProviderId = subscriptionMetadata.ContentProviderId;

            if (!await ValidContentProvider(contentProviderId))
            {
                return NotFound();
            }

            subscriptionMetadata.SetIdentifiers();
            subscriptionMetadata.Type = ContentProviderContainerType.SubscriptionMetadata;

            var subscriptionId = await _contentProviderRepository.CreateSubscription(subscriptionMetadata);
            return Ok(subscriptionId);
        }

        #endregion

        #region helper methods

        /// <summary>
        /// Tells whether there is a known content provider for the given ID
        /// </summary>
        /// <param name="contentProviderId">content provider ID</param>
        /// <returns>`true` if a content provider exists for the ID, `false` otherwise</returns>
        private async Task<bool> ValidContentProvider(Guid contentProviderId)
        {
            var contentProvider = await this._contentProviderRepository.GetContentProviderById(contentProviderId);
            return contentProvider != default(ContentProviderDto);
        }

        #endregion
    }
}
