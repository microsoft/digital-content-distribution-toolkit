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
using blendnet.common.dto;

namespace blendnet.incentive.listener.IntegrationEventHandling
{
    /// <summary>
    /// Handles the User Streamed Content event
    /// </summary>
    public class UserStreamContentIncentiveIntegrationEventHandler : IIntegrationEventHandler<UserStreamContentIncentiveIntegrationEvent>
    {
        private readonly UserIntegrationEventHandler _userIntegrationEventHandler;

        public UserStreamContentIncentiveIntegrationEventHandler(UserIntegrationEventHandler userIntegrationEventHandler)
        {
            _userIntegrationEventHandler = userIntegrationEventHandler;
        }

        /// <summary>
        /// Insert the incentive event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(UserStreamContentIncentiveIntegrationEvent integrationEvent)
        {
            const string operationName = "UserStreamContentIncentiveIntegrationEventHandler.Handle";

            await _userIntegrationEventHandler.Handle(integrationEvent.IncentiveEvent, operationName);
        }
    }
}
