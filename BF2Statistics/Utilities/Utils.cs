using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Net;

namespace BF2Statistics
{
    public static class Utils
    {
        /// <summary>
        /// Returns an embedded resource's stream
        /// </summary>
        /// <param name="ResourceName"></param>
        /// <returns></returns>
        public static Stream GetResource(string ResourceName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName);
        }

        /// <summary>
        /// Gets the string contents of an embedded resource
        /// </summary>
        /// <param name="ResourceName"></param>
        /// <returns></returns>
        public static string GetResourceAsString(string ResourceName)
        {
            string Result = "";
            using (Stream ResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName))
                using (StreamReader Reader = new StreamReader(ResourceStream))
                    Result = Reader.ReadToEnd();

            return Result;
        }

        /// <summary>
        /// Gets the lines of a resource file
        /// </summary>
        /// <param name="ResourceName"></param>
        /// <returns></returns>
        public static string[] GetResourceFileLines(string ResourceName)
        {
            List<string> Lines = new List<string>();
            using (Stream ResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName))
                using (StreamReader Reader = new StreamReader(ResourceStream))
                    while(!Reader.EndOfStream)
                        Lines.Add(Reader.ReadLine());

            return Lines.ToArray();
        }

        /// <summary>
        /// Converts a Timespan of seconds into Hours, Minutes, and Seconds
        /// </summary>
        /// <param name="seconds">Seconds to convert</param>
        /// <returns></returns>
        public static string Sec2hms(int seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            StringBuilder SB = new StringBuilder();
            char[] trim = new char[] { ',', ' ' };
            int Hours = t.Hours;

            // If we have more then 24 hours, then we need to
            // convert the days to hours
            if (t.Days > 0)
                Hours += t.Days * 24;

            // Format
            if (Hours > 0)
                SB.AppendFormat("{0} Hours, ", Hours);

            if (t.Minutes > 0)
                SB.AppendFormat("{0} Minutes, ", t.Minutes);

            if (t.Seconds > 0)
                SB.AppendFormat("{0} Seconds, ", t.Seconds);

            return SB.ToString().TrimEnd(trim);
        }
    }
}
