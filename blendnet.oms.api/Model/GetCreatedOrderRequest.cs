// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto;
using blendnet.common.dto.Oms;
using System.ComponentModel.DataAnnotations;

namespace blendnet.oms.api.Model
{
    public class GetCreatedOrderRequest
    {

        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Title_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Title_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string RetailerPartnerProvidedId { get; set; }
        public string RetailerPartnerCode { get; set; }
    }
}
