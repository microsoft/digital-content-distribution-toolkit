// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;

namespace blendnet.common.dto.Device
{
    public class DeviceContentByContentProviderIdResponse
    {
       
        /// <summary>
        /// Content id
        /// </summary>
        public Guid ContentId { get; set; }

        /// <summary>
        /// Content provider id
        /// </summary>
        public Guid? ContentProviderId { get; set; }
    }
}
