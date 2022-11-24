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
    /// Content Transform Integration Event
    /// </summary>
    public class ContentTransformIntegrationEvent: IntegrationEvent
    {
        public ContentCommand ContentTransformCommand { get; set; }
    }
}
