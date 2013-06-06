using System;
using System.Text;
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
        /// Gets the string contents of an embedded resource
        /// </summary>
        /// <param name="ResourceName"></param>
        /// <returns></returns>
        public static string GetResourceString(string ResourceName)
        {
            string Result = "";
            using (Stream ResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName))
                using (StreamReader Reader = new StreamReader(ResourceStream))
                    Result = Reader.ReadToEnd();

            return Result;
        }

        /// <summary>
        /// Returns the current UNIX timestamp
        /// </summary>
        /// <returns></returns>
        public static int UnixTimestamp()
        {
            TimeSpan unix_time = (System.DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (int)unix_time.TotalSeconds;
        }

        /// <summary>
        /// Returns whether the IP address specified is a Local Ip Address
        /// </summary>
        /// <param name="host">The ip address to check</param>
        /// <returns></returns>
        public static bool IsLocalIpAddress(string host)
        {
            try
            { // get host IP addresses
                IPAddress[] hostIPs = Dns.GetHostAddresses(host);

                // get local IP addresses
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

                // test if any host IP equals to any local IP or to localhost
                foreach (IPAddress hostIP in hostIPs)
                {
                    // is localhost
                    if (IPAddress.IsLoopback(hostIP)) 
                        return true;

                    // is local address
                    foreach (IPAddress localIP in localIPs)
                        if (hostIP.Equals(localIP)) 
                            return true;
                }
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Converts a long to an IP Address
        /// </summary>
        /// <param name="longIP"></param>
        /// <see cref="http://geekswithblogs.net/rgupta/archive/2009/04/29/convert-ip-to-long-and-vice-versa-c.aspx"/>
        /// <returns></returns>
        static public string LongToIP(long longIP)
        {
            string ip = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                int num = (int)(longIP / Math.Pow(256, (3 - i)));
                longIP = longIP - (long)(num * Math.Pow(256, (3 - i)));
                if (i == 0)
                    ip = num.ToString();
                else
                    ip = ip + "." + num.ToString();
            }
            return ip;
        }

        /// <summary>
        /// Converts a string IP address into MySQL INET_ATOA long
        /// </summary>
        /// <param name="ip">THe IP Address</param>
        /// <see cref="http://geekswithblogs.net/rgupta/archive/2009/04/29/convert-ip-to-long-and-vice-versa-c.aspx"/>
        /// <returns></returns>
        public static long IP2Long(string ip)
        {
            string[] ipBytes;
            double num = 0;
            if (!string.IsNullOrEmpty(ip))
            {
                ipBytes = ip.Split('.');
                for (int i = ipBytes.Length - 1; i >= 0; i--)
                {
                    num += ((int.Parse(ipBytes[i]) % 256) * Math.Pow(256, (3 - i)));
                }
            }
            return (long)num;
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
