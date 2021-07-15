using AutoMapper;
using blendnet.common.dto;
using blendnet.common.dto.Incentive;
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
    [AuthorizeRoles(KaizalaIdentityRoles.User)]
    public class IncentiveBrowseController : ControllerBase
    {
        private IMapper _mapper;

        private readonly ILogger _logger;

        private IIncentiveRepository _incentiveRepository;

        private IncentiveAppSettings _incentiveAppSettings;

        IStringLocalizer<SharedResource> _stringLocalizer;

        public IncentiveBrowseController(IIncentiveRepository incentiveRepository,
                                ILogger<IncentiveController> logger,
                                IMapper mapper,
                                IOptionsMonitor<IncentiveAppSettings> optionsMonitor,
                                IStringLocalizer<SharedResource> stringLocalizer)
        {
            _incentiveRepository = incentiveRepository;

            _logger = logger;

            _mapper = mapper;

            _incentiveAppSettings = optionsMonitor.CurrentValue;

            _stringLocalizer = stringLocalizer;

        }

        #region Incentive browse apis

        /// <summary>
        /// Returns all consumer incentive plans with given plan type
        /// </summary>
        /// <param name="planType"></param>
        /// <returns></returns>
        [HttpGet("consumer/{planType}", Name = nameof(GetConsumerActivePlan))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<IncentivePlanDto>> GetConsumerActivePlan(PlanType planType)
        {
            Audience audience = new Audience()
            {
                AudienceType = AudienceType.CONSUMER,
                SubTypeName = ApplicationConstants.Common.CONSUMER
            };

            IncentivePlan incentivePlan = await _incentiveRepository.GetCurrentConsumerActivePlan(planType);

            if (incentivePlan == null)
            {
                return NotFound();
            }

            IncentivePlanDto incentivePlanDto = _mapper.Map<IncentivePlan, IncentivePlanDto>(incentivePlan);

            return Ok(incentivePlanDto);
        }

        #endregion
    }
}
