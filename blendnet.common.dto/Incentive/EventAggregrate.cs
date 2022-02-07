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
        public List<EventType> EventTypes { get; set; }

        public string[] EventCreatedFor { get; set; }

        public AudienceType AudienceType { get; set; }

        public string SubTypeName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }

    /// <summary>
    /// Event Aggregrate Response
    /// </summary>
    public class EventAggregrateResponse
    {
        /// <summary>
        /// Aggregrate Sum of Calculated Value
        /// </summary>
        public double AggregratedCalculatedValue { get; set; }

        /// <summary>
        /// Aggregrated Sum of Original Value
        /// </summary>
        public double AggregratedOriginalValue { get; set; }

        /// <summary>
        /// Aggregrated Count
        /// </summary>
        public double AggregratedCount { get; set; }

        /// <summary>
        /// Event Create for
        /// </summary>
        public string EventCreatedFor { get; set; }
        
        /// <summary>
        /// Event Type
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// Event Sub Type
        /// </summary>
        public string EventSubType { get; set; }

    }

    public class EventAggregateData
    {
        /// <summary>
        /// List of event aggregate response
        /// </summary>
        public List<EventAggregrateResponse> EventAggregateResponses { get; set; }

        /// <summary>
        /// Summation of all aggregate values
        /// </summary>
        public double TotalValue { get; set; }
    }
}
