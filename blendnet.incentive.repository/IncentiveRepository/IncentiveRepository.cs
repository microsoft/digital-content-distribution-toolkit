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

        public async Task<IncentivePlan> GetPlan(Guid planId, string subtype)
        {
            try
            {
                ItemResponse<IncentivePlan> response = await _container.ReadItemAsync<IncentivePlan>(planId.ToString(), new PartitionKey(subtype));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public Task<List<IncentivePlan>> GetCurrentConsumerActivePlan(PlanType planType)
        {
            var curDate = DateTime.UtcNow;

            var queryString = "select * from c where c.planType = @planType " +
                "and c.audience.audienceType = @audienceType " +
                "and c.startDate <= @now and c.endDate >= @now";

            var queryDef = new QueryDefinition(queryString)
                .WithParameter("@planType", planType)
                .WithParameter("@audienceType", AudienceType.CONSUMER)
                .WithParameter("@now", curDate);

            return GetCurrentActivePlan(queryDef);

        }

        public Task<List<IncentivePlan>> GetCurrentRetailerActivePlan(PlanType planType, string audienceSubTypeName)
        {
            var curDate = DateTime.UtcNow;

            var queryString = "select * from c where c.planType = @planType " +
                "and c.audience.audienceType = @audienceType " +
                "and c.audience.subTypeName = @audienceSubTypeName " +
                "and c.startDate <= @now and c.endDate >= @now";

            var queryDef = new QueryDefinition(queryString)
                .WithParameter("@planType", planType)
                .WithParameter("@audienceType", AudienceType.CONSUMER)
                .WithParameter("@audienceSubTypeName", audienceSubTypeName)
                .WithParameter("@now", curDate);

            return GetCurrentActivePlan(queryDef);
        }

        private async Task<List<IncentivePlan>> GetCurrentActivePlan(QueryDefinition queryDefinition)
        {
            try
            {
                var activePlans = await _container.ExtractDataFromQueryIterator<IncentivePlan>(queryDefinition);
                return activePlans;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }
}
