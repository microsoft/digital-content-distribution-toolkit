using blendnet.common.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.api.proxy.Retailer
{
    public class RetailerProxy
    {
        private readonly HttpClient _rmsHttpClient;

        public RetailerProxy(IHttpClientFactory clientFactory)
        {
            _rmsHttpClient = clientFactory.CreateClient(ApplicationConstants.HttpClientKeys.RETAILER_HTTP_CLIENT);
        }

        public async Task<RetailerDto> GetRetailerById(Guid retailerId)
        {
            return new RetailerDto(); //stub
        }
    }
}
