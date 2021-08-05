using blendnet.common.dto;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace blendnet.retailer.api.Models
{
    public class CreateUnlinkedRetailerRequest
    {
        /// <summary>
        /// Partner-provided ID of the retailer
        /// </summary>
        [Required]
        public string PartnerProvidedId { get; set; }

        /// <summary>
        /// Retailer's Name
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Additional attributes as a property bag
        /// </summary>
        public Dictionary<string, string> AdditionalAttibutes { get; set; }

        /// <summary>
        /// Address of the retailer
        /// </summary>
        [Required]
        public AddressDto Address { get; set; }
    }
}
