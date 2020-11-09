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

namespace blendnet.crm.user.api.Repository.GraphRepository
{
    public class IdentityRepository : IIdentityRespository
    {
        GraphServiceClient _graphServiceClient;

        public IdentityRepository(GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
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

    }
}
