using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Events
{
    /// <summary>
    /// Occurs when the update data of all orders is complete
    /// </summary>
    public class OrderDataUpdateCompletedIntegrationEvent: BaseDataOperationCompletedIntegrationEvent
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
