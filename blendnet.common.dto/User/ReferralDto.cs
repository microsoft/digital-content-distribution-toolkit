using System;

namespace blendnet.common.dto.User
{
    public class ReferralDto
    {
        /// <summary>
        /// ID of the retailer who referred the customer
        /// </summary>
        public Guid RetailerId { get; set; }

        /// <summary>
        /// Partner ID of the retailer who referred the customer
        /// </summary>
        public string RetailerPartnerId { get; set; }

        /// <summary>
        /// Referral code
        /// </summary>
        public string RetailerReferralCode { get; set; }

        /// <summary>
        /// Date of Referral assigned to the customer
        /// </summary>
        public int? ReferralDate { get; set; }

        /// <summary>
        /// Date and time of Referral assigned to the customer
        /// </summary>
        public DateTime ReferralDateTime { get; set; }
    }
}
