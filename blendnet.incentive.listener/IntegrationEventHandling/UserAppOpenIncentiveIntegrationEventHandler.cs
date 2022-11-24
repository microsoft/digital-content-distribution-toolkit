// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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

            await _userIntegrationEventHandler.Handle(integrationEvent.IncentiveEvent, operationName);
        }
    }
}
