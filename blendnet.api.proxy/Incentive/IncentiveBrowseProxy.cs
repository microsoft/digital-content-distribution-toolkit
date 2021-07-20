using blendnet.api.proxy.Common;
using blendnet.common.dto;
using blendnet.common.dto.Incentive;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.api.proxy.Incentive
{
    /// <summary>
    /// Proxy to invoke Incentive Browse Controller
    /// </summary>
    public class IncentiveBrowseProxy : BaseProxy
    {
        private readonly HttpClient _incentiveBrowseHttpClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="cache"></param>
        public IncentiveBrowseProxy(IHttpClientFactory clientFactory,
                           IConfiguration configuration,
                           ILogger<IncentiveEventProxy> logger,
                           IDistributedCache cache)
              : base(configuration, clientFactory, logger, cache)
        {
            _incentiveBrowseHttpClient = clientFactory.CreateClient(ApplicationConstants.HttpClientKeys.INCENTIVE_HTTP_CLIENT);
        }

        /// <summary>
        /// Get the consumer Active Plan
        /// </summary>
        /// <param name="planType"></param>
        /// <returns></returns>
        public async Task<IncentivePlan> GetConsumerActivePlan(PlanType planType)
        {
            string url = $"IncentiveBrowse/consumer/active/{planType}";

            string accessToken = await base.GetServiceAccessToken();

            IncentivePlan activeIncentivePlan = null;

            try
            {
                activeIncentivePlan = await _incentiveBrowseHttpClient.Get<IncentivePlan>(url, accessToken);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            { }

            return activeIncentivePlan;
        }
    }
}
