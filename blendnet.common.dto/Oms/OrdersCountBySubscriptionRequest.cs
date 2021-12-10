
using System;
using System.ComponentModel.DataAnnotations;

namespace blendnet.common.dto.Oms
{
    /// <summary>
    /// Request class for getting count of orders by subscription and a cutoff date
    /// </summary>
    public class OrdersCountBySubscriptionRequest
    {
        /// <summary>
        /// Subscription ID
        /// </summary>
        [Required]
        public Guid SubscriptionId {get; set; }

        /// <summary>
        /// Cut-off date - orders should have been created after this date
        /// </summary>
        [Required]
        public DateTime CutoffDateTime {get; set;}
    }
}