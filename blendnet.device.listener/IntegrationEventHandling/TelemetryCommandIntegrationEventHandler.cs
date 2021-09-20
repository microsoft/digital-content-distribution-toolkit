using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.device.listener.IntegrationEventHandling
{
    public class TelemetryCommandIntegrationEventHandler : IIntegrationEventHandler<TelemetryCommandIntegrationEvent>
    {
        /// <summary>
        /// To handle the command recieved via IOT Central
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public Task Handle(TelemetryCommandIntegrationEvent integrationEvent)
        {
            throw new NotImplementedException();
        }
    }
}
