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
    /// Event for Sending the TAR to SES
    /// </summary>
    public class ContentBroadcastIntegrationEvent: IntegrationEvent
    {
        public ContentCommand ContentBroadcastCommand { get; set; }
    }
}
