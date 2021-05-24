using blendnet.common.dto.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Events
{
    [EventDetails(  EventName = ApplicationConstants.BroadcastJobStatuses.MOMORDERACTIVE, 
                    PerformJsonSerialization =false)]
    public class MomOrderActiveIntegrationEvent: IntegrationEvent
    {
        public override string EventName
        {
            get
            {
                return ApplicationConstants.BroadcastJobStatuses.MOMORDERACTIVE;
            }
        }
    }
}
