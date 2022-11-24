// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Oms
{
    /// <summary>
    /// Class to hold data that needs to be exported for given user
    /// </summary>
    public class OrderToExport
    {
        /// <summary>
        /// Order Completed By
        /// </summary>
        public string OrderCompletedBy { get; set; }

        /// <summary>
        /// Subscription Title
        /// </summary>
        public string SubscriptionTitle { get; set; }

        /// <summary>
        /// Subscription Duration in Days
        /// </summary>
        public int SubscriptionDurationDays { get; set; }

        /// <summary>
        /// Subscription Price
        /// </summary>
        public float SubscriptionPrice { get; set; }

        /// <summary>
        /// Subscription Redemption Value
        /// </summary>
        public int SubscriptionRedemptionValue { get; set; }

        /// <summary>
        /// Total Amount Collected
        /// </summary>
        public float? TotalAmountCollected { get; set; }

        /// <summary>
        /// Order Status
        /// </summary>
        public OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// Deposit Date
        /// </summary>
        public DateTime? DepositDate { get; set; }

        /// <summary>
        /// Is Redeemed
        /// </summary>
        public bool IsRedeemed { get; set; }

        /// <summary>
        /// Total Redemmed Value
        /// </summary>
        public int? TotalRedemmedValue { get; set; }

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

        
    }
}
