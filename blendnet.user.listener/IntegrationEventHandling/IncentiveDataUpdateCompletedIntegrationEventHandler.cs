// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using blendnet.user.listener.Common;

namespace blendnet.user.listener.IntegrationEventHandling
{
    /// <summary>
    /// Handle Incentive Data Update Completed
    /// </summary>
    public class IncentiveDataUpdateCompletedIntegrationEventHandler : IIntegrationEventHandler<IncentiveDataUpdateCompletedIntegrationEvent>
    {
        private DataUpdateCompletedIntegrationEventHandler _dataUpdateCompletedIntegrationEventHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataExportCompletedIntegrationEventHandler"></param>
        public IncentiveDataUpdateCompletedIntegrationEventHandler(DataUpdateCompletedIntegrationEventHandler dataUpdateCompletedIntegrationEventHandler)
        {
            _dataUpdateCompletedIntegrationEventHandler = dataUpdateCompletedIntegrationEventHandler;
        }

        /// <summary>
        /// Handle the completed event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(IncentiveDataUpdateCompletedIntegrationEvent integrationEvent)
        {
            await _dataUpdateCompletedIntegrationEventHandler.Handle(integrationEvent, "IncentiveDataUpdateCompletedIntegrationEventHandler.Handle");
        }
    }
}
