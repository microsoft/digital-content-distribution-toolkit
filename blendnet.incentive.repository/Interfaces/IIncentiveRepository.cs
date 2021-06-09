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
        /// <returns>Status</returns>
        public Task<int> CreateIncentivePlan(IncentivePlan incentivePlan);

        /// <summary>
        /// Adds event to repository
        /// </summary>
        /// <param name="incentiveEvent"></param>
        /// <returns>Status</returns>
        public Task<int> AddEvent(Event incentiveEvent);
    }
}
