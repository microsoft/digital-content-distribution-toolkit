using blendnet.common.dto.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Events
{
    /// <summary>
    /// Base class for data export completion event
    /// </summary>
    public abstract class BaseDataOperationCompletedIntegrationEvent:IntegrationEvent
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

        /// <summary>
        /// Service Name
        /// </summary>
        public abstract string ServiceName { get;}

        /// <summary>
        /// Completion Date Time
        /// </summary>
        public DateTime CompletionDateTime { get; set; }

        /// <summary>
        /// True in case there is no data available to operate
        /// </summary>
        public bool NoDataToOperate { get; set; }

        /// <summary>
        /// Whether failed
        /// </summary>
        public bool IsFailed { get; set; } = false;

        /// <summary>
        /// Failure Message (if any)
        /// </summary>
        public string FailureReason { get; set; }

    }
}
