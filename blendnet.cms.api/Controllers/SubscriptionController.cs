// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.api.proxy.oms;
using blendnet.cms.api.Model;
using blendnet.cms.repository.Interfaces;
using blendnet.common.dto;
using blendnet.common.dto.cms;
using blendnet.common.dto.Cms;
using blendnet.common.dto.User;
using blendnet.common.infrastructure.Authentication;
using blendnet.common.infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

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
        private readonly IContentRepository _contentRepository;
        private readonly OrderProxy _orderProxy;
        IStringLocalizer<SharedResource> _stringLocalizer;
        AppSettings _appSettings;

        public SubscriptionController(
                                        ILogger<SubscriptionController> logger, 
                                        IContentProviderRepository contentProviderRepository,
                                        IContentRepository contentRepository,
                                        OrderProxy orderProxy,
                                        IStringLocalizer<SharedResource> stringLocalizer,
                                        IOptionsMonitor<AppSettings> optionsMonitor)
        {
            this._logger = logger;
            this._contentProviderRepository = contentProviderRepository;
            this._contentRepository = contentRepository;
            this._orderProxy = orderProxy;
            _stringLocalizer = stringLocalizer;
            _appSettings = optionsMonitor.CurrentValue;
        }

        #region methods

        /// <summary>
        /// Gets all subscriptions for a content provider
        /// </summary>
        /// <param name="contentProviderId">Content Provider ID</param>
        /// <returns></returns>
        [HttpGet(Name = nameof(GetSubscriptions))]
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
        [HttpGet("{subscriptionId:guid}", Name = nameof(GetSubscription))]
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
        [HttpPost(Name = nameof(CreateSubscription))]
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

                List<string> subscriptionDataErrors = ValidateSubscriptionData(subscription);

                listOfValidationErrors.AddRange(subscriptionDataErrors);

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

                if (listOfValidationErrors.Count > 0)
                {
                    return BadRequest(listOfValidationErrors);
                }
            }

            Guid callerUserId = UserClaimData.GetUserId(User.Claims);

            subscription.SetIdentifiers();
            subscription.ContentProviderId = contentProviderId;
            subscription.PublishMode = SubscriptionPublishMode.DRAFT;
            subscription.CreatedDate = now;
            subscription.CreatedByUserId = callerUserId;
            subscription.Type = ContentProviderContainerType.Subscription;

            var subscriptionId = await this._contentProviderRepository.CreateSubscription(subscription);
            return Ok(subscriptionId);
        }

        /// <summary>
        /// Publish Subscription which was in draft mode
        /// </summary>
        /// <param name="contentProviderId">contentProvider ID</param>
        /// <param name="subscriptionId">subscription ID</param>
        /// <returns></returns>
        [HttpPut("{subscriptionId:guid}/publish", Name = nameof(PublishSubscription))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> PublishSubscription(Guid contentProviderId, Guid subscriptionId)
        {
            List<string> listOfValidationErrors = new List<string>();

            if (!await ValidContentProvider(contentProviderId))
            {
                listOfValidationErrors.Add(String.Format(_stringLocalizer["CMS_ERR_0018"], contentProviderId));
            }

            var existingSubscription = await _contentProviderRepository.GetSubscription(contentProviderId, subscriptionId);

            if (existingSubscription is null)
            {
                 listOfValidationErrors.Add(String.Format(_stringLocalizer["CMS_ERR_0047"], subscriptionId));
            }

            List<string> subscriptionDataErrors = ValidateSubscriptionData(existingSubscription);
            
            listOfValidationErrors.AddRange(subscriptionDataErrors);

            if (listOfValidationErrors.Count > 0)
            {
                return BadRequest(listOfValidationErrors);
            }

            // If subscription is TVOD, also validate the contents in the subscription
            if (existingSubscription.SubscriptionType == SubscriptionType.TVOD)
            {
                int days = existingSubscription.DurationDays;

                if (_appSettings.MinDaysContentExistInSubscription > 0)
                {
                    days = Math.Min(days, _appSettings.MinDaysContentExistInSubscription);
                }

                DateTime subscriptionValidDate = existingSubscription.EndDate.AddDays(days);
                List<string> errorInTvodContents = await ValidateContentsInTvodSubscription(existingSubscription, subscriptionValidDate);
                listOfValidationErrors.AddRange(errorInTvodContents);
            }

            if (listOfValidationErrors.Count > 0)
            {
                return BadRequest(listOfValidationErrors);
            }

            Guid callerUserId = UserClaimData.GetUserId(User.Claims);

            existingSubscription.PublishMode = SubscriptionPublishMode.PUBLISHED;
            existingSubscription.ModifiedDate = DateTime.UtcNow;
            existingSubscription.ModifiedByByUserId = callerUserId;

            var response = await this._contentProviderRepository.UpdateSubscription(contentProviderId, existingSubscription);

            return NoContent();
        }

        /// <summary>
        /// Update a subscription which is in draft mode
        /// </summary>
        /// <param name="contentProviderId">contentProvider ID</param>
        /// <param name="subscriptionId">subscription ID</param>
        /// <param name="subscription">new subscription data</param>
        /// <returns></returns>
        [HttpPut("{subscriptionId:guid}", Name = nameof(UpdateSubscription))]
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
                    return BadRequest(new string[] {
                        String.Format(_stringLocalizer["CMS_ERR_0018"], contentProviderId),
                    });
                }

                var existingSubscription = await _contentProviderRepository.GetSubscription(contentProviderId, subscriptionId);
                
                if (existingSubscription is null)
                {
                    return NotFound();
                }

                // Only in draft mode subscription can be updated
                if (existingSubscription.PublishMode != SubscriptionPublishMode.DRAFT)
                {
                    listOfValidationErrors.Add(_stringLocalizer["CMS_ERR_0038"]);
                }

                List<string> subscriptionDataErrors = ValidateSubscriptionData(subscription);

                listOfValidationErrors.AddRange(subscriptionDataErrors);

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
            subscription.PublishMode = SubscriptionPublishMode.DRAFT;

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
        [HttpPost("{subscriptionId:guid}/updateEndDate", Name = nameof(UpdateSubscriptionEndDate))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<String>> UpdateSubscriptionEndDate(Guid contentProviderId,
                                                                            Guid subscriptionId,
                                                                            UpdateSubscriptionEndDateRequest request)
        {

            if (!await ValidContentProvider(contentProviderId))
            {
                return BadRequest(new string[] {
                    String.Format(_stringLocalizer["CMS_ERR_0018"], contentProviderId),
                });
            }

            var existingSubscription = await _contentProviderRepository.GetSubscription(contentProviderId, subscriptionId);
            if (existingSubscription is null)
            {
                return NotFound();
            }

            // subscription should be published
            if (existingSubscription.PublishMode != SubscriptionPublishMode.PUBLISHED)
            {
                return BadRequest(new string[] {
                    _stringLocalizer["CMS_ERR_0039"],
                });
            }

            // end date should be after start date
            if (request.EndDate < existingSubscription.StartDate)
            {
                return BadRequest(new string[] {
                    _stringLocalizer["CMS_ERR_0021"],
                });
            }

            // For TVOD subscription, check for content broadcast end date with request end data
            if(existingSubscription.SubscriptionType == SubscriptionType.TVOD)
            {
                int days = existingSubscription.DurationDays;

                if(_appSettings.MinDaysContentExistInSubscription > 0)
                {
                    days = Math.Min(days, _appSettings.MinDaysContentExistInSubscription);
                }

                DateTime subscriptionValidityDate = request.EndDate.AddDays(days);
                List<string> listOfValidationErrors = await ValidateContentsInTvodSubscription (existingSubscription, subscriptionValidityDate);
                
                if (listOfValidationErrors.Count > 0)
                {
                    return BadRequest(listOfValidationErrors);
                }
            }
            

            // update is allowed only if there are no orders after the requested end date
            if (await OrdersExistForSubscription(contentProviderId, subscriptionId, request.EndDate))
            {
                return BadRequest(new string[] {
                    string.Format(_stringLocalizer["CMS_ERR_0034"], subscriptionId),
                });
            }

            Guid callerUserId = UserClaimData.GetUserId(User.Claims);

            // Now we are OK to update the end date
            existingSubscription.EndDate = request.EndDate;
            existingSubscription.ModifiedDate = DateTime.UtcNow;
            existingSubscription.ModifiedByByUserId = callerUserId;

            await _contentProviderRepository.UpdateSubscription(existingSubscription.ContentProviderId, existingSubscription);
            return NoContent();
        }

        /// <summary>
        /// Delete a subscription if it is in draft mode
        /// </summary>
        /// <param name="contentProviderId">contentProvider ID</param>
        /// <param name="subscriptionId">subscription ID</param>
        /// <returns></returns>
        [HttpDelete("{subscriptionId:guid}", Name = nameof(DeleteSubscription))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<ActionResult<String>> DeleteSubscription(Guid contentProviderId,
                                                                    Guid subscriptionId)
        {
            if (!await ValidContentProvider(contentProviderId))
            {
                return BadRequest(new string[] {
                    String.Format(_stringLocalizer["CMS_ERR_0018"], contentProviderId),
                });
            }

            var existingSubscription = await _contentProviderRepository.GetSubscription(contentProviderId, subscriptionId);
            if (existingSubscription is null)
            {
                return NotFound();
            }

            // Cannot delete subscription if it is published
            if (existingSubscription.PublishMode != SubscriptionPublishMode.DRAFT)
            {
                return BadRequest(new string[]
                {
                    _stringLocalizer["CMS_ERR_0040"]
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

            if (subscription.SubscriptionType == SubscriptionType.TVOD)
            {
                listOfValidationErrors.AddRange(ValidateTvodSubscriptionType(subscription));
            }

            if (subscription.PublishMode != SubscriptionPublishMode.DRAFT)
            {
                listOfValidationErrors.Add(_stringLocalizer["CMS_ERR_0042"]);
            }

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

        /// <summary>
        /// Validate if content list count in subscription is greater than 0 and less than max limit
        /// </summary>
        private List<string> ValidateTvodSubscriptionType(ContentProviderSubscriptionDto subscription)
        {
            List<string> listOfValidationErrors = new List<string>();
            if (subscription.ContentIds.Count <= 0)
            {
                listOfValidationErrors.Add(_stringLocalizer["CMS_ERR_0043"]);
            }

            if (subscription.ContentIds.Count > _appSettings.SubscriptionContentListMaxLimit)
            {
                listOfValidationErrors.Add(String.Format(_stringLocalizer["CMS_ERR_0044"], _appSettings.SubscriptionContentListMaxLimit));
            }

            // Check if duplicate content id is not present in the list
            listOfValidationErrors.AddRange(DuplicateContentIdCheck(subscription.ContentIds));

            return listOfValidationErrors;
        }


        /// <summary>
        /// Validate Content in TVOD subscription and the subscription start date should be after every content broadcast start date
        /// and subscription end date should be before every content broadcast end date 
        /// </summary>
        private async Task<List<string>> ValidateContentsInTvodSubscription(ContentProviderSubscriptionDto subscription, DateTime subscriptionValidityDate)
        {
            List<string> listOfValidationErrors = new List<string>();

            List<Content> contentList = await _contentRepository.GetContentByIds(subscription.ContentIds);

            // Validate if there is invalid content id in the list
            ValidateContentIds(subscription.ContentIds, contentList, listOfValidationErrors);

            if (contentList != null && contentList.Count > 0)
            {
                foreach (Content content in contentList)
                {
                    // Validate if contents in subscription is for the same content provider
                    if(content.ContentProviderId.ToString() != subscription.ContentProviderId.ToString())
                    {
                        listOfValidationErrors.Add(String.Format(_stringLocalizer["CMS_ERR_0049"], content.Title, subscription.ContentProviderId.ToString()));
                    }

                    if (content.ContentBroadcastStatus == ContentBroadcastStatus.BroadcastOrderComplete)
                    {
                        if (subscription.StartDate < content.ContentBroadcastedBy.BroadcastRequest.StartDate)
                        {
                            listOfValidationErrors.Add(_stringLocalizer["CMS_ERR_0045"]);
                        }

                        if (subscriptionValidityDate > content.ContentBroadcastedBy.BroadcastRequest.EndDate)
                        {
                            listOfValidationErrors.Add(_stringLocalizer["CMS_ERR_0046"]);
                        }
                    }
                    else
                    {
                        listOfValidationErrors.Add(String.Format(_stringLocalizer["CMS_ERR_0050"], content.Title ,ContentBroadcastStatus.BroadcastOrderComplete));
                    }
                }
            }
            else
            {
                listOfValidationErrors.Add(_stringLocalizer["CMS_ERR_0030"]);
            }

            return listOfValidationErrors;
        }

        /// <summary>
        /// Validate Content Ids
        /// </summary>
        /// <param name="parentIds"></param>
        /// <param name="retrievedContents"></param>
        /// <param name="errorList"></param>
        private void ValidateContentIds(List<Guid> parentIds, List<Content> retrievedContents, List<string> errorList)
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
        /// List of Invalid Content Ids
        /// </summary>
        /// <param name="parentList"></param>
        /// <param name="retrievedContents"></param>
        /// <returns></returns>
        private List<Guid> GetInvalidContentIds(List<Guid> parentList, List<Content> retrievedContents)
        {
            List<Guid> invalidContentIds = new List<Guid>();

            foreach (Guid contentId in parentList)
            {
                if (!retrievedContents.Exists(c => c.Id.Value.ToString().Equals(contentId.ToString())))
                {
                    invalidContentIds.Add(contentId);
                }
            }

            return invalidContentIds;
        }

        /// <summary>
        /// Duplicate Content Id Check
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        private List<string> DuplicateContentIdCheck(List<Guid> contents)
        {
            List<string> errorList = new List<string>();

            HashSet<Guid> contentIds = new HashSet<Guid>();

            foreach (Guid contentId in contents)
            {
                if (!contentIds.Add(contentId))
                {
                    // Duplicate ContentIds in the file
                    string info = String.Format(_stringLocalizer["CMS_ERR_0048"], contentId);
                    errorList.Add(info);
                }

            }

            return errorList;
        }

        #endregion
    }
}
