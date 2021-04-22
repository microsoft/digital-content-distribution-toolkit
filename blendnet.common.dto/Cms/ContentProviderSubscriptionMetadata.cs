using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto
{
    public class ContentProviderSubscriptionMetadataDto
    {
        /// <summary>
        /// Subscription Id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid? Id { get; set; }

        /// <summary>
        /// Content Provider ID
        /// </summary>
        public Guid ContentProviderId { get; set; }

        public ContentProviderContainerType Type { get; set; } = ContentProviderContainerType.SubscriptionMetadata;

        public string Title { get; set; }

        public int DurationDays { get; set; }

        public float Price { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public void SetIdentifiers()
        {
            this.Id = Guid.NewGuid();
        }
    }
}
