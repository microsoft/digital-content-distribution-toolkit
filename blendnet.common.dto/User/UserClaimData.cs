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
            var userClaim = claims.Where(x => x.Type.Equals(ApplicationConstants.KaizalaIdentityClaims.UId));

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
        /// returns the partner code by mapping claims' user ID to the partner code mentioned in the service ID mapping
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="serviceIdMapping"></param>
        /// <returns></returns>
        public static string GetPartnerCode(IEnumerable<Claim> claims, Dictionary<string, string> serviceIdMapping)
        {
            var userId = GetUserId(claims).ToString();
            var mappedPartnerCode = serviceIdMapping.GetValueOrDefault(userId, ApplicationConstants.PartnerCode.UNKNOWN);
            return mappedPartnerCode;
        }
    }
}
