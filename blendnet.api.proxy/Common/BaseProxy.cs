using blendnet.common.dto;
using blendnet.common.dto.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private readonly IDistributedCache _cache;

        /// <summary>
        /// Base Proxy Class
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="clientFactory"></param>
        /// <param name="logger"></param>
        public BaseProxy(IConfiguration configuration, 
                         IHttpClientFactory clientFactory,
                         ILogger logger,
                         IDistributedCache cache)
        {
            _configuration = configuration;

            _kaizalaIdentityHttpClient = clientFactory.CreateClient(ApplicationConstants.HttpClientKeys.KAIZALA_HTTP_CLIENT);

            _logger = logger;

            _cache = cache;
        }
       

        /// <summary>
        /// Get Access Token for service Account
        /// To Do : Add Token Caching support as the current implementation will kill the peformance
        /// </summary>
        /// <returns></returns>
        protected async Task<string> GetServiceAccessToken()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            bool tokenFromCache = false;

            string serviceId = _configuration["CloudApiServiceAccountId"];

            string accessTokenCacheKey = $"{serviceId}{ApplicationConstants.DistributedCacheKeySuffix.SERVICEACCOUNTKEY}";

            string accessTokenFromCache = await _cache.GetStringAsync(accessTokenCacheKey);

            string accessTokenToReturn = string.Empty;

            if (!string.IsNullOrEmpty(accessTokenFromCache))
            {
                tokenFromCache = true;

                accessTokenToReturn =  accessTokenFromCache;
            }
            else
            {
                Uri keyVaultUrl = new Uri($"https://{_configuration["KeyVaultName"]}.vault.azure.net/");

                string kaizalaIdentityBaseUrl = string.Format(_configuration["KaizalaIdentityBaseUrl"], ""); // base url do not require SU information

                string certificateName = _configuration["CloudApiServiceAccountCertName"];

                string kaizalaIdentityAppName = _configuration["KaizalaIdentityAppName"];

                int tokenExpiry = _configuration.GetValue<int>("CloudApiAccessTokenExpiryInHrs");

                string requestData = $"?serviceId={serviceId}&appName={kaizalaIdentityAppName}&tls={tokenExpiry}";

                string signedData = await CertificateHelper.GetSignedDataFromCertificate(keyVaultUrl, certificateName, requestData);

                string url = $"{kaizalaIdentityBaseUrl}v1/GetAccessTokenForPartnerService{requestData}";

                _kaizalaIdentityHttpClient.DefaultRequestHeaders.Remove("Authorization");

                _kaizalaIdentityHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", signedData);

                GetAccessTokenForPartnerServiceResponse successResponse = null;

                try
                {
                    successResponse = await _kaizalaIdentityHttpClient.Get<GetAccessTokenForPartnerServiceResponse>(url, string.Empty);

                    DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();

                    //let the token expire 15 minutes in advance 
                    options.SetAbsoluteExpiration(TimeSpan.FromHours(tokenExpiry - 0.25));

                    //Set the token to cache
                    await _cache.SetStringAsync(accessTokenCacheKey, successResponse.AccessToken,options);
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogInformation($"401 from GetAccessTokenForPartnerService - Request Data {requestData} - Signed Data - {signedData}");
                }

                accessTokenToReturn = successResponse.AccessToken;
            }

            stopwatch.Stop();

            _logger.LogInformation($" Time taken to get service access token ({tokenFromCache}) for {serviceId} is {stopwatch.ElapsedMilliseconds} (ms)");

            return accessTokenToReturn;

        }

        /// <summary>
        /// Returns the Kaizala Base Url by Phone Number Last Digit
        /// Mainly for lower environments.
        /// For production set the same URL fro all the digits
        /// </summary>
        /// <param name="lastDigit"></param>
        /// <returns></returns>
        public string GetBaseUrlByPhoneDigit(string lastDigit)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();

            string baseUrl = _configuration["KaizalaIdentityBaseUrl"];
            _configuration.GetSection("KaizalaIdentity").Bind(settings);

            string scaleUnit = GetScaleUnit(settings[lastDigit.ToString()]);

            string baseUrlWithSu = string.Format(baseUrl, scaleUnit);

            return baseUrlWithSu;
        }

        public int GetUserScaleUnitByPhoneDigit(string lastDigit)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();

            _configuration.GetSection("KaizalaIdentity").Bind(settings);

            string scaleUnit = settings[lastDigit.ToString()];

            return int.Parse(scaleUnit);
        }

        public List<int> GetScaleUnitsForCurrentEnv()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();

            _configuration.GetSection("KaizalaIdentity").Bind(settings);

            HashSet<int> scaleUnits = new HashSet<int>(settings.Values.ToList().Select(x => int.Parse(x)).ToList());

            return new List<int>(scaleUnits);
        }

        private string GetScaleUnit(string scaleUnit)
        {
            return "0".Equals(scaleUnit) ? "" : scaleUnit;
        }
    } 
}
