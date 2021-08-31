using Newtonsoft.Json;

namespace blendnet.common.dto.User
{
    /// <summary>
    /// Whitelisted User
    /// </summary>
    public class WhitelistedUserDto
    {
        /// <summary>
        /// Id of the user (same as phone number)
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id => PhoneNumber;

        /// <summary>
        /// Phone number of the user
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Email ID associated with this user
        /// </summary>
        public string EmailId { get; set; }
    }
}
