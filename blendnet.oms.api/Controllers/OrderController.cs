using AutoMapper;
using blendnet.api.proxy;
using blendnet.api.proxy.Cms;
using blendnet.api.proxy.Retailer;
using blendnet.common.dto;
using blendnet.common.dto.Oms;
using blendnet.common.dto.User;
using blendnet.common.infrastructure;
using blendnet.oms.api.Model;
using blendnet.oms.repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace blendnet.oms.api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger _logger;

        private IOMSRepository _omsRepository;

        private SubscriptionProxy _subscriptionProxy;

        public OrderController(ILogger<WeatherForecastController> logger,
            ContentProxy contentProxy, 
            RetailerProxy retailerProxy,
            SubscriptionProxy subscriptionProxy, 
            IOMSRepository omsRepository)
        {
            _omsRepository = omsRepository;

            _logger = logger;

            _subscriptionProxy = subscriptionProxy;
        }

        #region Order management methods
        /// <summary>
        /// Upload Contents
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult> CreateOrder(OrderRequest orderRequest)
        {
            // Get Subscription
            ContentProviderSubscriptionDto subscription = await _subscriptionProxy.GetSubscription(orderRequest.ContentProviderId, orderRequest.SubscriptionId);

            // Get User
            User user = UserProxy.Instance.GetUser(orderRequest.UserId);

            string errorInfo = null;

            if (user == null || subscription == null)
            {
                errorInfo = "User or subscription not found";
                return BadRequest(errorInfo);
            }

            if(!ValidateUser(user, orderRequest.PhoneNumber))
            {
                user.UserName = "Unknown";
                user.Id = orderRequest.UserId;
                user.PhoneNumber = orderRequest.PhoneNumber;

                //errorInfo = "Invalid user id or phone number"; -- change back this once user is created
                //return BadRequest(errorInfo);
            }

            errorInfo = ValidateSubscription(subscription);
            if(errorInfo != null)
            {
                return BadRequest(errorInfo);
            }

            // Check if user do not hold active order for same content provider
            if(await ActiveOrderExists(orderRequest.UserId, orderRequest.ContentProviderId))
            {
                errorInfo = "User already holds order for same content provider";
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


        [HttpPost]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Create))]
        public async Task<ActionResult> AssignRetailer(AssignRetailerRequest assignRequest)
        {
            // Get Retailer



            //Validate retailer

            //Get Order by order id

            //Validate order

            //validate amount collected

            // Update order object

            // Update order


            return Ok(assignRequest.PartnerReferenceNumber);
        }


        #endregion

            #region private methods

            private bool ValidateUser(User user, string phoneNumber)
        {
            return user.PhoneNumber != null && user.PhoneNumber.Equals(phoneNumber);
        }

        private string ValidateSubscription(ContentProviderSubscriptionDto subscription)
        {
            // Validate Subscription
            // Subscription exists
            // Validate subscription is active
            if(subscription.Type != ContentProviderContainerType.Subscription)
            {
                return "Invalid subscription type";
            }

            DateTime subscriptionEndDate = subscription.EndDate;

            if(subscriptionEndDate < DateTime.UtcNow)
            {
                return "Subscription is not active";
            }

            return null;
        }

        private async Task<bool> ActiveOrderExists(Guid userId, Guid contentProviderId)
        {
            List<Order> activeOrders = await _omsRepository.GetOrder(userId, contentProviderId);

            if(activeOrders != null && activeOrders.Count > 0)
            {
                return true;
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

        #endregion
    }
}
