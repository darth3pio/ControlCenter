using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
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
            if (input == null || count == 0)
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
        /// Returns this string, in reverse order (abcde becomes edcba)
        /// </summary>
        public static string Inverse(this string input)
        {
            return new string(input.Reverse().ToArray());
        }

        /// <summary>
        /// Returns all the index's of the given string input
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<int> IndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("The search string must not be empty", "value");

            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        /// <summary>
        /// Returns an Enumeration of this string, split by the specified size
        /// </summary>
        /// <param name="chunkSize">The size of each chunk, in which this string is split by</param>
        /// <returns></returns>
        public static IEnumerable<string> SplitBySize(this string str, int chunkSize)
        {
            if (chunkSize < 1)
                throw new ArgumentException("chunkSize cannot be less then 1");

            return Enumerable.Range(0, (int)Math.Ceiling(str.Length / (double)chunkSize)).Select(i => str.Substring(i * chunkSize, chunkSize));
        }

        public static IEnumerable<string> SplitByLength(this string str, int maxLength)
        {
            int index = 0;
            while (index + maxLength < str.Length)
            {
                yield return str.Substring(index, maxLength);
                index += maxLength;
            }

            yield return str.Substring(index);
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

        /// <summary>
        /// Converts the input string into its MD5 Hex variant
        /// </summary>
        /// <param name="upperCase">Uppercase the characters?</param>
        /// <param name="Encoding">The encoding of the string. Default is UTF8</param>
        /// <returns></returns>
        public static string GetMD5Hash(this string input, bool upperCase = true, Encoding Encoding = null)
        {
            using(MD5 Md5 = MD5.Create())
            {
                if (Encoding == null) Encoding = Encoding.UTF8;
                return Md5.ComputeHash(Encoding.GetBytes(input)).ToHex(upperCase);
            }
        }
    }
}
