using AutoMapper;
using blendnet.api.proxy.Notification;
using blendnet.api.proxy.Retailer;
using blendnet.common.dto;
using blendnet.common.dto.Events;
using blendnet.common.dto.Notification;
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

        private readonly IDistributedCache _cache;


        public UserController(IUserRepository userRepository,
                              ILogger<UserController> logger,
                              RetailerProxy retailerProxy,
                              RetailerProviderProxy retailerProviderProxy,
                              NotificationProxy notificationProxy,
                              IEventBus eventBus,
                              IOptionsMonitor<UserAppSettings> optionsMonitor,
                              IStringLocalizer<SharedResource> stringLocalizer,
                              IMapper mapper,
                              IDistributedCache cache,
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
            _cache = cache;
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
            UserCommand userCommand = await _userRepository.GetCommand(request.PhoneNumber, request.CommandId);

            if (userCommand is null)
            {
                return NotFound();
            }else
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