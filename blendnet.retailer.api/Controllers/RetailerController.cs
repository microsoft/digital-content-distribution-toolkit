using blendnet.common.dto;
using blendnet.common.dto.Retailer;
using blendnet.retailer.api.Models;
using blendnet.retailer.repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class RetailerController : ControllerBase
    {
        private readonly ILogger _logger;

        private IRetailerRepository _retailerRepository;

        public RetailerController(ILogger<RetailerController> logger, IRetailerRepository retailerRepository)
        {
            this._logger = logger;
            this._retailerRepository = retailerRepository;
        }

        #region API methods

        [HttpPost]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<string>> CreateRetailer(CreateRetailerRequest retailerRequest)
        {
            var partnerCode = GetPartnerCodeFromRequest();

            var listOfValidationErrors = new List<string>();

            // validations
            {
                string phoneNumber = retailerRequest.PhoneNumber;
                if (phoneNumber.Length != 10
                    || phoneNumber.StartsWith("+")
                    || !int.TryParse(phoneNumber, out _)
                    )
                {
                    listOfValidationErrors.Add("Invalid Phone number format");
                }

                if (!retailerRequest.Address.MapLocation.isValid())
                {
                    listOfValidationErrors.Add("Map Location is not valid");
                }

                string retailerPartnerId = RetailerDto.CreatePartnerId(partnerCode, retailerRequest.RetailerId);
                RetailerDto existingRetailer = await this._retailerRepository.GetRetailerByPartnerId(retailerPartnerId);
                if (existingRetailer != null)
                {
                    return Conflict("Already Exists");
                }
            }

            // check and return validation errors
            if (listOfValidationErrors.Count > 0)
            {
                return BadRequest(listOfValidationErrors);
            }

            DateTime now = DateTime.UtcNow;

            // create RetailerDto from request
            RetailerDto retailer = new RetailerDto()
            {
                // Base propeties
                CreatedByUserId = retailerRequest.UserId, // TODO: this should be from Claim
                CreatedDate = now,

                // Person Properties
                Id = retailerRequest.UserId,
                PhoneNumber = retailerRequest.PhoneNumber,
                UserName = retailerRequest.Name,

                // User properties
                // Retailer properties
                PartnerProvidedId = retailerRequest.RetailerId,
                PartnerCode = partnerCode, // TODO: extract from Claim
                Address = retailerRequest.Address,
                Services = new List<ServiceType>() { ServiceType.Media },
                AdditionalAttibutes = retailerRequest.AdditionalAttributes,

                StartDate = now,
                EndDate = DateTime.MaxValue,
            };

            // create retailer in DB
            string retailerId = await this._retailerRepository.CreateRetailer(retailer);

            return Ok(retailerId);
        }

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
                listOfValidationErrors.Add("Invalid Address");
            }

            if (retailer.StartDate > retailer.EndDate)
            {
                listOfValidationErrors.Add("Invalid Start and End Dates");
            }

            if (listOfValidationErrors.Count > 0)
            {
                return BadRequest(listOfValidationErrors);
            }

            // update the metadata
            retailer.ModifiedDate = DateTime.UtcNow;
            retailer.ModifiedByByUserId = retailer.Id; // TODO: update with auth integration

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

        private string GetPartnerCodeFromRequest()
        {
            // TODO: implement
            return ApplicationConstants.PartnerCode.NovoPay;
        }
    }
}
