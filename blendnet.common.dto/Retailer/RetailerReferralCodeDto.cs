// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Retailer
{
    public class RetailerReferralCodeDto
    {
        /// <summary>
        /// Same as Referral Code 
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id => ReferralCode;

        public RetailerContainerType Type => RetailerContainerType.RetailerReferralCode;

        public string ReferralCode { get; set; }

        public string PartnerId { get; set; }
    }
}
