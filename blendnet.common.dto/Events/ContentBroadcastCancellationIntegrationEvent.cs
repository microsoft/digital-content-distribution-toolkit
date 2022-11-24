// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Cms;
using blendnet.common.dto.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.dto.Events
{
    /// <summary>
    /// Event to handle content broadcast cancellation
    /// </summary>
    public class ContentBroadcastCancellationIntegrationEvent: IntegrationEvent
    {
        public ContentCommand ContentBroadcastCancellationCommand { get; set; }
    }
}
