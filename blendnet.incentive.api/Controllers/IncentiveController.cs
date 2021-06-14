using blendnet.api.proxy.Retailer;
using blendnet.common.dto;
using blendnet.common.dto.Incentive;
using blendnet.common.dto.Retailer;
using blendnet.common.dto.User;
using blendnet.common.infrastructure.Authentication;
using blendnet.incentive.api.Common;
using blendnet.incentive.api.Model;
using blendnet.incentive.repository.Interfaces;
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
    //[AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin)]
    public class IncentiveController : ControllerBase
    {
        private const string C_CONSUMER = "CONSUMER";

        private const string C_RETAILER = "RETAILER";

        private readonly ILogger _logger;

        private IIncentiveRepository _incentiveRepository;

        private IncentiveAppSettings _incentiveAppSettings;

        IStringLocalizer<SharedResource> _stringLocalizer;

        private RetailerProviderProxy _retailerProviderProxy;

        private IncentiveCalculationHelper _incentiveCalculationHelper;

        public IncentiveController(IIncentiveRepository incentiveRepository,
                                ILogger<IncentiveController> logger,
                                IOptionsMonitor<IncentiveAppSettings> optionsMonitor,
                                IStringLocalizer<SharedResource> stringLocalizer,
                                RetailerProviderProxy retailerProviderProxy,
                                IncentiveCalculationHelper incentiveCalculationHelper)
        {
            _incentiveRepository = incentiveRepository;

            _logger = logger;

            _incentiveAppSettings = optionsMonitor.CurrentValue;

            _stringLocalizer = stringLocalizer;

            _retailerProviderProxy = retailerProviderProxy;

            _incentiveCalculationHelper = incentiveCalculationHelper;
        }

        #region Incentive management methods

        /// <summary>
        /// Create incentivePlan by a retailer
        /// </summary>
        /// <param name="incentivePlanRequest"></param>
        /// <returns></returns>
        [HttpPost("incentiveplan", Name = nameof(CreateIncentivePlan))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult> CreateIncentivePlan(IncentivePlanRequest incentivePlanRequest)
        {
            List<string> errorInfo;
            // Validate date

            errorInfo = await ValidateDate(incentivePlanRequest);

            if(HasError(errorInfo))
            {
                return BadRequest(errorInfo);
            }

            //Validate PlanDetail

            errorInfo = ValidatePlanDetail(incentivePlanRequest);

            if(HasError(errorInfo))
            {
                return BadRequest(errorInfo);
            }

            //Get retailer provider info if it is a retailer plan
            RetailerProviderDto retailerProviderDto = null;

            if(incentivePlanRequest.Audience.AudienceType == AudienceType.RETAILER)
            {
                retailerProviderDto = await _retailerProviderProxy.GetRetailerProviderByPartnerCode(incentivePlanRequest.Audience.SubTypeName);

                if(retailerProviderDto == null)
                {
                    errorInfo.Add(_stringLocalizer["INC_ERR_0002"]);
                    return BadRequest(errorInfo);
                }
            }

            IncentivePlan incentivePlan = CreatePlan(incentivePlanRequest, retailerProviderDto);

            Guid id = await _incentiveRepository.CreateIncentivePlan(incentivePlan);

            return Ok(id);
        }

        [HttpGet("{planId:guid}/{subtypeName}", Name = nameof(GetIncentivePlan))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<IncentivePlan>> GetIncentivePlan(Guid planId, string subtypeName)
        {
            IncentivePlan plan = await _incentiveRepository.GetPlan(planId, subtypeName);

            if(plan == null)
            {
                return NotFound();
            }

            return Ok(plan);
        }


        #endregion

        #region private methods

        private bool HasError(List<string> errorInfo)
        {
            return errorInfo != null && errorInfo.Count > 0;
        }

        private async Task<List<string>> ValidateDate(IncentivePlanRequest incentivePlanRequest)
        {
            List<string> errorInfo = new List<string>();
            if(incentivePlanRequest.StartDate > incentivePlanRequest.EndDate)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0001"]);
            }

            // check if active plan exists for given plan type and audience type

            IncentivePlan activePlan;

            if(incentivePlanRequest.Audience.AudienceType == AudienceType.CONSUMER)
            {
                activePlan = await _incentiveRepository.GetCurrentConsumerActivePlan(incentivePlanRequest.PlanType);
            }
            else
            {
                activePlan = await _incentiveRepository.GetCurrentRetailerActivePlan(incentivePlanRequest.PlanType, incentivePlanRequest.Audience.SubTypeName);
            }

            if (activePlan != null)
            {
                if (incentivePlanRequest.StartDate < activePlan.EndDate)
                {
                    errorInfo.Add(_stringLocalizer["INC_ERR_0007"]);
                }
            }
            

            //if(activePlan != null)
            //{
            //    errorInfo.Add(_stringLocalizer["INC_ERR_0006"]);
            //}
            //else if(activePlans.Count == 1)
            //{
            //    IncentivePlan currentPlan = activePlans[0];

            //    if(incentivePlanRequest.StartDate < currentPlan.EndDate)
            //    {
            //        errorInfo.Add(_stringLocalizer["INC_ERR_0007"]);
            //    }
            //}

            return errorInfo;
        }

        private List<string> ValidatePlanDetail(IncentivePlanRequest incentivePlanRequest)
        {
            List<PlanDetail> planDetails = incentivePlanRequest.PlanDetails;

            List<string> errorInfo = new List<string>();

            if(planDetails.Count == 0)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0003"]);
                return errorInfo;
            }

            AudienceType audienceType = incentivePlanRequest.Audience.AudienceType;

            HashSet<PlanDetail> processed = new HashSet<PlanDetail>(new PlanDetailComparer());

            foreach(PlanDetail planDetail in planDetails)
            {
                if(processed.Contains(planDetail))
                {
                    errorInfo.Add(string.Format(_stringLocalizer["INC_ERR_0005"], planDetail.EventType));
                    return errorInfo;
                }

                if((planDetail.EventType == EventType.RETAILER_INCOME_ORDER_COMPLETED || planDetail.EventType == EventType.CONSUMER_INCOME_ORDER_COMPLETED) && string.IsNullOrEmpty(planDetail.EventSubType))
                {
                    errorInfo.Add(_stringLocalizer["INC_ERR_0010"]);
                    return errorInfo;
                }

                if (!IsEventForAudience(audienceType, planDetail.EventType))
                {
                    errorInfo.Add(string.Format(_stringLocalizer["INC_ERR_0004"], planDetail.EventType.ToString(), audienceType));
                    return errorInfo;
                }

                if(!IsRuleTypeValid(planDetail.RuleType, incentivePlanRequest.PlanType))
                {
                    errorInfo.Add(string.Format(_stringLocalizer["INC_ERR_0008"], planDetail.EventType.ToString(), audienceType));
                    return errorInfo;
                }

                if(!IsFormulaTypeValid(planDetail.Formula.FormulaType, incentivePlanRequest.PlanType))
                {
                    errorInfo.Add(string.Format(_stringLocalizer["INC_ERR_0009"], planDetail.EventType.ToString(), audienceType));
                    return errorInfo;
                }

                planDetail.DetailId = Guid.NewGuid();

                processed.Add(planDetail);
            }

            return errorInfo;

        }

        

        private IncentivePlan CreatePlan(IncentivePlanRequest incentivePlanRequest, RetailerProviderDto retailerProviderDto)
        {
            IncentivePlan incentivePlan = new IncentivePlan();
            incentivePlan.Id = Guid.NewGuid();
            incentivePlan.PlanName = incentivePlanRequest.PlanName;
            incentivePlan.PlanType = incentivePlanRequest.PlanType;
            incentivePlan.StartDate = incentivePlanRequest.StartDate;
            incentivePlan.EndDate = incentivePlanRequest.EndDate;

            incentivePlan.Audience = new Audience();
            incentivePlan.Audience.AudienceType = incentivePlanRequest.Audience.AudienceType;

            if (incentivePlan.Audience.AudienceType == AudienceType.CONSUMER)
            {
                //incentivePlan.Audience.SubTypeName = Common.CONSUMER;
            }
            else
            {
                incentivePlan.Audience.SubTypeName = retailerProviderDto.PartnerCode;
            }

            incentivePlan.PlanDetails = incentivePlanRequest.PlanDetails;

            incentivePlan.CreatedByUserId = UserClaimData.GetUserId(User.Claims);
            incentivePlan.CreatedDate = DateTime.UtcNow;

            return incentivePlan;
        }

        private bool IsEventForAudience(AudienceType audienceType, EventType eventSubType)
        {
            if(audienceType == AudienceType.CONSUMER)
            {
                return eventSubType.ToString().StartsWith(C_CONSUMER);
            }

            return eventSubType.ToString().StartsWith(C_RETAILER);
        }

        private bool IsFormulaTypeValid(FormulaType formulaType, PlanType planType)
        {
            if(planType == PlanType.REGULAR)
            {
                return formulaType == FormulaType.PLUS 
                    || formulaType == FormulaType.MINUS 
                    || formulaType == FormulaType.MULTIPLY 
                    || formulaType == FormulaType.PERCENTAGE;
            }

            return true;
        }

        private bool IsRuleTypeValid(RuleType ruleType, PlanType planType)
        {
            if (planType == PlanType.REGULAR)
            {
                return ruleType == RuleType.SUM;
            }

            return true;
        }

        #endregion
    }
}
