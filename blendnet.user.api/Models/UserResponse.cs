using blendnet.common.dto.User;

namespace blendnet.user.api.Models
{
    public class UserResponse
    {
        /// <summary>
        /// Id of the person
        /// </summary>
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
    }
}