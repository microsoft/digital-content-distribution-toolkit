using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Oms
{
    /// <summary>
    /// This class stores the aggregated data of purchase for given retailer in the mentione duration
    /// </summary>
    public class OrderSummary
    {
        /// <summary>
        /// Total purchase count
        /// Rename Count
        /// </summary>
        public int PurchaseCount { get; set; }

        /// <summary>
        /// Retailer phone number
        /// </summary>
        public string RetailerPhoneNumber { get; set; }

        /// <summary>
        /// Payment deposit date in format yyyymmdd stored as integer
        /// Rename to Date
        /// </summary>
        public int PaymentDepositDate { get; set; }

        /// <summary>
        /// Content provider id of the subscription
        /// </summary>
        public Guid ContentProviderId { get; set; }

        /// <summary>
        /// Subscription id of the purchase
        /// </summary>
        public Guid SubscriptionId { get; set; }

        /// <summary>
        /// Title of the subscription
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Total amount 
        /// </summary>
        public float TotalAmount { get; set; }


    }
}
