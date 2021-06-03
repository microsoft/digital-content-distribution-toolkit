using blendnet.common.dto.Incentive;
using blendnet.incentive.repository.Interfaces;
using System;
using System.Threading.Tasks;

namespace blendnet.incentive.repository.IncentiveRepository
{
    public class IncentiveRepository : IIncentiveRepository
    {
        public Task<int> AddEvent(Event incentiveEvent)
        {
            throw new NotImplementedException();
        }

        public Task<int> CreateIncentivePlan(IncentivePlan incentivePlan)
        {
            throw new NotImplementedException();
        }
    }
}
