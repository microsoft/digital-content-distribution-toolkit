using blendnet.api.proxy.Retailer;
using blendnet.common.dto;
using blendnet.common.dto.User;
using blendnet.common.infrastructure.Authentication;
using blendnet.user.api.Models;
using blendnet.user.repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace blendnet.user.api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = KaizalaIdentityAuthOptions.DefaultScheme)]
    public class WhitelistedUserController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;
        private readonly UserAppSettings _appSettings;
        private readonly RetailerProxy _retailerProxy;
        IStringLocalizer<SharedResource> _stringLocalizer;

        public WhitelistedUserController(IUserRepository userRepository,
                              ILogger<WhitelistedUserController> logger,
                              IOptionsMonitor<UserAppSettings> optionsMonitor,
                              RetailerProxy retailerProxy,
                              IStringLocalizer<SharedResource> stringLocalizer
                              )
        {
            _logger = logger;
            _userRepository = userRepository;
            _appSettings = optionsMonitor.CurrentValue;
            _retailerProxy = retailerProxy;
            _stringLocalizer = stringLocalizer;
        }

        /// <summary>
        /// API to create a new whitelisted user
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        [HttpPost("create", Name = nameof(CreateWhitelistedUser))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin, ApplicationConstants.KaizalaIdentityRoles.Retailer)]
        public async Task<ActionResult<string>> CreateWhitelistedUser(CreateWhitelistedUserRequest request)
        {
            if (!common.dto.User.User.IsPhoneNumberValid(request.PhoneNumber))
            {
                return BadRequest(new string[]
                {
                    _stringLocalizer["USR_ERR_010"],
                });
            }

            var existingWhitelistedUser = await _userRepository.GetWhitelistedUser(request.PhoneNumber);
            if (existingWhitelistedUser is not null)
            {
                // already exists, return
                return Ok(request.PhoneNumber);
            }

            var callerUserId = UserClaimData.GetUserId(this.User.Claims);
            var callerRole = GetCurrentUserRoleForWhitelisting();

            if (string.IsNullOrEmpty(callerRole))
            {
                return BadRequest(new string[] {
                    _stringLocalizer["USR_ERR_017"],
                });
            }

            if (callerRole == ApplicationConstants.KaizalaIdentityRoles.Retailer)
            {
                // validate the retailer details as well
                var existingRetailer = await _retailerProxy.GetRetailerById(request.PartnerProvidedRetailerId, request.PartnerCode);
                if (existingRetailer is null  ||  existingRetailer.UserId != callerUserId) // retailer should exist and should be the self retailer
                {
                    return BadRequest(new string[] {
                        _stringLocalizer["USR_ERR_018"],
                    });
                }
            }

            // should not exceed global limit
            var totalCount = await _userRepository.WhitelistedUsersTotalCount();
            var globalLimit = _appSettings.WhitelistedUsersGlobalLimit;

            if (totalCount >= globalLimit)
            {
                // global limit reached
                _logger.LogInformation($"Global whitelisting limit reached  Current count: {totalCount}  limit {globalLimit}");

                return BadRequest(new string[] {
                    _stringLocalizer["USR_ERR_019"],
                });
            }

            // per-source limit is skipped for super-admin
            bool shouldSkipPerSourceLimit = this.User.IsInRole(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin);
            if (!shouldSkipPerSourceLimit)
            {
                var sourceCount = await _userRepository.CountOfUsersWhitelistedByUserId(callerUserId);
                var sourceLimit = _appSettings.WhitelistedUsersPerSourceLimit;
                if (sourceCount >= sourceLimit)
                {
                    // per-source limit reached
                    _logger.LogInformation($"whitelisting limit reached for user {callerUserId}  Current count: {totalCount}  limit {globalLimit}");
    
                    return BadRequest(new string[] {
                        _stringLocalizer["USR_ERR_019"],
                    });
                }
            }

            // create new entry
            var whitelistedUser = new WhitelistedUserDto()
            {
                PhoneNumber = request.PhoneNumber,
                WhitelistedByUserId = callerUserId,
                EmailId = request.EmailId,
                PartnerCode = request.PartnerCode,
                PartnerProvidedRetailerId = request.PartnerProvidedRetailerId,
                WhitelistedByUserRole = callerRole,
            };

            var result = await _userRepository.CreateWhitelistedUser(whitelistedUser);
            return Ok(result);
        }

        /// <summary>
        /// API to delete a whitelisted user
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        [HttpDelete("delete", Name = nameof(DeleteWhitelistedUser))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<int>> DeleteWhitelistedUser(DeleteWhitelistedUserRequest request)
        {
            if (!common.dto.User.User.IsPhoneNumberValid(request.PhoneNumber))
            {
                return BadRequest(new string[]
                {
                    _stringLocalizer["USR_ERR_010"],
                });
            }

            var result = await _userRepository.DeleteWhitelistedUser(request.PhoneNumber);
            if (result == (int)System.Net.HttpStatusCode.NoContent)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result);
            }
        }

        private string GetCurrentUserRoleForWhitelisting()
        {
            if (this.User.IsInRole(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin))
            {
                return ApplicationConstants.KaizalaIdentityRoles.SuperAdmin;
            }

            if (this.User.IsInRole(ApplicationConstants.KaizalaIdentityRoles.Retailer))
            {
                return ApplicationConstants.KaizalaIdentityRoles.Retailer;
            }

            return null;
        }
    }
}
