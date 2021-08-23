using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Device
{
    /// <summary>
    /// Filter Update Payload
    /// </summary>
    public class ProxyModuleFilterUpdatePayload
    {
        /// <summary>
        /// Device Id
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Command Id
        /// </summary>
        public Guid CommandId { get; set; }

        /// <summary>
        /// List of filters
        /// </summary>
        public List<string> Filters { get; set; }

    }
}
