using blendnet.common.dto.AIEvents;

namespace blendnet.cms.api.Model
{
    /// <summary>
    /// Base Content Provider AI Event
    /// </summary>
    public class BaseContentProviderAIEvent: BaseAIEvent
    {
        /// <summary>
        /// Content provider id
        /// </summary>
        public Guid ContentProviderId { get; set; }

    }
}
