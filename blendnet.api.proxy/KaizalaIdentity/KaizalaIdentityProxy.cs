using AutoMapper.Configuration;
using blendnet.common.dto;
using blendnet.common.dto.Identity;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using blendnet.api.proxy.Common;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text.Json;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace blendnet.api.proxy.KaizalaIdentity
{
    /// <summary>
    /// Invokes the Kaizala Identity Validate Token
    /// </summary>
    public class KaizalaIdentityProxy
    {
        private readonly HttpClient _kaizalaIdentityHttpClient;

        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        private const string URN_MICROSOFT_CREDENTIALS = "urn:microsoft:credentials";

        private const string UID = "uid";

        ILogger<KaizalaIdentityProxy> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientFactory"></param>
        /// <param name="configuration"></param>
        public KaizalaIdentityProxy(IHttpClientFactory clientFactory,
                                    Microsoft.Extensions.Configuration.IConfiguration configuration,
                                    ILogger<KaizalaIdentityProxy> logger)
        {
            _kaizalaIdentityHttpClient = clientFactory.CreateClient(ApplicationConstants.HttpClientKeys.KAIZALAIDENTITY_HTTP_CLIENT);

            _configuration = configuration;

            _logger = logger;

        }


        /// <summary>
        /// Validate Partner Access Token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<ValidatePartnerAccessTokenResponse> ValidatePartnerAccessToken(string accessToken)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();

            _configuration.GetSection("KaizalaIdentity").Bind(settings);

            string kaizalaApplicationName = _configuration.GetValue<string>("KaizalaIdentityAppName");

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            JwtSecurityToken securityToken = handler.ReadJwtToken(accessToken);

            Claim claim = securityToken.Claims.Where(c => c.Type == URN_MICROSOFT_CREDENTIALS).FirstOrDefault();

            Claim userIdClaim = securityToken.Claims.Where(c => c.Type == UID).FirstOrDefault();

            if (claim == null)
            {
                _logger.LogInformation($"{URN_MICROSOFT_CREDENTIALS} not found in the token");
            }

            KiazalaCredentials credentials = JsonSerializer.Deserialize<KiazalaCredentials>(claim.Value,Utilties.GetJsonSerializerOptions());

            if (credentials == null)
            {
                _logger.LogInformation($"Not able to serialize {claim.Value} to KiazalaCredentials");
            }

            char lastDigit = credentials.PhoneNumber[credentials.PhoneNumber.Length - 1];

            string httpBaseHref = settings[lastDigit.ToString()];

            string url = $"{httpBaseHref}v1/ValidatePartnerAccessToken?applicationName={kaizalaApplicationName}";

            _kaizalaIdentityHttpClient.DefaultRequestHeaders.Remove("accessToken");

            _kaizalaIdentityHttpClient.DefaultRequestHeaders.Add("accessToken", accessToken);

            ValidatePartnerAccessTokenResponse successResponse = null;

            bool validationPassed = false;

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                successResponse = await _kaizalaIdentityHttpClient.Get<ValidatePartnerAccessTokenResponse>(url);

                successResponse.KiazalaCredentials = credentials;

                successResponse.UID = userIdClaim.Value.Split(":")[1];

                validationPassed = true;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogInformation($"401 from ValidatePartnerAccessToken - {accessToken}");

                validationPassed = false;
            }

            stopwatch.Stop();

            _logger.LogInformation($" Time taken to validate ({validationPassed}) {accessToken} is {stopwatch.ElapsedMilliseconds} (ms)");

            return successResponse;
        }

    }
}
