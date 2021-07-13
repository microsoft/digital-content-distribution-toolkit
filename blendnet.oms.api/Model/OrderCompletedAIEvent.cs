using blendnet.common.dto.AIEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.oms.api.Model
{
    public class OrderCompletedAIEvent : BaseAIEvent
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
        /// Marks if its redemmed
        /// </summary>
        public bool IsRedeemed { get; set; }

        /// <summary>
        /// Retailer id
        /// </summary>
        public Guid? RetailerId { get; set; }

        /// <summary>
        /// Retailer partner id
        /// </summary>
        public string RetailerPartnerId { get; set; }

        /// <summary>
        /// Reailer partner code
        /// </summary>
        public string RetailerPartnerCode { get; set; }

        /// <summary>
        /// Retailer additional attributes
        /// </summary>
        public Dictionary<string, string> RetailerAdditionalAttributes { get; set; }
        
        /// <summary>
        /// List of subscriptions bought in the order
        /// </summary>
        public string OrderItems { get; set; }

        /// <summary>
        /// order created date time
        /// </summary>
        public string OrderPlacedDateTime { get; set; }

        /// <summary>
        /// payment deposit date time
        /// </summary>
        public string PaymentDepositDateTime { get; set; }

        /// <summary>
        /// order completed date time
        /// </summary>
        public string OrderCompletedDateTime { get; set; }

    }

    public class OrderItem
    {
        /// <summary>
        /// Subscription id
        /// </summary>
        public Guid SubscriptionId { get; set; }

        /// <summary>
        /// Content provider id
        /// </summary>
        public Guid ContentProviderId { get; set; }

        /// <summary>
        /// Subscription name
        /// </summary>
        public string SubscriptionName { get; set; }

        /// <summary>
        /// Subscription value
        /// </summary>
        public float SubscriptionValue { get; set; }

        /// <summary>
        /// Qty of points required to redeem
        /// </summary>
        public int RedemptionValue { get; set; }
    }

}
