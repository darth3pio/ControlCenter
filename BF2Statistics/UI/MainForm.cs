using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BF2Statistics.ASP;
using BF2Statistics.Database;
using BF2Statistics.Database.QueryBuilder;
using BF2Statistics.Gamespy;
using BF2Statistics.Gamespy.Redirector;
using BF2Statistics.Net;
using BF2Statistics.Properties;
using BF2Statistics.Utilities;
using BF2Statistics.Web;

namespace BF2Statistics
{
    /// <summary>
    /// This form represents the Main GUI window of the application
    /// </summary>
    public partial class MainForm : NativeForm
    {
        /// <summary>
        /// The User Config object
        /// </summary>
        protected Settings Config = Settings.Default;

        /// <summary>
        /// The instance of this form
        /// </summary>
        public static MainForm Instance { get; protected set; }

        /// <summary>
        /// The current selected mod foldername
        /// </summary>
        public static BF2Mod SelectedMod { get; protected set; }

        /// <summary>
        /// Returns the NotifyIcon for on the main form
        /// </summary>
        public static NotifyIcon SysIcon { get; protected set; }

        /// <summary>
        /// The Battlefield 2 Client process (when running)
        /// </summary>
        private Process ClientProcess;

        /// <summary>
        /// Constructor. Initializes and Displays the Applications main GUI
        /// </summary>
        public MainForm()
        {
            // Set form Instance
            Instance = this;

            // Create Form Controls
            InitializeComponent();

            // Make sure the basic configuration settings are setup by the user,
            // and load the BF2 server and installed mods
            if (!SetupManager.Run())
            {
                this.Load += (s, e) => this.Close();
                return;
            }

            // Fill the Mod Select Dropdown with the loaded server mods
            LoadModList();

            // Set BF2Statistics Python Install / Ranked Status
            CheckPythonStatus();

            // Load Cross Session Settings
            ParamBox.Text = Config.ClientParams;
            GlobalServerSettings.Checked = Config.UseGlobalSettings;
            ShowConsole.Checked = Config.ShowServerConsole;
            MinimizeConsole.Checked = Config.MinimizeServerConsole;
            ForceAiBots.Checked = Config.ServerForceAi;
            FileMoniter.Checked = Config.ServerFileMoniter;
            labelTotalWebRequests.Text = Config.TotalASPRequests.ToString();
            HostsLockCheckbox.Checked = Config.LockHostsFile;
            HostsLockCheckbox.CheckedChanged += HostsLockCheckbox_CheckedChanged;

            // If we dont have a client path, disable the Launch Client button
            LaunchClientBtn.Enabled = (!String.IsNullOrWhiteSpace(Config.ClientPath) && File.Exists(Path.Combine(Config.ClientPath, "bf2.exe")));

            // Register for ASP server events
            HttpServer.Started += ASPServer_OnStart;
            HttpServer.Stopped += ASPServer_OnShutdown;
            HttpServer.RequestRecieved += ASPServer_ClientConnected;
            StatsManager.SnapshotProcessed += StatsManager_SnapshotProccessed;
            StatsManager.SnapshotReceived += StatsManager_SnapshotReceived;

            // Register for Gamespy server events
            GamespyEmulator.Started += GamespyServer_OnStart;
            GamespyEmulator.Stopped += GamespyServer_OnShutdown;
            GamespyEmulator.OnClientsUpdate += GamepsyServer_OnUpdate;
            GamespyEmulator.OnServerlistUpdate += (s, e) => BeginInvoke((Action)delegate 
            { 
                ServerListSize.Text = GamespyEmulator.ServerCount.ToString(); 
            });

            // Register for BF2 Server events
            BF2Server.Started += BF2Server_Started;
            BF2Server.Exited += BF2Server_Exited;
            BF2Server.ServerPathChanged += LoadModList;

            // Add administrator title to program title bar if in Admin mode
            if (Program.IsAdministrator)
                this.Text += " (Administrator)";

            // Set some tooltips
            Tipsy.SetToolTip(GamespyStatusPic, "Gamespy server is currently offline");
            Tipsy.SetToolTip(AspStatusPic, "Asp server is currently offline");
            Tipsy.SetToolTip(labelSnapshotsProc, "Processed / Received");
            SysIcon = NotificationIcon;

            // Check for updates last
            ProgramUpdater.CheckCompleted += Updater_CheckCompleted;
            if (Program.Config.UpdateCheck)
            {
                UpdateStatusPic.Show();
                UpdateLabel.Text = "(Checking For Updates...)";
                ProgramUpdater.CheckForUpdateAsync();
            }

            // Once the form is shown, asynchronously load the redirect service
            this.Shown += async (s, e) =>
            {
                // Since we werent registered for Bf2Server events before, do this here
                if (BF2Server.IsRunning)
                    BF2Server_Started();

                // Initialize the Gamespy Redirection Service
                bool AllSystemsGo = await Redirector.Initialize();

                // Check
                CheckRedirectService();
            };
        }

        #region Startup Methods

        /// <summary>
        /// This method sets the Stats Python status of the BF2s python files
        /// on the main GUI.
        /// </summary>
        /// <remarks>
        /// - Initial loading of the StatsPythonConfig object
        /// - Enables / Disables Stats config and Medal editor buttons
        /// - Sets the texts, font colors and status icon image for the stats python elements
        /// </remarks>
        private void CheckPythonStatus()
        {
            // Cross Threaded Crap
            if (StatsPython.Installed)
            {
                InstallBox.ForeColor = Color.Green;
                InstallBox.Text = "BF2 Statistics server files are currently installed.";
                BF2sInstallBtn.Text = "Uninstall BF2 Statistics Python";
                BF2sConfigBtn.Enabled = true;
                BF2sEditMedalDataBtn.Enabled = true;

                // ----------------------------------------------------------
                // Try and access the stats pthon config file
                // This will throw an exception if a config key is missing, or
                // is improperly formated. Missing keys could be a result of an
                // update to the files
                // ----------------------------------------------------------
                try
                {
                    // Updated status based on Ranked Mode Status
                    if (StatsPython.Config.StatsEnabled)
                    {
                        StatsStatusPic.Image = Resources.check;
                        Tipsy.SetToolTip(StatsStatusPic, "BF2 Server Stats are Enabled (Ranked)");
                    }
                    else
                    {
                        StatsStatusPic.Image = Resources.error;
                        Tipsy.SetToolTip(StatsStatusPic, "BF2 Server Stats are Disabled (Non-Ranked)");
                    }
                }
                catch (Exception)
                {
                    StatsStatusPic.Image = Resources.warning;
                    Tipsy.SetToolTip(StatsStatusPic, "Failed to load Stats Python Config");
                }
            }
            else
            {
                InstallBox.ForeColor = Color.Red;
                InstallBox.Text = "BF2 Statistics server files are currently NOT installed";
                BF2sInstallBtn.Text = "Install BF2 Statistics Python";
                BF2sConfigBtn.Enabled = false;
                BF2sEditMedalDataBtn.Enabled = false;
                StatsStatusPic.Image = Resources.error;
                Tipsy.SetToolTip(StatsStatusPic, "BF2 Statistics server files are currently NOT installed");
            }
        }

        /// <summary>
        /// Loads up all the supported mods, and adds them to the Mod select list
        /// </summary>
        private void LoadModList()
        {
            // Clear the list
            ModSelectList.Items.Clear();

            // Add each valid mod to the mod selection list
            foreach (BF2Mod Mod in BF2Server.Mods)
            {
                ModSelectList.Items.Add(Mod);
                if (Mod.Name == "bf2")
                    ModSelectList.SelectedIndex = ModSelectList.Items.Count - 1;
            }

            // Make sure we have a mod selected. This can fail to happen in the bf2 mod folder is changed
            if (ModSelectList.SelectedIndex == -1)
                ModSelectList.SelectedIndex = 0;

            // Add errors to icon
            if (BF2Server.ModLoadErrors.Count > 0)
            {
                ModStatusPic.Visible = true;
                Tipsy.SetToolTip(ModStatusPic, " * " + String.Join(Environment.NewLine.Repeat(1) + " * ", BF2Server.ModLoadErrors), true, 10000);
            }
            else
                ModStatusPic.Visible = false;
        }

        /// <summary>
        /// Fills the Gamespy Redirects tab with the associated
        /// information from the Redirector
        /// </summary>
        private void CheckRedirectService()
        {
            // Main Tab Status pic
            HostsStatusPic.Image = Resources.loading;

            // Set Redirect Mode Text
            HostsSecGroupBox.Enabled = false;
            switch (Redirector.RedirectMethod)
            {
                case RedirectMode.DnsServer: labelRedirectMode.Text = "Dns Server"; break;
                case RedirectMode.HostsIcsFile: labelRedirectMode.Text = "Hosts Ics FIle"; break;
                case RedirectMode.HostsFile:
                    HostsLockStatus.Text = (SysHostsFile.IsLocked) ? "Locked" : "UnLocked";
                    HostsLockStatus.ForeColor = (SysHostsFile.IsLocked) ? Color.Green : Color.Red;
                    HostsSecGroupBox.Enabled = true;
                    labelRedirectMode.Text = "System HOSTS";
                    break;
            }

            // Set Launcher resource status image
            if (Redirector.RedirectsEnabled)
            {
                // Sets the Status box information
                labelRedirectStatus.Text = "Enabled";
                labelRedirectStatus.ForeColor = Color.LimeGreen;
                RedirectButton.Text = (Redirector.RedirectMethod == RedirectMode.DnsServer)
                    ? "ReConfigure Redirects"
                    : "Disable Redirects";

                // Stats Address Boxes
                SSAddress1.Text = "Loading...";
                SSAddress2.Text = "Loading...";
                SStatus.Image = Resources.loading;

                // Gamespy Address Boxes
                GSAddress1.Text = "Loading...";
                GSAddress2.Text = "Loading...";
                GStatus.Image = Resources.loading;

                // Update the cache status
                UpdateCacheStatus();
            }
            else
            {
                // Status Window
                HostsStatusPic.Image = Resources.error;
                labelRedirectStatus.Text = "Disabled";
                labelRedirectStatus.ForeColor = SystemColors.ControlDark;
                RedirectButton.Text = "Configure Gamespy Redirects";

                // Reset Lock Status
                HostsLockStatus.Text = "UnLocked";
                HostsLockStatus.ForeColor = Color.Red;

                // Resets Stats Address Boxes
                SSAddress1.Text = "Disabled";
                SSAddress2.Text = "";
                SStatus.Image = Resources.error;

                // Resets Gamespy Address Boxes
                GSAddress1.Text = "Disabled";
                GSAddress2.Text = "";
                GStatus.Image = Resources.error;
            }

            // Enable button
            DiagnosticsBtn.Enabled = Redirector.RedirectsEnabled;
        }

        /// <summary>
        /// Updates the Cache Address Verification section of the redirects tab with
        /// the latest cache information report
        /// </summary>
        private void UpdateCacheStatus()
        {
            // Stop drawing the gamespy redirect section until we are ready
            groupBox31.SuspendLayout();
            bool GsIsFaulted = false;

            // Resets Stats Address Boxes
            if (Redirector.StatsServerAddress == null)
            {
                SSAddress1.Text = "Disabled";
                SSAddress2.Text = "";
                SStatus.Image = Resources.error;
            }

            // Resets Gamespy Address Boxes
            if (Redirector.GamespyServerAddress == null)
            {
                GSAddress1.Text = "Disabled";
                GSAddress2.Text = "";
                GStatus.Image = Resources.error;
            }

            // Check Stats
            foreach (DnsCacheResult Res in Redirector.DnsCacheReport.Entries.Values)
            {
                // Simplify this. If we got what we wanted, just display the supplied address
                string addy = (Res.GotExpectedResult || Res.IsFaulted) 
                    ? Res.ExpectedAddress.ToString() 
                    : Res.ResultAddresses[0].ToString();

                // Stats Server
                if (Res.HostName == Redirector.Bf2StatsHost)
                {
                    if (Res.IsFaulted)
                    {
                        SSAddress1.Text = "Unable to fetch the stats server address from the Windows DNS Cache";
                        SSAddress2.Text = "";
                        SStatus.Image = Resources.error;
                        continue;
                    }

                    SSAddress1.Text = "Configured Address: " + Res.ExpectedAddress;
                    SSAddress2.Text = "Found Address: " + addy;
                    SStatus.Image = Res.GotExpectedResult ? Resources.check : Resources.warning;
                }
                else // Gamespy Server
                {
                    // Quit if we have faulted
                    if (GsIsFaulted) continue;
                    GsIsFaulted = (Res.GotExpectedResult == false || Res.IsFaulted);

                    if (Res.IsFaulted)
                    {
                        GSAddress1.Text = "Unable to fetch one of the Gamespy server addresses from the Windows DNS Cache";
                        GSAddress2.Text = "";
                        GStatus.Image = Resources.error;
                        continue;
                    }

                    GSAddress1.Text = "Configured Address: " + Res.ExpectedAddress;
                    GSAddress2.Text = "Found Address: " + addy;
                    GStatus.Image = Res.GotExpectedResult ? Resources.check : Resources.warning;
                }
            }

            // Update changes
            groupBox31.ResumeLayout();

            // Update Hosts Status Pic
            if (Redirector.DnsCacheReport.ErrorFree)
            {
                HostsStatusPic.Image = Resources.check;
                Tipsy.SetToolTip(HostsStatusPic, "Gamespy redirects are active and working");
            }
            else
            {
                HostsStatusPic.Image = Resources.warning;
                Tipsy.SetToolTip(HostsStatusPic, "Gamespy redirects are NOT working");
            }
        }

        /// <summary>
        /// Gets a count of processed and unprocessed snapshots
        /// </summary>
        private void CountSnapshots()
        {
            BeginInvoke((Action)delegate
            {
                /// Unprocessed
                TotalUnProcSnapCount.Text = Directory.GetFiles(Paths.SnapshotTempPath).Length.ToString();
                // Processed
                TotalSnapCount.Text = Directory.GetFiles(Paths.SnapshotProcPath).Length.ToString();
            });
        }

        /// <summary>
        /// Builds the Login Tab's client list
        /// </summary>
        private void BuildClientsList()
        {
            StringBuilder Sb = new StringBuilder();
            foreach (GpcmClient C in GamespyEmulator.ConnectedClients)
                Sb.AppendFormat(" {0} ({1}) - {2}{3}", C.PlayerNick, C.PlayerId, C.RemoteEndPoint.Address, Environment.NewLine);

            // Update connected clients count, and list, on main thread
            BeginInvoke((Action)delegate
            {
                ConnectedClients.Clear();
                ConnectedClients.Text = Sb.ToString();
            });
        }

        #endregion Startup Methods

        #region Launcher Tab

        /// <summary>
        /// Event fired when the Launch emulator button is pushed
        /// </summary>
        private async void LaunchEmuBtn_Click(object sender, EventArgs e)
        {
            if (!GamespyEmulator.IsRunning)
            {
                // Make sure the Http web server is running, Cant generate PID's
                // for accounts if the ASP isnt up :P
                if (!HttpServer.IsRunning)
                {
                    DialogResult Res = MessageBox.Show(
                        "The Gamespy Server needs to be able to communicate with the ASP Stats server."
                        + Environment.NewLine.Repeat(1)
                        + "Would you like to start the ASP stats server now?",
                        "Asp Stats Server Required",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    // Start HTTP server if the user requests
                    if (Res == DialogResult.Yes)
                        StartAspServerBtn_Click(sender, e);

                    return;
                }

                // Start the loading process
                ToggleGamespyEmuBtn.Enabled = false;
                ToggleGamespyServerBtn.Enabled = false;
                GamespyStatusPic.Image = Resources.loading;

                // Await Servers Async, Dont want MySQL locking up the GUI
                try
                {
                    await Task.Run(() => GamespyEmulator.Start());
                }
                catch (Exception E)
                {
                    // Exception message will already be there
                    GamespyStatusPic.Image = Resources.warning;
                    ToggleGamespyServerBtn.Enabled = true;
                    ToggleGamespyEmuBtn.Enabled = true;
                    Tipsy.SetToolTip(GamespyStatusPic, E.Message);

                    // Show the DB exception form if its a DB connection error
                    if (E is DbConnectException)
                        ExceptionForm.ShowDbConnectError(E as DbConnectException);
                }
            }
            else
            {
                GamespyEmulator.Shutdown();
                Tipsy.SetToolTip(GamespyStatusPic, "Gamespy server is currently offline.");
            }
        }

        /// <summary>
        /// Client Launcher Button Click
        /// </summary>
        private async void LaunchClientBtn_Click(object sender, EventArgs e)
        {
            // Lock button to prevent spam
            LaunchClientBtn.Enabled = false;

            // Launching
            if (ClientProcess == null)
            {
                // Make sure the Bf2 client supports this mod
                if (!Directory.Exists(Path.Combine(Config.ClientPath, "mods", SelectedMod.Name)))
                {
                    MessageBox.Show("The Battlefield 2 client installation does not have the selected mod installed." +
                        " Please install the mod before launching the BF2 client", "Mod Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    return;
                }

                // Test the ASP stats service here if redirects are enabled. 
                if (Redirector.RedirectsEnabled && Redirector.StatsServerAddress != null)
                {
                    DnsCacheResult Result;

                    // First things first, we fecth the IP address from the DNS cache of our stats server
                    try
                    {
                        // Fetch the address
                        Result = await Networking.ValidateAddressAsync("bf2web.gamespy.com", Redirector.StatsServerAddress);
                        if (!Result.GotExpectedResult)
                        {
                            MessageBox.Show(
                                "Redirect IP address does not match the IP address found by Windows DNS."
                                + Environment.NewLine.Repeat(1)
                                + "Expected: " + Result.ExpectedAddress + "; Found: " + Result.ResultAddresses[0]
                                + Environment.NewLine + "This error can be caused if the HOSTS file cannot be read by windows "
                                + "(Ex: permissions too strict for System)",
                                "Stats Redirect Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );

                            // Unlock Btn
                            LaunchClientBtn.Enabled = true;
                            return;
                        }
                    }
                    catch (Exception Ex)
                    {
                        // ALert user
                        MessageBox.Show(
                            "Failed to obtain an IP address for the ASP stats server from Windows DNS: "
                            + Environment.NewLine.Repeat(1)
                            + Ex.Message 
                            + Environment.NewLine.Repeat(1)
                            + "You may choose to ignore this message and continue, but note that stats may not be working correctly in the BFHQ.",
                            "Stats Redirect Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );

                        // Unlock Btn
                        LaunchClientBtn.Enabled = true;
                        return;
                    }

                    // Loopback for the Retry Button
                    CheckAsp:
                    {
                        try
                        {
                            // Check ASP service
                            await Task.Run(() => StatsManager.ValidateASPService("http://" + Result.ResultAddresses[0]));
                        }
                        catch (Exception Ex)
                        {
                            // ALert user
                            DialogResult Res = MessageBox.Show(
                                "There was an error trying to validate The ASP Stats server defined in your HOSTS file."
                                + Environment.NewLine.Repeat(1)
                                + "Error Message: " + Ex.Message + Environment.NewLine
                                + "Server Address: " + Result.ResultAddresses[0]
                                + Environment.NewLine.Repeat(1)
                                + "You may choose to ignore this message and continue, but note that stats will not be working correctly in the BFHQ.",
                                "Stats Server Verification",
                                MessageBoxButtons.AbortRetryIgnore,
                                MessageBoxIcon.Warning
                            );

                            // User Button Selection
                            if (Res == DialogResult.Retry)
                            {
                                goto CheckAsp;
                            }
                            else if (Res == DialogResult.Abort || Res != DialogResult.Ignore)
                            {
                                // Unlock Btn
                                LaunchClientBtn.Enabled = true;
                                return;
                            }
                        }
                    }
                }

                // Start new BF2 proccess
                ProcessStartInfo Info = new ProcessStartInfo();
                Info.Arguments = String.Format(" +modPath mods/{0} {1}", SelectedMod.Name, ParamBox.Text.Trim());
                Info.FileName = "bf2.exe";
                Info.WorkingDirectory = Config.ClientPath;
                ClientProcess = Process.Start(Info);

                // Hook into the proccess so we know when its running, and register a closing event
                ClientProcess.EnableRaisingEvents = true;
                ClientProcess.Exited += BF2Client_Exited;

                // Update button
                LaunchClientBtn.Enabled = true;
                LaunchClientBtn.Text = "Shutdown Battlefield 2";
            }
            else
            {
                try
                {
                    // prevent button spam
                    LoadingForm.ShowScreen(this);
                    SetNativeEnabled(false);
                    ClientProcess.Kill();
                }
                catch (Exception E)
                {
                    MessageBox.Show("Unable to stop Battlefield 2 client process!"
                        + Environment.NewLine.Repeat(1) + E.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Event fired when Server has exited
        /// </summary>
        private void BF2Client_Exited(object sender, EventArgs e)
        {
            // Make this cross thread safe
            BeginInvoke((Action)delegate
            {
                ClientProcess.Close();
                LaunchClientBtn.Text = "Play Battlefield 2";
                LaunchClientBtn.Enabled = true;
                ClientProcess = null;
                SetNativeEnabled(true);
                LoadingForm.CloseForm();
            });
        }

        /// <summary>
        /// Server Launcher Button Click
        /// </summary>
        private async void LaunchServerBtn_Click(object sender, EventArgs e)
        {
            // Show the loading icon
            ServerStatusPic.Image = Resources.loading;

            // === Starting Server
            if (!BF2Server.IsRunning)
            {
                // We are going to test the ASP stats service here if stats are enabled. 
                // I coded this because it sucks to get ingame and everyone's stats are reset
                // because you forgot to start the stats server, or some kind of error.
                // The BF2 server will continue to load, even if it cant connect
                if (StatsPython.Installed && StatsPython.Config.StatsEnabled)
                {
                    // Loopback for the Retry Button
                    CheckAsp:
                    {
                        try
                        {
                            // Check ASP service
                            await Task.Run(() => StatsManager.ValidateASPService("http://" + StatsPython.Config.AspAddress));
                        }
                        catch (Exception Ex)
                        {
                            // ALert user
                            DialogResult Res = MessageBox.Show(
                                "Unable to connect to the Stats ASP webservice defined in the BF2Statistics config! "
                                + "Please double check your Asp Server settings and check that your ASP server is running."
                                + Environment.NewLine.Repeat(1)
                                + "Server Address: http://" + StatsPython.Config.AspAddress + Environment.NewLine
                                + "Error Message: " + Ex.Message,
                                "Stats Server Connection Failure",
                                MessageBoxButtons.AbortRetryIgnore,
                                MessageBoxIcon.Warning
                            );

                            // User Button Selection
                            if (Res == DialogResult.Retry)
                            {
                                goto CheckAsp;
                            }
                            else if (Res == DialogResult.Abort || Res != DialogResult.Ignore)
                            {
                                // Reset image
                                ServerStatusPic.Image = Resources.error;
                                return;
                            }

                        }
                    }
                }

                // Use the global server settings file?
                string Arguments = "";
                if (GlobalServerSettings.Checked)
                    Arguments += " +config \"" + Path.Combine(Program.RootPath, "Python", "GlobalServerSettings.con") + "\"";

                // Moniter Con Files?
                if (FileMoniter.Checked)
                    Arguments += " +fileMonitor 1";

                // Force AI Bots?
                if (ForceAiBots.Checked)
                    Arguments += " +ai 1";

                // Start the server
                try
                {
                    BF2Server.Start(SelectedMod, Arguments, ShowConsole.Checked, MinimizeConsole.Checked);
                }
                catch (Exception Ex)
                {
                    MessageBox.Show("Unable to start the BF2 server process!"
                        + Environment.NewLine.Repeat(1) + Ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                try
                {
                    // Prevent button spam
                    LoadingForm.ShowScreen(this);
                    SetNativeEnabled(false);
                    BF2Server.Stop();
                }
                catch (Exception E)
                {
                    MessageBox.Show("Unable to stop the BF2 server process!"
                        + Environment.NewLine.Repeat(1) + E.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Event called when the BF2 server has successfully started
        /// </summary>
        private void BF2Server_Started()
        {
            // Make this cross thread safe
            BeginInvoke((Action)delegate
            {
                // Set status to online
                ServerStatusPic.Image = Resources.check;
                LaunchServerBtn.Text = "Shutdown Server";

                // Disable the Restore bf2s python files while server is running
                BF2sRestoreBtn.Enabled = false;
                BF2sInstallBtn.Enabled = false;
            });
        }

        /// <summary>
        /// Event fired when Server has exited
        /// </summary>
        private void BF2Server_Exited()
        {
            // Make this cross thread safe
            BeginInvoke((Action)delegate
            {
                ServerStatusPic.Image = Resources.error;
                LaunchServerBtn.Text = "Launch Server";
                BF2sRestoreBtn.Enabled = true;
                BF2sInstallBtn.Enabled = true;
                SetNativeEnabled(true);
                LoadingForm.CloseForm();
            });
        }

        /// <summary>
        /// Event fired when the selected mod changes. When fired, this method fills in the
        /// "Next Map to be Played" area of the GUI.
        /// </summary>
        private void ModSelectList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reset form texts
            FirstMapBox.Text = MapModeBox.Text = MapSizeBox.Text = "";

            // Set our new selected mod variable
            SelectedMod = ModSelectList.SelectedItem as BF2Mod;

            // Grab the first map to be played from the maplist.con
            MapListEntry Entry = SelectedMod.MapList.Entries.FirstOrDefault();

            // Make sure we dont have an empty MapList file, or a bad Map Name
            if (Entry != default(MapListEntry) && !String.IsNullOrWhiteSpace(Entry.MapName))
            {
                // Fill the Map Name field
                try
                {
                    // Try and load the map descriptor file, so we can fetch the real name of the map
                    FirstMapBox.Text = SelectedMod.LoadMap(Entry.MapName).Title;
                }
                catch
                {
                    // If we cant load the map, lets parse the name the best we can
                    //
                    // First, convert mapname into an array, splitting by the underscore
                    string[] Parts = Entry.MapName.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

                    // Rebuild the map name into a string, uppercasing the first letter of each word
                    StringBuilder MapParts = new StringBuilder();
                    foreach (string value in Parts)
                        MapParts.AppendFormat("{0} ", value.UppercaseFirst());

                    // Set map name
                    FirstMapBox.Text = MapParts.ToString().TrimEnd();
                }

                // Convert gametype
                switch (Entry.GameMode)
                {
                    case "coop":
                        MapModeBox.Text = "Coop";
                        break;
                    case "cq":
                        MapModeBox.Text = "Conquest";
                        break;
                    case "sp1":
                    case "sp2":
                    case "sp3":
                        MapModeBox.Text = "SinglePlayer";
                        break;
                }

                // Set mapsize
                MapSizeBox.Text = Entry.MapSize.ToString();
            }
        }

        /// <summary>
        /// Event fired when the Extra Params button is clicked
        /// </summary>
        private void ExtraParamBtn_Click(object sender, EventArgs e)
        {
            using (ClientParamsForm F = new ClientParamsForm(ParamBox.Text))
            {
                if (F.ShowDialog() == DialogResult.OK)
                    ParamBox.Text = ClientParamsForm.ParamString;
            }
        }

        #endregion Launcher Tab

        #region Login Emulator Tab

        /// <summary>
        /// Event fired when the login server starts
        /// </summary>
        private void GamespyServer_OnStart()
        {
            // Make this cross thread safe
            BeginInvoke((Action)delegate
            {
                GamespyStatusPic.Image = Resources.check;
                ToggleGamespyEmuBtn.Text = "Shutdown Gamespy Server";
                ToggleGamespyEmuBtn.Enabled = true;
                ToggleGamespyServerBtn.Text = "Shutdown Gamespy Server";
                ToggleGamespyServerBtn.Enabled = true;
                ManageGpDbBtn.Enabled = false;
                EditAcctBtn.Enabled = true;
                LoginStatusLabel.Text = "Running";
                LoginStatusLabel.ForeColor = Color.LimeGreen;
                Tipsy.SetToolTip(GamespyStatusPic, "Gamespy server is Running");
            });
        }

        /// <summary>
        /// Event fired when the login emulator shutsdown
        /// </summary>
        private void GamespyServer_OnShutdown()
        {
            // Make this cross thread safe
            BeginInvoke((Action)delegate
            {
                ConnectedClients.Clear();
                GamespyStatusPic.Image = Resources.error;
                ClientCountLabel.Text = "0";
                ToggleGamespyEmuBtn.Text = "Start Gamespy Server";
                ToggleGamespyEmuBtn.Enabled = true;
                ToggleGamespyServerBtn.Text = "Start Gamespy Server";
                ToggleGamespyServerBtn.Enabled = true;
                ManageGpDbBtn.Enabled = true;
                EditAcctBtn.Enabled = false;
                LoginStatusLabel.Text = "Stopped";
                LoginStatusLabel.ForeColor = SystemColors.ControlDark;
                Tipsy.SetToolTip(GamespyStatusPic, "Gamespy server is currently offline");
            });
        }

        /// <summary>
        /// This method updates the connected clients area of the login emulator tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GamepsyServer_OnUpdate(object sender, EventArgs e)
        {
            // DO processing in this thread
            int PeakClients = Int32.Parse(labelPeakClients.Text);
            int Connected = GamespyEmulator.NumClientsConnected;
            if (PeakClients < Connected)
                PeakClients = Connected;

            // Update connected clients count, and list, on main thread
            BeginInvoke((Action)delegate
            {
                ClientCountLabel.Text = Connected.ToString();
                labelPeakClients.Text = PeakClients.ToString();
                if (tabControl1.SelectedIndex == 2 && RefreshChkBox.Checked)
                    BuildClientsList();
            });
        }

        private void StartLoginserverBtn_Click(object sender, EventArgs e)
        {
            LaunchEmuBtn_Click(sender, e);
        }

        private void ManageGpDbBtn_Click(object sender, EventArgs e)
        {
            SetupManager.ShowDatabaseSetupForm(DatabaseMode.Gamespy, this);
        }

        private void EditGamespyConfigBtn_Click(object sender, EventArgs e)
        {
            using (GamespyConfigForm form = new GamespyConfigForm())
            {
                form.ShowDialog();
            }
        }

        private void EditAcctBtn_Click(object sender, EventArgs e)
        {
            using (AccountListForm Form = new AccountListForm())
            {
                Form.ShowDialog();
            }
        }

        private void RefreshChkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (RefreshChkBox.Checked)
                Task.Run(() => { BuildClientsList(); });
        }

        /// <summary>
        /// This event is used to prevent the Connected Login Clients window
        /// from being activatable, giving it the appearence of being disabled
        /// </summary>
        private void ConnectedClients_Enter(object sender, EventArgs e)
        {
            ConnectedClients.Enabled = false;
            ConnectedClients.Enabled = true;
        }

        #endregion Login Emulator Tab

        #region BF2s Config Tab

        /// <summary>
        /// When the Install button is clicked, its checked whether the BF2statisticsConfig.py
        /// file is located in the "python/bf2" directory, and either installs or removes the
        /// bf2statistics python
        /// </summary>
        private async void InstallButton_Click(object sender, EventArgs e)
        {
            // Display the LoadingForm. This is a Modal so the mainform is locked
            LoadingForm.ShowScreen(this, true);
            SetNativeEnabled(false);

            // Install or Remove files
            try
            {
                // Put this in a task incase the HDD is being slow (busy)
                await Task.Run(() =>
                {
                    if (!StatsPython.Installed)
                        StatsPython.BackupAndInstall();
                    else
                        StatsPython.RemoveAndRestore();
                });
            }
            catch (Exception E)
            {
                Program.ErrorLog.Write("ERROR: [BF2sPythonInstall] " + E.Message);
                throw;
            }
            finally
            {
                // Unlock now that we are done
                CheckPythonStatus();
                SetNativeEnabled(true);
                LoadingForm.CloseForm();
            }
        }

        /// <summary>
        /// This button opens up the BF2Statistics config form
        /// </summary>
        private void BF2sConfig_Click(object sender, EventArgs e)
        {
            using (BF2sConfig Form = new BF2sConfig())
            {
                Form.ShowDialog();
                CheckPythonStatus();
            }
        }

        /// <summary>
        /// This button opens up the Medal Data Editor
        /// </summary>
        private void BF2sEditMedalDataBtn_Click(object sender, EventArgs e)
        {
            using (MedalData.MedalDataEditor Form = new MedalData.MedalDataEditor())
            {
                Form.ShowDialog();
            }
        }

        /// <summary>
        /// This button restores the clients Ranked Python files to the original state
        /// </summary>
        private void BF2sRestoreBtn_Click(object sender, EventArgs e)
        {
            // Confirm that the user wants to do this
            if (MessageBox.Show(
                    "Restoring the BF2Statistics python files will erase any and all modifications to the BF2Statistics " +
                    "python files. Are you sure you want to continue?",
                    "Confirmation",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning)
                == DialogResult.OK)
            {
                // Lock the console to prevent errors!
                this.Enabled = false;
                LoadingForm.ShowScreen(this);

                // Replace files with the originals
                try
                {
                    StatsPython.RestoreRankedPyFiles();

                    // Reset medal data profile
                    if (StatsPython.Installed)
                    {
                        StatsPython.Config.MedalDataProfile = "";
                        StatsPython.Config.Save();
                    }

                    // Show Success Message
                    Notify.Show("Stats Python Files Have Been Restored!", "Operation Successful", AlertType.Success);
                }
                catch (Exception E)
                {
                    using (ExceptionForm EForm = new ExceptionForm(E, false))
                    {
                        EForm.Message = "Failed to restore stats python files!";
                        EForm.ShowDialog();
                    }
                }
                finally
                {
                    this.Enabled = true;
                    LoadingForm.CloseForm();
                }
            }
        }

        #endregion BF2s Config Tab

        #region Server Settings Tab

        /// <summary>
        /// Opens the Maplist form
        /// </summary>
        private void EditMapListBtn_Click(object sender, EventArgs e)
        {
            using (MapListForm Form = new MapListForm())
            {
                Form.ShowDialog();
            }

            // Update maplist
            ModSelectList_SelectedIndexChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event fired when the Randomize Maplist button is pushed
        /// </summary>
        private void RandomMapListBtn_Click(object sender, EventArgs e)
        {
            using (RandomizeForm F = new RandomizeForm())
            {
                F.ShowDialog();
            }

            // Update maplist
            ModSelectList_SelectedIndexChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Opens the Edit Server Settings Form
        /// </summary>
        private void EditServerSettingsBtn_Click(object sender, EventArgs e)
        {
            using (ServerSettingsForm SS = new ServerSettingsForm(GlobalServerSettings.Checked))
            {
                SS.ShowDialog();
            }
        }

        /// <summary>
        /// Shows the Edit Score Settings form
        /// </summary>
        private void EditScoreSettingsBtn_Click(object sender, EventArgs e)
        {
            // Show score form
            using (ScoreSettings SS = new ScoreSettings())
            {
                SS.ShowDialog();
            }
        }

        #endregion Server Settings Tab

        #region Hosts File Redirect

        /// <summary>
        /// This is the main HOSTS file button event handler.
        /// </summary>
        private void RedirectButton_Click(object sender, EventArgs e)
        {
            // If we do not have a redirect in the hosts file...
            if (!Redirector.RedirectsEnabled || Redirector.RedirectMethod == RedirectMode.DnsServer)
            {
                using (GamespyRedirectForm F = new GamespyRedirectForm())
                {
                    F.ShowDialog();
                }
            }
            else
            {
                Redirector.RemoveRedirects();
            }

            // Re check the redirects status
            CheckRedirectService();
        }

        /// <summary>
        /// Event fired when the Run Diagnostics button is pushed on the Redirects Tab
        /// </summary>
        private void HostsDiagnosticsBtn_Click(object sender, EventArgs e)
        {
            using (HostsFileTestForm f = new HostsFileTestForm())
            {
                f.ShowDialog();
                UpdateCacheStatus();
            }
        }

        /// <summary>
        /// Event fired when the Lock/Unlock hosts file checkbox is modified
        /// </summary>
        private void HostsLockCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            // Save config
            Config.LockHostsFile = HostsLockCheckbox.Checked;
            Config.Save();

            // We only do something if redirects are enabled
            if (Redirector.RedirectsEnabled)
            {
                // Grab our hosts file
                SysHostsFile hFile = Redirector.HostsFileSys;

                // Lock or unlock the file
                if (HostsLockCheckbox.Checked && !SysHostsFile.IsLocked)
                {
                    // Attempt to lock the hosts file
                    if (!hFile.Lock())
                    {
                        MessageBox.Show(
                            "Unable to lock the HOSTS file! Reason: " + hFile.LastException.Message,
                            "Hosts File Error", MessageBoxButtons.OK, MessageBoxIcon.Error
                        );
                        return;
                    }

                    // Update the GUI
                    HostsLockStatus.Text = "Locked";
                    HostsLockStatus.ForeColor = Color.Green;
                    HostsStatusPic.Image = Resources.check;
                    Tipsy.SetToolTip(HostsStatusPic, "Gamespy redirects are currently active.");
                }
                else if (!HostsLockCheckbox.Checked && SysHostsFile.IsLocked)
                {
                    // Attempt to unlock the hosts file
                    if (!hFile.UnLock())
                    {
                        MessageBox.Show(
                            "Unable to unlock the HOSTS file! Reason: " + hFile.LastException.Message,
                            "Hosts File Error", MessageBoxButtons.OK, MessageBoxIcon.Error
                        );
                        return;
                    }

                    // Update the GUI
                    HostsLockStatus.Text = "UnLocked";
                    HostsLockStatus.ForeColor = Color.Red;
                    HostsStatusPic.Image = Resources.warning;
                    Tipsy.SetToolTip(HostsStatusPic, "HOSTS file is unlocked, Redirects will not work!");
                }
            }

        }

        #endregion Hosts File Redirect

        #region ASP Server

        /// <summary>
        /// Starts and stops the ASP HTTP server
        /// </summary>
        private async void StartAspServerBtn_Click(object sender, EventArgs e)
        {
            if (!HttpServer.IsRunning)
            {
                // Display loading image and disable buttons
                AspStatusPic.Image = Resources.loading;
                ToggleAspServerBtn.Enabled = false;
                ToggleWebServerBtn.Enabled = false;
                ToggleGamespyEmuBtn.Enabled = false;
                ToggleGamespyServerBtn.Enabled = false;

                // Start Server
                try
                {
                    // Start Server in a different thread, Dont want MySQL locking up the GUI
                    await Task.Run(() => HttpServer.Start());
                }
                catch (HttpListenerException E)
                {
                    // Custom port 80 in use message
                    string Message = (E.ErrorCode == 32) ? "Port 80 is already in use by another program." : E.Message;

                    // Warn the user of the exception
                    AspStatusPic.Image = Resources.warning;
                    Tipsy.SetToolTip(AspStatusPic, Message);
                    Notify.Show("Failed to start ASP Server!", Message, AlertType.Warning);
                }
                catch (Exception E)
                {
                    // Check for specific error
                    Program.ErrorLog.Write("[ASP Server] " + E.Message);
                    AspStatusPic.Image = Resources.warning;
                    Tipsy.SetToolTip(AspStatusPic, E.Message);

                    // Show the DB exception form if its a DB connection error
                    if (E is DbConnectException)
                        ExceptionForm.ShowDbConnectError(E as DbConnectException);
                    else
                        Notify.Show("Failed to start ASP Server!", E.Message, AlertType.Warning);
                }
                finally
                {
                    // Enable buttons again
                    ToggleAspServerBtn.Enabled = true;
                    ToggleWebServerBtn.Enabled = true;
                    ToggleGamespyEmuBtn.Enabled = true;
                    ToggleGamespyServerBtn.Enabled = true;
                }
            }
            else
            {
                try
                {
                    HttpServer.Stop();
                    Tipsy.SetToolTip(AspStatusPic, "Asp server is currently offline");
                }
                catch (Exception E)
                {
                    Program.ErrorLog.Write(E.Message);
                }
            }
        }

        /// <summary>
        /// Starts the ASP Webserver
        /// </summary>
        private void StartWebserverBtn_Click(object sender, EventArgs e)
        {
            StartAspServerBtn_Click(sender, e);
        }

        /// <summary>
        /// Update the GUI when the ASP starts up
        /// </summary>
        private void ASPServer_OnStart(object sender, EventArgs E)
        {
            BeginInvoke((Action)delegate
            {
                AspStatusPic.Image = Resources.check;
                ToggleAspServerBtn.Enabled = true;
                ToggleAspServerBtn.Text = "Shutdown ASP Server";
                ViewBf2sCloneBtn.Enabled = true;
                EditPlayerBtn.Enabled = true;
                EditASPDatabaseBtn.Enabled = false;
                ClearStatsBtn.Enabled = true;
                AspStatusLabel.Text = "Running";
                AspStatusLabel.ForeColor = Color.LimeGreen;
                ToggleWebServerBtn.Text = "Stop Webserver";
                ToggleWebServerBtn.Enabled = true;
                Tipsy.SetToolTip(AspStatusPic, "ASP Server is currently running");
            });
        }

        /// <summary>
        /// Update the GUI when the ASP shutsdown
        /// </summary>
        private void ASPServer_OnShutdown(object sender, EventArgs E)
        {
            BeginInvoke((Action)delegate
            {
                AspStatusPic.Image = Resources.error;
                ToggleAspServerBtn.Text = "Start ASP Server";
                ToggleAspServerBtn.Enabled = true;
                ViewBf2sCloneBtn.Enabled = false;
                EditPlayerBtn.Enabled = false;
                EditASPDatabaseBtn.Enabled = true;
                ClearStatsBtn.Enabled = false;
                AspStatusLabel.Text = "Stopped";
                AspStatusLabel.ForeColor = SystemColors.ControlDark;
                ToggleWebServerBtn.Text = "Start Webserver";
                ToggleWebServerBtn.Enabled = true;
                labelSessionWebRequests.Text = "0";
                Tipsy.SetToolTip(AspStatusPic, "ASP Server is currently offline");
            });
        }

        /// <summary>
        /// Update the GUI when a client connects
        /// </summary>
        private void ASPServer_ClientConnected()
        {
            BeginInvoke((Action)delegate
            {
                labelTotalWebRequests.Text = (++Config.TotalASPRequests).ToString();
                labelSessionWebRequests.Text = HttpServer.SessionRequests.ToString();
            });
        }

        /// <summary>
        /// Updates the GUI when a snapshot is proccessed
        /// </summary>
        private void StatsManager_SnapshotProccessed()
        {
            BeginInvoke((Action)delegate
            {
                labelSnapshotsProc.Text = StatsManager.SnapshotsCompleted + " / " + StatsManager.SnapshotsRecieved;
                CountSnapshots();
            });
        }

        /// <summary>
        /// Updates the GUI when a snapshot is recieved successfully
        /// </summary>
        private void StatsManager_SnapshotReceived()
        {
            BeginInvoke((Action)delegate
            {
                labelSnapshotsProc.Text = StatsManager.SnapshotsCompleted + " / " + StatsManager.SnapshotsRecieved;
            });
        }

        /// <summary>
        /// Reset total web requests link click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelReset_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Config.TotalASPRequests = 0;
            labelTotalWebRequests.Text = "0";
            Config.Save();
        }

        /// <summary>
        /// View ASP Access Log Button Click Event
        /// </summary>
        private void ViewAccessLogBtn_Click(object sender, EventArgs e)
        {
            Process.Start(Path.Combine(Program.RootPath, "Logs", "AspAccess.log"));
        }

        /// <summary>
        /// View ASP Error Log Button Click Event
        /// </summary>
        private void ViewErrorLogBtn_Click(object sender, EventArgs e)
        {
            Process.Start(Path.Combine(Program.RootPath, "Logs", "AspServer.log"));
        }

        /// <summary>
        /// View Snapshot Logs Button Click Event
        /// </summary>
        private void ViewSnapshotLogBtn_Click(object sender, EventArgs e)
        {
            // Make sure the log file exists... It doesnt get created on startup like the others
            string fPath = Path.Combine(Program.RootPath, "Logs", "StatsDebug.log");
            if (!File.Exists(fPath))
                File.Create(fPath).Close();

            Process.Start(fPath);
        }

        /// <summary>
        /// View Snapshots Button Click Event
        /// </summary>
        private void ViewSnapshotBtn_Click(object sender, EventArgs e)
        {
            using (SnapshotViewForm Form = new SnapshotViewForm())
            {
                Form.ShowDialog();
                CountSnapshots();
            }
        }

        /// <summary>
        /// Edit Stats Database Settings Button Click Event
        /// </summary>
        private void EditASPDatabaseBtn_Click(object sender, EventArgs e)
        {
            SetupManager.ShowDatabaseSetupForm(DatabaseMode.Stats, this);
        }

        /// <summary>
        /// Edit ASP Settings Button Click Event
        /// </summary>
        private void EditASPSettingsBtn_Click(object sender, EventArgs e)
        {
            using (ASPConfigForm Form = new ASPConfigForm())
            {
                Form.ShowDialog();
            }
        }

        /// <summary>
        /// Edit BF2sClone Config Button Click Event
        /// </summary>
        private void EditBf2sCloneBtn_Click(object sender, EventArgs e)
        {
            using (LeaderboardConfigForm F = new LeaderboardConfigForm())
            {
                F.ShowDialog();
            }
        }

        /// <summary>
        /// View Leaderboard Button Click Event
        /// </summary>
        private void ViewBf2sCloneBtn_Click(object sender, EventArgs e)
        {
            if (!Program.Config.BF2S_Enabled)
            {
                DialogResult Res = MessageBox.Show("The Battlefield 2 Leaderboard is currently disabled! Would you like to enable it now?.",
                    "Disabled Leaderboard", MessageBoxButtons.YesNo, MessageBoxIcon.Question
                );

                if (Res == DialogResult.Yes)
                {
                    Program.Config.BF2S_Enabled = true;
                    Program.Config.Save();
                    Process.Start("http://localhost/bf2stats");
                }

                return;
            }

            Process.Start("http://localhost/bf2stats");
        }

        /// <summary>
        /// Edit Player Button Click Event
        /// </summary>
        private void EditPlayerBtn_Click(object sender, EventArgs e)
        {
            using (PlayerSearchForm Form = new PlayerSearchForm())
            {
                Form.ShowDialog();
            }
        }

        /// <summary>
        /// Clear Stats Database Button Click Event
        /// </summary>
        private void ManageStatsDBBtn_Click(object sender, EventArgs e)
        {
            using (ManageStatsDBForm Form = new ManageStatsDBForm())
            {
                Form.ShowDialog();
            }
        }

        #endregion ASP Server

        #region Status Window OnClick Events

        private void HostsFileStatusLabel_DoubleClick(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 4;
        }

        private void LoginStatusDesc_DoubleClick(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }

        private void StatsStatusDesc_DoubleClick(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private void AspStatusDesc_DoubleClick(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
        }

        private void ServerStatusDesc_DoubleClick(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected tab
            int tab = tabControl1.SelectedIndex;

            // Run in a new task
            Task.Run(() =>
            {
                if (tab == 2 && RefreshChkBox.Checked)
                {
                    BuildClientsList();
                }
                else if (tab == 3)
                {
                    CountSnapshots();
                }
            });
        }

        #endregion Status Window OnClick Events

        #region About Tab

        private void Bf2StatisticsLink_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.bf2statistics.com/");
        }

        /// <summary>
        /// Setup Button Click Event. Relaunches the setup client/server paths screen
        /// </summary>
        private void SetupBtn_Click(object sender, EventArgs e)
        {
            Config.ServerPath = "";
            if (!SetupManager.Run())
            {
                Config.Reload();
            }
        }

        /// <summary>
        /// Open Program Folder Click Event
        /// </summary>
        private void OpenRootBtn_Click(object sender, EventArgs e)
        {
            Process.Start(Program.RootPath);
        }

        /// <summary>
        /// Check for Updates Button
        /// </summary>
        private void ChkUpdateBtn_Click(object sender, EventArgs e)
        {
            // Process.Start("https://github.com/BF2Statistics/ControlCenter/releases/latest");
            ChkUpdateBtn.Enabled = false;
            UpdateStatusPic.Show();
            UpdateLabel.Text = "(Checking For Updates...)";
            UpdateLabel.ForeColor = Color.Black;
            ProgramUpdater.CheckForUpdateAsync();
        }

        /// <summary>
        /// Report Issue or Bug Button
        /// </summary>
        private void ReportBugBtn_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/BF2Statistics/ControlCenter/issues");
        }

        #endregion About Tab

        #region Misc. Form Events

        /// <summary>
        /// Event fired when an update check has finished
        /// </summary>
        private void Updater_CheckCompleted(object sender, EventArgs e)
        {
            if (ProgramUpdater.UpdateAvailable)
            {
                // Update the status
                UpdateLabel.Text = "(Update Available!)";
                UpdateLabel.ForeColor = Color.Green;
                UpdateStatusPic.Hide();

                // Ask User
                DialogResult r = MessageBox.Show(
                    "An Update for this program is avaiable for download (" + ProgramUpdater.NewVersion + ")."
                    + Environment.NewLine.Repeat(1)
                    + "Would you like to download and install this update now?",
                    "Update Available",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

                // Apply update
                if (r == DialogResult.Yes)
                    ProgramUpdater.DownloadUpdateAsync();
            }
            else
            {
                UpdateStatusPic.Hide();
                UpdateLabel.Text = "(Up to Date)";
                UpdateLabel.ForeColor = Color.Black;
            }

            // Re-Enable button
            ChkUpdateBtn.Enabled = true;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Save Cross Session Settings
            Config.ClientParams = ParamBox.Text;
            Config.UseGlobalSettings = GlobalServerSettings.Checked;
            Config.ShowServerConsole = ShowConsole.Checked;
            Config.MinimizeServerConsole = MinimizeConsole.Checked;
            Config.ServerForceAi = ForceAiBots.Checked;
            Config.ServerFileMoniter = FileMoniter.Checked;
            Config.Save();

            // Unlock the hosts file
            if (Redirector.RedirectsEnabled && Redirector.RedirectMethod == RedirectMode.HostsFile)
                Redirector.HostsFileSys.UnLock();

            // Shutdown login servers
            if (GamespyEmulator.IsRunning)
                GamespyEmulator.Shutdown();

            // Shutdown ASP Server
            if (HttpServer.IsRunning)
                HttpServer.Stop();
        }

        #endregion Misc. Form Events
    }
}
