using blendnet.common.dto;
using blendnet.common.dto.Retailer;
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

        /// <summary>
        /// Get retailer by Partner-provided retailer ID and Partner Code
        /// </summary>
        /// <param name="partnerProvidedRetailerId"></param>
        /// <param name="partnerCode"></param>
        /// <returns></returns>
        public async Task<RetailerDto> GetRetailerById(string partnerProvidedRetailerId, string partnerCode)
        {
            string retailerPartnerId = RetailerDto.CreatePartnerId(partnerCode, partnerProvidedRetailerId);
            // STUB
            RetailerDto retailer = new RetailerDto()
            {
                PartnerCode = partnerCode,
                PartnerProvidedId = partnerProvidedRetailerId,
                PartnerId = partnerProvidedRetailerId,
                UserName = "STUB",
            };

            return retailer;
        }
    }
}
