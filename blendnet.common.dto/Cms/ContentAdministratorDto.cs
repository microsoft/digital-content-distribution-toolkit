using System;
using System.ComponentModel.DataAnnotations;

namespace blendnet.common.dto
{
    public class ContentAdministratorDto
    {
        public Guid? Id { get; set; }

        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Phone_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Phone_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.Numeric, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.Numeric_ErrorCode)]
        public string PhoneNumber { get; set; }

        [Required]
        public Guid UserId { get; set; }

    }
}