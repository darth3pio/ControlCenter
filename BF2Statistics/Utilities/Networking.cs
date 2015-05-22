using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace BF2Statistics.Utilities
{
    static class Networking
    {
        /// <summary>
        /// Takes a domain name, or IP address, and returns the Correct IP address.
        /// If multiple IP addresses are found, the first one is returned
        /// </summary>
        /// <param name="text">Domain name or IP Address</param>
        /// <returns></returns>
        public static IPAddress GetIpAddress(string text)
        {
            // Make sure the IP address is valid!
            IPAddress Address;
            bool isValid = IPAddress.TryParse(text, out Address);

            // If the IP address is invalid, try to parse as domain name
            if (!isValid)
            {
                // Try to get dns value
                IPAddress[] Addresses;
                try {
                    Addresses = Dns.GetHostAddresses(text);
                }
                catch {
                    throw new Exception("Invalid Hostname or IP Address");
                }

                if (Addresses.Length == 0)
                    throw new Exception("Invalid Hostname or IP Address");

                // Return first address
                return Addresses[0];
            }

            return Address;
        }

        /// <summary>
        /// Takes a domain name, or IP address, and returns the Correct IP address.
        /// If multiple IP addresses are found, the first one is returned
        /// </summary>
        /// <param name="text"></param>
        /// <param name="Ip"></param>
        /// <returns>Returns whether the IP or Hostname was valid</returns>
        public static bool TryGetIpAddress(string text, out IPAddress Ip)
        {
            try
            {
                Ip = GetIpAddress(text);
                return true;
            }
            catch
            {
                Ip = null;
                return false;
            }
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
        /// Returns whether the supplied IP Address in on the local Lan network
        /// </summary>
        /// <remarks>http://stackoverflow.com/questions/7232287/check-if-ip-is-in-lan-behind-firewalls-and-routers</remarks>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool IsLanIP(IPAddress address)
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var iface in interfaces)
            {
                var properties = iface.GetIPProperties();
                foreach (var ifAddr in properties.UnicastAddresses)
                {
                    if (ifAddr.IPv4Mask != null &&
                        ifAddr.Address.AddressFamily == AddressFamily.InterNetwork &&
                        CheckMask(ifAddr.Address, ifAddr.IPv4Mask, address))
                        return true;
                }
            }
            return false;
        }

        private static bool CheckMask(IPAddress address, IPAddress mask, IPAddress target)
        {
            if (mask == null)
                return false;

            byte[] ba = address.GetAddressBytes();
            byte[] bm = mask.GetAddressBytes();
            byte[] bb = target.GetAddressBytes();

            if (ba.Length != bm.Length || bm.Length != bb.Length)
                return false;

            for (var i = 0; i < ba.Length; i++)
            {
                int m = bm[i];
                int a = ba[i] & m;
                int b = bb[i] & m;
                if (a != b)
                    return false;
            }

            return true;
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
                    num += ((int.Parse(ipBytes[i]) % 256) * Math.Pow(256, (3 - i)));
            }

            return (long)num;
        }
    }
}
