using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace blendnet.common.dto.User
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UserContainerType
    {
        User = 0,
        Command = 1
    }
}
