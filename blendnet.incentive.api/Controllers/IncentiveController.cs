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
    [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin)]
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
        [HttpPost("retailer", Name = nameof(CreateRetailerIncentivePlan))]
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

            if (HasError(errorInfo))
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
        [HttpPost("consumer", Name = nameof(CreateConsumerIncentivePlan))]
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

        /// <summary>
        /// Updates existing draft plan with given plan id and other details
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="updatePlanRequest"></param>
        /// <returns></returns>
        [HttpPut("retailer/{planId:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> UpdateRetailerIncentivePlan(Guid planId, IncentivePlanRequest updatePlanRequest)
        {
            IncentivePlan plan = await _incentiveRepository.GetPlan(planId, updatePlanRequest.Audience.SubTypeName);
            return await UpdatePlan(planId, updatePlanRequest, plan);
        }

        /// <summary>
        /// Updates existing draft plan with given plan id and other details
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="updatePlanRequest"></param>
        /// <returns></returns>
        [HttpPut("consumer/{planId:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> UpdateConsumerIncentivePlan(Guid planId, IncentivePlanRequest updatePlanRequest)
        {
            IncentivePlan plan = await _incentiveRepository.GetPlan(planId, updatePlanRequest.Audience.SubTypeName);
            return await UpdatePlan(planId, updatePlanRequest, plan);

        }

        /// <summary>
        /// Deletes given plan id if it is in draft mode
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="subtypeName"></param>
        /// <returns></returns>
        [HttpDelete("retailer/{planId:guid}/{subtypeName}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<ActionResult> DeleteRetailerIncentivePlan(Guid planId, string subtypeName)
        {
            IncentivePlan plan = await _incentiveRepository.GetPlan(planId, subtypeName);

            return await DeleteIncentivePlan(plan);
        }

        /// <summary>
        /// Deletes given plan id if it is in draft mode
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="subtypeName"></param>
        /// <returns></returns>
        [HttpDelete("consumer/{planId:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<ActionResult> DeleteConsumerIncentivePlan(Guid planId)
        {
            IncentivePlan plan = await _incentiveRepository.GetPlan(planId, ApplicationConstants.Common.CONSUMER);
            return await DeleteIncentivePlan(plan);
        }

        /// <summary>
        /// Updates end date of published plan with given end date
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpPut("consumer/changeenddate/{planId:guid}/{endDate}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> CloseConsumerIncentivePlan(Guid planId, DateTime endDate)
        {
            IncentivePlan plan = await _incentiveRepository.GetPlan(planId, ApplicationConstants.Common.CONSUMER);

            if (plan == null)
            {
                return NotFound();
            }

            List<string> errorInfo = new List<string>();

            errorInfo = await ValidateClosePlan(plan, endDate);

            if(HasError(errorInfo))
            {
                return BadRequest(errorInfo);
            }

            plan.EndDate = endDate;

            int status = await _incentiveRepository.UpdateIncentivePlan(plan);

            if (status == (int)System.Net.HttpStatusCode.OK)
            {
                return NoContent();
            }

            errorInfo.Add(_stringLocalizer["INC_ERR_0023"]);

            return BadRequest(errorInfo);
        }

        /// <summary>
        /// Updates end date of published plan with given end date
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpPut("retailer/changeenddate/{planId:guid}/{subTypeName}/{endDate}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> CloseRetailerIncentivePlan(Guid planId, string subTypeName, DateTime endDate)
        {
            IncentivePlan plan = await _incentiveRepository.GetPlan(planId, subTypeName);

            if (plan == null)
            {
                return NotFound();
            }

            List<string> errorInfo = new List<string>();

            errorInfo = await ValidateClosePlan(plan, endDate);

            if (HasError(errorInfo))
            {
                return BadRequest(errorInfo);
            }

            plan.EndDate = endDate;

            int status = await _incentiveRepository.UpdateIncentivePlan(plan);

            if (status == (int)System.Net.HttpStatusCode.OK)
            {
                return NoContent();
            }

            errorInfo.Add(_stringLocalizer["INC_ERR_0023"]);

            return BadRequest(errorInfo);
        }

        /// <summary>
        /// Publishes retailer incentive plan which was in draft mode if date validations succeeed
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="subtypeName"></param>
        /// <returns></returns>
        [HttpPut("retailer/publish/{planId:guid}/{subtypeName}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> PublishRetailerPlan(Guid planId, string subtypeName)
        {
            IncentivePlan plan = await _incentiveRepository.GetPlan(planId, subtypeName);

            var response = await PublishPlan(plan);

            return response;
        }

        /// <summary>
        /// Publishes consumer incentive plan which was in draft mode if date validations succeeed
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpPut("consumer/publish/{planId:guid}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> PublishConsumerPlan(Guid planId)
        {
            IncentivePlan plan = await _incentiveRepository.GetPlan(planId, ApplicationConstants.Common.CONSUMER);

            var response = await PublishPlan(plan);

            return response;

        }

        /// <summary>
        /// Returns incentive plan with given id and subtypename
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="subtypeName"></param>
        /// <returns></returns>
        [HttpGet("retailer/{planId:guid}/{subtypeName}", Name = nameof(GetRetailerIncentivePlan))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<IncentivePlan>> GetRetailerIncentivePlan(Guid planId, string subtypeName)
        {
            IncentivePlan plan = await _incentiveRepository.GetPlan(planId, subtypeName);

            if (plan == null)
            {
                return NotFound();
            }

            return Ok(plan);
        }

        /// <summary>
        /// Returns incentive plan with given id and subtypename
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet("consumer/{planId:guid}", Name = nameof(GetConsumerIncentivePlan))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<IncentivePlan>> GetConsumerIncentivePlan(Guid planId)
        {
            IncentivePlan plan = await _incentiveRepository.GetPlan(planId, ApplicationConstants.Common.CONSUMER);

            if (plan == null)
            {
                return NotFound();
            }

            return Ok(plan);
        }

        /// <summary>
        /// Returns all consumer incentive plans with given plan type
        /// </summary>
        /// <param name="planType"></param>
        /// <returns></returns>
        [HttpGet("consumer/{planType}", Name=nameof(GetConsumerIncentivePlans))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<IncentivePlan>>> GetConsumerIncentivePlans(PlanType planType)
        {
            Audience audience = new Audience()
            {
                AudienceType = AudienceType.CONSUMER,
                SubTypeName = ApplicationConstants.Common.CONSUMER
            };

            List<IncentivePlan> incentivePlans = await _incentiveRepository.GetIncentivePlans(audience, planType);

            if(incentivePlans == null || incentivePlans.Count == 0)
            {
                return NotFound();
            }

            return Ok(incentivePlans);
        }

        /// <summary>
        /// Returns all retailer incentive plans with given plan type and subtype name
        /// </summary>
        /// <param name="planType"></param>
        /// <param name="subTypeName"></param>
        /// <returns></returns>
        [HttpGet("retailer/{planType}/{subTypeName}", Name = nameof(GetRetailerIncentivePlans))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<IncentivePlan>>> GetRetailerIncentivePlans(PlanType planType, string subTypeName)
        {
            Audience audience = new Audience()
            {
                AudienceType = AudienceType.RETAILER,
                SubTypeName = subTypeName
            };

            List<IncentivePlan> incentivePlans = await _incentiveRepository.GetIncentivePlans(audience, planType);

            if (incentivePlans == null || incentivePlans.Count == 0)
            {
                return NotFound();
            }

            return Ok(incentivePlans);
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

            if (currentDraftPlan != null && !currentDraftPlan.Id.Equals(planId))
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

        private async Task<List<string>> ValidateDate(IncentivePlan incentivePlan)
        {
            DateTime curDate = DateTime.UtcNow;

            List<string> errorInfo = new List<string>();
            if (incentivePlan.StartDate > incentivePlan.EndDate)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0001"]);
            }

            if (incentivePlan.StartDate < curDate)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0011"]);
            }

            IncentivePlan publishedPlanWrtStDate = null, publishedPlanWrtEndDate = null, publishedInDateRange = null;
            DateTime startDate = incentivePlan.StartDate;
            DateTime endDate = incentivePlan.EndDate; 

            if (incentivePlan.Audience.AudienceType == AudienceType.RETAILER)
            {
                publishedPlanWrtStDate = await _incentiveRepository.GetRetailerPublishedPlan(incentivePlan.PlanType, incentivePlan.Audience.SubTypeName, startDate);
                publishedPlanWrtEndDate = await _incentiveRepository.GetRetailerPublishedPlan(incentivePlan.PlanType, incentivePlan.Audience.SubTypeName, endDate);
                publishedInDateRange = await _incentiveRepository.GetRetailerPublishedPlanInRange(incentivePlan.PlanType, incentivePlan.Audience.SubTypeName, startDate, endDate);
            }
            else
            {
                publishedPlanWrtStDate = await _incentiveRepository.GetConsumerPublishedPlan(incentivePlan.PlanType, startDate);
                publishedPlanWrtEndDate = await _incentiveRepository.GetConsumerPublishedPlan(incentivePlan.PlanType, endDate);
                publishedInDateRange = await _incentiveRepository.GetConsumerPublishedPlanInRange(incentivePlan.PlanType, startDate, endDate);
            }

            if (publishedPlanWrtStDate != null)
            {
                errorInfo.Add(string.Format(_stringLocalizer["INC_ERR_0019"], publishedPlanWrtStDate.Id));
            }

            if (publishedPlanWrtEndDate != null)
            {
                errorInfo.Add(string.Format(_stringLocalizer["INC_ERR_0020"], publishedPlanWrtEndDate.Id));
            }

            if(publishedInDateRange != null)
            {
                errorInfo.Add(string.Format(_stringLocalizer["INC_ERR_0024"], publishedInDateRange.Id));
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

        private async Task<ActionResult> PublishPlan(IncentivePlan plan)
        {
            if (plan == null)
            {
                return NotFound();
            }

            List<string> errorInfo = await ValidatePublish(plan);

            if (HasError(errorInfo))
            {
                return BadRequest(errorInfo);
            }

            plan.StartDate = plan.StartDate;
            plan.EndDate = plan.EndDate; 

            plan.PublishMode = PublishMode.PUBLISHED;

            int statusCode = await _incentiveRepository.UpdateIncentivePlan(plan);

            if (statusCode != (int)System.Net.HttpStatusCode.OK)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0016"]);
                return BadRequest(errorInfo);
            }

            return NoContent();
        }

        private async Task<List<string>> ValidatePublish(IncentivePlan plan)
        {
            List<string> errorInfo = new List<string>();

            if (plan.PublishMode != PublishMode.DRAFT)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0014"]);
                return errorInfo;
            }

            errorInfo = await ValidateDate(plan);

            if (HasError(errorInfo))
            {
                return errorInfo;
            }

            return errorInfo;

        }

        private async Task<List<string>> ValidateClosePlan(IncentivePlan plan, DateTime endDate)
        {
            List<string> errorInfo = new List<string>();

            if (plan.PublishMode != PublishMode.PUBLISHED)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0021"]);
                return errorInfo;
            }

            List<IncentivePlan> overlappingPlans = await _incentiveRepository.GetIncentivePlanList(plan.PlanType, plan.Audience, endDate);

            if (overlappingPlans != null && overlappingPlans.Count > 0)
            {
                var otherPlans = overlappingPlans.Where(x => !x.Id.Equals(plan.Id)).ToList();

                if (otherPlans.Count > 0)
                {
                    errorInfo.Add(string.Format(_stringLocalizer["INC_ERR_0022"], otherPlans.First().StartDate, otherPlans.First().EndDate));
                    return errorInfo;
                }
            }

            overlappingPlans = await _incentiveRepository.GetPublishedIncentivePlansInRange(plan.PlanType, plan.Audience, plan.StartDate, endDate);

            if (overlappingPlans != null && overlappingPlans.Count > 0)
            {
                var otherPlans = overlappingPlans.Where(x => !x.Id.Equals(plan.Id)).ToList();

                if (otherPlans.Count > 0)
                {
                    errorInfo.Add(string.Format(_stringLocalizer["INC_ERR_0024"], otherPlans.First().StartDate, otherPlans.First().EndDate));
                    return errorInfo;
                };
            }

            return errorInfo;
        }

        private async Task<ActionResult> DeleteIncentivePlan(IncentivePlan plan)
        {
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

            int statusCode = await _incentiveRepository.DeleteIncentivePlan(plan.Id.Value, plan.Audience.SubTypeName);

            if (statusCode == (int)System.Net.HttpStatusCode.OK)
            {
                return NoContent();
            }

            return NotFound();
        }

        private async Task<ActionResult> UpdatePlan(Guid planId, IncentivePlanRequest updatePlanRequest, IncentivePlan plan)
        {
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


        #endregion
    }
}
