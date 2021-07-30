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
    public class UserOnboardingRatingSubmittedIncentiveIntegrationEventHandler : IIntegrationEventHandler<UserOnbrdngRtngSbmttdIncentiveIntegrationEvent>
    {
        private readonly UserIntegrationEventHandler _userIntegrationEventHandler;

        public UserOnboardingRatingSubmittedIncentiveIntegrationEventHandler(UserIntegrationEventHandler userIntegrationEventHandler)
        {
            _userIntegrationEventHandler = userIntegrationEventHandler;
        }

        /// <summary>
        /// Handle the integration event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(UserOnbrdngRtngSbmttdIncentiveIntegrationEvent integrationEvent)
        {
            const string operationName = "UserOnboardingRatingSubmittedIncentiveIntegrationEventHandler.Handle";

            await _userIntegrationEventHandler.Handle(integrationEvent.IncentiveEvent, operationName);
        }
    }
}
