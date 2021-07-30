using blendnet.api.proxy.KaizalaIdentity;
using blendnet.common.dto;
using blendnet.common.dto.Identity;
using blendnet.common.dto.User;
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
    /// Kaizala Authentication handler
    /// </summary>
    public class KaizalaIdentityAuthHandler : BaseKiazalaIdentityAuthHandler
    {
        IUserDetails _userDetails;

        ILogger<KaizalaIdentityAuthHandler> _authLogger;

        public KaizalaIdentityAuthHandler(  IOptionsMonitor<KaizalaIdentityAuthOptions> options, 
                                            ILoggerFactory logger, 
                                            UrlEncoder encoder, 
                                            ISystemClock clock,
                                            KaizalaIdentityProxy kaizalaIdentityProxy,
                                            ILogger<KaizalaIdentityAuthHandler> authLogger,
                                            IUserDetails userDetails) : base(options, logger, encoder, clock, kaizalaIdentityProxy, authLogger)
        {
            _userDetails = userDetails;

            _authLogger = authLogger;
        }

        /// <summary>
        /// Add the additional check if the user exists in blendnet user collection or not.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <param name="headerValue"></param>
        /// <returns></returns>
        public async override Task<AdditionalValidationResponse> PerformAdditionalValidation(HttpRequest request, 
                                                                ValidatePartnerAccessTokenResponse response,
                                                                AuthenticationHeaderValue headerValue)
        {
            AdditionalValidationResponse additionalValidationResponse = new AdditionalValidationResponse() { ValidationPassed = false };

            string phoneNumber = response.KiazalaCredentials.PhoneNumber.Replace(ApplicationConstants.CountryCodes.India, "");

            User user = await _userDetails.GetUserDetails(phoneNumber);

            if (user is null)
            {
                _authLogger.LogInformation($"Failed to get user details from user collection for {headerValue.Parameter}");
                
                return additionalValidationResponse;
            }

            _authLogger.LogInformation($"Performing additional validation for : KID : {response.UID} UID : {user.UserId} ");

            //Add the blendnet user id
            Claim claim = new Claim(ApplicationConstants.BlendNetClaims.UId, user.UserId.ToString());

            additionalValidationResponse.ValidationPassed = true;

            additionalValidationResponse.Claim = claim;

            return additionalValidationResponse;

        }
       
    }
}
