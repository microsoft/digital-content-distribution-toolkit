using blendnet.api.proxy.Common;
using blendnet.common.dto;
using blendnet.common.dto.Cms;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.api.proxy.Cms
{
    /// <summary>
    /// Makes the call to Content API.
    /// In future, if caching is required, it will be added here.
    /// </summary>
    public class ContentProxy:BaseProxy
    {
        private readonly HttpClient _cmsHttpClient;

        public ContentProxy(IHttpClientFactory clientFactory,
                            IConfiguration configuration,
                            ILogger<ContentProxy> logger,
                            IDistributedCache cache)
               : base(configuration,clientFactory,logger,cache)
        {
            _cmsHttpClient = clientFactory.CreateClient(ApplicationConstants.HttpClientKeys.CMS_HTTP_CLIENT);
        }

        /// <summary>
        /// Returns the content id details based on the content Id
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        public async Task<Content> GetContentById(Guid contentId)
        {
            string url = $"Content/{contentId}";

            string accessToken = await base.GetServiceAccessToken();

            Content content = null;

            try
            {
                content = await _cmsHttpClient.Get<Content>(url, accessToken);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {}

            return content;
        }

        /// <summary>
        /// Returns list of content data containing content id and content provider id for given content ids
        /// </summary>
        /// <param name="contentIds"></param>
        /// <returns></returns>
        public async Task<List<ContentInfo>> GetContentProviderIds(List<Guid> contentIds)
        {
            string url = $"Content/contentIds";

            List<ContentInfo> contents = new List<ContentInfo>();

            string accessToken = await base.GetServiceAccessToken();

            try
            {
                contents = await _cmsHttpClient.Post<List<Guid>, List<ContentInfo>>(url, contentIds, accessToken:accessToken);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            { }

            return contents;
        }
    }
}
