using blendnet.api.proxy.Common;
using blendnet.common.dto;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace blendnet.api.proxy.Cms
{
    /// <summary>
    /// Proxy to ContentProvider API
    /// </summary>
    public class ContentProviderProxy : BaseProxy
    {

        private readonly HttpClient _cmsHttpClient;

        public ContentProviderProxy(IHttpClientFactory clientFactory,
                            IConfiguration configuration,
                            ILogger<ContentProviderProxy> logger,
                            IDistributedCache cache)
               : base(configuration, clientFactory, logger, cache)
        {
            _cmsHttpClient = clientFactory.CreateClient(ApplicationConstants.HttpClientKeys.CMS_HTTP_CLIENT);
        }


        /// <summary>
        /// Get Content Provider by Id
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        public async Task<ContentProviderDto> GetContentProviderById(Guid contentProviderId)
        {
            string url = $"ContentProvider/{contentProviderId}";

            string accessToken = await base.GetServiceAccessToken();

            ContentProviderDto contentProvider = null;

            try
            {
                contentProvider = await _cmsHttpClient.Get<ContentProviderDto>(url, accessToken);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            { }

            return contentProvider;
        }
    }
}
