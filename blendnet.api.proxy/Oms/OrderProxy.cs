using blendnet.api.proxy.Common;
using blendnet.common.dto;
using blendnet.common.dto.Oms;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace blendnet.api.proxy.oms
{
    /// <summary>
    /// Makes the call to Order API.
    /// In future, if caching is required, it will be added here.
    /// </summary>
    public class OrderProxy : BaseProxy
    {
        private readonly HttpClient _omsHttpClient;

        public OrderProxy(IHttpClientFactory clientFactory,
                            IConfiguration configuration,
                            ILogger<OrderProxy> logger,
                            IDistributedCache cache)
               : base(configuration, clientFactory, logger, cache)
        {
            _omsHttpClient = clientFactory.CreateClient(ApplicationConstants.HttpClientKeys.OMS_HTTP_CLIENT);
        }

        /// <summary>
        /// Returns the count of orders placed by subscription ID
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="cutoffDate"></param>
        /// <returns></returns>
        public async Task<int> GetOrdersCountBySubscriptionId(Guid subscriptionId, DateTime cutoffDate)
        {
            string url = $"Order/countBySubscription";

            var requestBody = new OrdersCountBySubscriptionRequest()
            {
                CutoffDateTime = cutoffDate,
                SubscriptionId = subscriptionId,
            };

            string accessToken = await base.GetServiceAccessToken();

            int orders = 0;

            orders = await _omsHttpClient.Post<OrdersCountBySubscriptionRequest, int>(url, requestBody, accessToken: accessToken);

            return orders;
        }
    }
}
