// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.api.proxy;
using blendnet.api.proxy.Retailer;
using blendnet.common.dto.Incentive;
using blendnet.common.dto.Retailer;
using blendnet.common.dto.User;
using blendnet.common.infrastructure.Authentication;
using blendnet.incentive.api.Common;
using blendnet.incentive.api.Model;
using blendnet.incentive.repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using static blendnet.common.dto.ApplicationConstants;

namespace blendnet.incentive.api.Controllers
{
    /// <summary>
    /// Incentive Event Controller
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [AuthorizeRoles(KaizalaIdentityRoles.Retailer, 
                    KaizalaIdentityRoles.SuperAdmin, 
                    KaizalaIdentityRoles.AnalyticsReporter)]
    public class IncentiveEventController : ControllerBase
    {
        private const string C_CONSUMER = "CONSUMER";

        private const string C_RETAILER = "RETAILER";

        private readonly ILogger _logger;

        private IIncentiveRepository _incentiveRepository;

        private IncentiveAppSettings _incentiveAppSettings;

        IStringLocalizer<SharedResource> _stringLocalizer;

        private RetailerProxy _retailerProxy;

        private readonly UserProxy _userProxy;

        private IncentiveCalculationHelper _incentiveCalculationHelper;

        private readonly IConfiguration _configuration;

        public IncentiveEventController(IIncentiveRepository incentiveRepository,
                                ILogger<IncentiveEventController> logger,
                                IConfiguration configuration,
                                IOptionsMonitor<IncentiveAppSettings> optionsMonitor,
                                IStringLocalizer<SharedResource> stringLocalizer,
                                RetailerProxy retailerProxy,
                                UserProxy userProxy,
                                IncentiveCalculationHelper incentiveCalculationHelper)
        {
            _incentiveRepository = incentiveRepository;

            _logger = logger;

            _configuration = configuration;

            _incentiveAppSettings = optionsMonitor.CurrentValue;

            _stringLocalizer = stringLocalizer;

            _retailerProxy = retailerProxy;

            _userProxy = userProxy;

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
        [HttpGet("retailer/milestone/{partnerCode}/{retailerPartnerProvidedId}", Name = nameof(GetRetailerCalculatedMilestone))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(KaizalaIdentityRoles.Retailer, KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<IncentivePlan>> GetRetailerCalculatedMilestone(string partnerCode, string retailerPartnerProvidedId,Guid? planId)
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
        /// Returns the milestone for the given user phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        [HttpPost("consumer/regular", Name = nameof(GetConsumerCalculatedRegularByPhoneNumber))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<EventAggregateData>> GetConsumerCalculatedRegularByPhoneNumber(ConsumerCalculatedRegularByPhoneNumberRequest request)
        {
            User user = await _userProxy.GetUserByPhoneNumber(request.PhoneNumber);

            if (user is null || user.AccountStatus != UserAccountStatus.Active)
            {
                _logger.LogInformation("User not found or is not Active.");
                return NotFound();
            }

            var response = await _incentiveCalculationHelper.CalculateRandomIncentiveForConsumer(request.PhoneNumber);

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
        /// Returns the retailer regular incetive data for reporting
        /// </summary>
        /// <returns></returns>
        [HttpPost("retailer/regular/report", Name = nameof(GetRetailerRegularReportCalculated))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin, KaizalaIdentityRoles.AnalyticsReporter)]
        public async Task<ActionResult<CalculatedIncentivePlan>> GetRetailerRegularReportCalculated(IncentiveReportRequest reportRequest)
        {
            return await GetRetailerReportCalculated(reportRequest, PlanType.REGULAR);
        }

        /// <summary>
        /// Returns the retailer milestone incetive data for reporting
        /// </summary>
        /// <returns></returns>
        [HttpPost("retailer/milestone/report", Name = nameof(GetRetailerMilestoneReportCalculated))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin, KaizalaIdentityRoles.AnalyticsReporter)]
        public async Task<ActionResult<CalculatedIncentivePlan>> GetRetailerMilestoneReportCalculated(IncentiveReportRequest reportRequest)
        {
            return await GetRetailerReportCalculated(reportRequest, PlanType.MILESTONE);
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

            int maxDaysGap = _configuration.GetValue<int>("MaxDaysGapInQuery");

            if (numberOfDays > maxDaysGap)
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

        /// <summary>
        /// Get Retailer Report Calculated
        /// </summary>
        /// <param name="reportRequest"></param>
        /// <param name="planType"></param>
        /// <returns></returns>
        private async Task<ActionResult<CalculatedIncentivePlan>> GetRetailerReportCalculated(  IncentiveReportRequest reportRequest, 
                                                                                                PlanType planType)
        {
            List<string> errorInfo = new List<string>();

            DateTime reportingDate;

            bool isDateCorrect = DateTime.TryParseExact($"{reportRequest.ReportingDate.Year}{reportRequest.ReportingDate.Month.ToString("00")}{reportRequest.ReportingDate.Day.ToString("00")}",
                                                          DateTimeFormats.FormatYYYYMMDD,
                                                          CultureInfo.InvariantCulture,
                                                          DateTimeStyles.None,
                                                          out reportingDate);

            if (!isDateCorrect)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0043"]);

                return BadRequest(errorInfo);
            }

            //in  case no partner ids are passed.
            if (reportRequest.PartnerIds == null || reportRequest.PartnerIds.Length <= 0)
            {
                errorInfo.Add(_stringLocalizer["INC_ERR_0044"]);

                return BadRequest(errorInfo);
            }

            //set the reporting date to end of IST represented in UTC.
            reportingDate = new DateTime(reportingDate.Year,
                                         reportingDate.Month,
                                         reportingDate.Day, 18, 29, 59, DateTimeKind.Utc);

            //get the published plan for reporting date
            IncentivePlan incentivePlan = await _incentiveRepository.GetRetailerPublishedPlan(planType, reportRequest.PartnerCode, reportingDate);

            errorInfo = ValidateIncentivePlan(incentivePlan);

            if (errorInfo.Count > 0)
            {
                return BadRequest(errorInfo);
            }

            //calculate incentive plan
            CalculatedIncentivePlan calculatedIncentivePlan = await _incentiveCalculationHelper.CalculateIncentivePlan(incentivePlan,
                                                                                                                        reportingDate,
                                                                                                                        reportRequest.PartnerIds);

            return Ok(calculatedIncentivePlan);
        }

        #endregion
    }
}
