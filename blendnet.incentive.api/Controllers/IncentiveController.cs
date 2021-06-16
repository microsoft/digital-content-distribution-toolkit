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
        [HttpPost("retailerincentiveplan", Name = nameof(CreateRetailerIncentivePlan))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult> CreateRetailerIncentivePlan(IncentivePlanRequest incentivePlanRequest)
        {
            List<string> errorInfo = new List<string>();
            RetailerProviderDto retailerProviderDto = null;

            if (incentivePlanRequest.Audience.AudienceType != AudienceType.RETAILER)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0018"]);
                return BadRequest(errorInfo);
            }

            retailerProviderDto = await _retailerProviderProxy.GetRetailerProviderByPartnerCode(incentivePlanRequest.Audience.SubTypeName);
            errorInfo = await ValidatePlan(null, incentivePlanRequest, retailerProviderDto);
            
            if(HasError(errorInfo))
            {
                return BadRequest(errorInfo);
            }

            IncentivePlan incentivePlan = CreatePlan(incentivePlanRequest, retailerProviderDto);

            Guid id = await _incentiveRepository.CreateIncentivePlan(incentivePlan);

            return Ok(id);
        }

        /// <summary>
        /// Create incentivePlan by a consumer
        /// </summary>
        /// <param name="incentivePlanRequest"></param>
        /// <returns></returns>
        [HttpPost("consumerincentiveplan", Name = nameof(CreateConsumerIncentivePlan))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult> CreateConsumerIncentivePlan(IncentivePlanRequest incentivePlanRequest)
        {
            List<string> errorInfo = new List<string>();
            if (incentivePlanRequest.Audience.AudienceType != AudienceType.CONSUMER)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0018"]);
                return BadRequest(errorInfo);
            }

            errorInfo = await ValidatePlan(null, incentivePlanRequest, null);

            if (HasError(errorInfo))
            {
                return BadRequest(errorInfo);
            }

            IncentivePlan incentivePlan = CreatePlan(incentivePlanRequest, null);

            Guid id = await _incentiveRepository.CreateIncentivePlan(incentivePlan);

            return Ok(id);
        }

        [HttpPut("{planId:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> UpdateIncentivePlan(Guid planId, IncentivePlanRequest updatePlanRequest)
        {
            IncentivePlan plan = await _incentiveRepository.GetPlan(planId, updatePlanRequest.Audience.SubTypeName);

            if (plan == null)
            {
                return NotFound();
            }

            List<string> errorInfo = new List<string>();

            // Only draft mode plans can be updated
            if (plan.PublishMode != PublishMode.DRAFT)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0012"]);
                return BadRequest(errorInfo);
            }

            RetailerProviderDto retailerProviderDto = null;

            if (updatePlanRequest.Audience.AudienceType == AudienceType.RETAILER)
            {
                retailerProviderDto = await _retailerProviderProxy.GetRetailerProviderByPartnerCode(updatePlanRequest.Audience.SubTypeName);
            }

            errorInfo = await ValidatePlan(planId, updatePlanRequest, retailerProviderDto);

            if (HasError(errorInfo))
            {
                return BadRequest(errorInfo);
            }

            plan.PlanName = updatePlanRequest.PlanName;
            plan.PlanType = updatePlanRequest.PlanType;
            plan.StartDate = updatePlanRequest.StartDate;
            plan.EndDate = updatePlanRequest.EndDate;
            plan.Audience = updatePlanRequest.Audience;
            plan.PlanDetails = updatePlanRequest.PlanDetails;

            int statusCode = await _incentiveRepository.UpdateIncentivePlan(plan);

            if (statusCode == (int)System.Net.HttpStatusCode.OK)
            {
                return NoContent();
            }

            errorInfo.Add(_stringLocalizer["INC_ERR_0013"]);

            return BadRequest(errorInfo);

        }

        [HttpDelete("{planId:guid}/{subtypeName}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<ActionResult> DeleteIncentivePlan(Guid planId, string subtypeName)
        {
            IncentivePlan plan = await _incentiveRepository.GetPlan(planId, subtypeName);

            if (plan == null)
            {
                return NotFound();
            }

            List<string> errorInfo = new List<string>();

            if (plan.PublishMode != PublishMode.DRAFT)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0014"]);
                return BadRequest(errorInfo);
            }

            int statusCode = await _incentiveRepository.DeleteIncentivePlan(planId, subtypeName);

            if (statusCode == (int)System.Net.HttpStatusCode.OK)
            {
                return NoContent();
            }

            return NotFound();
        }

        [HttpPut("publish/{planId:guid}/{subtypeName}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<ActionResult> PublishPlan(Guid planId, string subtypeName)
        {
            IncentivePlan plan = await _incentiveRepository.GetPlan(planId, subtypeName);

            if (plan == null)
            {
                return NotFound();
            }

            List<string> errorInfo = new List<string>();

            if (plan.PublishMode != PublishMode.DRAFT)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0014"]);
                return BadRequest(errorInfo);
            }

            errorInfo = ValidateDate(plan);
           
            if (HasError(errorInfo))
            {
                return BadRequest(errorInfo);
            }

            IncentivePlan currentActivePlan = null;
            DateTime startDate = plan.StartDate.Date;

            if (plan.Audience.AudienceType == AudienceType.RETAILER)
            {
                currentActivePlan = await _incentiveRepository.GetCurrentRetailerPublishedPlan(plan.PlanType, plan.Audience.SubTypeName, startDate);
            }
            else
            {
                currentActivePlan = await _incentiveRepository.GetCurrentConsumerPublishedPlan(plan.PlanType, startDate);
            }

            int statusCode;
            if (currentActivePlan != null && currentActivePlan.EndDate > plan.StartDate)
            {
                
                currentActivePlan.EndDate = startDate.AddSeconds(-1); // sets date to previous date 11:59:59 pm
                statusCode = await _incentiveRepository.UpdateIncentivePlan(currentActivePlan);

                if (statusCode != (int)System.Net.HttpStatusCode.OK)
                {
                    errorInfo.Add(_stringLocalizer["INC_ERR_0016"]);
                    return BadRequest(errorInfo);
                }
            }

            plan.StartDate = startDate; // setting startdate to 12:00 AM
            plan.PublishMode = PublishMode.PUBLISHED;

            statusCode = await _incentiveRepository.UpdateIncentivePlan(plan);

            if (statusCode != (int)System.Net.HttpStatusCode.OK)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0016"]);
                return BadRequest(errorInfo);
            }

            return NoContent();
        }

        [HttpGet("{planId:guid}/{subtypeName}", Name = nameof(GetIncentivePlan))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<IncentivePlan>> GetIncentivePlan(Guid planId, string subtypeName)
        {
            IncentivePlan plan = await _incentiveRepository.GetPlan(planId, subtypeName);

            if (plan == null)
            {
                return NotFound();
            }

            return Ok(plan);
        }


        #endregion

        #region private methods

        private async Task<List<string>> ValidatePlan(Guid? planId, IncentivePlanRequest incentivePlanRequest, RetailerProviderDto retailerProviderDto)
        {
            List<string> errorInfo = new List<string>();

            // check if any other plan exists and is not same as current plan
            IncentivePlan currentDraftPlan = null;

            if (incentivePlanRequest.Audience.AudienceType == AudienceType.RETAILER)
            {
                currentDraftPlan = await _incentiveRepository.GetCurrentRetailerDraftPlan(incentivePlanRequest.PlanType, incentivePlanRequest.Audience.SubTypeName);
            }
            else
            {
                currentDraftPlan = await _incentiveRepository.GetCurrentConsumerDraftPlan(incentivePlanRequest.PlanType);
            }

            if(currentDraftPlan != null && !currentDraftPlan.Id.Equals(planId))
            {
                
                errorInfo.Add(_stringLocalizer["INC_ERR_0017"]);
                return errorInfo;
            }

            //Validate PlanDetail
            errorInfo = ValidatePlanDetail(incentivePlanRequest);

            if (HasError(errorInfo))
            {
                return errorInfo;
            }

            if (incentivePlanRequest.Audience.AudienceType == AudienceType.RETAILER)
            {
                //Get retailer provider info if it is a retailer plan
                
                if (retailerProviderDto == null)
                {
                    errorInfo.Add(_stringLocalizer["INC_ERR_0002"]);
                    return errorInfo;
                }
            }

            return errorInfo;
        }

        private bool HasError(List<string> errorInfo)
        {
            return errorInfo != null && errorInfo.Count > 0;
        }

        private List<string> ValidateDate(IncentivePlan incentivePlanRequest)
        {
            DateTime curDate = DateTime.UtcNow;

            List<string> errorInfo = new List<string>();
            if (incentivePlanRequest.StartDate > incentivePlanRequest.EndDate)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0001"]);
            }

            if (incentivePlanRequest.StartDate < curDate)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0011"]);
            }

            return errorInfo;
        }

        private List<string> ValidatePlanDetail(IncentivePlanRequest incentivePlanRequest)
        {
            List<PlanDetail> planDetails = incentivePlanRequest.PlanDetails;

            List<string> errorInfo = new List<string>();

            if (planDetails.Count == 0)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0003"]);
                return errorInfo;
            }

            AudienceType audienceType = incentivePlanRequest.Audience.AudienceType;

            HashSet<PlanDetail> processed = new HashSet<PlanDetail>(new PlanDetailComparer());

            foreach (PlanDetail planDetail in planDetails)
            {
                if (processed.Contains(planDetail))
                {
                    errorInfo.Add(string.Format(_stringLocalizer["INC_ERR_0005"], planDetail.EventType));
                    return errorInfo;
                }

                if ((planDetail.EventType == EventType.RETAILER_INCOME_ORDER_COMPLETED || planDetail.EventType == EventType.CONSUMER_INCOME_ORDER_COMPLETED) && string.IsNullOrEmpty(planDetail.EventSubType))
                {
                    errorInfo.Add(_stringLocalizer["INC_ERR_0010"]);
                    return errorInfo;
                }

                if (!IsEventForAudience(audienceType, planDetail.EventType))
                {
                    errorInfo.Add(string.Format(_stringLocalizer["INC_ERR_0004"], planDetail.EventType.ToString(), audienceType));
                    return errorInfo;
                }

                if (!IsRuleTypeValid(planDetail.RuleType, incentivePlanRequest.PlanType))
                {
                    errorInfo.Add(string.Format(_stringLocalizer["INC_ERR_0008"], planDetail.EventType.ToString(), audienceType));
                    return errorInfo;
                }

                if (!IsFormulaTypeValid(planDetail.Formula.FormulaType, incentivePlanRequest.PlanType))
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
                incentivePlan.Audience.SubTypeName = ApplicationConstants.Common.CONSUMER;
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
            if (audienceType == AudienceType.CONSUMER)
            {
                return eventSubType.ToString().StartsWith(C_CONSUMER);
            }

            return eventSubType.ToString().StartsWith(C_RETAILER);
        }

        private bool IsFormulaTypeValid(FormulaType formulaType, PlanType planType)
        {
            if (planType == PlanType.REGULAR)
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
