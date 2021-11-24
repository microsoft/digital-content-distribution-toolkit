using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Device
{
    /// <summary>
    /// Provides the content availability count on device
    /// </summary>
    public class DeviceContentAvailability
    {
        /// <summary>
        /// Device ID
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Total Active Content
        /// </summary>
        public int? ActiveContentCount { get; set; }
        
    }
}
