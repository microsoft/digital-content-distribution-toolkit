using blendnet.common.dto.Events;
using blendnet.common.dto.Incentive;
using blendnet.common.infrastructure;
using blendnet.incentive.listener.Util;
using blendnet.incentive.repository.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using blendnet.common.dto;
using System.Linq;

namespace blendnet.incentive.listener.IntegrationEventHandling
{
    /// <summary>
    /// Handles the User incentive events
    /// </summary>
    public class UserIntegrationEventHandler
    {
        private readonly IEventRepository _eventRepository;
        private readonly IIncentiveRepository _incentiveRepository;
        private readonly ILogger _logger;
        private readonly TelemetryClient _telemetryClient;

        public UserIntegrationEventHandler(ILogger<UserIntegrationEventHandler> logger,
                                            IEventRepository eventRepository,
                                            TelemetryClient tc,
                                            IIncentiveRepository incentiveRepository)
        {
            _eventRepository = eventRepository;
            _incentiveRepository = incentiveRepository;
            _logger = logger;
            _telemetryClient = tc;
        }

        /// <summary>
        /// Insert the incentive event
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(IncentiveEvent incentiveEvent, string operationName)
        {
            string userId = incentiveEvent.Properties
                                .Where(prop => prop.Name == ApplicationConstants.IncentiveEventAdditionalPropertyKeys.UserId)
                                .Select(prop => prop.Value)
                                .FirstOrDefault();
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>(operationName))
                {
                    _logger.LogInformation($"Adding {incentiveEvent.EventType} event for user {userId}");

                    var activePlan = await _incentiveRepository.GetCurrentConsumerActivePlan(PlanType.REGULAR);

                    CalculateValue(incentiveEvent, activePlan);

                    await _eventRepository.CreateIncentiveEvent(incentiveEvent);
                    
                    _logger.LogInformation($"Done Adding {incentiveEvent.EventType} event for user {userId}");
                }

            } catch (Exception e)
            {
                _logger.LogError(e, $"{operationName} failed for user {userId}");
            }
        }

        private void CalculateValue(IncentiveEvent incentiveEvent, IncentivePlan activePlan)
        {
            PlanDetail planDetail = IncentiveUtil.GetPlanDetailForEvent(activePlan, incentiveEvent.EventType, incentiveEvent.EventSubType);

            if (planDetail == null)
            {
                _logger.LogWarning($"Storing orphan event as no active plan exists for consumer regular plan with event id {incentiveEvent.EventId}, Event generator id {incentiveEvent.EventCreatedFor} and event type {incentiveEvent.EventType}");
                incentiveEvent.CalculatedValue = 0;
            }
            else
            {
                incentiveEvent.CalculatedValue = IncentiveUtil.GetComputedValue(incentiveEvent.OriginalValue, planDetail.Formula);
            }
        }
    }
}
