// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Identity;
using Microsoft.Net.Http.Headers;
using System.ComponentModel.DataAnnotations;

namespace blendnet.retailer.api.Models
{
    public class RetailersByPartnerCodeRequest
    {
        /// <summary>
        /// Partner Code
        /// </summary>
        [Required]
        public string PartnerCode { get; set; }

        /// <summary>
        /// Continuation Token
        /// </summary>
        public string ContinuationToken { get; set; }

        /// <summary>
        /// Whether to fetch active retailers only
        /// </summary>
        public bool ShouldGetInactiveRetailer { get; set; } = true;

        /// <summary>
        /// page size
        /// </summary>
        [Range(1, 100)]
        public int PageSize { get; set; } = 50;      
    }
}
