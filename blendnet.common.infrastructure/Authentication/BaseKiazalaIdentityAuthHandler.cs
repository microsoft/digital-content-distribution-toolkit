using blendnet.api.proxy.KaizalaIdentity;
using blendnet.common.dto;
using blendnet.common.dto.Extensions;
using blendnet.common.dto.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;


namespace blendnet.common.infrastructure.Authentication
{
    /// <summary>
    /// Base Kiazala authentication handler.
    /// It does the check of user existence in kiazala identity.
    /// </summary>
    public class BaseKiazalaIdentityAuthHandler: AuthenticationHandler<KaizalaIdentityAuthOptions>
    {
        KaizalaIdentityProxy _kaizalaIdentityProxy;

        ILogger _authLogger;

        public BaseKiazalaIdentityAuthHandler(  IOptionsMonitor<KaizalaIdentityAuthOptions> options,
                                                ILoggerFactory logger,
                                                UrlEncoder encoder,
                                                ISystemClock clock,
                                                KaizalaIdentityProxy kaizalaIdentityProxy,
                                                ILogger authLogger) : base(options, logger, encoder, clock)
        {
            _kaizalaIdentityProxy = kaizalaIdentityProxy;

            _authLogger = authLogger;
        }

        /// <summary>
        /// Handle Authenticate Async
        /// </summary>
        /// <returns></returns>
        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                //Authorization header not in request
                return AuthenticateResult.NoResult();
            }

            if (!AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out AuthenticationHeaderValue headerValue))
            {
                //Invalid Authorization header
                return AuthenticateResult.NoResult();
            }

            if (!"bearer".Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                //Not Basic authentication header
                return AuthenticateResult.NoResult();
            }

            if (string.IsNullOrEmpty(headerValue.Parameter))
            {
                return AuthenticateResult.NoResult();
            }

            ValidatePartnerAccessTokenResponse response = await _kaizalaIdentityProxy.ValidatePartnerAccessToken(headerValue.Parameter);

            //If the response is null
            if (response is null)
            {
                _authLogger.LogInformation($"Kaizala auth returned null for : {headerValue.Parameter.Mask()}");

                return AuthenticateResult.Fail("Unauthorized");
            }

            AdditionalValidationResponse additionalResponse = await PerformAdditionalValidation(Request, response, headerValue);

            if (!additionalResponse.ValidationPassed)
            {
                _authLogger.LogInformation($"Additional authentication validation failed for : {headerValue.Parameter.Mask()}");

                return AuthenticateResult.Fail("Unauthorized");
            }

            var identities = new List<ClaimsIdentity> 
                                            { 
                                                    new ClaimsIdentity(GetClaims(response, additionalResponse.Claim, headerValue.Parameter), 
                                                    base.Options.Scheme) 
                                            };

            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identities), base.Options.Scheme);

            return AuthenticateResult.Success(ticket);

        }

        /// <summary>
        /// Default implementation of perform validation always returns true
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="headerValue"></param>
        /// <returns></returns>
        public virtual Task<AdditionalValidationResponse> PerformAdditionalValidation(HttpRequest request,
                                                                ValidatePartnerAccessTokenResponse response,
                                                                AuthenticationHeaderValue headerValue)
        {
            _authLogger.LogInformation($"Performing base additional validation for : KID : {response.UID}");

            AdditionalValidationResponse additionalValidationResponse = new AdditionalValidationResponse() { ValidationPassed = true };

            return Task.FromResult(additionalValidationResponse);
        }
        
        

        /// <summary>
        /// Get Claims based on returned response
        /// </summary>
        /// <param name="response"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public List<Claim> GetClaims(ValidatePartnerAccessTokenResponse response, Claim additionalClaim, string accessToken)
        {
            List<Claim> claims = new List<Claim>();

            string[] roles = response.UserRole.Split(',');

            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            
            //add additional claim if any
            if (additionalClaim != null)
            {
                claims.Add(additionalClaim);
            }

            //remove country code from phone number
            string userIdStr = response.KiazalaCredentials.PhoneNumber.Replace(ApplicationConstants.CountryCodes.India, "");

            Claim claim = new Claim(ClaimTypes.Name, userIdStr);

            claims.Add(claim);

            claim = new Claim(ApplicationConstants.KaizalaIdentityClaims.IdentityUId, response.UID);

            claims.Add(claim);

            claim = new Claim(ApplicationConstants.KaizalaIdentityClaims.ApplicationType, response.KiazalaCredentials.ApplicationType.ToString());

            claims.Add(claim);

            claim = new Claim(ApplicationConstants.KaizalaIdentityClaims.AppName, response.KiazalaCredentials.AppName);

            claims.Add(claim);

            claim = new Claim(ApplicationConstants.KaizalaIdentityClaims.CId, response.KiazalaCredentials.CId);

            claims.Add(claim);

            claim = new Claim(ApplicationConstants.KaizalaIdentityClaims.Permissions, response.KiazalaCredentials.Permissions);

            claims.Add(claim);

            claim = new Claim(ApplicationConstants.KaizalaIdentityClaims.PhoneNumber, response.KiazalaCredentials.PhoneNumber);

            claims.Add(claim);

            claim = new Claim(ApplicationConstants.KaizalaIdentityClaims.TestSender, response.KiazalaCredentials.TestSender);

            claims.Add(claim);

            claim = new Claim(ApplicationConstants.KaizalaIdentityClaims.TokenValidFrom, response.KiazalaCredentials.TokenValidFrom.ToString());

            claims.Add(claim);

            claim = new Claim(ApplicationConstants.KaizalaIdentityClaims.AccessToken, accessToken);

            claims.Add(claim);

            return claims;
        }

        
    }

    /// <summary>
    /// KaizalaIdentityAuthOptions
    /// </summary>
    public class KaizalaIdentityAuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "KaizalaIdentityScheme";

        public string Scheme { get; set; } = "KaizalaIdentityScheme";

        public const string BasicIdentityScheme = "KaizalaBasicIdentityScheme";

    }

    /// <summary>
    /// Response of Additional Validation
    /// </summary>
    public class AdditionalValidationResponse
    {
        public bool ValidationPassed { get; set; }

        public Claim Claim { get; set; }

    }
}
