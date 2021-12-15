using blendnet.api.proxy.oms;
using blendnet.cms.api.Model;
using blendnet.cms.repository.Interfaces;
using blendnet.common.dto;
using blendnet.common.dto.User;
using blendnet.common.infrastructure.Authentication;
using blendnet.common.infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace blendnet.cms.api.Controllers
{
    /// <summary>
    /// Controller for managing Subscription metadata
    /// </summary>
    [Route("api/v{version:apiVersion}/ContentProvider/{contentProviderId:guid}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
    public class SubscriptionController : Controller
    {
        private readonly ILogger _logger;
        private readonly IContentProviderRepository _contentProviderRepository;
        private readonly OrderProxy _orderProxy;
        IStringLocalizer<SharedResource> _stringLocalizer;

        public SubscriptionController(
                                        ILogger<SubscriptionController> logger, 
                                        IContentProviderRepository contentProviderRepository,
                                        OrderProxy orderProxy,
                                        IStringLocalizer<SharedResource> stringLocalizer)
        {
            this._logger = logger;
            this._contentProviderRepository = contentProviderRepository;
            this._orderProxy = orderProxy;
            _stringLocalizer = stringLocalizer;
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
            result = result.OrderByDescending(o => o.CreatedDate).ToList();
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
        public async Task<ActionResult<ContentProviderSubscriptionDto>> GetSubscription(Guid contentProviderId, Guid subscriptionId)
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
            DateTime now = DateTime.UtcNow;

            // validations
            {
                List<string> listOfValidationErrors = new List<string>();

                if (!await ValidContentProvider(contentProviderId))
                {
                    listOfValidationErrors.Add(String.Format(_stringLocalizer["CMS_ERR_0018"], contentProviderId));
                }

                // convert dates to IST, ignoring the time part
                var subscriptionStartDate = subscription.StartDate.ToIndiaStandardTime().Date;
                var startOfDayNow = now.ToIndiaStandardTime().Date;

                if (subscriptionStartDate < startOfDayNow)
                {
                    listOfValidationErrors.Add(_stringLocalizer["CMS_ERR_0019"]);
                }

                if (subscription.EndDate < now)
                {
                    listOfValidationErrors.Add(_stringLocalizer["CMS_ERR_0019"]);
                }

                listOfValidationErrors.AddRange(ValidateSubscriptionData(subscription));

                if (listOfValidationErrors.Count > 0)
                {
                    return BadRequest(listOfValidationErrors);
                }
            }

            Guid callerUserId = UserClaimData.GetUserId(User.Claims);

            subscription.SetIdentifiers();
            subscription.ContentProviderId = contentProviderId;
            subscription.CreatedDate = now;
            subscription.CreatedByUserId = callerUserId;
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
                    listOfValidationErrors.Add(String.Format(_stringLocalizer["CMS_ERR_0018"], contentProviderId));
                }

                listOfValidationErrors.AddRange(ValidateSubscriptionData(subscription));

                // Update is allowed only if there are no orders against this subscription ever
                if (await OrdersExistForSubscription(contentProviderId, subscriptionId, DateTime.MinValue))
                {
                    listOfValidationErrors.Add(string.Format(_stringLocalizer["CMS_ERR_0034"], subscriptionId));
                }

                if (listOfValidationErrors.Count > 0)
                {
                    return BadRequest(listOfValidationErrors);
                }
            }

            Guid callerUserId = UserClaimData.GetUserId(User.Claims);

            subscription.Id = subscriptionId;
            subscription.ContentProviderId = contentProviderId;
            subscription.ModifiedDate = DateTime.UtcNow;
            subscription.ModifiedByByUserId = callerUserId;
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
        /// Update a subscription's end date
        /// </summary>
        /// <param name="contentProviderId">contentProvider ID</param>
        /// <param name="subscriptionId">subscription ID</param>
        /// <param name="request">new data</param>
        /// <returns></returns>
        [HttpPost("{subscriptionId:guid}/updateEndDate")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<String>> UpdateSubscriptionEndDate(Guid contentProviderId,
                                                                            Guid subscriptionId,
                                                                            UpdateSubscriptionEndDateRequest request)
        {
            var existingSubscription = await _contentProviderRepository.GetSubscription(contentProviderId, subscriptionId);
            if (existingSubscription is null)
            {
                return NotFound();
            }

            // end date should be after start date
            if (request.EndDate < existingSubscription.StartDate)
            {
                return BadRequest(new string[] {
                    _stringLocalizer["CMS_ERR_0021"],
                });
            }

            // update is allowed only if there are no orders after the requested end date
            if (await OrdersExistForSubscription(contentProviderId, subscriptionId, request.EndDate))
            {
                return BadRequest(new string[] {
                    string.Format(_stringLocalizer["CMS_ERR_0034"], subscriptionId),
                });
            }

            // Now we are OK to update the end date
            existingSubscription.EndDate = request.EndDate;

            await _contentProviderRepository.UpdateSubscription(existingSubscription.ContentProviderId, existingSubscription);
            return NoContent();
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
            // Delete is allowed only if there are no orders ever against this subscription
            if (await OrdersExistForSubscription(contentProviderId, subscriptionId, DateTime.MinValue))
            {
                return BadRequest(new string[] {
                    string.Format(_stringLocalizer["CMS_ERR_0034"], subscriptionId)
                });
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

        private async Task<bool> OrdersExistForSubscription(Guid contentProviderId, Guid subscriptionId, DateTime cutoffDate)
        {
            var ordersForSubscription = await _orderProxy.GetOrdersCountBySubscriptionId(contentProviderId, subscriptionId, cutoffDate);
            
            return ordersForSubscription > 0;
        }

        private List<string> ValidateSubscriptionData(ContentProviderSubscriptionDto subscription)
        {
            List<string> listOfValidationErrors = new List<string>();

            DateTime now = DateTime.UtcNow;

            if (subscription.StartDate >= subscription.EndDate)
            {
                listOfValidationErrors.Add(_stringLocalizer["CMS_ERR_0021"]);
            }

            if (subscription.DurationDays <= 0)
            {
                listOfValidationErrors.Add(_stringLocalizer["CMS_ERR_0022"]);
            }

            if (subscription.Price <= 0)
            {
                listOfValidationErrors.Add(_stringLocalizer["CMS_ERR_0023"]);
            }

            if (subscription.IsRedeemable && subscription.RedemptionValue <= 0)
            {
                listOfValidationErrors.Add(_stringLocalizer["CMS_ERR_0026"]);
            }

            if (string.IsNullOrWhiteSpace(subscription.Title))
            {
                listOfValidationErrors.Add(_stringLocalizer["CMS_ERR_0031"]);
            }

            return listOfValidationErrors;
        }

        #endregion
    }
}
