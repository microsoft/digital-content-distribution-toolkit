using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using blendnet.user.listener.Common;

namespace blendnet.user.listener.IntegrationEventHandling
{
    /// <summary>
    /// Handler User Data Export Completed
    /// </summary>
    public class UserDataExportCompletedIntegrationEventHandler : IIntegrationEventHandler<UserDataExportCompletedIntegrationEvent>
    {
        private DataExportCompletedIntegrationEventHandler _dataExportCompletedIntegrationEventHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataExportCompletedIntegrationEventHandler"></param>
        public UserDataExportCompletedIntegrationEventHandler(DataExportCompletedIntegrationEventHandler dataExportCompletedIntegrationEventHandler)
        {
            _dataExportCompletedIntegrationEventHandler = dataExportCompletedIntegrationEventHandler;
        }

        /// <summary>
        /// Handle the completed event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(UserDataExportCompletedIntegrationEvent integrationEvent)
        {
            await _dataExportCompletedIntegrationEventHandler.Handle(integrationEvent, "UserDataExportCompletedIntegrationEventHandler.Handle");
        }
    }
}
