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
    /// MomOrder Created Integration EventHandler
    /// </summary>
    public class MomOrderCreatedIntegrationEventHandler : IIntegrationEventHandler<MomOrderCreatedIntegrationEvent>
    {
        private MomOrderIntegrationEventHandler _momOrderIntegrationEventHandler;

        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        /// <summary>
        /// Mom Order Created Integration Event Handler
        /// </summary>
        /// <param name="momOrderIntegrationEventHandler"></param>
        public MomOrderCreatedIntegrationEventHandler(ILogger<MomOrderIntegrationEventHandler> logger,
                                                      TelemetryClient tc,
                                                      MomOrderIntegrationEventHandler momOrderIntegrationEventHandler)
        {
            _momOrderIntegrationEventHandler = momOrderIntegrationEventHandler;

            _logger = logger;

            _telemetryClient = tc;
        }
        /// <summary>
        /// Handle Mom Order Created Integration Event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public Task Handle(MomOrderCreatedIntegrationEvent integrationEvent)
        {
            //await _momOrderIntegrationEventHandler.Handle(integrationEvent,
            //                                              "MomOrderCreatedIntegrationEventHandler.Handle", 
            //                                              integrationEvent.EventName, 
            //                                              ContentBroadcastStatus.BroadcastOrderCreated);

            using (_telemetryClient.StartOperation<RequestTelemetry>("MomOrderCreatedIntegrationEventHandler.Handle"))
            {
                _logger.LogInformation($"Broadcast MomOrderCreated status message recieved for command id: {integrationEvent.CorrelationId} Message Body : {integrationEvent.Body}");
            }

            return Task.CompletedTask;
        }
    }
}
