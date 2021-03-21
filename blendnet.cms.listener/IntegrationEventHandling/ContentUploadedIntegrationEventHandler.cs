using Azure.Storage.Blobs;
using blendnet.cms.listener.Model;
using blendnet.common.dto;
using blendnet.common.dto.Events;
using blendnet.common.infrastructure;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.cms.listener.IntegrationEventHandling
{
    /// <summary>
    /// Responsible to hand content upload event.
    /// Move the Media content from Raw Container to Mezzanine Container.
    /// Move the Attachments from Raw Container to CDN Enabled Storage
    /// </summary>
    public class ContentUploadedIntegrationEventHandler : IIntegrationEventHandler<ContentUploadedIntegrationEvent>
    {
        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        private readonly AppSettings _appSettings;

        BlobServiceClient _cmsBlobServiceClient;

        BlobServiceClient _cmsCdnBlobServiceClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="tc"></param>
        /// <param name="blobClientFactory"></param>
        /// <param name="optionsMonitor"></param>
        public ContentUploadedIntegrationEventHandler(ILogger<ContentUploadedIntegrationEventHandler> logger,
                                                             TelemetryClient tc,
                                                             IAzureClientFactory<BlobServiceClient> blobClientFactory,
                                                             IOptionsMonitor<AppSettings> optionsMonitor)
        {
            _logger = logger;

            _telemetryClient = tc;

            _cmsBlobServiceClient = blobClientFactory.CreateClient(ApplicationConstants.StorageInstanceNames.CMSStorage);

            _cmsCdnBlobServiceClient = blobClientFactory.CreateClient(ApplicationConstants.StorageInstanceNames.CMSCDNStorage);

            _appSettings = optionsMonitor.CurrentValue;
        }


        /// <summary>
        /// Handle the content uploaded integration event handler
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public Task Handle(ContentUploadedIntegrationEvent integrationEvent)
        {
            throw new NotImplementedException();
        }
    }
}
