using blendnet.common.dto.Events;
using blendnet.common.dto.Incentive;
using blendnet.common.dto.User;
using blendnet.common.infrastructure;
using blendnet.incentive.listener.Model;
using blendnet.incentive.listener.Util;
using blendnet.incentive.repository.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using blendnet.common.infrastructure.Extensions;
using blendnet.common.dto;

namespace blendnet.incentive.listener.IntegrationEventHandling
{
    /// <summary>
    /// Handles the retailed assigned to consumer
    /// </summary>
    public class RetailerAssignedIntegrationEventHandler : IIntegrationEventHandler<RetailerAssignedIntegrationEvent>
    {
        private readonly IEventRepository _eventRepository;

        private readonly IIncentiveRepository _incentiveRepository;

        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        public RetailerAssignedIntegrationEventHandler(ILogger<RetailerAssignedIntegrationEventHandler> logger,
                                                        TelemetryClient tc,
                                                        IEventRepository eventRepository,
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
        public async Task Handle(RetailerAssignedIntegrationEvent integrationEvent)
        {
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>("RetailerAssignedIntegrationEventHandler.Handle"))
                {
                    _logger.LogInformation($"Adding events of referral completion for retailer {integrationEvent.User.ReferralInfo.RetailerPartnerId} and user {integrationEvent.User.UserId} ");

                    User user = integrationEvent.User;

                    IncentivePlan activeRetailerRegularPlan = await _incentiveRepository.GetCurrentRetailerActivePlan(PlanType.REGULAR, user.ReferralInfo.RetailerPartnerCode);

                    IncentiveEvent incentiveEvent = GetIncentiveEventForReferral(user, activeRetailerRegularPlan);

                    await _eventRepository.CreateIncentiveEvent(incentiveEvent);

                    //report the same info to AI for analytics consumption
                    _telemetryClient.TrackEvent(new IncentiveAIEvent(incentiveEvent));

                    _logger.LogInformation($"Done adding event in RetailerAssignedIntegrationEventHandler for retailer {integrationEvent.User.ReferralInfo.RetailerPartnerId} and user {integrationEvent.User.UserId} ");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"RetailerAssignedIntegrationEventHandler.Handle failed for retailer {integrationEvent.User.ReferralInfo.RetailerPartnerId} and user {integrationEvent.User.UserId} ");
            }
        }

        private IncentiveEvent GetIncentiveEventForReferral(User user, IncentivePlan activeRetailerRegularPlan)
        {
            IncentiveEvent incentiveEvent = IncentiveUtil.CreateIncentiveEvent();

            incentiveEvent.Audience = new Audience()
            {
                AudienceType = AudienceType.RETAILER,
                SubTypeName = user.ReferralInfo.RetailerPartnerId
            };

            incentiveEvent.EventCreatedFor = user.ReferralInfo.RetailerPartnerId;
            incentiveEvent.EventCategoryType = EventCategoryType.INCOME;
            incentiveEvent.EventType = EventType.RETAILER_INCOME_REFERRAL_COMPLETED;
            incentiveEvent.OriginalValue = 0;

            PlanDetail planDetail = IncentiveUtil.GetPlanDetailForEvent(activeRetailerRegularPlan, EventType.RETAILER_INCOME_REFERRAL_COMPLETED, incentiveEvent.EventSubType);

            if (planDetail == null)
            {
                _logger.LogWarning($"Storing orphan event as no active plan exists for retailer regular plan with event id {incentiveEvent.EventId}, Event generator id {incentiveEvent.EventCreatedFor} and event type {EventType.RETAILER_INCOME_REFERRAL_COMPLETED}");

                incentiveEvent.CalculatedValue = 0;
            }
            else
            {
                IncentiveUtil.SetComputedValue(planDetail.Formula, incentiveEvent);
            }

            AddProperties(incentiveEvent, user);

            return incentiveEvent;
        }

        private void AddProperties(IncentiveEvent incentiveEvent, User user)
        {
            incentiveEvent.Properties = new List<Property>();

            Property userData = new Property()
            {
                Name =  ApplicationConstants.IncentiveEventAdditionalPropertyKeys.UserPhone,
                Value = user.PhoneNumber
            };
            incentiveEvent.Properties.Add(userData);

            Property userIdData = new Property()
            {
                Name = ApplicationConstants.IncentiveEventAdditionalPropertyKeys.UserId,
                Value = user.UserId.ToString()
            };
            incentiveEvent.Properties.Add(userIdData);

            Property retailerData = new Property()
            {
                Name = ApplicationConstants.IncentiveEventAdditionalPropertyKeys.RetailerPartnerId,
                Value = user.ReferralInfo.RetailerPartnerId
            };
            incentiveEvent.Properties.Add(retailerData);

            Property retailerReferralCodeData = new Property()
            {
                Name = ApplicationConstants.IncentiveEventAdditionalPropertyKeys.RetailerRefferalCode,
                Value = user.ReferralInfo.RetailerReferralCode
            };
            incentiveEvent.Properties.Add(retailerReferralCodeData);

            Property retailerReferralDate = new Property()
            {
                Name = ApplicationConstants.IncentiveEventAdditionalPropertyKeys.RetailerRefferalDate,
                Value = user.ReferralInfo.ReferralDateTime.ToString()
            };
            incentiveEvent.Properties.Add(retailerReferralDate);

            Property retailerPartnerCodeData = new Property()
            {
                Name = ApplicationConstants.IncentiveEventAdditionalPropertyKeys.RetailerPartnerCode,
                Value = user.ReferralInfo.RetailerPartnerCode
            };
            incentiveEvent.Properties.Add(retailerPartnerCodeData);

        }
    }
}
