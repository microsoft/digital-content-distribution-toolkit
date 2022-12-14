// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using AutoMapper;
using blendnet.api.proxy.Cms;
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
    [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin, KaizalaIdentityRoles.Retailer)]
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

        private IMapper _mapper;

        private RetailerProxy _retailerProxy;

        private ContentProviderProxy _contentProviderProxy;

        public IncentiveController( IIncentiveRepository incentiveRepository,
                                    ILogger<IncentiveController> logger,
                                    IOptionsMonitor<IncentiveAppSettings> optionsMonitor,
                                    IStringLocalizer<SharedResource> stringLocalizer,
                                    RetailerProviderProxy retailerProviderProxy,
                                    IncentiveCalculationHelper incentiveCalculationHelper,
                                    RetailerProxy retailerProxy,
                                    ContentProviderProxy contentProviderProxy,
                                    IMapper mapper)
        {
            _incentiveRepository = incentiveRepository;

            _logger = logger;

            _incentiveAppSettings = optionsMonitor.CurrentValue;

            _stringLocalizer = stringLocalizer;

            _retailerProviderProxy = retailerProviderProxy;

            _incentiveCalculationHelper = incentiveCalculationHelper;

            _mapper = mapper;

            _retailerProxy = retailerProxy;

            _contentProviderProxy = contentProviderProxy;
        }

        #region Incentive management methods

        /// <summary>
        /// Create incentivePlan by a retailer
        /// </summary>
        /// <param name="incentivePlanRequest"></param>
        /// <returns></returns>
        [HttpPost("retailer", Name = nameof(CreateRetailerIncentivePlan))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin)]
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
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin)]
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
        [HttpPut("retailer/{planId:guid}", Name = nameof(UpdateRetailerIncentivePlan))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin)]
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
        [HttpPut("consumer/{planId:guid}", Name = nameof(UpdateConsumerIncentivePlan))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin)]
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
        [HttpDelete("retailer/{planId:guid}/{subtypeName}", Name = nameof(DeleteRetailerIncentivePlan))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin)]
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
        [HttpDelete("consumer/{planId:guid}", Name = nameof(DeleteConsumerIncentivePlan))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin)]
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
        [HttpPut("consumer/changeenddate/{planId:guid}/{endDate}", Name = nameof(ChangeConsumerIncentivePlan))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult> ChangeConsumerIncentivePlan(Guid planId, DateTime endDate)
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
        [HttpPut("retailer/changeenddate/{planId:guid}/{subTypeName}/{endDate}", Name = nameof(ChangeRetailerIncentivePlan))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult> ChangeRetailerIncentivePlan(Guid planId, string subTypeName, DateTime endDate)
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
        [HttpPut("retailer/publish/{planId:guid}/{subtypeName}", Name = nameof(PublishRetailerPlan))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin)]
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
        [HttpPut("consumer/publish/{planId:guid}", Name = nameof(PublishConsumerPlan))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin)]
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
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin)]
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
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin)]
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
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin)]
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
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin)]
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

        [HttpGet("eventlist/{audienceType}", Name = nameof(GetEventTypes))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin)]
        public ActionResult<List<string>> GetEventTypes(AudienceType audienceType)
        {
            List<string> eventTypes = new List<string>();

            if (audienceType == AudienceType.RETAILER)
            {
                eventTypes.Add(EventType.RETAILER_INCOME_ORDER_COMPLETED.ToString());
                eventTypes.Add(EventType.RETAILER_INCOME_REFERRAL_COMPLETED.ToString());
                eventTypes.Add(EventType.RETAILER_INCOME_DOWNLOAD_MEDIA_COMPLETED.ToString());
            }
            else
            {
                eventTypes.Add(EventType.CONSUMER_INCOME_APP_ONCE_OPEN.ToString());
                eventTypes.Add(EventType.CONSUMER_INCOME_FIRST_SIGNIN.ToString());
                eventTypes.Add(EventType.CONSUMER_INCOME_ONBOARDING_RATING_SUBMITTED.ToString());
                eventTypes.Add(EventType.CONSUMER_INCOME_STREAMED_CONTENT_ONCE_PER_CONTENTPROVIDER.ToString());
                eventTypes.Add(EventType.CONSUMER_INCOME_ORDER_COMPLETED.ToString());
                eventTypes.Add(EventType.CONSUMER_INCOME_DOWNLOAD_MEDIA_COMPLETED.ToString());
                eventTypes.Add(EventType.CONSUMER_EXPENSE_SUBSCRIPTION_REDEEM.ToString());
            }

            return Ok(eventTypes);
        }

        /// <summary>
        /// Returns current active consumer incentive plan with given plan type
        /// </summary>
        /// <param name="planType"></param>
        /// <returns></returns>
        [HttpGet("consumer/active/{planType}", Name = nameof(GetConsumerActiveIncentivePlan))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<IncentivePlan>> GetConsumerActiveIncentivePlan(PlanType planType)
        {
            IncentivePlan incentivePlan = await _incentiveRepository.GetCurrentConsumerActivePlan(planType);

            if (incentivePlan == null)
            {
                return NotFound();
            }

            return Ok(incentivePlan);
        }

        #endregion

        #region Retailer Specific
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

            List<IncentivePlan> incentivePlans = await _incentiveRepository.GetIncentivePlans(audience, planType, true);

            if (incentivePlans == null || incentivePlans.Count == 0)
            {
                return NotFound();
            }

            List<IncentivePlanDto> incentivePlanDto = _mapper.Map<List<IncentivePlan>, List<IncentivePlanDto>>(incentivePlans);

            return Ok(incentivePlanDto);
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
            errorInfo = await ValidatePlanDetail(incentivePlanRequest);

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
                errorInfo.Add(string.Format(_stringLocalizer["INC_ERR_0024"], startDate,endDate));
            }

            return errorInfo;
        }

        private async Task<List<string>> ValidatePlanDetail(IncentivePlanRequest incentivePlanRequest)
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
                //checking for duplicate based on Event and Event Subtype
                if (processed.Contains(planDetail))
                {
                    errorInfo.Add(string.Format(_stringLocalizer["INC_ERR_0005"], planDetail.EventType));
                    return errorInfo;
                }

                if ((planDetail.EventType == EventType.RETAILER_INCOME_ORDER_COMPLETED || 
                    planDetail.EventType == EventType.CONSUMER_INCOME_ORDER_COMPLETED))
                {
                    //now order completed event can be added without selecting the content provider
                    if (!string.IsNullOrEmpty(planDetail.EventSubType))
                    {
                        Guid contentProviderId;

                        //in case of order complete, sub type should have content provider id and it should be GUID
                        if (!Guid.TryParse(planDetail.EventSubType, out contentProviderId))
                        {
                            errorInfo.Add(_stringLocalizer["INC_ERR_0046"]);
                            return errorInfo;
                        }

                        ContentProviderDto contentProvider = await _contentProviderProxy.GetContentProviderById(contentProviderId);

                        if (contentProvider == null)
                        {
                            errorInfo.Add(_stringLocalizer["INC_ERR_0046"]);
                            return errorInfo;
                        }
                    }
                } 
                else if(planDetail.EventSubType != null)
                {
                    errorInfo.Add(string.Format(_stringLocalizer["INC_ERR_0039"], planDetail.EventType));
                    return errorInfo;
                }

                if (!IsEventForAudience(audienceType, planDetail.EventType))
                {
                    errorInfo.Add(string.Format(_stringLocalizer["INC_ERR_0004"], planDetail.EventType.ToString(), audienceType));
                    return errorInfo;
                }

                if (!IsRuleTypeValid(planDetail.RuleType, incentivePlanRequest.PlanType))
                {
                    errorInfo.Add(_stringLocalizer["INC_ERR_0008"]);
                    return errorInfo;
                }

                errorInfo = IsFormulaValid(planDetail.Formula, incentivePlanRequest.PlanType);
                if (errorInfo != null && errorInfo.Count > 0)
                {
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

        private List<string> IsFormulaValid(Formula formula, PlanType planType)
        {
            List<string> errorInfo = new List<string>();

            var formulaType = formula.FormulaType;
            if (planType == PlanType.REGULAR)
            {
                if(!(formulaType == FormulaType.PLUS
                    || formulaType == FormulaType.MINUS
                    || formulaType == FormulaType.MULTIPLY
                    || formulaType == FormulaType.PERCENTAGE))
                {
                    errorInfo.Add(_stringLocalizer["INC_ERR_0009"]);
                    return errorInfo;
                }
            }

            if(formulaType == FormulaType.DIVIDE_AND_MULTIPLY)
            {
                if(formula.FirstOperand == null || formula.SecondOperand == null)
                {
                    errorInfo.Add(_stringLocalizer["INC_ERR_0040"]);
                    return errorInfo;
                }

                if(formula.FirstOperand.Value <= 0 || formula.SecondOperand.Value <= 0)
                {
                    errorInfo.Add(_stringLocalizer["INC_ERR_0042"]);
                    return errorInfo;
                }
            } 
            else if(formulaType == FormulaType.RANGE)
            {
                if(formula.RangeOperand == null || formula.RangeOperand.Count == 0)
                {
                    errorInfo.Add(_stringLocalizer["INC_ERR_0035"]);
                    return errorInfo;
                }

                HashSet<double> set = new HashSet<double>();
                SortedDictionary<double, int> sortedRange = new SortedDictionary<double, int>();

                foreach(var range in formula.RangeOperand)
                {
                    if(range.StartRange > range.EndRange)
                    {
                        errorInfo.Add(_stringLocalizer["INC_ERR_0036"]);
                        return errorInfo;
                    }

                    sortedRange.Add(range.StartRange, 1);
                    sortedRange.Add(range.EndRange, -1);

                    if(range.Output <= 0)
                    {
                        errorInfo.Add(_stringLocalizer["INC_ERR_0038"]);
                        return errorInfo;
                    }
                }

                if(isOverlappingRangeExists(sortedRange))
                {
                    errorInfo.Add(_stringLocalizer["INC_ERR_0037"]);
                    return errorInfo;
                }
            } 
            else
            {
                if (formula.FormulaType == FormulaType.PLUS || formula.FormulaType == FormulaType.PERCENTAGE || formula.FormulaType == FormulaType.MULTIPLY)
                {
                    if (formula.FirstOperand == null)
                    {
                        errorInfo.Add(string.Format(_stringLocalizer["INC_ERR_0041"], formulaType));
                        return errorInfo;
                    }

                    if (formula.FirstOperand.Value <= 0)
                    {
                        errorInfo.Add(_stringLocalizer["INC_ERR_0042"]);
                        return errorInfo;
                    }
                }
            }

            return errorInfo;
        }

        /// <summary>
        /// Checks if overlapping range exists for given range of values
        /// </summary>
        /// <param name="sortedRange"></param>
        /// <returns></returns>
        private bool isOverlappingRangeExists(SortedDictionary<double, int> sortedRange)
        {
            int count = 0;

            foreach(var key in sortedRange.Keys)
            {
                count += sortedRange[key];
                if (count > 1) return true;
            }

            return false;
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
                errorInfo.Add(_stringLocalizer["INC_ERR_0015"]);
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

            if (endDate < plan.StartDate)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0001"]);
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

            if (statusCode == (int)System.Net.HttpStatusCode.NoContent)
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

            plan.ModifiedByByUserId = UserClaimData.GetUserId(User.Claims);
            plan.ModifiedDate = DateTime.UtcNow;

            int statusCode = await _incentiveRepository.UpdateIncentivePlan(plan);

            if (statusCode == (int)System.Net.HttpStatusCode.OK)
            {
                return NoContent();
            }

            errorInfo.Add(_stringLocalizer["INC_ERR_0013"]);

            return BadRequest(errorInfo);
        }

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
