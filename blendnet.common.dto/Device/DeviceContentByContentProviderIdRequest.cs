// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace blendnet.common.dto.Device
{
    /// <summary>
    /// Request class for getting device contents of content providers
    /// </summary
    public class DeviceContentByContentProviderIdRequest
    {
        /// <summary>
        /// List of Content Provider IDs
        /// </summary>
        [Required]
        public List<Guid> ContentProviderIds { get; set; }

    }
}
