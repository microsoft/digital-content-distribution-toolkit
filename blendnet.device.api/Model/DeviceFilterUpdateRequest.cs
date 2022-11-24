// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.device.api.Model
{
    /// <summary>
    /// Device Filter Update Request
    /// </summary>
    public class DeviceFilterUpdateRequest
    {
        [Required]
        public List<string> DeviceIds { get; set; }

        [Required]
        public List<string> Filters { get; set; }

    }
}
