using blendnet.common.dto.Events;
using blendnet.common.dto.Incentive;
using blendnet.common.dto.User;
using blendnet.common.infrastructure;
using blendnet.incentive.listener.Util;
using blendnet.incentive.repository.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.incentive.listener.IntegrationEventHandling
{
    /// <summary>
    /// Handles the retailed assigned to consumer
    /// </summary>
    public class RetailerAssignedIntegrationEventHandler : IIntegrationEventHandler<RetailerAssignedIntegrationEvent>
    {
        private const string C_UserPhone = "UserPhone";
        private const string C_RetailerId = "RetailerId";
        private const string C_UserId = "UserId";
        private const string C_RetailerRefferalCode = "RetailerRefferalCode";
        private const string C_RetailerRefferalDate = "RetailerRefferalDate";
        private const string C_RetailerPartnerCode = "RetailerPartnerCode";

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
                    _logger.LogInformation($"Adding events of referral completion for retailer {integrationEvent.User.ReferralInfo.RetailerId} and user {integrationEvent.User.Id} ");

                    User user = integrationEvent.User;

                    IncentivePlan activeRetailerRegularPlan = await _incentiveRepository.GetCurrentRetailerPublishedPlan(PlanType.REGULAR, user.ReferralInfo.RetailerPartnerCode, null);

                    IncentiveEvent incentiveEvent = GetIncentiveEventForReferral(user, activeRetailerRegularPlan);

                    await _eventRepository.CreateIncentiveEvent(incentiveEvent);
                    
                    _logger.LogInformation($"Done adding event in RetailerAssignedIntegrationEventHandler for retailer {integrationEvent.User.ReferralInfo.RetailerId} and user {integrationEvent.User.Id} ");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"RetailerAssignedIntegrationEventHandler.Handle failed for retailer {integrationEvent.User.ReferralInfo.RetailerId} and user {integrationEvent.User.Id} ");
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
            incentiveEvent.EventType = EventType.RETAILER_INCOME_REFFRAL_COMPLETED;
            incentiveEvent.OriginalValue = 1;

            PlanDetail planDetail = IncentiveUtil.GetPlanDetailForEvent(activeRetailerRegularPlan, EventType.RETAILER_INCOME_REFFRAL_COMPLETED, incentiveEvent.EventSubType);

            if (planDetail == null)
            {
                _logger.LogWarning($"Storing orphan event as no active plan exists for retailer regular plan with event id {incentiveEvent.EventId}, Event generator id {incentiveEvent.EventCreatedFor} and event type {EventType.RETAILER_INCOME_REFFRAL_COMPLETED}");
                incentiveEvent.CalculatedValue = 0;
            }
            else
            {
                incentiveEvent.CalculatedValue = IncentiveUtil.GetComputedValue(incentiveEvent.OriginalValue, planDetail.Formula);
            }

            AddProperties(incentiveEvent, user);

            return incentiveEvent;
        }

        private void AddProperties(IncentiveEvent incentiveEvent, User user)
        {
            incentiveEvent.Properties = new List<Property>();

            Property userData = new Property()
            {
                Name = C_UserPhone,
                Value = user.PhoneNumber
            };
            incentiveEvent.Properties.Add(userData);

            Property userIdData = new Property()
            {
                Name = C_UserId,
                Value = user.Id.ToString()
            };
            incentiveEvent.Properties.Add(userIdData);

            Property retailerData = new Property()
            {
                Name = C_RetailerId,
                Value = user.ReferralInfo.RetailerId.ToString()
            };
            incentiveEvent.Properties.Add(retailerData);

            Property retailerReferralCodeData = new Property()
            {
                Name = C_RetailerRefferalCode,
                Value = user.ReferralInfo.RetailerReferralCode
            };
            incentiveEvent.Properties.Add(retailerReferralCodeData);

            Property retailerReferralDate = new Property()
            {
                Name = C_RetailerRefferalDate,
                Value = user.ReferralInfo.ReferralDateTime.ToString()
            };
            incentiveEvent.Properties.Add(retailerReferralDate);

            Property retailerPartnerCodeData = new Property()
            {
                Name = C_RetailerPartnerCode,
                Value = user.ReferralInfo.RetailerPartnerCode
            };
            incentiveEvent.Properties.Add(retailerPartnerCodeData);

        }
    }
}
