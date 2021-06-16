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
        /// Retreives events for given audience in selected date range
        /// </summary>
        /// <param name="eventGeneratorId"></param>
        /// <param name="audience"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<IncentiveEvent> GetEvents(string eventGeneratorId, Audience audience, DateTime? startDate, DateTime? endDate)
        {
            throw new NotImplementedException();
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
                                            WHERE c.eventGeneratorId = @eventGeneratorId
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
                eventDateTimeCondition = "AND (c.eventDateTime >= @startDate AND c.eventDateTime <= @endDate )";
            }

            if (request.AggregrateType == RuleType.COUNT)
            {
                queryString = string.Format(queryString, "COUNT(c)",RuleType.COUNT.ToString(), eventTypeAndCondition, eventDateTimeCondition);

            }else if (request.AggregrateType == RuleType.SUM)
            {
                queryString = string.Format(queryString, "SUM(c.calculatedValue)",RuleType.SUM.ToString(), eventTypeAndCondition, eventDateTimeCondition);
            }

            var queryDefinition = new QueryDefinition(queryString)
                .WithParameter("@eventGeneratorId", request.EventGeneratorId)
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
