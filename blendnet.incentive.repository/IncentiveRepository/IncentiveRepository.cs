using blendnet.common.dto;
using blendnet.common.dto.Incentive;
using blendnet.incentive.repository.Interfaces;
using blendnet.common.infrastructure.Extensions;
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
            await this._container.CreateItemAsync<IncentivePlan>(incentivePlan, new PartitionKey(incentivePlan.Audience.SubTypeName));
            return incentivePlan.Id.Value;
        }

        public async Task<int> UpdateIncentivePlan(IncentivePlan incentivePlan)
        {
            try
            {
                var response = await this._container.ReplaceItemAsync<IncentivePlan>(incentivePlan,
                                                                                        incentivePlan.Id.Value.ToString(),
                                                                                        new PartitionKey(incentivePlan.Audience.SubTypeName));

                return (int)response.StatusCode;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (int)ex.StatusCode;
            }
        }

        public async Task<IncentivePlan> GetPlan(Guid planId, string subtypeName)
        {
            try
            {
                ItemResponse<IncentivePlan> response = await _container.ReadItemAsync<IncentivePlan>(planId.ToString(), new PartitionKey(subtypeName));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the current active plan for Consumer audience
        /// </summary>
        /// <param name="planType"></param>
        /// <returns></returns>
        public Task<IncentivePlan> GetCurrentConsumerActivePlan(PlanType planType)
        {
            var curDate = DateTime.UtcNow;

            const string queryString = @"select * from c where c.planType = @planType 
                and c.audience.audienceType = @audienceType 
                and c.startDate <= @now and c.endDate >= @now";

            var queryDef = new QueryDefinition(queryString)
                .WithParameter("@planType", planType)
                .WithParameter("@audienceType", AudienceType.CONSUMER)
                .WithParameter("@now", curDate);

            return GetCurrentActivePlan(queryDef);

        }

        /// <summary>
        /// Returns the current active plan for Retailer audience
        /// </summary>
        /// <param name="planType"></param>
        /// <param name="audienceSubTypeName"></param>
        /// <returns></returns>
        public Task<IncentivePlan> GetCurrentRetailerActivePlan(PlanType planType, string audienceSubTypeName)
        {
            var curDate = DateTime.UtcNow;

            const string queryString = @"select * from c where c.planType = @planType 
                and c.audience.audienceType = @audienceType 
                and c.audience.subTypeName = @audienceSubTypeName 
                and c.startDate <= @now and c.endDate >= @now";

            var queryDef = new QueryDefinition(queryString)
                .WithParameter("@planType", planType)
                .WithParameter("@audienceType", AudienceType.RETAILER)
                .WithParameter("@audienceSubTypeName", audienceSubTypeName)
                .WithParameter("@now", curDate);

            return GetCurrentActivePlan(queryDef);
        }

        private async Task<IncentivePlan> GetCurrentActivePlan(QueryDefinition queryDefinition)
        {
            try
            {
                var activePlans = await _container.ExtractDataFromQueryIterator<IncentivePlan>(queryDefinition);

                return activePlans.FirstOrDefault();
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<int> DeleteIncentivePlan(Guid planId, string subtypeName)
        {
            try
            {
                var response = await this._container.DeleteItemAsync<IncentivePlan>(planId.ToString(), new PartitionKey(subtypeName));

                return (int)response.StatusCode;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (int)ex.StatusCode;
            }
        }
    }
}
