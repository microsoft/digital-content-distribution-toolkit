using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.listener.IntegrationEventHandling
{
    /// <summary>
    /// Content Provider Created Integration EventHandler
    /// </summary>
    public class ContentProviderCreatedIntegrationEventHandler : IIntegrationEventHandler<ContentProviderCreatedIntegrationEvent>
    {
        private readonly ILogger _logger;

        public ContentProviderCreatedIntegrationEventHandler(ILogger<ContentProviderCreatedIntegrationEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(ContentProviderCreatedIntegrationEvent integrationEvent)
        {
            return Task.CompletedTask;
        }
    }
}
