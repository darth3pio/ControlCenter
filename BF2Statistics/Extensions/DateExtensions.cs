using System;

namespace BF2Statistics
{
    public static class DateExtensions
    {
        /// <summary>
        /// Returns the current Unix Timestamp
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int ToUnixTimestamp(this DateTime target)
        {
            DateTime Unix = (new DateTime(target.Ticks, target.Kind)).ToUniversalTime();
            return (int)(Unix.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))).TotalSeconds;
        }

        /// <summary>
        /// Converts a timestamp to a UTC DataTime
        /// </summary>
        /// <param name="timestamp">The number of seconds from Epoch</param>
        /// <returns></returns>
        public static DateTime FromUnixTimestamp(this DateTime target, int timestamp)
        {
            DateTime Date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Date.AddSeconds(timestamp);
        }
    }
}
