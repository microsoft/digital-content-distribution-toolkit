using System;

namespace blendnet.common.dto.Incentive
{
    /// <summary>
    /// User Incentive Event
    /// </summary>
    public class IncentiveEventToExport
    {
        /// <summary>
        /// User phone number of consumer or Partner id of retailer
        /// </summary>
        public string EventCreatedFor { get; set; }

        /// <summary>
        /// Event type of the event
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// Time during which event was created. Might differ than CreatedDate when event is 
        /// called from client and there are network delays
        /// </summary>
        public DateTime EventOccuranceTime { get; set; }

        /// <summary>
        /// Category of the event indicating whether it is an income event or expense event
        /// </summary>
        public EventCategoryType EventCategoryType { get; set; }

        /// <summary>
        /// Computed Value of the event which is later used for computation
        /// </summary>
        public double Coins { get; set; }

    }
}
