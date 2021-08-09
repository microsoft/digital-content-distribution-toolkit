using blendnet.api.proxy.Retailer;
using blendnet.common.dto;
using blendnet.common.dto.Events;
using blendnet.common.dto.Retailer;
using blendnet.common.dto.User;
using blendnet.common.infrastructure;
using blendnet.common.infrastructure.Authentication;
using blendnet.common.infrastructure.Extensions;
using blendnet.user.api.Models;
using blendnet.user.repository.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace blendnet.user.api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = KaizalaIdentityAuthOptions.BasicIdentityScheme)]
    public class UserBasicController : ControllerBase
    {
        private readonly ILogger _logger;

        private IUserRepository _userRepository;

        private UserAppSettings _appSettings;

        IStringLocalizer<SharedResource> _stringLocalizer;

        private TelemetryClient _telemetryClient;

        private RetailerProxy _retailerProxy;

        private readonly IEventBus _eventBus;

        public UserBasicController(IUserRepository userRepository,
                              ILogger<UserController> logger,
                              IOptionsMonitor<UserAppSettings> optionsMonitor,
                              IStringLocalizer<SharedResource> stringLocalizer,
                              IEventBus eventBus,
                              RetailerProxy retailerProxy,
                              TelemetryClient telemetryClient)
        {
            _logger = logger;
            _userRepository = userRepository;
            _appSettings = optionsMonitor.CurrentValue;
            _stringLocalizer = stringLocalizer;
            _telemetryClient = telemetryClient;
            _eventBus = eventBus;
            _retailerProxy = retailerProxy;
        }

        /// <summary>
        /// Create BlendNet User
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status</returns>
        [HttpPost("user", Name = nameof(CreateUserNew))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult> CreateUserNew(CreateUserRequest request)
        {
            Guid identityId = GetIdentityUserId(User.Claims);
            String phoneNumber = this.User.Identity.Name;

            User user = await CreateUserIfNotExistsInternal(request);

            return Ok(user.UserId);
        }

        /// <summary>
        /// API to link User identity to unlinked retailer
        /// </summary>
        /// <param name="linkRetailerRequest"></param>
        /// <returns></returns>
        [HttpPost("linkRetailer", Name = nameof(LinkRetailer))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult> LinkRetailer(LinkRetailerRequest linkRetailerRequest)
        {
            Guid identityId = GetIdentityUserId(User.Claims);
            String phoneNumber = this.User.Identity.Name;

            User user = await CreateUserIfNotExistsInternal(new CreateUserRequest() {
                ChannelId = Channel.CMSPortal,
                UserName = "",
            });

            RetailerDto retailerToLink = await _retailerProxy.GetRetailerById(linkRetailerRequest.PartnerProvidedId, linkRetailerRequest.PartnerCode);

            if (retailerToLink is null)
            {
                return BadRequest(new string[] {
                    String.Format(_stringLocalizer["USR_ERR_014"], linkRetailerRequest.PartnerProvidedId, linkRetailerRequest.PartnerCode),
                });
            }

            if (retailerToLink.UserId != Guid.Empty) // retailer is alredy linked
            {
                if (retailerToLink.UserId == user.UserId)
                {
                    // trying to link to same user, so should be OK, and nothing more to do here
                    return Ok();
                }
                else
                {
                    // trying to link when retailer is linked to another user
                    return BadRequest(new string[] {
                        String.Format(_stringLocalizer["USR_ERR_015"], linkRetailerRequest.PartnerProvidedId, linkRetailerRequest.PartnerCode),
                    });
                }
            } 

            LinkRetailerIntegrationEvent linkRetailerIntegrationEvent = new LinkRetailerIntegrationEvent()
            {
                PartnerProvidedId = linkRetailerRequest.PartnerProvidedId,
                PartnerCode = linkRetailerRequest.PartnerCode,
                User = user,
            };

            await _eventBus.Publish(linkRetailerIntegrationEvent);

            // AI event
            LinkRetailerAIEvent linkRetailerAIEvent = new LinkRetailerAIEvent()
            {
                AdditionalAttributes = retailerToLink.AdditionalAttibutes,
                City = retailerToLink.Address.City,
                Latitude = retailerToLink.Address.MapLocation.Latitude,
                Longitude = retailerToLink.Address.MapLocation.Longitude,
                Name = retailerToLink.Name,
                PartnerCode = linkRetailerRequest.PartnerCode,
                PartnerProvidedId = linkRetailerRequest.PartnerProvidedId,
                PinCode = retailerToLink.Address.PinCode,
                RetailerPartnerId = retailerToLink.PartnerId,
                State = retailerToLink.Address.State,
            };

            _telemetryClient.TrackEvent(linkRetailerAIEvent);

            return Ok();
        }

        #region Private methods

        /// <summary>
        /// Returns user's Identity id guid from claims list.
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        private static Guid GetIdentityUserId(IEnumerable<Claim> claims)
        {
            Guid identityId = claims
                                .Where(x => x.Type.Equals(ApplicationConstants.KaizalaIdentityClaims.IdentityUId))
                                .Select(x => new Guid(x.Value))
                                .First();

            return identityId;
        }

        /// <summary>
        /// (internal) helper method for creating user
        /// Also posts AI event
        /// </summary>
        /// <param name="createUserRequest"></param>
        /// <returns></returns>
        private async Task<User> CreateUserIfNotExistsInternal(CreateUserRequest createUserRequest)
        {
            String phoneNumber = this.User.Identity.Name;
            Guid identityId = GetIdentityUserId(User.Claims);

            User existingUser = await _userRepository.GetUserByPhoneNumber(phoneNumber);
            if (existingUser is not null)
            {
                // user already exists, return it
                return existingUser;
            }

            var generatedId = Guid.NewGuid();
            User user = new User
            {
                UserId = generatedId,
                PhoneNumber = phoneNumber,
                Name = createUserRequest.UserName,
                ChannelId = createUserRequest.ChannelId,
                CreatedDate = DateTime.UtcNow,
                CreatedByUserId = generatedId,
                Type = UserContainerType.User,
                IdentityId = identityId,
            };

            await _userRepository.CreateUser(user);

            // Track the user created event to Application Insights
            CreateUserAIEvent createUserAIEvent = new CreateUserAIEvent()
            {
                UserId = generatedId,
                IdentityId = identityId,
                ChannelId = createUserRequest.ChannelId,
            };

            _telemetryClient.TrackEvent(createUserAIEvent);

            return user;
        }

        #endregion
    }
}
