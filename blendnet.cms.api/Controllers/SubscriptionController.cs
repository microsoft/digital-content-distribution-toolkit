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
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<ContentProviderSubscriptionDto>>> GetSubscriptions(Guid contentProviderId)
        {
            var result = await this._contentProviderRepository.GetSubscriptions(contentProviderId);
            if (result.Count > 0)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Gets subscription for the given ID and content provider ID
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        [HttpGet("{subscriptionId:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
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
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<String>> CreateSubscription(Guid contentProviderId,
                                                                    ContentProviderSubscriptionDto subscription)
        {
            // validations
            {
                List<string> listOfValidationErrors = new List<string>();

                if (!await ValidContentProvider(contentProviderId))
                {
                    listOfValidationErrors.Add($"No content provider found for ID {contentProviderId}");
                }

                listOfValidationErrors.AddRange(ValidateSubscriptionData(subscription));

                if (listOfValidationErrors.Count > 0)
                {
                    return BadRequest(listOfValidationErrors);
                }
            }

            subscription.SetIdentifiers();
            subscription.ContentProviderId = contentProviderId;
            subscription.DateCreated = subscription.DateModified = DateTime.UtcNow;
            subscription.Type = ContentProviderContainerType.Subscription;

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
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult<String>> UpdateSubscription(Guid contentProviderId,
                                                                    Guid subscriptionId,
                                                                    ContentProviderSubscriptionDto subscription)
        {
            // Validations
            {
                List<string> listOfValidationErrors = new List<string>();

                if (!await ValidContentProvider(contentProviderId))
                {
                    listOfValidationErrors.Add($"No content provider found for ID {contentProviderId}");
                }

                listOfValidationErrors.AddRange(ValidateSubscriptionData(subscription));

                if (listOfValidationErrors.Count > 0)
                {
                    return BadRequest(listOfValidationErrors);
                }
            }

            subscription.Id = subscriptionId;
            subscription.ContentProviderId = contentProviderId;
            subscription.DateModified = DateTime.UtcNow;
            subscription.Type = ContentProviderContainerType.Subscription;

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
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<ActionResult<String>> DeleteSubscription(Guid contentProviderId,
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
            // TODO: implement this once Orders subsystem is available
            // For now, returning false always
            return Task.FromResult(false);
        }

        private List<string> ValidateSubscriptionData(ContentProviderSubscriptionDto subscription)
        {
            List<string> listOfValidationErrors = new List<string>();

            DateTime now = DateTime.UtcNow;

            if (subscription.StartDate < now)
            {
                listOfValidationErrors.Add("StartDate is in past");
            }

            if (subscription.EndDate < now)
            {
                listOfValidationErrors.Add("EndDate is in past");
            }

            if (subscription.StartDate >= subscription.EndDate)
            {
                listOfValidationErrors.Add("Start Date has to be before EndDate");
            }

            if (subscription.DurationDays <= 0)
            {
                listOfValidationErrors.Add("DurationDays should be minimum 1");
            }

            if (subscription.Price < 0)
            {
                listOfValidationErrors.Add("Price should be minimum 0");
            }

            return listOfValidationErrors;
        }

        #endregion
    }
}
