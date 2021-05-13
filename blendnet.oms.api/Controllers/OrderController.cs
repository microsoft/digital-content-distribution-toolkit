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
using Microsoft.AspNetCore.Authorization;
using static blendnet.common.dto.ApplicationConstants;
using blendnet.common.dto.Retailer;
using blendnet.common.infrastructure.Authentication;

namespace blendnet.oms.api.Controllers
{
    /// <summary>
    /// Controller to manage order related apis
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin, KaizalaIdentityRoles.User)]
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
            List<string> errorInfo = new List<string>();

            Guid userId = UserClaimData.GetUserId(User.Claims);
            string userPhoneNumber = User.Identity.Name;

            // Get Subscription
            ContentProviderSubscriptionDto subscription = await _subscriptionProxy.GetSubscription(orderRequest.ContentProviderId, orderRequest.SubscriptionId);

            if (subscription == null)
            {
                errorInfo.Add("Subscription not found");
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
                errorInfo.Add("User already holds order for same content provider");
                return BadRequest(errorInfo);
            }

            //Populate following in Order object
            // Phone number, user id, user name, content provider id, subscription, Order status, OrderCreatedDate

            Order order = CreateOrder(userId, userPhoneNumber, subscription);

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
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin, KaizalaIdentityRoles.RetailerManagement)]
        public async Task<ActionResult> CompleteOrder(CompleteOrderRequest completeOrderRequest)
        {
            // Get Retailer
            RetailerDto retailer = await _retailerProxy.GetRetailerById(completeOrderRequest.RetailerPartnerProvidedId, PartnerCode.NovoPay /*get this from SAS token when available*/);

            List<string> errorInfo = new List<string>();

            //Validate retailer

            if (!ValidateRetailer(retailer))
            {
                errorInfo .Add("Invalid retailer");
                return BadRequest(errorInfo);
            }

            //Get Order by order id
            Order order = await _omsRepository.GetOrderByOrderId(completeOrderRequest.OrderId, completeOrderRequest.UserPhoneNumber);
            
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
        [HttpGet("summary/{retailerPartnerProvidedId}", Name = nameof(GetOrderSummary))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin, KaizalaIdentityRoles.RetailerManagement)]
        public async Task<ActionResult> GetOrderSummary(string retailerPartnerProvidedId, int startDate, int endDate)
        {
            List<string> errorDetails = new List<string>();

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

            // Get Retailer
            RetailerDto retailer = await _retailerProxy.GetRetailerById(retailerPartnerProvidedId, PartnerCode.NovoPay/*get this from SAS token when available*/);
            
            if (retailer == null)
            {
                errorDetails.Add("Retailer not found");
                return BadRequest(errorDetails);
            }

            List<OrderSummary> purchaseData = await _omsRepository.GetOrderSummary(retailer.PartnerId, startDate, endDate);
            
            if(purchaseData == null || purchaseData.Count == 0)
            {
                return NotFound();
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
            var userPhoneNumber = User.Identity.Name;

            Order order = await _omsRepository.GetOrderByOrderId(orderId, userPhoneNumber);

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
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin, KaizalaIdentityRoles.Retailer)]
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

        /// <summary>
        /// API to get Orders by customer phoneNumber and OrderStatus
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="orderFilter"></param>
        /// <returns></returns>
        [HttpPost("orderlist")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<List<Order>>> GetOrder(OrderStatusFilter orderFilter)
        {
            string phoneNumber = User.Identity.Name;
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

        /// <summary>
        ///  API to get order details by orderId
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet("active")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<OrderItem>> GetActiveSubscriptionOrders()
        {
            var userPhoneNumber = User.Identity.Name;

            List<OrderItem> orderItems = await _omsRepository.GetActiveSubscriptionOrders(userPhoneNumber);

            if (orderItems == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(orderItems);
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

                orderStatusFilter.OrderStatuses.Add(OrderStatus.Completed);

                if(await ActiveOrderExists(this.User.Identity.Name, content.ContentProviderId, orderStatusFilter))
                {
                    validSubscriptionExists = true;
                } 
                else
                {
                    errorDetails.Add($"No Valid Subscription for {this.User.Identity.Name} and {content.ContentProviderId}");
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
            DateTime subscriptionEndDate = subscription.EndDate;

            if (subscriptionEndDate < DateTime.UtcNow)
            {
                return "Subscription is not active";
            }

            return null;
        }

        private async Task<bool> ActiveOrderExists(string userPhoneNumber, Guid contentProviderId, OrderStatusFilter orderFilter)
        {
            List<Order> activeOrders = await _omsRepository.GetOrderByContentProviderId(userPhoneNumber, contentProviderId, orderFilter);

            if (activeOrders != null && activeOrders.Count > 0)
            {
                foreach(Order activeOrder in activeOrders)
                {
                    if(activeOrder.OrderStatus == OrderStatus.Completed && activeOrder.OrderItems[0].PlanEndDate > DateTime.UtcNow)
                    {
                        return true;
                    }

                    if(activeOrder.OrderStatus == OrderStatus.Created)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private Order CreateOrder(Guid userId, string userPhoneNumber, ContentProviderSubscriptionDto subscription)
        {
            Order order = new Order();

            OrderItem orderItem = new OrderItem();
            orderItem.Subscription = subscription;

            order.UserId = userId;
            order.PhoneNumber = userPhoneNumber;

            order.CreatedByUserId = userId;
            order.CreatedDate = DateTime.UtcNow;

            order.OrderItems.Add(orderItem);

            return order;
        }

        private bool ValidateRetailer(RetailerDto retailer)
        {
            if (retailer == null)
            {
                return false;
            }

            return retailer.IsActive();
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
            var currentDate = DateTime.UtcNow;

            order.RetailerId = retailer.Id;
            order.RetailerPartnerId = retailer.PartnerId;
            order.PaymentDepositDate = currentDate.Year * 10000 + currentDate.Month * 100 + currentDate.Day;
            order.DepositDate = completeOrderRequest.DepositDate;

            order.ModifiedByByUserId = UserClaimData.GetUserId(User.Claims);
            order.ModifiedDate = DateTime.UtcNow;

            //assuming single item in order

            order.TotalAmountCollected = completeOrderRequest.AmountCollected;

            var orderItem = order.OrderItems.First();

            orderItem.AmountCollected = completeOrderRequest.AmountCollected;
            orderItem.PlanStartDate = currentDate.Date;
            orderItem.PlanEndDate = currentDate.Date.AddDays(orderItem.Subscription.DurationDays);
            orderItem.PartnerReferenceNumber = completeOrderRequest.PartnerReferenceNumber;

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
