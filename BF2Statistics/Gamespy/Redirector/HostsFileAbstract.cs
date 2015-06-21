using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BF2Statistics
{
    public abstract class HostsFileAbstract
    {
        /// <summary>
        /// The fileinfo object for the HostsFile
        /// </summary>
        protected FileInfo HostFile;

        /// <summary>
        /// Each line of the hosts file stored in a list. ALl redirects are removed
        /// from this list before being stored here.
        /// </summary>
        public List<string> OrigContents = new List<string>();

        /// <summary>
        /// A list of "hostname" => "IPAddress" in the hosts file.
        /// </summary>
        protected Dictionary<string, IPAddress> Entries = new Dictionary<string, IPAddress>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Hosts file security object
        /// </summary>
        protected FileSecurity Security;

        /// <summary>
        /// The windows permission that represents everyone
        /// </summary>
        protected static SecurityIdentifier WorldSid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

        /// <summary>
        /// Returns whether the HOSTS file can be read from
        /// </summary>
        public bool CanRead { get; protected set; }

        /// <summary>
        /// Returns whether the HOSTS file can be written to
        /// </summary>
        public bool CanWrite { get; protected set; }

        public HostsFileAbstract()
        {
            CanRead = CanWrite = false;
        }

        /// <summary>
        /// Sets a domain name with an IP in the hosts file
        /// </summary>
        /// <param name="Domain">The domain name</param>
        /// <param name="Ip">The IP address</param>
        public void Set(string Domain, IPAddress Ip)
        {
            if (Entries.ContainsKey(Domain))
                Entries[Domain] = Ip;
            else
                Entries.Add(Domain, Ip);
        }

        /// <summary>
        /// Removes a domain name from the hosts file
        /// </summary>
        /// <param name="Domain">The domain name</param>
        public void Remove(string Domain)
        {
            if (Entries.ContainsKey(Domain))
                Entries.Remove(Domain);
        }

        /// <summary>
        /// Returns whether the hostsfile contains a domain name
        /// </summary>
        /// <param name="Domain">The domain name</param>
        /// <returns></returns>
        public bool HasEntry(string Domain)
        {
            return Entries.ContainsKey(Domain);
        }

        /// <summary>
        /// Returns the IP address for the provided domain name
        /// </summary>
        /// <param name="Domain">The domain name</param>
        /// <returns></returns>
        public IPAddress Get(string Domain)
        {
            return Entries[Domain];
        }

        /// <summary>
        /// Saves all currently set domains and IPs to the hosts file
        /// </summary>
        public void Save()
        {
            using (FileStream Str = HostFile.Open(FileMode.Truncate, FileAccess.Write, FileShare.Read))
            using (StreamWriter Writer = new StreamWriter(Str))
            {
                foreach (KeyValuePair<String, IPAddress> line in Entries)
                    Writer.WriteLine(String.Format("{0}\t{1}", line.Value, line.Key));
            }
        }

        /// <summary>
        /// Returns a list of all current hosts file lines
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, IPAddress> GetLines()
        {
            return Entries;
        }

        protected void ParseEntries(string[] FileLines)
        {
            // Parse hosts file lines
            foreach (string line in FileLines)
            {
                // Dont add empty lines or comments
                string cLine = line.Trim();
                if (String.IsNullOrEmpty(cLine) || cLine[0] == '#')
                    continue;

                // Add line if we have a valid address and hostname
                Match M = Regex.Match(
                    cLine,
                    @"^([\s|\t]+)?(?<address>[a-z0-9\.:]+)[\s|\t]+(?<hostname>[a-z0-9\.\-_\s]+)$",
                    RegexOptions.IgnoreCase
                );

                // Add line
                if (M.Success)
                {
                    string hostname = M.Groups["hostname"].Value.ToLower();
                    if (!Entries.ContainsKey(hostname))
                    {
                        IPAddress addy = null;
                        if (IPAddress.TryParse(M.Groups["address"].Value, out addy))
                            Entries.Add(hostname, addy);
                    }
                }
            }

            // Make sure we have a localhost loopback! Save aswell, so its available for future
            if (!Entries.ContainsKey("localhost"))
            {
                OrigContents.Add("127.0.0.1\tlocalhost");
                Entries.Add("localhost", IPAddress.Loopback);
            }
        }
    }
}
