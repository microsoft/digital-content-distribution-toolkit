using blendnet.cms.listener.IntegrationEventHandling;
using blendnet.common.dto.cms;
using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using blendnet.common.infrastructure.ServiceBus;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

        private readonly AppSettings _appSettings;

        public EventListener(   ILogger<EventListener> logger, 
                                IEventBus eventBus,
                                IOptionsMonitor<AppSettings> optionsMonitor)
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
            _logger.LogInformation("Starting Eventlistner of blendnet.cms.listener");

            _eventBus.Subscribe<ContentProviderCreatedIntegrationEvent,ContentProviderCreatedIntegrationEventHandler>();

            _eventBus.Subscribe<ContentUploadedIntegrationEvent, ContentUploadedIntegrationEventHandler>();

            _eventBus.Subscribe<ContentDeletedIntegrationEvent, ContentDeletedIntegrationEventHandler>();

            _eventBus.Subscribe<ContentTransformIntegrationEvent, ContentTransformIntegrationEventHandler>();

            _eventBus.Subscribe<ContentBroadcastIntegrationEvent, ContentBroadcastIntegrationEventHandler>();

            CustomPropertyCorrelationRule correlationRule = new CustomPropertyCorrelationRule()
            {
                PropertyName = _appSettings.AmsEventGridPropertyName,
                PropertValue = _appSettings.AmsEventGridSubscriptionName,
            };

            _eventBus.Subscribe<MediaServiceJobIntegrationEvent, MediaServiceJobIntegrationEventHandler>(correlationRule);

            await _eventBus.StartProcessing();

            //todo: read from config once we finalize that we are ok to consume blob created event here.
            //CustomPropertyCorrelationRule correlationRule = new CustomPropertyCorrelationRule()
            //{
            //    PropertyName = "aeg-subscription-name",
            //    PropertValue = "BLOBTOPICSUBSCRIPTION",
            //};

            //_eventBus.Subscribe<MicrosoftStorageBlobCreatedIntegrationEvent, MicrosoftStorageBlobCreatedIntegrationEventHandler>(correlationRule);

            _logger.LogInformation("Subscribe complete by blendnet.cms.listener");

            //return Task.CompletedTask;
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
