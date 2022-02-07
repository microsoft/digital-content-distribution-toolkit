using blendnet.common.dto;
using System.ComponentModel.DataAnnotations;

namespace blendnet.incentive.api.Model
{
    /// <summary>
    /// Request to accept phone number
    /// </summary>
    public class ConsumerCalculatedRegularByPhoneNumberRequest
    {
        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Phone_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Phone_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.Numeric, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.Numeric_ErrorCode)]
        public string PhoneNumber { get; set; }
    }
}
