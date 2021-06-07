using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace blendnet.common.dto.Retailer
{
    public class RetailerDto : User.User
    {
        /// <summary>
        /// Original ID provided by the onboarding partner
        /// </summary>
        [Required]
        public string PartnerProvidedId { get; set; }

        /// <summary>
        /// Globally Unique ID for the retailer
        /// This is a combination of Partner Code and partner-provided ID
        /// </summary>
        public string PartnerId
        {
            get
            {
                return CreatePartnerId(PartnerCode, PartnerProvidedId);
            }
            set
            {
                // No-op
            }
        }

        /// <summary>
        /// Partner who onboarded this retailer
        /// </summary>
        [Required]
        public string PartnerCode { get; set; }

        /// <summary>
        /// Retailer Address
        /// </summary>
        [Required]
        public AddressDto Address { get; set; }

        /// <summary>
        /// List of supported services
        /// </summary>
        [Required]
        public List<ServiceType> Services { get; set; }

        /// <summary>
        /// Assigned Referral Code
        /// </summary>
        [Required]
        public string ReferralCode { get; set; }

        /// <summary>
        /// Date since this retailer is active
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Date till when retailer is active
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        public new RetailerContainerType Type
        {
            get
            {
                return RetailerContainerType.Retailer;
            }
            set
            {
                // No-Op
            }
        }

        /// <summary>
        /// Additional attributes as a property bag
        /// </summary>
        public Dictionary<string, string> AdditionalAttibutes { get; set; }

        public bool IsActive()
        {
            var now = DateTime.UtcNow;
            return StartDate < now && EndDate > now;
        }

        /// <summary>
        /// Util method to create the PartnerID from partner code and partner-provided ID
        /// </summary>
        /// <param name="partnerCode"></param>
        /// <param name="partnerProvidedId"></param>
        public static string CreatePartnerId(string partnerCode, string partnerProvidedId)
        {
            return $"{partnerCode}-{partnerProvidedId}";
        }
    }

    public class RetailerWithDistanceDto
    {
        /// <summary>
        /// Retailer entity
        /// </summary>
        public RetailerDto Retailer { get; set; }

        /// <summary>
        /// Distance (in meters) from the queried location
        /// </summary>
        public double DistanceMeters { get; set; }
    }
}