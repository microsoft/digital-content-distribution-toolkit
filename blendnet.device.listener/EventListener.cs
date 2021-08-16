using blendnet.common.dto.Device;
using blendnet.common.infrastructure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace blendnet.device.listener
{
    public class EventListener : IHostedService
    {
        private readonly ILogger<EventListener> _logger;

        private readonly IEventBus _eventBus;

        private readonly DeviceAppSettings _appSettings;

        public EventListener(ILogger<EventListener> logger,
                                IEventBus eventBus,
                                IOptionsMonitor<DeviceAppSettings> optionsMonitor)
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
            _logger.LogInformation("Starting Eventlistner of blendnet.device.listener");

            ////todo : add device based event handler
            //_eventBus.Subscribe<ContentProviderCreatedIntegrationEvent, ContentProviderCreatedIntegrationEventHandler>();

            await _eventBus.StartProcessing();

            _logger.LogInformation("Subscribe complete by blendnet.device.listener");

            //return Task.CompletedTask;
        }

        /// <summary>
        /// Stop Async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Eventlistner of blendnet.device.listener");

            return Task.CompletedTask;
        }
    }
}
