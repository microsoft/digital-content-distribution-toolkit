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
