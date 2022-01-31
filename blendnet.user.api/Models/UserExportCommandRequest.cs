using blendnet.common.dto;
using System.ComponentModel.DataAnnotations;

namespace blendnet.user.api.Models
{
    /// <summary>
    /// Request for get user export command
    /// </summary>
    public class UserExportCommandRequest
    {
        // <summary>
        /// Phone Number of user. Needed only when called by admin
        /// </summary>
        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Phone_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Phone_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.Numeric, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.Numeric_ErrorCode)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Command Id
        /// </summary>
        [Required]
        public Guid CommandId { get; set; }
    }
}
