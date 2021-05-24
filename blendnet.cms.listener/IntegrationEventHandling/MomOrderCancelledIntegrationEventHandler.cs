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
    /// MomOrder Cancelled Integration EventHandler
    /// </summary>
    public class MomOrderCancelledIntegrationEventHandler : IIntegrationEventHandler<MomOrderCancelledIntegrationEvent>
    {
        private MomOrderIntegrationEventHandler _momOrderIntegrationEventHandler;

        /// <summary>
        /// Mom Order Cancelled Integration Event Handler
        /// </summary>
        /// <param name="momOrderIntegrationEventHandler"></param>
        public MomOrderCancelledIntegrationEventHandler(MomOrderIntegrationEventHandler momOrderIntegrationEventHandler)
        {
            _momOrderIntegrationEventHandler = momOrderIntegrationEventHandler;
        }
        /// <summary>
        /// Handle Mom Order Cancelled Integration Event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(MomOrderCancelledIntegrationEvent integrationEvent)
        {
            await _momOrderIntegrationEventHandler.Handle(integrationEvent,
                                                          "MomOrderCancelledIntegrationEventHandler.Handle", 
                                                          integrationEvent.EventName, 
                                                          ContentBroadcastStatus.BroadcastOrderCancelled);
        }
    }
}
