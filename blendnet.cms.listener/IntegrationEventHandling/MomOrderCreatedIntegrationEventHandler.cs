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
    /// MomOrder Created Integration EventHandler
    /// </summary>
    public class MomOrderCreatedIntegrationEventHandler : IIntegrationEventHandler<MomOrderCreatedIntegrationEvent>
    {
        /// <summary>
        /// Handle Mom Order Created Integration Event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public Task Handle(MomOrderCreatedIntegrationEvent integrationEvent)
        {
            throw new NotImplementedException();
        }
    }
}
