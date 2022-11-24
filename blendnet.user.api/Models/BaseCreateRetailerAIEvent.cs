// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.AIEvents;
using System.Collections.Generic;

namespace blendnet.user.api.Models
{
    /// <summary>
    /// Base class for Create Retailer event for Application Insights
    /// </summary>
    public abstract class BaseCreateRetailerAIEvent : BaseAIEvent
    {
        /// <summary>
        /// Retailer ID as provided by partner
        /// </summary>
        /// <value></value>
        public string PartnerProvidedId { get; set; }

        /// <summary>
        /// Partner Code
        /// </summary>
        /// <value></value>
        public string PartnerCode { get; set; }

        /// <summary>
        /// Retailer Name
        /// </summary>
        /// <value></value>
        public string Name { get; set; }

        /// <summary>
        /// Combined ID from Partner Code and Retailer-provided ID
        /// </summary>
        /// <value></value>
        public string RetailerPartnerId { get; set; }

        /// <summary>
        /// State
        /// </summary>
        /// <value></value>
        public string State { get; set; }

        /// <summary>
        /// City
        /// </summary>
        /// <value></value>
        public string City { get; set; }

        /// <summary>
        /// Pin Code
        /// </summary>
        /// <value></value>
        public int PinCode { get; set; }

        /// <summary>
        /// Latitude of the location coordinates
        /// </summary>
        /// <value></value>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude of the location coordinates
        /// </summary>
        /// <value></value>
        public double Longitude { get; set; }

        /// <summary>
        /// Additional Attributes for the retailer
        /// </summary>
        /// <value></value>
        public Dictionary<string, string> AdditionalAttributes { get; set; }
    }
}
