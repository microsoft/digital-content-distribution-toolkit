using blendnet.common.dto.Integration;
using System;
using blendnet.common.dto.Cms;

namespace blendnet.common.dto.Events
{
    /// <summary>
    /// Base class for User Incentive events
    /// </summary>
    public abstract class BaseUserIncentiveIntegrationEvent : IntegrationEvent
    {
        /// <summary>
        /// User ID (from KMS)
        /// </summary>
        /// <value></value>
        public Guid UserId { get; set; }

        /// <summary>
        /// Phone number of the user
        /// </summary>
        /// <value></value>
        public string UserPhone { get; set; }

        /// <summary>
        /// Original time of playing the content
        /// </summary>
        /// <value></value>
        public DateTime OriginalTime { get; set; }
    }
}