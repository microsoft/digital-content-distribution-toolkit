using blendnet.common.dto;
using blendnet.common.dto.Retailer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.retailer.repository.Interfaces
{
    public interface IRetailerRepository
    {
        /// <summary>
        /// Create a new Retailer
        /// </summary>
        /// <param name="retailer">Retailer entity</param>
        /// <returns>Partner Provided ID</returns>
        Task<string> CreateRetailer(RetailerDto retailer);

        /// <summary>
        /// Gets Retailer by RetailerPartnerId (composed)
        /// </summary>
        /// <param name="retailerPartnerId"></param>
        /// <param name="shouldGetInactiveRetailer"></param>
        /// <returns>Retailer Entity</returns>
        Task<RetailerDto> GetRetailerByPartnerId(string retailerPartnerId, bool shouldGetInactiveRetailer = false);

        /// <summary>
        /// Gets Retailer by Referral Code
        /// </summary>
        /// <param name="referralCode"></param>
        /// <param name="shouldGetInactiveRetailer"></param>
        /// <returns>Retailer Entity</returns>
        Task<RetailerDto> GetRetailerByReferralCode(string referralCode, bool shouldGetInactiveRetailer = false);

        /// <summary>
        /// Gets nearby retailers for a given location and within distance
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="distance">in meters</param>
        /// <returns>List of Retailer entities and their distance from the location</returns>
        Task<List<RetailerWithDistanceDto>> GetNearbyRetailers(double lat, double lng, double distance, bool shouldGetInactiveRetailers = false);

        /// <summary>
        /// Update a retailer entity
        /// </summary>
        /// <param name="retailer"></param>
        /// <returns>status code</returns>
        Task<int> UpdateRetailer(RetailerDto retailer);

        /// <summary>
        /// Gets the list of retailers to whom the specified device was ever assigned
        /// </summary>
        /// <param name="deviceId">device ID</param>
        /// <returns>Retailers as list</returns>
        Task<List<RetailerDto>> GetRetailersByDeviceId(string deviceId);
    }
}
