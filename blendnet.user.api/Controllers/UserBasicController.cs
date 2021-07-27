using blendnet.common.dto;
using blendnet.common.dto.User;
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

        public UserBasicController(IUserRepository userRepository,
                              ILogger<UserController> logger,
                              IOptionsMonitor<UserAppSettings> optionsMonitor,
                              IStringLocalizer<SharedResource> stringLocalizer,
                              TelemetryClient telemetryClient)
        {
            _logger = logger;
            _userRepository = userRepository;
            _appSettings = optionsMonitor.CurrentValue;
            _stringLocalizer = stringLocalizer;
            _telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Create BlendNet User
        /// </summary>
        /// <param name="User"></param>
        /// <returns>Status</returns>
        [HttpPost("user", Name = nameof(CreateUserNew))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult> CreateUserNew(CreateUserRequest request)
        {
            Guid identityId = GetIdentityUserId(User.Claims);
            String phoneNumber = this.User.Identity.Name;

            User existingUser = await _userRepository.GetUserByPhoneNumber(phoneNumber);
            if (existingUser != null)
            {
                return Ok(existingUser.UserId);
            }

            var generatedId = Guid.NewGuid();
            User user = new User
            {
                UserId = generatedId,
                PhoneNumber = phoneNumber,
                Name = request.UserName,
                ChannelId = request.ChannelId,
                CreatedDate = DateTime.UtcNow,
                CreatedByUserId = generatedId,
                Type = UserContainerType.User,
                IdentityId = identityId,
            };

            await _userRepository.CreateUser(user);

            //Track the user created event to Application Insights
            CreateUserAIEvent createUserAIEvent = new CreateUserAIEvent()
            {
                UserId = generatedId,
                IdentityId = identityId,
                ChannelId = request.ChannelId,
            };

            _telemetryClient.TrackEvent(createUserAIEvent);

            return Ok(user.UserId);
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

        #endregion
    }
}
