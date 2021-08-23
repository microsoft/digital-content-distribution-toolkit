using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Device
{
    /// <summary>
    /// Proxy Module Command Response
    /// </summary>
    public class ProxyModuleCommandResponse
    {
        /// <summary>
        /// Status 0 or 1
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Result Recieved
        /// </summary>
        public string Result { get; set; }

    }
}
