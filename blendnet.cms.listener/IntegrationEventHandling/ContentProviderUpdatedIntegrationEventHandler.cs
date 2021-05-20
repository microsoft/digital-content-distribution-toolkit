using blendnet.api.proxy.KaizalaIdentity;
using blendnet.common.dto;
using blendnet.common.dto.cms;
using blendnet.common.dto.Events;
using blendnet.common.dto.Identity;
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
    /// Content Provider Update
    /// </summary>
    public class ContentProviderUpdatedIntegrationEventHandler : IIntegrationEventHandler<ContentProviderUpdatedIntegrationEvent>
    {
        private readonly ILogger _logger;

        private TelemetryClient _telemetryClient;

        private readonly AppSettings _appSettings;

        KaizalaIdentityProxy _kaizalaIdentityProxy;

        public ContentProviderUpdatedIntegrationEventHandler(ILogger<ContentProviderCreatedIntegrationEventHandler> logger,
                                                             TelemetryClient tc,
                                                             KaizalaIdentityProxy kaizalaIdentityProxy,
                                                             IOptionsMonitor<AppSettings> optionsMonitor)
        {
            _logger = logger;

            _telemetryClient = tc;

            _kaizalaIdentityProxy = kaizalaIdentityProxy;

            _appSettings = optionsMonitor.CurrentValue;
        }

        /// <summary>
        /// Handle Content Provider Created Integration EventHandler
        /// </summary>
        /// <param name="integrationEvent"></param>
        /// <returns></returns>
        public async Task Handle(ContentProviderUpdatedIntegrationEvent integrationEvent)
        {
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>("ContentProviderUpdatedIntegrationEventHandler.Handle"))
                {
                    _logger.LogInformation($"Update Message Recieved for content provider id: {integrationEvent.BeforeUpdateContentProvider.Id}");

                    List<ContentAdministratorDto> addedContentAdministators = new List<ContentAdministratorDto>();

                    List<ContentAdministratorDto> deletedContentAdministators = new List<ContentAdministratorDto>();


                    GetModifiedDeletedContentAdministrators(integrationEvent.BeforeUpdateContentProvider,
                                                            integrationEvent.AfterUpdateContentProvider,
                                                            addedContentAdministators,
                                                            deletedContentAdministators);

                    //If any content administrators were added
                    if (addedContentAdministators.Count > 0)
                    {
                        await AssignContentAdministratorRole(integrationEvent.BeforeUpdateContentProvider.Id.Value,addedContentAdministators);

                    }else
                    {
                        _logger.LogInformation($"No added content administrators for content provider id in update: {integrationEvent.BeforeUpdateContentProvider.Id.Value}");
                    }

                    //If any content administrators were deleted
                    if (deletedContentAdministators.Count > 0)
                    {
                        await UnAssignContentAdministratorRole(integrationEvent.BeforeUpdateContentProvider.Id.Value, deletedContentAdministators);

                    }
                    else
                    {
                        _logger.LogInformation($"No deleted content administrators for content provider id in update: {integrationEvent.BeforeUpdateContentProvider.Id.Value}");
                    }

                    _logger.LogInformation($"Update Message Process Completed for content provider id: {integrationEvent.BeforeUpdateContentProvider.Id.Value}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ContentProviderUpdatedIntegrationEventHandler.Handle failed for {integrationEvent.BeforeUpdateContentProvider.Id.Value}");
            }
        }


        /// <summary>
        /// Get Modified Deleted Content Administrators
        /// </summary>
        /// <param name="beforeContentProvider"></param>
        /// <param name="afterContentProvider"></param>
        /// <param name="addedContentAdministrators"></param>
        /// <param name="deletedContentAdministrators"></param>
        private void GetModifiedDeletedContentAdministrators(ContentProviderDto beforeContentProvider, 
                                                            ContentProviderDto afterContentProvider,
                                                            List<ContentAdministratorDto> addedContentAdministrators,
                                                            List<ContentAdministratorDto> deletedContentAdministrators)
        {
            if ((beforeContentProvider.ContentAdministrators == null || beforeContentProvider.ContentAdministrators.Count <= 0) &&
                (afterContentProvider.ContentAdministrators == null || afterContentProvider.ContentAdministrators.Count <= 0))
            {
                _logger.LogInformation($"No content administrator in previous and latest content provider id: {beforeContentProvider.Id.Value}");

                return;
            }

            //check if all added
            if ((beforeContentProvider.ContentAdministrators == null || beforeContentProvider.ContentAdministrators.Count <= 0) &&
                (afterContentProvider.ContentAdministrators != null && afterContentProvider.ContentAdministrators.Count > 0))
            {
                //all added
                foreach (ContentAdministratorDto contentAdministrator in afterContentProvider.ContentAdministrators)
                {
                    addedContentAdministrators.Add(contentAdministrator);
                }

                return;
            }

            //check if all deleted
            if ((beforeContentProvider.ContentAdministrators != null && beforeContentProvider.ContentAdministrators.Count > 0) &&
                (afterContentProvider.ContentAdministrators == null || afterContentProvider.ContentAdministrators.Count <= 0))
            {
                //all deleted
                foreach (ContentAdministratorDto contentAdministrator in beforeContentProvider.ContentAdministrators)
                {
                    deletedContentAdministrators.Add(contentAdministrator);
                }

                return;
            }

            //Find out added content administrators
            foreach (ContentAdministratorDto afterContentAdministrator in afterContentProvider.ContentAdministrators)
            {
                if (!beforeContentProvider.ContentAdministrators.Exists(ca=>ca.PhoneNumber.Equals(afterContentAdministrator.PhoneNumber,StringComparison.InvariantCultureIgnoreCase)))
                {
                    addedContentAdministrators.Add(afterContentAdministrator);
                }
            }

            //Find out deleted content administrators
            foreach (ContentAdministratorDto beforeContentAdministrator in beforeContentProvider.ContentAdministrators)
            {
                if (!afterContentProvider.ContentAdministrators.Exists(ca => ca.PhoneNumber.Equals(beforeContentAdministrator.PhoneNumber, StringComparison.InvariantCultureIgnoreCase)))
                {
                    deletedContentAdministrators.Add(beforeContentAdministrator);
                }
            }
        }



        /// <summary>
        /// Assigns the content administrator role to the give list of content administrators
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <param name="addedContentAdministrators"></param>
        /// <returns></returns>
        private async Task AssignContentAdministratorRole(Guid contentProviderId, List<ContentAdministratorDto> addedContentAdministrators)
        {
            AddPartnerUsersRoleRequest addPartnerUsersRoleRequest = new AddPartnerUsersRoleRequest();

            addPartnerUsersRoleRequest.ApplicationName = _appSettings.KaizalaIdentityAppName;

            addPartnerUsersRoleRequest.PhoneRoleList = new List<PhoneRole>();

            foreach (ContentAdministratorDto contentAdministrator in addedContentAdministrators)
            {
                PhoneRole phoneRole = new PhoneRole();
                phoneRole.PhoneNo = $"{ApplicationConstants.CountryCodes.India}{contentAdministrator.PhoneNumber}";
                phoneRole.Role = ApplicationConstants.KaizalaIdentityRoles.ContentAdmin;
                addPartnerUsersRoleRequest.PhoneRoleList.Add(phoneRole);
            }

            try
            {
                await _kaizalaIdentityProxy.AddPartnerUsersRole(addPartnerUsersRoleRequest);

                _logger.LogInformation($"Assigned Content Administrator roles for content provider {contentProviderId} from update. Content Admin Count {addedContentAdministrators.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to assign Content Administrator roles for content provider {contentProviderId} from update");
            }
        }



        /// <summary>
        /// Deletes the content administrator role to the give list of content administrators
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <param name="addedContentAdministrators"></param>
        /// <returns></returns>
        private async Task UnAssignContentAdministratorRole(Guid contentProviderId, List<ContentAdministratorDto> deletedContentAdministrators)
        {
            DeletePartnerUsersRoleRequest deletePartnerUsersRoleRequest = new DeletePartnerUsersRoleRequest();

            deletePartnerUsersRoleRequest.ApplicationName = _appSettings.KaizalaIdentityAppName;

            deletePartnerUsersRoleRequest.PhoneRoleList = new List<PhoneRole>();

            foreach (ContentAdministratorDto contentAdministrator in deletedContentAdministrators)
            {
                PhoneRole phoneRole = new PhoneRole();
                phoneRole.PhoneNo = $"{ApplicationConstants.CountryCodes.India}{contentAdministrator.PhoneNumber}";
                phoneRole.Role = ApplicationConstants.KaizalaIdentityRoles.ContentAdmin;
                deletePartnerUsersRoleRequest.PhoneRoleList.Add(phoneRole);
            }

            try
            {
                await _kaizalaIdentityProxy.DeletePartnerUsersRole(deletePartnerUsersRoleRequest);

                _logger.LogInformation($"Deleted Content Administrator roles for content provider {contentProviderId} from update. Content Admin Count {deletedContentAdministrators.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete Content Administrator roles for content provider {contentProviderId} from update");
            }
        }



    }
}
