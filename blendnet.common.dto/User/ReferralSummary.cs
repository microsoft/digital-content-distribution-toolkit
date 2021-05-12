using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.User
{
    /// <summary>
    /// This class stores the aggregated data of referral for given retailer in the mentioned duration
    /// </summary>
    public class ReferralSummary
    {
        /// <summary>
        /// Retailer phone number
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Referral added date
        /// </summary>
        public int? Date { get; set; }
    }
}
