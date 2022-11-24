// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Common;
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
        /// Returns the unique list or retailer id or phone number.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<ResultData<string>> GetUniqueAudienceForEventAggregrates(EventAggregrateRequest request, string continuationToken, int batchSize);

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

        /// <summary>
        /// GetUserEvents
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="continuationToken"></param>
        /// <param name="maxItemCount"></param>
        /// <returns></returns>
        public Task<ResultData<IncentiveEvent>> GetUserEvents(string phoneNumber, string continuationToken, int maxItemCount = 50);


        /// <summary>
        /// Get retailer events which were for the given user.
        /// Query is not based on partition key.
        /// Use in exceptional scenarios
        /// </summary>
        /// <param name="userPhoneNumber"></param>
        /// <param name="continuationToken"></param>
        /// <param name="maxItemCount"></param>
        /// <returns></returns>
        public Task<ResultData<IncentiveEvent>> GetRetailerEventsForUser(string userPhoneNumber, string continuationToken, int maxItemCount = 50);

        /// <summary>
        /// Inserts Events in Transction Batch
        /// Since updating the partition key is not allowed. Deleting and Inserting
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="eventsToInsert"></param>
        /// <returns></returns>
        public Task<int> InsertIncentiveEvents(string partitionKey, List<IncentiveEvent> eventsToInsert);

        /// <summary>
        /// Delete Incentive Events in Batch
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="eventsToDelete"></param>
        /// <returns></returns>
        public Task<int> DeleteIncentiveEvents(string partitionKey,List<Guid> eventsToDelete);

        /// <summary>
        /// Update Incentive Events in Batch
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="eventsToUpdate"></param>
        /// <returns></returns>

        public Task<int> UpdateIncentiveEvents(string partitionKey, List<IncentiveEvent> eventsToUpdate);
    }

}
