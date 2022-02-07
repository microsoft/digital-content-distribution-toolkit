using blendnet.common.infrastructure.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using blendnet.common.dto.Incentive;
using Microsoft.AspNetCore.Authorization;
using blendnet.incentive.repository.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using blendnet.incentive.api.Common;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using blendnet.incentive.api.Model;

namespace blendnet.incentive.api.Controllers
{
    /// <summary>
    /// Incentive Event Controller
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class IncentiveEventBrowseController : ControllerBase
    {
        private const string C_CONSUMER = "CONSUMER";

        private const string C_RETAILER = "RETAILER";

        private readonly ILogger _logger;

        private IIncentiveRepository _incentiveRepository;

        private IncentiveAppSettings _incentiveAppSettings;

        IStringLocalizer<SharedResource> _stringLocalizer;

        private IncentiveCalculationHelper _incentiveCalculationHelper;

        private readonly IConfiguration _configuration;

        private IMapper _mapper;

        public IncentiveEventBrowseController(IIncentiveRepository incentiveRepository,
                                ILogger<IncentiveEventController> logger,
                                IConfiguration configuration,
                                IOptionsMonitor<IncentiveAppSettings> optionsMonitor,
                                IStringLocalizer<SharedResource> stringLocalizer,
                                IncentiveCalculationHelper incentiveCalculationHelper,
                                IMapper mapper)
        {
            _incentiveRepository = incentiveRepository;

            _logger = logger;

            _configuration = configuration;

            _incentiveAppSettings = optionsMonitor.CurrentValue;

            _stringLocalizer = stringLocalizer;

            _incentiveCalculationHelper = incentiveCalculationHelper;

            _mapper = mapper;
        }

     
        /// <summary>
        /// Returns the calculated regular incentive plan for the consumer
        /// If the plan id is passed, calculates the same
        /// If plan id is not passed, find the active and returns the details
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet("consumer/regular", Name = nameof(GetConsumerCalculatedRegular))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<EventAggregateData>> GetConsumerCalculatedRegular()
        {
            string phoneNumber = User.Identity.Name;

            var response = await _incentiveCalculationHelper.CalculateRandomIncentiveForConsumer(phoneNumber);

            if (response.EventAggregateResponses.Count == 0)
            {
                return NotFound();
            }

            return Ok(response);
        }


        /// <summary>
        /// Returns the event aggregrates for consumer the given start date and end date
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet("consumer/regular/range", Name = nameof(GetConsumerCalculatedRndmIncentives))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<EventAggregateData>> GetConsumerCalculatedRndmIncentives(DateTime startDate,DateTime endDate)
        {
            string phoneNumber = User.Identity.Name;
         
            List<string> errorInfo = new List<string>();

            if (startDate == default(DateTime) || endDate == default(DateTime))
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0001"]);
                return BadRequest(errorInfo);
            }

            if (startDate > endDate)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0001"]);
                return BadRequest(errorInfo);
            }

            var eventAggregrateResponses = await _incentiveCalculationHelper.CalculateRandomIncentiveForConsumer(phoneNumber, startDate, endDate);

            if (eventAggregrateResponses.EventAggregateResponses.Count == 0)
            {
                return NotFound();
            }

            return Ok(eventAggregrateResponses);

        }

       
        /// <summary>
        /// Returns the event lists consumer events in the given start date and end date
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet("consumer/events", Name = nameof(GetConsumerEvents))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<IncentiveEventDto>>> GetConsumerEvents(EventType eventType, DateTime startDate, DateTime endDate)
        {
            List<string> errorInfo = new List<string>();

            string phoneNumber = User.Identity.Name;

            if (!eventType.ToString().StartsWith(C_CONSUMER))
            {
                errorInfo.Add(string.Format(_stringLocalizer["INC_ERR_0029"], eventType));
                return BadRequest(errorInfo);
            }

            if (startDate > endDate)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0001"]);
                return BadRequest(errorInfo);
            }

            var numberOfDays = (endDate - startDate).TotalDays;

            int maxDaysGap = _configuration.GetValue<int>("MaxDaysGapInQuery");

            if (numberOfDays > maxDaysGap)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0030"]);
                return BadRequest(errorInfo);
            }

            List<IncentiveEvent> incentiveEvents = await _incentiveCalculationHelper.GetConsumerIncentiveEvents(phoneNumber, eventType, startDate, endDate);

            if (incentiveEvents.Count == 0)
            {
                return NotFound();
            }

            List<IncentiveEventDto> incentiveEventsToReturn = _mapper.Map<List<IncentiveEvent>, List<IncentiveEventDto>>(incentiveEvents);

            return Ok(incentiveEventsToReturn);
        }
    }
}
