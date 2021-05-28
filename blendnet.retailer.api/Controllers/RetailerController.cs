﻿using blendnet.common.dto;
using blendnet.common.dto.Retailer;
using blendnet.common.dto.User;
using blendnet.common.infrastructure.Authentication;
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
    [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
    public class RetailerController : ControllerBase
    {
        private readonly ILogger _logger;

        private IRetailerRepository _retailerRepository;

        IStringLocalizer<SharedResource> _stringLocalizer;

        public RetailerController(ILogger<RetailerController> logger, IRetailerRepository retailerRepository, IStringLocalizer<SharedResource> stringLocalizer)
        {
            this._logger = logger;
            this._retailerRepository = retailerRepository;
            _stringLocalizer = stringLocalizer;
        }

        #region API methods

        [HttpGet("byPartnerId/{retailerPartnerId}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<RetailerDto>> GetRetailerByPartnerId(string retailerPartnerId /* composed */)
        {
            RetailerDto retailer = await this._retailerRepository.GetRetailerByPartnerId(retailerPartnerId);
            return retailer != null ? Ok(retailer) : NotFound();
        }

        [HttpGet("byReferralCode/{referralCode}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<RetailerDto>> GetRetailerByReferralCode(string referralCode)
        {
            RetailerDto retailer = await this._retailerRepository.GetRetailerByReferralCode(referralCode);
            return retailer != null ? Ok(retailer) : NotFound();
        }

        [HttpPut("{retailerPartnerId}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult<string>> UpdateRetailer(string retailerPartnerId /* composed */,
                                                                RetailerDto retailer)
        {
            var existingRetailer = await this._retailerRepository.GetRetailerByPartnerId(retailerPartnerId);
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

        #endregion
    }
}
