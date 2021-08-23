using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Device
{
    public class RunModuleCommandRequest<I>
    {
        public IOTModuleCommandRequest<I> IOTModuleCommandRequest { get; set; }

        public string DeviceName { get; set; }

        public string ModuleName { get; set; }

        public string CommandName { get; set; }

        public string APIToken { get; set; }

        public string APIVersion { get; set; }
 
    }
}
