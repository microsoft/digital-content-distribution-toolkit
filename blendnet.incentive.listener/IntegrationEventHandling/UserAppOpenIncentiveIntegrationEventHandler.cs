using blendnet.common.dto.Events;
using blendnet.common.dto.Incentive;
using blendnet.common.infrastructure;
using blendnet.incentive.listener.Util;
using blendnet.incentive.repository.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace blendnet.incentive.listener.IntegrationEventHandling
{
    /// <summary>
    /// Handles the User App Open event
    /// </summary>
    public class UserAppOpenIncentiveIntegrationEventHandler : IIntegrationEventHandler<UserAppOpenIncentiveIntegrationEvent>
    {
        private readonly UserIntegrationEventHandler _userIntegrationEventHandler;

        public UserAppOpenIncentiveIntegrationEventHandler(ILogger<UserAppOpenIncentiveIntegrationEventHandler> logger,
                                                            TelemetryClient tc,
                                                            IEventRepository eventRepository,
                                                            UserIntegrationEventHandler userIntegrationEventHandler,
                                                            IIncentiveRepository incentiveRepository)
        {
            _userIntegrationEventHandler = userIntegrationEventHandler;
        }

        /// <summary>
        /// Handle the incentive event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(UserAppOpenIncentiveIntegrationEvent integrationEvent)
        {
            const string operationName = "UserAppOpenIncentiveIntegrationEventHandler.Handle";
            IncentiveEvent incentiveEvent = IncentiveUtil.CreateUserIncentiveEvent(integrationEvent.UserPhone, 
                                                                                        integrationEvent.UserId, 
                                                                                        EventType.CONSUMER_INCOME_APP_ONCE_OPEN, 
                                                                                        integrationEvent.OriginalTime);
            await _userIntegrationEventHandler.Handle(incentiveEvent, operationName);
        }
    }
}
