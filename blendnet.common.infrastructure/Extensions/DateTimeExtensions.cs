using System;
using System.Linq;

namespace blendnet.common.infrastructure.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// TimeZoneInfo that represents India Standard Time (GMT+530)
        /// 
        /// Created manually as the Timezone information is not embedded in runtime system
        /// </summary>
        /// <value></value>
        private static TimeZoneInfo IndiaTimeZoneInfo => TimeZoneInfo.CreateCustomTimeZone( "India Standard Time", 
                                                                                            new TimeSpan(5, 30, 0), 
                                                                                            "India Standard Time", 
                                                                                            "India Standard Time");

        /// <summary>
        /// Tolerance for accommodating time difference between server and client
        /// </summary>
        private const uint TOLENRANCE_FOR_CURRENT_TIME_MINUTES = 5;

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

        /// <summary>
        /// Validation - date can't be from far future
        /// Tolerate little into future to accommodate clock difference between server and client
        /// </summary>
        /// <param name="dateTime">DateTime to check (should be in UTC)</param>
        /// <returns></returns>
        public static Boolean IsCurrentOrPast(this DateTime dateTime)
        {
            DateTime now = DateTime.UtcNow;
            return dateTime < now.AddMinutes(TOLENRANCE_FOR_CURRENT_TIME_MINUTES);
        }
    }
}