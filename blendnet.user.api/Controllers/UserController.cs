using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using blendnet.api.proxy.Retailer;
using blendnet.common.dto;
using blendnet.common.dto.User;
using blendnet.user.api.Request;
using blendnet.user.repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace blendnet.user.api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger _logger;

        private IUserRepository _userRepository;

        private RetailerProxy _retailerProxy;

        public UserController(IUserRepository userRepository,
                              ILogger<UserController> logger,
                              RetailerProxy retailerProxy)
        {
            _logger = logger;
            _userRepository = userRepository;
            _retailerProxy = retailerProxy;
        }

        /// <summary>
        /// Create BlendNet User
        /// </summary>
        /// <param name="User"></param>
        /// <returns>Status</returns>
        [HttpPost("createuser", Name = nameof(CreateUser))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult> CreateUser(CreateUserRequest request)
        {
            List<string> errorInfo = new List<string>();

            Guid userId = UserClaimData.GetUserId(User.Claims);
            String phoneNumber = this.User.Identity.Name;

            if (await _userRepository.GetUserByPhoneNumber(phoneNumber) != null)
            {
                errorInfo.Add($"User Already exists in the system {phoneNumber}");
                return BadRequest(errorInfo);
            }

            User user = new User
            {
                    Id = userId, 
                    PhoneNumber = phoneNumber, 
                    UserName = request.UserName,
                    ChannelId = request.ChannelId,
                    CreatedDate = DateTime.UtcNow,
                    CreatedByUserId = userId
            };

            await _userRepository.CreateUser(user);

            return Ok(user.Id);
        }

        /// <summary>
        /// Get user using phone number
        /// </summary>
        /// <param name="User"></param>
        /// <returns>User Object</returns>
        [HttpGet("getuser/{phoneNumber}", Name = nameof(GetUserByPhoneNumber))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
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
        /// Get current user details
        /// </summary>
        /// <param name="User"></param>
        /// <returns>User Object</returns>
        [HttpGet("getuser", Name = nameof(GetUser))]
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
        [HttpPost("assignretailer", Name = nameof(AssignRetailer))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<User>> AssignRetailer(ReferralDto referralDto)
        {
            List<string> errorInfo = new List<string>();

            String phoneNumber = this.User.Identity.Name;
            User user = await _userRepository.GetUserByPhoneNumber(phoneNumber);

            if (user == null)
            {
                errorInfo.Add($"No valid details found for current user {phoneNumber}");
                return NotFound(errorInfo);
            }

            if (user.ChannelId != Channel.Customer)
            {
                errorInfo.Add("Only Customers are allowerd to enter referral info");
                return BadRequest(errorInfo);
            }

            if (user.ReferralInfo != null)
            {
                errorInfo.Add("ReferralCode is already assigned");
                return BadRequest(errorInfo);
            }

            if (ValidateReferralData(referralDto) == false)
            {
                errorInfo.Add("Invalid Retailer Data");
                return BadRequest(errorInfo);
            }

            var currentDate = DateTime.UtcNow;
            referralDto.ReferralDate = Int32.Parse(currentDate.ToString(ApplicationConstants.DateTimeFormats.FormatYYYYDDMM));
            referralDto.ReferralDateTime = currentDate;

            user.ReferralInfo = referralDto;

            user.ModifiedByByUserId = UserClaimData.GetUserId(User.Claims);
            user.ModifiedDate = DateTime.UtcNow;

            int statusCode = await _userRepository.UpdateUser(user);
            if (statusCode == (int)System.Net.HttpStatusCode.OK)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("summary/{retailerPartnerId}", Name = nameof(GetReferralSummary))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult> GetReferralSummary(string retailerPartnerId, int startDate, int endDate)
        {
            List<string> errorDetails = new List<string>();

            if (startDate <= 0 || endDate <= 0)
            {
                errorDetails.Add("Invalid start or end date");
                return BadRequest(errorDetails);
            }

            if (startDate > endDate)
            {
                errorDetails.Add("Invalid date range");
                return BadRequest(errorDetails);
            }

            List<ReferralSummary> referralData = await _userRepository.GetReferralSummary(retailerPartnerId, startDate, endDate);
            if (referralData == null || referralData.Count == 0)
            {
                return NotFound();
            }

            return Ok(referralData);
        }

        #region private methods
        /// <summary>
        /// Validate Retailer referral info
        /// </summary>
        /// <param name="ReferralDto"></param>
        /// <returns>Success/Fail</returns>
        private bool ValidateReferralData(ReferralDto referralDto)
        {
            //TODO: Validation check to be implemented
            return true;
        }
        #endregion
    }
}