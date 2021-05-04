using blendnet.common.dto;
using blendnet.common.dto.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.api.proxy.Common
{
    /// <summary>
    /// Base Proxy.
    /// </summary>
    public class BaseProxy
    {
        private IConfiguration _configuration;
        
        private readonly HttpClient _kaizalaIdentityHttpClient;

        private ILogger _logger;

        /// <summary>
        /// Base Proxy Class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="clientFactory"></param>
        /// <param name="logger"></param>
        public BaseProxy(IConfiguration configuration, 
                         IHttpClientFactory clientFactory,
                         ILogger logger)
        {
            _configuration = configuration;

            _kaizalaIdentityHttpClient = clientFactory.CreateClient(ApplicationConstants.HttpClientKeys.KAIZALAIDENTITY_HTTP_CLIENT);

            _logger = logger;
        }
       

        /// <summary>
        /// Get Access Token for service Account
        /// To Do : Add Token Caching support as the current implementation will kill the peformance
        /// </summary>
        /// <returns></returns>
        protected async Task<string> GetServiceAccessToken()
        {
            Uri keyVaultUrl = new Uri($"https://{_configuration["KeyVaultName"]}.vault.azure.net/");

            string kaizalaIdentityBaseUrl = _configuration["KaizalaIdentityBaseUrl"];

            string certificateName = _configuration["CloudApiServiceAccountCertName"];

            string serviceId = _configuration["CloudApiServiceAccountId"];

            string kaizalaIdentityAppName = _configuration["KaizalaIdentityAppName"];

            int tokenExpiry = _configuration.GetValue<int>("CloudApiAccessTokenExpiryInHrs");

            string requestData = $"?serviceId={serviceId}&serviceName={kaizalaIdentityAppName}&tls={tokenExpiry}";

            string signedData = await CertificateHelper.GetSignedDataFromCertificate(keyVaultUrl, certificateName, requestData);

            string url = $"{kaizalaIdentityBaseUrl}v1/GetAccessTokenForPartnerService?serviceId={serviceId}&appName={kaizalaIdentityAppName}";

            _kaizalaIdentityHttpClient.DefaultRequestHeaders.Remove("Authorization");

            _kaizalaIdentityHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", signedData);

            GetAccessTokenForPartnerServiceResponse successResponse = null;

            try
            {
                successResponse = await _kaizalaIdentityHttpClient.Get<GetAccessTokenForPartnerServiceResponse>(url,string.Empty);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogInformation($"401 from GetAccessTokenForPartnerService - Request Data {requestData} - Signed Data - {signedData}");
            }

            return successResponse.AccessToken;
        }

       
    }
}
