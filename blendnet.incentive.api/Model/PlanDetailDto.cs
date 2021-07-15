using blendnet.common.dto.Incentive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.incentive.api.Model
{
    public class PlanDetailDto
    {
        /// <summary>
        /// Sub type of event
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// Content provider id in case of order completed event
        /// </summary>
        public string EventSubType { get; set; }

        /// <summary>
        /// Title of event
        /// </summary>
        public string EventTitle { get; set; }

        /// <summary>
        /// Rule type indicating whether it is a sum or count event
        /// </summary>
        public RuleType RuleType { get; set; }

        /// <summary>
        /// Formula associated with the event
        /// </summary>
        public Formula Formula { get; set; }

        /// <summary>
        /// Result after application of Rule Type and Formula
        /// </summary>
        public Result Result { get; set; }

    }
}
