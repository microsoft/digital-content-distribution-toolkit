using blendnet.common.dto.Integration;
using System;
using System.Collections.Generic;
namespace blendnet.common.dto.Events
{
    /// <summary>
    /// Incentive Data Export Completed Integration Event
    /// </summary>
    public class IncentiveDataExportCompletedIntegrationEvent: BaseDataOperationCompletedIntegrationEvent
    {
        /// <summary>
        /// return service name
        /// </summary>
        public override string ServiceName
        {
            get
            {
                return ApplicationConstants.BlendNetServices.IncentiveService;
            }
        }
    }
}
