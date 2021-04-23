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
    [Route("api/v{version:apiVersion}/ContentProvider/{contentProviderId:guid}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class SubscriptionController : Controller
    {
        private readonly ILogger _logger;

        private IContentProviderRepository _contentProviderRepository;

        public SubscriptionController(ILogger<SubscriptionController> logger, IContentProviderRepository contentProviderRepository)
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
        [HttpGet]
        public async Task<ActionResult<List<ContentProviderSubscriptionDto>>> GetSubscriptions(Guid contentProviderId)
        {
            var result = await this._contentProviderRepository.GetSubscriptions(contentProviderId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<ContentProviderSubscriptionDto>>> GetSubscription(Guid contentProviderId, Guid subscriptionId)
        {
            var result = await this._contentProviderRepository.GetSubscription(contentProviderId, subscriptionId);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Creates a new subscription
        /// </summary>
        /// <param name="subscription">subscription data</param>
        /// <returns>ID of the created subscription</returns>
        [HttpPost]
        public async Task<ActionResult<String>> CreateSubscription( Guid contentProviderId,
                                                                    ContentProviderSubscriptionDto subscription)
        {
            if (!await ValidContentProvider(contentProviderId))
            {
                return NotFound();
            }

            subscription.SetIdentifiers();
            subscription.ContentProviderId = contentProviderId;
            subscription.Type = ContentProviderContainerType.SubscriptionMetadata;

            var subscriptionId = await this._contentProviderRepository.CreateSubscription(subscription);
            return Ok(subscriptionId);
        }

        /// <summary>
        /// Update a subscription
        /// </summary>
        /// <param name="contentProviderId">contentProvider ID</param>
        /// <param name="subscriptionId">subscription ID</param>
        /// <param name="subscription">new subscription data</param>
        /// <returns></returns>
        [HttpPut("{subscriptionId:guid}")]
        public async Task<ActionResult<String>> UpdateSubscription( Guid contentProviderId,
                                                                    Guid subscriptionId,
                                                                    ContentProviderSubscriptionDto subscription)
        {
            if (!await ValidContentProvider(contentProviderId))
            {
                return NotFound();
            }

            // Update is allowed only if there are no orders against this subscription
            if (await OrdersExistForSubscription(subscriptionId))
            {
                return Conflict();
            }

            subscription.Id = subscriptionId;
            subscription.ContentProviderId = contentProviderId;
            subscription.Type = ContentProviderContainerType.SubscriptionMetadata;

            int response = await this._contentProviderRepository.UpdateSubscription(contentProviderId, subscription);
            if (response == (int)System.Net.HttpStatusCode.OK)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Delete a subscription
        /// </summary>
        /// <param name="contentProviderId">contentProvider ID</param>
        /// <param name="subscriptionId">subscription ID</param>
        /// <returns></returns>
        [HttpDelete("{subscriptionId:guid}")]
        public async Task<ActionResult<String>> DeleteSubscription( Guid contentProviderId,
                                                                    Guid subscriptionId)
        {
            // Delete is allowed only if there are no orders against this subscription
            if (await OrdersExistForSubscription(subscriptionId))
            {
                return Conflict();
            }

            var response = await this._contentProviderRepository.DeleteSubscription(contentProviderId, subscriptionId);
            if (response == (int)System.Net.HttpStatusCode.NoContent)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
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

        private Task<bool> OrdersExistForSubscription(Guid subscriptionId)
        {
            return Task.FromResult(false);
        }

        #endregion
    }
}
