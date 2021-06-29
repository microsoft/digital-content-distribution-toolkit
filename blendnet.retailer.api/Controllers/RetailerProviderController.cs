using blendnet.common.dto;
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
    /// <summary>
    /// Controller class for managing Retailer Providers (Partners)
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)]
    public class RetailerProviderController : ControllerBase
    {
        private readonly ILogger _logger;

        private IRetailerProviderRepository _retailerProviderRepository;

        private IStringLocalizer<SharedResource> _stringLocalizer;

        public RetailerProviderController(ILogger<RetailerProviderController> logger, IRetailerProviderRepository retailerProviderRepository, IStringLocalizer<SharedResource> stringLocalizer)
        {
            this._logger = logger;
            this._retailerProviderRepository = retailerProviderRepository;
            _stringLocalizer = stringLocalizer;
        }

        #region API methods

        /// <summary>
        /// Creates a new retailer provider
        /// </summary>
        /// <param name="retailerProvider"></param>
        /// <returns>Service Account ID of the retaielr provider</returns>
        [HttpPost("create")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<Guid>> CreateRetailerProvider(RetailerProviderDto retailerProvider)
        {
            // Validations
            {
                List<string> listOfValidationErrors = new List<string>();
                // check partner code should not exist
                RetailerProviderDto byPartnerCode = await _retailerProviderRepository.GetRetailerProviderByPartnerCode(retailerProvider.PartnerCode);
                if (byPartnerCode != null)
                {
                    listOfValidationErrors.Add(string.Format(_stringLocalizer["RMS_ERR_0003"], retailerProvider.PartnerCode));
                }

                // check account ID should not exist
                RetailerProviderDto byServiceAccountId = await _retailerProviderRepository.GetRetailerProviderByServiceAccountId(retailerProvider.ServiceAccountId);
                if (byServiceAccountId != null)
                {
                    listOfValidationErrors.Add(string.Format(_stringLocalizer["RMS_ERR_0004"], retailerProvider.ServiceAccountId));
                }

                if (listOfValidationErrors.Count > 0)
                {
                    return BadRequest(listOfValidationErrors);
                }
            }

            Guid callerUserId = UserClaimData.GetUserId(User.Claims);

            DateTime now = DateTime.UtcNow;

            // update creation params
            retailerProvider.CreatedDate = now;
            retailerProvider.CreatedByUserId = callerUserId;
            retailerProvider.StartDate = now;
            retailerProvider.EndDate = DateTime.MaxValue;

            // create the retailer
            Guid result = await _retailerProviderRepository.CreateRetailerProvider(retailerProvider);
            return Ok(result);
        }

        /// <summary>
        /// Gets Retailer Provider by Service Account ID
        /// </summary>
        /// <param name="serviceAccountId"></param>
        /// <returns></returns>
        [HttpGet("byServiceAccountId/{serviceAccountId:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<RetailerProviderDto>> GetRetailerProviderByServiceAccountId(Guid serviceAccountId)
        {
            RetailerProviderDto retailerProvider = await _retailerProviderRepository.GetRetailerProviderByServiceAccountId(serviceAccountId);
            return retailerProvider == null ? NotFound() : Ok(retailerProvider);
        }

        /// <summary>
        /// Gets Retailer Provider by Partner Code
        /// </summary>
        /// <param name="partnerCode"></param>
        /// <returns></returns>
        [HttpGet("byPartnerCode/{partnerCode}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<RetailerProviderDto>> GetRetailerProviderByPartnerCode(string partnerCode)
        {
            RetailerProviderDto retailerProvider = await _retailerProviderRepository.GetRetailerProviderByPartnerCode(partnerCode);
            return retailerProvider == null ? NotFound() : Ok(retailerProvider);
        }

        /// <summary>
        /// Gets list of all Retailer Providers
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<RetailerProviderDto>> GetAllRetailerProviders()
        {
            var retailerProviders = await _retailerProviderRepository.GetAllRetailerProviders();
            return retailerProviders == null || retailerProviders.Count == 0 ? NotFound() : Ok(retailerProviders);
        }

        #endregion
        
        #region Private methods

        #endregion
    }
}
