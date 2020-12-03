using blendnet.common.dto.Integration;
using System;
using System.Collections.Generic;
using System.Text;

namespace blendnet.common.dto.Events
{
    public class ContentProviderCreatedIntegrationEvent: IntegrationEvent
    {
        public ContentProviderDto ContentProvider { get; set; }

        public string ContainerBaseName { get; set; }
    }
}
