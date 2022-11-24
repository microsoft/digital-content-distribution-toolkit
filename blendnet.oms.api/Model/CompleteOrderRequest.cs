// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace blendnet.oms.api.Model
{
    public class CompleteOrderRequest
    {
        [Required]
        public Guid OrderId { get; set; }

        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Phone_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Phone_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.Numeric, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.Numeric_ErrorCode)]
        public string UserPhoneNumber { get; set; }

        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Title_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Title_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string RetailerPartnerProvidedId { get; set; }

        public string RetailerPartnerCode { get; set; }

        [Required]
        public float AmountCollected { get; set; }

        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Title_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Title_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string PartnerReferenceNumber { get; set;}

        [Required]
        public DateTime DepositDate { get; set; }

    }
}
