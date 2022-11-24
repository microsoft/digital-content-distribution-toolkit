// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Common;
using blendnet.common.dto.Oms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.oms.repository.Interfaces
{
    public interface IOMSRepository
    {
        /// <summary>
        /// Create Order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<Guid> CreateOrder(Order order);

        /// <summary>
        /// Get Order by OrderId and phone number
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<Order> GetOrderByOrderId(Guid orderId, string phoneNumber);

        /// <summary>
        /// Returns orders by content provider id and user phone number. By default returns only active orders. 
        /// </summary>
        /// <param name="userPhoneNumber"></param>
        /// <param name="contentProviderId"></param>
        /// <param name="orderFilter"></param>
        /// <returns></returns>
        Task<List<Order>> GetOrderByContentProviderId(string userPhoneNumber, Guid contentProviderId, OrderStatusFilter orderFilter);

        /// <summary>
        /// Update Order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<int> UpdateOrder(Order order);

        /// <summary>
        /// Get purchase data based on retailer phone number and given date range
        /// </summary>
        /// <param name="retailerPhoneNumber"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        Task<List<OrderSummary>> GetOrderSummary(string retailerPhoneNumber, int startDate, int endDate);

        /// <summary>
        /// Get Orders based on customer phone number and order statuses
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="orderFilter"></param>
        /// <returns></returns>
        Task<List<Order>> GetOrdersByPhoneNumber(string phoneNumber, OrderStatusFilter orderFilter, bool onlyRedeemed = false);

        /// <summary>
        /// Get user orders based on continuation token and phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="continuationToken"></param>
        /// <param name="maxItemCount"></param>
        /// <returns></returns>
        Task<ResultData<Order>> GetOrdersByPhoneNumber(string phoneNumber, string continuationToken, int maxItemCount);


        /// <summary>
        /// Gets order items with active subscriptions
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        Task<List<OrderItem>> GetActiveSubscriptionOrders(string phoneNumber);

        /// <summary>
        /// Gets Count of Orders that were placed for a given subscription
        /// </summary>
        /// <param name="subscriptionId">Subscription ID</param>
        /// <param name="cutoffDate">Cutoff date - orders after this date will be counted</param>
        /// <returns>Count of orders</returns>
        Task<int> GetOrdersCountBySubscriptionId(Guid subscriptionId, DateTime cutoffDate);

        /// <summary>
        /// Get User Orders for export
        /// </summary>
        /// <param name="userPhoneNumber"></param>
        /// <returns></returns>
        Task<List<OrderToExport>> GetUserOrders(string userPhoneNumber);

        /// <summary>
        /// Insert Orders in Batch
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="ordersToInsert"></param>
        /// <returns></returns>
        Task<int> InsertOrders(string partitionKey, List<Order> ordersToInsert);

        /// <summary>
        /// Delete orders in batch
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="ordersToDelete"></param>
        /// <returns></returns>
        Task<int> DeleteOrders(string partitionKey, List<Guid> ordersToDelete);
    }
}
