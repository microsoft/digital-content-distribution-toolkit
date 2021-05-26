﻿using blendnet.common.dto.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Events
{
    [EventDetails(  EventName = ApplicationConstants.BroadcastJobStatuses.MOMORDERFAILED, 
                    PerformJsonSerialization =false)]
    public class MomOrderFailedIntegrationEvent: IntegrationEvent
    {
        public override string EventName
        {
            get
            {
                return ApplicationConstants.BroadcastJobStatuses.MOMORDERFAILED;
            }
        }
    }
}