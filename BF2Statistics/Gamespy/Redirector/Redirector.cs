using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BF2Statistics
{
    class Redirector
    {
        private static bool IsInitialized = false;

        public static bool RedirectsEnabled { get; protected set; }

        public static RedirectMode RedirectMethod { get; protected set; }


        protected static HostsFile HostsFile;

        public static IPAddress StatsServerAddress { get; protected set; }

        public static IPAddress GamespyServerAddres { get; protected set; }


        /// <summary>
        /// Returns an array of all gamespy related service hostnames
        /// </summary>
        public static readonly string[] GamespyHosts = {
                "gpcm.gamespy.com",
                "gpsp.gamespy.com",
                "motd.gamespy.com",
                "master.gamespy.com",
                "gamestats.gamespy.com",
                "battlefield2.ms14.gamespy.com",
                "battlefield2.master.gamespy.com",
                "battlefield2.available.gamespy.com"
            };



        public void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;

            
        }

        public static bool SetRedirectMode(RedirectMode Mode)
        {
            // If not change is made, return
            if (Mode == RedirectMethod) return false;

            // Remove old redirects first
            RemoveRedirects();

            // Set new method
            RedirectMethod = Mode;

            return true;
        }

        public static void ApplyRedirects(IPAddress StatsServer, IPAddress GamespyServer)
        {

        }

        public static void RemoveRedirects()
        {

        }

        public static Task RebuildDNSCacheAsync()
        {
            return Task.Run(() => { });
        }

        [DllImport("dnsapi.dll", EntryPoint = "DnsFlushResolverCache")]
        private static extern UInt32 DnsFlushResolverCache();
    }
}
