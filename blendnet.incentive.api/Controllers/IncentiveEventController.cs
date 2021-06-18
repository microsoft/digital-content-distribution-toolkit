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
         //<summary>
         //Returns the calculated milestone for the consumer
         //</summary>
         //<param name = "planId" ></ param >
         //< returns ></ returns >
        [HttpGet("consumer/milestone", Name = nameof(GetConsumerCalculatedMilestone))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<IncentivePlan>> GetConsumerCalculatedMilestone(Guid? planId)
        {
            if (planId.HasValue)
            {
                //Continue with the given plan id
            }
            else
            {
                //Get Current Active Plan
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the calculated milestone for the retailer
        /// If the plan id is passed, calculates the same
        /// If plan id is not passed, find the active and returns the details
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet("retailer/milestone", Name = nameof(GetRetailerCalculatedMilestone))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.Retailer)]
        public async Task<ActionResult<IncentivePlan>> GetRetailerCalculatedMilestone(Guid? planId)
        {
            if (planId.HasValue)
            {
                //Continue with the given plan id
            }
            else
            {
                //Get Current Active Plan
            }

            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns the calculated regular incentive plan for the consumer
        /// If the plan id is passed, calculates the same
        /// If plan id is not passed, find the active and returns the details
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet("consumer/regular", Name = nameof(GetConsumerCalculatedIncentives))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<IncentivePlan>> GetConsumerCalculatedIncentives(Guid? planId)
        {
            //Start Date & End Date is Mandatory
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the calculated regular incentive plan for the consumer
        /// If the plan id is passed, calculates the same
        /// If plan id is not passed, find the active and returns the details
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet("retailer/regular/{partnerCode}/{retailerPartnerProvidedId}", Name = nameof(GetRetailerCalculatedIncentives))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.Retailer)]
        public async Task<ActionResult<IncentivePlan>> GetRetailerCalculatedIncentives(string partnerCode,
                                                                                       string retailerPartnerProvidedId,
                                                                                       Guid? planId)
        {
            //Token user id should match with the user id in database against the give RetailerPartnerProvidedId & partnerCode combination
            //Start Date & End Date is Mandatory
            throw new NotImplementedException();
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
        public async Task<ActionResult<EventAggregrateResponse>> GetConsumerCalculatedRndmIncentives(DateTime startDate,
                                                                                                      DateTime endDate)
        {
            //Start Date & End Date is Mandatory
            throw new NotImplementedException();
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
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.Retailer)]
        public async Task<ActionResult<EventAggregrateResponse>> GetRetailerCalculatedRndmIncentives(string partnerCode,
                                                                                        string retailerPartnerProvidedId,
                                                                                        DateTime startDate,
                                                                                        DateTime endDate)
        {
            //Token user id should match with the user id in database against the give RetailerPartnerProvidedId & partnerCode combination
            //Start Date & End Date is Mandatory
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the event lists consumer events in the given start date and end date
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet("consumer/events", Name = nameof(GetConsumerEvents))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<EventAggregrateResponse>> GetConsumerEvents(DateTime startDate,DateTime endDate)
        {
            //Start Date & End Date is Mandatory
            //Only allow the events which are valid for consumers. Same validation exists on incentive plan create
            throw new NotImplementedException();
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
        [AuthorizeRoles(ApplicationConstants.KaizalaIdentityRoles.Retailer)]
        public async Task<ActionResult<EventAggregrateResponse>> GetRetailerEvents(string partnerCode,
                                                                                   string retailerPartnerProvidedId,
                                                                                   DateTime startDate,
                                                                                   DateTime endDate)
        {
            //Token user id should match with the user id in database against the give RetailerPartnerProvidedId & partnerCode combination
            //Only allow the events which are valid for retailers. Same validation exists on incentive plan create
            //Start Date & End Date is Mandatory
            throw new NotImplementedException();
        }
    }
}
