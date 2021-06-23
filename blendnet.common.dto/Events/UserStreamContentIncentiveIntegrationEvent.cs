using blendnet.common.dto.Integration;
using System;
using blendnet.common.dto.Cms;

namespace blendnet.common.dto.Events
{
    public class UserStreamContentIncentiveIntegrationEvent : BaseUserIncentiveIntegrationEvent
    {
        /// <summary>
        /// The content that was played
        /// </summary>
        /// <value></value>
        public Content Content { get; set; }
    }
}