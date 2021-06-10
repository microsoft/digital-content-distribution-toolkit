using blendnet.common.dto;
using blendnet.common.dto.Identity;
using System;
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
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.Extensions.Caching.Distributed;

namespace blendnet.api.proxy.KaizalaIdentity
{
    /// <summary>
    /// Invokes the Kaizala Identity Validate Token
    /// </summary>
    public class KaizalaIdentityProxy: BaseProxy
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
                                    IConfiguration configuration,
                                    ILogger<KaizalaIdentityProxy> logger,
                                    IDistributedCache cache)
        : base(configuration, clientFactory, logger, cache)
        {
            _kaizalaIdentityHttpClient = clientFactory.CreateClient(ApplicationConstants.HttpClientKeys.KAIZALA_HTTP_CLIENT);

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
            string kaizalaApplicationName = _configuration.GetValue<string>("KaizalaIdentityAppName");

            Claim userIdClaim;

            KiazalaCredentials credentials = GetKiazalaCredentials(accessToken, out userIdClaim);

            string urlToInvoke = $"v1/ValidatePartnerAccessToken?applicationName={kaizalaApplicationName}";

            string url = PrepareHttpClient(credentials, accessToken, urlToInvoke);
            
            ValidatePartnerAccessTokenResponse successResponse = null;

            bool validationPassed = false;

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                successResponse = await _kaizalaIdentityHttpClient.Get<ValidatePartnerAccessTokenResponse>(url);

                successResponse.KiazalaCredentials = credentials;

                successResponse.UID = userIdClaim.Value.Split(":")[1];
                
                //remove scale unit from kaizala
                successResponse.UID = successResponse.UID.Split('@')[0];

                validationPassed = true;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogInformation($"401 from ValidatePartnerAccessToken - {accessToken}.  Exception {ex} ");

                validationPassed = false;
            }

            stopwatch.Stop();

            _logger.LogInformation($" Time taken to validate ({validationPassed}) {accessToken} is {stopwatch.ElapsedMilliseconds} (ms)");

            return successResponse;
        }

        /// <summary>
        /// Adds the user role assignment
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task AddPartnerUsersRole (AddPartnerUsersRoleRequest request)
        {
            string kaizalaApplicationName = _configuration.GetValue<string>("KaizalaIdentityAppName");

            request.ApplicationName = kaizalaApplicationName;

            Claim userIdClaim;

            string accessToken = await base.GetServiceAccessToken();

            KiazalaCredentials credentials = GetKiazalaCredentials(accessToken, out userIdClaim);

            string urlToInvoke = $"v1/AddPartnerUsersRole";

            string url = PrepareHttpClient(credentials, accessToken, urlToInvoke);

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {

                JsonSerializerOptions jsonSerializerOptions =  Utilties.GetJsonSerializerOptions();

                jsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

                await _kaizalaIdentityHttpClient.Post<AddPartnerUsersRoleRequest,object>(url,request,false,jsonSerializerOptions);

            }
            catch (Exception ex) 
            {
                _logger.LogInformation($"Bad Request from AddPartnerUsersRole for {accessToken}. Exception {ex}");

                throw;
            }

            stopwatch.Stop();

            _logger.LogInformation($" Time taken to AddPartnerUsersRole is {stopwatch.ElapsedMilliseconds} (ms)");

        }


        /// <summary>
        /// Deletes the user role assignment
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task DeletePartnerUsersRole(DeletePartnerUsersRoleRequest request)
        {
            string kaizalaApplicationName = _configuration.GetValue<string>("KaizalaIdentityAppName");

            request.ApplicationName = kaizalaApplicationName;

            Claim userIdClaim;

            string accessToken = await base.GetServiceAccessToken();

            KiazalaCredentials credentials = GetKiazalaCredentials(accessToken, out userIdClaim);

            string urlToInvoke = $"v1/DeletePartnerUsersRole";

            string url = PrepareHttpClient(credentials, accessToken, urlToInvoke);

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {

                JsonSerializerOptions jsonSerializerOptions = Utilties.GetJsonSerializerOptions();

                jsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

                await _kaizalaIdentityHttpClient.Post<DeletePartnerUsersRoleRequest, object>(url, request, false, jsonSerializerOptions);

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Bad Request from DeletePartnerUsersRole for {accessToken}. Exception {ex}");

                throw;
            }

            stopwatch.Stop();

            _logger.LogInformation($" Time taken to DeletePartnerUsersRole is {stopwatch.ElapsedMilliseconds} (ms)");

        }

        /// <summary>
        /// Sets The base url
        /// Adds the authorization header
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="kaizalaApplicationName"></param>
        /// <param name="accessToken"></param>
        private string PrepareHttpClient( KiazalaCredentials credentials,
                                        string accessToken,
                                        string urlToInvoke)
        {
            char lastDigit = credentials.PhoneNumber[credentials.PhoneNumber.Length - 1];

            string httpBaseHref = GetBaseUrlByPhoneDigit(lastDigit.ToString());

            string url = $"{httpBaseHref}{urlToInvoke}";

            _kaizalaIdentityHttpClient.DefaultRequestHeaders.Remove("accessToken");

            _kaizalaIdentityHttpClient.DefaultRequestHeaders.Add("accessToken", accessToken);

            return url;
        }


        /// <summary>
        /// Get Kaizala Credentials
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="userIdClaim"></param>
        /// <returns></returns>
        private KiazalaCredentials GetKiazalaCredentials(string accessToken, out Claim userIdClaim)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            JwtSecurityToken securityToken = handler.ReadJwtToken(accessToken);

            Claim claim = securityToken.Claims.Where(c => c.Type == URN_MICROSOFT_CREDENTIALS).FirstOrDefault();

            userIdClaim = securityToken.Claims.Where(c => c.Type == UID).FirstOrDefault();

            if (claim == null)
            {
                _logger.LogInformation($"{URN_MICROSOFT_CREDENTIALS} not found in the token");
            }

            KiazalaCredentials credentials = JsonSerializer.Deserialize<KiazalaCredentials>(claim.Value, Utilties.GetJsonSerializerOptions());

            if (credentials == null)
            {
                _logger.LogInformation($"Not able to serialize {claim.Value} to KiazalaCredentials");
            }

            return credentials;
        }
    }
}
