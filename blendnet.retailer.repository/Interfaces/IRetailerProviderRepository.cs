using blendnet.common.dto.Retailer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace blendnet.retailer.repository.Interfaces
{
    public interface IRetailerProviderRepository
    {
        /// <summary>
        /// Creates a retailer provider in database
        /// </summary>
        /// <param name="retailerProvider"></param>
        /// <returns>Service Account ID of the created retailer</returns>
        Task<Guid> CreateRetailerProvider(RetailerProviderDto retailerProvider);

        /// <summary>
        /// Gets Retailer Provider by Partner Code
        /// </summary>
        /// <param name="partnerCode"></param>
        /// <returns></returns>
        Task<RetailerProviderDto> GetRetailerProviderByPartnerCode(string partnerCode);

        /// <summary>
        /// Gets Retailer Provider by Service Account ID
        /// </summary>
        /// <param name="serviceAccountId"></param>
        /// <returns></returns>
        Task<RetailerProviderDto> GetRetailerProviderByServiceAccountId(Guid serviceAccountId);

        /// <summary>
        /// Gets all Retailer Providers
        /// </summary>
        /// <returns></returns>
        Task<List<RetailerProviderDto>> GetAllRetailerProviders();
    }
}
