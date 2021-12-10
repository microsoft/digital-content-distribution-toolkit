using blendnet.common.dto;
using blendnet.common.dto.Oms;
using blendnet.common.infrastructure.Extensions;
using blendnet.oms.repository.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Creates the order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public async Task<Guid> CreateOrder(Order order)
        {
            await this._container.CreateItemAsync<Order>(order, new PartitionKey(order.PhoneNumber));

            return order.Id.Value;
        }

        /// <summary>
        /// Update the order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns order with given phone number and order id
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<Order> GetOrderByOrderId(Guid orderId, string phoneNumber)
        {
            try
            {
                ItemResponse<Order> response =  await _container.ReadItemAsync<Order>(orderId.ToString(), new PartitionKey(phoneNumber));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        /// <summary>
        /// Retuns list of orders of user for given content provider
        /// </summary>
        /// <param name="userPhoneNumber"></param>
        /// <param name="contentProviderId"></param>
        /// <param name="returnAll"></param>
        /// <returns></returns>

        public async Task<List<Order>> GetOrderByContentProviderId(string userPhoneNumber, Guid contentProviderId, OrderStatusFilter orderFilter)
        {
            var queryString = "select value o from o join oi in o.orderItems " +
                "where oi.subscription.contentProviderId = @contentProviderId and o.phoneNumber = @userPhoneNumber";

            if (orderFilter != null && orderFilter.OrderStatuses != null && orderFilter.OrderStatuses.Count() > 0)
            {
                queryString = queryString + " and o.orderStatus in ({0})";
                var orderString = "\"" + string.Join("\",\"", orderFilter.OrderStatuses) + "\"";
                queryString = string.Format(queryString, orderString);
            }

            var queryDef = new QueryDefinition(queryString)
                                .WithParameter("@contentProviderId", contentProviderId)
                                .WithParameter("@userPhoneNumber", userPhoneNumber);

            var orders = await _container.ExtractDataFromQueryIterator<Order>(queryDef);

            return orders;

        }

        public async Task<List<OrderSummary>> GetOrderSummary(string retailerPartnerId, int startDate, int endDate)
        {
            var queryString = "SELECT count(o) as count, o.retailerPartnerId, o.paymentDepositDate as date, oi.subscription.contentProviderId, oi.subscription.id as subscriptionId, oi.subscription.title, sum(oi.subscription.price) as totalAmount" +
                            " FROM o join oi in o.orderItems" +
                            " WHERE o.retailerPartnerId = @retailerPartnerId and o.orderStatus = \"Completed\" and o.paymentDepositDate >= @startDate and o.paymentDepositDate <= @endDate" +
                            " GROUP BY o.retailerPartnerId, o.paymentDepositDate, oi.subscription.contentProviderId, oi.subscription.id, oi.subscription.title";

            var queryDef = new QueryDefinition(queryString)
                                .WithParameter("@retailerPartnerId", retailerPartnerId)
                                .WithParameter("@startDate", startDate)
                                .WithParameter("@endDate", endDate);

            var purchaseData = await _container.ExtractDataFromQueryIterator<OrderSummary>(queryDef);

            return purchaseData;
        }

        public async Task<List<Order>> GetOrdersByPhoneNumber(string phoneNumber, OrderStatusFilter orderFilter, bool onlyRedeemed = false)
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

                //incase only redeemed needs to be read
                if (onlyRedeemed)
                {
                    queryString = queryString + " and c.isRedeemed = true ";
                }

                var queryDef = new QueryDefinition(queryString).WithParameter("@phoneNumber", phoneNumber);
                var orders = await _container.ExtractDataFromQueryIterator<Order>(queryDef);
                return orders;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<List<OrderItem>> GetActiveSubscriptionOrders(string phoneNumber)
        {
            try
            {

                var queryString = "SELECT oi.subscription, oi.amountCollected, oi.planStartDate, oi.planEndDate, oi.partnerReferenceNumber FROM o join oi in o.orderItems where o.phoneNumber = @phoneNumber and o.orderStatus = \"Completed\" and oi.planEndDate >= @currentDate";
                var queryDef = new QueryDefinition(queryString)
                    .WithParameter("@phoneNumber", phoneNumber)
                    .WithParameter("@currentDate", DateTime.UtcNow);

                var orderItems = await _container.ExtractDataFromQueryIterator<OrderItem>(queryDef);
                return orderItems;


            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets Count of Orders that were placed for a given subscription
        /// </summary>
        /// <param name="subscriptionId">Subscription ID</param>
        /// <param name="cutoffDate">Cutoff date - orders after this date will be counted</param>
        /// <returns>Count of orders</returns>
        async Task<int> IOMSRepository.GetOrdersCountBySubscriptionId(Guid subscriptionId, DateTime cutoffDate)
        {
            // Generated the query string using the below LINQ:
            // var queryLinq = _container.GetItemLinqQueryable<Order>()
            //                     .Where(o => o.OrderItems.Where(oi => oi.Subscription.Id! == subscriptionId).Count() > 0)
            //                     .Where(o => o.OrderCreatedDate >= cutoffDate)
            //                     .Select(o => o.Id);

            const string queryString = @"SELECT VALUE COUNT(root) FROM root 
                                            JOIN (SELECT VALUE COUNT(1) FROM root JOIN oi0 IN root.orderItems WHERE (oi0.subscription.id = @subscriptionId)) AS v0 
                                            WHERE (v0 > 0) AND root.orderCreatedDate >= @cutoffDate";
            var queryDef = new QueryDefinition(queryString)
                            .WithParameter("@subscriptionId", subscriptionId)
                            .WithParameter("@cutoffDate", cutoffDate);

            var ordersCount = await _container.ExtractFirstDataFromQueryIterator<int>(queryDef);
            return ordersCount;
        }

        #region private methods

        #endregion
    }
}
