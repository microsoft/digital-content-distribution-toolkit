// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace blendnet.user.api.Models
{
    public class CreateRetailerRequest
    {
        /// <summary>
        /// ID assigned by the partner to the retailer
        /// </summary>
        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Title_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Title_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string RetailerId { get; set; }

        /// <summary>
        /// the user ID from Kaizala Identity
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Phone number without country code
        /// </summary>
        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Phone_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Phone_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.Numeric, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.Numeric_ErrorCode)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Retailer name
        /// </summary>
        [Required]
        [StringLength(ApplicationConstants.MaxMinLength.Title_Max_Length, MinimumLength = ApplicationConstants.MaxMinLength.Title_Min_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.AlphaNumericLimitedSplChars_ErrorCode)]
        public string Name { get; set; }

        /// <summary>
        /// Retailer Address
        /// </summary>
        [Required]
        public AddressDto Address { get; set; }

        /// <summary>
        /// Addition Attributes as a property bag
        /// </summary>
        public Dictionary<string, string> AdditionalAttributes {get; set;}
    }
}
