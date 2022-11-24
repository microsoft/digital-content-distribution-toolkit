// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Device
{
    /// <summary>
    /// Proxy Module Command Request
    /// </summary>
    public class ProxyModuleCommandRequest
    {
        public string ModuleClientName { get; set; }

        public string CommandName { get; set; }

        public string Payload { get; set; }

        public double? ConnectionTimeOutInMts { get; set; }
    }

    
}
