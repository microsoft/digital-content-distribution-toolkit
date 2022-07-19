using AutoMapper;
using blendnet.api.proxy;
using blendnet.api.proxy.Cms;
using blendnet.api.proxy.Device;
using blendnet.api.proxy.Retailer;
using blendnet.common.dto.Cms;
using blendnet.common.dto.Device;
using blendnet.common.dto.Events;
using blendnet.common.dto.Oms;
using blendnet.common.dto.Retailer;
using blendnet.common.dto.User;
using blendnet.common.infrastructure;
using blendnet.common.infrastructure.Authentication;
using blendnet.oms.api.Common;
using blendnet.oms.api.Model;
using blendnet.oms.repository.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static blendnet.common.dto.ApplicationConstants;

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

        private readonly IOMSRepository _omsRepository;

        private readonly RetailerProxy _retailerProxy;

        private readonly RetailerProviderProxy _retailerProviderProxy;

        private readonly SubscriptionProxy _subscriptionProxy;

        private readonly UserProxy _userProxy;

        private readonly IEventBus _eventBus;

        private readonly OmsAppSettings _omsAppSettings;

        private readonly IStringLocalizer<SharedResource> _stringLocalizer;

        private readonly TelemetryClient _telemetryClient;

        private readonly OrderHelper _orderHelper;

        private readonly DeviceProxy _deviceProxy;

        private readonly ContentProxy _contentProxy;

        private readonly IMapper _mapper;

        public OrderController(IOMSRepository omsRepository,
                                ILogger<OrderController> logger,
                                RetailerProxy retailerProxy,
                                RetailerProviderProxy retailerProviderProxy,
                                SubscriptionProxy subscriptionProxy,
                                UserProxy userProxy,
                                IEventBus eventBus,
                                IOptionsMonitor<OmsAppSettings> optionsMonitor,
                                IStringLocalizer<SharedResource> stringLocalizer,
                                TelemetryClient telemetryClient,
                                OrderHelper orderHelper,
                                DeviceProxy deviceProxy,
                                IMapper mapper,
                                ContentProxy contentProxy)
        {
            _omsRepository = omsRepository;

            _logger = logger;

            _retailerProxy = retailerProxy;

            _retailerProviderProxy = retailerProviderProxy;

            _subscriptionProxy = subscriptionProxy;

            _userProxy = userProxy;

            _eventBus = eventBus;

            _omsAppSettings = optionsMonitor.CurrentValue;

            _stringLocalizer = stringLocalizer;

            _telemetryClient = telemetryClient;

            _orderHelper = orderHelper;

            _deviceProxy = deviceProxy;

            _contentProxy = contentProxy;

            _mapper = mapper;
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

            errorInfo = ValidateRequestParams(completeOrderRequest.RetailerPartnerCode);

            if(errorInfo.Count != 0)
            {
                return BadRequest(errorInfo);
            }

            User user = await _userProxy.GetUserByPhoneNumber(completeOrderRequest.UserPhoneNumber);
            
            if (user is null || user.AccountStatus != UserAccountStatus.Active)
            {
                errorInfo.Add(_stringLocalizer["OMS_ERR_0022"]);
                return BadRequest();
            }

            RetailerProviderDto retailerProvider = await GetRetailerProvider(completeOrderRequest.RetailerPartnerCode);
                
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
            User user = await _userProxy.GetUserByPhoneNumber(userPhoneNumber);

            if (user is null || user.AccountStatus != UserAccountStatus.Active)
            {
                _logger.LogInformation("Requested user not found or not active");
                return NotFound();
            }

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
        /// API to get Created Orders by customer phoneNumber
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{phoneNumber}/orderlist", Name = nameof(GetCreatedOrderForUser))]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        [AuthorizeRoles(KaizalaIdentityRoles.SuperAdmin, KaizalaIdentityRoles.Retailer, KaizalaIdentityRoles.RetailerManagement)]
        public async Task<ActionResult<List<CreatedOrderResponse>>> GetCreatedOrderForUser(string phoneNumber, GetCreatedOrderRequest request)
        {

            List<string> errorInfo = new List<string>();

            errorInfo = ValidateRequestParams(request.RetailerPartnerCode);

            if (errorInfo.Count != 0)
            {
                return BadRequest(errorInfo);
            }

            // Validate User exist in system
            User user = await _userProxy.GetUserByPhoneNumber(phoneNumber);

            if (user is null || user.AccountStatus != UserAccountStatus.Active)
            {
                _logger.LogInformation("Requested user not found or not active");
                return NotFound();
            }

            // Get Retailer
            RetailerProviderDto retailerProvider = await GetRetailerProvider(request.RetailerPartnerCode);

            if (retailerProvider == null)
            {
                errorInfo.Add(_stringLocalizer["OMS_ERR_0016"]);
                return BadRequest(errorInfo);
            }

            RetailerDto retailer = await _retailerProxy.GetRetailerById(request.RetailerPartnerProvidedId, retailerProvider.PartnerCode);

            //Validate retailer

            if (!ValidateRetailer(retailer))
            {
                errorInfo.Add(_stringLocalizer["OMS_ERR_0004"]);
                return BadRequest(errorInfo);
            }

            OrderStatusFilter orderFilter = new OrderStatusFilter();
            orderFilter.OrderStatuses.Add(OrderStatus.Created);

            List<Order> orderlist = await _omsRepository.GetOrdersByPhoneNumber(phoneNumber, orderFilter);

            if (orderlist.Count == 0)
            {
                return NotFound();
            }
            
            List<Guid> contentProviderIds = GetContentProviderIds(orderlist);

            List<DeviceContentByContentProviderIdResponse> contentInDevice = new List<DeviceContentByContentProviderIdResponse>();

            RetailerDeviceAssignment device = retailer.DeviceAssignments.Where(assignment => assignment.IsActive).FirstOrDefault();

            if (device != null && contentProviderIds.Count > 0)
            {
                contentInDevice = await _deviceProxy.GetContentByDeviceId(device.DeviceId, contentProviderIds);
            }

            List<Guid> absentContentIds = GetContentsNotPresentInDevice(contentInDevice, orderlist);

            List<ContentInfo> contents = new List<ContentInfo>();

            if (absentContentIds.Count > 0)
            {
                contents = await _contentProxy.GetContentProviderIds(absentContentIds);
            }

            List<CreatedOrderResponse> response = MapOrderAndContentNotPresent(orderlist, contents);

            return Ok(response);
            
           
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
            orderItem.PlanEndDate = currentDate.AddDays(orderItem.Subscription.DurationDays);
            orderItem.PartnerReferenceNumber = completeOrderRequest.PartnerReferenceNumber;

            order.OrderCompletedDate = currentDate;
            order.OrderStatus = OrderStatus.Completed;
        }

        private List<string> ValidateRequestParams(string retailerPartnerCode)
        {
            List<string> errorInfo = new List<string>();

            if(User.IsInRole(KaizalaIdentityRoles.RetailerManagement))
            {
                if(!string.IsNullOrEmpty(retailerPartnerCode))
                {
                    errorInfo.Add(_stringLocalizer["OMS_ERR_0020"]);
                    return errorInfo;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(retailerPartnerCode))
                {
                    errorInfo.Add(_stringLocalizer["OMS_ERR_0021"]);
                    return errorInfo;
                }
            }

            return errorInfo;
        } 

        private async Task<RetailerProviderDto> GetRetailerProvider(string retailerPartnerCode)
        {
            RetailerProviderDto retailerProvider = null;

            if(User.IsInRole(KaizalaIdentityRoles.RetailerManagement))
            {
                Guid callerUserId = UserClaimData.GetUserId(User.Claims);
                retailerProvider = await _retailerProviderProxy.GetRetailerProviderByUserId(callerUserId);
            }
            else
            {
                retailerProvider = await _retailerProviderProxy.GetRetailerProviderByPartnerCode(retailerPartnerCode);
            }

            return retailerProvider;
        }

        // Returns the list of contents that are not present in retailer device
        private List<Guid> GetContentsNotPresentInDevice(List<DeviceContentByContentProviderIdResponse> deviceContents, List<Order> orders)
        {
            List<Guid> absentContentIds = new List<Guid>();
            List<Guid> contentIdInOrders = new List<Guid>();
            List<Guid> deviceContentIds = deviceContents.Select(c => c.ContentId).ToList();

            orders.ForEach(order =>
            {
                order.OrderItems.ForEach(item =>
                {
                    if (item.Subscription.SubscriptionType == common.dto.SubscriptionType.TVOD)
                    {
                        contentIdInOrders.AddRange(item.Subscription.ContentIds);
                    }
                });
            });

            contentIdInOrders.ForEach(id =>
            {
                // Add contentId to the absent list if it is not present in device content list and also not added in absent content list
                if (!deviceContentIds.Contains(id) && !absentContentIds.Contains(id))
                {
                    absentContentIds.Add(id);
                }
            });

            return absentContentIds;
        }

        private List<Guid> GetContentProviderIds(List<Order> orderlist)
        {
            List<Guid> contentProviderIds = new List<Guid>();

            orderlist.ForEach(order =>
            {
                order.OrderItems.ForEach(item =>
                {
                    if (item.Subscription.SubscriptionType == common.dto.SubscriptionType.TVOD && !contentProviderIds.Contains(item.Subscription.ContentProviderId))
                    {
                        contentProviderIds.Add(item.Subscription.ContentProviderId);
                    }
                });

            });

            return contentProviderIds;
        }

        private List<CreatedOrderResponse> MapOrderAndContentNotPresent(List<Order> orders, List<ContentInfo> contents)
        {
            List<CreatedOrderResponse> response = new List<CreatedOrderResponse>();
            orders.ForEach(order =>
            {
                List<ContentInfo> contentNotPresent = new List<ContentInfo>();
                order.OrderItems.ForEach(item =>
                {
                    contents.ForEach(c =>
                    {
                        if (item.Subscription.ContentIds != null && item.Subscription.ContentIds.Contains(c.ContentId))
                        {
                            contentNotPresent.Add(c);
                        }
                    });
                });
                CreatedOrderResponse createdOrderResponse = new CreatedOrderResponse();
                createdOrderResponse.Order = order;
                createdOrderResponse.Contents = contentNotPresent;
                response.Add(createdOrderResponse);
            });

            return response;
        }



        #endregion
    }
}
