// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace blendnet.common.dto
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContentProviderContainerType
    {
        ContentProvider = 0,
        Subscription = 1,
    }
}
