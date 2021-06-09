using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace blendnet.common.dto.Incentive
{
    /// <summary>
    /// Defines the audience of event or incentive plan
    /// </summary>
    public class Audience
    {
        /// <summary>
        /// Audience type
        /// </summary>
        public AudienceType AudienceType { get; set; }

        /// <summary>
        /// "Nil GUID" for Consumer, Selected RetailerProvider GUID in case of Retailer
        /// </summary>
        public Guid SubTypeId { get; set; }

        /// <summary>
        /// "All" for Consumer, Selected RP Name in case of Retailer
        /// </summary>
        public string SubTypeName { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AudienceType
    {
        CONSUMER,
        RETAILER
    }
}
