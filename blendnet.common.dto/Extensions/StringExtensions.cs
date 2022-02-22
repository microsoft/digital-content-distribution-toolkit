using System;
using System.Security.Cryptography;
using System.Text;

namespace blendnet.common.dto.Extensions
{
    /// <summary>
    /// String Extensions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Mask few characters of a string
        /// </summary>
        /// <param name="sourcestring"></param>
        /// <returns></returns>
        public static string Mask(this string sourcestring)
        {
            string stringToReturn = sourcestring;

            if (!string.IsNullOrEmpty(stringToReturn))
            {
                stringToReturn = stringToReturn.Remove(0, 1).Insert(0, "*");
                stringToReturn = stringToReturn.Remove(stringToReturn.Length - 1, 1).Insert(stringToReturn.Length - 1, "*");
            }

            return stringToReturn;
        }

        /// <summary>
        /// Creates Checksum of a string
        /// Uses SHA256 checksum algorithm
        /// </summary>
        /// <param name="sourceString">input string</param>
        /// <returns>checksum (SHA256) in lowercase</returns>
        public static string Checksum(this string sourceString)
        {
            if (string.IsNullOrEmpty(sourceString))
            {
                return null;
            }

            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(sourceString));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
    
}
