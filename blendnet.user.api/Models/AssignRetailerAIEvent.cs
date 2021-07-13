using blendnet.common.dto.AIEvents;
using System;
using System.Collections.Generic;

namespace blendnet.user.api.Models
{
    /// <summary>
    /// Assign Retailer (to user) event for Application Insights
    /// </summary>
    public class AssignRetailerAIEvent : BaseAIEvent
    {
        /// <summary>
        /// User ID
        /// </summary>
        /// <value></value>
        public Guid UserId { get; set; }

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
        /// Combined ID from Partner Code and Retailer-provided ID
        /// </summary>
        /// <value></value>
        public string RetailerPartnerId { get; set; }

        /// <summary>
        /// Additional Attributes of the retailer
        /// </summary>
        /// <value></value>
        public Dictionary<string, string> RetailerAdditionalAttributes { get; set; }
    }
}
