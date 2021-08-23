using blendnet.common.dto.Device;
using blendnet.common.dto.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Events
{
    public class FilterUpdateIntegrationEvent: IntegrationEvent
    {
        public DeviceCommand FilterUpdateCommand { get; set; }
    }
}
