// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Events
{
    [EventDetails(  EventName = ApplicationConstants.BroadcastJobStatuses.MOMORDERCREATED, 
                    PerformJsonSerialization =false)]
    public class MomOrderCreatedIntegrationEvent: IntegrationEvent
    {
        public override string EventName
        {
            get
            {
                return ApplicationConstants.BroadcastJobStatuses.MOMORDERCREATED;
            }
        }
    }
}
