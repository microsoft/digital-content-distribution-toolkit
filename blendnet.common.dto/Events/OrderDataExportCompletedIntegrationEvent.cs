using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Events
{
    /// <summary>
    /// Order Data Export Completed Integration Event
    /// </summary>
    public class OrderDataExportCompletedIntegrationEvent:BaseDataExportCompletedIntegrationEvent
    {
        /// <summary>
        /// return service name
        /// </summary>
        public override string ServiceName 
        { 
            get
            {
                return ApplicationConstants.BlendNetServices.OMSService;
            }
        }
    }
}
