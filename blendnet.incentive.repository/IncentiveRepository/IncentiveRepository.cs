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

        public async Task<IncentivePlan> GetPlan(Guid planId)
        {
            try
            {
                ItemResponse<IncentivePlan> response = await _container.ReadItemAsync<IncentivePlan>(planId.ToString(), new PartitionKey(planId.ToString()));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<List<IncentivePlan>> GetCurrentActivePlan(PlanType planType, AudienceType audienceType)
        {
            try
            {
                var curDate = DateTime.UtcNow;

                var queryString = "select * from c where c.planType = @planType " +
                    "and c.audience.audienceType = @audienceType " +
                    "and c.startDate <= @now and c.endDate >= @now";

                var queryDef = new QueryDefinition(queryString)
                    .WithParameter("@planType", planType)
                    .WithParameter("@audienceType", audienceType)
                    .WithParameter("@now", curDate);

                var activePlans = await _container.ExtractDataFromQueryIterator<IncentivePlan>(queryDef);
                return activePlans;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }
}
