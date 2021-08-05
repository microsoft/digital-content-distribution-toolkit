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
    public class LinkRetailerIntegrationEventHandler : IIntegrationEventHandler<LinkRetailerIntegrationEvent>
    {
        private readonly IRetailerRepository _retailerRepository;

        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        private readonly KaizalaIdentityProxy _kaizalaIdentityProxy;

        private readonly RetailerAppSettings _appSettings;

        public LinkRetailerIntegrationEventHandler(ILogger<LinkRetailerIntegrationEventHandler> logger,
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

        public async Task Handle(LinkRetailerIntegrationEvent integrationEvent)
        {
            string partnerId = RetailerDto.CreatePartnerId(integrationEvent.PartnerCode, integrationEvent.PartnerProvidedId);

            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>("LinkRetailerIntegrationEvent.Handle"))
                {

                    await AssignRoleInKaizalaIdentity(integrationEvent);

                    // Create retailer
                    _logger.LogInformation($"Linking Retailer {partnerId} to User {integrationEvent.User.UserId}");
                    RetailerDto existingRetailer = await _retailerRepository.GetRetailerByPartnerId(partnerId, shouldGetInactiveRetailer: true);
                    if (existingRetailer is null)
                    {
                        _logger.LogError($"Could not find retailer for partner ID : {partnerId}");
                        return;
                    }

                    DateTime now = DateTime.UtcNow;

                    existingRetailer.UserId = integrationEvent.User.UserId;
                    existingRetailer.StartDate = now;
                    existingRetailer.EndDate = DateTime.MaxValue;
                    existingRetailer.ModifiedByByUserId = integrationEvent.User.UserId;
                    existingRetailer.ModifiedDate = now;
                    existingRetailer.PhoneNumber = integrationEvent.User.PhoneNumber;

                    _logger.LogInformation($"Update Retailer in DB {partnerId}");
                    await this._retailerRepository.UpdateRetailer(existingRetailer);

                    _logger.LogInformation($"Done linking Retailer {partnerId}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"LinkRetailerIntegrationEvent.Handle failed for PartnerID: {partnerId}");
            }
        }

        private async Task AssignRoleInKaizalaIdentity(LinkRetailerIntegrationEvent integrationEvent)
        {
            string partnerId = RetailerDto.CreatePartnerId(integrationEvent.PartnerCode, integrationEvent.PartnerProvidedId);
            AddPartnerUsersRoleRequest addPartnerUsersRoleRequest = new AddPartnerUsersRoleRequest();
            addPartnerUsersRoleRequest.ApplicationName = _appSettings.KaizalaIdentityAppName;
            addPartnerUsersRoleRequest.PhoneRoleList = new List<PhoneRole>();

            PhoneRole phoneRole = new PhoneRole();
            phoneRole.PhoneNo = $"{ApplicationConstants.CountryCodes.India}{integrationEvent.User.PhoneNumber}";
            phoneRole.Role = ApplicationConstants.KaizalaIdentityRoles.Retailer;
            addPartnerUsersRoleRequest.PhoneRoleList.Add(phoneRole);

            try
            {
                await _kaizalaIdentityProxy.AddPartnerUsersRole(addPartnerUsersRoleRequest);
                _logger.LogInformation($"Assigned Retailer role for Retailer {partnerId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to assign Retailer role for Retailer {partnerId}");
            }
        }
    }
}
