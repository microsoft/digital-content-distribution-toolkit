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
    /// MomOrder Rejected Integration EventHandler
    /// </summary>
    public class MomOrderRejectedIntegrationEventHandler : IIntegrationEventHandler<MomOrderRejectedIntegrationEvent>
    {
        private MomOrderIntegrationEventHandler _momOrderIntegrationEventHandler;

        /// <summary>
        /// Mom Order Rejected Integration Event Handler
        /// </summary>
        /// <param name="momOrderIntegrationEventHandler"></param>
        public MomOrderRejectedIntegrationEventHandler(MomOrderIntegrationEventHandler momOrderIntegrationEventHandler)
        {
            _momOrderIntegrationEventHandler = momOrderIntegrationEventHandler;
        }
        /// <summary>
        /// Handle Mom Order Rejected Integration Event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(MomOrderRejectedIntegrationEvent integrationEvent)
        {
            await _momOrderIntegrationEventHandler.Handle(integrationEvent,
                                                          "MomOrderRejectedIntegrationEventHandler.Handle", 
                                                          integrationEvent.EventName, 
                                                          ContentBroadcastStatus.BroadcastOrderRejected);
        }
    }
}
