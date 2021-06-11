using blendnet.common.dto.Incentive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.incentive.repository.Interfaces
{
    public interface IEventRepository
    {
        /// <summary>
        /// Stores the event in container
        /// </summary>
        /// <param name="eventItem"></param>
        /// <returns></returns>
        public Task<Guid> StoreEvent(IncentiveEvent eventItem);

        public List<IncentiveEvent> GetEvents(string eventGeneratorId, Audience audience, DateTime? startDate, DateTime? endDate);

        
    }
}
