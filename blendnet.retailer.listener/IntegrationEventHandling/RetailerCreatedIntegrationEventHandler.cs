using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.retailer.listener.IntegrationEventHandling
{
    public class RetailerCreatedIntegrationEventHandler : IIntegrationEventHandler<RetailerCreatedIntegrationEvent>
    {
        /// <summary>
        /// Handle Retailer Created Event
        /// 1) Insert a record in retailer collection
        /// 2) Assign the retailer role
        /// 3) Refer ContentProviderCreatedIntegrationEventHandler for details
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public Task Handle(RetailerCreatedIntegrationEvent integrationEvent)
        {
            throw new NotImplementedException();
        }
    }
}
