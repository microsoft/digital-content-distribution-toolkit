using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using System.Threading.Tasks;

namespace blendnet.incentive.listener.IntegrationEventHandling
{
    /// <summary>
    /// Handles the Media Download Completed event
    /// </summary>
    public class UserMediaDownloadCompletedIncentiveIntegrationEventHandler : IIntegrationEventHandler<UserMediaDnldCmpltdIncentiveIntegrationEvent>
    {
        private readonly UserIntegrationEventHandler _userIntegrationEventHandler;

        public UserMediaDownloadCompletedIncentiveIntegrationEventHandler(UserIntegrationEventHandler userIntegrationEventHandler)
        {
            _userIntegrationEventHandler = userIntegrationEventHandler;
        }

        /// <summary>
        /// Handle the integration event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(UserMediaDnldCmpltdIncentiveIntegrationEvent integrationEvent)
        {
            const string operationName = "UserMediaDownloadCompletedIncentiveIntegrationEventHandler.Handle";

            await _userIntegrationEventHandler.Handle(integrationEvent.IncentiveEvent, operationName);
            await _userIntegrationEventHandler.Handle(integrationEvent.RetailerIncentiveEvent, operationName);
        }
    }
}
