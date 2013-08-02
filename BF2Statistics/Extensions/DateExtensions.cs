using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            return (int)(target - new DateTime(1970, 1, 1, 0, 0, 0, target.Kind)).TotalSeconds;
        }

        /// <summary>
        /// Converts a timestamp to a DataTime
        /// </summary>
        /// <param name="target"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this DateTime target, int timestamp)
        {
            DateTime Date = new DateTime(1970, 1, 1, 0, 0, 0, target.Kind);
            return Date.AddSeconds(timestamp);
        }
    }
}
