using blendnet.common.dto;
using blendnet.common.dto.User;
using blendnet.common.infrastructure.Extensions;
using blendnet.user.repository.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User = blendnet.common.dto.User.User;

namespace blendnet.user.repository.CosmosRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly Container _container;
        private readonly Container _whitelistedUsersContainer;
        private readonly ILogger _logger;
        private readonly UserAppSettings _appSettings;

        public UserRepository(CosmosClient dbClient,
                                IOptionsMonitor<UserAppSettings> optionsMonitor,
                                ILogger<UserRepository> logger)
        {
            _appSettings = optionsMonitor.CurrentValue;

            _logger = logger;

            this._container = dbClient.GetContainer(_appSettings.DatabaseName, ApplicationConstants.CosmosContainers.User);
            this._whitelistedUsersContainer = dbClient.GetContainer(_appSettings.DatabaseName, ApplicationConstants.CosmosContainers.WhitelistedUser);
        }

        /// <summary>
        /// Create BlendNet user
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public async Task<int> CreateUser(User user)
        {
            try
            {
                var response = await this._container.CreateItemAsync<User>(user, new PartitionKey(user.PhoneNumber));
                return (int)response.StatusCode;
            }
            catch (CosmosException ex) 
            {
                return (int)ex.StatusCode;
            }
        }

        /// <summary>
        /// Udate BlendNet user
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public async Task<int> UpdateUser(User user)
        {
            try
            {
                var response = await this._container.ReplaceItemAsync<User>(user, user.UserId.ToString(), new PartitionKey(user.PhoneNumber));
                return (int)response.StatusCode;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (int)ex.StatusCode;
            }
        }

        /// <summary>
        /// Returns the BlendNet User By PhoneNumber
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns>User Object</returns>
        public async Task<User> GetUserByPhoneNumber(string phoneNumber)
        {
            var queryString = "select * from c where c.phoneNumber = @phoneNumber and c.type = @type";
            
            var queryDef = new QueryDefinition(queryString)
                                                .WithParameter("@phoneNumber", phoneNumber)
                                                .WithParameter("@type", UserContainerType.User);


            return await _container.ExtractFirstDataFromQueryIterator<User>(queryDef);
        }

        /// <summary>
        /// Get Referral data based on retailer partner id and given date range
        /// </summary>
        /// <param name="retailerPartnerId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<List<ReferralSummary>> GetReferralSummary(string retailerPartnerId, int startDate, int endDate)
        {
            var queryString = "SELECT count(u) as Count, u.referralInfo.referralDate as Date FROM u " +
                "WHERE u.referralInfo.retailerPartnerId = @retailerPartnerId  and u.referralInfo.referralDate >= @startDate and u.referralInfo.referralDate <= @endDate and u.type = @type " +
                "Group By u.referralInfo.referralDate";

            var queryDef = new QueryDefinition(queryString)
                        .WithParameter("@retailerPartnerId", retailerPartnerId)
                        .WithParameter("@startDate", startDate)
                        .WithParameter("@endDate", endDate)
                        .WithParameter("@type", UserContainerType.User);

            var referralData = await _container.ExtractDataFromQueryIterator<ReferralSummary>(queryDef);

            return referralData;
        }

        /// <summary>
        /// Gets whitelisted user by phone number
        /// </summary>
        /// <param name="phoneNumber">phone number</param>
        /// <returns></returns>
        async Task<WhitelistedUserDto> IUserRepository.GetWhitelistedUser(string phoneNumber)
        {
            var queryString = "select * from c where c.phoneNumber = @phoneNumber";

            var queryDef = new QueryDefinition(queryString)
                            .WithParameter("@phoneNumber", phoneNumber);

            var whitelistedUser = await _whitelistedUsersContainer.ExtractFirstDataFromQueryIterator<WhitelistedUserDto>(queryDef);

            return whitelistedUser;
        }

        /// <summary>
        /// Create Whitelisted user
        /// </summary>
        /// <param name="whitelistedUser">user to be created</param>
        /// <returns></returns>
        async Task<string> IUserRepository.CreateWhitelistedUser(WhitelistedUserDto whitelistedUser)
        {
            await _whitelistedUsersContainer.CreateItemAsync<WhitelistedUserDto>(whitelistedUser);
            return whitelistedUser.PhoneNumber;
        }

        /// <summary>
        /// Get Whitelisted user count for user ID
        /// </summary>
        /// <param name="userId">user ID of source who created the whitelisted users</param>
        /// <returns></returns>
        async Task<int> IUserRepository.CountOfUsersWhitelistedByUserId(Guid userId)
        {
            const string queryString = "select value count(c) from c where c.whitelistedByUserId = @userId";

            var queryDef = new QueryDefinition(queryString)
                            .WithParameter("@userId", userId);

            var count = await _whitelistedUsersContainer.ExtractFirstDataFromQueryIterator<int>(queryDef);

            return count;
        }

        /// <summary>
        /// Get Whitelisted user count (total)
        /// </summary>
        /// <returns></returns>
        async Task<int> IUserRepository.WhitelistedUsersTotalCount()
        {
            const string queryString = "select value count(c) from c";

            var queryDef = new QueryDefinition(queryString);

            var count = await _whitelistedUsersContainer.ExtractFirstDataFromQueryIterator<int>(queryDef);

            return count;
        }

        /// <summary>
        /// Delete Whitelisted user
        /// </summary>
        /// <param name="phoneNumber">phone number</param>
        /// <returns></returns>
        async Task<int> IUserRepository.DeleteWhitelistedUser(string phoneNumber)
        {
            try
            {
                var response = await _whitelistedUsersContainer.DeleteItemAsync<WhitelistedUserDto>(phoneNumber, new PartitionKey(phoneNumber));
                return (int)response.StatusCode;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (int)ex.StatusCode;
            }
        }

        /// <summary>
        /// Gets the Data Export Command for a user
        /// </summary>
        /// <param name="phoneNumber">phone number of the user</param>
        /// <param name="commandId">Data Export Command ID</param>
        /// <returns>Data Export Command</returns>
        async Task<UserDataExportCommand> IUserRepository.GetDataExportCommand(string phoneNumber, Guid commandId)
        {
            UserDataExportCommand command = await _container.ReadItemAsync<UserDataExportCommand>(commandId.ToString(), new PartitionKey(phoneNumber));
            return command;
        }

        /// <summary>
        /// Creates the Data Export Command
        /// </summary>
        /// <param name="userDataExportCommand">Command to be created</param>
        /// <param name="user">user</param>
        /// <returns>status code</returns>
        async Task<int> IUserRepository.CreateDataExportCommandBatch(UserDataExportCommand userDataExportCommand, User user)
        {
            TransactionalBatch batch = _container.CreateTransactionalBatch(new PartitionKey(userDataExportCommand.PhoneNumber));
            TransactionalBatchResponse batchResponse = await batch.CreateItem<UserDataExportCommand>(userDataExportCommand)
                                                                    .ReplaceItem<User>(user.UserId.ToString(), user)
                                                                    .ExecuteAsync();
            
            if (!batchResponse.IsSuccessStatusCode)
            {
                string errorMessage = $"{nameof(IUserRepository.CreateDataExportCommandBatch)} failed for user ID {user.UserId} and export command ID {userDataExportCommand.Id}";

                throw batchResponse.GetTransactionalBatchException(errorMessage);
            }

            return (int)batchResponse.StatusCode;
        }

        /// <summary>
        /// Updates the data export command
        /// </summary>
        /// <param name="userDataExportCommand">Command to be updated</param>
        /// <returns>status code</returns>
        async Task<int> IUserRepository.UpdateDataExportCommand(UserDataExportCommand userDataExportCommand)
        {
            try
            {
                var response = await this._container.ReplaceItemAsync<UserDataExportCommand>(   userDataExportCommand, 
                                                                                                userDataExportCommand.Id.ToString(), 
                                                                                                new PartitionKey(userDataExportCommand.PhoneNumber));
                return (int)response.StatusCode;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (int)ex.StatusCode;
            }        
        }

        #region private methods

        #endregion
    }
}
