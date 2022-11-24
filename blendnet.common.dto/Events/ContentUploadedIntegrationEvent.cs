// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Cms;
using blendnet.common.dto.Integration;
using System;
using System.Collections.Generic;
using System.Text;

namespace blendnet.common.dto.Events
{
    /// <summary>
    /// Content Uploaded Integration Event
    /// </summary>
    public class ContentUploadedIntegrationEvent: IntegrationEvent
    {
        public ContentCommand ContentUploadCommand { get; set; }
    }
}
