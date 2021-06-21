using blendnet.common.dto.Incentive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.incentive.repository.Interfaces
{
    public interface IIncentiveRepository
    {
        /// <summary>
        /// Create incentive plan
        /// </summary>
        /// <param name="incentivePlan"></param>
        /// <returns>Plan id</returns>
        public Task<Guid> CreateIncentivePlan(IncentivePlan incentivePlan);

        /// <summary>
        /// Update incentive plan
        /// </summary>
        /// <param name="incentivePlan"></param>
        /// <returns></returns>
        public Task<int> UpdateIncentivePlan(IncentivePlan incentivePlan);

        /// <summary>
        /// Deletes the plan 
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="subtypeName"></param>
        /// <returns></returns>
        public Task<int> DeleteIncentivePlan(Guid planId, string subtypeName);

        /// <summary>
        /// Retrieve current active plan for consumer audience type
        /// </summary>
        /// <param name="planType"></param>
        /// <returns></returns>
        public Task<IncentivePlan> GetConsumerPublishedPlan(PlanType planType, DateTime startDate);

        /// <summary>
        /// Get consumer published plan with given Id
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public Task<IncentivePlan> GetConsumerPublishedPlan(Guid planId, PlanType planType);

        /// <summary>
        /// Retrieve current active plan for retailer with given subtype name
        /// </summary>
        /// <param name="planType"></param>
        /// <param name="audienceSubTypeName"></param>
        /// <returns></returns>
        public Task<IncentivePlan> GetRetailerPublishedPlan(PlanType planType, string audienceSubTypeName, DateTime startDate);

        /// <summary>
        /// Get retailer published plan with given Id
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="subTypeName"></param>
        /// <returns></returns>
        public Task<IncentivePlan> GetRetailerPublishedPlan(Guid planId, string subTypeName, PlanType planType);

        /// <summary>
        /// Returns published list of events overlapping with given date
        /// </summary>
        /// <param name="planType"></param>
        /// <param name="audience"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public Task<List<IncentivePlan>> GetIncentivePlanList(PlanType planType, Audience audience, DateTime date);

        /// <summary>
        /// Returns published list of events overlapping with given date range
        /// </summary>
        /// <param name="planType"></param>
        /// <param name="audience"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public Task<List<IncentivePlan>> GetPublishedIncentivePlansInRange(PlanType planType, Audience audience, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Returns consumer published plan in given date range
        /// </summary>
        /// <param name="planType"></param>
        /// <returns></returns>
        public Task<IncentivePlan> GetConsumerPublishedPlanInRange(PlanType planType, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Returns retailer published plan in given date range
        /// </summary>
        /// <param name="planType"></param>
        /// <param name="audienceSubTypeName"></param>
        /// <returns></returns>
        public Task<IncentivePlan> GetRetailerPublishedPlanInRange(PlanType planType, string audienceSubTypeName, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Returns current active plan for the retailer
        /// </summary>
        /// <param name="planType"></param>
        /// <param name="audienceSubTypeName"></param>
        /// <returns></returns>
        public Task<IncentivePlan> GetCurrentRetailerActivePlan(PlanType planType, string audienceSubTypeName);

        /// <summary>
        /// Returns current active plan for the consumer
        /// </summary>
        /// <param name="planType"></param>
        /// <param name="audienceSubTypeName"></param>
        /// <returns></returns>
        public Task<IncentivePlan> GetCurrentConsumerActivePlan(PlanType planType);


        /// <summary>
        /// Returns retailer draft plan
        /// </summary>
        /// <param name="planType"></param>
        /// <param name="audienceSubTypeName"></param>
        /// <returns></returns>
        public Task<IncentivePlan> GetCurrentRetailerDraftPlan(PlanType planType, string audienceSubTypeName);

        /// <summary>
        /// Returns consumer draft plan
        /// </summary>
        /// <param name="planType"></param>
        /// <returns></returns>
        public Task<IncentivePlan> GetCurrentConsumerDraftPlan(PlanType planType);

        /// <summary>
        /// Gets plan with given plan id
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public Task<IncentivePlan> GetPlan(Guid planId, string subtypeName);

        /// <summary>
        /// Returns list of incentive plans for given audience and plan type
        /// </summary>
        /// <param name="audience"></param>
        /// <param name="planType"></param>
        /// <returns></returns>
        public Task<List<IncentivePlan>> GetIncentivePlans(Audience audience, PlanType planType);




    }
}
