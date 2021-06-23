using System;
using System.Linq;

namespace blendnet.common.infrastructure.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// TimeZoneInfo that represents India Standard Time (GMT+530)
        /// </summary>
        /// <value></value>
        private static TimeZoneInfo IndiaTimeZoneInfo { get; }

        /// <summary>
        /// Static constructor
        /// </summary>
        static DateTimeExtensions()
        {
            // get India TimeZone object
            // need this logic based on Offset, as TimeZone IDs are different on each machine
            // Caching this in memory because SystemTimeZones call can cause disk IO
            IndiaTimeZoneInfo = TimeZoneInfo.GetSystemTimeZones().Where(e => e.BaseUtcOffset.Hours == 5 && e.BaseUtcOffset.Minutes == 30).First();
        }

        /// <summary>
        /// Converts given DateTime object from UTC to India Time zone
        /// </summary>
        /// <param name="sourceUtc">source DateTime (should be in UTC)</param>
        /// <returns>DateTime in IST time zone</returns>
        public static DateTime ToIndiaStandardTime(this DateTime sourceUtc)
        {
            DateTime dateTimeInIST = TimeZoneInfo.ConvertTimeFromUtc(sourceUtc, IndiaTimeZoneInfo);
            return dateTimeInIST;
        }

        /// <summary>
        /// Converts given DateTime object from India Standard Time to UTC
        /// </summary>
        /// <param name="sourceIST">source DateTime (in IST)</param>
        /// <returns>DateTime in UTC time zone</returns>
        public static DateTime UtcFromIndiaStandardTime(this DateTime sourceIST)
        {
            DateTime dateTimeUtc = TimeZoneInfo.ConvertTimeToUtc(sourceIST, IndiaTimeZoneInfo);
            return dateTimeUtc;
        }
    }
}