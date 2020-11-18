using Azure.Storage.Blobs;
using blendnet.cms.listener.Model;
using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
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
    /// </summary>
    public class ContentProviderCreatedIntegrationEventHandler : IIntegrationEventHandler<ContentProviderCreatedIntegrationEvent>
    {
        private readonly ILogger _logger;
        
        private TelemetryClient _telemetryClient;

        private readonly AppSettings _appSettings;

        public ContentProviderCreatedIntegrationEventHandler(ILogger<ContentProviderCreatedIntegrationEventHandler> logger, 
                                                             TelemetryClient tc, IOptionsMonitor<AppSettings> optionsDelegate)
        {
            _logger = logger;

            _telemetryClient = tc;

            _appSettings = optionsDelegate.CurrentValue;
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
                    _logger.LogInformation($"Message Recieved for content provider id: {integrationEvent.ContentProviderId}");

                    await CreateStorageContainers(integrationEvent);

                    _logger.LogInformation($"Message Process Completed for content provider id: {integrationEvent.ContentProviderId}");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        /// <summary>
        /// Creates the storage container if does not exists
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public  async Task CreateStorageContainers(ContentProviderCreatedIntegrationEvent integrationEvent)
        {
            var client = new BlobServiceClient(_appSettings.CMSStorageConnectionString);

            var baseName = integrationEvent.ContainerBaseName.Trim().ToLower();
            
            string stagingContainerName = $"{baseName}staginge";
            
            string mezzContainerName = $"{baseName}mezzanine";
            
            string processedContainerName = $"{baseName}processed";

            var containers = client.GetBlobContainers(prefix: baseName);

            if (!containerExists(stagingContainerName))
            {
                await client.CreateBlobContainerAsync(stagingContainerName);
            }
            else
            {
                _logger.LogInformation($"{stagingContainerName} already exists");
            }

            if (!containerExists(mezzContainerName))
            {
                await client.CreateBlobContainerAsync(mezzContainerName);
            }else
            {
                _logger.LogInformation($"{mezzContainerName} already exists");
            }

            if (!containerExists(processedContainerName))
            {
                await client.CreateBlobContainerAsync(processedContainerName);
            }else
            {
                _logger.LogInformation($"{processedContainerName} already exists");
            }

            bool containerExists(string containerName)
            {
                return containers.Where(container => container.Name == containerName).ToList().Count > 0;
            }
        }
    }
}
