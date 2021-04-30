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
        OmsAppSettings _appSettings;


        public OMSRepository(CosmosClient dbClient,
                                IOptionsMonitor<OmsAppSettings> optionsMonitor,
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

        public async Task<Order> GetOrderByOrderId(Guid orderId, string phoneNumber)
        {
            try
            {
                ItemResponse<Order> response = await this._container.ReadItemAsync<Order>(orderId.ToString(), new PartitionKey(phoneNumber.ToString()));

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public Task<List<Order>> GetOrdersByPhoneNumber(string phoneNumber, bool returnAll = false)
        {
            throw new NotImplementedException();
        }

        public Task<List<Order>> GetOrdersByUserId(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> UpdateOrder(Order order)
        {
            try
            {
                var response = await this._container.ReplaceItemAsync<Order>(order,
                                                                                        order.Id.Value.ToString(),
                                                                                        new PartitionKey(order.PhoneNumber.ToString()));

                return (int)response.StatusCode;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (int)ex.StatusCode;
            }
        }

        public Task<List<Order>> GetOrder(Guid UserId, Guid contentProviderId, bool returnAll = false)
        {
            throw new NotImplementedException();
        }
    }
}
