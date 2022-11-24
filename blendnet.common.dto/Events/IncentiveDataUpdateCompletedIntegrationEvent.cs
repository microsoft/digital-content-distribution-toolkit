// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Events
{
    /// <summary>
    /// Incentive Data Update Complete Integration Event
    /// </summary>
    public class IncentiveDataUpdateCompletedIntegrationEvent: BaseDataOperationCompletedIntegrationEvent
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
