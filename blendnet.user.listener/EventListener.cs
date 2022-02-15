using blendnet.common.dto.Events;
using blendnet.common.dto.User;
using blendnet.common.infrastructure;
using blendnet.user.listener.IntegrationEventHandling;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.user.listener
{
    /// <summary>
    /// Event Listener
    /// </summary>
    public class EventListener : IHostedService
    {
        private readonly ILogger<EventListener> _logger;

        private readonly IEventBus _eventBus;

        private readonly UserAppSettings _appSettings;

        public EventListener(ILogger<EventListener> logger,
                                IEventBus eventBus,
                                IOptionsMonitor<UserAppSettings> optionsMonitor)
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
            _logger.LogInformation("Starting Eventlistner of blendnet.user.listener");

            _eventBus.Subscribe<ExportUserDataIntegrationEvent, ExportUserDataIntegrationEventHandler>();
            _eventBus.Subscribe<UpdateUserDataIntegrationEvent, UpdateUserDataIntegrationEventHandler>();

            //export completed handlers
            _eventBus.Subscribe<IncentiveDataExportCompletedIntegrationEvent, IncentiveDataExportCompletedIntegrationEventHandler>();
            _eventBus.Subscribe<OrderDataExportCompletedIntegrationEvent, OrderDataExportCompletedIntegrationEventHandler>();
            _eventBus.Subscribe<UserDataExportCompletedIntegrationEvent, UserDataExportCompletedIntegrationEventHandler>();

            //update completed handlers
            _eventBus.Subscribe<IncentiveDataUpdateCompletedIntegrationEvent, IncentiveDataUpdateCompletedIntegrationEventHandler>();
            _eventBus.Subscribe<OrderDataUpdateCompletedIntegrationEvent, OrderDataUpdateCompletedIntegrationEventHandler>();
            _eventBus.Subscribe<UserDataUpdateCompletedIntegrationEvent, UserDataUpdateCompletedIntegrationEventHandler>();

            await _eventBus.StartProcessing();

            _logger.LogInformation("Subscribe complete by blendnet.user.listener");
        }

        /// <summary>
        /// Stop Async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Eventlistner of blendnet.user.listener");

            return Task.CompletedTask;
        }
    }
}
