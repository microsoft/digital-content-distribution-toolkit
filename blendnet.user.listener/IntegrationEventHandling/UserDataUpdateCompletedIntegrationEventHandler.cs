// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using blendnet.user.listener.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.user.listener.IntegrationEventHandling
{
    /// <summary>
    /// User Data Update Completed Integration Event Handler
    /// </summary>
    public class UserDataUpdateCompletedIntegrationEventHandler : IIntegrationEventHandler<UserDataUpdateCompletedIntegrationEvent>
    {
        private DataUpdateCompletedIntegrationEventHandler _dataUpdateCompletedIntegrationEventHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataExportCompletedIntegrationEventHandler"></param>
        public UserDataUpdateCompletedIntegrationEventHandler(DataUpdateCompletedIntegrationEventHandler dataUpdateCompletedIntegrationEventHandler)
        {
            _dataUpdateCompletedIntegrationEventHandler = dataUpdateCompletedIntegrationEventHandler;
        }

        /// <summary>
        /// Handle the completed event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(UserDataUpdateCompletedIntegrationEvent integrationEvent)
        {
            await _dataUpdateCompletedIntegrationEventHandler.Handle(integrationEvent, "UserDataUpdateCompletedIntegrationEventHandler.Handle");
        }
    }
}
