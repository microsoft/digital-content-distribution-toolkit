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
        public Task<int> StoreEvent(Event eventItem);

        public List<Event> GetEvents(string eventGeneratorId, DateTime? startDate, DateTime? endDate);

        
    }
}
