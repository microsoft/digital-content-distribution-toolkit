using blendnet.crm.common.dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.crm.retailer.repository.Interfaces
{
    public interface IRetailerRepository
    {
        Task<List<RetailerDto>> GetRetailers();
        Task<RetailerDto> GetRetailerById(Guid id);
        Task<Guid> CreateRetailer(RetailerDto retailer);
        Task<int> UpdateRetailer(RetailerDto updatedRetailer);
        Task<int> DeleteRetailer(Guid retailerId);

    }
}
