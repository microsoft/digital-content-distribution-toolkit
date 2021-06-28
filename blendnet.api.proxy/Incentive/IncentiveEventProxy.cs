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
    /// Proxy for Incentives Events controller
    /// </summary>
    public class IncentiveEventProxy : BaseProxy
    {
        private readonly HttpClient _incentiveEventHttpClient;

        public IncentiveEventProxy(IHttpClientFactory clientFactory,
                           IConfiguration configuration,
                           ILogger<IncentiveEventProxy> logger,
                           IDistributedCache cache)
              : base(configuration, clientFactory, logger, cache)
        {
            _incentiveEventHttpClient = clientFactory.CreateClient(ApplicationConstants.HttpClientKeys.INCENTIVE_HTTP_CLIENT);
        }


       /// <summary>
       /// Returns the consumer accumulated points
       /// </summary>
       /// <param name="userPhoneNumber"></param>
       /// <returns></returns>
        public async Task<EventAggregateData> GetConsumerCalculatedRegular(string userPhoneNumber)
        {
            string url = $"IncentiveEvent/consumer/regular/{userPhoneNumber}";

            string accessToken = await base.GetServiceAccessToken();

            EventAggregateData aggregatedData = null;

            try
            {
                aggregatedData = await _incentiveEventHttpClient.Get<EventAggregateData>(url, accessToken);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            { }

            return aggregatedData;
        }

    }
}
