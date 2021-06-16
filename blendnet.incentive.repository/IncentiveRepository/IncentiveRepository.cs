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

        #region Repository apis
        /// <summary>
        /// Creates the given incentive plan
        /// </summary>
        /// <param name="incentivePlan"></param>
        /// <returns></returns>
        public async Task<Guid> CreateIncentivePlan(IncentivePlan incentivePlan)
        {
            await this._container.CreateItemAsync<IncentivePlan>(incentivePlan, new PartitionKey(incentivePlan.Audience.SubTypeName));
            return incentivePlan.Id.Value;
        }

        /// <summary>
        /// Updates incentive plan
        /// </summary>
        /// <param name="incentivePlan"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns plan with given id and subtypename
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="subtypeName"></param>
        /// <returns></returns>
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
        public Task<IncentivePlan> GetCurrentConsumerPublishedPlan(PlanType planType, DateTime startDate)
        {
            return GetPublishedPlan(planType, AudienceType.CONSUMER, ApplicationConstants.Common.CONSUMER, startDate);
        }


        /// <summary>
        /// Returns the current active plan for Retailer audience
        /// </summary>
        /// <param name="planType"></param>
        /// <param name="audienceSubTypeName"></param>
        /// <returns></returns>
        public Task<IncentivePlan> GetCurrentRetailerPublishedPlan(PlanType planType, string audienceSubTypeName, DateTime startDate)
        {
            return GetPublishedPlan(planType, AudienceType.RETAILER, audienceSubTypeName, startDate);
        }

        /// <summary>
        /// Returns current retailer active plan
        /// </summary>
        /// <param name="planType"></param>
        /// <param name="audienceSubTypeName"></param>
        /// <returns></returns>
        public Task<IncentivePlan> GetCurrentRetailerActivePlan(PlanType planType, string audienceSubTypeName)
        {
            return GetCurrentRetailerPublishedPlan(planType, audienceSubTypeName, DateTime.UtcNow);
        }

        /// <summary>
        /// Returns current consumer active plan
        /// </summary>
        /// <param name="planType"></param>
        /// <returns></returns>
        public Task<IncentivePlan> GetCurrentConsumerActivePlan(PlanType planType)
        {
            return GetCurrentConsumerPublishedPlan(planType, DateTime.UtcNow);
        }


        /// <summary>
        /// Deletes the plan with given id and subtypename
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="subtypeName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// returns current retailer draft plan
        /// </summary>
        /// <param name="planType"></param>
        /// <param name="audienceSubTypeName"></param>
        /// <returns></returns>
        public Task<IncentivePlan> GetCurrentRetailerDraftPlan(PlanType planType, string audienceSubTypeName)
        {
            return GetCurrentDraftPlan(planType, AudienceType.RETAILER, audienceSubTypeName);
        }

        /// <summary>
        /// Returns current consumer draft plan
        /// </summary>
        /// <param name="planType"></param>
        /// <returns></returns>
        public Task<IncentivePlan> GetCurrentConsumerDraftPlan(PlanType planType)
        {
            return GetCurrentDraftPlan(planType, AudienceType.CONSUMER, ApplicationConstants.Common.CONSUMER);
        }

        #endregion

        /// <summary>
        /// Returns published plan with given type, audience and date
        /// </summary>
        /// <param name="planType"></param>
        /// <param name="audienceType"></param>
        /// <param name="audienceSubTypeName"></param>
        /// <param name="validationDate"></param>
        /// <returns></returns>
        private Task<IncentivePlan> GetPublishedPlan(PlanType planType, AudienceType audienceType, string audienceSubTypeName, DateTime validationDate)
        {
            const string queryString = @"select * from c where c.planType = @planType 
                and c.audience.audienceType = @audienceType 
                and c.audience.subTypeName = @audienceSubTypeName 
                and c.startDate <= @now and c.endDate >= @now";

            var queryDef = new QueryDefinition(queryString)
                .WithParameter("@planType", planType)
                .WithParameter("@audienceType", audienceType)
                .WithParameter("@audienceSubTypeName", audienceSubTypeName)
                .WithParameter("@now", validationDate);

            return GetPlanFromQueryDef(queryDef);
        }


        /// <summary>
        /// Returns current draft plan of given type and audience
        /// </summary>
        /// <param name="planType"></param>
        /// <param name="audienceType"></param>
        /// <param name="audienceSubTypeName"></param>
        /// <returns></returns>
        private Task<IncentivePlan> GetCurrentDraftPlan(PlanType planType, AudienceType audienceType, string audienceSubTypeName)
        {
            const string queryString = @"select * from c where c.planType = @planType 
                and c.audience.audienceType = @audienceType 
                and c.audience.subTypeName = @audienceSubTypeName 
                and c.publishMode = @publishMode";

            var queryDef = new QueryDefinition(queryString)
                .WithParameter("@planType", planType)
                .WithParameter("@audienceType", audienceType)
                .WithParameter("@audienceSubTypeName", audienceSubTypeName)
                .WithParameter("@publishMode", PublishMode.DRAFT);

            return GetPlanFromQueryDef(queryDef);
        }

        /// <summary>
        /// Returns single plan from given query definition
        /// </summary>
        /// <param name="queryDefinition"></param>
        /// <returns></returns>
        private async Task<IncentivePlan> GetPlanFromQueryDef(QueryDefinition queryDefinition)
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

        /// <summary>
        /// Returns list of plans from given query definition
        /// </summary>
        /// <param name="queryDefinition"></param>
        /// <returns></returns>
        private async Task<List<IncentivePlan>> GetPlansFromQueryDef(QueryDefinition queryDefinition)
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
