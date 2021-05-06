using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using blendnet.api.proxy.KaizalaIdentity;
using blendnet.cms.listener.Model;
using blendnet.common.dto;
using blendnet.common.dto.cms;
using blendnet.common.dto.Events;
using blendnet.common.dto.Identity;
using blendnet.common.infrastructure;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.listener.IntegrationEventHandling
{
    /// <summary>
    /// Content Provider Created Integration EventHandler
    /// 1) Responsible for creating blob storage 
    /// 2) Responsible for adding user to content administrator user group
    /// </summary>
    public class ContentProviderCreatedIntegrationEventHandler : IIntegrationEventHandler<ContentProviderCreatedIntegrationEvent>
    {
        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        private readonly AppSettings _appSettings;

        BlobServiceClient _cmsBlobServiceClient;

        BlobServiceClient _cmsCdnBlobServiceClient;

        KaizalaIdentityProxy _kaizalaIdentityProxy;

        public ContentProviderCreatedIntegrationEventHandler(ILogger<ContentProviderCreatedIntegrationEventHandler> logger,
                                                             TelemetryClient tc,
                                                             IAzureClientFactory<BlobServiceClient> blobClientFactory,
                                                             KaizalaIdentityProxy kaizalaIdentityProxy,
                                                             IOptionsMonitor<AppSettings> optionsMonitor)
        {
            _logger = logger;

            _telemetryClient = tc;

            _cmsBlobServiceClient = blobClientFactory.CreateClient(ApplicationConstants.StorageInstanceNames.CMSStorage);

            _cmsCdnBlobServiceClient = blobClientFactory.CreateClient(ApplicationConstants.StorageInstanceNames.CMSCDNStorage);

            _kaizalaIdentityProxy = kaizalaIdentityProxy;

            _appSettings = optionsMonitor.CurrentValue;
        }

        /// <summary>
        /// Handle Content Provider Created Integration EventHandler
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(ContentProviderCreatedIntegrationEvent integrationEvent)
        {
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>("ContentProviderCreatedIntegrationEventHandler.Handle"))
                {
                    _logger.LogInformation($"Message Recieved for content provider id: {integrationEvent.ContentProvider.Id}");

                    _logger.LogInformation($"Creating storage containers for content provider id: {integrationEvent.ContentProvider.Id}");

                    await CreateStorageContainers(integrationEvent);

                    await CreateCDNStorageContainers(integrationEvent);

                    if (integrationEvent.ContentProvider.ContentAdministrators != null &&
                        integrationEvent.ContentProvider.ContentAdministrators.Count > 0)
                    {
                        _logger.LogInformation($"Adding administrators to Azure AD for : {integrationEvent.ContentProvider.Id.Value}");

                        await AddUserToAzureAD(integrationEvent);
                    }

                    _logger.LogInformation($"Message Process Completed for content provider id: {integrationEvent.ContentProvider.Id.Value}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ContentProviderCreatedIntegrationEventHandler.Handle failed for {integrationEvent.ContentProvider.Id.Value}");
            }
        }

        /// <summary>
        /// Creates the storage container if does not exists
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task CreateStorageContainers(ContentProviderCreatedIntegrationEvent integrationEvent)
        {
            try
            {
                var baseName = integrationEvent.ContentProvider.Id.ToString().Trim().ToLower();

                string rawContainerName = $"{baseName}{ApplicationConstants.StorageContainerSuffix.Raw}";

                string mezzContainerName = $"{baseName}{ApplicationConstants.StorageContainerSuffix.Mezzanine}";

                string processedContainerName = $"{baseName}{ApplicationConstants.StorageContainerSuffix.Processed}";

                var containers = _cmsBlobServiceClient.GetBlobContainers(prefix: baseName);

                if (!containerExists(rawContainerName))
                {
                    //Create Blob Container
                    await _cmsBlobServiceClient.CreateBlobContainerAsync(rawContainerName);

                    Dictionary<string, string> policies = new Dictionary<string, string>();

                    policies.Add(ApplicationConstants.StorageContainerPolicyNames.RawReadOnly, ApplicationConstants.Policy.ReadOnlyPolicyPermissions);

                    policies.Add(ApplicationConstants.StorageContainerPolicyNames.RawReadWriteAll, ApplicationConstants.Policy.ReadWriteAllPolicyPermissions);

                    //Create Container Policy
                    await CreateContainerPolicy(rawContainerName, policies);
                }
                else
                {
                    _logger.LogInformation($"{rawContainerName} already exists");
                }

                if (!containerExists(mezzContainerName))
                {
                    await _cmsBlobServiceClient.CreateBlobContainerAsync(mezzContainerName);

                    //Create Container Policy
                    await CreateContainerPolicy(mezzContainerName,
                                                ApplicationConstants.StorageContainerPolicyNames.MezzanineReadOnly,
                                                ApplicationConstants.Policy.ReadOnlyPolicyPermissions);
                }
                else
                {
                    _logger.LogInformation($"{mezzContainerName} already exists");
                }

                if (!containerExists(processedContainerName))
                {
                    await _cmsBlobServiceClient.CreateBlobContainerAsync(processedContainerName);

                    //Create Container Policy
                    await CreateContainerPolicy(processedContainerName,
                                                ApplicationConstants.StorageContainerPolicyNames.ProcessedReadOnly,
                                                ApplicationConstants.Policy.ReadOnlyPolicyPermissions);
                }
                else
                {
                    _logger.LogInformation($"{processedContainerName} already exists");
                }

                bool containerExists(string containerName)
                {
                    return containers.Where(container => container.Name == containerName).ToList().Count > 0;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"CreateStorageContainers failed for {integrationEvent.ContentProvider.Id.Value}");
            }
        }

        /// <summary>
        /// Creates a container in CDN storage
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task CreateCDNStorageContainers(ContentProviderCreatedIntegrationEvent integrationEvent)
        {
            try
            {
                var baseName = integrationEvent.ContentProvider.Id.ToString().Trim().ToLower();

                string cdnContainerName = $"{baseName}{ApplicationConstants.StorageContainerSuffix.Cdn}";

                var containers = _cmsCdnBlobServiceClient.GetBlobContainers(prefix: baseName);

                if (!containerExists(cdnContainerName))
                {
                    await _cmsCdnBlobServiceClient.CreateBlobContainerAsync(cdnContainerName,Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
                }
                else
                {
                    _logger.LogInformation($"{cdnContainerName} already exists");
                }

                bool containerExists(string containerName)
                {
                    return containers.Where(container => container.Name == containerName).ToList().Count > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"CreateCDNStorageContainers failed for {integrationEvent.ContentProvider.Id.Value}");
            }
        }

        /// <summary>
        /// Create container policy
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="policyName"></param>
        /// <param name="policyPermissions"></param>
        /// <returns></returns>
        public async Task CreateContainerPolicy(string containerName, string policyName, string policyPermissions)
        {
            Dictionary<string, string> policies = new Dictionary<string, string>();

            policies.Add(policyName, policyPermissions);

            await CreateContainerPolicy(containerName, policies);
        }

        /// <summary>
        /// Create Container Policy
        /// https://docs.microsoft.com/en-us/azure/storage/common/storage-stored-access-policy-define-dotnet?tabs=dotnet
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="policyName"></param>
        /// <param name="policyPermissions"></param>
        /// <returns></returns>
        public async Task CreateContainerPolicy(string containerName, Dictionary<string,string> policyDetails)
        {
            List<BlobSignedIdentifier> signedIdentifiers = new List<BlobSignedIdentifier>();

            BlobSignedIdentifier blobSignedIdentifier;

            foreach (KeyValuePair<string, string> entry in policyDetails)
            {
                blobSignedIdentifier = new BlobSignedIdentifier()
                {
                    Id = entry.Key,
                    AccessPolicy = new BlobAccessPolicy
                    {
                        Permissions = entry.Value
                    }
                };

                signedIdentifiers.Add(blobSignedIdentifier);
            }

            BlobContainerClient containerClient = _cmsBlobServiceClient.GetBlobContainerClient(containerName);

            // Set the container's access policy.
            await containerClient.SetAccessPolicyAsync(permissions: signedIdentifiers);
        }


        /// <summary>
        /// Checks if the user is not the part of group adds it to the Azure AD
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task AddUserToAzureAD(ContentProviderCreatedIntegrationEvent integrationEvent)
        {
            AddPartnerUsersRoleRequest addPartnerUsersRoleRequest = new AddPartnerUsersRoleRequest();

            addPartnerUsersRoleRequest.ApplicationName = _appSettings.KaizalaIdentityAppName;

            addPartnerUsersRoleRequest.PhoneRoleList = new List<PhoneRole>();

            foreach (ContentAdministratorDto contentAdministrator in integrationEvent.ContentProvider.ContentAdministrators)
            {
                PhoneRole phoneRole = new PhoneRole();
                phoneRole.PhoneNo = $"{ApplicationConstants.CountryCodes.India}{contentAdministrator.Mobile}";
                phoneRole.Role = ApplicationConstants.KaizalaIdentityRoles.ContentAdmin;
                addPartnerUsersRoleRequest.PhoneRoleList.Add(phoneRole);
            }
          
            try
            {
                await _kaizalaIdentityProxy.AddPartnerUsersRole(addPartnerUsersRoleRequest);

                _logger.LogInformation($"Assigned Content Administrator roles for content provider {integrationEvent.ContentProvider.Id}. Content Admin Count {integrationEvent.ContentProvider.ContentAdministrators.Count}");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Failed to assign Content Administrator roles for content provider {integrationEvent.ContentProvider.Id}");
            }
            
        }
    }
}
