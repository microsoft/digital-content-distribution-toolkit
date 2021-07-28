using blendnet.api.proxy.Common;
using blendnet.common.dto;
using blendnet.notification.api.Model;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace blendnet.api.proxy.Notification
{
    public class NotificationProxy : BaseProxy
    {
        private readonly HttpClient _notificationHttpClient;

        private ILogger<NotificationProxy> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="cache"></param>
        public NotificationProxy(IHttpClientFactory clientFactory,
                           IConfiguration configuration,
                           ILogger<NotificationProxy> logger,
                           IDistributedCache cache)
              : base(configuration, clientFactory, logger, cache)
        {
            _notificationHttpClient = clientFactory.CreateClient(ApplicationConstants.HttpClientKeys.NOTIFICATION_HTTP_CLIENT);
            _logger = logger;
        }

        public async Task SendNotification(NotificationRequest notificationRequest)
        {
            string url = $"Notification/send";

            string accessToken = await base.GetServiceAccessToken();

            _notificationHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            await _notificationHttpClient.Post<NotificationRequest, object>(url, notificationRequest, false);

        }
    }
}
