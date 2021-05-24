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
    /// MomOrder Completed Integration EventHandler
    /// </summary>
    public class MomOrderCompletedIntegrationEventHandler : IIntegrationEventHandler<MomOrderCompletedIntegrationEvent>
    {
        private MomOrderIntegrationEventHandler _momOrderIntegrationEventHandler;

        /// <summary>
        /// Mom Order Completed Integration Event Handler
        /// </summary>
        /// <param name="momOrderIntegrationEventHandler"></param>
        public MomOrderCompletedIntegrationEventHandler(MomOrderIntegrationEventHandler momOrderIntegrationEventHandler)
        {
            _momOrderIntegrationEventHandler = momOrderIntegrationEventHandler;
        }
        /// <summary>
        /// Handle Mom Order Completed Integration Event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(MomOrderCompletedIntegrationEvent integrationEvent)
        {
            await _momOrderIntegrationEventHandler.Handle(integrationEvent,
                                                          "MomOrderCompletedIntegrationEventHandler.Handle", 
                                                          integrationEvent.EventName, 
                                                          ContentBroadcastStatus.BroadcastOrderComplete);
        }
    }
}
