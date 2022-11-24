// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using blendnet.user.listener.Common;

namespace blendnet.user.listener.IntegrationEventHandling
{
    /// <summary>
    /// Handle Incentive Data Export Completed
    /// </summary>
    public class IncentiveDataExportCompletedIntegrationEventHandler : IIntegrationEventHandler<IncentiveDataExportCompletedIntegrationEvent>
    {
        private DataExportCompletedIntegrationEventHandler _dataExportCompletedIntegrationEventHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataExportCompletedIntegrationEventHandler"></param>
        public IncentiveDataExportCompletedIntegrationEventHandler(DataExportCompletedIntegrationEventHandler dataExportCompletedIntegrationEventHandler)
        {
            _dataExportCompletedIntegrationEventHandler = dataExportCompletedIntegrationEventHandler;
        }

        /// <summary>
        /// Handle the completed event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(IncentiveDataExportCompletedIntegrationEvent integrationEvent)
        {
            await _dataExportCompletedIntegrationEventHandler.Handle(integrationEvent, "IncentiveDataExportCompletedIntegrationEventHandler.Handle");
        }
    }
}
