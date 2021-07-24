using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace blendnet.common.dto.User
{
    /// <summary>
    /// Helper class to extract required information from claims data
    /// </summary>
    public class UserClaimData
    {
        /// <summary>
        /// Returns user id guid from claims list. This removes scale unit information from claim
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public static Guid GetUserId(IEnumerable<Claim> claims)
        {
            var userClaim = claims.Where(x => x.Type.Equals(ApplicationConstants.BlendNetClaims.UId));

            string userIdStr = userClaim.First().Value;

            Guid userId = new Guid(userIdStr);

            return userId;
        }

        /// <summary>
        /// Returns identity user id guid from claims list. 
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public static Guid GetIdentityUserId(IEnumerable<Claim> claims)
        {
            var userClaim = claims.Where(x => x.Type.Equals(ApplicationConstants.KaizalaIdentityClaims.IdentityUId));

            string userIdStr = userClaim.First().Value;

            Guid userId = new Guid(userIdStr);

            return userId;
        }

        /// <summary>
        /// Returns user phone number from claims. Country code if present is retained
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public static string GetUserPhoneNumber(IEnumerable<Claim> claims)
        {
            var userClaim = claims.Where(x => x.Type.Equals(ApplicationConstants.KaizalaIdentityClaims.PhoneNumber));

            return userClaim.First().Value;
        }
    }
}
