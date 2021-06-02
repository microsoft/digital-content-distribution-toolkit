using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace blendnet.common.dto.Incentive
{
    /// <summary>
    /// Plan which indicates how the incentive calculation needs to be done
    /// </summary>
    public class IncentivePlan : BaseDto
    {
        /// <summary>
        /// Unique id of the plan
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid PlanId { get; set; }

        /// <summary>
        /// Name given to plan
        /// </summary>
        public string PlanName { get; set; }

        /// <summary>
        /// Start date of the plan
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the plan
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Target audience of the plan
        /// </summary>
        public Audience Audience { get; set; }

        /// <summary>
        /// List of events and corresponding formula associated with plan
        /// </summary>
        public List<Detail> IncentiveDetails { get; set; }
    }

    
}
