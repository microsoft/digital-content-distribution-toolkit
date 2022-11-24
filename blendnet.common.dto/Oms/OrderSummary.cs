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
    /// This class stores the aggregated data of purchase for given retailer in the mentione duration
    /// </summary>
    public class OrderSummary
    {
        /// <summary>
        /// Total purchase count
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Retailer partner id
        /// </summary>
        public string RetailerPartnerId { get; set; }

        /// <summary>
        /// Payment deposit date in format yyyymmdd stored as integer
        /// </summary>
        public int Date { get; set; }

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
