using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace BF2Statistics
{
    static class StringExtensions
    {
        /// <summary>
        /// Repeats the current string the number of times specified
        /// </summary>
        /// <param name="input">The string that is being repeated</param>
        /// <param name="count">The number of times to repeat this string</param>
        /// <param name="delimiter">The sequence of one or more characters used to specify the boundary between repeats</param>
        public static string Repeat(this string input, int count = 1, string delimiter = "")
        {
            // Make sure we arent null!
            if (input == null)
                return "";
            else if (count == 0)
                return input;

            // Create a new string builder
            StringBuilder builder = new StringBuilder(input.Length + ((input.Length + delimiter.Length) * count));

            // Do repeats
            builder.Append(input);
            for (int i = 0; i < count; i++)
                builder.Append(delimiter + input);

            return builder.ToString();
        }

        /// <summary>
        /// Escapes this string, so it may be stored inside an XML format
        /// </summary>
        public static string EscapeXML(this string s)
        {
            return !SecurityElement.IsValidText(s) ? SecurityElement.Escape(s) : s;
        }

        /// <summary>
        /// Removes and XML converted formating back into its original value.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Removes any invalid file path characters from this string
        /// </summary>
        public static string MakeFileNameSafe(this string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
    }
}
