using blendnet.common.dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace blendnet.user.api.Models
{
    /// <summary>
    /// Link Retailer Request
    /// </summary>
    public class LinkRetailerRequest
    {
        /// <summary>
        /// Partner code of the retailer provider
        /// </summary>
        /// <value></value>
        public string PartnerCode { get; set; }

        /// <summary>
        /// ID provided by retailer provider
        /// </summary>
        /// <value></value>
        public string PartnerProvidedId { get; set; }
    }
}
