// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.listener.IntegrationEventHandling
{
    public class MicrosoftStorageBlobCreatedIntegrationEventHandler : IIntegrationEventHandler<MicrosoftStorageBlobCreatedIntegrationEvent>
    {
        public Task Handle(MicrosoftStorageBlobCreatedIntegrationEvent integrationEvent)
        {
            return Task.CompletedTask;
        }
    }
}
