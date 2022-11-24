// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace blendnet.common.dto.Retailer
{
    public class RetailerProviderDto : BaseDto
    {
        /// <summary>
        /// ID of this provider, same as Partner Code
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id => this.PartnerCode;

        /// <summary>
        /// Friendly name of the retailer provider
        /// </summary>
        /// <value></value>
        public string Name { get; set; }
        
        /// <summary>
        /// Assigned partner code (usually 4-character)
        /// </summary>
        /// <value></value>
        public string PartnerCode {get; set;}

        /// <summary>
        /// User ID (corresponding User's ID)
        /// </summary>
        /// <value></value>
        public Guid UserId { get; set;}

        /// <summary>
        /// Start Date of the retailer provider
        /// </summary>
        /// <value></value>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End Date of the retailer provider
        /// </summary>
        /// <value></value>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Container type
        /// </summary>
        public RetailerProviderContainerType Type => RetailerProviderContainerType.RetailerProvider;

        /// <summary>
        /// Features state
        /// </summary>
        [Required] // for API response
        [JsonRequired] // for Database
        public RetailerProviderFeatures SupportedFeatures { get; set; }
    }

    /// <summary>
    /// Class to represent features enabled state for Retailer Providers
    /// </summary>
    public class RetailerProviderFeatures
    {
        /// <summary>
        /// Is Retailer Dashboard enabled in web app?
        /// </summary>
        [Required] // for API response
        [JsonRequired] // for Database
        public bool WebRetailerDashboardEnabled { get; set; }

        /// <summary>
        /// Is Order Completion enabled in web app?
        /// </summary>
        [Required] // for API response
        [JsonRequired] // for Database
        public bool WebOrderCompletionEnabled { get; set; }
    }
}
