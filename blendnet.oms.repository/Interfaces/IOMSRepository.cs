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
        /// Get Order by OrderId
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<Order> GetOrderByOrderId(Guid orderId);

        /// <summary>
        /// Returns orders by content provider id and user id. By default returns only active orders. 
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        Task<List<Order>> GetOrder(Guid userId, Guid contentProviderId, bool returnAll = false);

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
        Task<List<Order>> GetOrdersByPhoneNumber(string phoneNumber, OrderStatusFilter orderFilter);

    }
}
