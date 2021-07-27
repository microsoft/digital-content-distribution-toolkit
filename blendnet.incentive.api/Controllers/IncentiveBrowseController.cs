using AutoMapper;
using blendnet.api.proxy.Retailer;
using blendnet.common.dto;
using blendnet.common.dto.Incentive;
using blendnet.common.dto.Retailer;
using blendnet.common.dto.User;
using blendnet.common.infrastructure.Authentication;
using blendnet.incentive.api.Model;
using blendnet.incentive.repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static blendnet.common.dto.ApplicationConstants;

namespace blendnet.incentive.api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin, KaizalaIdentityRoles.User, KaizalaIdentityRoles.Retailer)]
    public class IncentiveBrowseController : ControllerBase
    {
        private IMapper _mapper;

        private readonly ILogger _logger;

        private IIncentiveRepository _incentiveRepository;

        private IncentiveAppSettings _incentiveAppSettings;

        IStringLocalizer<SharedResource> _stringLocalizer;

        private RetailerProxy _retailerProxy;

        public IncentiveBrowseController(IIncentiveRepository incentiveRepository,
                                ILogger<IncentiveController> logger,
                                IMapper mapper,
                                RetailerProxy retailerProxy,
                                IOptionsMonitor<IncentiveAppSettings> optionsMonitor,
                                IStringLocalizer<SharedResource> stringLocalizer)
        {
            _incentiveRepository = incentiveRepository;

            _logger = logger;

            _mapper = mapper;

            _retailerProxy = retailerProxy;

            _incentiveAppSettings = optionsMonitor.CurrentValue;

            _stringLocalizer = stringLocalizer;

        }

        #region Incentive browse apis

        /// <summary>
        /// Returns all consumer incentive plans with given plan type
        /// </summary>
        /// <param name="planType"></param>
        /// <returns></returns>
        [HttpGet("consumer/active/{planType}", Name = nameof(GetConsumerActivePlan))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin, KaizalaIdentityRoles.User)]
        public async Task<ActionResult<IncentivePlanDto>> GetConsumerActivePlan(PlanType planType)
        {
            IncentivePlan incentivePlan = await _incentiveRepository.GetCurrentConsumerActivePlan(planType);

            if (incentivePlan == null)
            {
                return NotFound();
            }

            IncentivePlanDto incentivePlanDto = _mapper.Map<IncentivePlan, IncentivePlanDto>(incentivePlan);

            return Ok(incentivePlanDto);
        }

        /// <summary>
        /// Returns retailer current active incentive plan with given plan type
        /// </summary>
        /// <param name="planType"></param>
        /// <returns></returns>
        [HttpGet("retailer/active/{retailerPartnerProvidedId}/{planType}/{subtypeName}", Name = nameof(GetRetailerActivePlan))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin, KaizalaIdentityRoles.Retailer)]
        public async Task<ActionResult<IncentivePlanDto>> GetRetailerActivePlan(string retailerPartnerProvidedId, PlanType planType, string subtypeName)
        {
            RetailerDto retailer = await _retailerProxy.GetRetailerById(retailerPartnerProvidedId, subtypeName);

            List<string> errorInfo = ValidateRetailer(retailer);

            if (errorInfo.Count > 0)
            {
                return BadRequest(errorInfo);
            }

            IncentivePlan incentivePlan = await _incentiveRepository.GetCurrentRetailerActivePlan(planType, subtypeName);

            if (incentivePlan == null)
            {
                return NotFound();
            }

            IncentivePlanDto incentivePlanDto = _mapper.Map<IncentivePlan, IncentivePlanDto>(incentivePlan);

            return Ok(incentivePlanDto);
        }

        /// <summary>
        /// Returns retailer incentive plans with given plan type
        /// </summary>
        /// <param name="planType"></param>
        /// <returns></returns>
        [HttpGet("retailer/all/{retailerPartnerProvidedId}/{planType}/{subtypeName}", Name = nameof(GetRetailerAllIncentivePlans))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin, KaizalaIdentityRoles.Retailer)]
        public async Task<ActionResult<List<IncentivePlanDto>>> GetRetailerAllIncentivePlans(string retailerPartnerProvidedId, PlanType planType, string subtypeName)
        {
            RetailerDto retailer = await _retailerProxy.GetRetailerById(retailerPartnerProvidedId, subtypeName);

            List<string> errorInfo = ValidateRetailer(retailer);

            if (errorInfo.Count > 0)
            {
                return BadRequest(errorInfo);
            }

            Audience audience = new Audience()
            {
                AudienceType = AudienceType.RETAILER,
                SubTypeName = subtypeName
            };

            List<IncentivePlan> incentivePlans = await _incentiveRepository.GetIncentivePlans(audience, planType);

            if (incentivePlans == null || incentivePlans.Count == 0)
            {
                return NotFound();
            }

            List<IncentivePlanDto> incentivePlanDto = _mapper.Map<List<IncentivePlan>, List<IncentivePlanDto>>(incentivePlans);

            return Ok(incentivePlanDto);
        }

        #endregion

        #region private methods
        /// <summary>
        /// Checks if retailer exists and if retailer id is same as current user id
        /// </summary>
        /// <param name="retailer"></param>
        /// <param name="retailerPartnerProvidedId"></param>
        /// <param name="partnerCode"></param>
        /// <returns></returns>
        private List<string> ValidateRetailer(RetailerDto retailer)
        {
            List<string> errorInfo = new List<string>();

            if (retailer == null)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0025"]);
                return errorInfo;
            }

            if (User.IsInRole(KaizalaIdentityRoles.SuperAdmin))
            {
                // Retailer specific validation is not required for superadmin
                return errorInfo;
            }

            Guid userId = UserClaimData.GetUserId(User.Claims);

            if (!userId.Equals(retailer.UserId))
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0026"]);
                return errorInfo;
            }

            return errorInfo;
        }

        #endregion
    }
}
