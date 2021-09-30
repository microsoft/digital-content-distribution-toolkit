using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace blendnet.common.dto.Retailer
{
    public class RetailerDto : BaseDto
    {
        /// <summary>
        /// Id of the retailer
        /// </summary>
        /// <value></value>
        [JsonProperty(PropertyName = "id")]
        public string RetailerId => this.PartnerId;

        /// <summary>
        /// Associated User ID (into User table)
        /// </summary>
        /// <value></value>
        public Guid UserId { get; set; }

        /// <summary>
        /// Retailer's phone number (same as that in User table)
        /// </summary>
        /// <value></value>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Name of the retailer
        /// </summary>
        /// <value></value>
        public string Name { get; set; }

        /// <summary>
        /// Original ID provided by the onboarding partner
        /// </summary>
        [Required]
        public string PartnerProvidedId { get; set; }

        /// <summary>
        /// Globally Unique ID for the retailer
        /// This is a combination of Partner Code and partner-provided ID
        /// </summary>
        public string PartnerId => CreatePartnerId(PartnerCode, PartnerProvidedId);

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

        public RetailerContainerType Type => RetailerContainerType.Retailer;

        /// <summary>
        /// Additional attributes as a property bag
        /// </summary>
        public Dictionary<string, string> AdditionalAttibutes { get; set; }

        /// <summary>
        /// List of device assignments to the retailer
        /// </summary>
        public List<RetailerDeviceAssignment> DeviceAssignments { get; set; } = new List<RetailerDeviceAssignment>();

        public bool IsActive()
        {
            var now = DateTime.UtcNow;
            return StartDate < now && EndDate > now;
        }

        [JsonIgnore] // do not put this prop in DB
        public bool HasActiveDevice => this.DeviceAssignments.Exists(assignment => assignment.IsActive);

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