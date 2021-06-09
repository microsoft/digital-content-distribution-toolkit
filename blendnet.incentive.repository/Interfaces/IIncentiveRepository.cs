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
        /// Retrive current active plans for given plan type and audience type
        /// </summary>
        /// <param name="planType"></param>
        /// <param name="audienceType"></param>
        /// <returns></returns>
        public Task<List<IncentivePlan>> GetCurrentActivePlan(PlanType planType, AudienceType audienceType);

        /// <summary>
        /// Gets plan with given plan id
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public Task<IncentivePlan> GetPlan(Guid planId);

    }
}
