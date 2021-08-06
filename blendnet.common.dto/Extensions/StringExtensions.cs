using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
    
}
