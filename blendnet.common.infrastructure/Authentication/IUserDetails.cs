using blendnet.common.dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.infrastructure.Authentication
{
    /// <summary>
    /// Implement the interface to return user details for the given phone number
    /// </summary>
    public interface IUserDetails
    {
        /// <summary>
        /// Get the User Details based on given phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public Task<User> GetUserDetails(string phoneNumber);
    }
}
