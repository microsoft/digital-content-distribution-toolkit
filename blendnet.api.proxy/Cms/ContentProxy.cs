using blendnet.api.proxy.Common;
using blendnet.common.dto;
using blendnet.common.dto.Cms;
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
    public class ContentProxy
    {
        private readonly HttpClient _cmsHttpClient;

        public ContentProxy(IHttpClientFactory clientFactory)
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
            
            var successResponse = await _cmsHttpClient.Get<Content>(url);

            return successResponse;
        }
    }
}
