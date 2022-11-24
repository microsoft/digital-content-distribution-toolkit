// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.common.dto.Oms;
using blendnet.common.infrastructure.Extensions;
using blendnet.oms.api.Model;
using Microsoft.ApplicationInsights;
using System.Collections.Generic;

namespace blendnet.oms.api.Common
{
    /// <summary>
    /// Class to share code between 2 controllers
    /// </summary>
    public class OrderHelper
    {
        private TelemetryClient _telemetryClient;

        public OrderHelper(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Sends order completed AI event 
        /// </summary>
        /// <param name="order"></param>
        public void SendOrderCompletedAIEvent(Order order, Dictionary<string, string> retailerAdditionalAttributes)
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

            OrderCompletedAIEvent orderCompletedAIEvent = new OrderCompletedAIEvent()
            {
                OrderId = order.Id.Value,
                UserId = order.UserId,
                IsRedeemed = order.IsRedeemed,
                OrderCompletedDateTime = System.Text.Json.JsonSerializer.Serialize(order.OrderCompletedDate.Value),
                OrderPlacedDateTime = System.Text.Json.JsonSerializer.Serialize(order.OrderCreatedDate),
                OrderItems = System.Text.Json.JsonSerializer.Serialize(orderItems)
            };

            if (!order.IsRedeemed)
            {
                orderCompletedAIEvent.RetailerPartnerId = order.RetailerPartnerId;
                orderCompletedAIEvent.RetailerPartnerCode = order.RetailerPartnerCode;
                orderCompletedAIEvent.RetailerAdditionalAttributes = retailerAdditionalAttributes;
                orderCompletedAIEvent.PaymentDepositDateTime = System.Text.Json.JsonSerializer.Serialize(order.DepositDate.Value);
            }

            _telemetryClient.TrackEvent(orderCompletedAIEvent);
        }

    }
}
