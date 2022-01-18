using blendnet.common.dto.Integration;
using blendnet.common.dto.User;
using System;

namespace blendnet.common.dto.Events
{
    /// <summary>
    /// Export User Data Integration Event
    /// </summary>
    public class ExportUserDataIntegrationEvent: IntegrationEvent
    {
        /// <summary>
        /// User Id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// User Phone Number
        /// </summary>
        public string UserPhoneNumber { get; set; }

        /// <summary>
        /// Command Id
        /// </summary>
        public Guid CommandId { get; set; }
    }
}
