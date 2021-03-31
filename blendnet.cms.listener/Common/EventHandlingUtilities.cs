using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using blendnet.common.dto.cms;
using Microsoft.Azure.Management.Media;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.listener.IntegrationEventHandling
{
    public static class EventHandlingUtilities
    {
        /// <summary>
        /// https://docs.microsoft.com/en-us/azure/storage/common/storage-stored-access-policy-define-dotnet?tabs=dotnet
        /// </summary>
        /// <param name="blobClient"></param>
        /// <param name="identifier"></param>
        /// <param name="expiryMinutes"></param>
        /// <returns></returns>
        public static string GetServiceSasUriForBlob(BlobClient blobClient, string identifier, int expiryMinutes)
        {
            // Create a SAS token that's valid for one hour.
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                BlobName = blobClient.Name,
                Identifier = identifier,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes)
            };

            Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

            return sasUri.AbsoluteUri;
        }

        /// <summary>
        /// Creates the AzureMediaServicesClient object based on the credentials
        /// supplied in local configuration file.
        /// </summary>
        /// <param name="config">The param is of type ConfigWrapper. This class reads values from local configuration file.</param>
        /// <returns></returns>
        public static async Task<IAzureMediaServicesClient> CreateMediaServicesClientAsync(AppSettings appSettings)
        {
            var credentials = await GetCredentialsAsync(appSettings);

            return new AzureMediaServicesClient(new Uri(appSettings.AmsArmEndPoint), credentials)
            {
                SubscriptionId = appSettings.AmsSubscriptionId
            };
        }

        /// <summary>
        /// Create the ServiceClientCredentials object based on the credentials supplied in local configuration file.
        /// </summary>
        private static async Task<ServiceClientCredentials> GetCredentialsAsync(AppSettings appSettings)
        {
            ClientCredential clientCredential = new ClientCredential(appSettings.AmsClientId, appSettings.AmsClientSecret);

            return await ApplicationTokenProvider.LoginSilentAsync(appSettings.AmsTenantId, clientCredential, ActiveDirectoryServiceSettings.Azure);
        }
    }
}
