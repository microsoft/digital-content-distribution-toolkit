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
    /// Content Delete Integration Event
    /// </summary>
    public class ContentDeletedIntegrationEvent: IntegrationEvent
    {
        public ContentCommand ContentDeleteCommand { get; set; }
    }
}
