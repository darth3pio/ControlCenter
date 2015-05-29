using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace BF2Statistics
{
    /// <summary>
    /// Auto updater for the BF2Statistics control center
    /// </summary>
    public class Updater
    {
        /// <summary>
        /// Path to the Versions file
        /// </summary>
        public static readonly Uri Url = new Uri("https://api.github.com/repos/BF2Statistics/ControlCenter/releases");

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
        /// Indicates whether we are currently downloading an update
        /// </summary>
        protected static bool IsDownloading = false;

        /// <summary>
        /// The webclient used to make the requests to github
        /// </summary>
        protected static WebClient Web;

        /// <summary>
        /// Specifies the path to the new update archive on the client PC
        /// </summary>
        public static string UpdateFileLocation { get; protected set; }

        static Updater()
        {
            // By pass SSL Cert checks
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }

        /// <summary>
        /// Checks for a new update Async.
        /// </summary>
        public static async void CheckForUpdateAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    // Use WebClient to download the latest version string
                    using (Web = new WebClient())
                    {
                        // Simulate some headers, Github throws a fit otherwise
                        Web.Headers["User-Agent"] = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.2.6) Gecko/20100625 Firefox/3.6.6 (.NET CLR 3.5.30729)";
                        Web.Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                        Web.Headers["Accept-Language"] = "en-US,en;q=0.8";
                        Web.Proxy = null; // Disable proxy because this can cause slowdown on some machines

                        // Download file
                        string V = Web.DownloadString(Url);

                        // Parse the Json Response
                        var jsonReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(V), new XmlDictionaryReaderQuotas());
                        XDocument root = XDocument.Load(jsonReader);
                        XElement ele = root.Descendants().FirstOrDefault(x => x.Name == "tag_name");

                        // Parse version
                        Version.TryParse(ele.Value, out NewVersion);
                    }
                });
            }
            catch(Exception e)
            {
                Program.ErrorLog.Write("WARNING: Error occured while trying to fetch the new release version: " + e.Message);
                NewVersion = Program.Version;
            }

            // Fire Check Completed Event
            CheckCompleted(NewVersion, EventArgs.Empty);
        }


        /// <summary>
        /// Downloads the new update from Github Async.
        /// </summary>
        public static bool DownloadUpdateAsync()
        {
            // Returns if there is no update
            if (!UpdateAvailable)
                return false;

            // Simulate some headers, Github throws a fit otherwise
            Web = new WebClient();
            Web.Headers["User-Agent"] = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.2.6) Gecko/20100625 Firefox/3.6.6 (.NET CLR 3.5.30729)";
            Web.Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            Web.Headers["Accept-Language"] = "en-US,en;q=0.8";
            Web.Proxy = null; // Disable proxy because this can cause slowdown on some machines

            // Github file location
            string Fp = NewVersion.ToString() + "/BF2Statistics_ControlCenter_" + NewVersion.ToString() + ".zip";
            Uri FileLocation = new Uri("https://github.com/BF2Statistics/ControlCenter/releases/download/" + Fp);

            // Path to the Downloaded file
            UpdateFileLocation = Path.Combine(Paths.DocumentsFolder, "BF2Statistics_ControlCenter_" + NewVersion.ToString() + ".zip");

            // Show Task Form
            IsDownloading = true;
            TaskForm.Cancelled += TaskForm_Cancelled;
            TaskForm.Show(MainForm.Instance, "Downloading Update", "Downloading Update... Please Standby", true, ProgressBarStyle.Blocks, 100);
            TaskForm.UpdateStatus("Preparing the download...");

            try
            {
                // Download the new version Zip file
                Web.DownloadProgressChanged += Wc_DownloadProgressChanged;
                Web.DownloadFileCompleted += Wc_DownloadFileCompleted;
                Web.DownloadFileAsync(FileLocation, UpdateFileLocation);
            }
            catch (Exception ex)
            {
                // Close that task form if its open!
                if (TaskForm.IsOpen)
                    TaskForm.CloseForm();

                // Create Exception Log
                Program.ErrorLog.Write("WARNING: Unable to Download new update archive :: Generating Exception Log");
                ExceptionHandler.GenerateExceptionLog(ex);

                // Alert User
                MessageBox.Show(
                    "Failed to download update archive! Reason: " + ex.Message
                    + Environment.NewLine.Repeat(1)
                    + "An exception log has been generated and created inside the My Documents/BF2Statistics folder.",
                    "Download Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return false;
            }

            // Tell the mainform that we have this handled
            return true;
        }

        /// <summary>
        /// Event called when the Cancel button is pushed on the main form
        /// </summary>
        static void TaskForm_Cancelled(object sender, CancelEventArgs e)
        {
            if (IsDownloading)
            {
                Web.CancelAsync();
            }
        }

        /// <summary>
        /// Event called when the progress of the download has changed
        /// </summary>
        private static void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            TaskForm.UpdateStatus(String.Format("Downloaded {0} of {1}", e.BytesReceived.ToFileSize(), e.TotalBytesToReceive.ToFileSize()));
            TaskForm.SetBarPercentage(e.ProgressPercentage);
        }

        /// <summary>
        /// Event called when an update file has completed its download
        /// </summary>
        private static void Wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // Close task form
            TaskForm.CloseForm();
            IsDownloading = false;

            // Dispose webclient
            Web.Dispose();

            // If we cancelled, stop here
            if (e.Cancelled)
            {
                // Delete junk files
                if (File.Exists(UpdateFileLocation))
                    File.Delete(UpdateFileLocation);

                return;
            }

            // Try to start the isntaller
            try
            {
                // Extract setup.exe
                string exFile = Path.Combine(Paths.DocumentsFolder, "setup.exe");
                using (ZipArchive file = ZipFile.Open(UpdateFileLocation, ZipArchiveMode.Read))
                {
                    ZipArchiveEntry setupFile = file.Entries.FirstOrDefault(x => x.FullName == "setup.exe");
                    if (setupFile != null)
                    {
                        // Extract and start the new update installer
                        setupFile.ExtractToFile(exFile, true);
                        Process installer = Process.Start(exFile);
                        installer.WaitForInputIdle();
                    }
                    else
                    {
                        MessageBox.Show(
                            "The Setup.exe file appears to be missing from the update archive! You will need to manually apply the update.",
                            "Installation Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
            catch(Exception Ex)
            {
                MessageBox.Show(
                    "An Occured while trying to install the new update. You will need to manually apply the update."
                    + Environment.NewLine.Repeat(1) + "Error Message: " + Ex.Message,
                    "Installation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                ExceptionHandler.GenerateExceptionLog(Ex);
            }

            // Exit the application
            Application.Exit();
        }
    }
}
