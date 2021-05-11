using blendnet.common.dto.Events;
using blendnet.common.dto.Retailer;
using blendnet.common.infrastructure;
using blendnet.retailer.listener.IntegrationEventHandling;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace blendnet.retailer.listener
{
    public class EventListener: IHostedService
    {
        private readonly ILogger<EventListener> _logger;

        private readonly IEventBus _eventBus;

        private readonly RetailerAppSettings _appSettings;

        public EventListener(ILogger<EventListener> logger,
                                IEventBus eventBus,
                                IOptionsMonitor<RetailerAppSettings> optionsMonitor)
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
            _logger.LogInformation("Starting Eventlistner of blendnet.retailer.listener");

            _eventBus.Subscribe<RetailerCreatedIntegrationEvent, RetailerCreatedIntegrationEventHandler>();

            await _eventBus.StartProcessing();

            _logger.LogInformation("Subscribe complete by blendnet.retailer.listener");

        }

        /// <summary>
        /// Stop Async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Eventlistner of blendnet.retailer.listener");

            return Task.CompletedTask;
        }
    }
}
