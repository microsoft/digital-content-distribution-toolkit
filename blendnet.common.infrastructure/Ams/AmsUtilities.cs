using Microsoft.Azure.Management.Media;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
    }
}
