// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto;
using System.ComponentModel.DataAnnotations;

namespace blendnet.user.api.Models
{
    /// <summary>
    /// Request for get user command
    /// </summary>
    public class UserCommandRequest
    {
        // <summary>
        /// Phone Number of user. Needed only when called by admin
        /// </summary>
        [StringLength(ApplicationConstants.MaxMinLength.Phone_Max_Length)]
        [RegularExpression(ApplicationConstants.ValidationRegularExpressions.Numeric, ErrorMessage = ApplicationConstants.ValidationRegularExpressions.Numeric_ErrorCode)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// In case the client wants to read based on User Id
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Command Id
        /// </summary>
        [Required]
        public Guid CommandId { get; set; }
    }
}
