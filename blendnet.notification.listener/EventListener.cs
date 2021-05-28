using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using blendnet.oms.listener.IntegrationEventHandling;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace blendnet.oms.listener
{
    public class EventListener : IHostedService
    {
        private readonly ILogger<EventListener> _logger;

        private readonly IEventBus _eventBus;

        public EventListener(ILogger<EventListener> logger,
                                IEventBus eventBus)
        {
            _logger = logger;

            _eventBus = eventBus;
        }

        /// <summary>
        /// Start Async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Eventlistner of blendnet.oms.listener");

            _eventBus.Subscribe<OrderCompletedIntegrationEvent, OrderCompleteEventHandler>();

            await _eventBus.StartProcessing();

            _logger.LogInformation("Subscribe complete by blendnet.oms.listener");
        }

        /// <summary>
        /// Stop Async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Eventlistner of blendnet.oms.listener");

            return Task.CompletedTask;
        }
    }
}
