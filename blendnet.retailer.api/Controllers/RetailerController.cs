using blendnet.api.proxy.Retailer;
using blendnet.common.dto;
using blendnet.common.dto.Events;
using blendnet.common.dto.Retailer;
using blendnet.common.dto.User;
using blendnet.common.infrastructure;
using blendnet.common.infrastructure.Authentication;
using blendnet.retailer.api.Models;
using blendnet.retailer.repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace blendnet.retailer.api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin, ApplicationConstants.KaizalaIdentityRoles.Retailer)]
    public class RetailerController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IRetailerRepository _retailerRepository;
        private readonly IStringLocalizer<SharedResource> _stringLocalizer;
        private readonly IRetailerProviderRepository _retailerProviderRepository;

        public RetailerController(ILogger<RetailerController> logger, IRetailerRepository retailerRepository, IStringLocalizer<SharedResource> stringLocalizer, IRetailerProviderRepository retailerProviderRepository)
        {
            this._logger = logger;
            this._retailerRepository = retailerRepository;
            this._retailerProviderRepository = retailerProviderRepository;
            _stringLocalizer = stringLocalizer;
        }

        #region API methods

        [HttpGet("byPartnerId/{retailerPartnerId}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<RetailerDto>> GetRetailerByPartnerId(string retailerPartnerId /* composed */)
        {
            bool isSuperAdmin = UserClaimData.isSuperAdmin(this.User.Claims);
            RetailerDto retailer = await this._retailerRepository.GetRetailerByPartnerId(retailerPartnerId, shouldGetInactiveRetailer: isSuperAdmin);
            return retailer != null ? Ok(retailer) : NotFound();
        }

        [HttpGet("byReferralCode/{referralCode}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<RetailerDto>> GetRetailerByReferralCode(string referralCode)
        {
            RetailerDto retailer = await this._retailerRepository.GetRetailerByReferralCode(referralCode);
            return retailer != null ? Ok(retailer) : NotFound();
        }

        [HttpPut("{retailerPartnerId}")]
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
        [HttpGet("{partnerCode}/{partnerProvidedRetailerId}/me")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.Retailer)]
        public async Task<ActionResult<RetailerDto>> GetSelfRetailer(string partnerCode, string partnerProvidedRetailerId)
        {
            Guid callerUserId = UserClaimData.GetUserId(this.User.Claims);
            string partnerId = RetailerDto.CreatePartnerId(partnerCode, partnerProvidedRetailerId);

            RetailerDto retailer = await this._retailerRepository.GetRetailerByPartnerId(partnerId);

            // for ME, we also match the caller details
            if (retailer != null && retailer.UserId == callerUserId)
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
                RetailerId = Guid.NewGuid(),
                StartDate = DateTime.MinValue,
                UserId = Guid.Empty,
                ReferralCode = null,
            };

            await _retailerRepository.CreateRetailer(retailerToCreate);

            return Ok(retailerToCreate.RetailerId);
        }

        #endregion
    }
}
