
using System;
using System.Collections.Generic;
using blendnet.common.dto;
using blendnet.common.dto.Retailer;

namespace blendnet.retailer.api.Models
{
    public class RetailerResponse
    {
        /// <summary>
        /// Id of the retailer
        /// </summary>
        /// <value></value>
        public string RetailerId { get; set; }

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
        public string PartnerProvidedId { get; set; }

        /// <summary>
        /// Globally Unique ID for the retailer
        /// This is a combination of Partner Code and partner-provided ID
        /// </summary>
        public string PartnerId { get; set; }

        /// <summary>
        /// Partner who onboarded this retailer
        /// </summary>
        public string PartnerCode { get; set; }

        /// <summary>
        /// Retailer Address
        /// </summary>
        public AddressDto Address { get; set; }

        /// <summary>
        /// List of supported services
        /// </summary>
        public List<ServiceType> Services { get; set; }

        /// <summary>
        /// Assigned Referral Code
        /// </summary>
        public string ReferralCode { get; set; }

        /// <summary>
        /// Date since this retailer is active
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Date till when retailer is active
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Additional attributes as a property bag
        /// </summary>
        public Dictionary<string, string> AdditionalAttibutes { get; set; }

        /// <summary>
        /// List of device assignments to the retailer
        /// </summary>
        public List<RetailerDeviceAssignment> DeviceAssignments { get; set; } = new List<RetailerDeviceAssignment>();

        public bool HasActiveDevice { get; set; }
    }

    public class RetailerWithDistanceResponse
    {
        /// <summary>
        /// Retailer entity
        /// </summary>
        public RetailerResponse Retailer { get; set; }

        /// <summary>
        /// Distance (in meters) from the queried location
        /// </summary>
        public double DistanceMeters { get; set; }
    }
}