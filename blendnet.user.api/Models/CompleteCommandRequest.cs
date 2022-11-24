// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto;
using System.ComponentModel.DataAnnotations;

namespace blendnet.user.api.Models
{
    public class CompleteCommandRequest
    {
        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Phone_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Phone_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.Numeric, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.Numeric_ErrorCode)]
        public string UserPhoneNumber { get; set; }
    }
}
