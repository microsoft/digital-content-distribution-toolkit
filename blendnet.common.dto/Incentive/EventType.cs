using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace blendnet.common.dto.Incentive
{

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EventGroupType
    {
        COMMISSION,
        REFERRAL,
        CONSUMER
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EventType
    {
        CNSR_INCM_FIRST_SIGNIN,
        CNSR_INCM_APP_ONCE_OPEN,
        CNSR_INCM_ORDER_COMPLTD,
        RTLR_INCM_ORDER_COMPLTD,
        RTLR_INCM_REFFRAL_COMPLTD,
        CNSR_EXPN_SUBS_REDEEM
    }
}
