using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.incentive.listener.IntegrationEventHandling
{
    /// <summary>
    /// Handles the retailed assigned to consumer
    /// </summary>
    public class RetailerAssignedIntegrationEventHandler : IIntegrationEventHandler<RetailerAssignedIntegrationEvent>
    {
        /// <summary>
        /// Insert the incentive event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public Task Handle(RetailerAssignedIntegrationEvent integrationEvent)
        {
            throw new NotImplementedException();
        }
    }
}
