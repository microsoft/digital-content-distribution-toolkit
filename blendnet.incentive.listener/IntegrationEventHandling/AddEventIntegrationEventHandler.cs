using blendnet.api.proxy.KaizalaIdentity;
using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using blendnet.incentive.repository.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace blendnet.incentive.listener.IntegrationEventHandling
{
    public class AddEventIntegrationEventHandler : IIntegrationEventHandler<AddEventIntegrationEvent>
    {

        private readonly IIncentiveRepository _incentiveRepository;

        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        public AddEventIntegrationEventHandler(ILogger<AddEventIntegrationEventHandler> logger,
                                                        TelemetryClient tc,
                                                        IIncentiveRepository incentiveRepository)
        {
            _incentiveRepository = incentiveRepository;
            _logger = logger;
            _telemetryClient = tc;
        }

        /// <summary>
        /// Handle AddEvent
        /// Insert a record in Event collection
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(AddEventIntegrationEvent integrationEvent)
        {
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>("AddEventIntegrationEventHandler.Handle"))
                {
                    _logger.LogInformation($"Ading event");
                    await _incentiveRepository.AddEvent(integrationEvent.Event);
                    _logger.LogInformation($"Done adding event");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"AddEventIntegrationEventHandler.Handle failed ");
            }
        }
    }
}
