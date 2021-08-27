using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace blendnet.common.infrastructure.Ams
{
    public static class AmsUtilities
    {
        /// <summary>
        /// Creates the AzureMediaServicesClient object based on the credentials
        /// supplied in local configuration file.
        /// </summary>
        /// <param name="config">The param is of type ConfigWrapper. This class reads values from local configuration file.</param>
        /// <returns></returns>
        public static async Task<IAzureMediaServicesClient> CreateMediaServicesClientAsync( string amsArmEndPoint, 
                                                                                            string amsClientId,
                                                                                            string amsClientSecret,
                                                                                            string amsTenantId,
                                                                                            string amsSubscriptionId)
        {
            var credentials = await GetCredentialsAsync(amsClientId,amsClientSecret, amsTenantId);

            return new AzureMediaServicesClient(new Uri(amsArmEndPoint), credentials)
            {
                SubscriptionId = amsSubscriptionId
            };
        }

        /// <summary>
        /// Create the ServiceClientCredentials object based on the credentials supplied in local configuration file.
        /// </summary>
        private static async Task<ServiceClientCredentials> GetCredentialsAsync(string amsClientId, 
                                                                                string amsClientSecret,
                                                                                string amsTenantId)
        {
            ClientCredential clientCredential = new ClientCredential(amsClientId, amsClientSecret);

            return await ApplicationTokenProvider.LoginSilentAsync( amsTenantId, 
                                                                    clientCredential, 
                                                                    ActiveDirectoryServiceSettings.Azure);
        }

        /// <summary>
        /// Generates the token for the Content and Command Id
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="commandId"></param>
        /// <returns></returns>
        public static async Task<string> GetContentToken(AmsData amsData,
                                                         Guid contentId, 
                                                         Guid commandId)
        {
            IAzureMediaServicesClient amsclient = await CreateMediaServicesClientAsync(amsData.AmsArmEndPoint,
                                                                                        amsData.AmsClientId,
                                                                                        amsData.AmsClientSecret,
                                                                                        amsData.AmsTenantId,
                                                                                        amsData.AmsSubscriptionId);

            string token = string.Empty;

            string locatorName = $"{contentId}|{commandId}";

            byte[] tokenSigningKey = Convert.FromBase64String(amsData.AmsTokenSigningKey);

            // Get Streaming locator
            StreamingLocator locator = await amsclient.StreamingLocators.GetAsync(amsData.AmsResourceGroupName,
                                                                                  amsData.AmsAccountName,
                                                                                  locatorName);

            if (locator != null)
            {
                // In this example, we want to play the Widevine (CENC) encrypted stream. 
                // We need to get the key identifier of the content key where its type is CommonEncryptionCenc.
                string keyIdentifier = locator.ContentKeys.Where(k => k.Type == StreamingLocatorContentKeyType.CommonEncryptionCenc).First().Id.ToString();

                // In order to generate our test token we must get the ContentKeyId to put in the ContentKeyIdentifierClaim claim.
                token = GenerateTokenAsync(amsData.AmsTokenIssuer,
                                           amsData.AmsTokenAudience,
                                           keyIdentifier,
                                           amsData.AmsTokenExpiryInMts,
                                           tokenSigningKey,
                                           amsData.AmsTokenMaxUses);
            }
            
            return token;
        }



        // <summary>
        /// Create a token that will be used to protect your stream.
        /// Only authorized clients would be able to play the video.  
        /// </summary>
        /// <param name="issuer">The issuer is the secure token service that issues the token. </param>
        /// <param name="audience">The audience, sometimes called scope, describes the intent of the token or the resource the token authorizes access to. </param>
        /// <param name="keyIdentifier">The content key ID.</param>
        /// <param name="tokenVerificationKey">Contains the key that the token was signed with. </param>
        /// <param name="tokenReplayMaxUses">To set a limit on how many times the same token can be used to request a key or a license.. </param>
        /// <returns></returns>
        private static string GenerateTokenAsync(string issuer,
                                            string audience,
                                            string keyIdentifier,
                                            int tokenExpiryInMts,
                                            byte[] tokenVerificationKey,
                                            int tokenReplayMaxUses)
        {
            var tokenSigningKey = new SymmetricSecurityKey(tokenVerificationKey);

            SigningCredentials cred = new SigningCredentials(
                tokenSigningKey,
                // Use the  HmacSha256 and not the HmacSha256Signature option, or the token will not work!
                SecurityAlgorithms.HmacSha256,
                SecurityAlgorithms.Sha256Digest);

            Claim[] claims = new Claim[]
            {
                new Claim(ContentKeyPolicyTokenClaim.ContentKeyIdentifierClaim.ClaimType, keyIdentifier),
                new Claim("urn:microsoft:azure:mediaservices:maxuses", tokenReplayMaxUses.ToString())
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
    }

    public class AmsData
    {
        public string AmsArmEndPoint { get; set; }
        
        public string AmsClientId { get; set; }
        
        public string AmsClientSecret { get; set; }
        
        public string AmsTenantId { get; set; }
        
        public string AmsSubscriptionId { get; set; }
        
        public string AmsTokenSigningKey { get; set; }

        public string AmsResourceGroupName { get; set; }
        
        public string AmsAccountName { get; set; }
        
        public string AmsTokenIssuer { get; set; }
        
        public string AmsTokenAudience { get; set; }
        
        public int AmsTokenExpiryInMts { get; set; }

        public int AmsTokenMaxUses { get; set; }
    }
}
