using AutoMapper;
using blendnet.api.proxy;
using blendnet.api.proxy.Cms;
using blendnet.api.proxy.Incentive;
using blendnet.common.dto;
using blendnet.common.dto.Cms;
using blendnet.common.dto.Events;
using blendnet.common.dto.Incentive;
using blendnet.common.dto.Oms;
using blendnet.common.dto.User;
using blendnet.common.infrastructure;
using blendnet.common.infrastructure.Ams;
using blendnet.common.infrastructure.Extensions;
using blendnet.oms.api.Common;
using blendnet.oms.api.Model;
using blendnet.oms.repository.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.oms.api.Controllers
{
    /// <summary>
    /// Controller to manage order related apis
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class OrderBasicController : ControllerBase
    {
        private readonly ILogger _logger;

        private IOMSRepository _omsRepository;

        private SubscriptionProxy _subscriptionProxy;

        private ContentProxy _contentProxy;

        private IncentiveEventProxy _incentiveEventProxy;

        private IncentiveProxy _incentiveProxy;

        private IEventBus _eventBus;

        private OmsAppSettings _omsAppSettings;

        IStringLocalizer<SharedResource> _stringLocalizer;

        private TelemetryClient _telemetryClient;

        private OrderHelper _orderHelper;

        private readonly IMapper _mapper;

        public OrderBasicController(IOMSRepository omsRepository,
                                ILogger<OrderBasicController> logger,
                                ContentProxy contentProxy,
                                SubscriptionProxy subscriptionProxy,
                                IncentiveEventProxy incentiveEventProxy,
                                IncentiveProxy incentiveProxy,
                                IEventBus eventBus,
                                IOptionsMonitor<OmsAppSettings> optionsMonitor,
                                IStringLocalizer<SharedResource> stringLocalizer,
                                TelemetryClient telemetryClient,
                                IMapper mapper,
                                OrderHelper orderHelper)
        {
            _omsRepository = omsRepository;

            _logger = logger;

            _subscriptionProxy = subscriptionProxy;

            _contentProxy = contentProxy;

            _incentiveEventProxy = incentiveEventProxy;

            _incentiveProxy = incentiveProxy;

            _eventBus = eventBus;

            _omsAppSettings = optionsMonitor.CurrentValue;

            _stringLocalizer = stringLocalizer;

            _telemetryClient = telemetryClient;

            _orderHelper = orderHelper;

            _mapper = mapper;
        }

        /// <summary>
        /// Create order 
        /// </summary>
        /// <param name="orderRequest"></param>
        /// <returns></returns>
        [HttpPost("createorder", Name = nameof(CreateOrder))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult> CreateOrder(OrderRequest orderRequest)
        {
            List<string> errorInfo = new List<string>();

            Guid userId = UserClaimData.GetUserId(User.Claims);
            string userPhoneNumber = User.Identity.Name;

            // Get Subscription
            ContentProviderSubscriptionDto subscription = await _subscriptionProxy.GetSubscription(orderRequest.ContentProviderId, orderRequest.SubscriptionId);

            if (subscription == null)
            {
                errorInfo.Add(_stringLocalizer["OMS_ERR_0001"]);
                return BadRequest(errorInfo);
            }

            var error = ValidateSubscription(subscription);
            if (error != null)
            {
                errorInfo.Add(error);
                return BadRequest(errorInfo);
            }

            var orderStatusFilter = new OrderStatusFilter();

            orderStatusFilter.OrderStatuses.Add(OrderStatus.Created);
            orderStatusFilter.OrderStatuses.Add(OrderStatus.Completed);

            // Check if user do not hold active order for same content provider
            if (await ActiveOrderExists(userPhoneNumber, orderRequest.ContentProviderId, orderStatusFilter))
            {
                errorInfo.Add(_stringLocalizer["OMS_ERR_0002"]);
                return BadRequest(errorInfo);
            }

            //Populate following in Order object
            // Phone number, user id, user name, content provider id, subscription, Order status, OrderCreatedDate

            Order order = CreateOrder(userId, userPhoneNumber, subscription);

            //Insert order object in db
            Guid orderId = await _omsRepository.CreateOrder(order);

            SendOrderCreatedAIEvent(order);

            // return order id to caller

            return Ok(orderId);
        }


        /// <summary>
        /// Allows the end user to redeem subscription agains the accumulated points
        /// </summary>
        /// <param name="orderRequest"></param>
        /// <returns></returns>
        [HttpPost("redeem", Name = nameof(RedeemOrder))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult> RedeemOrder(OrderRequest orderRequest)
        {
            List<string> errorInfo = new List<string>();

            Guid userId = UserClaimData.GetUserId(User.Claims);

            string userPhoneNumber = User.Identity.Name;

            //check when the last redemption was done. make sure there is some gap of X seconds
            if (!await AllowOrderRedemption(userPhoneNumber))
            {
                return new StatusCodeResult((int)System.Net.HttpStatusCode.TooManyRequests);
            }

            // Get Subscription
            ContentProviderSubscriptionDto subscription = await _subscriptionProxy.GetSubscription(orderRequest.ContentProviderId, orderRequest.SubscriptionId);

            if (subscription == null)
            {
                errorInfo.Add(_stringLocalizer["OMS_ERR_0001"]);
                return BadRequest(errorInfo);
            }

            //validate the subscription end date and if its marked for redemption
            var error = ValidateSubscription(subscription, true);

            if (error != null)
            {
                errorInfo.Add(error);
                return BadRequest(errorInfo);
            }

            var orderStatusFilter = new OrderStatusFilter();

            orderStatusFilter.OrderStatuses.Add(OrderStatus.Created);

            orderStatusFilter.OrderStatuses.Add(OrderStatus.Completed);

            // Check if user do not hold active order for same content provider
            if (await ActiveOrderExists(userPhoneNumber, orderRequest.ContentProviderId, orderStatusFilter))
            {
                errorInfo.Add(_stringLocalizer["OMS_ERR_0002"]);
                return BadRequest(errorInfo);
            }

            //check if the active incentive plan allows for redemption
            bool isRedemptionAllowedInActivePlan = await IsRedemptionAllowedInActivePlan();

            if (!isRedemptionAllowedInActivePlan)
            {
                errorInfo.Add(_stringLocalizer["OMS_ERR_0019"]);
                return BadRequest(errorInfo);
            }

            //Check if user has the sufficient balance
            Tuple<bool, double> balance = await HasSufficientBalance(userPhoneNumber, subscription.RedemptionValue);

            //Get the coins balance validate against the coins required. Call to incentive proxy
            if (!balance.Item1)
            {
                errorInfo.Add(string.Format(_stringLocalizer["OMS_ERR_0018"], subscription.RedemptionValue, balance.Item2));
                return BadRequest(errorInfo);
            }

            //Populate following in Order object
            //Phone number, user id, user name, content provider id, subscription, Order status, OrderCreatedDate
            Order order = GetOrderForRedemption(userId, userPhoneNumber, subscription);

            //Insert order object in db
            Guid orderId = await _omsRepository.CreateOrder(order);

            OrderCompletedIntegrationEvent orderCompletedIntegrationEvent = new OrderCompletedIntegrationEvent()
            {
                Order = order,
            };

            await _eventBus.Publish(orderCompletedIntegrationEvent);

            _orderHelper.SendOrderCompletedAIEvent(order, null);

            return Ok(orderId);
        }

        /// <summary>
        /// Cancel order 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPut("cancel/{orderId:guid}", Name = nameof(CancelOrder))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> CancelOrder(Guid orderId)
        {
            var userPhoneNumber = User.Identity.Name;

            //Get Order by order id
            Order order = await _omsRepository.GetOrderByOrderId(orderId, userPhoneNumber);

            List<string> errorInfo = new List<string>();
            if (order == null)
            {
                errorInfo.Add(_stringLocalizer["OMS_ERR_0005"]);
                return BadRequest(errorInfo);
            }

            if (order.OrderStatus != OrderStatus.Created)
            {
                errorInfo.Add(_stringLocalizer["OMS_ERR_0006"]);
                return BadRequest(errorInfo);
            }

            order.OrderStatus = OrderStatus.Cancelled;
            order.OrderCancelledDate = DateTime.UtcNow;

            order.ModifiedByByUserId = UserClaimData.GetUserId(User.Claims);
            order.ModifiedDate = DateTime.UtcNow;

            // Update order
            var statusCode = await _omsRepository.UpdateOrder(order);

            if (statusCode == (int)System.Net.HttpStatusCode.OK)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }


        /// <summary>
        /// Returns the Token to view the content
        /// Todo: Change the get order call in Validate subscription
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        [HttpGet("token/{contentId:guid}", Name = nameof(GetContentToken))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<string>> GetContentToken(Guid contentId)
        {
            List<string> errorDetails = new List<string>();

            Content content = await _contentProxy.GetContentById(contentId);

            //Check if content is valid
            if (content == null)
            {
                errorDetails.Add(String.Format(_stringLocalizer["OMS_ERR_0007"], contentId));

                return BadRequest(errorDetails);
            }

            //Check the the content transformation status
            if (content.ContentTransformStatus != ContentTransformStatus.TransformComplete)
            {
                errorDetails.Add(String.Format(_stringLocalizer["OMS_ERR_0008"], content.ContentTransformStatus));

                return BadRequest(errorDetails);
            }

            //Check if Valid Subcription Exists
            if (!await IsValidSubcriptionExists(content, errorDetails))
            {
                return BadRequest(errorDetails);
            }

            string token = await GetContentTokenFromAms(content.ContentId.Value, content.ContentTransformStatusUpdatedBy.Value);

            return token;
        }

        /// <summary>
        ///  API to get order details by orderId
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet("{orderId:guid}", Name = nameof(GetOrderByOrderId))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<Order>> GetOrderByOrderId(Guid orderId)
        {
            var userPhoneNumber = User.Identity.Name;

            Order order = await _omsRepository.GetOrderByOrderId(orderId, userPhoneNumber);

            if (order == null)
            {
                return NotFound();
            }
            else
            {
                OrderResponse orderResponse = _mapper.Map<Order, OrderResponse>(order);

                return Ok(orderResponse);
            }
        }


        /// <summary>
        /// API to get Orders by customer phoneNumber and OrderStatus
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="orderFilter"></param>
        /// <returns></returns>
        [HttpPost("orderlist", Name = nameof(GetOrder))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<List<OrderResponse>>> GetOrder(OrderStatusFilter orderFilter)
        {
            string phoneNumber = User.Identity.Name;

            List<Order> orderlist = await _omsRepository.GetOrdersByPhoneNumber(phoneNumber, orderFilter);

            if (orderlist.Count() > 0)
            {
                List<OrderResponse> orderResponses = _mapper.Map<List<Order>, List<OrderResponse>>(orderlist);

                return Ok(orderResponses);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        ///  API to get order details by orderId
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet("active", Name = nameof(GetActiveSubscriptionOrders))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<common.dto.Oms.OrderItem>> GetActiveSubscriptionOrders()
        {
            var userPhoneNumber = User.Identity.Name;

            List<common.dto.Oms.OrderItem> orderItems = await _omsRepository.GetActiveSubscriptionOrders(userPhoneNumber);

            if (orderItems == null || orderItems.Count <= 0)
            {
                return NotFound();
            }
            else
            {
                List<OrderItemResponse> orderItemsResponse = _mapper.Map<List<common.dto.Oms.OrderItem>, List<OrderItemResponse>>(orderItems);

                return Ok(orderItemsResponse);
            }
        }

        #region Private Methods
        /// <summary>
        /// Validates subscription should not have ended in past date
        /// Also in case of redemptions, checks if this can be redeemed or not
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        private string ValidateSubscription(ContentProviderSubscriptionDto subscription, bool validateRedemption = false)
        {
            DateTime subscriptionEndDate = subscription.EndDate;

            if (validateRedemption)
            {
                if (!subscription.IsRedeemable)
                {
                    return _stringLocalizer["OMS_ERR_0017"];
                }
            }

            if (subscriptionEndDate < DateTime.UtcNow)
            {
                return _stringLocalizer["OMS_ERR_0013"];
            }

            return null;
        }

        /// <summary>
        /// Checks if an order / active subscription already exists for the give content provider
        /// </summary>
        /// <param name="userPhoneNumber"></param>
        /// <param name="contentProviderId"></param>
        /// <param name="orderFilter"></param>
        /// <returns></returns>
        private async Task<bool> ActiveOrderExists(string userPhoneNumber, Guid contentProviderId, OrderStatusFilter orderFilter)
        {
            List<Order> activeOrders = await _omsRepository.GetOrderByContentProviderId(userPhoneNumber, contentProviderId, orderFilter);

            if (activeOrders != null && activeOrders.Count > 0)
            {
                foreach (Order activeOrder in activeOrders)
                {
                    if (activeOrder.OrderStatus == OrderStatus.Completed && activeOrder.OrderItems[0].PlanEndDate > DateTime.UtcNow)
                    {
                        return true;
                    }

                    if (activeOrder.OrderStatus == OrderStatus.Created)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Create Order Object
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userPhoneNumber"></param>
        /// <param name="subscription"></param>
        /// <returns></returns>
        private Order CreateOrder(Guid userId, string userPhoneNumber, ContentProviderSubscriptionDto subscription)
        {
            Order order = new Order();

            common.dto.Oms.OrderItem orderItem = new common.dto.Oms.OrderItem();
            orderItem.Subscription = subscription;

            order.UserId = userId;
            order.PhoneNumber = userPhoneNumber;

            order.CreatedByUserId = userId;
            order.CreatedDate = DateTime.UtcNow;

            order.OrderItems.Add(orderItem);

            return order;
        }

        
        /// <summary>
        /// Sends order created AI event 
        /// </summary>
        /// <param name="order"></param>
        private void SendOrderCreatedAIEvent(Order order)
        {
            List<Model.OrderItem> orderItems = new List<Model.OrderItem>();

            foreach (var orderItem in order.OrderItems)
            {
                var orderItemDetail = new Model.OrderItem()
                {
                    SubscriptionId = orderItem.Subscription.Id.Value,
                    SubscriptionName = orderItem.Subscription.Title,
                    ContentProviderId = orderItem.Subscription.ContentProviderId,
                    SubscriptionValue = orderItem.Subscription.Price,
                    RedemptionValue = orderItem.RedeemedValue
                };

                orderItems.Add(orderItemDetail);
            }

            OrderCreatedAIEvent orderCreatedAIEvent = new OrderCreatedAIEvent()
            {
                OrderId = order.Id.Value,
                UserId = order.UserId,
                OrderPlacedDateTime = System.Text.Json.JsonSerializer.Serialize(order.OrderCreatedDate),
                OrderItems = System.Text.Json.JsonSerializer.Serialize(orderItems)
            };

            _telemetryClient.TrackEvent(orderCreatedAIEvent);
        }

        /// <summary>
        /// Checks if redemption is allowed.
        /// In case there is no active incentive plan for consumer, dont allow redemption.
        /// In case there is no plan detail with Expense event, dont allow redemption
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsRedemptionAllowedInActivePlan()
        {
            IncentivePlan incentivePlan = await _incentiveProxy.GetConsumerActivePlan(PlanType.REGULAR);

            //in case there is no active incentive plan for consumer, dont allow redemption.
            //there has to be one active plan for consumer. Atleast with expense.
            if (incentivePlan == null)
            {
                return false;
            }

            //in case there is no plan detail with Expense event, dont allow redemption
            PlanDetail planDetail = incentivePlan.PlanDetails.Where(pd => pd.EventType == EventType.CONSUMER_EXPENSE_SUBSCRIPTION_REDEEM).FirstOrDefault();

            if (planDetail == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if redemption to be allowed
        /// It checks for the time stamp for last redemeed order. 
        /// </summary>
        /// <param name="userPhoneNumber"></param>
        /// <returns></returns>
        private async Task<bool> AllowOrderRedemption(string userPhoneNumber)
        {
            var orderStatusFilter = new OrderStatusFilter();

            orderStatusFilter.OrderStatuses.Add(OrderStatus.Completed);

            List<Order> completedOrders = await _omsRepository.GetOrdersByPhoneNumber(userPhoneNumber, orderStatusFilter, true);

            if (completedOrders != null && completedOrders.Count > 0)
            {
                DateTime currentDate = DateTime.UtcNow;

                //if the redemption is happening with in 30 secs, reject it
                Order lastRedeemedOrder = completedOrders.
                                                Where(co => (currentDate.Subtract(co.OrderCompletedDate.Value).TotalSeconds <= _omsAppSettings.OrderRedemptionThrottleInSecs)).FirstOrDefault();

                if (lastRedeemedOrder != null)
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Checks if the user has sufficient balance to redeem
        /// </summary>
        /// <param name="userPhoneNumber"></param>
        /// <param name="balanceRequired"></param>
        /// <returns></returns>
        private async Task<Tuple<bool, double>> HasSufficientBalance(string userPhoneNumber, int balanceRequired)
        {
            double actualBalance = 0;

            EventAggregateData data = await _incentiveEventProxy.GetConsumerCalculatedRegular(userPhoneNumber);

            if (data == null)
            {
                return new Tuple<bool, double>(false, actualBalance);
            }

            actualBalance = data.TotalValue;

            if (actualBalance >= balanceRequired)
            {
                return new Tuple<bool, double>(true, actualBalance);
            }

            return new Tuple<bool, double>(false, actualBalance); ;

        }

        /// <summary>
        /// Returns the populated object for redemption
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userPhoneNumber"></param>
        /// <param name="subscription"></param>
        /// <returns></returns>
        private Order GetOrderForRedemption(Guid userId,
                                            string userPhoneNumber,
                                            ContentProviderSubscriptionDto subscription)
        {
            var currentDate = DateTime.UtcNow;

            Order order = new Order();

            common.dto.Oms.OrderItem orderItem = new common.dto.Oms.OrderItem();

            orderItem.Subscription = subscription;

            orderItem.PlanStartDate = currentDate;

            orderItem.PlanEndDate = currentDate.AddDays(orderItem.Subscription.DurationDays);

            orderItem.RedeemedValue = subscription.RedemptionValue;

            order.OrderCompletedDate = currentDate;

            order.OrderStatus = OrderStatus.Completed;

            order.UserId = userId;

            order.PhoneNumber = userPhoneNumber;

            order.CreatedByUserId = userId;

            order.CreatedDate = currentDate;

            order.IsRedeemed = true;

            order.OrderItems.Add(orderItem);

            return order;
        }


        
        /// <summary>
        /// Checks if valid subscription exists
        /// </summary>
        /// <param name="content"></param>
        /// <param name="errorDetails"></param>
        /// <returns></returns>
        private async Task<bool> IsValidSubcriptionExists(Content content, List<string> errorDetails)
        {
            Guid contentProviderId = content.ContentProviderId;

            bool validSubscriptionExists = false;

            if (content.IsFreeContent)
            {
                _logger.LogInformation($"Returing the token for Content {content.ContentId.Value} for User {this.User.Identity.Name} as it is marked as free");

                validSubscriptionExists = true;

            }
            else
            {
                //Get Orders by phone number
                var orderStatusFilter = new OrderStatusFilter();

                orderStatusFilter.OrderStatuses.Add(OrderStatus.Completed);

                if (await ActiveOrderExists(this.User.Identity.Name, content.ContentProviderId, orderStatusFilter))
                {
                    validSubscriptionExists = true;
                }
                else
                {
                    errorDetails.Add(String.Format(_stringLocalizer["OMS_ERR_0012"], this.User.Identity.Name, content.ContentProviderId));
                }
            }

            return validSubscriptionExists;
        }

        /// <summary>
        /// Generates the token for the Content and Command Id
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="commandId"></param>
        /// <returns></returns>
        private async Task<string> GetContentTokenFromAms(Guid contentId, Guid commandId)
        {
            AmsData amsData = new AmsData();

            amsData.AmsAccountName = _omsAppSettings.AmsAccountName;
            amsData.AmsArmEndPoint = _omsAppSettings.AmsArmEndPoint;
            amsData.AmsClientId = _omsAppSettings.AmsClientId;
            amsData.AmsClientSecret = _omsAppSettings.AmsClientSecret;
            amsData.AmsResourceGroupName = _omsAppSettings.AmsResourceGroupName;
            amsData.AmsSubscriptionId = _omsAppSettings.AmsSubscriptionId;
            amsData.AmsTenantId = _omsAppSettings.AmsTenantId;
            amsData.AmsTokenAudience = _omsAppSettings.AmsTokenAudience;
            amsData.AmsTokenExpiryInMts = _omsAppSettings.AmsTokenExpiryInMts;
            amsData.AmsTokenIssuer = _omsAppSettings.AmsTokenIssuer;
            amsData.AmsTokenSigningKey = _omsAppSettings.AmsTokenSigningKey;
            amsData.AmsTokenMaxUses = _omsAppSettings.AmsTokenMaxUses;

            string token = await AmsUtilities.GetContentToken(amsData, contentId, commandId);

            return token;
        }

        #endregion
    }
}
