using blendnet.api.proxy.Common;
using blendnet.common.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.api.proxy.Cms
{
    /// <summary>
    /// Makes the call to Subscription API.
    /// In future, if caching is required, it will be added here.
    /// </summary>
    public class SubscriptionProxy
    {
        private readonly HttpClient _cmsHttpClient;

        public SubscriptionProxy(IHttpClientFactory clientFactory)
        {
            _cmsHttpClient = clientFactory.CreateClient(ApplicationConstants.HttpClientKeys.CMS_HTTP_CLIENT);
        }

        /// <summary>
        /// Get Subscriptions for the given content provider Id
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        public async Task<List<ContentProviderSubscriptionDto>> GetSubscriptions(Guid contentProviderId)
        {
            string url = $"ContentProvider/{contentProviderId}/Subscription";

            var successResponse = await _cmsHttpClient.Get<List<ContentProviderSubscriptionDto>>(url);

            return successResponse;
        }

        /// <summary>
        /// Get the Subcription Details
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        public async Task<ContentProviderSubscriptionDto> GetSubscription(Guid contentProviderId, Guid subscriptionId)
        {
            string url = $"ContentProvider/{contentProviderId}/Subscription/{subscriptionId}";

            var successResponse = await _cmsHttpClient.Get<ContentProviderSubscriptionDto>(url);

            return successResponse;
        }
    }
}
