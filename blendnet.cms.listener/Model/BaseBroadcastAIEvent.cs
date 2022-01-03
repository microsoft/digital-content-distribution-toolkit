using blendnet.common.dto.AIEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.listener.Model
{
    /// <summary>
    /// Base Broadcast Event
    /// </summary>
    public class BaseBroadcastAIEvent:BaseAIEvent
    {
        /// <summary>
        /// Command Id for Broadcasting or Cancellation
        /// </summary>
        public Guid CommandId { get; set; }

        /// <summary>
        /// Unique Content Id
        /// </summary>
        public Guid ContentId { get; set; }

        /// <summary>
        /// Content Provider Id
        /// </summary>
        public Guid ContentProviderId { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Short Description of the media content
        /// </summary>
        public string ShortDescription { get; set; }
    }
}
