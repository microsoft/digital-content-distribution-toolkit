using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto
{
    /// <summary>
    /// Base DTO for all the objects
    /// </summary>
    public class BaseDto
    {
        /// <summary>
        /// https://github.com/Azure/azure-cosmos-dotnet-v3/issues/165
        /// </summary>
        [JsonProperty(PropertyName = "_etag")]
        public string ETag { get; set; }

        public Guid CreatedByUserId { get; set; }

        public Guid? ModifiedByByUserId { get; set; }

        /// <summary>
        /// Created Date
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Modified Date
        /// </summary>
        public DateTime? ModifiedDate { get; set; }
    }
}
