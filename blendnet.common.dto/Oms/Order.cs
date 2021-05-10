using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace blendnet.common.dto.Oms
{
    /// <summary>
    /// Order
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Unique Order Id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid? Id { get; set; }

        /// <summary>
        /// User Phone Number
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Unique User Id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Retailer Phone Number
        /// </summary>
        public string RetailerPhoneNumber { get; set; }

        /// <summary>
        /// Retailer Id
        /// </summary>
        public Guid? RetailerId { get; set; }

        /// <summary>
        /// Order Items
        /// </summary>
        public List<OrderItem> OrderItems { get; set; }

        /// <summary>
        /// Total Amount
        /// </summary>
        public float? TotalAmountCollected { get; set; }

        /// <summary>
        /// Order Status
        /// </summary>
        public OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// Payment deposit date obtained from partner
        /// </summary>
        public DateTime DepositDate { get; set; }

        /// <summary>
        /// Date when complete order request was placed -- stores date in format yyyymmdd
        /// </summary>
        public int? PaymentDepositDate { get; set; }

        /// <summary>
        /// Order Created Date
        /// </summary>
        public DateTime OrderCreatedDate { get; set; }

        /// <summary>
        /// Order Completion Date
        /// </summary>
        public DateTime? OrderCompletedDate { get; set; }

        /// <summary>
        /// Order Cancelled Date
        /// </summary>
        public DateTime? OrderCancelledDate { get; set; }

        /// <summary>
        /// Adds created by information
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Adds modified by information
        /// </summary>
        public Guid ModifiedBy { get; set; }

        public void SetIdentifier()
        {
            this.Id = Guid.NewGuid();
        }

        public Order()
        {
            SetIdentifier();
            OrderStatus = OrderStatus.Created;
            OrderCreatedDate = DateTime.UtcNow;
            OrderItems = new List<OrderItem>();
        }
    }

    /// <summary>
    /// Order Item
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// Subscription Details
        /// </summary>
        public ContentProviderSubscriptionDto Subscription { get; set; }

        /// <summary>
        /// Amount Collected
        /// </summary>
        public float? AmountCollected { get; set; }

        /// <summary>
        /// Purchased Plan Date.
        /// </summary>
        public DateTime? PlanStartDate { get; set; }

        /// <summary>
        /// Purchased Plan End Date.
        /// To Be calculated based on the configured subscription days 
        /// </summary>
        public DateTime? PlanEndDate { get; set; }

        /// <summary>
        /// Partner Payment Reference Number
        /// </summary>
        public string PartnerReferenceNumber { get; set; }

    }

    /// <summary>
    /// Role of artists present
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrderStatus
    {
        Created = 0,
        Completed = 1,
        Cancelled = 2
    }
}
