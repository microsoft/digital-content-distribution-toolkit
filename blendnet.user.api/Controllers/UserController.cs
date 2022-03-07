using AutoMapper;
using blendnet.api.proxy.Notification;
using blendnet.api.proxy.Retailer;
using blendnet.common.dto;
using blendnet.common.dto.Events;
using blendnet.common.dto.Extensions;
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
using Microsoft.Extensions.Options;

namespace blendnet.user.api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = KaizalaIdentityAuthOptions.DefaultScheme)]
    public class UserController : ControllerBase
    {
        private readonly ILogger _logger;

        private readonly IUserRepository _userRepository;

        private readonly RetailerProxy _retailerProxy;

        private readonly RetailerProviderProxy _retailerProviderProxy;

        private readonly NotificationProxy _notificationProxy;

        private readonly IEventBus _eventBus;

        private readonly UserAppSettings _appSettings;

        private readonly IStringLocalizer<SharedResource> _stringLocalizer;

        private readonly IMapper _mapper;

        private readonly TelemetryClient _telemetryClient;

        public UserController(IUserRepository userRepository,
                              ILogger<UserController> logger,
                              RetailerProxy retailerProxy,
                              RetailerProviderProxy retailerProviderProxy,
                              NotificationProxy notificationProxy,
                              IEventBus eventBus,
                              IOptionsMonitor<UserAppSettings> optionsMonitor,
                              IStringLocalizer<SharedResource> stringLocalizer,
                              IMapper mapper,
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
            _mapper = mapper;
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
        [HttpGet("dataexport/list", Name = nameof(GetUsersForExport))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<List<User>>> GetUsersForExport()
        {
            List<User> users = await _userRepository.GetUsersForExport();

            return Ok(users);
        }

        /// <summary>
        /// Get current user details
        /// </summary>
        /// <param name="User"></param>
        /// <returns>User Object</returns>
        [HttpGet("datadelete/list", Name = nameof(GetUsersForDelete))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<List<User>>> GetUsersForDelete()
        {
            List<User> users = await _userRepository.GetUsersForDelete();

            return Ok(users);
        }

        /// <summary>
        /// Get Referral Summary
        /// </summary>
        /// <param name="partnerProvidedRetailerId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet("summary/{partnerProvidedRetailerId}", Name = nameof(GetReferralSummary))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.RetailerManagement)]
        public async Task<ActionResult> GetReferralSummary(string partnerProvidedRetailerId, int startDate, int endDate)
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

            string partnerId = RetailerDto.CreatePartnerId(retailerProvider.PartnerCode, partnerProvidedRetailerId);

            List<ReferralSummary> referralData = await _userRepository.GetReferralSummary(partnerId, startDate, endDate);
            if (referralData == null || referralData.Count == 0)
            {
                return NotFound();
            }

            return Ok(referralData);
        }


        /// <summary>
        /// Complete the data export command
        /// </summary>
        /// <returns></returns>
        [HttpPost("dataexport/complete", Name = nameof(CompleteDataExportCommand))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult> CompleteDataExportCommand(CompleteCommandRequest completeDataExportCommandRequest)
        {
            List<string> errorInfo = new List<string>();

            string phoneNumber = completeDataExportCommandRequest.UserPhoneNumber;

            var userId = UserClaimData.GetUserId(this.User.Claims);

            var existingUser = await _userRepository.GetUserByPhoneNumber(phoneNumber);

            if (existingUser is null || !existingUser.DataExportStatusUpdatedBy.HasValue)
            {
                return NotFound();
            }

            //User should be in Active State
            if (existingUser.AccountStatus != UserAccountStatus.Active)
            {
                errorInfo.Add(_stringLocalizer["USR_ERR_024"]);

                return BadRequest(errorInfo);
            }


            //get existing command details
            UserCommand existingExportCommand = await _userRepository.GetCommand(phoneNumber, existingUser.DataExportStatusUpdatedBy.Value);

            if (existingExportCommand is null)
            {
                return NotFound();
            }

            //if the request is in progress do not accept another request
            //command and user status should be in submitted state
            if (existingUser.DataExportRequestStatus != DataExportRequestStatus.Submitted ||
                existingExportCommand.DataExportRequestStatus != DataExportRequestStatus.Submitted
                )
            {
                errorInfo.Add(_stringLocalizer["USR_ERR_022"]);

                return BadRequest(errorInfo);
            }

            DateTime now = DateTime.UtcNow;

            existingUser.DataExportRequestStatus = DataExportRequestStatus.ExportInProgress;
            existingUser.ModifiedByByUserId = userId;
            existingUser.ModifiedDate = now;

            existingExportCommand.DataExportRequestStatus = DataExportRequestStatus.ExportInProgress;
            existingExportCommand.ModifiedByByUserId = userId;
            existingExportCommand.ModifiedDate = now;

            CommandExecutionDetails executionDetails = new CommandExecutionDetails()
            {
                EventName = DataExportRequestStatus.ExportInProgress.ToString(),
                EventDateTime = now
            };

            existingExportCommand.ExecutionDetails.Add(executionDetails);

            //update the command and user object 
            await _userRepository.UpdateCommandBatch(existingExportCommand, existingUser);

            //publish the event so that each individual listener can export the data and notify back
            ExportUserDataIntegrationEvent exportUserDataIntegrationEvent = new ExportUserDataIntegrationEvent()
            {
                UserId = existingUser.UserId,
                UserPhoneNumber = existingUser.PhoneNumber,
                CommandId = existingExportCommand.Id,
            };

            await _eventBus.Publish(exportUserDataIntegrationEvent);

            return NoContent();
        }



        /// <summary>
        /// Complete the data delete command
        /// </summary>
        /// <returns></returns>
        [HttpPost("datadelete/complete", Name = nameof(CompleteDataDeleteCommand))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult> CompleteDataDeleteCommand(CompleteCommandRequest completeDataDeleteCommandRequest)
        {
            List<string> errorInfo = new List<string>();

            string phoneNumber = completeDataDeleteCommandRequest.UserPhoneNumber;

            var userId = UserClaimData.GetUserId(this.User.Claims);

            var existingUser = await _userRepository.GetUserByPhoneNumber(phoneNumber);

            if (existingUser is null || !existingUser.DataUpdateStatusUpdatedBy.HasValue)
            {
                return NotFound();
            }

            // User should not be in Active State
            if (existingUser.AccountStatus == UserAccountStatus.Active)
            {
                errorInfo.Add(_stringLocalizer["USR_ERR_026"]);
                return BadRequest(errorInfo);
            }

            //get existing command details
            UserCommand existingUpdateCommand = await _userRepository.GetCommand(phoneNumber, existingUser.DataUpdateStatusUpdatedBy.Value);

            if (existingUpdateCommand is null)
            {
                return NotFound();
            }

            //if the request is in progress do not accept another request
            //if the user is deleted does not 
            if (existingUser.DataUpdateRequestStatus != DataUpdateRequestStatus.Submitted ||
                existingUpdateCommand.DataUpdateRequestStatus != DataUpdateRequestStatus.Submitted
                )
            {
                errorInfo.Add(_stringLocalizer["USR_ERR_023"]);

                return BadRequest(errorInfo);
            }

            DateTime now = DateTime.UtcNow;

            existingUser.DataUpdateRequestStatus = DataUpdateRequestStatus.UpdateInProgress;
            existingUser.ModifiedByByUserId = userId;
            existingUser.ModifiedDate = now;

            existingUpdateCommand.DataUpdateRequestStatus = DataUpdateRequestStatus.UpdateInProgress;
            existingUpdateCommand.ModifiedByByUserId = userId;
            existingUpdateCommand.ModifiedDate = now;

            CommandExecutionDetails executionDetails = new CommandExecutionDetails()
            {
                EventName = DataUpdateRequestStatus.UpdateInProgress.ToString(),
                EventDateTime = now
            };

            existingUpdateCommand.ExecutionDetails.Add(executionDetails);

            //update the command and user object 
            await _userRepository.UpdateCommandBatch(existingUpdateCommand, existingUser);

            //publish the event so that each individual listener can update the data and notify back
            UpdateUserDataIntegrationEvent updateUserDataIntegrationEvent = new UpdateUserDataIntegrationEvent()
            {
                UserId = existingUser.UserId,
                UserPhoneNumber = existingUser.PhoneNumber,
                CommandId = existingUpdateCommand.Id,
            };

            await _eventBus.Publish(updateUserDataIntegrationEvent);

            return NoContent();
        }


        /// <summary>
        /// Get command details
        /// </summary>
        /// <param name="User"></param>
        /// <returns>User Object</returns>
        [HttpPost("command", Name = nameof(GetCommand))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<UserCommand>> GetCommand(UserCommandRequest request)
        {
            List<string> errorInfo = new List<string>();

            string partitionKey = string.Empty;

            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                partitionKey = request.PhoneNumber;

            }
            else if (request.UserId.HasValue && request.UserId.Value != Guid.Empty)
            {
                partitionKey = request.UserId.ToString();
            }

            if (string.IsNullOrEmpty(partitionKey))
            {
                errorInfo.Add(_stringLocalizer["USR_ERR_025"]);

                return BadRequest(errorInfo);
            }

            UserCommand userCommand = await _userRepository.GetCommand(partitionKey, request.CommandId);

            if (userCommand is null)
            {
                return NotFound();

            }
            else
            {
                return Ok(userCommand);
            }
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
                    PhoneNumberChecksum = retailerRequest.PhoneNumber.Checksum(),
                    Name = "",
                    ChannelId = Channel.NovoRetailerApp, // TODO: this should be from the Claim / partnerCode
                    CreatedDate = now,
                    CreatedByUserId = callerUserId,
                    AccountStatus = UserAccountStatus.Active,
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
                UserId = user.UserId,
            };

            _telemetryClient.TrackEvent(createRetailerAIEvent);

            return Ok(retailerRequest.RetailerId);
        }

        #endregion
    }
}
