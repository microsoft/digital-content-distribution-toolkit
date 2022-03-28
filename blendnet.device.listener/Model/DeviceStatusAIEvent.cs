using blendnet.common.dto.AIEvents;
using blendnet.common.dto.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.device.listener.Model
{
    /// <summary>
    /// Device Provision AI Event
    /// </summary>
    public class DeviceStatusAIEvent: BaseAIEvent
    {
        /// <summary>
        /// Device Id
        /// </summary>
        public string DeviceId { get; set; }
        

        /// <summary>
        /// Device Status
        /// </summary>
        public DeviceStatus DeviceStatus { get; set; }

        /// <summary>
        /// Device Status Updated On
        /// </summary>
        public DateTime? DeviceStatusUpdatedOn { get; set; }

    }
}
