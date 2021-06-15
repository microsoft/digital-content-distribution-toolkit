using blendnet.common.dto.Incentive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Incentive
{
    /// <summary>
    /// EventAggregrateReqest
    /// </summary>
    public class EventAggregrateRequest
    {
        public RuleType AggregrateType { get; set; }

        public List<EventType> EventTypes { get; set; }

        public string EventGeneratorId { get; set; }

        public AudienceType AudienceType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }

    /// <summary>
    /// Event Aggregrate Response
    /// </summary>
    public class EventAggregrateResponse
    {
        /// <summary>
        /// Aggregrate Value
        /// </summary>
        public double AggregratedValue { get; set; }

        /// <summary>
        /// Event Type
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// Event Sub Type
        /// </summary>
        public string EventSubType { get; set; }

        /// <summary>
        /// Rule Type for which value is obtained
        /// </summary>
        public RuleType RuleType { get; set; }
    }
}
