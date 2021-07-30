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
                    _logger.LogInformation($"Updating {incentiveEvent.EventType} event for user {userId}");

                    var activePlan = await _incentiveRepository.GetCurrentConsumerActivePlan(PlanType.REGULAR);

                    var oldCalculatedValue = incentiveEvent.CalculatedValue;
                    CalculateValue(incentiveEvent, activePlan);
                    var newCalculatedValue = incentiveEvent.CalculatedValue;

                    // skip update if calculated value is not changed
                    if (oldCalculatedValue == newCalculatedValue)
                    {
                        _logger.LogInformation($"Skipping {incentiveEvent.EventType} event for user {userId} as value not changed");
                    }

                    int response = await _eventRepository.UpdateIncentiveEvent(incentiveEvent);
                    if (response == (int)System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogInformation($"Done Updating {incentiveEvent.EventType} event for user {userId}");
                    }
                    else
                    {
                        _logger.LogError($"Failed Updating {incentiveEvent.EventType} event for user {userId}, Response Code : {response}");
                    }
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{operationName} failed for user {userId} for Event {incentiveEvent.EventType}");
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
