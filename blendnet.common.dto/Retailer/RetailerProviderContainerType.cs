// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace blendnet.common.dto.Retailer
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RetailerProviderContainerType
    {
        RetailerProvider = 0,
    }
}
