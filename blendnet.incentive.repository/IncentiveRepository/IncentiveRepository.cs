using blendnet.common.dto;
using blendnet.common.dto.Incentive;
using blendnet.incentive.repository.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.incentive.repository.IncentiveRepository
{
    public class IncentiveRepository : IIncentiveRepository
    {
        private Container _container;
        private readonly ILogger _logger;
        IncentiveAppSettings _appSettings;

        public IncentiveRepository(CosmosClient dbClient,
                               IOptionsMonitor<IncentiveAppSettings> optionsMonitor,
                               ILogger<IncentiveRepository> logger)
        {
            _appSettings = optionsMonitor.CurrentValue;

            _logger = logger;

            this._container = dbClient.GetContainer(_appSettings.DatabaseName, ApplicationConstants.CosmosContainers.IncentivePlan);
        }

        public async Task<Guid> CreateIncentivePlan(IncentivePlan incentivePlan)
        {
            await this._container.CreateItemAsync<IncentivePlan>(incentivePlan, new PartitionKey(incentivePlan.PlanId.ToString()));
            return incentivePlan.PlanId.Value;
        }

        public async Task<int> UpdateIncentivePlan(IncentivePlan incentivePlan)
        {
            try
            {
                var response = await this._container.ReplaceItemAsync<IncentivePlan>(incentivePlan,
                                                                                        incentivePlan.PlanId.Value.ToString(),
                                                                                        new PartitionKey(incentivePlan.PlanId.ToString()));

                return (int)response.StatusCode;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (int)ex.StatusCode;
            }
        }

        public async Task<List<IncentivePlan>> GetCurrentActivePlan(PlanType planType, AudienceType audienceType)
        {
            try
            {
                var curDate = DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:ss.fffZ");

                var queryString = "select * from c where c.planType = @planType " +
                    "and c.audience.audienceType = @audienceType " +
                    "and c.startDate <= @stDate and c.endDate >= @endDate";

                var queryDef = new QueryDefinition(queryString)
                    .WithParameter("@planType", planType)
                    .WithParameter("@audienceType", (int)audienceType)
                    .WithParameter("@stDate", curDate)
                    .WithParameter("@endDate", curDate);

                var activePlans = await ExtractDataFromQueryIterator<IncentivePlan>(queryDef);
                return activePlans;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        #region private methods
        /// <summary>
        /// Helper method to run a SELECT query and return all results as a list
        /// </summary>
        /// <typeparam name="T">Result type</typeparam>
        /// <param name="queryDef">the SELECT query</param>
        /// <returns>List of items that match the query</returns>
        private async Task<List<T>> ExtractDataFromQueryIterator<T>(QueryDefinition queryDef)
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
