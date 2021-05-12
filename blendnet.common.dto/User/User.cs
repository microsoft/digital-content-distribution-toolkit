using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace blendnet.common.dto.User
{
    /// <summary>
    /// Channel Info
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Channel
    {
        Customer = 0,
        Retailer = 1,
        Portal = 2
    }

    /// <summary>
    /// User
    /// </summary>
    public class User : PersonDto
    {
        /// <summary>
        /// Channel ID who creates the user
        /// </summary>
        public Channel ChannelId { get; set; }

        /// <summary>
        /// Refferal information of the user
        /// </summary>
        public ReferralDto ReferralInfo { get; set; }
    }
}
