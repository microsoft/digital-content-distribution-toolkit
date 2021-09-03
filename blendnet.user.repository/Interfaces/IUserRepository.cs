using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using blendnet.common.dto.User;

namespace blendnet.user.repository.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Create BlendNet user
        /// </summary>
        /// <param name="User"></param>
        /// <returns>Status</returns>
        public Task<int> CreateUser(User user);

        /// <summary>
        /// Create BlendNet user
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public Task<int> UpdateUser(User user);

        /// <summary>
        /// Returns the BlendNet User By PhoneNumber
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns>User object</returns>
        public Task<common.dto.User.User> GetUserByPhoneNumber(string phoneNumber);

        /// <summary>
        /// Get Referral data based on retailer partner id and given date range
        /// </summary>
        /// <param name="retailerPartnerId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        Task<List<ReferralSummary>> GetReferralSummary(string retailerPartnerId, int startDate, int endDate);

        /// <summary>
        /// Create Whitelisted user
        /// </summary>
        /// <param name="whitelistedUser">user to be created</param>
        /// <returns>the whitelisted phone number</returns>
        Task<string> CreateWhitelistedUser(WhitelistedUserDto whitelistedUser);

        /// <summary>
        /// Get Whitelisted user by phone number
        /// </summary>
        /// <param name="phoneNumber">phone number</param>
        /// <returns></returns>
        Task<WhitelistedUserDto> GetWhitelistedUser(string phoneNumber);

        /// <summary>
        /// Get Whitelisted users count by user ID
        /// </summary>
        /// <param name="userId">user ID who created the whitelisted users</param>
        /// <returns></returns>
        Task<int> CountOfUsersWhitelistedByUserId(Guid userId);
        
        /// <summary>
        /// Get Whitelisted users count (total)
        /// </summary>
        /// <returns></returns>
        Task<int> WhitelistedUsersTotalCount();

        /// <summary>
        /// Delete Whitelisted user
        /// </summary>
        /// <param name="phoneNumber">phone number</param>
        /// <returns></returns>
        Task<int> DeleteWhitelistedUser(string phoneNumber);
    }
}
