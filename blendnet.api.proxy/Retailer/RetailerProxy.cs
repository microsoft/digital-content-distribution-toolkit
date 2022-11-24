// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.api.proxy.Common;
using blendnet.common.dto;
using blendnet.common.dto.Retailer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace blendnet.api.proxy.Retailer
{
    public class RetailerProxy : BaseProxy
    {
        private readonly HttpClient _rmsHttpClient;

        public RetailerProxy(IHttpClientFactory clientFactory,
                                IConfiguration configuration,
                                ILogger<RetailerProxy> logger,
                                IDistributedCache cache)
                : base(configuration, clientFactory, logger, cache)
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

            string url = $"Retailer/byPartnerId/{retailerPartnerId}";
            string accessToken = await base.GetServiceAccessToken();

            RetailerDto retailer = null;
            try
            {
                retailer = await _rmsHttpClient.Get<RetailerDto>(url, accessToken);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            { }

            return retailer;
        }

        /// <summary>
        /// Get retailer by referral Code
        /// </summary>
        /// <param name="referralCode"></param>
        /// <returns></returns>
        public async Task<RetailerDto> GetRetailerByReferralCode(string referralCode)
        {
            string url = $"Retailer/byReferralCode/{referralCode}";
            string accessToken = await base.GetServiceAccessToken();

            RetailerDto retailer = null;
            try
            {
                retailer = await _rmsHttpClient.Get<RetailerDto>(url, accessToken);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            { }

            return retailer;
        }

        /// <summary>
        /// Gets the retailer to whom the given device is currently assigned
        /// </summary>
        /// <param name="deviceId">Device ID</param>
        /// <returns></returns>
        public async Task<RetailerDto> GetCurrentRetailerByDeviceId(string deviceId)
        {
            string url = $"Retailer/byDeviceId/{deviceId}";
            string accessToken = await base.GetServiceAccessToken();

            List<RetailerDto> retailers = null;
            RetailerDto retailer = null;
            try
            {
                retailers = await _rmsHttpClient.Get<List<RetailerDto>>(url, accessToken);

                // this gets all retailer where device was ever assigned, so need to filter for active assignment
                retailer = retailers
                            .Where(r => r.DeviceAssignments.Where(asg => asg.DeviceId == deviceId && asg.IsActive).Any())
                            .FirstOrDefault();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            { }

            return retailer;
        }
    }
}
