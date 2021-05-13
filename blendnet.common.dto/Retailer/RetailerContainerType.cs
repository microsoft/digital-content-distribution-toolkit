using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace blendnet.common.dto
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RetailerContainerType
    {
        Retailer = 0,
        RetailerReferralCode = 1,
    }
}
