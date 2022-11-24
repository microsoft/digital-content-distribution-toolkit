// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Integration;
using System;
using System.Collections.Generic;
using System.Text;

namespace blendnet.common.dto.Events
{
    public class ContentProviderUpdatedIntegrationEvent: IntegrationEvent
    {
        public ContentProviderDto BeforeUpdateContentProvider { get; set; }

        public ContentProviderDto AfterUpdateContentProvider { get; set; }
    }
}
