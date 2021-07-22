using blendnet.common.dto.User;
using blendnet.common.infrastructure.Authentication;
using blendnet.user.repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blendnet.user.api.Common
{
    /// <summary>
    /// Returns the user details via repository
    /// </summary>
    public class UserDetails : IUserDetails
    {
        private IUserRepository _userRepository;

        public UserDetails(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Retrieves the user details via repository
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task<User> GetUserDetails(string phoneNumber)
        {
            return await _userRepository.GetUserByPhoneNumber(phoneNumber);
        }
    }
}
