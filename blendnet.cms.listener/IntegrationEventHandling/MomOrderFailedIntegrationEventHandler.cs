// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.cms.listener.Common;
using blendnet.common.dto.Cms;
using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.listener.IntegrationEventHandling
{
    /// <summary>
    /// MomOrder Failed Integration EventHandler
    /// </summary>
    public class MomOrderFailedIntegrationEventHandler : IIntegrationEventHandler<MomOrderFailedIntegrationEvent>
    {
        private MomOrderIntegrationEventHandler _momOrderIntegrationEventHandler;

        /// <summary>
        /// Mom Order Failed Integration Event Handler
        /// </summary>
        /// <param name="momOrderIntegrationEventHandler"></param>
        public MomOrderFailedIntegrationEventHandler(MomOrderIntegrationEventHandler momOrderIntegrationEventHandler)
        {
            _momOrderIntegrationEventHandler = momOrderIntegrationEventHandler;
        }
        /// <summary>
        /// Handle Mom Order Failed Integration Event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(MomOrderFailedIntegrationEvent integrationEvent)
        {
            await _momOrderIntegrationEventHandler.Handle(integrationEvent,
                                                          "MomOrderFailedIntegrationEventHandler.Handle", 
                                                          integrationEvent.EventName, 
                                                          ContentBroadcastStatus.BroadcastOrderFailed);
        }
    }
}
