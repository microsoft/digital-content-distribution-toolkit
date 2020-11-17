using blendnet.crm.common.api;
using blendnet.crm.common.dto.Identity;
using blendnet.crm.contentprovider.repository.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.crm.contentprovider.api.IntegrationEventHandling
{
    /// <summary>
    /// User Changed Event Handler
    /// </summary>
    public class UserChangedIntegrationEventHandler : IIntegrationEventHandler<UserChangedIntegrationEvent>
    {
        private readonly ILogger _logger;

        private IContentProviderRepository _contentProviderRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contentProviderRepository"></param>
        /// <param name="logger"></param>
        public UserChangedIntegrationEventHandler(  IContentProviderRepository contentProviderRepository, 
                                                    ILogger<UserChangedIntegrationEventHandler> logger)
        {
            _contentProviderRepository = contentProviderRepository;

            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public Task Handle(UserChangedIntegrationEvent integrationEvent)
        {
            _logger.LogInformation($"User Changed integration event called {integrationEvent.Id}");

            return Task.FromResult(0);
        }
    }
}
