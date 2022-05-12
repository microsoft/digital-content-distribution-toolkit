using blendnet.api.proxy.KaizalaIdentity;
using blendnet.common.dto;
using blendnet.common.dto.Events;
using blendnet.common.dto.Incentive;
using blendnet.common.dto.Oms;
using blendnet.common.infrastructure;
using blendnet.incentive.listener.Model;
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
using blendnet.common.infrastructure.Extensions;

namespace blendnet.incentive.listener.IntegrationEventHandling
{
    /// <summary>
    /// Generates the incentive on order completion for Retailer and Consumer
    /// </summary>
    public class OrderCompletedEventIntegrationEventHandler : IIntegrationEventHandler<OrderCompletedIntegrationEvent>
    {
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
                    _logger.LogInformation($"Adding events of order completion for order id {integrationEvent.Order.Id} User {integrationEvent.Order.PhoneNumber}");

                    Order order = integrationEvent.Order;

                    if (!order.IsRedeemed)
                    {
                        IncentivePlan activeRetailerRegularPlan = await _incentiveRepository.GetCurrentRetailerActivePlan(PlanType.REGULAR, order.RetailerPartnerCode);

                        List<IncentiveEvent> retailerEvents = GetRetailerEventsForOrderCompletion(order, activeRetailerRegularPlan);

                        foreach (var retailerEvent in retailerEvents)
                        {
                            //save the retailer incentive to database
                            await _eventRepository.CreateIncentiveEvent(retailerEvent);

                            //report the same info to AI for analytics consumption
                            _telemetryClient.TrackEvent( new IncentiveAIEvent(retailerEvent));
                        }

                        IncentivePlan activeConsumerRegularPlan = await _incentiveRepository.GetCurrentConsumerActivePlan(PlanType.REGULAR);

                        List<IncentiveEvent> consumerEvents = GetConsumerEventsForOrderCompletion(order, activeConsumerRegularPlan);

                        foreach (var consumerEvent in consumerEvents)
                        {
                            //save the consumer incentive
                            await _eventRepository.CreateIncentiveEvent(consumerEvent);

                            //report the same info to AI for analytics consumption
                            _telemetryClient.TrackEvent(new IncentiveAIEvent(consumerEvent));
                        }
                    }else
                    {
                        _logger.LogInformation($"Since order is redeemed no credit to user {integrationEvent.Order.PhoneNumber} and retailer. Order id {integrationEvent.Order.Id}");

                        await AddRedemptionEvents(order);
                    }

                    _logger.LogInformation($"Done adding events for order id {integrationEvent.Order.Id} User {integrationEvent.Order.PhoneNumber} ");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"OrderCompletedEventIntegrationEventHandler.Handle failed ");
            }
        }

        /// <summary>
        /// Adds redemption Expense events
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private async Task AddRedemptionEvents(Order order)
        {
            IncentivePlan activeConsumerRegularPlan = await _incentiveRepository.GetCurrentConsumerActivePlan(PlanType.REGULAR);

            List<IncentiveEvent> consumerEvents = GetConsumerEventsForRedemption(order, activeConsumerRegularPlan);

            foreach (var consumerEvent in consumerEvents)
            {
                try
                {
                    //save redemption event to database
                    await _eventRepository.CreateIncentiveEvent(consumerEvent);

                    //report the same info to AI for analytics consumption
                    _telemetryClient.TrackEvent(new IncentiveAIEvent(consumerEvent));

                }
                catch (Exception ex)
                {
                    _telemetryClient.TrackEvent(GetRedemptionDeductionFailureAIEvent(consumerEvent, order));

                    _logger.LogError(ex, $"Failed to add expense event for User {consumerEvent.EventCreatedFor} Order {order.Id} Event Sub Type : {consumerEvent.EventSubType} ");
                }
            }
        }

        /// <summary>
        /// Get the failure Event
        /// </summary>
        /// <param name="incentiveEvent"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        private RedemptionDeductionFailureAIEvent GetRedemptionDeductionFailureAIEvent(IncentiveEvent incentiveEvent, Order order)
        {
            RedemptionDeductionFailureAIEvent aiEvent = new RedemptionDeductionFailureAIEvent();

            aiEvent.OrderId = order.Id.Value;

            aiEvent.OrderCompletedDate = order.OrderCompletedDate;

            aiEvent.UserId = order.UserId;

            aiEvent.EventType = incentiveEvent.EventType.ToString();
            
            aiEvent.EventSubType = incentiveEvent.EventSubType;

            aiEvent.OriginalValue = incentiveEvent.OriginalValue;

            aiEvent.CalculatedValue = incentiveEvent.CalculatedValue;

            Property orderItem = incentiveEvent.Properties.Where(p => p.Name.Equals(C_OrderItem)).FirstOrDefault();

            if(orderItem != null)
            {
                aiEvent.OrderItem = orderItem.Value;
            }

            return aiEvent;
        }

        /// <summary>
        /// Creates retailer event for order completion which is used for retailer incentive
        /// </summary>
        /// <param name="order"></param>
        /// <param name="activeRetailerRegularPlan"></param>
        /// <returns></returns>
        private List<IncentiveEvent> GetRetailerEventsForOrderCompletion(Order order, IncentivePlan activeRetailerRegularPlan)
        {
            List<IncentiveEvent> incentiveEvents = new List<IncentiveEvent>();

            foreach (var orderItem in order.OrderItems)
            {
                IncentiveEvent incentiveEvent = IncentiveUtil.CreateIncentiveEvent();

                incentiveEvent.Audience = new Audience()
                {
                    AudienceType = AudienceType.RETAILER,
                    SubTypeName = order.RetailerPartnerCode
                };

                incentiveEvent.EventCreatedFor = order.RetailerPartnerId;
                incentiveEvent.EventType = EventType.RETAILER_INCOME_ORDER_COMPLETED;
                incentiveEvent.EventSubType = orderItem.Subscription.ContentProviderId.ToString();
                incentiveEvent.OriginalValue = (double)orderItem.AmountCollected;

                PlanDetail planDetail = IncentiveUtil.GetPlanDetailForEvent(activeRetailerRegularPlan, EventType.RETAILER_INCOME_ORDER_COMPLETED, incentiveEvent.EventSubType);

                if (planDetail == null)
                {
                    _logger.LogWarning($"Storing orphan event as no active plan exists for retailer regular plan with event id {incentiveEvent.EventId}, Event generator id {incentiveEvent.EventCreatedFor} and order id {order.Id}");

                    incentiveEvent.CalculatedValue = 0;
                }
                else
                {
                    IncentiveUtil.SetComputedValue(planDetail.Formula, incentiveEvent);
                }

                AddProperties(incentiveEvent, order, orderItem);

                incentiveEvents.Add(incentiveEvent);
            }

            return incentiveEvents;
        }

        /// <summary>
        /// Creates consumer event for order completion which is used for coins incentive
        /// </summary>
        /// <param name="order"></param>
        /// <param name="activeRetailerRegularPlan"></param>
        /// <returns></returns>
        private List<IncentiveEvent> GetConsumerEventsForOrderCompletion(Order order, IncentivePlan activeConsumerRegularPlan)
        {
            List<IncentiveEvent> incentiveEvents = new List<IncentiveEvent>();

            foreach (var orderItem in order.OrderItems)
            {
                IncentiveEvent incentiveEvent = IncentiveUtil.CreateIncentiveEvent();

                incentiveEvent.Audience = new Audience()
                {
                    AudienceType = AudienceType.CONSUMER,
                    SubTypeName = ApplicationConstants.Common.CONSUMER
                };

                incentiveEvent.EventCreatedFor = order.PhoneNumber;
                incentiveEvent.EventCategoryType = EventCategoryType.INCOME;
                incentiveEvent.EventType = EventType.CONSUMER_INCOME_ORDER_COMPLETED;
                incentiveEvent.EventSubType = orderItem.Subscription.ContentProviderId.ToString();
                incentiveEvent.OriginalValue = (double)orderItem.AmountCollected;

                PlanDetail planDetail = IncentiveUtil.GetPlanDetailForEvent(activeConsumerRegularPlan, EventType.CONSUMER_INCOME_ORDER_COMPLETED, incentiveEvent.EventSubType);

                if (planDetail == null)
                {
                    _logger.LogWarning($"Storing orphan event as no active plan exists for consumer regular plan with event id {incentiveEvent.EventId}, Event generator id {incentiveEvent.EventCreatedFor} and order id {order.Id}");
                    incentiveEvent.CalculatedValue = 0;
                }
                else
                {
                    IncentiveUtil.SetComputedValue(planDetail.Formula, incentiveEvent);
                }

                AddProperties(incentiveEvent, order, orderItem);

                incentiveEvents.Add(incentiveEvent);
            }

            return incentiveEvents;

        }

        /// <summary>
        /// Generates the incentive events to insert.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private List<IncentiveEvent> GetConsumerEventsForRedemption(Order order, IncentivePlan activeConsumerRegularPlan)
        {
            List<IncentiveEvent> incentiveEvents = new List<IncentiveEvent>();

            foreach (var orderItem in order.OrderItems)
            {
                IncentiveEvent incentiveEvent = IncentiveUtil.CreateIncentiveEvent(EventCategoryType.EXPENSE);

                incentiveEvent.Audience = new Audience()
                {
                    AudienceType = AudienceType.CONSUMER,
                    SubTypeName = ApplicationConstants.Common.CONSUMER
                };

                incentiveEvent.EventCreatedFor = order.PhoneNumber;
                incentiveEvent.EventCategoryType = EventCategoryType.EXPENSE;
                incentiveEvent.EventType = EventType.CONSUMER_EXPENSE_SUBSCRIPTION_REDEEM;
                incentiveEvent.EventSubType = orderItem.Subscription.ContentProviderId.ToString();
                incentiveEvent.OriginalValue = orderItem.RedeemedValue;

                PlanDetail planDetail = IncentiveUtil.GetPlanDetailForEvent(activeConsumerRegularPlan, EventType.CONSUMER_EXPENSE_SUBSCRIPTION_REDEEM, null);

                if (planDetail == null)
                {
                    _logger.LogWarning($"Storing orphan expense event as no active consumer plan exists for event id {incentiveEvent.EventId}, Event generator id {incentiveEvent.EventCreatedFor} and order id {order.Id}");

                    incentiveEvent.CalculatedValue = 0;
                }
                else
                {
                    incentiveEvent.CalculatedValue = orderItem.RedeemedValue * -1;
                }

                AddProperties(incentiveEvent, order, orderItem);

                incentiveEvents.Add(incentiveEvent);
            }

            return incentiveEvents;

        }

        /// <summary>
        /// Adds all order related details in the properties which can be used on client side for further tracking or processing
        /// </summary>
        /// <param name="incentiveEvent"></param>
        /// <param name="order"></param>
        private void AddProperties(IncentiveEvent incentiveEvent, Order order, OrderItem orderItem)
        {
            incentiveEvent.Properties = new List<Property>();

            Property orderData = new Property()
            {
                Name = ApplicationConstants.OrderEventAdditionalPropertyKeys.OrderId,
                Value = order.Id.ToString()
            };

            incentiveEvent.Properties.Add(orderData);

            dynamic value = new JObject();
            
            value.subscriptionId = orderItem.Subscription.Id;
            
            if (!order.IsRedeemed)
            {
                value.transactionId = orderItem.PartnerReferenceNumber;
            }
            
            value.subscriptionTitle = orderItem.Subscription.Title;
            
            value.contentProviderId = orderItem.Subscription.ContentProviderId;

            Property transactionData = new Property()
            {
                Name = C_OrderItem,
                Value = JsonConvert.SerializeObject(value)
            };
            
            incentiveEvent.Properties.Add(transactionData);

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

            if (!order.IsRedeemed)
            {
                Property retailerData = new Property()
                {
                    Name = C_RetailerId,
                    Value = order.RetailerPartnerId
                };

                incentiveEvent.Properties.Add(retailerData);
            }
            
        }
    }
}
