using AutoMapper;
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
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;

namespace blendnet.user.api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = KaizalaIdentityAuthOptions.DefaultScheme)]
    public class UserBasicController : ControllerBase
    {
        private readonly ILogger _logger;

        private readonly IUserRepository _userRepository;

        private readonly RetailerProxy _retailerProxy;

        private readonly IEventBus _eventBus;

        private readonly IStringLocalizer<SharedResource> _stringLocalizer;

        private readonly IMapper _mapper;

        private readonly IDistributedCache _cache;

        private readonly TelemetryClient _telemetryClient;

        public UserBasicController( IUserRepository userRepository,
                                    ILogger<UserController> logger,
                                    RetailerProxy retailerProxy,
                                    IEventBus eventBus,
                                    IStringLocalizer<SharedResource> stringLocalizer,
                                    IMapper mapper,
                                    IDistributedCache cache,
                                    TelemetryClient telemetryClient)
        {
            _logger = logger;
            _userRepository = userRepository;
            _retailerProxy = retailerProxy;
            _eventBus = eventBus;
            _stringLocalizer = stringLocalizer;
            _mapper = mapper;
            _cache = cache;
            _telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Get current user details
        /// </summary>
        /// <param name="User"></param>
        /// <returns>User Object</returns>
        [HttpGet("me", Name = nameof(GetUser))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<UserResponse>> GetUser()
        {
            var callerPhoneNumber = this.User.Identity.Name;

            var user = await _userRepository.GetUserByPhoneNumber(callerPhoneNumber);

            if (user is null)
            {
                return NotFound();
            }

            var mappedUser = _mapper.Map<UserResponse>(user);

            return Ok(mappedUser);
        }

        /// <summary>
        /// Update User Profile
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        [HttpPut("profile", Name = nameof(UpdateProfile))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> UpdateProfile(UpdateProfileRequest request)
        {
            List<string> errorInfo = new List<string>();
            User user = await _userRepository.GetUserByPhoneNumber(this.User.Identity.Name);
            if(user == null){
                return NotFound();
            }
            user.Name = request.Name;
            int response = await _userRepository.UpdateUser(user);
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
        /// Assign Retailer(Referral) data  to the Customer
        /// </summary>
        /// <param name="referralDto"></param>
        /// <returns>/returns>
        [HttpPost("assignretailer/{referralCode}", Name = nameof(AssignRetailer))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult> AssignRetailer(string referralCode)
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
                RetailerUserId = retailerDto.UserId,
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

                // publish AI event
                AssignRetailerAIEvent assignRetailerAIEvent = new AssignRetailerAIEvent()
                {
                    PartnerCode = retailerDto.PartnerCode,
                    PartnerProvidedId = retailerDto.PartnerProvidedId,
                    RetailerPartnerId = retailerDto.PartnerId,
                    UserId = user.UserId,
                    IdentityId = user.IdentityId,
                    RetailerAdditionalAttributes = retailerDto.AdditionalAttibutes,
                };

                _telemetryClient.TrackEvent(assignRetailerAIEvent);

                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// API to create a new data export request for user
        /// </summary>
        /// <returns></returns>
        [HttpPost("dataexport/create", Name = nameof(CreateDataExportCommand))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<int>> CreateDataExportCommand()
        {
            List<string> errorInfo = new List<string>();

            string phoneNumber = this.User.Identity.Name;
            
            var userId = UserClaimData.GetUserId(this.User.Claims);

            var existingUser = await _userRepository.GetUserByPhoneNumber(phoneNumber);

            if (existingUser is null)
            {
                return NotFound();
            }

            //if the request is in progress do not accept another request
            if (existingUser.DataExportRequestStatus != DataExportRequestStatus.NotInitialized &&
                existingUser.DataExportRequestStatus != DataExportRequestStatus.ExportedDataNotified )
            {
                errorInfo.Add(_stringLocalizer["USR_ERR_021"]);

                return BadRequest(errorInfo);
            }

            var now = DateTime.UtcNow;

            //create new command
            var newDataExportCommand = new UserCommand(UserCommandType.Export)
            {
                CreatedByUserId = userId,
                CreatedDate = now,
                Id = Guid.NewGuid(),
                PhoneNumber = phoneNumber,
                DataExportRequestStatus = DataExportRequestStatus.Submitted,
                UserId = userId
            };

            CommandExecutionDetails executionDetails = new CommandExecutionDetails()
            {
                EventName = DataExportRequestStatus.Submitted.ToString(),
                EventDateTime = now
            };

            newDataExportCommand.ExecutionDetails.Add(executionDetails);

            existingUser.DataExportRequestStatus = DataExportRequestStatus.Submitted;
            existingUser.DataExportStatusUpdatedBy = newDataExportCommand.Id;
            existingUser.ModifiedByByUserId = userId;
            existingUser.ModifiedDate = now;

            var response = await _userRepository.CreateCommandBatch(newDataExportCommand, existingUser);

            var aiEvent = new CreateUserDataExportCommandAIEvent()
            {
                RequestId = newDataExportCommand.Id,
                UserId = userId,
            };

            _telemetryClient.TrackEvent(aiEvent);

            return Ok(response);
        }

        /// <summary>
        /// API to request deletion of user's account
        /// </summary>
        /// <returns></returns>
        [HttpDelete("user", Name = nameof(DeleteUserAccount))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<ActionResult> DeleteUserAccount()
        {
            List<string> errorInfo = new List<string>();

            string phoneNumber = this.User.Identity.Name;

            Guid userId = UserClaimData.GetUserId(this.User.Claims);

            User existingUser = await _userRepository.GetUserByPhoneNumber(phoneNumber);

            if (existingUser is null)
            {
                return NotFound();
            }

            //if the request is in progress do not accept another request
            if (existingUser.AccountStatus != UserAccountStatus.Active)
            {
                errorInfo.Add(_stringLocalizer["USR_ERR_021"]);

                return BadRequest(errorInfo);
            }

            //
            var now = DateTime.UtcNow;

            //create new command
            var newDataUpdateCommand = new UserCommand(UserCommandType.Update)
            {
                Id = Guid.NewGuid(),
                CreatedByUserId = userId,
                CreatedDate = now,
                PhoneNumber = phoneNumber,
                DataUpdateRequestStatus = DataUpdateRequestStatus.Submitted,
                UserId = userId
            };

            CommandExecutionDetails executionDetails = new CommandExecutionDetails()
            {
                EventName = DataUpdateRequestStatus.Submitted.ToString(),
                EventDateTime = now
            };

            newDataUpdateCommand.ExecutionDetails.Add(executionDetails);

            // mark user for deletion
            existingUser.AccountStatus = UserAccountStatus.InActive;
            existingUser.DataUpdateRequestStatus = DataUpdateRequestStatus.Submitted;
            existingUser.DataUpdateStatusUpdatedBy = newDataUpdateCommand.Id;
            existingUser.ModifiedByByUserId = userId;
            existingUser.ModifiedDate = now;

            //create command and update user in batch
            await _userRepository.CreateCommandBatch(newDataUpdateCommand, existingUser);

            //remove the user from cache. so that new state with new status gets loaded during authentication process
            string userByPhoneNoCacheKey = $"{phoneNumber}{ApplicationConstants.DistributedCacheKeySuffix.USERBYPHONEKEY}";

            await _cache.RemoveAsync(userByPhoneNoCacheKey);

            // record telemetry
            var aiEvent = new DeleteUserDataAIEvent()
            {
                UserId = userId,
                RequestId = newDataUpdateCommand.Id
            };

            _telemetryClient.TrackEvent(aiEvent);

            return NoContent();
        }

        #region private methods

        #endregion
    }
}
