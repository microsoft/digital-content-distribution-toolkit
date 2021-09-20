using blendnet.common.dto.Device;
using blendnet.common.dto.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Events
{
    public class TelemetryCommandIntegrationEvent: IntegrationEvent
    {
        public string applicationId { get; set; }
  
        public string deviceId { get; set; }

        public string messageSource { get; set; }
        
        public string templateId { get; set; }

        public DateTime enqueuedTime { get; set; }
    
        public string module { get; set; }

        public Telemetry telemetry { get; set; }
  
    }

    public class Telemetry
    {
        public string DeviceIdInData { get; set; }
        
        public string ApplicationName { get; set; }

        public string ApplicationVersion { get; set; }

        public TelemetryCommand TelemetryCommandData { get; set; }

    }


}
