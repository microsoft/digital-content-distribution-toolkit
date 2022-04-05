using blendnet.api.proxy.Device;
using blendnet.common.dto;
using blendnet.common.dto.Common;
using blendnet.common.dto.Retailer;
using blendnet.common.dto.User;
using blendnet.common.infrastructure.Authentication;
using blendnet.common.infrastructure.Extensions;
using blendnet.retailer.api.Models;
using blendnet.retailer.repository.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.retailer.api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin, 
                    ApplicationConstants.KaizalaIdentityRoles.Retailer, 
                    ApplicationConstants.KaizalaIdentityRoles.HubDeviceManagement,
                    ApplicationConstants.KaizalaIdentityRoles.AnalyticsReporter)]
    public class RetailerController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IRetailerRepository _retailerRepository;
        private readonly IStringLocalizer<SharedResource> _stringLocalizer;
        private readonly IRetailerProviderRepository _retailerProviderRepository;
        private readonly DeviceProxy _deviceProxy;
        private readonly TelemetryClient _telemetryClient;

        public RetailerController(  ILogger<RetailerController> logger,
                                    IRetailerRepository retailerRepository,
                                    IStringLocalizer<SharedResource> stringLocalizer,
                                    IRetailerProviderRepository retailerProviderRepository,
                                    TelemetryClient telemetryClient,
                                    DeviceProxy deviceProxy)
        {
            this._logger = logger;
            this._retailerRepository = retailerRepository;
            this._retailerProviderRepository = retailerProviderRepository;
            this._stringLocalizer = stringLocalizer;
            this._telemetryClient = telemetryClient;
            this._deviceProxy = deviceProxy;
        }

        #region API methods

        [HttpGet("byPartnerId/{retailerPartnerId}", Name = nameof(GetRetailerByPartnerId))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<RetailerDto>> GetRetailerByPartnerId(string retailerPartnerId /* composed */)
        {
            bool isSuperAdmin = UserClaimData.isSuperAdmin(this.User.Claims);
            RetailerDto retailer = await this._retailerRepository.GetRetailerByPartnerId(retailerPartnerId, shouldGetInactiveRetailer: isSuperAdmin);
            return retailer != null ? Ok(retailer) : NotFound();
        }

        [HttpGet("byReferralCode/{referralCode}", Name = nameof(GetRetailerByReferralCode))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<RetailerDto>> GetRetailerByReferralCode(string referralCode)
        {
            RetailerDto retailer = await this._retailerRepository.GetRetailerByReferralCode(referralCode);
            return retailer != null ? Ok(retailer) : NotFound();
        }

        [HttpPut("{retailerPartnerId}", Name = nameof(UpdateRetailer))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<string>> UpdateRetailer(string retailerPartnerId /* composed */,
                                                                RetailerDto retailer)
        {
            var existingRetailer = await this._retailerRepository.GetRetailerByPartnerId(retailerPartnerId, true);
            if (existingRetailer == null)
            {
                return NotFound();
            }
        
            var listOfValidationErrors = new List<string>();
            if (retailer.Address == null || retailer.Address.MapLocation == null || !retailer.Address.MapLocation.isValid())
            {
                listOfValidationErrors.Add(_stringLocalizer["RMS_ERR_0001"]);
            }

            if (retailer.StartDate > retailer.EndDate)
            {
                listOfValidationErrors.Add(_stringLocalizer["RMS_ERR_0002"]);
            }

            if (listOfValidationErrors.Count > 0)
            {
                return BadRequest(listOfValidationErrors);
            }

            Guid callerUserId = UserClaimData.GetUserId(User.Claims);

            // update the metadata
            retailer.ModifiedDate = DateTime.UtcNow;
            retailer.ModifiedByByUserId = callerUserId;

            int response = await this._retailerRepository.UpdateRetailer(retailer);
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
        /// API to get Retailer object for self
        /// </summary>
        /// <param name="partnerCode">Partner Code</param>
        /// <param name="partnerProvidedRetailerId">Retailer ID as assigned by the partner</param>
        /// <returns></returns>
        [HttpGet("{partnerCode}/{partnerProvidedRetailerId}/me", Name = nameof(GetSelfRetailer))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin, ApplicationConstants.KaizalaIdentityRoles.Retailer)]
        public async Task<ActionResult<RetailerDto>> GetSelfRetailer(string partnerCode, string partnerProvidedRetailerId)
        {
            Guid callerUserId = UserClaimData.GetUserId(this.User.Claims);
            string partnerId = RetailerDto.CreatePartnerId(partnerCode, partnerProvidedRetailerId);

            RetailerDto retailer = await this._retailerRepository.GetRetailerByPartnerId(partnerId);

            if (retailer is null)
            {
                return NotFound();
            }

            // return result if caller matches, or caller is super admin
            if (retailer.UserId == callerUserId || UserClaimData.isSuperAdmin(this.User.Claims))
            {
                return Ok(retailer);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// API to create unlinked retailer
        /// </summary>
        /// <param name="partnerCode">Partner Code</param>
        /// <param name="retailerRequest">Request</param>
        /// <returns></returns>
        [HttpPost("{partnerCode}/unlinkedRetailer", Name = nameof(CreateUnlinkedRetailer))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult> CreateUnlinkedRetailer(string partnerCode, CreateUnlinkedRetailerRequest retailerRequest)
        {
            var callerUserId = UserClaimData.GetUserId(this.User.Claims);

            string requestedPartnerId = RetailerDto.CreatePartnerId(partnerCode, retailerRequest.PartnerProvidedId);
            
            var existingRetailer = await _retailerRepository.GetRetailerByPartnerId(requestedPartnerId, shouldGetInactiveRetailer: true);
            if (existingRetailer != null) 
            {
                // retailer already exists, error out
                return BadRequest(new string[] {
                    string.Format(_stringLocalizer["RMS_ERR_0006"], retailerRequest.PartnerProvidedId, partnerCode),
                });
            }

            var retailerProvider = await _retailerProviderRepository.GetRetailerProviderByPartnerCode(partnerCode);
            if (retailerProvider == null)
            {
                // retailer provider not found, error out
                return BadRequest(new string[] {
                    string.Format(_stringLocalizer["RMS_ERR_0007"], partnerCode),
                });
            }

            // map validation
            if (!retailerRequest.Address.MapLocation.isValid())
            {
                // retailer provider not found, error out
                return BadRequest(new string[] {
                    _stringLocalizer["RMS_ERR_0008"],
                });
            }

            var now = DateTime.UtcNow;
            // Create the retailer
            RetailerDto retailerToCreate = new RetailerDto()
            {
                AdditionalAttibutes = retailerRequest.AdditionalAttibutes,
                Address = retailerRequest.Address,
                CreatedByUserId = callerUserId,
                CreatedDate = now,
                EndDate = DateTime.MinValue,
                Name = retailerRequest.Name,
                PartnerCode = retailerProvider.PartnerCode,
                PartnerProvidedId = retailerRequest.PartnerProvidedId,
                PhoneNumber = null,
                Services = new List<ServiceType>() { ServiceType.Media },
                ModifiedByByUserId = null,
                ModifiedDate = null,
                StartDate = DateTime.MinValue,
                UserId = Guid.Empty,
                ReferralCode = null,
            };

            await _retailerRepository.CreateRetailer(retailerToCreate);

            return Ok(retailerToCreate.RetailerId);
        }

        /// <summary>
        /// API to get retailers to whom the specified device was ever assigned
        /// </summary>
        /// <param name="deviceId">Device ID</param>
        /// <returns>Retailer</returns>
        [HttpGet("byDeviceId/{deviceId}", Name = nameof(GetRetailersByDevice))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin, ApplicationConstants.KaizalaIdentityRoles.HubDeviceManagement)]
        public async Task<ActionResult<List<RetailerDto>>> GetRetailersByDevice(string deviceId)
        {
            List<RetailerDto> retailers = await _retailerRepository.GetRetailersByDeviceId(deviceId);
            if (retailers is null || retailers.Count == 0)
            {
                return NotFound();
            }
            else
            {
                // keep only the device that is asked for
                foreach (var retailer in retailers)
                {
                    retailer.DeviceAssignments = retailer.DeviceAssignments.Where(asg => asg.DeviceId == deviceId).ToList();
                }

                return Ok(retailers);
            }
        }

        /// <summary>
        /// API to assign a device to retailer
        /// </summary>
        /// <param name="assignRequest">device assignment request</param>
        /// <returns></returns>
        [HttpPost("assignDevice", Name = nameof(AssignDeviceToRetailer))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin, ApplicationConstants.KaizalaIdentityRoles.HubDeviceManagement)]
        public async Task<ActionResult> AssignDeviceToRetailer(AssignDeviceRequest assignRequest)
        {
            var callerUserId = UserClaimData.GetUserId(this.User.Claims);
            var retailerProvider = await _retailerProviderRepository.GetRetailerProviderByPartnerCode(assignRequest.PartnerCode);
            if (retailerProvider is null)
            {
                _logger.LogInformation($"Unknown Partner code - {assignRequest.PartnerCode}");
                return BadRequest(new string[] {
                    string.Format(_stringLocalizer["RMS_ERR_0007"], assignRequest.PartnerCode),
                });
            }

            string partnerId = RetailerDto.CreatePartnerId(assignRequest.PartnerCode, assignRequest.PartnerProvidedRetailerId);
            RetailerDto existingRetailer = await _retailerRepository.GetRetailerByPartnerId(partnerId);

            if (existingRetailer is null)
            {
                _logger.LogInformation($"Unknown retailer ID - {partnerId}");
                return BadRequest(new string[] {
                    string.Format(_stringLocalizer["RMS_ERR_0009"], assignRequest.PartnerProvidedRetailerId),
                });
            }

            var existingDevice = await _deviceProxy.GetDevice(assignRequest.DeviceId);
            if (existingDevice is null)
            {
                _logger.LogInformation($"Unknown Device ID - {assignRequest.DeviceId}");
                return BadRequest(new string[] {
                    string.Format(_stringLocalizer["RMS_ERR_0010"], assignRequest.DeviceId),
                });
            }

            if (existingDevice.DeviceStatus != common.dto.Device.DeviceStatus.Provisioned)
            {
                _logger.LogInformation($"Device is not in right state. Device ID - {assignRequest.DeviceId}  Status - {existingDevice.DeviceStatus}");
                return BadRequest(new string[] {
                    _stringLocalizer["RMS_ERR_0013"],
                });
            }

            var now = DateTime.UtcNow;

            // Requested retailer should not have an active device already
            var existingRetailerActiveDeviceAssignment = existingRetailer.DeviceAssignments
                                                            .Where(asg => asg.IsActive)
                                                            .FirstOrDefault();
            if (existingRetailerActiveDeviceAssignment is not null)
            {
                _logger.LogInformation($"retailer {existingRetailer.PartnerId} already has an active device {existingRetailerActiveDeviceAssignment.DeviceId}");
                return BadRequest(new string[] {
                    _stringLocalizer["RMS_ERR_0011"],
                });
            }

            List<RetailerDto> retailersWithDeviceId = await _retailerRepository.GetRetailersByDeviceId(assignRequest.DeviceId);
            var activeAssignmentOfDevice = retailersWithDeviceId
                                                .SelectMany(r => r.DeviceAssignments) // flatten the device assignment records across all retailers
                                                .Where(asg => asg.DeviceId == assignRequest.DeviceId) // keep only the requested device ID records
                                                .Where(asg => asg.IsActive) // keep only records that is active on the requested start date
                                                .FirstOrDefault();
            if (activeAssignmentOfDevice is not null)
            {
                // Device is actively assigned to some retailer
                _logger.LogInformation($"Requested device {assignRequest.DeviceId} is already assigned to some retailer");
                return BadRequest(new string[] {
                    _stringLocalizer["RMS_ERR_0012"],
                });
            }

            // all seems OK now, so assign the requested device
            var newAssignment = new RetailerDeviceAssignment()
            {
                DeviceId = assignRequest.DeviceId,
                AssignmentStartDate = now,
                AssignmentEndDate = DateTime.MaxValue,
            };

            existingRetailer.DeviceAssignments.Add(newAssignment);
            existingRetailer.ModifiedByByUserId = callerUserId;
            existingRetailer.ModifiedDate = now;

            await _retailerRepository.UpdateRetailer(existingRetailer);

            AssignDeviceToRetailerAIEvent aiEvent = new AssignDeviceToRetailerAIEvent()
            {
                PartnerCode = existingRetailer.PartnerCode,
                PartnerProvidedId = existingRetailer.PartnerProvidedId,
                RetailerId = existingRetailer.PartnerId,
                DeviceId = newAssignment.DeviceId,
                AssignmentStartDate = newAssignment.AssignmentStartDate,
            };

            _telemetryClient.TrackEvent(aiEvent);

            return NoContent();
        }

        /// <summary>
        /// API to unassign active device for retailer
        /// </summary>
        /// <param name="unassignRequest">Unassign Device Request</param>
        /// <returns></returns>
        [HttpPost("unassignDevice", Name = nameof(UnassignDevice))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin, ApplicationConstants.KaizalaIdentityRoles.HubDeviceManagement)]
        public async Task<ActionResult> UnassignDevice(UnassignDeviceRequest unassignRequest)
        {
            var callerUserId = UserClaimData.GetUserId(this.User.Claims);
            var retailerProvider = await _retailerProviderRepository.GetRetailerProviderByPartnerCode(unassignRequest.PartnerCode);
            if (retailerProvider is null)
            {
                _logger.LogInformation($"Unknown Partner code - {unassignRequest.PartnerCode}");
                return BadRequest(new string[] {
                    string.Format(_stringLocalizer["RMS_ERR_0007"], unassignRequest.PartnerCode),
                });
            }

            string partnerId = RetailerDto.CreatePartnerId(unassignRequest.PartnerCode, unassignRequest.PartnerProvidedRetailerId);
            RetailerDto existingRetailer = await _retailerRepository.GetRetailerByPartnerId(partnerId);

            if (existingRetailer is null)
            {
                _logger.LogInformation($"Unknown retailer ID - {partnerId}");
                return BadRequest(new string[] {
                    string.Format(_stringLocalizer["RMS_ERR_0009"], partnerId),
                });
            }

            var activeAssignment = existingRetailer.DeviceAssignments
                                            .Where(asg => asg.DeviceId == unassignRequest.DeviceId)
                                            .Where(asg => asg.IsActive)
                                            .FirstOrDefault();
            if (activeAssignment is null)
            {
                _logger.LogInformation($"No assignment found for retailer {partnerId} for device {unassignRequest.DeviceId}");
                return BadRequest(new string[] {
                    _stringLocalizer["RMS_ERR_0015"],
                });
            }

            var now = DateTime.UtcNow;

            activeAssignment.AssignmentEndDate = now;
            existingRetailer.ModifiedByByUserId = callerUserId;
            existingRetailer.ModifiedDate = now;

            await _retailerRepository.UpdateRetailer(existingRetailer);

            UnassignDeviceToRetailerAIEvent aiEvent = new UnassignDeviceToRetailerAIEvent()
            {
                PartnerCode = existingRetailer.PartnerCode,
                PartnerProvidedId = existingRetailer.PartnerProvidedId,
                RetailerId = existingRetailer.PartnerId,
                DeviceId = unassignRequest.DeviceId,
                AssignmentStartDate = activeAssignment.AssignmentStartDate,
                AssignmentEndDate = activeAssignment.AssignmentEndDate,
            };

            _telemetryClient.TrackEvent(aiEvent);

            return NoContent();
        }

        /// <summary>
        /// API to mark a device as deployed at Retailer's location
        /// </summary>
        /// <param name="deployDeviceRequest">Deploy Device Request</param>
        /// <returns></returns>
        [HttpPost("deployDevice", Name = nameof(DeployDevice))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin, ApplicationConstants.KaizalaIdentityRoles.HubDeviceManagement)]
        public async Task<ActionResult> DeployDevice(DeployDeviceRequest deployDeviceRequest)
        {
            var callerUserId = UserClaimData.GetUserId(this.User.Claims);

            var retailerProvider = await _retailerProviderRepository.GetRetailerProviderByPartnerCode(deployDeviceRequest.PartnerCode);
            if (retailerProvider is null)
            {
                _logger.LogInformation($"Unknown Partner code - {deployDeviceRequest.PartnerCode}");
                return BadRequest(new string[] {
                    string.Format(_stringLocalizer["RMS_ERR_0007"], deployDeviceRequest.PartnerCode),
                });
            }

            string partnerId = RetailerDto.CreatePartnerId(deployDeviceRequest.PartnerCode, deployDeviceRequest.PartnerProvidedRetailerId);
            RetailerDto existingRetailer = await _retailerRepository.GetRetailerByPartnerId(partnerId);

            if (existingRetailer is null)
            {
                _logger.LogInformation($"Unknown retailer ID - {partnerId}");
                return BadRequest(new string[] {
                    string.Format(_stringLocalizer["RMS_ERR_0009"], partnerId),
                });
            }

            var activeAssignment = existingRetailer.DeviceAssignments
                                        .Where(asg => !asg.IsDeployed)
                                        .Where(asg => asg.IsActive)
                                        .Where(asg => asg.DeviceId == deployDeviceRequest.DeviceId)
                                        .FirstOrDefault();

            if (activeAssignment is null)
            {
                _logger.LogInformation($"No assignment found for retailer {partnerId} for device {deployDeviceRequest.DeviceId}");
                return BadRequest(new string[] {
                    _stringLocalizer["RMS_ERR_0015"],
                });
            }

            DateTime now = DateTime.UtcNow;

            // update the data
            activeAssignment.IsDeployed = true;
            activeAssignment.DeploymentDate = now;
            existingRetailer.ModifiedDate = now;
            existingRetailer.ModifiedByByUserId = callerUserId;

            await _retailerRepository.UpdateRetailer(existingRetailer);

            DeployDeviceToRetailerAIEvent aiEvent = new DeployDeviceToRetailerAIEvent()
            {
                PartnerCode = existingRetailer.PartnerCode,
                PartnerProvidedId = existingRetailer.PartnerProvidedId,
                DeviceId = activeAssignment.DeviceId,
                DeploymentDate = activeAssignment.DeploymentDate.Value,
                RetailerId = existingRetailer.RetailerId,
            };

            _telemetryClient.TrackEvent(aiEvent);

            return NoContent();
        }

        /// <summary>
        /// Returns the list of retailer 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("byPartnerCode", Name = nameof(GetRetailersByPartnerCode))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin, ApplicationConstants.KaizalaIdentityRoles.AnalyticsReporter)]
        public async Task<ActionResult<ResultData<RetailerDto>>> GetRetailersByPartnerCode(RetailersByPartnerCodeRequest request)
        {
            var retailerProvider = await _retailerProviderRepository.GetRetailerProviderByPartnerCode(request.PartnerCode);
            
            if (retailerProvider is null)
            {
                return BadRequest(new string[] {
                    string.Format(_stringLocalizer["RMS_ERR_0007"], request.PartnerCode),
                });
            }

            ResultData<RetailerDto> retailers = await _retailerRepository.GetRetailersByPartnerCode(request.PartnerCode,
                                                                                                    request.ContinuationToken,
                                                                                                    request.ShouldGetInactiveRetailer,
                                                                                                    request.PageSize);
            
            if (retailers is null || retailers.Data is null || retailers.Data.Count == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(retailers);
            }
        }

        /// <summary>
        /// Only returns the ids for the given partner code
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        [HttpPost("byPartnerCode/ids", Name = nameof(GetRetailerIdssByPartnerCode))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin, ApplicationConstants.KaizalaIdentityRoles.AnalyticsReporter)]
        public async Task<ActionResult<ResultData<string>>> GetRetailerIdssByPartnerCode(RetailersByPartnerCodeRequest request)
        {
            var retailerProvider = await _retailerProviderRepository.GetRetailerProviderByPartnerCode(request.PartnerCode);

            if (retailerProvider is null)
            {
                return BadRequest(new string[] {
                    string.Format(_stringLocalizer["RMS_ERR_0007"], request.PartnerCode),
                });
            }

            ResultData<RetailerDto> retailers = await _retailerRepository.GetRetailersByPartnerCode(request.PartnerCode,
                                                                                                    request.ContinuationToken,
                                                                                                    request.ShouldGetInactiveRetailer,
                                                                                                    request.PageSize);

            if (retailers is null || retailers.Data is null || retailers.Data.Count == 0)
            {
                return NotFound();
            }
            else
            {
                List<string> retailerIds = retailers.Data.Select(r=>r.PartnerId).ToList();

                ResultData<string> result = new ResultData<string>(retailerIds, retailers.ContinuationToken);

                return Ok(result);
            }
        }
        #endregion
    }
}
