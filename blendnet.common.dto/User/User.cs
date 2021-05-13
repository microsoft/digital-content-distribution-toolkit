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
        ConsumerApp = 0,
        NovoRetailerApp = 1,
        CMSPortal = 2
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
