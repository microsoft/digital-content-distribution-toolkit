using blendnet.api.proxy;
using blendnet.common.dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blendnet.common.infrastructure.Authentication
{
    /// <summary>
    /// Returns the user details by using proxy
    /// </summary>
    public class UserDetailsByProxy : IUserDetails
    {
        UserProxy _userProxy;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userProxy"></param>
        public UserDetailsByProxy(UserProxy userProxy)
        {
            _userProxy = userProxy;
        }

        /// <summary>
        /// Returns the user object based on phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public Task<User> GetUserDetails(string phoneNumber)
        {
            return _userProxy.GetUserByPhoneNumber(phoneNumber);
        }
    }
}
