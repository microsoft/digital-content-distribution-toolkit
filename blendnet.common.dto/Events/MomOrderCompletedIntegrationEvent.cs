using blendnet.common.dto.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Events
{
    [EventDetails(  EventName = ApplicationConstants.BroadcastJobStatuses.MOMORDERCOMPLETED, 
                    PerformJsonSerialization =false)]
    public class MomOrderCompletedIntegrationEvent: IntegrationEvent
    {
        public override string EventName
        {
            get
            {
                return ApplicationConstants.BroadcastJobStatuses.MOMORDERCOMPLETED;
            }
        }
    }
}
