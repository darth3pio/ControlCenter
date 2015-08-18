using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BF2Statistics.Net
{
    /// <summary>
    /// This object represents a DNS Cache Entry
    /// </summary>
    public class DnsCacheResult
    {
        /// <summary>
        /// The hostname used to grab the IP Address from the windows DNS cache
        /// </summary>
        public string HostName;

        /// <summary>
        /// If an expected IPAddress was supplied, this property returns
        /// whether the expected IPaddress was found in the list of returned
        /// results from the DNS cache
        /// </summary>
        public bool GotExpectedResult
        {
            get 
            { 
                return (ExpectedAddress == IPAddress.None || IsFaulted) ? false : ResultAddresses.Contains(ExpectedAddress); 
            }
        }

        /// <summary>
        /// Gets the expected IP address if one was supplied in the constructor
        /// </summary>
        public IPAddress ExpectedAddress { get; protected set; }

        /// <summary>
        /// Gets the result IPAddresses found for <see cref="HostName"/> in the DNS Cache
        /// </summary>
        public IPAddress[] ResultAddresses { get; protected set; }

        /// <summary>
        /// Indicates whether there was an error fetching the results of this Dns Result
        /// </summary>
        public bool IsFaulted { get; protected set; }

        /// <summary>
        /// If this Dns query was faulted, then the error is stored here
        /// </summary>
        public Exception Error = null;

        /// <summary>
        /// Creates a new instance of DnsCacheResult
        /// </summary>
        /// <param name="hostname">The hostname to check against the DNS cache</param>
        /// <param name="expecting">
        ///   If expecting a IPAddress from <see cref="hostname"/>, than provide it here
        ///   to unlock the use of <see cref="GotExpectedResult"/>
        /// </param>
        public DnsCacheResult(string hostname, IPAddress expecting = null)
        {
            // Set internals
            HostName = hostname;
            ExpectedAddress = expecting ?? IPAddress.None;
            ResultAddresses = new IPAddress[0];

            // Fetch Results
            try
            {
                ResultAddresses = Dns.GetHostAddresses(hostname);
            }
            catch (Exception e)
            {
                Error = e;
                IsFaulted = true;
            }
        }
    }
}
