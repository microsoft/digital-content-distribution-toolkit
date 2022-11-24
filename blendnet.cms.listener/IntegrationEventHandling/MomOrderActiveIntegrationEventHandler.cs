// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.cms.listener.Common;
using blendnet.common.dto.Cms;
using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.listener.IntegrationEventHandling
{
    /// <summary>
    /// MomOrder Active Integration EventHandler
    /// </summary>
    public class MomOrderActiveIntegrationEventHandler : IIntegrationEventHandler<MomOrderActiveIntegrationEvent>
    {
        private MomOrderIntegrationEventHandler _momOrderIntegrationEventHandler;

        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        /// <summary>
        /// Mom Order Active Integration Event Handler
        /// </summary>
        /// <param name="momOrderIntegrationEventHandler"></param>
        public MomOrderActiveIntegrationEventHandler( ILogger<MomOrderIntegrationEventHandler> logger,
                                                      TelemetryClient tc, 
                                                      MomOrderIntegrationEventHandler momOrderIntegrationEventHandler)
        {
            _momOrderIntegrationEventHandler = momOrderIntegrationEventHandler;

            _logger = logger;

            _telemetryClient = tc;

        }
        /// <summary>
        /// Handle Mom Order Active Integration Event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public Task Handle(MomOrderActiveIntegrationEvent integrationEvent)
        {
            //await _momOrderIntegrationEventHandler.Handle(integrationEvent,
            //                                              "MomOrderActiveIntegrationEventHandler.Handle", 
            //                                              integrationEvent.EventName, 
            //                                              ContentBroadcastStatus.BroadcastOrderActive);

            using (_telemetryClient.StartOperation<RequestTelemetry>("MomOrderActiveIntegrationEventHandler.Handle"))
            {
                _logger.LogInformation($"Broadcast MomOrderActive status message recieved for command id: {integrationEvent.CorrelationId} Message Body : {integrationEvent.Body}");
            } 

            return Task.CompletedTask;
        }
    }
}
