using blendnet.common.dto;
using blendnet.common.dto.User;
using blendnet.common.infrastructure.Extensions;
using blendnet.user.repository.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        #region private methods

        #endregion
    }
}
