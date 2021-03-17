using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace blendnet.common.dto.Cms
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ContentContainerType
    {
        Content = 0,
        Command = 1,
        Device = 2
    }
}
