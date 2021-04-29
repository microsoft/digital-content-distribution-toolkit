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
        /// Get order by order id
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<Order> GetOrderByOrderId(Guid orderId, string phoneNumber);


        /// <summary>
        /// Returns orders by content provider id and user id. By default returns only active orders. 
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        Task<List<Order>> GetOrder(Guid UserId, Guid contentProviderId, bool returnAll = false);

        /// <summary>
        /// Update Order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Task<int> UpdateOrder(Order order);

        /// <summary>
        /// Get Orders by userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<Order>> GetOrdersByUserId(Guid userId);

        /// <summary>
        /// Get orders by phone number. By default returns only orders in created state
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        Task<List<Order>> GetOrdersByPhoneNumber(string phoneNumber, bool returnAll = false);


    }
}
