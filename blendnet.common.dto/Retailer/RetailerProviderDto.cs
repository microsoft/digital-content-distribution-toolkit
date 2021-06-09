using Newtonsoft.Json;
using System;

namespace blendnet.common.dto.Retailer
{
    public class RetailerProviderDto : BaseDto
    {
        /// <summary>
        /// ID of this provider, same as Partner Code
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id => this.PartnerCode;

        /// <summary>
        /// Friendly name of the retailer provider
        /// </summary>
        /// <value></value>
        public string Name { get; set; }
        
        /// <summary>
        /// Assigned partner code (usually 4-character)
        /// </summary>
        /// <value></value>
        public string PartnerCode {get; set;}

        /// <summary>
        /// Service Account ID, this maps to corresponding Kaizala Identity user ID
        /// </summary>
        /// <value></value>
        public Guid ServiceAccountId { get; set;}

        /// <summary>
        /// Start Date of the retailer provider
        /// </summary>
        /// <value></value>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End Date of the retailer provider
        /// </summary>
        /// <value></value>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Container type
        /// </summary>
        public RetailerProviderContainerType Type => RetailerProviderContainerType.RetailerProvider;
    }
}
