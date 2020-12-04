using blendnet.crm.user.api.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Extensions.Options;
using blendnet.crm.user.api.Model;
using Microsoft.Identity.Client;
using Microsoft.Graph.Auth;
using blendnet.common.dto.Identity;

namespace blendnet.crm.user.api.Repository.GraphRepository
{
    public class IdentityRepository : IIdentityRespository
    {
        private AppSettings _appSettings;

        private GraphServiceClient _graphServiceClient;

        public IdentityRepository(GraphServiceClient graphServiceClient,
                                  IOptionsMonitor<AppSettings> options)
        {
            _graphServiceClient = graphServiceClient;

            _appSettings = options.CurrentValue;
        }

        /// <summary>
        /// Returns the list of groups user is member of
        /// </summary>
        /// <param name="userObjectId"></param>
        /// <returns></returns>
        public async Task<List<string>> ListMemberOf(string userObjectId)
        {
            var memberOf = await _graphServiceClient.Users[userObjectId].MemberOf
                .Request()
                .GetAsync();

            var groups = memberOf.CurrentPage.ToList<DirectoryObject>();

            List<string> groupsToReturns = new List<string>();

            string groupName;

            foreach (DirectoryObject group in groups)
            {
                if (group is Group)
                {
                    groupName = ((Group)group).DisplayName;

                    groupsToReturns.Add(groupName);
                }
            }

            return groupsToReturns;
        }

        /// <summary>
        /// Get the user by user principal name
        /// </summary>
        /// <param name="userPrincipalName"></param>
        /// <returns></returns>
        public async Task<IdentityUserDto> GetUser(string userPrincipalName)
        {
            string upn = System.Net.WebUtility.UrlEncode(userPrincipalName);

            var result = await _graphServiceClient.Users
                .Request()
                .Filter($"identities/any(c:c/issuerAssignedId eq '{upn}' and c/issuer eq '{_appSettings.GraphClientTenant}')")
                .Select(e => new { e.DisplayName, e.Id, e.GivenName, e.UserPrincipalName, e.UserType, e.Identities }).GetAsync();

            User user = result.CurrentPage.ToList<User>().FirstOrDefault();

            if (user != null)
            {
                IdentityUserDto identityUser = new IdentityUserDto()
                {
                    Id = user.Id,
                    DisplayName = user.DisplayName,
                    GivenName = user.GivenName,
                    UserPrincipalName = user.UserPrincipalName
                };

                return identityUser;
            }

            return null;
        }
        
    }
}
