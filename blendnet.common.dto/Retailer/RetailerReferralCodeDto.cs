using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Retailer
{
    public class RetailerReferralCodeDto
    {
        /// <summary>
        /// Same as Referral Code 
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id
        { 
            get
            {
                return ReferralCode;
            }
            set { }
        }

        public RetailerContainerType Type
        {
            get
            {
                return RetailerContainerType.RetailerReferralCode;
            }
            set
            {
                // No-op
            }
        }

        public string ReferralCode { get; set; }

        public string PartnerId { get; set; }
    }
}
