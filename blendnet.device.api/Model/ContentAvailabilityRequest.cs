// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Device;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.device.api.Model
{
    public class ContentAvailabilityRequest
    {
        [Required]
        public Guid ContentId { get; set; }

        [Required]
        public List<string> DeviceIds { get; set; }

        [Required]
        public bool IncludeActiveContentCount { get; set; } = false;

    }


    public class ContentAvailabilityCountRequest
    {
        [Required]
        public List<string> DeviceIds { get; set; }
    }
}
