using blendnet.common.dto.Incentive;
using blendnet.incentive.repository.Model;
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
        /// Creates the incentive event in container
        /// </summary>
        /// <param name="incentiveEvent"></param>
        /// <returns></returns>
        public Task<Guid> CreateIncentiveEvent(IncentiveEvent incentiveEvent);

        /// <summary>
        /// Retreives events for given audience in selected date range
        /// </summary>
        /// <param name="eventGeneratorId"></param>
        /// <param name="audience"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<IncentiveEvent> GetEvents(string eventGeneratorId, Audience audience, DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// Returns the COUNT or SUM aggregrate for the given list of events.
        /// It does the group based on EVENT and EVENT SUB TYPE 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<List<EventAggregrateResponse>> GetEventAggregrates(EventAggregrateRequest request);

    }

}
