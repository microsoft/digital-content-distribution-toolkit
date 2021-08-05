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
        /// Returns (Blendnet-generated) user id guid from claims list.
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
        /// Returns user phone number from claims. Country code if present is retained
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public static string GetUserPhoneNumber(IEnumerable<Claim> claims)
        {
            var userClaim = claims.Where(x => x.Type.Equals(ApplicationConstants.KaizalaIdentityClaims.PhoneNumber));

            return userClaim.First().Value;
        }

        /// <summary>
        /// Tells whether the Claims contains SuperAdmin role
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public static bool isSuperAdmin(IEnumerable<Claim> claims)
        {
            int superAdminClaimCount = claims
                                        .Where(x => x.Type == ClaimTypes.Role && x.Value == ApplicationConstants.KaizalaIdentityRoles.SuperAdmin)
                                        .Count();
            return superAdminClaimCount != 0;
        }
    }
}
