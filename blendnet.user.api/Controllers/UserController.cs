using blendnet.api.proxy.Kaizala;
using blendnet.api.proxy.Retailer;
using blendnet.common.dto;
using blendnet.common.dto.Events;
using blendnet.common.dto.Notification;
using blendnet.common.dto.Retailer;
using blendnet.common.dto.User;
using blendnet.common.infrastructure;
using blendnet.common.infrastructure.Authentication;
using blendnet.common.infrastructure.Extensions;
using blendnet.common.infrastructure.Notification;
using blendnet.user.api.Models;
using blendnet.user.repository.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static blendnet.common.dto.ApplicationConstants;

namespace blendnet.user.api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ILogger _logger;

        private IUserRepository _userRepository;

        private RetailerProxy _retailerProxy;

        private RetailerProviderProxy _retailerProviderProxy;

        private NotificationProxy _notificationProxy;

        private IEventBus _eventBus;

        private UserAppSettings _appSettings;

        IStringLocalizer<SharedResource> _stringLocalizer;

        private TelemetryClient _telemetryClient;

        public UserController(IUserRepository userRepository,
                              ILogger<UserController> logger,
                              RetailerProxy retailerProxy,
                              RetailerProviderProxy retailerProviderProxy,
                              NotificationProxy notificationProxy,
                              IEventBus eventBus,
                              IOptionsMonitor<UserAppSettings> optionsMonitor,
                              IStringLocalizer<SharedResource> stringLocalizer,
                              TelemetryClient telemetryClient)
        {
            _logger = logger;
            _userRepository = userRepository;
            _retailerProxy = retailerProxy;
            _retailerProviderProxy = retailerProviderProxy;
            _notificationProxy = notificationProxy;
            _eventBus = eventBus;
            _appSettings = optionsMonitor.CurrentValue;
            _stringLocalizer = stringLocalizer;
            _telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Create BlendNet User
        /// </summary>
        /// <param name="User"></param>
        /// <returns>Status</returns>
        [HttpPost("user", Name = nameof(CreateUser))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult> CreateUser(CreateUserRequest request)
        {
            List<string> errorInfo = new List<string>();

            Guid userId = UserClaimData.GetUserId(User.Claims);
            String phoneNumber = this.User.Identity.Name;

            if (await _userRepository.GetUserByPhoneNumber(phoneNumber) != null)
            {
                errorInfo.Add(String.Format(_stringLocalizer["USR_ERR_001"], phoneNumber));
                return BadRequest(errorInfo);
            }

            User user = new User
            {
                    Id = userId, 
                    PhoneNumber = phoneNumber, 
                    UserName = request.UserName,
                    ChannelId = request.ChannelId,
                    CreatedDate = DateTime.UtcNow,
                    CreatedByUserId = userId,
                    Type = UserContainerType.User
            };

            await _userRepository.CreateUser(user);

            //Track the user created event to Application Insights
            CreateUserAIEvent createUserAIEvent = new CreateUserAIEvent()
            {
                UserId = userId,
                ChannelId = request.ChannelId,
            };

            _telemetryClient.TrackEvent(createUserAIEvent);

            return Ok(user.Id);
        }

        /// <summary>
        /// Creates a new retailer - to be called by partner
        /// </summary>
        /// <param name="retailerRequest">Request containg retailer details</param>
        /// <returns>Retailer ID of the created retailer</returns>
        [HttpPost("retailer")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.RetailerManagement)]
        public async Task<ActionResult<string>> CreateRetailer(CreateRetailerRequest retailerRequest)
        {
            return await this.CreateRetailerInternal(retailerRequest);
        }

        /// <summary>
        /// Get user using phone number
        /// </summary>
        /// <param name="User"></param>
        /// <returns>User Object</returns>
        [HttpGet("user/{phoneNumber}", Name = nameof(GetUserByPhoneNumber))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<User>> GetUserByPhoneNumber(string phoneNumber)
        {
            User user = await _userRepository.GetUserByPhoneNumber(phoneNumber);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(user);
            }
        }

        /// <summary>
        /// Send notification to all users
        /// </summary>
        /// <param name="Notification"></param>
        /// <returns></returns>
        [HttpPost("notification", Name = nameof(SendNotification))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult> SendNotification(SendNotificationRequest request)
        {
            List<string> errorInfo = new List<string>();
            List<UserData> userData = await _userRepository.GetUsersDataByChannelId(Channel.ConsumerApp);
            string payload = NotificationUtilities.GetNotificationPayload(request.Title, request.Body, request.AttachmentUrl, null, request.Type, _appSettings.KaizalaIdentityAppName);
            if((payload.Length * sizeof(Char)) > 4096)
            {
                errorInfo.Add(String.Format(_stringLocalizer["USR_ERR_014"]));
                return BadRequest(errorInfo);
            } 
            NotificationRequest notificationRequest = new NotificationRequest
            {
                Payload = payload,
                UserData = userData
            };
            try
            {
                await _notificationProxy.SendNotification(notificationRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                errorInfo.Add(String.Format(_stringLocalizer["USR_ERR_015"], ex));
                return BadRequest(errorInfo);
            }
        }

        /// <summary>
        /// Get current user details
        /// </summary>
        /// <param name="User"></param>
        /// <returns>User Object</returns>
        [HttpGet("me", Name = nameof(GetUser))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<User>> GetUser()
        {
            List<string> errorInfo = new List<string>();
            return await GetUserByPhoneNumber(this.User.Identity.Name);
        }

        /// <summary>
        /// Assign Retailer(Referral) data  to the Customer
        /// </summary>
        /// <param name="referralDto"></param>
        /// <returns>/returns>
        [HttpPost("assignretailer/{referralCode}", Name = nameof(AssignRetailer))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<User>> AssignRetailer(string referralCode)
        {
            List<string> errorInfo = new List<string>();

            String phoneNumber = this.User.Identity.Name;
            User user = await _userRepository.GetUserByPhoneNumber(phoneNumber);
            if (user == null)
            {
                errorInfo.Add(String.Format(_stringLocalizer["USR_ERR_002"], phoneNumber));
                return NotFound(errorInfo);
            }

            if (user.ChannelId != Channel.ConsumerApp)
            {
                errorInfo.Add(_stringLocalizer["USR_ERR_003"]);
                return BadRequest(errorInfo);
            }

            if (user.ReferralInfo != null)
            {
                errorInfo.Add(string.Format(_stringLocalizer["USR_ERR_004"],user.ReferralInfo.RetailerReferralCode));
                return BadRequest(errorInfo);
            }

            RetailerDto retailerDto = await _retailerProxy.GetRetailerByReferralCode(referralCode);
            if(retailerDto == null)
            {
                errorInfo.Add(_stringLocalizer["USR_ERR_005"]);
                return BadRequest(errorInfo);
            }

            var currentDate = DateTime.UtcNow;
            user.ReferralInfo = new ReferralDto
            {
                RetailerId = retailerDto.Id,
                RetailerPartnerCode = retailerDto.PartnerCode,
                RetailerPartnerId = retailerDto.PartnerId,
                RetailerReferralCode = retailerDto.ReferralCode,
                ReferralDate = Int32.Parse(currentDate.ToString(ApplicationConstants.DateTimeFormats.FormatYYYYMMDD)),
                ReferralDateTime = currentDate,
            };

            user.ModifiedByByUserId = UserClaimData.GetUserId(User.Claims);
            user.ModifiedDate = currentDate;

            int statusCode = await _userRepository.UpdateUser(user);
            if (statusCode == (int)System.Net.HttpStatusCode.OK)
            {
                RetailerAssignedIntegrationEvent retailerAssignedIntegrationEvent = new RetailerAssignedIntegrationEvent()
                {
                    User = user,
                };

                await _eventBus.Publish(retailerAssignedIntegrationEvent);

                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Get Referral Summary
        /// </summary>
        /// <param name="retailerPartnerId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet("summary/{retailerPartnerId}", Name = nameof(GetReferralSummary))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.RetailerManagement)]
        public async Task<ActionResult> GetReferralSummary(string retailerPartnerId, int startDate, int endDate)
        {
            List<string> errorDetails = new List<string>();

            if (startDate <= 0 || endDate <= 0)
            {
                errorDetails.Add(_stringLocalizer["USR_ERR_007"]);
                return BadRequest(errorDetails);
            }

            if (startDate > endDate)
            {
                errorDetails.Add(_stringLocalizer["USR_ERR_008"]);
                return BadRequest(errorDetails);
            }

            Guid callerUserId = UserClaimData.GetUserId(User.Claims);
            RetailerProviderDto retailerProvider = await _retailerProviderProxy.GetRetailerProviderByServiceAccountId(callerUserId);
            if (retailerProvider == null)
            {
                errorDetails.Add(string.Format(_stringLocalizer["USR_ERR_013"], callerUserId));
                return BadRequest(errorDetails);
            }
            
            string partnerId = RetailerDto.CreatePartnerId(retailerProvider.PartnerCode, retailerPartnerId);

            List<ReferralSummary> referralData = await _userRepository.GetReferralSummary(partnerId, startDate, endDate);
            if (referralData == null || referralData.Count == 0)
            {
                return NotFound();
            }

            return Ok(referralData);
        }

        #region private methods
        /// <summary>
        /// Creates the user, if not already exists
        /// </summary>
        /// <param name="user">user to be created</param>
        /// <returns>true if the user was created, false otherwise</returns>
        private async Task<bool> CreateUserIfNotExist(User user)
        {
            if (await _userRepository.GetUserByPhoneNumber(user.PhoneNumber) == null)
            {
                await _userRepository.CreateUser(user);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Logic for creating retailer from the request
        /// </summary>
        /// <param name="retailerRequest">request</param>
        /// <returns>Retailer ID of the created retailer</returns>
        private async Task<ActionResult<string>> CreateRetailerInternal(CreateRetailerRequest retailerRequest)
        {
            Guid callerUserId = UserClaimData.GetUserId(User.Claims);
            RetailerProviderDto retailerProvider = await _retailerProviderProxy.GetRetailerProviderByServiceAccountId(callerUserId);

            // validations
            {
                var listOfValidationErrors = new List<string>();

                string phoneNumber = retailerRequest.PhoneNumber;
                if (!common.dto.User.User.IsPhoneNumberValid(phoneNumber))
                {
                    listOfValidationErrors.Add(_stringLocalizer["USR_ERR_010"]);
                }

                if (!retailerRequest.Address.MapLocation.isValid())
                {
                    listOfValidationErrors.Add(_stringLocalizer["USR_ERR_011"]);
                }

                if (retailerProvider == null)
                {
                    listOfValidationErrors.Add(string.Format(_stringLocalizer["USR_ERR_13"], callerUserId));
                }

                var existingRetailer = await _retailerProxy.GetRetailerById(retailerRequest.RetailerId, retailerProvider.PartnerCode);
                if (existingRetailer != null)
                {
                    listOfValidationErrors.Add(String.Format(_stringLocalizer["USR_ERR_012"], retailerRequest.RetailerId));
                }

                // check and return validation errors
                if (listOfValidationErrors.Count > 0)
                {
                    return BadRequest(listOfValidationErrors);
                }
            }

            DateTime now = DateTime.UtcNow;

            // create user if not exists
            User user = new User
            {
                Id = retailerRequest.UserId, 
                PhoneNumber = retailerRequest.PhoneNumber, 
                UserName = retailerRequest.Name,
                ChannelId = Channel.NovoRetailerApp, // TODO: this should be from the Claim / partnerCode
                CreatedDate = now,
                CreatedByUserId = callerUserId,
                Type = UserContainerType.User
            };

            await this.CreateUserIfNotExist(user);

            // create RetailerDto from request
            RetailerDto retailer = new RetailerDto()
            {
                // Base propeties
                CreatedByUserId = callerUserId,
                CreatedDate = now,

                // Person Properties
                Id = retailerRequest.UserId,
                PhoneNumber = retailerRequest.PhoneNumber,
                UserName = retailerRequest.Name,

                // User properties
                // Retailer properties
                PartnerProvidedId = retailerRequest.RetailerId,
                PartnerCode = retailerProvider.PartnerCode,
                Address = retailerRequest.Address,
                Services = new List<ServiceType>() { ServiceType.Media },
                AdditionalAttibutes = retailerRequest.AdditionalAttributes,

                StartDate = now,
                EndDate = DateTime.MaxValue,
            };

            RetailerCreatedIntegrationEvent retailerCreatedIntegrationEvent = new RetailerCreatedIntegrationEvent()
            {
                Retailer = retailer,
            };

            await _eventBus.Publish(retailerCreatedIntegrationEvent);

            //Track the Retailer created event to Application Insights
            CreateRetailerAIEvent createRetailerAIEvent = new CreateRetailerAIEvent()
            {
                Name = retailer.UserName,
                PartnerCode = retailer.PartnerCode,
                PartnerProvidedId = retailer.PartnerProvidedId,
                RetailerPartnerId = retailer.PartnerId,
                AdditionalAttributes = retailer.AdditionalAttibutes,
            };

            _telemetryClient.TrackEvent(createRetailerAIEvent);

            return Ok(retailerRequest.RetailerId);
        }

        #endregion
    }
}