using blendnet.common.dto.AIEvents;
using System.Collections.Generic;

namespace blendnet.user.api.Models
{
    /// <summary>
    /// Create Retailer event for Application Insights
    /// </summary>
    public class CreateRetailerAIEvent : BaseAIEvent
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
        /// Additional Attributes for the retailer
        /// </summary>
        /// <value></value>
        public Dictionary<string, string> AdditionalAttributes { get; set; }
    }
}
