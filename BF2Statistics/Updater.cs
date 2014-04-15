using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.ComponentModel;
using System.Threading;

namespace BF2Statistics
{
    /// <summary>
    /// Auto updater for the BF2Statistics control center
    /// <remarks>.NET is gay when it comes to HTTPS connections, This class DOES NOT work at the time being.</remarks>
    /// </summary>
    public class Updater
    {
        /// <summary>
        /// Path to the Versions file
        /// </summary>
        public static readonly Uri Url = new Uri("https://raw.githubusercontent.com/BF2Statistics/ControlCenter/master/Version.txt");

        /// <summary>
        /// The new updated version
        /// </summary>
        public static Version NewVersion;

        /// <summary>
        /// Indicates whether there is an update avaiable for download
        /// </summary>
        public static bool UpdateAvailable 
        {
            get
            {
                if (NewVersion == null)
                    return false;

                return Program.Version.CompareTo(NewVersion) < 0;
            }
        }

        /// <summary>
        /// Event fired when the update check has completed
        /// </summary>
        public static event EventHandler CheckCompleted;

        /// <summary>
        /// Event fired when the download progress of the new update changes
        /// </summary>
        public static event DownloadProgressChangedEventHandler DownloadProgressChanged;

        /// <summary>
        /// Event fired when the new update has finished downloading
        /// </summary>
        public static event AsyncCompletedEventHandler DownloadFinished;

        /// <summary>
        /// Checks for a new update Async.
        /// </summary>
        public static void CheckForUpdateAsync()
        {
            if (NewVersion == null)
            {
                DoCheckUpdates(null);
            }
        }

        /// <summary>
        /// Downloads the new update from Github Async.
        /// </summary>
        public static void DownloadUpdateAsync()
        {
            if (!UpdateAvailable)
                return;

            using (WebClient Wc = new WebClient())
            {
                // Github file location
                string Fp = NewVersion.ToString() + "/BF2Statistics_ControlCenter_" + NewVersion.ToString() + ".zip";
                Uri FileLocation = new Uri("https://github.com/BF2Statistics/ControlCenter/releases/download/" + Fp);

                // Path to the users Downloads folder
                string Dest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                if (!Directory.Exists(Dest))
                    Directory.CreateDirectory(Dest);
                Dest = Path.Combine(Dest, "BF2Statistics_ControlCenter_" + NewVersion.ToString() + ".zip");

                // Download the new version Zip file
                Wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Wc_DownloadProgressChanged);
                Wc.DownloadFileCompleted += new AsyncCompletedEventHandler(Wc_DownloadFileCompleted);
                Wc.DownloadFileAsync(FileLocation, Dest);
            }
        }

        /// <summary>
        /// Method that actually performs the update check
        /// </summary>
        /// <param name="State"></param>
        protected static void DoCheckUpdates(Object State)
        {
            using (WebClient Wc = new WebClient())
            {
                try
                {
                    // Simulate some headers
                    Wc.Headers["User-Agent"] = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.2.6) Gecko/20100625 Firefox/3.6.6 (.NET CLR 3.5.30729)";
                    Wc.Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                    Wc.Headers["Accept-Language"] = "en-US,en;q=0.8";
                    Wc.Headers["Accept-Encoding"] = "gzip,deflate,sdch";

                    // By pass SSL Cert checks
                    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    // Add basic Auth header
                    //Wc.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes("username:pass")));

                    // Download file
                    string V = Wc.DownloadString(Url);
                    Version.TryParse(V, out NewVersion);
                }
                catch (WebException e)
                {

                }
            }

            // Fire Check Completed Event
            CheckCompleted(NewVersion, new EventArgs());
        }

        private static void Wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (DownloadFinished != null)
                DownloadFinished(sender, e);
        }

        private static void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (DownloadProgressChanged != null)
                DownloadProgressChanged(sender, e);
        }
    }
}
