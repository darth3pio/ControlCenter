using System;

namespace BF2Statistics
{
    static class LongExtensions
    {
        /// <summary>
        /// Converts this long value to a file size
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static string ToFileSize(this long l)
        {
            return String.Format(new FileSizeFormatProvider(), "{0:fs}", l);
        }
    }
}
