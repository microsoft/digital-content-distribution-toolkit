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

        public async Task<Order> GetOrderByOrderId(Guid orderId)
        {
            try
            {
                string queryString = "SELECT * FROM c where c.id = @orderId";

                var queryDef = new QueryDefinition(queryString).WithParameter("@orderId", orderId);
                
                List<Order> results = await ExtractDataFromQueryIterator<Order>(queryDef);
                
                return results.FirstOrDefault();
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
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

        public async Task<List<Order>> GetOrder(Guid userId, Guid contentProviderId, bool returnAll = false)
        {
            var queryString = "select value c from c join i in c.orderItems " +
                "where i.subscription.contentProviderId = @contentProviderId and c.userId = @userId and c.orderStatus in ({0})";

            List<OrderStatus> orderStatus = new List<OrderStatus>();

            if(returnAll)
            {
                orderStatus.Add(OrderStatus.Created);
                orderStatus.Add(OrderStatus.Completed);
                orderStatus.Add(OrderStatus.Cancelled);
            }
            else
            {
                orderStatus.Add(OrderStatus.Created);
            }

            var orderString = "\"" + string.Join("\",\"", orderStatus) + "\"";
            queryString =  string.Format(queryString, orderString);

            var queryDef = new QueryDefinition(queryString)
                                .WithParameter("@contentProviderId", contentProviderId)
                                .WithParameter("@userId", userId);

            var orders = await ExtractDataFromQueryIterator<Order>(queryDef);

            return orders;

        }

        public async Task<List<OrderSummary>> GetOrderSummary(string retailerPhoneNumber, int startDate, int endDate)
        {
            var queryString = "SELECT count(o) as purchaseCount, o.retailerPhoneNumber, oi.paymentDepositDate, oi.subscription.contentProviderId, oi.subscription.id as subscriptionId, oi.subscription.title, sum(oi.subscription.price) as totalAmount" +
                            " FROM o join oi in o.orderItems" +
                            " WHERE o.retailerPhoneNumber = @retailerPhoneNumber and o.orderStatus = \"Completed\" and oi.paymentDepositDate >= @startDate and oi.paymentDepositDate <= @endDate" +
                            " GROUP BY o.retailerPhoneNumber, oi.paymentDepositDate, oi.subscription.contentProviderId, oi.subscription.id, oi.subscription.title";

            var queryDef = new QueryDefinition(queryString)
                                .WithParameter("@retailerPhoneNumber", retailerPhoneNumber)
                                .WithParameter("@startDate", startDate)
                                .WithParameter("@endDate", endDate);

            var purchaseData = await ExtractDataFromQueryIterator<OrderSummary>(queryDef);

            return purchaseData;
        }

        public async Task<List<Order>> GetOrdersByPhoneNumber(string phoneNumber, OrderStatusFilter orderFilter)
        {
            try
            {
                //if Filter object is null return all orders by customer phone number
                var queryString = "SELECT * FROM c where c.phoneNumber = @phoneNumber ";
                
                if (orderFilter != null && orderFilter.OrderStatuses != null && orderFilter.OrderStatuses.Count() > 0)
                {
                    queryString = queryString + " and c.orderStatus in ({0})";
                    var orderString = "\"" + string.Join("\",\"", orderFilter.OrderStatuses) + "\"";
                    queryString = string.Format(queryString, orderString);
                }

                var queryDef = new QueryDefinition(queryString).WithParameter("@phoneNumber", phoneNumber);
                var orders = await ExtractDataFromQueryIterator<Order>(queryDef);
                return orders;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }


        #region private methods
        /// <summary>
        /// Helper method to run a SELECT query and return all results as a list
        /// </summary>
        /// <typeparam name="T">Result type</typeparam>
        /// <param name="queryDef">the SELECT query</param>
        /// <returns>List of items that match the query</returns>
        private async Task<List<T>> ExtractDataFromQueryIterator<T>(QueryDefinition queryDef)
        {
            var returnList = new List<T>();
            var query = _container.GetItemQueryIterator<T>(queryDef);

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                returnList.AddRange(response.ToList());
            }

            return returnList;
        }
        #endregion
    }
}
