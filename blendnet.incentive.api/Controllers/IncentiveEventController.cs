using blendnet.common.infrastructure.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blendnet.common.dto;
using blendnet.common.dto.Incentive;
using Microsoft.AspNetCore.Authorization;
using blendnet.incentive.repository.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using blendnet.api.proxy.Retailer;
using blendnet.incentive.api.Common;
using Microsoft.Extensions.Options;
using static blendnet.common.dto.ApplicationConstants;
using blendnet.common.dto.Retailer;
using blendnet.common.dto.User;
using Microsoft.Extensions.Configuration;

namespace blendnet.incentive.api.Controllers
{
    /// <summary>
    /// Incentive Event Controller
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class IncentiveEventController : ControllerBase
    {
        private const string C_CONSUMER = "CONSUMER";

        private const string C_RETAILER = "RETAILER";

        private readonly ILogger _logger;

        private IIncentiveRepository _incentiveRepository;

        private IncentiveAppSettings _incentiveAppSettings;

        IStringLocalizer<SharedResource> _stringLocalizer;

        private RetailerProxy _retailerProxy;

        private IncentiveCalculationHelper _incentiveCalculationHelper;

        private readonly IConfiguration _configuration;

        public IncentiveEventController(IIncentiveRepository incentiveRepository,
                                ILogger<IncentiveEventController> logger,
                                IConfiguration configuration,
                                IOptionsMonitor<IncentiveAppSettings> optionsMonitor,
                                IStringLocalizer<SharedResource> stringLocalizer,
                                RetailerProxy retailerProxy,
                                IncentiveCalculationHelper incentiveCalculationHelper)
        {
            _incentiveRepository = incentiveRepository;

            _logger = logger;

            _configuration = configuration;

            _incentiveAppSettings = optionsMonitor.CurrentValue;

            _stringLocalizer = stringLocalizer;

            _retailerProxy = retailerProxy;

            _incentiveCalculationHelper = incentiveCalculationHelper;
        }

        ///<summary>
        ///Returns the calculated milestone for the consumer -- Disabling this method for now till it is needed
        ///</summary>
        ///<param name = "planId" ></ param >
        ///< returns ></ returns >
       // [HttpGet("consumer/milestone", Name = nameof(GetConsumerCalculatedMilestone))]
      //  [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
       /* public async Task<ActionResult<IncentivePlan>> GetConsumerCalculatedMilestone
            (Guid? planId)
        {
            string phoneNumber = User.Identity.Name;
            IncentivePlan incentivePlan;
            List<string> errorInfo;

            if (planId.HasValue)
            {
                incentivePlan = await _incentiveRepository.GetConsumerPublishedPlan(planId.Value, PlanType.MILESTONE);
            }
            else
            {
                incentivePlan = await _incentiveRepository.GetCurrentConsumerActivePlan(PlanType.MILESTONE);
            }

            errorInfo = ValidateIncentivePlan(incentivePlan);

            if (errorInfo.Count > 0)
            {
                return BadRequest(errorInfo);
            }

            incentivePlan = await _incentiveCalculationHelper.CalculateMilestoneForConsumer(incentivePlan, phoneNumber);

            return Ok(incentivePlan);
        */

        /// <summary>
        /// Returns the calculated milestone for the retailer
        /// If the plan id is passed, calculates the same
        /// If plan id is not passed, find the active and returns the details
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet("retailer/milestone/{retailerPartnerProvidedId}/{partnerCode}", Name = nameof(GetRetailerCalculatedMilestone))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(KaizalaIdentityRoles.Retailer, KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<IncentivePlan>> GetRetailerCalculatedMilestone(string retailerPartnerProvidedId, string partnerCode, Guid? planId)
        {
            IncentivePlan incentivePlan = null;

            RetailerDto retailer = await _retailerProxy.GetRetailerById(retailerPartnerProvidedId, partnerCode);
            
            List<string> errorInfo = ValidateRetailer(retailer);

            if(errorInfo.Count > 0)
            {
                return BadRequest(errorInfo);
            }

            if (planId.HasValue)
            {
                //Continue with the given plan id
                incentivePlan = await _incentiveRepository.GetRetailerPublishedPlan(planId.Value, partnerCode, PlanType.MILESTONE);
            }
            else
            {
                incentivePlan = await _incentiveRepository.GetCurrentRetailerActivePlan(PlanType.MILESTONE, partnerCode);
            }

            errorInfo = ValidateIncentivePlan(incentivePlan);

            if (errorInfo.Count > 0)
            {
                return BadRequest(errorInfo);
            }

            incentivePlan = await _incentiveCalculationHelper.CalculateMiletoneForRetailer(incentivePlan, retailer.PartnerId);

            return Ok(incentivePlan);
            
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

            if(response.EventAggregateResponses.Count == 0)
            {
                return NotFound();
            }

            return Ok(response);
        }

        /// <summary>
        /// Returns the milestone for the given user phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        [HttpGet("consumer/regular/{phoneNumber}", Name = nameof(GetConsumerCalculatedRegularByPhoneNumber))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<EventAggregateData>> GetConsumerCalculatedRegularByPhoneNumber(string phoneNumber)
        {
            var response = await _incentiveCalculationHelper.CalculateRandomIncentiveForConsumer(phoneNumber);

            if (response.EventAggregateResponses.Count == 0)
            {
                return NotFound();
            }

            return Ok(response);
        }

        /// <summary>
        /// Returns the calculated regular incentive plan for the consumer
        /// If the plan id is passed, calculates the same
        /// If plan id is not passed, find the active and returns the details
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet("retailer/regular/{partnerCode}/{retailerPartnerProvidedId}", Name = nameof(GetRetailerCalculatedRegular))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(KaizalaIdentityRoles.Retailer, KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<IncentivePlan>> GetRetailerCalculatedRegular(string partnerCode,
                                                                                       string retailerPartnerProvidedId,
                                                                                       Guid? planId)
        {
            IncentivePlan incentivePlan = null;

            RetailerDto retailer = await _retailerProxy.GetRetailerById(retailerPartnerProvidedId, partnerCode);

            List<string> errorInfo = ValidateRetailer(retailer);

            if (errorInfo.Count > 0)
            {
                return BadRequest(errorInfo);
            }

            if (planId.HasValue)
            {
                //Continue with the given plan id
                incentivePlan = await _incentiveRepository.GetRetailerPublishedPlan(planId.Value, partnerCode, PlanType.REGULAR);
            }
            else
            {
                incentivePlan = await _incentiveRepository.GetCurrentRetailerActivePlan(PlanType.REGULAR, partnerCode);
            }

            errorInfo = ValidateIncentivePlan(incentivePlan);

            if (errorInfo.Count > 0)
            {
                return BadRequest(errorInfo);
            }

            incentivePlan = await _incentiveCalculationHelper.CalculateIncentivePlanForRetailer(incentivePlan, retailer.PartnerId);

            return Ok(incentivePlan);
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
        public async Task<ActionResult<EventAggregateData>> GetConsumerCalculatedRndmIncentives(DateTime startDate,
                                                                                                      DateTime endDate)
        {
            string phoneNumber = User.Identity.Name;
            List<string> errorInfo = new List<string>();

            if (startDate == default(DateTime) || endDate == default(DateTime))
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0001"]);
                return BadRequest(errorInfo);
            }

            if(startDate > endDate)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0001"]);
                return BadRequest(errorInfo);
            }

            var eventAggregrateResponses = await _incentiveCalculationHelper.CalculateRandomIncentiveForConsumer(phoneNumber, startDate, endDate);

            if(eventAggregrateResponses.EventAggregateResponses.Count == 0)
            {
                return NotFound();
            }

            return Ok(eventAggregrateResponses);
           
        }

        /// <summary>
        /// Returns the event aggregrates for retailer the given start date and end date
        /// </summary>
        /// <param name="partnerCode"></param>
        /// <param name="retailerPartnerProvidedId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet("retailer/regular/range/{partnerCode}/{retailerPartnerProvidedId}", Name = nameof(GetRetailerCalculatedRndmIncentives))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(KaizalaIdentityRoles.Retailer, KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<EventAggregrateResponse>> GetRetailerCalculatedRndmIncentives(string partnerCode,
                                                                                        string retailerPartnerProvidedId,
                                                                                        DateTime startDate,
                                                                                        DateTime endDate)
        {
            //Token user id should match with the user id in database against the give RetailerPartnerProvidedId & partnerCode combination
            //Start Date & End Date is Mandatory

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

            RetailerDto retailer = await _retailerProxy.GetRetailerById(retailerPartnerProvidedId, partnerCode);
            
            errorInfo = ValidateRetailer(retailer);

            if(errorInfo.Count > 0)
            {
                return BadRequest(errorInfo);
            }

            var eventAggregrateResponses = await _incentiveCalculationHelper.CalculateRandomIncentiveForRetailer(retailer.PartnerId, startDate, endDate);

            if(eventAggregrateResponses.EventAggregateResponses.Count == 0)
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
        public async Task<ActionResult<List<IncentiveEvent>>> GetConsumerEvents(EventType eventType, DateTime startDate, DateTime endDate)
        {
            List<string> errorInfo = new List<string>();

            string phoneNumber = User.Identity.Name;

            if(!eventType.ToString().StartsWith(C_CONSUMER))
            {
                errorInfo.Add(string.Format(_stringLocalizer["INC_ERR_0029"], eventType));
                return BadRequest(errorInfo);
            }

            if(startDate > endDate)
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

            if(incentiveEvents.Count == 0)
            {
                return NotFound();
            }

            return Ok(incentiveEvents);
        }

        /// <summary>
        /// Returns the event aggregrates for retailer the given start date and end date
        /// </summary>
        /// <param name="partnerCode"></param>
        /// <param name="retailerPartnerProvidedId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet("retailer/events", Name = nameof(GetRetailerEvents))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(KaizalaIdentityRoles.Retailer, KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<IncentiveEvent>> GetRetailerEvents(string partnerCode,
                                                                                   string retailerPartnerProvidedId,
                                                                                   EventType eventType,
                                                                                   DateTime startDate,
                                                                                   DateTime endDate)
        {
            List<string> errorInfo = new List<string>();

            if (!eventType.ToString().StartsWith(C_RETAILER))
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

            if (numberOfDays > 30)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0030"]);
                return BadRequest(errorInfo);
            }

            RetailerDto retailer = await _retailerProxy.GetRetailerById(retailerPartnerProvidedId, partnerCode);

            errorInfo = ValidateRetailer(retailer);

            if(errorInfo.Count > 0)
            {
                return BadRequest(errorInfo);
            }

            List<IncentiveEvent> incentiveEvents = await _incentiveCalculationHelper.GetRetailerIncentiveEvents(retailer.PartnerId, eventType, startDate, endDate);

            if (incentiveEvents.Count == 0)
            {
                return NotFound();
            }

            return Ok(incentiveEvents);

        }

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

            if(!userId.Equals(retailer.UserId))
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0026"]);
                return errorInfo;
            }

            return errorInfo;
        }

        /// <summary>
        /// Checks if incentive plan exists
        /// </summary>
        /// <param name="incentivePlan"></param>
        /// <returns></returns>
        private List<string> ValidateIncentivePlan(IncentivePlan incentivePlan)
        {
            List<string> errorInfo = new List<string>();
            if (incentivePlan == null)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0027"]);
                return errorInfo;
            }

            return errorInfo;
        }

        #endregion
    }
}
