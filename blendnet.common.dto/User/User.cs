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
    public class User : BaseDto
    {
        /// <summary>
        /// Id of the person
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Phone number of the person
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Name of the user
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Channel ID who creates the user
        /// </summary>
        public Channel ChannelId { get; set; }

        /// <summary>
        /// Refferal information of the user
        /// </summary>
        public ReferralDto ReferralInfo { get; set; }

        public static bool IsPhoneNumberValid(string phoneNumber)
        {
            return phoneNumber != null
                    && phoneNumber.Length == 10
                    && !phoneNumber.StartsWith("+")
                    && int.TryParse(phoneNumber, out _);
        }
    }
}
