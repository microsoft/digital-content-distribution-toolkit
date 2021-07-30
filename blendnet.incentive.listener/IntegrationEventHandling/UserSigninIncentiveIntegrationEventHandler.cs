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
    /// Handles the User Sign in event
    /// </summary>
    public class UserSigninIncentiveIntegrationEventHandler : IIntegrationEventHandler<UserSigninIncentiveIntegrationEvent>
    {
        private readonly UserIntegrationEventHandler _userIntegrationEventHandler;

        public UserSigninIncentiveIntegrationEventHandler(UserIntegrationEventHandler userIntegrationEventHandler)
        {
            _userIntegrationEventHandler = userIntegrationEventHandler;
        }

        /// <summary>
        /// Handle the integration event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(UserSigninIncentiveIntegrationEvent integrationEvent)
        {
            const string operationName = "UserSigninIncentiveIntegrationEventHandler.Handle";

            await _userIntegrationEventHandler.Handle(integrationEvent.IncentiveEvent, operationName);
        }
    }
}
