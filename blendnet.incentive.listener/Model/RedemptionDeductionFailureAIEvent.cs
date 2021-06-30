using blendnet.common.dto.AIEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.incentive.listener.Model
{
    public class RedemptionDeductionFailureAIEvent: BaseAIEvent
    {
        /// <summary>
        /// User Id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Order Id
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// Order Completion Date
        /// </summary>
        public DateTime? OrderCompletedDate { get; set; }

        /// <summary>
        /// Event type of the event
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// Content provider id in case of order completed event
        /// </summary>
        public string EventSubType { get; set; }

        /// <summary>
        /// Order Item Details
        /// </summary>
        public string OrderItem { get; set; }

        /// <summary>
        /// Value set for redemption
        /// </summary>
        public double OriginalValue { get; set; }

        /// <summary>
        /// Calculated Value for Redemption
        /// </summary>
        public double CalculatedValue { get; set; }
    }
}
