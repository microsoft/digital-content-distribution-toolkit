using blendnet.api.proxy.Retailer;
using blendnet.common.dto;
using blendnet.common.dto.Incentive;
using blendnet.common.dto.Retailer;
using blendnet.common.dto.User;
using blendnet.common.infrastructure.Authentication;
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
    [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin)]
    public class IncentiveController : ControllerBase
    {
        private readonly ILogger _logger;

        private IIncentiveRepository _incentiveRepository;

        private IncentiveAppSettings _incentiveAppSettings;

        IStringLocalizer<SharedResource> _stringLocalizer;

        private RetailerProviderProxy _retailerProviderProxy;

        public IncentiveController(IIncentiveRepository incentiveRepository,
                                ILogger<IncentiveController> logger,
                                IOptionsMonitor<IncentiveAppSettings> optionsMonitor,
                                IStringLocalizer<SharedResource> stringLocalizer,
                                RetailerProviderProxy retailerProviderProxy
                                )
        {
            _incentiveRepository = incentiveRepository;

            _logger = logger;

            _incentiveAppSettings = optionsMonitor.CurrentValue;

            _stringLocalizer = stringLocalizer;
        }

        #region Incentive management methods

        /// <summary>
        /// Create incentivePlan by a retailer
        /// </summary>
        /// <param name="incentivePlanRequest"></param>
        /// <returns></returns>
        [HttpPost("createincentiveplan", Name = nameof(CreateIncentivePlan))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult> CreateIncentivePlan(IncentivePlanRequest incentivePlanRequest)
        {
            List<string> errorInfo;
            // Validate date

            errorInfo = ValidateDate(incentivePlanRequest);

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
                retailerProviderDto = await _retailerProviderProxy.GetRetailerProviderByServiceAccountId(incentivePlanRequest.Audience.SubTypeId);

                if(retailerProviderDto == null)
                {
                    errorInfo.Add(_stringLocalizer["INC_ERR_0002"]);
                    return BadRequest(errorInfo);
                }
            }
            else
            {
                if(!Common.NIL_GUID.Equals(incentivePlanRequest.Audience.SubTypeId))
                {
                    errorInfo.Add(_stringLocalizer["INC_ERR_0006"]);
                    return BadRequest(errorInfo);
                }
            }

            IncentivePlan incentivePlan = CreatePlan(incentivePlanRequest, retailerProviderDto);

            Guid planId = await _incentiveRepository.CreateIncentivePlan(incentivePlan);

            return Ok(planId);
        }


        #endregion

        #region private methods

        private bool HasError(List<string> errorInfo)
        {
            return errorInfo != null && errorInfo.Count > 0;
        }

        private List<string> ValidateDate(IncentivePlanRequest incentivePlanRequest)
        {
            List<string> errorInfo = new List<string>();
            if(incentivePlanRequest.StartDate > incentivePlanRequest.EndDate)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0001"]);
            }

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

            HashSet<PlanDetail> processed = new HashSet<PlanDetail>();

            foreach(PlanDetail planDetail in planDetails)
            {
                if(processed.Contains(planDetail))
                {
                    errorInfo.Add(string.Format(_stringLocalizer["INC_ERR_0005"], planDetail.EventType));
                    return errorInfo;
                }

                if (!isEventForAudience(audienceType, planDetail.EventType))
                {
                    errorInfo.Add(string.Format(_stringLocalizer["INC_ERR_0004"], planDetail.EventType.ToString(), audienceType));
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
            incentivePlan.PlanId = Guid.NewGuid();
            incentivePlan.PlanName = incentivePlanRequest.PlanName;
            incentivePlan.StartDate = incentivePlanRequest.StartDate;
            incentivePlan.EndDate = incentivePlanRequest.EndDate;

            incentivePlan.Audience = new Audience();
            incentivePlan.Audience.AudienceType = incentivePlanRequest.Audience.AudienceType;

            if (incentivePlan.Audience.AudienceType == AudienceType.CONSUMER)
            {
                incentivePlan.Audience.SubTypeId = new Guid(ApplicationConstants.Common.NIL_GUID);
                incentivePlan.Audience.SubTypeName = ApplicationConstants.Common.ALL;
            }
            else
            {
                incentivePlan.Audience.SubTypeId = retailerProviderDto.ServiceAccountId;
                incentivePlan.Audience.SubTypeName = retailerProviderDto.Name;
            }

            incentivePlan.PlanDetails = incentivePlanRequest.PlanDetails;

            incentivePlan.CreatedByUserId = UserClaimData.GetUserId(User.Claims);
            incentivePlan.CreatedDate = DateTime.UtcNow;

            return incentivePlan;
        }

        private bool isEventForAudience(AudienceType audienceType, EventType eventSubType)
        {
            if(audienceType == AudienceType.CONSUMER)
            {
                return eventSubType.ToString().StartsWith("CNSR");
            }

            return eventSubType.ToString().StartsWith("RTLR");
        }

        #endregion
    }
}
