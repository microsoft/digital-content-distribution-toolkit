using blendnet.common.dto;
using blendnet.common.dto.Incentive;
using blendnet.incentive.api.Model;
using blendnet.incentive.repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace blendnet.incentive.api.Controllers
{
    public class IncentiveController : ControllerBase
    {
        private readonly ILogger _logger;

        private IIncentiveRepository _incentiveRepository;

        private IncentiveAppSettings _incentiveAppSettings;

        IStringLocalizer<SharedResource> _stringLocalizer;

        public IncentiveController(IIncentiveRepository incentiveRepository,
                                ILogger<IncentiveController> logger,
                                IOptionsMonitor<IncentiveAppSettings> optionsMonitor,
                                IStringLocalizer<SharedResource> stringLocalizer)
        {
            _incentiveRepository = incentiveRepository;

            _logger = logger;

            _incentiveAppSettings = optionsMonitor.CurrentValue;

            _stringLocalizer = stringLocalizer;
        }

        #region Incentive management methods

        /// <summary>
        /// Create incentivePlan 
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

            // Validate Audience

            errorInfo = ValidateAudience(incentivePlanRequest);

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

            IncentivePlan incentivePlan = CreatePlan(incentivePlanRequest);

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

        private List<string> ValidateAudience(IncentivePlanRequest incentivePlanRequest)
        {
            AudienceType audienceType = incentivePlanRequest.Audience.AudienceType;
            List<string> errorInfo = new List<string>();

            if(audienceType == AudienceType.CONSUMER)
            {
                if(!ApplicationConstants.Common.NIL_GUID.Equals(incentivePlanRequest.Audience.SubTypeId))
                {
                    errorInfo.Add(_stringLocalizer["INC_ERR_0002"]);
                }

                if(!ApplicationConstants.Common.ALL.Equals(incentivePlanRequest.Audience.SubTypeName))
                {
                    errorInfo.Add(_stringLocalizer["INC_ERR_0003"]);
                }
            } 
            else
            {
                // Get retailer provider for given id

                //Validate retailer provider name

            }

            return errorInfo;
        }

        private List<string> ValidatePlanDetail(IncentivePlanRequest incentivePlanRequest)
        {
            throw new NotImplementedException();
        }

        private IncentivePlan CreatePlan(IncentivePlanRequest incentivePlanRequest)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
