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
    public abstract class BaseDataExportCompletedIntegrationEvent:IntegrationEvent
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
        /// True in case there is no data available to export
        /// </summary>
        public bool NoDataToExport { get; set; }

    }
}
