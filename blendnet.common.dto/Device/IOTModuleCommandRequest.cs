// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Device
{
    public class IOTModuleCommandRequest<I>
    {
        /// <summary>
        /// The payload for the device command
        /// </summary>
        public I Request { get; set; }

        /// <summary>
        /// Connection timeout in seconds to wait for a disconnected device to come online. 
        /// </summary>
        public int ConnectionTimeout { get; set; }

        /// <summary>
        /// Response timeout in seconds to wait for a command completion on a device
        /// </summary>
        public int ResponseTimeout { get; set; }

    }
}
