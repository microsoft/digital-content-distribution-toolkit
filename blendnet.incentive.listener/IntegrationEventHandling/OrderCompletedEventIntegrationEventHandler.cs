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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.incentive.listener.IntegrationEventHandling
{
    public class OrderCompletedEventIntegrationEventHandler : IIntegrationEventHandler<OrderCompletedIntegrationEvent>
    {
        private const string C_OrderId = "OrderId";
        private const string C_UserPhone = "UserPhone";
        private const string C_RetailerId = "RetailerId";
        private const string C_OrderItem = "OrderItem";
        private const string C_UserId = "UserId";
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
                    List<IncentivePlan> activeRetailerRegularPlan = await _incentiveRepository.GetCurrentRetailerActivePlan(PlanType.REGULAR, order.RetailerPartnerCode);

                    IncentiveEvent retailerEvent = GetRetailerEventForOrderCompletion(order, activeRetailerRegularPlan.Count > 0 ? activeRetailerRegularPlan[0] : null);

                    await _eventRepository.CreateIncentiveEvent(retailerEvent);

                    List<IncentivePlan> activeConsumerRegularPlan = await _incentiveRepository.GetCurrentConsumerActivePlan(PlanType.REGULAR);

                    IncentiveEvent consumerEvent = GetConsumerEventForOrderCompletion(order, activeConsumerRegularPlan.Count > 0 ? activeConsumerRegularPlan[0] : null);

                    await _eventRepository.CreateIncentiveEvent(consumerEvent);

                    _logger.LogInformation($"Done adding event");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"OrderCompletedEventIntegrationEventHandler.Handle failed ");
            }
        }

        /// <summary>
        /// Creates retailer event for order completion which is used for retailer incentive
        /// </summary>
        /// <param name="order"></param>
        /// <param name="activeRetailerRegularPlan"></param>
        /// <returns></returns>
        private IncentiveEvent GetRetailerEventForOrderCompletion(Order order, IncentivePlan activeRetailerRegularPlan)
        {
            
            IncentiveEvent incentiveEvent = CreateIncentiveEvent();

            incentiveEvent.Audience = new Audience()
            {
                AudienceType = AudienceType.RETAILER,
                SubTypeName = order.RetailerPartnerCode
            };

            incentiveEvent.EventGeneratorId = order.RetailerPartnerId;
            incentiveEvent.EventType = EventType.RTLR_INCM_ORDER_COMPLTD;
            incentiveEvent.OriginalValue = (double)order.TotalAmountCollected;
            
            PlanDetail planDetail = IncentiveUtil.GetPlanDetailForEvent(activeRetailerRegularPlan, EventType.RTLR_INCM_ORDER_COMPLTD);

            if (planDetail == null)
            {
                _logger.LogWarning($"Storing orphan event as no active plan exists for retailer regular plan with event id {incentiveEvent.EventId}, Event generator id {incentiveEvent.EventGeneratorId} and order id {order.Id}");
                incentiveEvent.CalculatedValue = 0;
            }
            else
            {
                incentiveEvent.CalculatedValue = IncentiveUtil.GetComputedValue(incentiveEvent.OriginalValue, planDetail.Formula);
            }

            AddProperties(incentiveEvent, order);

            return incentiveEvent;
        }

        /// <summary>
        /// Creates consumer event for order completion which is used for coins incentive
        /// </summary>
        /// <param name="order"></param>
        /// <param name="activeRetailerRegularPlan"></param>
        /// <returns></returns>
        private IncentiveEvent GetConsumerEventForOrderCompletion(Order order, IncentivePlan activeConsumerRegularPlan)
        {
            IncentiveEvent incentiveEvent = CreateIncentiveEvent();

            incentiveEvent.Audience = new Audience()
            {
                AudienceType = AudienceType.CONSUMER,
                SubTypeName = ApplicationConstants.Common.CONSUMER
            };

            incentiveEvent.EventGeneratorId = order.UserId.ToString();
            incentiveEvent.EventCategoryType = EventCategoryType.INCOME;
            incentiveEvent.EventType = EventType.CNSR_INCM_ORDER_COMPLTD;
            incentiveEvent.OriginalValue = 1;

            PlanDetail planDetail = IncentiveUtil.GetPlanDetailForEvent(activeConsumerRegularPlan, EventType.CNSR_INCM_ORDER_COMPLTD);

            if(planDetail == null)
            {
                _logger.LogWarning($"Storing orphan event as no active plan exists for consumer regular plan with event id {incentiveEvent.EventId}, Event generator id {incentiveEvent.EventGeneratorId} and order id {order.Id}");
                incentiveEvent.CalculatedValue = 0;
            }
            else
            {
                incentiveEvent.CalculatedValue = IncentiveUtil.GetComputedValue(incentiveEvent.OriginalValue, planDetail.Formula);
            }

            AddProperties(incentiveEvent, order);

            return incentiveEvent;

        }

        /// <summary>
        /// Creates basic incentive event 
        /// </summary>
        /// <returns></returns>
        private IncentiveEvent CreateIncentiveEvent()
        {
            var curDate = DateTime.UtcNow;
            IncentiveEvent incentiveEvent = new IncentiveEvent();
            incentiveEvent.EventId = Guid.NewGuid();
            incentiveEvent.EventDateTime = curDate;
            incentiveEvent.EventDate = Int32.Parse(curDate.ToString(ApplicationConstants.DateTimeFormats.FormatYYYYMMDD));
            incentiveEvent.CreatedDate = curDate;
            incentiveEvent.EventCategoryType = EventCategoryType.INCOME;
            return incentiveEvent;
        }

        /// <summary>
        /// Adds all order related details in the properties which can be used on client side for further tracking or processing
        /// </summary>
        /// <param name="incentiveEvent"></param>
        /// <param name="order"></param>
        private void AddProperties(IncentiveEvent incentiveEvent, Order order)
        {
            incentiveEvent.Properties = new List<Property>();

            Property orderData = new Property()
            {
                Name = C_OrderId,
                Value = order.Id.ToString()
            };
            incentiveEvent.Properties.Add(orderData);


            foreach (var orderItem in order.OrderItems)
            {
                dynamic value = new JObject();
                value.subscriptionId = orderItem.Subscription.Id;
                value.transactionId = orderItem.PartnerReferenceNumber;
                value.subscriptionTitle = orderItem.Subscription.Title;

                Property transactionData = new Property()
                {
                    Name = C_OrderItem,
                    Value = JsonConvert.SerializeObject(value)

                };
                incentiveEvent.Properties.Add(transactionData);
            }

            Property userData = new Property()
            {
                Name = C_UserPhone,
                Value = order.PhoneNumber
            };
            incentiveEvent.Properties.Add(userData);

            Property userIdData = new Property()
            {
                Name = C_UserId,
                Value = order.UserId.ToString()
            };
            incentiveEvent.Properties.Add(userIdData);

            Property retailerData = new Property()
            {
                Name = C_RetailerId,
                Value = order.RetailerPartnerId
            };
            incentiveEvent.Properties.Add(retailerData);
        }
    }
}
