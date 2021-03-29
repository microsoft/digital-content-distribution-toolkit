using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
