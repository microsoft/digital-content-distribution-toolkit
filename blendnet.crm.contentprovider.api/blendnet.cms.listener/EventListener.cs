using blendnet.cms.listener.IntegrationEventHandling;
using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace blendnet.cms.listener
{
    /// <summary>
    /// Event Listner
    /// </summary>
    public class EventListener : IHostedService
    {
        private readonly ILogger<EventListener> _logger;

        private readonly IEventBus _eventBus;
        
        public EventListener(ILogger<EventListener> logger, IEventBus eventBus)
        {
            _logger = logger;

            _eventBus = eventBus;

        }

        /// <summary>
        /// Start Async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Eventlistner of blendnet.cms.listener");

            _eventBus.Subscribe<ContentProviderCreatedIntegrationEvent,ContentProviderCreatedIntegrationEventHandler>();
            
            _logger.LogInformation("Subscribe complete by blendnet.cms.listener");

            return Task.CompletedTask;
        }

        /// <summary>
        /// Stop Async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Eventlistner of blendnet.cms.listener");

            return Task.CompletedTask;
        }
    }
}
