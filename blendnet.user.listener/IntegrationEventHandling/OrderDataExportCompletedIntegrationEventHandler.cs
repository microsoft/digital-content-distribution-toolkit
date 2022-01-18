using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using blendnet.user.listener.Common;

namespace blendnet.user.listener.IntegrationEventHandling
{
    /// <summary>
    /// Handle Order Data Export Completed
    /// </summary>
    public class OrderDataExportCompletedIntegrationEventHandler : IIntegrationEventHandler<OrderDataExportCompletedIntegrationEvent>
    {
        private DataExportCompletedIntegrationEventHandler _dataExportCompletedIntegrationEventHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataExportCompletedIntegrationEventHandler"></param>
        public OrderDataExportCompletedIntegrationEventHandler(DataExportCompletedIntegrationEventHandler dataExportCompletedIntegrationEventHandler)
        {
            _dataExportCompletedIntegrationEventHandler = dataExportCompletedIntegrationEventHandler;
        }

        /// <summary>
        /// Handle the completed event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(OrderDataExportCompletedIntegrationEvent integrationEvent)
        {
            await _dataExportCompletedIntegrationEventHandler.Handle(integrationEvent, "OrderDataExportCompletedIntegrationEventHandler.Handle");
        }
    }
}
