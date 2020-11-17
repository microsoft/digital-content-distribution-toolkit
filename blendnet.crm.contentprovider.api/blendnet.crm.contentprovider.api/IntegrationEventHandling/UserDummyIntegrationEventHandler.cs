using blendnet.common.infrastructure;
using blendnet.common.dto.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.crm.contentprovider.api.IntegrationEventHandling
{
    public class UserDummyIntegrationEventHandler : IIntegrationEventHandler<UserDummyIntegrationEvent>
    {
        private readonly ILogger _logger;

        public UserDummyIntegrationEventHandler(ILogger<UserChangedIntegrationEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(UserDummyIntegrationEvent integrationEvent)
        {
            _logger.LogInformation(integrationEvent.Id.ToString());

            return Task.FromResult(0);
        }
    }
}
