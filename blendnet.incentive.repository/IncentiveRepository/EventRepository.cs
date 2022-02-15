using blendnet.common.dto;
using blendnet.common.dto.Common;
using blendnet.common.dto.Incentive;
using blendnet.common.infrastructure.Extensions;
using blendnet.incentive.repository.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.incentive.repository.IncentiveRepository
{
    public class EventRepository : IEventRepository
    {
        private Container _container;
        private readonly ILogger _logger;
        IncentiveAppSettings _appSettings;

        public EventRepository(CosmosClient dbClient,
                               IOptionsMonitor<IncentiveAppSettings> optionsMonitor,
                               ILogger<IncentiveRepository> logger)
        {
            _appSettings = optionsMonitor.CurrentValue;

            _logger = logger;

            this._container = dbClient.GetContainer(_appSettings.DatabaseName, ApplicationConstants.CosmosContainers.IncentiveEvent);
        }

        /// <summary>
        /// Creates the incentive event in container
        /// </summary>
        /// <param name="eventItem"></param>
        /// <returns></returns>
        public async Task<Guid> CreateIncentiveEvent(IncentiveEvent eventItem)
        {
            await this._container.CreateItemAsync<IncentiveEvent>(eventItem, new PartitionKey(eventItem.EventCreatedFor));
            return eventItem.EventId.Value;
        }

        /// <summary>
        /// Updates the incentive event in container
        /// </summary>
        /// <param name="incentiveEvent"></param>
        /// <returns></returns>
        async Task<int> IEventRepository.UpdateIncentiveEvent(IncentiveEvent incentiveEvent)
        {
            try
            {
                var response = await this._container.ReplaceItemAsync<IncentiveEvent>(incentiveEvent, incentiveEvent.EventId.Value.ToString(), new PartitionKey(incentiveEvent.EventCreatedFor));
                return (int)response.StatusCode;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return (int)ex.StatusCode;
            }
        }

        /// <summary>
        /// Retrieves the event based on criteria.
        /// Event Generator id and Audience Type is mandatory. Rest all are optional
        /// </summary>
        /// <param name="eventCriteria"></param>
        /// <returns></returns>
        public async Task<List<IncentiveEvent>> GetEvents(EventCriteriaRequest eventCriteria)
        {
            string queryString = @"   SELECT *
                                      FROM c 
                                      WHERE c.eventCreatedFor = @eventCreatedFor
                                      AND c.audience.audienceType = @audienceType
                                      {0}
                                      {1} ";

            string eventTypeAndCondition = string.Empty;

            string eventDateTimeCondition = string.Empty;

            //if event types are provided
            if (eventCriteria.EventTypes != null && eventCriteria.EventTypes.Count > 0)
            {
                string eventTypesStringValue = string.Join(",", eventCriteria.EventTypes.Select(item => "'" + item + "'"));

                eventTypeAndCondition = $"AND c.eventType IN ({eventTypesStringValue})";
            }

            //if the date time is provided then add the add condition
            if (eventCriteria.StartDate.HasValue && eventCriteria.EndDate.HasValue)
            {
                eventDateTimeCondition = "AND (c.eventOccuranceTime >= @startDate AND c.eventOccuranceTime <= @endDate )";
            }

            queryString = string.Format(queryString, eventTypeAndCondition, eventDateTimeCondition);

            var queryDefinition = new QueryDefinition(queryString)
                .WithParameter("@eventCreatedFor", eventCriteria.EventCreatedFor)
                .WithParameter("@audienceType", eventCriteria.AudienceType);

            //if the date time is provided then add the parameters
            if (eventCriteria.StartDate.HasValue && eventCriteria.EndDate.HasValue)
            {
                queryDefinition.WithParameter("@startDate", eventCriteria.StartDate.Value);
                queryDefinition.WithParameter("@endDate", eventCriteria.EndDate.Value);
            }

            var incentiveEvents = await _container.ExtractDataFromQueryIterator<IncentiveEvent>(queryDefinition);

            return incentiveEvents;
        }


        /// <summary>
        /// Returns the unqiue audience retailer id / phone number
        /// Since the query uses DISTINCT and ORDER BY, COSMOS does not respect the batch size correctly. Even if we have 15 records in dtabase and the page size is 50 it returns only 2 -3 records in one page.
        /// Hence, with continuation token this will not scale. We will not be using this method for now.
        /// Keeping it, if in future, aggregrate based on retailer list does not scale and we consider this approach.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultData<string>> GetUniqueAudienceForEventAggregrates(EventAggregrateRequest request, 
                                                                                         string continuationToken,
                                                                                         int batchSize = 50)
        {
            string queryString = @"   SELECT DISTINCT VALUE c.eventCreatedFor
                                            FROM c 
                                            WHERE c.audience.audienceType = @audienceType
                                            AND c.audience.subTypeName = @subTypeName
                                            {0}
                                            {1}
                                      ORDER BY c.eventCreatedFor";

            string eventTypeAndCondition = string.Empty;

            string eventDateTimeCondition = string.Empty;

            //if event types are provided
            if (request.EventTypes != null && request.EventTypes.Count > 0)
            {
                eventTypeAndCondition = $"AND ARRAY_CONTAINS(@eventTypes, c.eventType)";
            }

            //if the date time is provided then add the add condition
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                eventDateTimeCondition = "AND (c.eventOccuranceTime >= @startDate AND c.eventOccuranceTime <= @endDate )";
            }

            queryString = string.Format(queryString, eventTypeAndCondition, eventDateTimeCondition);

            var queryDefinition = new QueryDefinition(queryString)
                .WithParameter("@audienceType", request.AudienceType)
                .WithParameter("@subTypeName", request.SubTypeName);

            //if the date time is provided then add the parameters
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                queryDefinition.WithParameter("@startDate", request.StartDate);
                queryDefinition.WithParameter("@endDate", request.EndDate);
            }

            if (request.EventTypes != null && request.EventTypes.Count > 0)
            {
                queryDefinition.WithParameter("@eventTypes", request.EventTypes);
            }

            continuationToken = String.IsNullOrEmpty(continuationToken) ? null : continuationToken;

            var uniqueAudiences = await this._container.ExtractDataFromQueryIteratorWithToken<string>(queryDefinition, 
                                                                                                      continuationToken, 
                                                                                                      batchSize);

            return uniqueAudiences;

        }


        /// <summary>
        /// Returns the COUNT or SUM aggregrate for the given list of events.
        /// It does the group based on EVENT and EVENT SUB TYPE 
        /// Group on Event Sub Type is required to support special scenarios like order complete 
        /// where the commission will be based on a particular sub type i.e. content provider.)
        /// After a certain amount in data , this method will result in high RU. We may have to look for other options in future where in we calculate the aggregrates at the time of insert. To start with, going with READ approach.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<List<EventAggregrateResponse>> GetEventAggregrates( EventAggregrateRequest request)
        {
            string queryString = @"   SELECT COUNT(c) as aggregratedCount,
                                            SUM(c.calculatedValue) as aggregratedCalculatedValue,
                                            SUM(c.originalValue) as aggregratedOriginalValue,
                                            c.eventCreatedFor,c.eventType,c.eventSubType
                                            FROM c 
                                            WHERE ARRAY_CONTAINS(@eventCreatedFor, c.eventCreatedFor)                                            
                                            AND c.audience.audienceType = @audienceType
                                            {0}
                                            {1}
                                            GROUP BY c.eventCreatedFor, c.eventType, c.eventSubType ";
            
            string eventTypeAndCondition = string.Empty;

            string eventDateTimeCondition = string.Empty;

            //if event types are provided
            if (request.EventTypes != null && request.EventTypes.Count > 0)
            {
                eventTypeAndCondition = $"AND ARRAY_CONTAINS(@eventTypes, c.eventType)";
            }

            //if the date time is provided then add the add condition
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                eventDateTimeCondition = "AND (c.eventOccuranceTime >= @startDate AND c.eventOccuranceTime <= @endDate )";
            }

            queryString = string.Format(queryString, eventTypeAndCondition, eventDateTimeCondition);

            var queryDefinition = new QueryDefinition(queryString)
                .WithParameter("@eventCreatedFor", request.EventCreatedFor)
                .WithParameter("@audienceType", request.AudienceType);

            //if the date time is provided then add the parameters
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                queryDefinition.WithParameter("@startDate", request.StartDate);
                queryDefinition.WithParameter("@endDate", request.EndDate);
            }

            //if event types is specified
            if (request.EventTypes != null && request.EventTypes.Count > 0)
            {
                queryDefinition.WithParameter("@eventTypes", request.EventTypes);
            }

            var eventAggregrates = await _container.ExtractDataFromQueryIterator<EventAggregrateResponse>(queryDefinition);

            return eventAggregrates;

        }


        /// <summary>
        /// Get User Events for export
        /// </summary>
        /// <param name="userPhoneNumber"></param>
        /// <returns></returns>
        public async Task<List<IncentiveEventToExport>> GetUserEvents(string userPhoneNumber)
        {
            string queryString = @"   SELECT  c.eventCreatedFor, c.eventType, 
                                              c.eventCategoryType , c.eventOccuranceTime ,
                                              c.calculatedValue as coins         
                                      FROM c where c.eventCreatedFor = @eventCreatedFor 
                                      AND c.audience.audienceType = @audienceType 
                                      ORDER BY c.createdDate ";

            
            var queryDefinition = new QueryDefinition(queryString)
                .WithParameter("@eventCreatedFor", userPhoneNumber)
                .WithParameter("@audienceType", AudienceType.CONSUMER);

            var userIncentiveEvents = await _container.ExtractDataFromQueryIterator<IncentiveEventToExport>(queryDefinition);

            return userIncentiveEvents;
        }


        /// <summary>
        /// Get User Events 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="continuationToken"></param>
        /// <param name="maxItemCount"></param>
        /// <returns></returns>
        public async Task<ResultData<IncentiveEvent>> GetUserEvents(string phoneNumber, 
                                                                    string continuationToken, 
                                                                    int maxItemCount)
        {
            string queryString = @"   SELECT *
                                      FROM c 
                                      WHERE c.eventCreatedFor = @eventCreatedFor
                                      AND c.audience.audienceType = @audienceType ";


            var queryDefinition = new QueryDefinition(queryString)
                .WithParameter("@eventCreatedFor", phoneNumber)
                .WithParameter("@audienceType", AudienceType.CONSUMER);

            continuationToken = String.IsNullOrEmpty(continuationToken) ? null : continuationToken;

            var incentiveEvents = await _container.ExtractDataFromQueryIteratorWithToken<IncentiveEvent>(   queryDefinition, 
                                                                                                            continuationToken,
                                                                                                            maxItemCount);

            return incentiveEvents;
        }

        /// <summary>
        /// Get retailer events which were for the given user.
        /// Query is not based on partition key.
        /// Use in exceptional scenarios
        /// </summary>
        /// <param name="userPhoneNumber"></param>
        /// <param name="continuationToken"></param>
        /// <param name="maxItemCount"></param>
        /// <returns></returns>
        public async Task<ResultData<IncentiveEvent>> GetRetailerEventsForUser(string userPhoneNumber,
                                                                        string continuationToken,
                                                                        int maxItemCount)
        {
            string queryString = @"   SELECT VALUE c
                                      FROM c
                                      JOIN (SELECT VALUE p 
                                            FROM p IN c.properties 
                                            WHERE p.name = @propertyName
                                            AND   p[""value""] = @userPhoneNumber) AS properties
                                      WHERE c.audience.audienceType = @audienceType ";


            var queryDefinition = new QueryDefinition(queryString)
                .WithParameter("@propertyName", ApplicationConstants.IncentiveEventAdditionalPropertyKeys.UserPhone)
                .WithParameter("@userPhoneNumber", userPhoneNumber)
                .WithParameter("@audienceType", AudienceType.RETAILER);

            var incentiveEvents = await _container.ExtractDataFromQueryIteratorWithToken<IncentiveEvent>(queryDefinition,
                                                                                                         continuationToken,
                                                                                                         maxItemCount);

            return incentiveEvents;
        }

        /// <summary>
        /// Delete incentive event in batch
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="userId"></param>
        /// <param name="eventsToDelete"></param>
        /// <returns></returns>
        public async Task<int> DeleteIncentiveEvents(string partitionKey,
                                                    List<Guid> eventsToDelete)
        {
            TransactionalBatch batch = _container.CreateTransactionalBatch(new PartitionKey(partitionKey));

            //delete the events
            foreach(Guid eventToDelete in eventsToDelete)
            {
                batch.DeleteItem(eventToDelete.ToString());
            }

            TransactionalBatchResponse batchResponse = await batch.ExecuteAsync();

            if (!batchResponse.IsSuccessStatusCode)
            {
                string errorMessage = $"{nameof(DeleteIncentiveEvents)} failed to delete user events for partition key {partitionKey}";

                throw batchResponse.GetTransactionalBatchException(errorMessage);
            }

            return (int)batchResponse.StatusCode;
        }

        /// <summary>
        /// Insert User Events
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="eventsToInsert"></param>
        /// <returns></returns>
        public async Task<int> InsertIncentiveEvents(   string partitionKey,
                                                        List<IncentiveEvent> eventsToInsert)
        {

            TransactionalBatch batch = _container.CreateTransactionalBatch(new PartitionKey(partitionKey));

            //insert the events
            foreach (IncentiveEvent eventToInsert in eventsToInsert)
            {
                batch.CreateItem<IncentiveEvent>(eventToInsert);
            }

            TransactionalBatchResponse batchResponse = await batch.ExecuteAsync();

            if (!batchResponse.IsSuccessStatusCode)
            {
                string errorMessage = $"{nameof(InsertIncentiveEvents)} failed to insert events for user {partitionKey}";

                throw batchResponse.GetTransactionalBatchException(errorMessage);
            }

            return (int)batchResponse.StatusCode;
        }

        /// <summary>
        /// Update Incentive Events
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="eventsToUpdate"></param>
        /// <returns></returns>
        public async Task<int> UpdateIncentiveEvents(   string partitionKey,
                                                        List<IncentiveEvent> eventsToUpdate)
        {

            TransactionalBatch batch = _container.CreateTransactionalBatch(new PartitionKey(partitionKey));

            //insert the events
            foreach (IncentiveEvent eventToUpdate in eventsToUpdate)
            {
                batch.ReplaceItem<IncentiveEvent>(eventToUpdate.EventId.ToString(), eventToUpdate);
            }

            TransactionalBatchResponse batchResponse = await batch.ExecuteAsync();

            if (!batchResponse.IsSuccessStatusCode)
            {
                string errorMessage = $"{nameof(UpdateIncentiveEvents)} failed to update events for partition key {partitionKey}";

                throw batchResponse.GetTransactionalBatchException(errorMessage);
            }

            return (int)batchResponse.StatusCode;
        }
    }
}
