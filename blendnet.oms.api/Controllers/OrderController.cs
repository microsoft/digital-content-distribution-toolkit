using AutoMapper;
using blendnet.api.proxy;
using blendnet.common.dto;
using blendnet.common.dto.Oms;
using blendnet.common.dto.User;
using blendnet.common.infrastructure;
using blendnet.oms.api.Model;
using blendnet.oms.repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.oms.api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger _logger;

        private IEventBus _eventBus;

        private IOMSRepository _omsRepository;

        public OrderController(IOMSRepository omsRepository,
                                           ILogger<OrderController> logger,
                                           IEventBus eventBus)
        {
            _omsRepository = omsRepository;

            _logger = logger;

            _eventBus = eventBus;
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
            ContentProviderSubscriptionDto subscription = SubscriptionProxy.Instance
                .GetSubscription(orderRequest.ContentProviderId, orderRequest.SubscriptionId);

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
                errorInfo = "Invalid user id or phone number";
                return BadRequest(errorInfo);
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
            if(subscription.Type != ContentProviderContainerType.SubscriptionMetadata)
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
            order.Subscription = subscription;
            order.UserId = (Guid)user.Id;
            order.PhoneNumber = user.PhoneNumber;
            order.UserName = user.UserName;

            return order;
        }

        #endregion
    }
}
