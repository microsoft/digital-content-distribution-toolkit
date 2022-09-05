using blendnet.api.proxy.Cms;
using blendnet.api.proxy.Device;
using blendnet.api.proxy.Retailer;
using blendnet.common.dto;
using blendnet.common.dto.Events;
using blendnet.common.dto.Incentive;
using blendnet.common.dto.Retailer;
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
        private readonly DeviceProxy _deviceProxy;
        private readonly RetailerProxy _retailerProxy;
        private readonly IEventBus _eventBus;
        private readonly IStringLocalizer<SharedResource> _stringLocalizer;

        public UserIncentiveEventsController(IEventRepository eventRepository,
                                                ILogger<UserIncentiveEventsController> logger,
                                                ContentProxy contentProxy,
                                                DeviceProxy deviceProxy,
                                                RetailerProxy retailerProxy,
                                                IEventBus eventBus,
                                                IStringLocalizer<SharedResource> stringLocalizer)
        {
            _eventRepository = eventRepository;
            _logger = logger;
            _contentProxy = contentProxy;
            _deviceProxy = deviceProxy;
            _retailerProxy = retailerProxy;
            _eventBus = eventBus;
            _stringLocalizer = stringLocalizer;
        }

        #region Incentive management methods

        /// <summary>
        /// API to record Sign-in event from consumer app
        /// </summary>
        /// <param name="signinUserEventRequest"></param>
        /// <returns></returns>
        [HttpPost("recordSignin", Name = nameof(RecordSigninEvent))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult> RecordSigninEvent(UserEventRequest signinUserEventRequest)
        {
            Guid callerUserId = UserClaimData.GetUserId(User.Claims);
            string callerPhoneNumber = this.User.Identity.Name;
            DateTime now = DateTime.UtcNow;

            // validation - date can't be from future
            if (!signinUserEventRequest.OriginalTime.IsCurrentOrPast())
            {
                // this event is from future, so reject
                string errorString = string.Format(_stringLocalizer["INC_ERR_0031"], EventType.CONSUMER_INCOME_FIRST_SIGNIN, callerUserId);
                _logger.LogInformation($"errorString  event time : {signinUserEventRequest.OriginalTime}");
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
                // Create event in repository
                var incentiveEvent = CreateUserIncentiveEvent(now, callerPhoneNumber, callerUserId, EventType.CONSUMER_INCOME_FIRST_SIGNIN, signinUserEventRequest.OriginalTime);
                await _eventRepository.CreateIncentiveEvent(incentiveEvent);

                // create integration event
                var integrationEvent = new UserSigninIncentiveIntegrationEvent()
                {
                    IncentiveEvent = incentiveEvent,
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
        [HttpPost("recordAppLaunch", Name = nameof(RecordAppLaunchEvent))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult> RecordAppLaunchEvent(UserEventRequest appLaunchUserEventRequest)
        {
            Guid callerUserId = UserClaimData.GetUserId(this.User.Claims);
            string callerPhoneNumber = this.User.Identity.Name;
            DateTime now = DateTime.UtcNow;

            // validation - date can't be from future
            if (!appLaunchUserEventRequest.OriginalTime.IsCurrentOrPast())
            {
                // this event is from future, so reject
                string errorString = string.Format(_stringLocalizer["INC_ERR_0031"], EventType.CONSUMER_INCOME_APP_ONCE_OPEN, callerUserId);
                _logger.LogInformation($"errorString  event time : {appLaunchUserEventRequest.OriginalTime}");
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
                // Create event in repository
                var incentiveEvent = CreateUserIncentiveEvent(now, callerPhoneNumber, callerUserId, EventType.CONSUMER_INCOME_APP_ONCE_OPEN, appLaunchUserEventRequest.OriginalTime);
                await _eventRepository.CreateIncentiveEvent(incentiveEvent);

                // create integration event
                UserAppOpenIncentiveIntegrationEvent integrationEvent = new UserAppOpenIncentiveIntegrationEvent()
                {
                    IncentiveEvent = incentiveEvent,
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
        [HttpPost("recordContentStreamed", Name = nameof(RecordContentStreamedEvent))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult> RecordContentStreamedEvent(UserEventRequest contentStreamedUserEventRequest)
        {
            Guid callerUserId = UserClaimData.GetUserId(this.User.Claims);
            string callerPhoneNumber = this.User.Identity.Name;
            DateTime now = DateTime.UtcNow;

            // validation - date can't be from future
            if (!contentStreamedUserEventRequest.OriginalTime.IsCurrentOrPast())
            {
                // this event is from future, so reject
                string errorString = string.Format(_stringLocalizer["INC_ERR_0031"], EventType.CONSUMER_INCOME_STREAMED_CONTENT_ONCE_PER_CONTENTPROVIDER, callerUserId);
                _logger.LogInformation($"errorString  event time : {contentStreamedUserEventRequest.OriginalTime}");
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
                // Create event in repository
                var incentiveEvent = CreateUserIncentiveEvent(now, callerPhoneNumber, callerUserId, EventType.CONSUMER_INCOME_STREAMED_CONTENT_ONCE_PER_CONTENTPROVIDER, contentStreamedUserEventRequest.OriginalTime);
                incentiveEvent.Properties.AddRange(new Property[]
                {
                    new Property()
                    {
                        Name = ApplicationConstants.IncentiveEventAdditionalPropertyKeys.ContentId,
                        Value = streamedContent.ContentId.ToString(),
                    },
                    new Property()
                    {
                        Name = ApplicationConstants.IncentiveEventAdditionalPropertyKeys.ContentProviderId,
                        Value = streamedContent.ContentProviderId.ToString(),
                    },
                });

                await _eventRepository.CreateIncentiveEvent(incentiveEvent);

                // create integration event
                UserStreamContentIncentiveIntegrationEvent integrationEvent = new UserStreamContentIncentiveIntegrationEvent()
                {
                    IncentiveEvent = incentiveEvent,
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
        [HttpPost("recordOnboardingRatingSubmitted", Name = nameof(RecordOnboardingRatingSubmittedEvent))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult> RecordOnboardingRatingSubmittedEvent(UserEventRequest onboardingRatingSubmittedUserEventRequest)
        {
            Guid callerUserId = UserClaimData.GetUserId(User.Claims);
            string callerPhoneNumber = this.User.Identity.Name;
            DateTime now = DateTime.UtcNow;

            // validation - date can't be from future
            if (!onboardingRatingSubmittedUserEventRequest.OriginalTime.IsCurrentOrPast())
            {
                // this event is from future, so reject
                string errorString = string.Format(_stringLocalizer["INC_ERR_0031"], EventType.CONSUMER_INCOME_ONBOARDING_RATING_SUBMITTED, callerUserId);
                _logger.LogInformation($"errorString  event time : {onboardingRatingSubmittedUserEventRequest.OriginalTime}");
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
                // Create event in repository
                var incentiveEvent = CreateUserIncentiveEvent(now, callerPhoneNumber, callerUserId, EventType.CONSUMER_INCOME_ONBOARDING_RATING_SUBMITTED, onboardingRatingSubmittedUserEventRequest.OriginalTime);
                await _eventRepository.CreateIncentiveEvent(incentiveEvent);

                // create integration event
                var integrationEvent = new UserOnbrdngRtngSbmttdIncentiveIntegrationEvent()
                {
                    IncentiveEvent = incentiveEvent,
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

        /// <summary>
        /// API to record Media Download Completed event from consumer app
        /// </summary>
        /// <param name="downloadMediaCompletedEventRequest"></param>
        /// <returns></returns>
        [HttpPost("recordDownloadMediaCompleted", Name = nameof(RecordDownloadMediaCompletedEvent))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult> RecordDownloadMediaCompletedEvent(UserEventRequest downloadMediaCompletedEventRequest)
        {
            Guid callerUserId = UserClaimData.GetUserId(User.Claims);
            string callerPhoneNumber = this.User.Identity.Name;
            DateTime now = DateTime.UtcNow;

            // validation - date can't be from future
            if (!downloadMediaCompletedEventRequest.OriginalTime.IsCurrentOrPast())
            {
                // this event is from future, so reject
                string errorString = string.Format(_stringLocalizer["INC_ERR_0031"], EventType.CONSUMER_INCOME_DOWNLOAD_MEDIA_COMPLETED, callerUserId);
                _logger.LogInformation($"errorString  event time : {downloadMediaCompletedEventRequest.OriginalTime}");
                return BadRequest(new string[] { errorString });
            }

            // validation - content info - contentId
            if (!downloadMediaCompletedEventRequest.ContentId.HasValue)
            {
                string errorString = string.Format(_stringLocalizer["INC_ERR_0033"], EventType.CONSUMER_INCOME_DOWNLOAD_MEDIA_COMPLETED, callerUserId);
                _logger.LogInformation(errorString);
                return BadRequest(new string[] { errorString });
            }

            // validation - content info - content
            var downloadedContent = await _contentProxy.GetContentById(downloadMediaCompletedEventRequest.ContentId.Value);
            if (downloadedContent == null)
            {
                string errorString = string.Format(_stringLocalizer["INC_ERR_0034"], EventType.CONSUMER_INCOME_DOWNLOAD_MEDIA_COMPLETED, downloadMediaCompletedEventRequest.ContentId.Value, callerUserId);
                _logger.LogInformation(errorString);
                return BadRequest(new string[] { errorString });
            }

            // validate - no existing event for same content
            DateTime startOfEventDay = downloadMediaCompletedEventRequest.OriginalTime
                                .ToIndiaStandardTime()  // convert to IST
                                .Date // take date componet. This gives StartOfDay in IST
                                .UtcFromIndiaStandardTime(); // Convert back to UTC
            DateTime endOfEventDay = startOfEventDay.AddDays(1).AddTicks(-1);
            var existingEvents = await _eventRepository.GetEvents(new EventCriteriaRequest()
            {
                AudienceType = AudienceType.CONSUMER,
                EventCreatedFor = callerPhoneNumber,
                EventTypes = new List<EventType>() { EventType.CONSUMER_INCOME_DOWNLOAD_MEDIA_COMPLETED },
                StartDate = startOfEventDay,
                EndDate = endOfEventDay,
            });

            if (existingEvents is null)
            {
                // force empty list
                existingEvents = new();
            }

            // filter existing events further by content ID in properties
            existingEvents = existingEvents.Where(
                                    evt => evt.Properties != null && evt.Properties.Exists(
                                            prop => prop.Name == ApplicationConstants.IncentiveEventAdditionalPropertyKeys.ContentId && string.Equals(prop.Value, downloadMediaCompletedEventRequest.ContentId.ToString())
                                    )).ToList();

            if (existingEvents.Any())
            {
                // event already exists so reject
                string errorString = string.Format(_stringLocalizer["INC_ERR_0032"], EventType.CONSUMER_INCOME_DOWNLOAD_MEDIA_COMPLETED, callerUserId);
                _logger.LogInformation(errorString);
                return BadRequest(new string[] { errorString });
            }

            // validate - device details
            var deviceId = downloadMediaCompletedEventRequest.DeviceId;
            if (string.IsNullOrEmpty(deviceId))
            {
                string errorString = _stringLocalizer["INC_ERR_0047"];
                _logger.LogInformation("Device ID is not present");
                return BadRequest(new string[] { errorString });
            }

            var device = await _deviceProxy.GetDevice(deviceId);
            if (device is null)
            {
                _logger.LogInformation($"Unknown Device ID - {deviceId}");
                return BadRequest(new string[] {
                    _stringLocalizer["INC_ERR_0047"],
                });
            }

            if (device.DeviceStatus != common.dto.Device.DeviceStatus.Provisioned)
            {
                _logger.LogInformation($"Device is not in Provisioned state. Device ID - {deviceId}  Status - {device.DeviceStatus}");
                return BadRequest(new string[] {
                    _stringLocalizer["INC_ERR_0047"],
                });
            }

            // validate - retailer who has the device
            var retailerForDevice = await _retailerProxy.GetCurrentRetailerByDeviceId(deviceId);
            if (retailerForDevice is null)
            {
                _logger.LogInformation($"No active retailer found for device ID ${deviceId}");
                return BadRequest(new string[] {
                    _stringLocalizer["INC_ERR_0047"],
                });
            }

            // now we have all details valid; create the events
            IncentiveEvent userIncentiveEvent = CreateUserIncentiveEvent(now, callerPhoneNumber, callerUserId, EventType.CONSUMER_INCOME_DOWNLOAD_MEDIA_COMPLETED, downloadMediaCompletedEventRequest.OriginalTime);
            IncentiveEvent retailerIncentiveEvent = CreateRetailerIncentiveEvent(now, callerUserId, downloadMediaCompletedEventRequest.OriginalTime, EventType.RETAILER_INCOME_DOWNLOAD_MEDIA_COMPLETED, retailerForDevice);

            // additional props
            var additionalProps = new Property[]
            {
                new()
                {
                    Name = ApplicationConstants.IncentiveEventAdditionalPropertyKeys.ContentId,
                    Value = downloadedContent.ContentId.Value.ToString(),
                },
                new()
                {
                    Name = ApplicationConstants.IncentiveEventAdditionalPropertyKeys.DeviceId,
                    Value = deviceId,
                },
            };

            userIncentiveEvent.Properties.AddRange(additionalProps);
            retailerIncentiveEvent.Properties.AddRange(additionalProps);


            await _eventRepository.CreateIncentiveEvent(userIncentiveEvent);
            await _eventRepository.CreateIncentiveEvent(retailerIncentiveEvent);

            // post on event bus
            // create integration event
            var integrationEvent = new UserMediaDnldCmpltdIncentiveIntegrationEvent()
            {
                IncentiveEvent = userIncentiveEvent,
                RetailerIncentiveEvent = retailerIncentiveEvent,
            };

            await _eventBus.Publish(integrationEvent);

            return Ok();
        }

        #endregion

        #region private methods

        /// <summary>
        /// Creates basic incentive event for User Events
        /// </summary>
        /// <param name="userPhoneNumber">Phone number of the user</param>
        /// <param name="userId">User Id of the user</param>
        /// <param name="eventType">Event Type</param>
        /// <param name="eventOriginalTime">Event's original time</param>
        /// <returns></returns>
        private IncentiveEvent CreateUserIncentiveEvent(DateTime now, string userPhoneNumber, Guid userId, EventType eventType, DateTime eventOriginalTime)
        {
            var incentiveEvent = new IncentiveEvent()
            {
                EventCreatedFor = userPhoneNumber,
                EventType = eventType,
                Audience = new Audience()
                {
                    AudienceType = AudienceType.CONSUMER,
                    SubTypeName = ApplicationConstants.Common.CONSUMER,
                },
                CreatedByUserId = userId,
                CreatedDate = now,
                EventCategoryType = EventCategoryType.INCOME,
                EventOccuranceTime = eventOriginalTime,
                EventSubType = null,
                EventId = Guid.NewGuid(),
                CalculatedValue = 0, // will be calculated later
                OriginalValue = 0,
                ModifiedByByUserId = null,
                ModifiedDate = null,
                Properties = new List<Property>()
                {
                    new Property()
                    {
                        Name = ApplicationConstants.IncentiveEventAdditionalPropertyKeys.UserId,
                        Value = userId.ToString(),
                    },
                },
            };

            return incentiveEvent;
        }

        /// <summary>
        /// Creates basic Retailer incentive event for User Events
        /// </summary>
        private static IncentiveEvent CreateRetailerIncentiveEvent(DateTime now, Guid userId, DateTime eventOriginalTime, EventType eventType, RetailerDto retailer)
        {
            var incentiveEvent = new IncentiveEvent()
            {
                Audience = new Audience()
                {
                    AudienceType = AudienceType.RETAILER,
                    SubTypeName = retailer.PartnerCode,
                },
                CalculatedValue = 0, // will be calculated later
                CreatedByUserId = userId,
                CreatedDate = now,
                EventCategoryType = EventCategoryType.INCOME,
                EventCreatedFor = retailer.RetailerId,
                EventId = Guid.NewGuid(),
                EventOccuranceTime = eventOriginalTime,
                EventSubType = null, // TODO: Check
                EventType = eventType,
                ModifiedByByUserId = null,
                ModifiedDate = null,
                OriginalValue = 0,
                IsUserDeleted = false,
                Entity1CalculatedValue = null,
                Entity2CalculatedValue = null,
                Entity3CalculatedValue = null,
                Entity4CalculatedValue = null,
                Properties = new()
                {
                    new Property()
                    {
                        Name = ApplicationConstants.IncentiveEventAdditionalPropertyKeys.UserId,
                        Value = userId.ToString(),
                    },
                },
            };

            return incentiveEvent;
        }

        #endregion
    }
}
