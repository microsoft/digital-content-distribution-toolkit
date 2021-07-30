using blendnet.common.dto;
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
        /// Returns the COUNT or SUM aggregrate for the given list of events.
        /// It does the group based on EVENT and EVENT SUB TYPE 
        /// Group on Event Sub Type is required to support special scenarios like order complete 
        /// where the commission will be based on a particular sub type i.e. content provider.)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<List<EventAggregrateResponse>> GetEventAggregrates( EventAggregrateRequest request)
        {
            string queryString = @"   SELECT {0} as aggregratedValue,c.eventType,c.eventSubType, '{1}' AS ruleType
                                            FROM c 
                                            WHERE c.eventCreatedFor = @eventCreatedFor
                                            AND c.audience.audienceType = @audienceType
                                            {2}
                                            {3}
                                            GROUP BY c.eventType, c.eventSubType ";
            
            string eventTypeAndCondition = string.Empty;

            string eventDateTimeCondition = string.Empty;

            //if event types are provided
            if (request.EventTypes != null && request.EventTypes.Count > 0)
            {
                string eventTypesStringValue = string.Join(",", request.EventTypes.Select(item => "'" + item + "'"));

                eventTypeAndCondition = $"AND c.eventType IN ({eventTypesStringValue})";
            }

            //if the date time is provided then add the add condition
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                eventDateTimeCondition = "AND (c.eventOccuranceTime >= @startDate AND c.eventOccuranceTime <= @endDate )";
            }

            if (request.AggregrateType == RuleType.COUNT)
            {
                queryString = string.Format(queryString, "COUNT(c)",RuleType.COUNT.ToString(), eventTypeAndCondition, eventDateTimeCondition);

            }else if (request.AggregrateType == RuleType.SUM)
            {
                queryString = string.Format(queryString, "SUM(c.calculatedValue)",RuleType.SUM.ToString(), eventTypeAndCondition, eventDateTimeCondition);
            }

            var queryDefinition = new QueryDefinition(queryString)
                .WithParameter("@eventCreatedFor", request.EventCreatedFor)
                .WithParameter("@audienceType", request.AudienceType);

            //if the date time is provided then add the parameters
            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                queryDefinition.WithParameter("@startDate", request.StartDate);
                queryDefinition.WithParameter("@endDate", request.EndDate);
            }

            var eventAggregrates = await _container.ExtractDataFromQueryIterator<EventAggregrateResponse>(queryDefinition);

            return eventAggregrates;

        }
    }
}
