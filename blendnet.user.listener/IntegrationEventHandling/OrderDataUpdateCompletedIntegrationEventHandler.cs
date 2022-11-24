// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using blendnet.user.listener.Common;

namespace blendnet.user.listener.IntegrationEventHandling
{
    /// <summary>
    /// Order Data Update Completed Integration Event Handler
    /// </summary>
    public class OrderDataUpdateCompletedIntegrationEventHandler : IIntegrationEventHandler<OrderDataUpdateCompletedIntegrationEvent>
    {
        private DataUpdateCompletedIntegrationEventHandler _dataUpdateCompletedIntegrationEventHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataExportCompletedIntegrationEventHandler"></param>
        public OrderDataUpdateCompletedIntegrationEventHandler(DataUpdateCompletedIntegrationEventHandler dataUpdateCompletedIntegrationEventHandler)
        {
            _dataUpdateCompletedIntegrationEventHandler = dataUpdateCompletedIntegrationEventHandler;
        }

        /// <summary>
        /// Handle the completed event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(OrderDataUpdateCompletedIntegrationEvent integrationEvent)
        {
            await _dataUpdateCompletedIntegrationEventHandler.Handle(integrationEvent, "OrderDataUpdateCompletedIntegrationEventHandler.Handle");
        }
    }
}
