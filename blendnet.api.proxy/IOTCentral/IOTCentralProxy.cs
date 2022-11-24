// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.api.proxy.Common;
using blendnet.common.dto;
using blendnet.common.dto.Device;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace blendnet.api.proxy.IOTCentral
{
    /// <summary>
    /// Proxy to Execute IOT Central Command
    /// </summary>
    public class IOTCentralProxy
    {
        private IConfiguration _configuration;

        private readonly HttpClient _iotCentralHttpClient;

        private ILogger _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public IOTCentralProxy(IHttpClientFactory clientFactory,
                         IConfiguration configuration,
                         ILogger<IOTCentralProxy> logger)
        {
            _iotCentralHttpClient = clientFactory.CreateClient(ApplicationConstants.HttpClientKeys.IOTCENTRAL_HTTP_CLIENT);

            _configuration = configuration;

            _logger = logger;
        }

        /// <summary>
        /// Execute Command
        /// 404 is returned if the proxy module is down or device is down. 
        /// 201 is returned in case of command executed on device.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IOTModuleCommandResponse<I,O>> RunModuleCommand<I,O>(RunModuleCommandRequest<I> request)
        {
            _iotCentralHttpClient.DefaultRequestHeaders.Remove("Authorization");

            _iotCentralHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", request.APIToken);

            string url = $"api/devices/{request.DeviceName}/modules/{request.ModuleName}/commands/{request.CommandName}?api-version={request.APIVersion}";

            IOTModuleCommandResponse<I, O> response = 
                                                await _iotCentralHttpClient.Post<IOTModuleCommandRequest<I>, IOTModuleCommandResponse<I,O>>
                                                (url, request.IOTModuleCommandRequest, true);

            return response;
        }
    }
}
