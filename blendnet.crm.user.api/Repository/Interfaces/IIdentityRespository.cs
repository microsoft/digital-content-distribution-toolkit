using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.crm.user.api.Repository.Interfaces
{
    public interface IIdentityRespository
    {
        /// <summary>
        /// Returns the list of group names the user is member of
        /// </summary>
        /// <param name="userObjectId"></param>
        /// <returns></returns>
        Task<List<string>> ListMemberOf(string userObjectId);
    }
}
