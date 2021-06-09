using blendnet.common.dto;
using blendnet.common.dto.Incentive;
using blendnet.incentive.repository.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.incentive.repository.IncentiveRepository
{
    public class EventRepository : IEventRepository
    {
        private Container _container;
        private readonly ILogger _logger;
        IncentiveAppSettings _appSettings;

        public EventRepository(CosmosClient dbClient,
                               IOptionsMonitor<IncentiveAppSettings> optionsMonitor,
                               ILogger<IncentiveRepository> logger)
        {
            _appSettings = optionsMonitor.CurrentValue;

            _logger = logger;

            this._container = dbClient.GetContainer(_appSettings.DatabaseName, ApplicationConstants.CosmosContainers.Event);
        }

        public async Task<Guid> StoreEvent(Event eventItem)
        {
            await this._container.CreateItemAsync<Event>(eventItem, new PartitionKey(eventItem.EventGeneratorId));
            return eventItem.EventId.Value;
        }

        public List<Event> GetEvents(string eventGeneratorId, Audience audience, DateTime? startDate, DateTime? endDate)
        {
            throw new NotImplementedException();
        }
    }
}
