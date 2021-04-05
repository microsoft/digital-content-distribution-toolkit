using blendnet.common.dto.cms;
using blendnet.common.infrastructure.Ams;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace blendnet.cms.api.Common
{
    /// <summary>
    /// Ams Helper Class to Generate the Token
    /// </summary>
    public class AmsHelper
    {
        private readonly ILogger _logger;

        AppSettings _appSettings;

        public AmsHelper(ILogger<AmsHelper> logger,
                        IOptionsMonitor<AppSettings> optionsMonitor)
        {
            _logger = logger;

            _appSettings = optionsMonitor.CurrentValue;
        }

        /// <summary>
        /// Generates the token for the Content and Command Id
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="commandId"></param>
        /// <returns></returns>
        public async Task<string> GetContentToken(Guid contentId, Guid commandId)
        {
            IAzureMediaServicesClient amsclient = await GetAmsClient();

            string token = string.Empty;

            string locatorName = $"{contentId}|{commandId}";

            byte[] tokenSigningKey = Convert.FromBase64String(_appSettings.AmsTokenSigningKey);

            // Get Streaming locator
            StreamingLocator locator = await amsclient.StreamingLocators.GetAsync(_appSettings.AmsResourceGroupName, 
                                                                                  _appSettings.AmsAccountName,
                                                                                  locatorName);

            if (locator != null)
            {
                // In this example, we want to play the Widevine (CENC) encrypted stream. 
                // We need to get the key identifier of the content key where its type is CommonEncryptionCenc.
                string keyIdentifier = locator.ContentKeys.Where(k => k.Type == StreamingLocatorContentKeyType.CommonEncryptionCenc).First().Id.ToString();

                // In order to generate our test token we must get the ContentKeyId to put in the ContentKeyIdentifierClaim claim.
                token = GenerateTokenAsync(_appSettings.AmsTokenIssuer, 
                                            _appSettings.AmsTokenAudience,
                                            keyIdentifier,
                                            _appSettings.AmsTokenExpiryInMts,
                                            tokenSigningKey);
            }
            else
            {
                _logger.LogInformation($"No streaming locator found for {locatorName}");
            }

            return token;
        }



        /// <summary>
        /// Create a token that will be used to protect your stream.
        /// Only authorized clients would be able to play the video.  
        /// </summary>
        /// <param name="issuer">The issuer is the secure token service that issues the token. </param>
        /// <param name="audience">The audience, sometimes called scope, describes the intent of the token or the resource the token authorizes access to. </param>
        /// <param name="keyIdentifier">The content key ID.</param>
        /// <param name="tokenVerificationKey">Contains the key that the token was signed with. </param>
        /// <returns></returns>
        private string GenerateTokenAsync(string issuer, 
                                            string audience, 
                                            string keyIdentifier, 
                                            int tokenExpiryInMts,
                                            byte[] tokenVerificationKey)
        {
            var tokenSigningKey = new SymmetricSecurityKey(tokenVerificationKey);

            SigningCredentials cred = new SigningCredentials(
                tokenSigningKey,
                // Use the  HmacSha256 and not the HmacSha256Signature option, or the token will not work!
                SecurityAlgorithms.HmacSha256,
                SecurityAlgorithms.Sha256Digest);

            Claim[] claims = new Claim[]
            {
                new Claim(ContentKeyPolicyTokenClaim.ContentKeyIdentifierClaim.ClaimType, keyIdentifier)
            };

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.Now.AddMinutes(-5),
                expires: DateTime.Now.AddMinutes(tokenExpiryInMts),
                signingCredentials: cred);

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(token);
        }

        /// <summary>
        /// Gets the AMS Client
        /// </summary>
        /// <returns></returns>
        private async Task<IAzureMediaServicesClient> GetAmsClient()
        {
            IAzureMediaServicesClient amsclient = await AmsUtilities.CreateMediaServicesClientAsync(_appSettings.AmsArmEndPoint,
                                                                                                     _appSettings.AmsClientId,
                                                                                                     _appSettings.AmsClientSecret,
                                                                                                     _appSettings.AmsTenantId,
                                                                                                    _appSettings.AmsSubscriptionId);

            return amsclient;

        }

    }
}
