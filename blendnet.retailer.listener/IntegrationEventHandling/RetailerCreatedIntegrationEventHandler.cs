// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using blendnet.api.proxy.KaizalaIdentity;
using blendnet.common.dto;
using blendnet.common.dto.Events;
using blendnet.common.dto.Identity;
using blendnet.common.dto.Retailer;
using blendnet.common.infrastructure;
using blendnet.retailer.repository.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace blendnet.retailer.listener.IntegrationEventHandling
{
    public class RetailerCreatedIntegrationEventHandler : IIntegrationEventHandler<RetailerCreatedIntegrationEvent>
    {
        private readonly IRetailerRepository _retailerRepository;

        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        private readonly KaizalaIdentityProxy _kaizalaIdentityProxy;

        private readonly RetailerAppSettings _appSettings;

        public RetailerCreatedIntegrationEventHandler(ILogger<RetailerCreatedIntegrationEventHandler> logger,
                                                        TelemetryClient tc,
                                                        KaizalaIdentityProxy kaizalaIdentityProxy,
                                                        IRetailerRepository retailerRepository,
                                                        IOptionsMonitor<RetailerAppSettings> optionsMonitor)
        {
            _retailerRepository = retailerRepository;
            _logger = logger;
            _telemetryClient = tc;
            _kaizalaIdentityProxy = kaizalaIdentityProxy;
            _appSettings = optionsMonitor.CurrentValue;
        }

        /// <summary>
        /// Handle Retailer Created Event
        /// 1) Insert a record in retailer collection
        /// 2) Assign the retailer role
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(RetailerCreatedIntegrationEvent integrationEvent)
        {
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>("RetailerCreatedIntegrationEvent.Handle"))
                {
                    var existingRetailer = await _retailerRepository.GetRetailerByPartnerId(integrationEvent.Retailer.PartnerId);
                    if (existingRetailer is not null)
                    {
                        _logger.LogInformation($"Skipping as retailer already exists for PartnerID: {integrationEvent.Retailer.PartnerId}");
                        return;
                    }

                    _logger.LogInformation($"Assigning Retailer role for Retailer {integrationEvent.Retailer.PartnerId}");
                    await AssignRoleInKaizalaIdentity(integrationEvent);

                    // Create retailer
                    _logger.LogInformation($"Creating Retailer in DB {integrationEvent.Retailer.PartnerId}");
                    await this._retailerRepository.CreateRetailer(integrationEvent.Retailer);

                    _logger.LogInformation($"Done Creating Retailer {integrationEvent.Retailer.PartnerId}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"RetailerCreatedIntegrationEvent.Handle failed for PartnerID: {integrationEvent.Retailer.PartnerId}");
            }
        }

        private async Task AssignRoleInKaizalaIdentity(RetailerCreatedIntegrationEvent integrationEvent)
        {
            AddPartnerUsersRoleRequest addPartnerUsersRoleRequest = new AddPartnerUsersRoleRequest();
            addPartnerUsersRoleRequest.ApplicationName = _appSettings.KaizalaIdentityAppName;
            addPartnerUsersRoleRequest.PhoneRoleList = new List<PhoneRole>();

            PhoneRole phoneRole = new PhoneRole();
            phoneRole.PhoneNo = $"{ApplicationConstants.CountryCodes.India}{integrationEvent.Retailer.PhoneNumber}";
            phoneRole.Role = ApplicationConstants.KaizalaIdentityRoles.Retailer;
            addPartnerUsersRoleRequest.PhoneRoleList.Add(phoneRole);

            try
            {
                await _kaizalaIdentityProxy.AddPartnerUsersRole(addPartnerUsersRoleRequest);
                _logger.LogInformation($"Assigned Retailer role for Retailer {integrationEvent.Retailer.PartnerId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to assign Retailer role for Retailer {integrationEvent.Retailer.PartnerId}");
            }
        }
    }
}
