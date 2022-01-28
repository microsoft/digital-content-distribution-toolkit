using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.User
{
    public class UserToExport
    {
        /// <summary>
        /// Phone number of the person
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Display name of the person
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Partner ID of the retailer who referred the customer
        /// </summary>
        public string ReferredBy { get; set; }

        /// <summary>
        /// Referral code
        /// </summary>
        public string RetailerReferralCode { get; set; }

        /// <summary>
        /// Date and time of Referral assigned to the customer
        /// </summary>
        public DateTime? ReferralDateTime { get; set; }
    }
}
