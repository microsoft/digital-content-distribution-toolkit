// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Device
{
    public class IOTModuleCommandResponse<I,O>
    {
        /// <summary>
        /// The payload for the device command
        /// </summary>
        public I Request { get; set; }

        /// <summary>
        /// The payload for the device response
        /// </summary>
        public O Response { get; set; }

        public int ConnectionTimeout { get; set; }

        public int ResponseTimeout { get; set; }

        public int ResponseCode { get; set; }

    }
}
