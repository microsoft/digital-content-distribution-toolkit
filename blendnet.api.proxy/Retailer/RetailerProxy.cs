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
            RetailerDto retailer =  new RetailerDto(); //stub
            retailer.Id = retailerId;
            retailer.FirstName = "Unknown";
            retailer.IsActive = true;
            retailer.Mobile = "9738353779";
            return retailer;
        }

        public async Task<RetailerDto> GetRetailerByPhoneNumber(string retailerPhoneNumber)
        {
            RetailerDto retailer = new RetailerDto(); //stub
            retailer.Id = new Guid("9e6469bc-8881-416a-b031-1c07a856f038"); //dummy id
            retailer.FirstName = "Unknown";
            retailer.IsActive = true;
            retailer.Mobile = retailerPhoneNumber;
            return retailer;
        }
    }
}
