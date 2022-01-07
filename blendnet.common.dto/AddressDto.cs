using System.ComponentModel.DataAnnotations;

namespace blendnet.common.dto
{
    public class AddressDto
    {
        /// <summary>
        /// Address Line 1
        /// </summary>
        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Description_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Description_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string Address1 { get; set; }

        /// <summary>
        /// Address Line 2
        /// </summary>
        [StringLength(ApplicationConstants.MaxMinLength.Description_Max_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string Address2 { get; set; }

        /// <summary>
        /// Address Line 3
        /// </summary>
        [StringLength(ApplicationConstants.MaxMinLength.Description_Max_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string Address3 { get; set; }
        
        /// <summary>
        /// City
        /// </summary>
        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Title_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Title_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string City { get; set; }
        
        /// <summary>
        /// State
        /// </summary>
        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Title_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Title_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string State { get; set; }
        
        /// <summary>
        /// PinCode
        /// </summary>
        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Zip_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Zip_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.Numeric, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.Numeric_ErrorCode)]
        public int PinCode { get; set; }

        /// <summary>
        /// Location data
        /// </summary>
        [Required]
        public MapLocationDto MapLocation { get; set; }
    }
}