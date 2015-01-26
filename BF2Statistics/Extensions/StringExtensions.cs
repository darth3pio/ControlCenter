using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security;

namespace BF2Statistics
{
    static class StringExtensions
    {
        public static string Repeat(this string input, int count)
        {
            StringBuilder builder = new StringBuilder((input == null ? 0 : input.Length) * count);

            for (int i = 0; i < count; i++)
                builder.Append(input);

            return builder.ToString();
        }

        public static string EscapeXML(this string s)
        {
            return !SecurityElement.IsValidText(s) ? SecurityElement.Escape(s) : s;
        }

        public static string UnescapeXML(this string s)
        {
            StringBuilder builder = new StringBuilder(s);
            builder.Replace("&apos;", "'");
            builder.Replace("&quot;", "\"");
            builder.Replace("&gt;", ">");
            builder.Replace("&lt;", "<");
            builder.Replace("&amp;", "&");
            return builder.ToString();
        }

        public static string MakeFileNameSafe(this string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
    }
}
