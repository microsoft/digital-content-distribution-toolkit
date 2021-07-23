using blendnet.api.proxy.KaizalaIdentity;
using blendnet.common.dto;
using blendnet.common.dto.Identity;
using blendnet.common.dto.User;
using Microsoft.AspNetCore.Authentication;
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
    /// Kaizala Authentication handler
    /// </summary>
    public class KaizalaIdentityAuthHandler : AuthenticationHandler<KaizalaIdentityAuthOptions>
    {
        KaizalaIdentityProxy _kaizalaIdentityProxy;

        KaizalaIdentityAuthOptions _kaizalaIdentityAuthOptions;

        IUserDetails _userDetails;

        ILogger<KaizalaIdentityAuthHandler> _authLogger;

        public KaizalaIdentityAuthHandler(  IOptionsMonitor<KaizalaIdentityAuthOptions> options, 
                                            ILoggerFactory logger, 
                                            UrlEncoder encoder, 
                                            ISystemClock clock,
                                            KaizalaIdentityProxy kaizalaIdentityProxy,
                                            ILogger<KaizalaIdentityAuthHandler> authLogger,
                                            IUserDetails userDetails) : base(options, logger, encoder, clock)
        {
            _kaizalaIdentityProxy = kaizalaIdentityProxy;

            _kaizalaIdentityAuthOptions = options.CurrentValue;

            _userDetails = userDetails;

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
                _authLogger.LogInformation($"Kaizala auth returned null for : {headerValue.Parameter}");

                return AuthenticateResult.Fail("Unauthorized");
            }

            string phoneNumber = response.KiazalaCredentials.PhoneNumber.Replace(ApplicationConstants.CountryCodes.India, "");

            User user = await _userDetails.GetUserDetails(phoneNumber);

            if (user is null)
            {
                _authLogger.LogInformation($"Failed to get user details from user collection for {headerValue.Parameter}");

                return AuthenticateResult.Fail("Unauthorized");
            }


            var identities = new List<ClaimsIdentity> { new ClaimsIdentity(GetClaims(response,user, headerValue.Parameter), _kaizalaIdentityAuthOptions.Scheme) };

            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identities), _kaizalaIdentityAuthOptions.Scheme);

            return AuthenticateResult.Success(ticket);

        }

        /// <summary>
        /// Get Claims based on returned response
        /// </summary>
        /// <param name="response"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        private List<Claim> GetClaims(ValidatePartnerAccessTokenResponse response, User user , string accessToken)
        {
            List<Claim> claims = new List<Claim>();

            string[] roles = response.UserRole.Split(',');

            foreach(string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //remove country code from phone number
            string userIdStr = response.KiazalaCredentials.PhoneNumber.Replace(ApplicationConstants.CountryCodes.India, "");

            Claim claim = new Claim(ClaimTypes.Name, userIdStr);

            claims.Add(claim);

            claim = new Claim(ApplicationConstants.KaizalaIdentityClaims.IdentityUId, response.UID);

            claims.Add(claim);

            //Add the blendnet user id
            claim = new Claim(ApplicationConstants.BlendNetClaims.UId, user.Id.ToString());

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

        public string Scheme => DefaultScheme;
    }
}
