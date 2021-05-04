using blendnet.api.proxy.Cms;
using blendnet.common.dto.Cms;
using blendnet.oms.repository.Interfaces;
using blendnet.api.proxy;
using blendnet.api.proxy.Retailer;
using blendnet.common.dto;
using blendnet.common.dto.Oms;
using blendnet.common.dto.User;
using blendnet.oms.api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blendnet.common.infrastructure.Ams;
using Microsoft.Extensions.Options;

namespace blendnet.oms.api.Controllers
{
    /// <summary>
    /// Controller to manage order related apis
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger _logger;

        private IOMSRepository _omsRepository;

        private SubscriptionProxy _subscriptionProxy;

        private ContentProxy _contentProxy;

        private RetailerProxy _retailerProxy;

        OmsAppSettings _omsAppSettings;


        public OrderController(IOMSRepository omsRepository,
                                ILogger<OrderController> logger,
                                ContentProxy contentProxy,
                                RetailerProxy retailerProxy,
                                SubscriptionProxy subscriptionProxy,
                                IOptionsMonitor<OmsAppSettings> optionsMonitor)
        {
            _omsRepository = omsRepository;

            _logger = logger;

            _subscriptionProxy = subscriptionProxy;

            _contentProxy = contentProxy;

            _retailerProxy = retailerProxy;

            _omsAppSettings = optionsMonitor.CurrentValue;
        }

        #region Order management methods
        /// <summary>
        /// Create order 
        /// </summary>
        /// <param name="orderRequest"></param>
        /// <returns></returns>
        [HttpPost("createorder", Name = nameof(CreateOrder))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult> CreateOrder(OrderRequest orderRequest)
        {
            // Get Subscription
            ContentProviderSubscriptionDto subscription = await _subscriptionProxy.GetSubscription(orderRequest.ContentProviderId, orderRequest.SubscriptionId);

            // Get User
            User user = UserProxy.Instance.GetUser(orderRequest.UserId);

            List<string> errorInfo = new List<string>();

            if (user == null || subscription == null)
            {
                errorInfo .Add("User or subscription not found");
                return BadRequest(errorInfo);
            }

            if (!ValidateUser(user, orderRequest.PhoneNumber))
            {
                user.UserName = "Unknown";
                user.Id = orderRequest.UserId;
                user.PhoneNumber = orderRequest.PhoneNumber;

                //errorInfo = "Invalid user id or phone number"; -- change back this once user is created
                //return BadRequest(errorInfo);
            }

            var error = ValidateSubscription(subscription);
            if (error != null)
            {
                errorInfo.Add(error);
                return BadRequest(errorInfo);
            }

            // Check if user do not hold active order for same content provider
            if (await ActiveOrderExists(orderRequest.UserId, orderRequest.ContentProviderId))
            {
                errorInfo.Add("User already holds order for same content provider");
                return BadRequest(errorInfo);
            }

            //Populate following in Order object
            // Phone number, user id, user name, content provider id, subscription, Order status, OrderCreatedDate

            Order order = CreateOrder(user, subscription);

            //Insert order object in db
            Guid orderId = await _omsRepository.CreateOrder(order);

            // return order id to caller

            return Ok(orderId);
        }

        /// <summary>
        /// Complete order
        /// </summary>
        /// <param name="completeOrderRequest"></param>
        /// <returns></returns>
        [HttpPut("completeorder", Name = nameof(CompleteOrder))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult> CompleteOrder(CompleteOrderRequest completeOrderRequest)
        {
            // Get Retailer
            RetailerDto retailer = await _retailerProxy.GetRetailerByPhoneNumber(completeOrderRequest.RetailerPhoneNumber);

            List<string> errorInfo = new List<string>();

            //Validate retailer

            if (!ValidateRetailer(retailer))
            {
                errorInfo .Add("Invalid retailer");
                return BadRequest(errorInfo);
            }

            //Get Order by order id
            Order order = await _omsRepository.GetOrderByOrderId(completeOrderRequest.OrderId);
            
            //Validate order
            var error  = ValidateOrder(order);
            if (error != null)
            {
                errorInfo.Add(error);
                return BadRequest(errorInfo);
            }
            
            
            //validate amount collected
            if(!ValidateAmount(completeOrderRequest))
            {
                errorInfo.Add("Invalid amount collected");
                return BadRequest(errorInfo);
            }

            // Update order object
            UpdateOrder(order, retailer, completeOrderRequest);

            // Update order
            await _omsRepository.UpdateOrder(order);

            return Ok(completeOrderRequest.PartnerReferenceNumber);
        }

        /// <summary>
        /// Cancel order 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpDelete("cancel/{orderId:guid}", Name = nameof(CancelOrder))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult> CancelOrder(Guid orderId)
        {
            //Get Order by order id
            Order order = await _omsRepository.GetOrderByOrderId(orderId);

            List<string> errorInfo = new List<string>();
            if(order == null)
            {
                errorInfo.Add("Order does not exist");
                return BadRequest(errorInfo);
            }

            if(order.OrderStatus != OrderStatus.Created)
            {
                errorInfo.Add("Order is already processed");
                return BadRequest(errorInfo);
            }

            order.OrderStatus = OrderStatus.Cancelled;
            order.OrderCancelledDate = DateTime.UtcNow;

            // Update order
            await _omsRepository.UpdateOrder(order);

            return Ok(orderId);

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
                errorDetails.Add($"No valid details found for givent content id {contentId}");

                return BadRequest(errorDetails);
            }

            //Check the the content transformation status
            if (content.ContentTransformStatus != ContentTransformStatus.TransformComplete)
            {
                errorDetails.Add($"The content tranform status should be complete. Current status is {content.ContentTransformStatus}");

                return BadRequest(errorDetails);
            }

            //Check if Valid Subcription Exists
            if (! await IsValidSubcriptionExists(content,errorDetails))
            {
                return BadRequest(errorDetails);
            }

            string token = await GetContentTokenFromAms(content.ContentId.Value, content.ContentTransformStatusUpdatedBy.Value);
            
            return token;
        }

        /// <summary>
        /// API to get order summary of the retailer
        /// </summary>
        /// <param name="retailerPhoneNumber">Phone number of retailer</param>
        /// <param name="startDate">Start date in numeric format yyyymmdd</param>
        /// <param name="endDate">End date in numeric format yyyymmdd</param>
        /// <returns></returns>
        [HttpGet("summary/{retailerPhoneNumber}", Name = nameof(GetOrderSummary))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult> GetOrderSummary(string retailerPhoneNumber, int startDate, int endDate)
        {
            // Get Retailer
            RetailerDto retailer = await _retailerProxy.GetRetailerByPhoneNumber(retailerPhoneNumber);

            List<string> errorDetails = new List<string>();

            if(retailer == null)
            {
                errorDetails.Add("Retailer not found");
                return BadRequest(errorDetails);
            }

            if(startDate == 0 || endDate == 0)
            {
                errorDetails.Add("Invalid start or end date");
                return BadRequest(errorDetails);
            }

            if(startDate > endDate)
            {
                errorDetails.Add("Invalid date range");
                return BadRequest(errorDetails);
            }

            List<OrderSummary> purchaseData = await _omsRepository.GetOrderSummary(retailerPhoneNumber, startDate, endDate);
            
            if(purchaseData == null || purchaseData.Count == 0)
            {
                errorDetails.Add("No data found");
                return NotFound(errorDetails);
            }

            return Ok(purchaseData);
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
            Order order = await _omsRepository.GetOrderByOrderId(orderId);

            if (order == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(order);
            }
        }

        /// <summary>
        /// API to get Orders by customer phoneNumber and OrderStatus
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="orderFilter"></param>
        /// <returns></returns>
        [HttpPost("{phoneNumber}/orderlist")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<List<Order>>> GetOrder(string phoneNumber, OrderStatusFilter orderFilter)
        {

            List<Order> orderlist = await _omsRepository.GetOrdersByPhoneNumber(phoneNumber, orderFilter);

            if (orderlist.Count() > 0)
            {
                return Ok(orderlist);
            }
            else
            {
                return NotFound();
            }
        }


        #endregion

        #region private methods

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

                orderStatusFilter.OrderStatuses = new List<OrderStatus>();

                orderStatusFilter.OrderStatuses.Add(OrderStatus.Completed);

                List<Order> orders = await _omsRepository.GetOrdersByPhoneNumber(this.User.Identity.Name, orderStatusFilter);

                if (orders == null || orders.Count <= 0)
                {
                    errorDetails.Add($"Not Active Subscription for {this.User.Identity.Name}");

                    return false;
                }

                DateTime currentDateTime = DateTime.UtcNow;

                foreach (Order order in orders)
                {
                    OrderItem orderItem = order.OrderItems.Where(oi => oi.Subscription.ContentProviderId == content.ContentProviderId).FirstOrDefault();

                    if (order.OrderStatus == OrderStatus.Completed &&
                        orderItem != null &&
                        (currentDateTime >= orderItem.PlanStartDate && currentDateTime <= orderItem.PlanEndDate))
                    {
                        validSubscriptionExists = true;

                        break;
                    }
                }

                if (!validSubscriptionExists)
                {
                    errorDetails.Add($"Not Valid Subscription for {this.User.Identity.Name} and {content.ContentProviderId}");
                }

            }

            return validSubscriptionExists;
        }

        /// <summary>
        /// Validates user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        private bool ValidateUser(User user, string phoneNumber)
        {
            return user != null && user.PhoneNumber != null && user.PhoneNumber.Equals(phoneNumber);
        }

        /// <summary>
        /// Validates subscription
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        private string ValidateSubscription(ContentProviderSubscriptionDto subscription)
        {
            if (subscription.Type != ContentProviderContainerType.Subscription)
            {
                return "Invalid subscription type";
            }

            DateTime subscriptionEndDate = subscription.EndDate;

            if (subscriptionEndDate < DateTime.UtcNow)
            {
                return "Subscription is not active";
            }

            return null;
        }

        private async Task<bool> ActiveOrderExists(Guid userId, Guid contentProviderId)
        {
            List<Order> activeOrders = await _omsRepository.GetOrder(userId, contentProviderId, returnAll:true);

            if (activeOrders != null && activeOrders.Count > 0)
            {
                foreach(Order activeOrder in activeOrders)
                {
                    if(activeOrder.OrderStatus == OrderStatus.Created)
                    {
                        return true;
                    }
                    if(activeOrder.OrderStatus == OrderStatus.Completed && activeOrder.OrderItems[0].PlanEndDate > DateTime.UtcNow)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private Order CreateOrder(User user, ContentProviderSubscriptionDto subscription)
        {
            Order order = new Order();

            OrderItem orderItem = new OrderItem();
            orderItem.Subscription = subscription;

            order.UserId = (Guid)user.Id;
            order.PhoneNumber = user.PhoneNumber;
            order.UserName = user.UserName;
            order.OrderItems.Add(orderItem);

            return order;
        }

        private bool ValidateRetailer(RetailerDto retailer)
        {
            if (retailer == null)
            {
                return false;
            }

            return retailer.IsActive;
        }

        private string ValidateOrder(Order order)
        {
            string errorInfo = null;

            if(order == null)
            {
                errorInfo = "Order does not exist";
            }
            else if(order.OrderStatus != OrderStatus.Created)
            {
                errorInfo = "Order is not active";
            }

            return errorInfo;
        }

        private bool ValidateAmount(CompleteOrderRequest completeOrderRequest)
        {
            if(completeOrderRequest.AmountCollected < 0)
            {
                return false;
            }

            return true;
        }

        private void UpdateOrder(Order order, RetailerDto retailer, CompleteOrderRequest completeOrderRequest)
        {
            order.RetailerId = retailer.Id;
            order.RetailerName = retailer.GetName();
            order.RetailerPhoneNumber = retailer.Mobile;

            //assuming single item in order

            order.TotalAmountCollected = completeOrderRequest.AmountCollected;

            var orderItem = order.OrderItems.First();
            var currentDate = DateTime.UtcNow;

            orderItem.AmountCollected = completeOrderRequest.AmountCollected;
            orderItem.PlanStartDate = currentDate.Date;
            orderItem.PlanEndDate = currentDate.Date.AddDays(orderItem.Subscription.DurationDays);
            orderItem.PartnerReferenceNumber = completeOrderRequest.PartnerReferenceNumber;
            orderItem.PaymentDepositDate = currentDate.Year * 10000 + currentDate.Month * 100 + currentDate.Day;

            order.OrderCompletedDate = currentDate;
            order.OrderStatus = OrderStatus.Completed;
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

            string token = await AmsUtilities.GetContentToken(amsData, contentId, commandId);

            return token;
        }

        #endregion
    }
}
