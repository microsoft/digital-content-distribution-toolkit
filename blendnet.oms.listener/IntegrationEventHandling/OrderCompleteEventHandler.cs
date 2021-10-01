using blendnet.api.proxy.Notification;
using blendnet.common.dto.Notification;
using blendnet.common.dto.Events;
using blendnet.common.dto.Oms;
using blendnet.common.infrastructure;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace blendnet.oms.listener.IntegrationEventHandling
{
    public class OrderCompleteEventHandler : IIntegrationEventHandler<OrderCompletedIntegrationEvent>
    {
        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        private readonly NotificationProxy _notificationProxy;

        private readonly OmsAppSettings _appSettings;

        public OrderCompleteEventHandler(ILogger<OrderCompleteEventHandler> logger,
                                                        TelemetryClient tc,
                                                        NotificationProxy notificationProxy,
                                                        IOptionsMonitor<OmsAppSettings> optionsMonitor)
        {
            _logger = logger;
            _telemetryClient = tc;
            _notificationProxy = notificationProxy;
            _appSettings = optionsMonitor.CurrentValue;
        }

        /// <summary>
        /// Handle Complete order notification event
        /// Send request to kms to send notification
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(OrderCompletedIntegrationEvent integrationEvent)
        {
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>("CompleteOrderNotificationIntegrationEvent.Handle"))
                {
                    _logger.LogInformation($"Raising order complete notification for orderid {integrationEvent.Order.Id}");
                    await SendOrderCompleteNotification(integrationEvent.Order);
                  
                    _logger.LogInformation($"Done sending notification for order complete with order id: {integrationEvent.Order.Id}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"CompleteOrderNotificationIntegrationEvent.Handle failed for order id: {integrationEvent.Order.Id}");
            }
        }

        private async Task SendOrderCompleteNotification(Order order)
        {
            List<UserData> userdata = new List<UserData>();
            userdata.Add(new UserData { UserId = order.UserId, PhoneNumber = order.PhoneNumber });

            NotificationRequest notificationRequest = new NotificationRequest()
            {
                Title = "Order Completed",
                Body = "Your order has been completed",
                Type = PushNotificationType.OrderComplete,
                UserData = userdata
            };

            try
            {
                await _notificationProxy.SendNotification(notificationRequest);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Failed to send notification for order id {order.Id} and user id {order.UserId}");
            }

        }
    }
}
