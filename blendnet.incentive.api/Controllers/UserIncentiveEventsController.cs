using blendnet.api.proxy.Cms;
using blendnet.common.dto;
using blendnet.common.dto.Events;
using blendnet.common.dto.Incentive;
using blendnet.common.dto.User;
using blendnet.common.infrastructure;
using blendnet.common.infrastructure.Extensions;
using blendnet.incentive.api.Model;
using blendnet.incentive.repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.incentive.api.Controllers
{
    /// <summary>
    /// Controller to record user activity events, which are used for calculating incentives for users
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class UserIncentiveEventsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEventRepository _eventRepository;
        private readonly ContentProxy _contentProxy;
        private readonly IEventBus _eventBus;
        private readonly IStringLocalizer<SharedResource> _stringLocalizer;

        public UserIncentiveEventsController(IEventRepository eventRepository,
                                                ILogger<UserIncentiveEventsController> logger,
                                                ContentProxy contentProxy,
                                                IEventBus eventBus,
                                                IStringLocalizer<SharedResource> stringLocalizer)
        {
            _eventRepository = eventRepository;
            _logger = logger;
            _contentProxy = contentProxy;
            _eventBus = eventBus;
            _stringLocalizer = stringLocalizer;
        }

        #region Incentive management methods

        /// <summary>
        /// API to record Sign-in event from consumer app
        /// </summary>
        /// <param name="signinUserEventRequest"></param>
        /// <returns></returns>
        [HttpPost("recordSignin")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult> RecordSigninEvent(UserEventRequest signinUserEventRequest)
        {
            Guid callerUserId = UserClaimData.GetUserId(User.Claims);
            string callerPhoneNumber = this.User.Identity.Name;
            DateTime now = DateTime.UtcNow;

            // validation - date can't be from future
            if (signinUserEventRequest.OriginalTime > now)
            {
                // this event is from future, so reject
                string errorString = string.Format(_stringLocalizer["INC_ERR_0031"], EventType.CONSUMER_INCOME_FIRST_SIGNIN, callerUserId);
                _logger.LogInformation(errorString);
                return BadRequest(new string[] { errorString });
            }

            // Validate event existence
            var existingEvents = await _eventRepository.GetEvents(new EventCriteriaRequest()
            {
                AudienceType = AudienceType.CONSUMER,
                EventCreatedFor = callerPhoneNumber,
                EventTypes = new List<EventType>() { EventType.CONSUMER_INCOME_FIRST_SIGNIN },
            });

            if (existingEvents == null || existingEvents.Count == 0) // no event exist
            {
                // create event from request
                var integrationEvent = new UserSigninIncentiveIntegrationEvent()
                {
                    UserId = callerUserId,
                    UserPhone = callerPhoneNumber,
                    OriginalTime = signinUserEventRequest.OriginalTime,
                };

                await _eventBus.Publish(integrationEvent);
                return Ok();
            }
            else
            {
                // event already exists so reject
                string errorString = string.Format(_stringLocalizer["INC_ERR_0032"], EventType.CONSUMER_INCOME_FIRST_SIGNIN, callerUserId);
                _logger.LogInformation(errorString);
                return BadRequest(new string[] { errorString });
            }
        }

        /// <summary>
        /// API to record App Launch event from consumer app
        /// </summary>
        /// <param name="appLaunchUserEventRequest"></param>
        /// <returns></returns>
        [HttpPost("recordAppLaunch")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult> RecordAppLaunchEvent(UserEventRequest appLaunchUserEventRequest)
        {
            Guid callerUserId = UserClaimData.GetUserId(this.User.Claims);
            string callerPhoneNumber = this.User.Identity.Name;
            DateTime now = DateTime.UtcNow;

            // validation - date can't be from future
            if (appLaunchUserEventRequest.OriginalTime > now)
            {
                // this event is from future, so reject
                string errorString = string.Format(_stringLocalizer["INC_ERR_0031"], EventType.CONSUMER_INCOME_APP_ONCE_OPEN, callerUserId);
                _logger.LogInformation(errorString);
                return BadRequest(new string[] { errorString });
            }

            // Validate event existence
            DateTime startOfEventDay = appLaunchUserEventRequest.OriginalTime
                                            .ToIndiaStandardTime()  // convert to IST
                                            .Date // take date componet. This gives StartOfDay in IST
                                            .UtcFromIndiaStandardTime(); // Convert back to UTC
            DateTime endOfEventDay = startOfEventDay.AddDays(1).AddTicks(-1);
            var existingEvents = await _eventRepository.GetEvents(new EventCriteriaRequest()
            {
                AudienceType = AudienceType.CONSUMER,
                EventCreatedFor = callerPhoneNumber,
                EventTypes = new List<EventType>() { EventType.CONSUMER_INCOME_APP_ONCE_OPEN },
                StartDate = startOfEventDay,
                EndDate = endOfEventDay,
            });

            if (existingEvents == null || existingEvents.Count == 0) // no event exist
            {
                // create event from request
                UserAppOpenIncentiveIntegrationEvent integrationEvent = new UserAppOpenIncentiveIntegrationEvent()
                {
                    UserId = callerUserId,
                    UserPhone = callerPhoneNumber,
                    OriginalTime = appLaunchUserEventRequest.OriginalTime,
                };

                await _eventBus.Publish(integrationEvent);
                return Ok();
            }
            else
            {
                // event already exists so reject
                string errorString = string.Format(_stringLocalizer["INC_ERR_0032"], EventType.CONSUMER_INCOME_APP_ONCE_OPEN, callerUserId);
                _logger.LogInformation(errorString);
                return BadRequest(new string[] { errorString });
            }
        }

        /// <summary>
        /// API to record Content Streamed event from consumer app
        /// </summary>
        /// <param name="contentStreamedUserEventRequest"></param>
        /// <returns></returns>
        [HttpPost("recordContentStreamed")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult> RecordContentStreamedEvent(UserEventRequest contentStreamedUserEventRequest)
        {
            Guid callerUserId = UserClaimData.GetUserId(this.User.Claims);
            string callerPhoneNumber = this.User.Identity.Name;
            DateTime now = DateTime.UtcNow;

            // validation - date can't be from future
            if (contentStreamedUserEventRequest.OriginalTime > now)
            {
                // this event is from future, so reject
                string errorString = string.Format(_stringLocalizer["INC_ERR_0031"], EventType.CONSUMER_INCOME_STREAMED_CONTENT_ONCE_PER_CONTENTPROVIDER, callerUserId);
                _logger.LogInformation(errorString);
                return BadRequest(new string[] { errorString });
            }

            // validation - content info - contentId
            if (!contentStreamedUserEventRequest.ContentId.HasValue)
            {
                string errorString = string.Format(_stringLocalizer["INC_ERR_0033"], EventType.CONSUMER_INCOME_STREAMED_CONTENT_ONCE_PER_CONTENTPROVIDER, callerUserId);
                _logger.LogInformation(errorString);
                return BadRequest(new string[] { errorString });
            }

            // validation - content info - content
            var streamedContent = await _contentProxy.GetContentById(contentStreamedUserEventRequest.ContentId.Value);
            if (streamedContent == null)
            {
                string errorString = string.Format(_stringLocalizer["INC_ERR_0034"], EventType.CONSUMER_INCOME_STREAMED_CONTENT_ONCE_PER_CONTENTPROVIDER, contentStreamedUserEventRequest.ContentId.Value, callerUserId);
                _logger.LogInformation(errorString);
                return BadRequest(new string[] { errorString });
            }

            // validation - existing events
            DateTime startOfEventDay = contentStreamedUserEventRequest.OriginalTime
                                            .ToIndiaStandardTime()  // convert to IST
                                            .Date // take date componet. This gives StartOfDay in IST
                                            .UtcFromIndiaStandardTime(); // Convert back to UTC
            DateTime endOfEventDay = startOfEventDay.AddDays(1).AddTicks(-1);
            var existingEvents = await _eventRepository.GetEvents(new EventCriteriaRequest()
            {
                AudienceType = AudienceType.CONSUMER,
                EventCreatedFor = callerPhoneNumber,
                EventTypes = new List<EventType>() { EventType.CONSUMER_INCOME_STREAMED_CONTENT_ONCE_PER_CONTENTPROVIDER },
                StartDate = startOfEventDay,
                EndDate = endOfEventDay,
            });

            if (existingEvents == null)
            {
                // force empty list
                existingEvents = new List<IncentiveEvent>();
            }

            // filter existing events further by content provider ID in properties
            existingEvents = existingEvents.Where(
                                    evt => evt.Properties != null && evt.Properties.Exists(
                                            prop => prop.Name == ApplicationConstants.IncentiveEventAdditionalPropertyKeys.ContentProviderId && prop.Value == streamedContent.ContentProviderId.ToString()
                                    )).ToList();
            if (existingEvents.Count == 0)
            {
                // no event exists already, so create a new one
                UserStreamContentIncentiveIntegrationEvent integrationEvent = new UserStreamContentIncentiveIntegrationEvent()
                {
                    UserId = callerUserId,
                    UserPhone = callerPhoneNumber,
                    OriginalTime = contentStreamedUserEventRequest.OriginalTime,
                    Content = streamedContent,
                };

                await _eventBus.Publish(integrationEvent);
                return Ok();
            }
            else
            {
                // event already exists so reject
                string errorString = string.Format(_stringLocalizer["INC_ERR_0032"], EventType.CONSUMER_INCOME_STREAMED_CONTENT_ONCE_PER_CONTENTPROVIDER, callerUserId);
                _logger.LogInformation(errorString);
                return BadRequest(new string[] { errorString });
            }
        }

        /// <summary>
        /// API to record Onboarding Rating Submitted event from consumer app
        /// </summary>
        /// <param name="onboardingRatingSubmittedUserEventRequest"></param>
        /// <returns></returns>
        [HttpPost("recordOnboardingRatingSubmitted")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult> RecordOnboardingRatingSubmittedEvent(UserEventRequest onboardingRatingSubmittedUserEventRequest)
        {
            Guid callerUserId = UserClaimData.GetUserId(User.Claims);
            string callerPhoneNumber = this.User.Identity.Name;
            DateTime now = DateTime.UtcNow;

            // validation - date can't be from future
            if (onboardingRatingSubmittedUserEventRequest.OriginalTime > now)
            {
                // this event is from future, so reject
                string errorString = string.Format(_stringLocalizer["INC_ERR_0031"], EventType.CONSUMER_INCOME_ONBOARDING_RATING_SUBMITTED, callerUserId);
                _logger.LogInformation(errorString);
                return BadRequest(new string[] { errorString });
            }

            // Validate event existence
            var existingEvents = await _eventRepository.GetEvents(new EventCriteriaRequest()
            {
                AudienceType = AudienceType.CONSUMER,
                EventCreatedFor = callerPhoneNumber,
                EventTypes = new List<EventType>() { EventType.CONSUMER_INCOME_ONBOARDING_RATING_SUBMITTED },
            });

            if (existingEvents == null || existingEvents.Count == 0) // no event exist
            {
                // create event from request
                var integrationEvent = new UserOnbrdngRtngSbmttdIncentiveIntegrationEvent()
                {
                    UserId = callerUserId,
                    UserPhone = callerPhoneNumber,
                    OriginalTime = onboardingRatingSubmittedUserEventRequest.OriginalTime,
                };

                await _eventBus.Publish(integrationEvent);
                return Ok();
            }
            else
            {
                // event already exists so reject
                string errorString = string.Format(_stringLocalizer["INC_ERR_0032"], EventType.CONSUMER_INCOME_ONBOARDING_RATING_SUBMITTED, callerUserId);
                _logger.LogInformation(errorString);
                return BadRequest(new string[] { errorString });
            }
        }

        #endregion

        #region private methods

        #endregion
    }
}
