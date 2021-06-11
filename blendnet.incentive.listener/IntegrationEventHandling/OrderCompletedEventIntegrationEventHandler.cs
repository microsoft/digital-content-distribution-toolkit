using blendnet.api.proxy.KaizalaIdentity;
using blendnet.common.dto;
using blendnet.common.dto.Events;
using blendnet.common.dto.Incentive;
using blendnet.common.dto.Oms;
using blendnet.common.infrastructure;
using blendnet.incentive.listener.Util;
using blendnet.incentive.repository.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.incentive.listener.IntegrationEventHandling
{
    public class OrderCompletedEventIntegrationEventHandler : IIntegrationEventHandler<OrderCompletedIntegrationEvent>
    {

        private readonly IEventRepository _eventRepository;

        private readonly IIncentiveRepository _incentiveRepository;

        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        public OrderCompletedEventIntegrationEventHandler(ILogger<OrderCompletedEventIntegrationEventHandler> logger,
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
        /// Handle AddEvent
        /// Insert a record in Event collection
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(OrderCompletedIntegrationEvent integrationEvent)
        {
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>("OrderCompletedEventIntegrationEventHandler.Handle"))
                {
                    _logger.LogInformation($"Adding events of order completion");
                    Order order = integrationEvent.Order;
                    List<IncentivePlan> activeRetailerRegularPlan = await _incentiveRepository.GetCurrentRetailerActivePlan(PlanType.REGULAR, order.RetailerPartnerId.Split('-')[0]);

                    IncentiveEvent retailerEvent = GetRetailerEventForOrderCompletion(order, activeRetailerRegularPlan.Count > 0 ? activeRetailerRegularPlan[0] : null);

                    await _eventRepository.StoreEvent(retailerEvent);

                    List<IncentivePlan> activeConsumerRegularPlan = await _incentiveRepository.GetCurrentConsumerActivePlan(PlanType.REGULAR);

                    IncentiveEvent consumerEvent = GetConsumerEventForOrderCompletion(order, activeConsumerRegularPlan.Count > 0 ? activeConsumerRegularPlan[0] : null);

                    await _eventRepository.StoreEvent(consumerEvent);

                    _logger.LogInformation($"Done adding event");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"OrderCompletedEventIntegrationEventHandler.Handle failed ");
            }
        }

        private IncentiveEvent GetRetailerEventForOrderCompletion(Order order, IncentivePlan activeRetailerRegularPlan)
        {
            var curDate = DateTime.UtcNow;

            IncentiveEvent incentiveEvent = new IncentiveEvent();
            incentiveEvent.EventId = Guid.NewGuid();
            incentiveEvent.EventDateTime = curDate;
            incentiveEvent.EventDate = Int32.Parse(curDate.ToString(ApplicationConstants.DateTimeFormats.FormatYYYYMMDD));
            incentiveEvent.Audience = new Audience()
            {
                AudienceType = AudienceType.RETAILER,
                SubTypeName = order.RetailerPartnerId.Split('-')[0] // assuming it'll always be in format PartnerCode-PartnerName
            };

            incentiveEvent.EventGeneratorId = order.RetailerPartnerId.Split('-')[1];
            incentiveEvent.EventGroupType = EventGroupType.COMMISSION;
            incentiveEvent.EventCategoryType = EventCategoryType.INCOME;
            incentiveEvent.EventType = EventType.RTLR_INCM_ORDER_COMPLTD;
            incentiveEvent.OriginalValue = (double)order.TotalAmountCollected;

            PlanDetail planDetail = IncentiveUtil.GetPlanDetailForEvent(activeRetailerRegularPlan, EventType.RTLR_INCM_ORDER_COMPLTD);
            incentiveEvent.Value = planDetail != null ? IncentiveUtil.GetComputedValue(incentiveEvent.OriginalValue, planDetail.Formula) : 0 /*orphan event */ ;

            Property orderData = new Property()
            {
                Name = "OrderId",
                Value = order.Id.ToString()
            };

            Property transactionData = new Property()
            {
                Name = "TransactionId",
                Value = order.OrderItems[0].PartnerReferenceNumber
            };

            Property userData = new Property()
            {
                Name = "UserPhone",
                Value = order.PhoneNumber
            };


            incentiveEvent.Properties = new List<Property>();
            incentiveEvent.Properties.Add(orderData);
            incentiveEvent.Properties.Add(transactionData);
            incentiveEvent.Properties.Add(userData);

            incentiveEvent.CreatedDate = curDate;
            return incentiveEvent;

        }

        private IncentiveEvent GetConsumerEventForOrderCompletion(Order order, IncentivePlan activeConsumerRegularPlan)
        {
            var curDate = DateTime.UtcNow;

            IncentiveEvent incentiveEvent = new IncentiveEvent();
            incentiveEvent.EventId = Guid.NewGuid();
            incentiveEvent.EventDateTime = curDate;
            incentiveEvent.EventDate = Int32.Parse(curDate.ToString(ApplicationConstants.DateTimeFormats.FormatYYYYMMDD));
            incentiveEvent.Audience = new Audience()
            {
                AudienceType = AudienceType.CONSUMER,
                SubTypeName = ApplicationConstants.Common.CONSUMER
            };

            incentiveEvent.EventGeneratorId = order.UserId.ToString();
            incentiveEvent.EventGroupType = EventGroupType.CONSUMER;
            incentiveEvent.EventCategoryType = EventCategoryType.INCOME;
            incentiveEvent.EventType = EventType.CNSR_INCM_ORDER_COMPLTD;
            incentiveEvent.OriginalValue = 1;

            PlanDetail planDetail = IncentiveUtil.GetPlanDetailForEvent(activeConsumerRegularPlan, EventType.CNSR_INCM_ORDER_COMPLTD);
            incentiveEvent.Value = planDetail != null ? IncentiveUtil.GetComputedValue(incentiveEvent.OriginalValue, planDetail.Formula) : 0 /*orphan event */ ;

            Property orderData = new Property()
            {
                Name = "OrderId",
                Value = order.Id.ToString()
            };

            Property transactionData = new Property()
            {
                Name = "TransactionId",
                Value = order.OrderItems[0].PartnerReferenceNumber
            };

            Property userData = new Property()
            {
                Name = "UserPhone",
                Value = order.PhoneNumber
            };

            Property retailerData = new Property()
            {
                Name = "RetailerId",
                Value = order.RetailerPartnerId
            };


            incentiveEvent.Properties = new List<Property>();
            incentiveEvent.Properties.Add(orderData);
            incentiveEvent.Properties.Add(transactionData);
            incentiveEvent.Properties.Add(userData);
            incentiveEvent.Properties.Add(retailerData);

            incentiveEvent.CreatedDate = curDate;
            return incentiveEvent;

        }
    }
}
