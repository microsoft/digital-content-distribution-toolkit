using System;
using System.Linq;

namespace blendnet.common.infrastructure.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts given DateTime object from UTC to India Time zone
        /// </summary>
        /// <param name="sourceUtc">source DateTime (should be in UTC)</param>
        /// <returns>DateTime in IST time zone</returns>
        public static DateTime ToIndiaStandardTime(this DateTime sourceUtc)
        {
            // get India TimeZone object
            // need this logic based on Offset, as TimeZone IDs are different on each machine
            TimeZoneInfo timeZoneIST = TimeZoneInfo.GetSystemTimeZones().Where(e => e.BaseUtcOffset.Hours == 5 && e.BaseUtcOffset.Minutes == 30).First();

            DateTime dateTimeInIST = TimeZoneInfo.ConvertTimeFromUtc(sourceUtc, timeZoneIST);
            return dateTimeInIST;
        }
    }
}