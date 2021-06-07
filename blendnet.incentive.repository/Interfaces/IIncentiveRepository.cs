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

    }
}
