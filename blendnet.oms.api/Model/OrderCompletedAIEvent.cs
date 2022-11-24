// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.AIEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.oms.api.Model
{
    public class OrderCompletedAIEvent : OrderAIEvent
    {

        /// <summary>
        /// payment deposit date time
        /// </summary>
        public string PaymentDepositDateTime { get; set; }

        /// <summary>
        /// order completed date time
        /// </summary>
        public string OrderCompletedDateTime { get; set; }

        /// <summary>
        /// Marks if its redemmed
        /// </summary>
        public bool IsRedeemed { get; set; }

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
    }


}
