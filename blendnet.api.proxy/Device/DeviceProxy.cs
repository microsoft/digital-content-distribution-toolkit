// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.api.proxy.Common;
using DTO=blendnet.common.dto;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using blendnet.common.dto.Device;

namespace blendnet.api.proxy.Device
{
    public class DeviceProxy : BaseProxy
    {
        private readonly HttpClient _deviceHttpClient;
        
        public DeviceProxy(IHttpClientFactory clientFactory,
                                IConfiguration configuration,
                                ILogger<DeviceProxy> logger,
                                IDistributedCache cache)
                : base(configuration, clientFactory, logger, cache)
        {
            _deviceHttpClient = clientFactory.CreateClient(DTO.ApplicationConstants.HttpClientKeys.DEVICE_HTTP_CLIENT);
        }

        public async Task<DTO.Device.Device> GetDevice(string deviceId)
        {
            string url = $"Device/{deviceId}";
            string accessToken = await base.GetServiceAccessToken();

            DTO.Device.Device device = null;
            try
            {
                device = await _deviceHttpClient.Get<DTO.Device.Device>(url, accessToken);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            { }

            return device;
        }

        public async Task<List<DeviceContentByContentProviderIdResponse>> GetContentByDeviceId(string deviceId, List<Guid> contentProviderIds)
        {
            string url = $"DeviceContent/{deviceId}/contents";
            string accessToken = await base.GetServiceAccessToken();

            List<DeviceContentByContentProviderIdResponse> response = new List<DeviceContentByContentProviderIdResponse>();

            var requestBody = new DeviceContentByContentProviderIdRequest()
            {
                ContentProviderIds = contentProviderIds,
            };

            try
            {
                response = await _deviceHttpClient.Post<DeviceContentByContentProviderIdRequest, List<DeviceContentByContentProviderIdResponse>>(url, requestBody, accessToken: accessToken);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            { }

            return response;

        }
    }
}
