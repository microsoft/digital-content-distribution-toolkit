// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.api.proxy.Common;
using blendnet.common.dto;
using blendnet.common.dto.Retailer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace blendnet.api.proxy.Retailer
{
    public class RetailerProviderProxy : BaseProxy
    {
        private readonly HttpClient _rmsHttpClient;

        public RetailerProviderProxy(IHttpClientFactory clientFactory,
                                        IConfiguration configuration,
                                        ILogger<RetailerProviderProxy> logger,
                                        IDistributedCache cache)
                : base(configuration, clientFactory, logger, cache)
        {
            _rmsHttpClient = clientFactory.CreateClient(ApplicationConstants.HttpClientKeys.RETAILER_HTTP_CLIENT);
        }

        /// <summary>
        /// Get Retailer provider by User ID
        /// </summary>
        /// <param name="userId">Service Account ID</param>
        /// <returns></returns>
        public async Task<RetailerProviderDto> GetRetailerProviderByUserId(Guid userId)
        {
            string url = $"RetailerProvider/byUserId/{userId}";
            string accessToken = await base.GetServiceAccessToken();

            RetailerProviderDto retailerProvider = null;

            try
            {
                retailerProvider = await _rmsHttpClient.Get<RetailerProviderDto>(url, accessToken);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            { }
            
            return retailerProvider;
        }

        public async Task<RetailerProviderDto> GetRetailerProviderByPartnerCode(string partnerCode)
        {
            string url = $"RetailerProvider/byPartnerCode/{partnerCode}";
            string accessToken = await base.GetServiceAccessToken();

            RetailerProviderDto retailerProvider = null;

            try
            {
                retailerProvider = await _rmsHttpClient.Get<RetailerProviderDto>(url, accessToken);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            { }

            return retailerProvider;
        }
    }
}
