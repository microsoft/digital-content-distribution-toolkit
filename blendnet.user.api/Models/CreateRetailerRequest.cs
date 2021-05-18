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
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Retailer name
        /// </summary>
        [Required]
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
