using blendnet.oms.repository.Interfaces;
using blendnet.api.proxy;
using blendnet.api.proxy.Retailer;
using blendnet.common.dto.Oms;
using blendnet.common.dto.User;
using blendnet.oms.api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using static blendnet.common.dto.ApplicationConstants;
using blendnet.common.dto.Retailer;
using blendnet.common.infrastructure.Authentication;
using blendnet.common.dto.Events;
using Microsoft.Extensions.Localization;
using blendnet.common.infrastructure;
using Microsoft.ApplicationInsights;
using blendnet.oms.api.Common;

namespace blendnet.oms.api.Controllers
{
    /// <summary>
    /// Controller to manage order related apis
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin,KaizalaIdentityRoles.RetailerManagement, KaizalaIdentityRoles.Retailer)]
    public class OrderController : ControllerBase
    {
        private readonly ILogger _logger;

        private IOMSRepository _omsRepository;

        private RetailerProxy _retailerProxy;

        private RetailerProviderProxy _retailerProviderProxy;

        private readonly SubscriptionProxy _subscriptionProxy;

        private IEventBus _eventBus;

        private OmsAppSettings _omsAppSettings;

        IStringLocalizer<SharedResource> _stringLocalizer;

        private TelemetryClient _telemetryClient;

        private OrderHelper _orderHelper;

        public OrderController(IOMSRepository omsRepository,
                                ILogger<OrderController> logger,
                                RetailerProxy retailerProxy,
                                RetailerProviderProxy retailerProviderProxy,
                                SubscriptionProxy subscriptionProxy,
                                IEventBus eventBus,
                                IOptionsMonitor<OmsAppSettings> optionsMonitor,
                                IStringLocalizer<SharedResource> stringLocalizer,
                                TelemetryClient telemetryClient,
                                OrderHelper orderHelper)
        {
            _omsRepository = omsRepository;

            _logger = logger;

            _retailerProxy = retailerProxy;

            _retailerProviderProxy = retailerProviderProxy;

            _subscriptionProxy = subscriptionProxy;

            _eventBus = eventBus;

            _omsAppSettings = optionsMonitor.CurrentValue;

            _stringLocalizer = stringLocalizer;

            _telemetryClient = telemetryClient;

            _orderHelper = orderHelper;
        }

        #region Order management methods
      

        /// <summary>
        /// Complete order
        /// </summary>
        /// <param name="completeOrderRequest"></param>
        /// <returns></returns>
        [HttpPut("completeorder", Name = nameof(CompleteOrder))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin, KaizalaIdentityRoles.RetailerManagement,
            KaizalaIdentityRoles.Retailer)]
        public async Task<ActionResult> CompleteOrder(CompleteOrderRequest completeOrderRequest)
        {
            List<string> errorInfo = new List<string>();

            errorInfo = ValidateRequestParams(completeOrderRequest);

            if(errorInfo.Count != 0)
            {
                return BadRequest(errorInfo);
            }

            RetailerProviderDto retailerProvider = await GetRetailerProvider(completeOrderRequest);
                
            if (retailerProvider == null)
            {
                errorInfo.Add(_stringLocalizer["OMS_ERR_0016"]);
                return BadRequest(errorInfo);
            }

            // Get Retailer
            RetailerDto retailer = await _retailerProxy.GetRetailerById(completeOrderRequest.RetailerPartnerProvidedId, retailerProvider.PartnerCode);

            //Validate retailer

            if (!ValidateRetailer(retailer))
            {
                errorInfo .Add(_stringLocalizer["OMS_ERR_0004"]);
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
            if(!ValidateAmount(completeOrderRequest, order))
            {
                errorInfo.Add(_stringLocalizer["OMS_ERR_0003"]);
                return BadRequest(errorInfo);
            }

            // Update order object
            UpdateOrder(order, retailer, completeOrderRequest);

            // Update order
            var statusCode = await _omsRepository.UpdateOrder(order);

            if (statusCode == (int)System.Net.HttpStatusCode.OK)
            {
                OrderCompletedIntegrationEvent orderCompletedIntegrationEvent = new OrderCompletedIntegrationEvent()
                {
                    Order = order,
                };

                await _eventBus.Publish(orderCompletedIntegrationEvent);

                _orderHelper.SendOrderCompletedAIEvent(order, retailer.AdditionalAttibutes);

                return NoContent();
            }
            else
            {
                return NotFound();
            }
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
        [AuthorizeRoles(KaizalaIdentityRoles.RetailerManagement)]
        public async Task<ActionResult> GetOrderSummary(string retailerPartnerProvidedId, int startDate, int endDate)
        {
            List<string> errorDetails = new List<string>();

            if(startDate == 0 || endDate == 0)
            {
                errorDetails.Add(_stringLocalizer["OMS_ERR_0009"]);
                return BadRequest(errorDetails);
            }

            if(startDate > endDate)
            {
                errorDetails.Add(_stringLocalizer["OMS_ERR_0010"]);
                return BadRequest(errorDetails);
            }

            Guid callerUserId = UserClaimData.GetUserId(User.Claims);
            RetailerProviderDto retailerProvider = await _retailerProviderProxy.GetRetailerProviderByUserId(callerUserId);
            if (retailerProvider == null)
            {
                errorDetails.Add(_stringLocalizer["OMS_ERR_0016"]);
                return BadRequest(errorDetails);
            }

            // Get Retailer
            RetailerDto retailer = await _retailerProxy.GetRetailerById(retailerPartnerProvidedId, retailerProvider.PartnerCode);
            
            if (retailer == null)
            {
                errorDetails.Add(_stringLocalizer["OMS_ERR_0011"]);
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
        ///  API to get order details by orderId by retailer managemet
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet("{orderId:guid}/{userPhoneNumber}", Name = nameof(GetOrderByOrderIdAndPhoneNumber))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin, KaizalaIdentityRoles.RetailerManagement)]
        public async Task<ActionResult<Order>> GetOrderByOrderIdAndPhoneNumber(Guid orderId, string userPhoneNumber)
        {
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
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin, KaizalaIdentityRoles.Retailer, KaizalaIdentityRoles.RetailerManagement)]
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
        /// API to get count of orders by subscription ID
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("countBySubscription", Name = nameof(GetOrdersCountBySubscription))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin)]
        public async Task<ActionResult<List<Order>>> GetOrdersCountBySubscription(OrdersCountBySubscriptionRequest request)
        {
            var subscription = await _subscriptionProxy.GetSubscription(request.ContentProviderId, request.SubscriptionId);

            if (subscription == null)
            {
                return NotFound();
            }

            var ordersCount = await _omsRepository.GetOrdersCountBySubscriptionId(request.SubscriptionId, request.CutoffDateTime);
            return Ok(ordersCount);
        }

        #endregion

        #region private methods
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


        private bool ValidateRetailer(RetailerDto retailer)
        {
            if (retailer == null)
            {
                return false;
            }

            return retailer.IsActive();
        }

        /// <summary>
        /// Checks if the order state is created or not
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private string ValidateOrder(Order order)
        {
            string errorInfo = null;

            if(order == null)
            {
                errorInfo = _stringLocalizer["OMS_ERR_0014"];
            }
            else if(order.OrderStatus != OrderStatus.Created)
            {
                errorInfo = _stringLocalizer["OMS_ERR_0015"];
            }

            return errorInfo;
        }

        /// <summary>
        /// Checks whether Amount collected is < 0
        /// </summary>
        /// <param name="completeOrderRequest"></param>
        /// <returns></returns>
        private bool ValidateAmount(CompleteOrderRequest completeOrderRequest, Order order)
        {
            if(completeOrderRequest.AmountCollected < 0)
            {
                return false;
            }

            float expectedTotal = 0;

            foreach(var orderItem in order.OrderItems)
            {
                expectedTotal += orderItem.Subscription.Price;
            }

            if(expectedTotal != completeOrderRequest.AmountCollected)
            {
                return false;
            }

            return true;
        }

        private void UpdateOrder(Order order, RetailerDto retailer, CompleteOrderRequest completeOrderRequest)
        {
            var currentDate = DateTime.UtcNow;

            order.RetailerPartnerId = retailer.PartnerId;
            order.RetailerPartnerCode = retailer.PartnerCode;
            order.PaymentDepositDate = currentDate.Year * 10000 + currentDate.Month * 100 + currentDate.Day;
            order.DepositDate = completeOrderRequest.DepositDate;

            order.ModifiedByByUserId = UserClaimData.GetUserId(User.Claims);
            order.ModifiedDate = DateTime.UtcNow;

            //assuming single item in order

            order.TotalAmountCollected = completeOrderRequest.AmountCollected;

            var orderItem = order.OrderItems.First();

            orderItem.AmountCollected = completeOrderRequest.AmountCollected;
            orderItem.PlanStartDate = currentDate;
            orderItem.PlanEndDate = currentDate.Date.AddDays(orderItem.Subscription.DurationDays).AddMilliseconds(1);//to ensure millisecond part is shown
            orderItem.PartnerReferenceNumber = completeOrderRequest.PartnerReferenceNumber;

            order.OrderCompletedDate = currentDate;
            order.OrderStatus = OrderStatus.Completed;
        }

        private List<string> ValidateRequestParams(CompleteOrderRequest completeOrderRequest)
        {
            List<string> errorInfo = new List<string>();

            if(User.IsInRole(KaizalaIdentityRoles.RetailerManagement))
            {
                if(!string.IsNullOrEmpty(completeOrderRequest.RetailerPartnerCode))
                {
                    errorInfo.Add(_stringLocalizer["OMS_ERR_0020"]);
                    return errorInfo;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(completeOrderRequest.RetailerPartnerCode))
                {
                    errorInfo.Add(_stringLocalizer["OMS_ERR_0021"]);
                    return errorInfo;
                }
            }

            return errorInfo;
        } 

        private async Task<RetailerProviderDto> GetRetailerProvider(CompleteOrderRequest completeOrderRequest)
        {
            RetailerProviderDto retailerProvider = null;

            if(User.IsInRole(KaizalaIdentityRoles.RetailerManagement))
            {
                Guid callerUserId = UserClaimData.GetUserId(User.Claims);
                retailerProvider = await _retailerProviderProxy.GetRetailerProviderByUserId(callerUserId);
            }
            else
            {
                retailerProvider = await _retailerProviderProxy.GetRetailerProviderByPartnerCode(completeOrderRequest.RetailerPartnerCode);
            }

            return retailerProvider;
        }

      


        #endregion
    }
}
