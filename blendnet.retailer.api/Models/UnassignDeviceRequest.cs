using blendnet.common.dto;
using System.ComponentModel.DataAnnotations;

namespace blendnet.retailer.api.Models
{
    public class UnassignDeviceRequest
    {
        [Required]
        public string PartnerCode { get; set; }

        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Title_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Title_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string PartnerProvidedRetailerId { get; set; }

        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Title_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Title_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumeric, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumeric_ErrorCode)]
        public string DeviceId { get; set; }
    }
}