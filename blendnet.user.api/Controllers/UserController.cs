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
using System.Threading.Tasks;

namespace blendnet.user.api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = KaizalaIdentityAuthOptions.DefaultScheme)]
    public class UserController : ControllerBase
    {
        private readonly ILogger _logger;

        private IUserRepository _userRepository;

        private RetailerProxy _retailerProxy;

        private RetailerProviderProxy _retailerProviderProxy;

        private IEventBus _eventBus;

        private UserAppSettings _appSettings;

        IStringLocalizer<SharedResource> _stringLocalizer;

        private TelemetryClient _telemetryClient;

        public UserController(IUserRepository userRepository,
                              ILogger<UserController> logger,
                              RetailerProxy retailerProxy,
                              RetailerProviderProxy retailerProviderProxy,
                              IEventBus eventBus,
                              IOptionsMonitor<UserAppSettings> optionsMonitor,
                              IStringLocalizer<SharedResource> stringLocalizer,
                              TelemetryClient telemetryClient)
        {
            _logger = logger;
            _userRepository = userRepository;
            _retailerProxy = retailerProxy;
            _retailerProviderProxy = retailerProviderProxy;
            _eventBus = eventBus;
            _appSettings = optionsMonitor.CurrentValue;
            _stringLocalizer = stringLocalizer;
            _telemetryClient = telemetryClient;
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
        [HttpPost("user", Name = nameof(GetUserByPhoneNumber))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<User>> GetUserByPhoneNumber(UserByPhoneRequest request)
        {
            User user = await _userRepository.GetUserByPhoneNumber(request.PhoneNumber);

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
        [HttpGet("me", Name = nameof(GetUser))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<User>> GetUser()
        {
            UserByPhoneRequest userByPhoneRequest = new UserByPhoneRequest() { PhoneNumber = this.User.Identity.Name };

            return await GetUserByPhoneNumber(userByPhoneRequest);
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
                RetailerId = retailerDto.RetailerId,
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
            RetailerProviderDto retailerProvider = await _retailerProviderProxy.GetRetailerProviderByUserId(callerUserId);
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

        /// <summary>
        /// API to get the latest Data Export Request for a user
        /// </summary>
        /// <param name="request">Request for user (needed only for admin, ignored for others)</param>
        /// <returns></returns>
        [HttpPost("dataExport", Name = nameof(GetDataExportCommand))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<UserDataExportCommand>> GetDataExportCommand(UserForDataExportRequest request)
        {
            string phoneNumber;

            if (UserClaimData.isSuperAdmin(this.User.Claims))
            {
                // expect a phone number in requestUrl
                phoneNumber = request.PhoneNumber;
            }
            else
            {
                phoneNumber = this.User.Identity.Name;
            }

            var existingUser = await _userRepository.GetUserByPhoneNumber(phoneNumber);
            if (existingUser is null || !existingUser.DataExportRequestedBy.HasValue)
            {
                return NotFound();
            }

            var dataExportCommand = await _userRepository.GetDataExportCommand(phoneNumber, existingUser.DataExportRequestedBy.Value);
            if (dataExportCommand is null)
            {
                return NotFound();
            }
            else
            {
                return Ok(dataExportCommand);
            }
        }

        /// <summary>
        /// API to create a new data export request for user
        /// </summary>
        /// <returns></returns>
        [HttpPost("dataExport/create", Name = nameof(CreateDataExportCommand))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<int>> CreateDataExportCommand()
        {
            string phoneNumber = this.User.Identity.Name;
            var userId = UserClaimData.GetUserId(this.User.Claims);

            var existingUser = await _userRepository.GetUserByPhoneNumber(phoneNumber);
            if (existingUser is null)
            {
                return NotFound();
            }

            if (existingUser.DataExportRequestedBy.HasValue)
            {
                var existingDataExportCommand = await _userRepository.GetDataExportCommand(phoneNumber, existingUser.DataExportRequestedBy.Value);
                if (existingDataExportCommand is not null && existingDataExportCommand.Status != DataExportRequestStatus.Completed)
                {
                    return BadRequest(new string[] {
                        "Request already exists"
                    });
                }
            }

            var now = DateTime.UtcNow;

            var newDataExportCommand = new UserDataExportCommand()
            {
                CreatedByUserId = userId,
                CreatedDate = now,
                Id = Guid.NewGuid(),
                PhoneNumber = phoneNumber,
                Status = DataExportRequestStatus.Active,
            };

            existingUser.DataExportRequestedBy = newDataExportCommand.Id;
            existingUser.ModifiedByByUserId = userId;
            existingUser.ModifiedDate = now;

            var response = await _userRepository.CreateDataExportCommandBatch(newDataExportCommand, existingUser);

            var aiEvent = new CreateUserDataExportCommandAIEvent()
            {
                RequestId = newDataExportCommand.Id,
                UserId = userId,
            };

            _telemetryClient.TrackEvent(aiEvent);

            return Ok(response);
        }

        /// <summary>
        /// API to update the result in a Data Export Request
        /// </summary>
        /// <param name="resultRequest">request</param>
        /// <returns>status code</returns>
        [HttpPost("dataExport/updateResult", Name = nameof(UpdateDataExportResult))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<int>> UpdateDataExportResult(UserDataExportResultRequest resultRequest)
        {
            Guid callerUserId = UserClaimData.GetUserId(this.User.Claims);
            string phoneNumber = resultRequest.PhoneNumber;
            var existingUser = await _userRepository.GetUserByPhoneNumber(phoneNumber);
            if (existingUser is null)
            {
                return BadRequest(new string[] {
                    "User not found"
                });
            }

            if (!existingUser.DataExportRequestedBy.HasValue)
            {
                return BadRequest(new string[] {
                    "No valid data export request found"
                });
            }

            var existingDataExportCommand = await _userRepository.GetDataExportCommand(phoneNumber, existingUser.DataExportRequestedBy.Value);
            if (existingDataExportCommand is null || existingDataExportCommand.Status != DataExportRequestStatus.Active)
            {
                return BadRequest(new string[] {
                    "No valid data export request found"
                });
            }

            var now = DateTime.UtcNow;

            existingDataExportCommand.Result = new UserDataExportResult()
            {
                DateCompleted = now,
                ExportedDataUrl = resultRequest.ExportedDataUrl,
                ExportedDataValidity = resultRequest.ExportedDataValidity,
            };

            existingDataExportCommand.Status = DataExportRequestStatus.Completed;
            existingDataExportCommand.ModifiedByByUserId = callerUserId;
            existingDataExportCommand.ModifiedDate = now;

            return await _userRepository.UpdateDataExportCommand(existingDataExportCommand);
        }

        #region private methods

        /// <summary>
        /// Logic for creating retailer from the request
        /// </summary>
        /// <param name="retailerRequest">request</param>
        /// <returns>Retailer ID of the created retailer</returns>
        private async Task<ActionResult<string>> CreateRetailerInternal(CreateRetailerRequest retailerRequest)
        {
            Guid callerUserId = UserClaimData.GetUserId(User.Claims);
            RetailerProviderDto retailerProvider = await _retailerProviderProxy.GetRetailerProviderByUserId(callerUserId);

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
                    listOfValidationErrors.Add(string.Format(_stringLocalizer["USR_ERR_013"], callerUserId));
                    return BadRequest(listOfValidationErrors);
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
            var user = await _userRepository.GetUserByPhoneNumber(retailerRequest.PhoneNumber);
            if (user is null)
            {
                user = new User
                {
                    UserId = Guid.NewGuid(),
                    IdentityId = retailerRequest.UserId, 
                    PhoneNumber = retailerRequest.PhoneNumber, 
                    Name = "",
                    ChannelId = Channel.NovoRetailerApp, // TODO: this should be from the Claim / partnerCode
                    CreatedDate = now,
                    CreatedByUserId = callerUserId,
                };

                await _userRepository.CreateUser(user);
            }

            // create RetailerDto from request
            RetailerDto retailer = new RetailerDto()
            {
                // Base propeties
                CreatedByUserId = callerUserId,
                CreatedDate = now,

                // Retailer properties
                RetailerId = Guid.NewGuid(),
                PhoneNumber = retailerRequest.PhoneNumber,
                Name = retailerRequest.Name,
                UserId = user.UserId, 
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
                City = retailer.Address.City,
                Latitude = retailer.Address.MapLocation.Latitude,
                Longitude = retailer.Address.MapLocation.Longitude,
                Name = retailer.Name,
                PartnerCode = retailer.PartnerCode,
                PartnerProvidedId = retailer.PartnerProvidedId,
                PinCode = retailer.Address.PinCode,
                RetailerPartnerId = retailer.PartnerId,
                State = retailer.Address.State,
                AdditionalAttributes = retailer.AdditionalAttibutes,
            };

            _telemetryClient.TrackEvent(createRetailerAIEvent);

            return Ok(retailerRequest.RetailerId);
        }

        #endregion
    }
}