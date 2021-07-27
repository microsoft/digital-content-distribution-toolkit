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
        CMSPortal = 2,
        System = 4
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
        public Guid UserId { get; set; }

        /// <summary>
        /// Associated Identity ID (from KMS)
        /// </summary>
        public Guid IdentityId { get; set; }

        /// <summary>
        /// Phone number of the person
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Name of the user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Channel ID who creates the user
        /// </summary>
        public Channel ChannelId { get; set; }

        /// <summary>
        /// Refferal information of the user
        /// </summary>
        public ReferralDto ReferralInfo { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        public UserContainerType Type { get; set; }

        public static bool IsPhoneNumberValid(string phoneNumber)
        {
            return phoneNumber != null
                    && phoneNumber.Length == 10
                    && !phoneNumber.StartsWith("+")
                    && long.TryParse(phoneNumber, out _);
        }
    }
}
