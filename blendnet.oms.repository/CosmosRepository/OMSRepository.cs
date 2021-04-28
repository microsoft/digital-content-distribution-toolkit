using blendnet.common.dto;
using blendnet.common.dto.cms;
using blendnet.common.dto.Oms;
using blendnet.oms.repository.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.oms.repository.CosmosRepository
{
    public class OMSRepository : IOMSRepository
    {
        private Container _container;
        private readonly ILogger _logger;
        AppSettings _appSettings;


        public OMSRepository(CosmosClient dbClient,
                                IOptionsMonitor<AppSettings> optionsMonitor,
                                ILogger<OMSRepository> logger)
        {
            _appSettings = optionsMonitor.CurrentValue;

            _logger = logger;

            this._container = dbClient.GetContainer(_appSettings.DatabaseName, ApplicationConstants.CosmosContainers.Order);
        }

        public async Task<Guid> CreateOrder(Order order)
        {
            await this._container.CreateItemAsync<Order>(order, new PartitionKey(order.PhoneNumber));

            return order.Id.Value;
        }

        public Task<Order> GetOrderByOrderId(Guid orderId, string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task<List<Order>> GetOrdersByPhoneNumber(string phoneNumber, bool returnAll = false)
        {
            throw new NotImplementedException();
        }

        public Task<List<Order>> GetOrdersByUserId(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateOrder(Order order)
        {
            throw new NotImplementedException();
        }

        public Task<List<Order>> GetOrder(Guid UserId, Guid contentProviderId, bool returnAll = false)
        {
            throw new NotImplementedException();
        }
    }
}
