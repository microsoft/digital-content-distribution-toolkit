// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.User
{
    /// <summary>
    /// Request class for Get User Details by Phone Number
    /// </summary>
    public class UserByPhoneRequest
    {
        // <summary>
        /// ID assigned by the partner to the retailer
        /// </summary>
        [Required]
        public string PhoneNumber { get; set; }
    }
}
