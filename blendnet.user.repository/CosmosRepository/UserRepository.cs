using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using blendnet.common.dto;
using blendnet.common.dto.User;
using blendnet.user.repository.Interfaces;
using User = blendnet.common.dto.User.User;

namespace blendnet.user.repository.CosmosRepository
{
    public class UserRepository : IUserRepository
    {
        private Container _container;
        private readonly ILogger _logger;
        UserAppSettings _appSettings;

        public UserRepository(CosmosClient dbClient,
                                IOptionsMonitor<UserAppSettings> optionsMonitor,
                                ILogger<UserRepository> logger)
        {
            _appSettings = optionsMonitor.CurrentValue;

            _logger = logger;

            this._container = dbClient.GetContainer(_appSettings.DatabaseName, ApplicationConstants.CosmosContainers.User);
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
                var response = await this._container.ReplaceItemAsync<User>(user, user.Id.ToString(), new PartitionKey(user.PhoneNumber));
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
            var queryString = "select * from c where c.phoneNumber = @phoneNumber";
            var queryDef = new QueryDefinition(queryString).WithParameter("@phoneNumber", phoneNumber);

            return await ExtractFirstDataFromQueryIterator<User>(queryDef);
        }

        /// <summary>
        /// Returns the BlendNet User By userId
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>User Object</returns>
        public async Task<User> GetUserById(string id)
        {
            var queryString = "select * from c where c.id = @id";
            var queryDef = new QueryDefinition(queryString).WithParameter("@id", id);

            return await ExtractFirstDataFromQueryIterator<User>(queryDef);
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
                "WHERE u.referralInfo.retailerPartnerId = @retailerPartnerId  and u.referralInfo.referralDate >= @startDate and u.referralInfo.referralDate <= @endDate " +
                "Group By u.referralInfo.referralDate";

            var queryDef = new QueryDefinition(queryString)
                        .WithParameter("@retailerPartnerId", retailerPartnerId)
                        .WithParameter("@startDate", startDate)
                        .WithParameter("@endDate", endDate);

            var referralData = await ExtractAllDataFromQueryIterator<ReferralSummary>(queryDef);

            return referralData;
        }

        #region private methods
        /// <summary>
        /// Helper method to run a SELECT query and return first object
        /// </summary>
        /// <typeparam name="User">Result type</typeparam>
        /// <param name="queryDef">the SELECT query</param>
        /// <returns></returns>
        private async Task<T> ExtractFirstDataFromQueryIterator<T>(QueryDefinition queryDef)
        {
            var query = _container.GetItemQueryIterator<T>(queryDef);
            if (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                if (response.Count > 0)
                {
                    return response.Resource.ElementAt(0);
                }
            }

            return default(T);
        }

        /// <summary>
        /// Helper method to run a SELECT query and return all results as a list
        /// </summary>
        /// <typeparam name="T">Result type</typeparam>
        /// <param name="queryDef">the SELECT query</param>
        /// <returns>List of items that match the query</returns>
        private async Task<List<T>> ExtractAllDataFromQueryIterator<T>(QueryDefinition queryDef)
        {
            var returnList = new List<T>();
            var query = _container.GetItemQueryIterator<T>(queryDef);

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                returnList.AddRange(response.ToList());
            }

            return returnList;
        }
        #endregion
    }
}
