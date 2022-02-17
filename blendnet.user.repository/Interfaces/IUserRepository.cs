using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using blendnet.common.dto.Common;
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
        public Task<int> CreateUser(common.dto.User.User user);

        /// <summary>
        /// Create BlendNet user
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public Task<int> UpdateUser(common.dto.User.User user);

        /// <summary>
        /// Returns the BlendNet User By PhoneNumber
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns>User object</returns>
        public Task<common.dto.User.User> GetUserByPhoneNumber(string phoneNumber);

        /// <summary>
        /// Returns the list of users who have ever requested for export
        /// </summary>
        /// <returns></returns>
        public Task<List<common.dto.User.User>> GetUsersForExport();


        /// <summary>
        /// Returns the list of users who have ever requested for delete
        /// </summary>
        /// <returns></returns>
        Task<List<common.dto.User.User>> GetUsersForDelete();


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

        /// <summary>
        /// Gets the Command
        /// </summary>
        /// <param name="partitionKey">phone number of the user / user id of the user in case of deleted user</param>
        /// <param name="commandId">Command ID</param>
        /// <returns>Command</returns>
        Task<UserCommand> GetCommand(string partitionKey, Guid commandId);

        /// <summary>
        /// Create Command and Update User in Batch
        /// </summary>
        /// <param name="userCommand"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<int> CreateCommandBatch(UserCommand userCommand, common.dto.User.User user);

        /// <summary>
        /// Updates Command
        /// </summary>
        /// <param name="userCommand"></param>
        /// <returns></returns>
        Task<int> UpdateCommand(UserCommand userCommand);

        /// <summary>
        /// Update User Command and User
        /// </summary>
        /// <param name="userCommand"></param>
        /// <param name="user"></param>
        /// <param name="performETAGValidation"></param>
        /// <returns></returns>
        Task<int> UpdateCommandBatch(UserCommand userCommand,
                                          common.dto.User.User user,
                                          bool performETAGValidation = false);
        /// <summary>
        /// Gets all the commands for the given user phone number.
        /// Also allows to provide the exlusion list of command ids
        /// </summary>
        /// <param name="userPhoneNumber"></param>
        /// <param name="excludeIds"></param>
        /// <param name="continuationToken"></param>
        /// <param name="maxItemCount"></param>
        /// <returns></returns>
        Task<ResultData<UserCommand>> GetCommands(string userPhoneNumber,
                                                  List<Guid> excludeIds,
                                                  string continuationToken,
                                                  int maxItemCount);

        /// <summary>
        /// Deletes the given list of commands for the given user phone number
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="commandsToDelete"></param>
        /// <returns></returns>
        Task<int> DeleteBatch(string partitionKey, List<Guid> idsToDelete);

        /// <summary>
        /// Insert the given list of commands for the given user phone number
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="commandsToInsert"></param>
        /// <returns></returns>
        Task<int> InsertCommands(string partitionKey, List<UserCommand> commandsToInsert);

        /// <summary>
        /// Inserts User and User Command in Batch
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="user"></param>
        /// <param name="userCommand"></param>
        /// <returns></returns>
        Task<int> InsertBatch(string partitionKey, User user, UserCommand userCommand);
    }
}
