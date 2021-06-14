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
        /// Retrieve current active plan for consumer audience type
        /// </summary>
        /// <param name="planType"></param>
        /// <returns></returns>
        public Task<IncentivePlan> GetCurrentConsumerActivePlan(PlanType planType);

        /// <summary>
        /// Retrieve current active plan for retailer with given subtype name
        /// </summary>
        /// <param name="planType"></param>
        /// <param name="audienceSubTypeName"></param>
        /// <returns></returns>
        public Task<IncentivePlan> GetCurrentRetailerActivePlan(PlanType planType, string audienceSubTypeName);

        /// <summary>
        /// Gets plan with given plan id
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public Task<IncentivePlan> GetPlan(Guid planId, string subtypeName);

    }
}
