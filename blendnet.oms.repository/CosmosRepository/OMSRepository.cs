using blendnet.common.dto.Oms;
using blendnet.oms.repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.oms.repository.CosmosRepository
{
    public class OMSRepository : IOMSRepository
    {
        public Task<Guid> CreateOrder(Order order)
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetOrderByOrderId(Guid orderId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Order>> GetOrdersByPhoneNumber(string phoneNumber)
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
    }
}
