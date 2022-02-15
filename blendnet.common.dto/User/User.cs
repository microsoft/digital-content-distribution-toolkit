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
    /// User Account Status
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UserAccountStatus
    {
        Active = 0,
        InActive = 1,
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
        public UserContainerType Type => UserContainerType.User;

        /// <summary>
        /// Account status
        /// </summary>
        public UserAccountStatus AccountStatus { get; set; } = UserAccountStatus.Active;

        /// <summary>
        /// ID of Data Export Request command
        /// </summary>
        public Guid? DataExportStatusUpdatedBy { get; set; }

        /// <summary>
        /// Data Export Request Status
        /// </summary>
        public DataExportRequestStatus DataExportRequestStatus { get; set; } = DataExportRequestStatus.NotInitialized;

        /// <summary>
        /// Data last Exported by command id
        /// </summary>
        public DataExportedBy DataExportedBy { get; set; }

        /// <summary>
        /// ID of Data Update Request command
        /// </summary>
        public Guid? DataUpdateStatusUpdatedBy { get; set; }

        /// <summary>
        /// Data Update Request Status
        /// </summary>
        public DataUpdateRequestStatus DataUpdateRequestStatus { get; set; } = DataUpdateRequestStatus.NotInitialized;


        /// <summary>
        /// If the user is deleted
        /// </summary>
        public bool IsUserDeleted { get; set; }

        public static bool IsPhoneNumberValid(string phoneNumber)
        {
            return phoneNumber != null
                    && phoneNumber.Length == 10
                    && !phoneNumber.StartsWith("+")
                    && long.TryParse(phoneNumber, out _);
        }
    }

    /// <summary>
    /// Last sucess data exported by
    /// </summary>
    public class DataExportedBy
    {
        public Guid? CommandId { get; set; }

        public DataExportResult DataExportResult { get; set; }

    }
}
