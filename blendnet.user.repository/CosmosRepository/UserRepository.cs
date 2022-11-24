// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto;
using blendnet.common.dto.Common;
using blendnet.common.dto.User;
using blendnet.common.infrastructure.Extensions;
using blendnet.user.repository.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using User = blendnet.common.dto.User.User;

namespace blendnet.user.repository.CosmosRepository
{
    /// <summary>
    /// User Repository 
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly Container _container;
        private readonly Container _whitelistedUsersContainer;
        private readonly ILogger _logger;
        private readonly UserAppSettings _appSettings;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbClient"></param>
        /// <param name="optionsMonitor"></param>
        /// <param name="logger"></param>
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
        /// Returns the list of users who have ever raised a request for export of data.
        /// </summary>
        /// <returns></returns>
        public async Task<List<common.dto.User.User>> GetUsersForExport()
        {
            var queryString = @"    SELECT  *
                                    FROM c
                                    where c.type = @type
                                    AND c.dataExportRequestStatus <> null
                                    AND c.dataExportRequestStatus <> @status
                                    AND (c.accountStatus = @accountStatus OR NOT IS_DEFINED(c.accountStatus))
                                    ORDER BY c.modifiedDate DESC";

            var queryDef = new QueryDefinition(queryString)
                               .WithParameter("@type", UserContainerType.User)
                               .WithParameter("@status", DataExportRequestStatus.NotInitialized)
                               .WithParameter("@accountStatus", UserAccountStatus.Active);

            var users = await this._container.ExtractDataFromQueryIterator<common.dto.User.User>(queryDef);

            return users;
        }

        /// <summary>
        /// Returns the list of users who have ever raised a request for delete of data.
        /// </summary>
        /// <returns></returns>
        public async Task<List<common.dto.User.User>> GetUsersForDelete()
        {
            var queryString = @"    SELECT  *
                                    FROM c
                                    where c.type = @type
                                    AND c.dataUpdateRequestStatus <> null
                                    AND c.dataUpdateRequestStatus <> @status
                                    ORDER BY c.modifiedDate DESC";

            var queryDef = new QueryDefinition(queryString)
                               .WithParameter("@type", UserContainerType.User)
                               .WithParameter("@status", DataUpdateRequestStatus.NotInitialized);

            var users = await this._container.ExtractDataFromQueryIterator<common.dto.User.User>(queryDef);

            return users;
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
        /// Gets the Command for a user
        /// </summary>
        /// <param name="phoneNumber">phone number of the user / user id in case of deleted user</param>
        /// <param name="commandId">Command ID</param>
        /// <returns>Command</returns>
        async Task<UserCommand> IUserRepository.GetCommand(string partitionKey, Guid commandId)
        {
            try
            {
                UserCommand command = await _container.ReadItemAsync<UserCommand>(commandId.ToString(), new PartitionKey(partitionKey));

                return command;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        /// <summary>
        /// Update User and Create Command In Batch
        /// </summary>
        /// <param name="userDataExportCommand">Command to be created</param>
        /// <param name="user">user</param>
        /// <returns>status code</returns>
        async Task<int> IUserRepository.CreateCommandBatch(UserCommand userCommand, User user)
        {
            TransactionalBatch batch = _container.CreateTransactionalBatch(new PartitionKey(userCommand.PhoneNumber));

            TransactionalBatchResponse batchResponse = await batch.CreateItem(userCommand)
                                                                    .ReplaceItem<User>(user.UserId.ToString(), user)
                                                                    .ExecuteAsync();

            if (!batchResponse.IsSuccessStatusCode)
            {
                string errorMessage = $"{nameof(IUserRepository.CreateCommandBatch)} failed for user ID {user.UserId} and command ID {userCommand.Id} command type : {userCommand.UserCommandType} ";

                throw batchResponse.GetTransactionalBatchException(errorMessage);
            }

            return (int)batchResponse.StatusCode;
        }

        /// <summary>
        /// Updates the data export command
        /// </summary>
        /// <param name="userDataExportCommand">Command to be updated</param>
        /// <returns>status code</returns>
        async Task<int> IUserRepository.UpdateCommand(UserCommand userCommand)
        {
            try
            {
                var response = await this._container.ReplaceItemAsync(userCommand,
                                                                     userCommand.Id.ToString(), 
                                                                     new PartitionKey(userCommand.PhoneNumber));
                return (int)response.StatusCode;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (int)ex.StatusCode;
            }        
        }

        /// <summary>
        /// Update User and Export Command in Batch
        /// </summary>
        /// <param name="userDataExportCommand"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        async Task<int> IUserRepository.UpdateCommandBatch(  UserCommand userCommand, 
                                                             User user,
                                                                bool performETAGValidation)
        {
            TransactionalBatch batch = _container.CreateTransactionalBatch(new PartitionKey(userCommand.PhoneNumber));


            TransactionalBatchItemRequestOptions userRequestOptions = null;

            TransactionalBatchItemRequestOptions dataExportRequestOptions = null;

            if (performETAGValidation)
            {
                userRequestOptions = new TransactionalBatchItemRequestOptions()
                {
                    IfMatchEtag = user.ETag
                };

                dataExportRequestOptions = new TransactionalBatchItemRequestOptions()
                {
                    IfMatchEtag = userCommand.ETag
                };
            }

            TransactionalBatchResponse batchResponse = await batch.ReplaceItem(  userCommand.Id.ToString(), 
                                                                                 userCommand,
                                                                                 dataExportRequestOptions)
                                                                  .ReplaceItem<User>(   user.UserId.ToString(), 
                                                                                        user,
                                                                                        userRequestOptions)
                                                                  .ExecuteAsync();

            if (!batchResponse.IsSuccessStatusCode)
            {
                string errorMessage = $"{nameof(IUserRepository.UpdateCommandBatch)} failed for user ID {user.UserId} and command ID {userCommand.Id}";

                throw batchResponse.GetTransactionalBatchException(errorMessage);
            }

            return (int)batchResponse.StatusCode;
        }


        /// <summary>
        /// Gets all the commands for the given user phone number.
        /// Also allows to provide the exlusion list of command ids
        /// </summary>
        /// <param name="userPhoneNumber"></param>
        /// <param name="excludeIds"></param>
        /// <param name="continuationToken"></param>
        /// <param name="maxItemCount"></param>
        /// <returns></returns>
        async Task<ResultData<UserCommand>> IUserRepository.GetCommands(    string userPhoneNumber,
                                                                            List<Guid> excludeIds,
                                                                            string continuationToken,
                                                                            int maxItemCount)
        {
            string queryString = @"   SELECT * FROM c 
                                      WHERE c.type = @type 
                                      AND c.phoneNumber = @userPhoneNumber 
                                      {0} ";

            string exceptionCondition = string.Empty;

            //exception list is provided
            if (excludeIds != null && excludeIds.Count > 0)
            {
                exceptionCondition = "AND NOT ARRAY_CONTAINS(@exceptionList, c.id)";
            }

            queryString = string.Format(queryString, exceptionCondition);


            var queryDefinition = new QueryDefinition(queryString)
                .WithParameter("@type", UserContainerType.Command)
                .WithParameter("@userPhoneNumber", userPhoneNumber);

            if (excludeIds != null && excludeIds.Count > 0)
            {
                queryDefinition.WithParameter("@exceptionList", excludeIds);
            }

            continuationToken = String.IsNullOrEmpty(continuationToken) ? null : continuationToken;

            var commands = await _container.ExtractDataFromQueryIteratorWithToken<UserCommand>(queryDefinition,
                                                                                               continuationToken,
                                                                                               maxItemCount);

            return commands;
        }


        /// <summary>
        /// Insert the given list of commands for the given user phone number
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="commandsToInsert"></param>
        /// <returns></returns>
        async Task<int> IUserRepository.InsertCommands(string partitionKey,
                                                       List<UserCommand> commandsToInsert)
        {

            TransactionalBatch batch = _container.CreateTransactionalBatch(new PartitionKey(partitionKey));

            //insert the events
            foreach (UserCommand commandToInsert in commandsToInsert)
            {
                batch.CreateItem<UserCommand>(commandToInsert);
            }

            TransactionalBatchResponse batchResponse = await batch.ExecuteAsync();

            if (!batchResponse.IsSuccessStatusCode)
            {
                string errorMessage = $"{nameof(IUserRepository.InsertCommands)} failed to insert commands for user {partitionKey}";

                throw batchResponse.GetTransactionalBatchException(errorMessage);
            }

            return (int)batchResponse.StatusCode;
        }


        /// <summary>
        /// Deletes the given list of commands for the given user phone number
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="commandsToDelete"></param>
        /// <returns></returns>
        async Task<int> IUserRepository.DeleteBatch( string partitionKey,
                                                     List<Guid> idsToDelete)
        {
            TransactionalBatch batch = _container.CreateTransactionalBatch(new PartitionKey(partitionKey));

            //delete the events
            foreach (Guid idToDelete in idsToDelete)
            {
                batch.DeleteItem(idToDelete.ToString());
            }

            TransactionalBatchResponse batchResponse = await batch.ExecuteAsync();

            if (!batchResponse.IsSuccessStatusCode)
            {
                string errorMessage = $"{nameof(IUserRepository.DeleteBatch)} failed to delete user data for partition key {partitionKey}";

                throw batchResponse.GetTransactionalBatchException(errorMessage);
            }

            return (int)batchResponse.StatusCode;
        }

        /// <summary>
        /// Inserts in Batch
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="user"></param>
        /// <param name="userCommand"></param>
        /// <returns></returns>
        async Task<int> IUserRepository.InsertBatch(string partitionKey,User user, UserCommand userCommand)
        {

            TransactionalBatch batch = _container.CreateTransactionalBatch(new PartitionKey(partitionKey));

            batch.CreateItem<User>(user);

            batch.CreateItem<UserCommand>(userCommand);
            
            TransactionalBatchResponse batchResponse = await batch.ExecuteAsync();

            if (!batchResponse.IsSuccessStatusCode)
            {
                string errorMessage = $"{nameof(IUserRepository.InsertBatch)} failed to insert user , command for user {partitionKey}";

                throw batchResponse.GetTransactionalBatchException(errorMessage);
            }

            return (int)batchResponse.StatusCode;
        }

        #region private methods

        #endregion
    }
}
