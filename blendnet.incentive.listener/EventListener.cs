using blendnet.common.dto.Events;
using blendnet.common.dto.Incentive;
using blendnet.common.infrastructure;
using blendnet.incentive.listener.IntegrationEventHandling;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace blendnet.incentive.listener
{
    public class EventListener : IHostedService
    {
        private readonly ILogger<EventListener> _logger;

        private readonly IEventBus _eventBus;

        private readonly IncentiveAppSettings _appSettings;

        public EventListener(ILogger<EventListener> logger,
                                IEventBus eventBus,
                                IOptionsMonitor<IncentiveAppSettings> optionsMonitor)
        {
            _logger = logger;

            _eventBus = eventBus;

            _appSettings = optionsMonitor.CurrentValue;
        }

        /// <summary>
        /// Start Async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Eventlistner of blendnet.incentive.listener");

            _eventBus.Subscribe<OrderCompletedIntegrationEvent, OrderCompletedEventIntegrationEventHandler>();

            _eventBus.Subscribe<RetailerAssignedIntegrationEvent, RetailerAssignedIntegrationEventHandler>();

            await _eventBus.StartProcessing();

            _logger.LogInformation("Subscribe complete by blendnet.incentive.listener");

        }

        /// <summary>
        /// Stop Async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Eventlistner of blendnet.incentive.listener");

            return Task.CompletedTask;
        }
    }
}
