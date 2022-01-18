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
        /// Creates the incentive event in container
        /// </summary>
        /// <param name="incentiveEvent"></param>
        /// <returns></returns>
        public Task<Guid> CreateIncentiveEvent(IncentiveEvent incentiveEvent);
        
        /// <summary>
        /// Updates the incentive event in container
        /// </summary>
        /// <param name="incentiveEvent"></param>
        /// <returns></returns>
        public Task<int> UpdateIncentiveEvent(IncentiveEvent incentiveEvent);

        /// <summary>
        /// Retreives events for given audience in selected date range
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public Task<List<IncentiveEvent>> GetEvents(EventCriteriaRequest eventCriteria);

        /// <summary>
        /// Returns the COUNT or SUM aggregrate for the given list of events.
        /// It does the group based on EVENT and EVENT SUB TYPE 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<List<EventAggregrateResponse>> GetEventAggregrates(EventAggregrateRequest request);

        /// <summary>
        /// Get User Events for export
        /// </summary>
        /// <param name="userPhoneNumber"></param>
        /// <returns></returns>
        public Task<List<IncentiveEventToExport>> GetUserEvents(string userPhoneNumber);
    }

}
