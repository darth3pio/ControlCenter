using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BF2Statistics.ASP;
using BF2Statistics.Gamespy;
using BF2Statistics.Properties;
using BF2Statistics.Utilities;
using BF2Statistics.Web;

namespace BF2Statistics
{
    /// <summary>
    /// This form represents the Main GUI window of the application
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// The User Config object
        /// </summary>
        public static Settings Config = Settings.Default;

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
        /// Indicates whether the hosts file redirects are active for the gamespy servers
        /// </summary>
        public static bool RedirectsEnabled { get; protected set; }

        /// <summary>
        /// The task object that is uesd to rebuild the DNS cache
        /// </summary>
        private Task HostTask;

        /// <summary>
        /// The cancellation token used to cancel the HostTask if the user
        /// undo's the redirects while the HostTask is still running
        /// </summary>
        private CancellationTokenSource HostTaskSource;

        /// <summary>
        /// Constructor. Initializes and Displays the Applications main GUI
        /// </summary>
        public MainForm()
        {
            // Create Form Controls and Set Instance
            InitializeComponent();
            Instance = this;

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
            SetInstallStatus();

            // Try to access the hosts file when the form is showed
            this.Shown += (s, e) => DoHOSTSCheck();

            // Load Cross Session Settings
            ParamBox.Text = Config.ClientParams;
            GlobalServerSettings.Checked = Config.UseGlobalSettings;
            ShowConsole.Checked = Config.ShowServerConsole;
            MinimizeConsole.Checked = Config.MinimizeServerConsole;
            ForceAiBots.Checked = Config.ServerForceAi;
            FileMoniter.Checked = Config.ServerFileMoniter;
            GpcmAddress.Text = (!String.IsNullOrWhiteSpace(Config.LastLoginServerAddress)) ? Config.LastLoginServerAddress : "localhost";
            Bf2webAddress.Text = (!String.IsNullOrWhiteSpace(Config.LastStatsServerAddress)) ? Config.LastStatsServerAddress : "localhost";
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
                ServerListSize.Text = GamespyEmulator.ServersOnline.ToString(); 
            });

            // Register for BF2 Server events
            BF2Server.Started += BF2Server_Started;
            BF2Server.Exited += BF2Server_Exited;
            BF2Server.ServerPathChanged += LoadModList;

            // Since we werent registered for Bf2Server events before, do this here
            if (BF2Server.IsRunning)
                this.Shown += (s, e) => BF2Server_Started();

            // Add administrator title to program title bar if in Admin mode
            if (Program.IsAdministrator)
                this.Text += " (Administrator)";

            // Set some tooltips
            Tipsy.SetToolTip(GamespyStatusPic, "Gamespy server is currently offline");
            Tipsy.SetToolTip(AspStatusPic, "Asp server is currently offline");
            Tipsy.SetToolTip(labelSnapshotsProc, "Processed / Received");
            SysIcon = NotificationIcon;
        }

        #region Startup Methods

        /// <summary>
        /// This method sets the Install status if the BF2s python files
        /// </summary>
        private void SetInstallStatus()
        {
            // Cross Threaded Crap
            if (StatsPython.Installed)
            {
                InstallBox.ForeColor = Color.Green;
                InstallBox.Text = "BF2 Statistics server files are currently installed.";
                BF2sInstallBtn.Text = "Uninstall BF2 Statistics Python";
                BF2sConfigBtn.Enabled = true;
                BF2sEditMedalDataBtn.Enabled = true;

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
        /// Checks the HOSTS file on startup, detecting existing redirects to the bf2web.gamespy
        /// or gpcm/gpsp.gamespy urls
        /// </summary>
        private void DoHOSTSCheck()
        {
            if (!HostsFile.CanRead)
            {
                HostsStatusPic.Image = Resources.warning;
                Tipsy.SetToolTip(HostsStatusPic, "Unable to read hosts file");
            }
            else if (!HostsFile.CanWrite)
            {
                HostsStatusPic.Image = Resources.warning;
                Tipsy.SetToolTip(HostsStatusPic, "Unable to write to hosts file");
            }
            else
            {
                bool MatchFound = false;

                // Login server redirect
                if (HostsFile.HasEntry("gpcm.gamespy.com"))
                {
                    MatchFound = true;
                    GpcmCheckbox.Checked = true;
                    if (String.IsNullOrWhiteSpace(Config.LastLoginServerAddress))
                        GpcmAddress.Text = HostsFile.Get("gpcm.gamespy.com");
                }

                // Stat server redirect
                if (HostsFile.HasEntry("bf2web.gamespy.com"))
                {
                    MatchFound = true;
                    Bf2webCheckbox.Checked = true;
                    if (String.IsNullOrWhiteSpace(Config.LastStatsServerAddress))
                        Bf2webAddress.Text = HostsFile.Get("bf2web.gamespy.com");
                }

                // Did we find any matches?
                if (MatchFound)
                {
                    Tipsy.SetToolTip(HostsStatusPic, "Gamespy redirects are currently active.");
                    UpdateHostFileStatus("- Found old redirect data in HOSTS file.");
                    RedirectsEnabled = true;
                    HostsStatusPic.Image = Resources.loading;
                    LockGroups();

                    RedirectButton.Enabled = true;
                    RedirectButton.Text = "Remove HOSTS Redirect";

                    // Refresh DNS cache with just the HOSTS file entries
                    RebuildDNSCacheAsync(false);
                }
                else
                    Tipsy.SetToolTip(HostsStatusPic, "Gamespy redirects are not active.");

                // Force checkbox
                //HostsLockCheckbox_CheckedChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets a count of processed and un processed snapshots
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
                LaunchEmuBtn.Enabled = false;
                StartLoginserverBtn.Enabled = false;
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
                    StartLoginserverBtn.Enabled = true;
                    LaunchEmuBtn.Enabled = true;
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
                if (RedirectsEnabled && HostsFile.HasEntry("bf2web.gamespy.com"))
                {
                    IPAddress foundIp = null;

                    // First things first, we fecth the IP address from the DNS cache of our stats server
                    try
                    {
                        // If we are unable to fecth any IP address at all, that means HOSTS file isnt working
                        if (!Networking.TryGetIpAddress("bf2web.gamespy.com", out foundIp))
                            throw new Exception(
                                "Failed to obtain the IP address for the ASP stats server from Windows DNS. Most likely the HOSTS "
                                + "file cannot be read by windows (permissions too strict for system)"
                            );

                        // Hosts file doesnt match whats been found in the cache
                        if (!foundIp.Equals(IPAddress.Parse(HostsFile.Get("bf2web.gamespy.com"))))
                            throw new Exception(
                                "HOSTS file IP address does not match the IP address found by Windows DNS."
                                + Environment.NewLine
                                + "Expected: " + HostsFile.Get("bf2web.gamespy.com") + "; Found: " + foundIp.ToString()
                                + Environment.NewLine + "This error can be caused if the HOSTS file cannot be read by windows "
                                + "(Ex: permissions too strict for System)"
                            );
                    }
                    catch (Exception Ex)
                    {
                        // ALert user
                        MessageBox.Show(
                            Ex.Message + Environment.NewLine.Repeat(1)
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
                            await Task.Run(() => StatsManager.ValidateASPService("http://" + foundIp));
                        }
                        catch (Exception Ex)
                        {
                            // ALert user
                            DialogResult Res = MessageBox.Show(
                                "There was an error trying to validate The ASP Stats server defined in your HOSTS file."
                                + Environment.NewLine.Repeat(1)
                                + "Error Message: " + Ex.Message + Environment.NewLine
                                + "Server Address: " + foundIp
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
                ClientProcess.Exited += new EventHandler(BF2Client_Exited);

                // Update button
                LaunchClientBtn.Enabled = true;
                LaunchClientBtn.Text = "Shutdown Battlefield 2";
            }
            else
            {
                try
                {
                    // prevent button spam
                    this.Enabled = false;
                    LoadingForm.ShowScreen(this);
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
                this.Enabled = true;
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
                    this.Enabled = false;
                    LoadingForm.ShowScreen(this);
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
                this.Enabled = true;
                LoadingForm.CloseForm();
            });
        }

        /// <summary>
        /// Event fired when the selected mod changes
        /// </summary>
        private void ModSelectList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedMod = (BF2Mod)ModSelectList.SelectedItem;
            string Mapname, Type, Size;
            SelectedMod.GetNextMapToBePlayed(out Mapname, out Type, out Size);

            // Make sure we have a next map :S
            if (!String.IsNullOrEmpty(Mapname))
            {
                // First, try and load the map descriptor file
                try
                {
                    FirstMapBox.Text = SelectedMod.LoadMap(Mapname).Title;
                }
                catch
                {
                    // If we cant load the map, lets parse the name the best we can
                    // First, convert mapname into an array, and capitalize each word
                    string[] Parts = Mapname.Split('_');
                    for (int i = 0; i < Parts.Length; i++)
                    {
                        // Ignore empty parts
                        if (String.IsNullOrWhiteSpace(Parts[i]))
                            continue;

                        // Uppercase first letter of ervery word
                        char[] a = Parts[i].ToCharArray();
                        a[0] = char.ToUpper(a[0]);
                        Parts[i] = new String(a);
                    }

                    // Rebuild the map name into a string
                    StringBuilder MapParts = new StringBuilder();
                    foreach (string value in Parts)
                        MapParts.AppendFormat("{0} ", value);

                    // Set map name
                    FirstMapBox.Text = MapParts.ToString();
                }
            }

            // Convert gametype
            switch (Type)
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
            MapSizeBox.Text = Size;
        }

        /// <summary>
        /// Event fired when the Extra Params button is clicked
        /// </summary>
        private void ExtraParamBtn_Click(object sender, EventArgs e)
        {
            ClientParamsForm F = new ClientParamsForm(ParamBox.Text);
            if (F.ShowDialog() == DialogResult.OK)
                ParamBox.Text = ClientParamsForm.ParamString;
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
                LaunchEmuBtn.Text = "Shutdown Gamespy Server";
                LaunchEmuBtn.Enabled = true;
                StartLoginserverBtn.Text = "Shutdown Gamespy Server";
                StartLoginserverBtn.Enabled = true;
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
                LaunchEmuBtn.Text = "Start Gamespy Server";
                LaunchEmuBtn.Enabled = true;
                StartLoginserverBtn.Text = "Start Gamespy Server";
                StartLoginserverBtn.Enabled = true;
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
            int Connected = GamespyEmulator.NumClientsConencted;
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
            SetupManager.ShowDatabaseSetupForm(DatabaseMode.Gamespy);
        }

        private void EditGamespyConfigBtn_Click(object sender, EventArgs e)
        {
            GamespyConfigForm form = new GamespyConfigForm();
            form.ShowDialog();
        }

        private void EditAcctBtn_Click(object sender, EventArgs e)
        {
            AccountListForm Form = new AccountListForm();
            Form.ShowDialog();
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
        private void InstallButton_Click(object sender, EventArgs e)
        {
            // Lock the console to prevent errors!
            this.Enabled = false;
            LoadingForm.ShowScreen(this);

            // Put this in a task incase the HDD is being slow (busy)
            Task.Run(() =>
            {
                try
                {
                    if (!StatsPython.Installed)
                        StatsPython.BackupAndInstall();
                    else
                        StatsPython.RemoveAndRestore();
                }
                catch (Exception E)
                {
                    Program.ErrorLog.Write("ERROR: [BF2sPythonInstall] " + E.Message);
                    throw;
                }
                finally
                {
                    // WE are cross threaded right now, so invoke
                    Invoke((Action)delegate
                    {
                        // Unlock now that we are done
                        SetInstallStatus();
                        this.Enabled = true;
                        LoadingForm.CloseForm();
                    });
                }
            });
        }

        /// <summary>
        /// This button opens up the BF2Statistics config form
        /// </summary>
        private void BF2sConfig_Click(object sender, EventArgs e)
        {
            BF2sConfig Form = new BF2sConfig();
            Form.ShowDialog();
            SetInstallStatus();
        }

        /// <summary>
        /// This button opens up the Medal Data Editor
        /// </summary>
        private void BF2sEditMedalDataBtn_Click(object sender, EventArgs e)
        {
            MedalData.MedalDataEditor Form = new MedalData.MedalDataEditor();
            Form.ShowDialog();
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
                    ExceptionForm EForm = new ExceptionForm(E, false);
                    EForm.Message = "Failed to restore stats python files!";
                    EForm.Show();
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
            MapList Form = new MapList();
            Form.ShowDialog();

            // Update maplist
            ModSelectList_SelectedIndexChanged(this, new EventArgs());
        }

        /// <summary>
        /// Event fired when the Randomize Maplist button is pushed
        /// </summary>
        private void RandomMapListBtn_Click(object sender, EventArgs e)
        {
            RandomizeForm F = new RandomizeForm();
            F.ShowDialog();

            // Update maplist
            ModSelectList_SelectedIndexChanged(this, new EventArgs());
        }

        /// <summary>
        /// Opens the Edit Server Settings Form
        /// </summary>
        private void EditServerSettingsBtn_Click(object sender, EventArgs e)
        {
            try
            {
                ServerSettingsForm SS = new ServerSettingsForm(GlobalServerSettings.Checked);
                SS.ShowDialog();
            }
            catch { }
        }

        /// <summary>
        /// Shows the Edit Score Settings form
        /// </summary>
        private void EditScoreSettingsBtn_Click(object sender, EventArgs e)
        {
            // Show score form
            ScoreSettings SS = new ScoreSettings();
            SS.ShowDialog();
        }

        #endregion Server Settings Tab

        #region Hosts File Redirect

        /// <summary>
        /// This is the main HOSTS file button event handler.
        /// </summary>
        private void RedirectButton_Click(object sender, EventArgs e)
        {
            // Clear the output window
            LogBox.Clear();

            // Show exception message on button push if we cant read or write
            if (!HostsFile.CanRead)
            {
                string message = "Unable to READ the HOST file! Please make sure this program is being ran as an administrator, or "
                    + "modify your HOSTS file permissions, allowing this program to read/modify it.";
                MessageBox.Show(message, "Hosts file Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (!HostsFile.CanWrite)
            {
                string message = "HOSTS file is not WRITABLE! Please make sure this program is being ran as an administrator, or "
                    + "modify your HOSTS file permissions, allowing this program to read/modify it.";
                MessageBox.Show(message, "Hosts file Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


            // If we do not have a redirect in the hosts file...
            else if (!RedirectsEnabled)
            {
                // Make sure we are going to redirect something...
                if (!Bf2webCheckbox.Checked && !GpcmCheckbox.Checked)
                {
                    MessageBox.Show(
                        "Please select at least 1 redirect option",
                        "Select an Option", MessageBoxButtons.OK, MessageBoxIcon.Information
                    );
                    return;
                }

                // Lock button and groupboxes
                LockGroups();

                // First, lets determine what the user wants to redirect
                if (Bf2webCheckbox.Checked)
                {
                    // Make sure we have a valid IP address in the address box!
                    string text = Bf2webAddress.Text.Trim().ToLower();
                    if (text.Length < 8)
                    {
                        MessageBox.Show(
                            "You must enter an IP address or Hostname in the Address box!",
                            "Invalid Address", MessageBoxButtons.OK, MessageBoxIcon.Warning
                        );
                        UnlockGroups();
                        Bf2webAddress.Focus();
                        return;
                    }

                    // Convert Localhost to the Loopback Address
                    IPAddress BF2Web;
                    if (text == "localhost")
                    {
                        BF2Web = IPAddress.Loopback;
                    }
                    else
                    {
                        try
                        {
                            UpdateHostFileStatus("- Resolving Hostname: " + text);
                            BF2Web = Networking.GetIpAddress(text);
                            UpdateHostFileStatus("- Found IP: " + BF2Web);
                        }
                        catch
                        {
                            MessageBox.Show(
                                "Stats server redirect address is invalid, or doesnt exist. Please enter a valid, and existing IPv4/6 or Hostname.",
                                "Invalid Address", MessageBoxButtons.OK, MessageBoxIcon.Warning
                            );

                            UpdateHostFileStatus("- Failed to Resolve Hostname!");
                            UnlockGroups();
                            return;
                        }
                    }

                    // Append line, and update status
                    HostsFile.Set("bf2web.gamespy.com", BF2Web.ToString());
                    Config.LastStatsServerAddress = Bf2webAddress.Text.Trim();
                    UpdateHostFileStatus("- Adding bf2web.gamespy.com redirect to hosts file");
                }

                // First, lets determine what the user wants to redirect
                if (GpcmCheckbox.Checked)
                {
                    // Make sure we have a valid IP address in the address box!
                    string text2 = GpcmAddress.Text.Trim().ToLower();
                    if (text2.Length < 8)
                    {
                        MessageBox.Show(
                            "You must enter an IP address or Hostname in the Address box!",
                            "Invalid Address", MessageBoxButtons.OK, MessageBoxIcon.Warning
                        );
                        UnlockGroups();
                        GpcmAddress.Focus();
                        return;
                    }

                    // Convert Localhost to the Loopback Address
                    IPAddress GpcmA;
                    if (text2 == "localhost")
                    {
                        GpcmA = IPAddress.Loopback;
                    }
                    else
                    {
                        try
                        {
                            UpdateHostFileStatus("- Resolving Hostname: " + text2);
                            GpcmA = Networking.GetIpAddress(text2);
                            UpdateHostFileStatus("- Found IP: " + GpcmA);
                        }
                        catch
                        {
                            MessageBox.Show(
                                "Login Server redirect address is invalid, or doesnt exist. Please enter a valid, and existing IPv4/6 or Hostname.",
                                "Invalid Address", MessageBoxButtons.OK, MessageBoxIcon.Warning
                            );

                            UpdateHostFileStatus("- Failed to Resolve Hostname!");
                            UnlockGroups();
                            return;
                        }
                    }

                    // Update status
                    UpdateHostFileStatus("- Adding gamespy redirects to hosts file");

                    // Append lines to hosts file
                    HostsFile.Set("gpcm.gamespy.com", GpcmA.ToString());
                    HostsFile.Set("gpsp.gamespy.com", GpcmA.ToString());
                    HostsFile.Set("motd.gamespy.com", GpcmA.ToString());
                    HostsFile.Set("master.gamespy.com", GpcmA.ToString());
                    HostsFile.Set("gamestats.gamespy.com", GpcmA.ToString());
                    HostsFile.Set("battlefield2.ms14.gamespy.com", GpcmA.ToString());
                    HostsFile.Set("battlefield2.master.gamespy.com", GpcmA.ToString());
                    HostsFile.Set("battlefield2.available.gamespy.com", GpcmA.ToString());
                    Config.LastLoginServerAddress = GpcmAddress.Text.Trim();
                }

                // Save last used addresses
                Config.Save();

                // Write the lines to the hosts file
                UpdateHostFileStatus("- Writting to hosts file... ", false);
                try
                {
                    // Save lines to hosts file
                    HostsFile.Save();
                    UpdateHostFileStatus("Success!");

                    // Set form data
                    RedirectsEnabled = true;
                    HostsStatusPic.Image = Resources.loading;
                    RedirectButton.Text = "Remove HOSTS Redirect";
                    RedirectButton.Enabled = true;

                    // Rebuilds the DNS cache async
                    RebuildDNSCacheAsync();
                }
                catch
                {
                    UpdateHostFileStatus("Failed!");
                    UnlockGroups();
                    MessageBox.Show(
                        "Unable to WRITE to HOSTS file! Please make sure to replace your HOSTS file with " +
                        "the one provided in the release package, or remove your current permissions from the HOSTS file. " +
                        "It may also help to run this program as an administrator.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );
                    HostsLockCheckbox.Enabled = true;
                }
            }
            else
            {
                // Lock the button
                RedirectButton.Enabled = true;

                // Stop worker if its busy
                if (HostTask != null && HostTask.Status == TaskStatus.Running)
                {
                    HostTaskSource.Cancel();
                    return;
                }

                UndoRedirects();
            }
        }

        private void HostsDiagnosticsBtn_Click(object sender, EventArgs e)
        {
            HostsFileTestForm f = new HostsFileTestForm();
            f.ShowDialog();
        }

        private void HostsLockCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            // Save config
            Config.LockHostsFile = HostsLockCheckbox.Checked;
            Config.Save();

            // We only do something if redirects are enabled
            if (RedirectsEnabled)
            {
                // Lock or unlock the file
                if (HostsLockCheckbox.Checked && !HostsFile.IsLocked)
                {
                    // Attempt to lock the hosts file
                    if (!HostsFile.Lock())
                    {
                        MessageBox.Show(
                            "Unable to lock the HOSTS file! Reason: " + HostsFile.LastException.Message,
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
                else if (!HostsLockCheckbox.Checked && HostsFile.IsLocked)
                {
                    // Attempt to unlock the hosts file
                    if (!HostsFile.UnLock())
                    {
                        MessageBox.Show(
                            "Unable to unlock the HOSTS file! Reason: " + HostsFile.LastException.Message,
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

        /// <summary>
        /// Method is used to unlock the input fields
        /// </summary>
        private void UnlockGroups()
        {
            RedirectButton.Enabled = true;
            GpcmGroupBox.Enabled = true;
            BF2webGroupBox.Enabled = true;
        }

        /// <summary>
        /// Method is used to lock the input fields while redirect is active
        /// </summary>
        private void LockGroups()
        {
            RedirectButton.Enabled = false;
            GpcmGroupBox.Enabled = false;
            BF2webGroupBox.Enabled = false;
        }

        /// <summary>
        /// Preforms the pings required to fill the dns cache, and locks the HOSTS file.
        /// The reason we ping, is because once the HOSTS file is locked, any request
        /// made to a url (when the DNS cache is empty), will skip the hosts file, because 
        /// it cant be read. If we ping first, then the DNS cache fills up with the IP 
        /// addresses in the hosts file.
        /// </summary>
        private void RebuildDNSCacheAsync(bool FlushDnsCache = true)
        {
            // Create a new Cancellation token sequence
            HostTaskSource = new CancellationTokenSource();
            CancellationToken CancelToken = HostTaskSource.Token;

            // Lock hosts file lock
            HostsLockCheckbox.Enabled = false;

            // Run as a task in a different thread
            HostTask = Task.Run(async () =>
            {
                try
                {
                    if (FlushDnsCache)
                    {
                        // Flush DNS Cache
                        UpdateHostFileStatus("- Rebuilding DNS Cache... ", false);
                        DnsFlushResolverCache();
                    }
                    else
                    {
                        UpdateHostFileStatus("- Refreshing DNS HOSTS File Entries... ", false);
                    }

                    // Dispose of the ping object correctly
                        // Rebuild the DNS cache with the hosts file redirects
                        foreach (KeyValuePair<String, String> IP in HostsFile.GetLines())
                        {
                            // Quit on cancel
                            if (CancelToken.IsCancellationRequested)
                                return;

                            try
                            {
                                // Ping server to get the IP address in the dns cache
                                await Dns.GetHostAddressesAsync(IP.Key);
                            }
                            catch
                            {
                                continue;
                            }
                        }

                    // Update status window
                    UpdateHostFileStatus("Done");

                    // Wait for DNS cache to catch up
                    await Task.Delay(1000);

                    // Lock the hosts file
                    if (HostsLockCheckbox.Checked)
                    {
                        UpdateHostFileStatus("- Locking HOSTS file");
                        HostsFile.Lock();
                    }

                    UpdateHostFileStatus("- All Done!");
                    Tipsy.SetToolTip(HostsStatusPic, "Gamespy redirects are currently active.");
                    BeginInvoke((Action)delegate
                    {
                        HostsStatusPic.Image = Resources.check;
                        HostsDiagnosticsBtn.Enabled = true;

                        // Set hosts file locked status
                        if(HostsFile.IsLocked)
                        {
                            HostsLockStatus.Text = "Locked";
                            HostsLockStatus.ForeColor = Color.Green;
                        }
                        else
                        {
                            HostsLockStatus.Text = "UnLocked";
                            HostsLockStatus.ForeColor = Color.Red;
                        }
                        HostsLockCheckbox.Enabled = true;
                    });
                }
                catch(Exception e)
                {
                    // Execute on the main thread
                    BeginInvoke((Action)delegate
                    {
                        // Update status window
                        UpdateHostFileStatus("Error!");
                        HostsStatusPic.Image = Resources.warning;
                        HostsLockCheckbox.Enabled = true;
                        Tipsy.SetToolTip(HostsStatusPic, e.Message);
                    });
                }

            }, CancelToken)

            // Runs if cancelled
            .ContinueWith((Action<Task>)delegate
            {
                // Execute on the main thread
                BeginInvoke((Action)delegate
                {
                    // Update status window
                    UpdateHostFileStatus("Cancelled!");

                    // Lock form and show loading screen
                    LoadingForm.ShowScreen(this);
                    this.Enabled = false;

                    // Undo the redirects
                    UndoRedirects();

                    // Close loading form and allow user to access form again
                    LoadingForm.CloseForm();
                    this.Enabled = true;
                });
            }, TaskContinuationOptions.OnlyOnCanceled);
        }

        /// <summary>
        /// Removes HOSTS file redirects.
        /// </summary>
        private void UndoRedirects()
        {
            // Tell the writter to restore the HOSTS file to its
            // original state
            UpdateHostFileStatus("- Unlocking HOSTS file");
            HostsFile.UnLock();

            // Restore the original hosts file contents
            UpdateHostFileStatus("- Restoring HOSTS file... ", false);
            try
            {
                HostsFile.Remove("bf2web.gamespy.com");
                HostsFile.Remove("gpcm.gamespy.com");
                HostsFile.Remove("gpsp.gamespy.com");
                HostsFile.Remove("motd.gamespy.com");
                HostsFile.Remove("master.gamespy.com");
                HostsFile.Remove("gamestats.gamespy.com");
                HostsFile.Remove("battlefield2.ms14.gamespy.com");
                HostsFile.Remove("battlefield2.master.gamespy.com");
                HostsFile.Remove("battlefield2.available.gamespy.com");
                HostsFile.Save();
                UpdateHostFileStatus("Success!");
            }
            catch
            {
                UpdateHostFileStatus("Failed!");
                MessageBox.Show(
                    "Unable to RESTORE to HOSTS file! Unfortunatly this error can only be fixed by manually removing the HOSTS file,"
                    + " and replacing it with a new one :( . If possible, you may also try changing the permissions yourself.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error
                );
            }

            // Flush the DNS!
            UpdateHostFileStatus("- Flushing DNS Cache");
            DnsFlushResolverCache();

            // Update status
            UpdateHostFileStatus("- All Done!");
            Tipsy.SetToolTip(HostsStatusPic, "Gamespy redirects are NOT active.");

            // Reset form data
            RedirectsEnabled = false;
            HostsStatusPic.Image = Resources.error;
            RedirectButton.Text = "Begin HOSTS Redirect";
            HostsDiagnosticsBtn.Enabled = false;
            HostsLockStatus.Text = "UnLocked";
            HostsLockStatus.ForeColor = Color.Red;
            UnlockGroups();
        }

        /// <summary>
        /// Adds a new line to the "status" window on the GUI
        /// </summary>
        /// <param name="message">The message to print</param>
        /// <param name="newLine">Add a new line for the next message?</param>
        public void UpdateHostFileStatus(string message, bool newLine = true)
        {
            // Add new line
            if (newLine) message = message + Environment.NewLine;

            // Ask if we need invoke to prevent an exception at startup 
            // because window handle wasnt created yet.
            if (InvokeRequired)
            {
                // Invoke the logbox update
                Invoke((MethodInvoker)delegate
                {
                    LogBox.Text += message;
                    LogBox.Refresh();
                });
            }
            else
            {
                LogBox.Text += message;
                LogBox.Refresh();
            }
        }

        [DllImport("dnsapi.dll", EntryPoint = "DnsFlushResolverCache")]
        private static extern UInt32 DnsFlushResolverCache();

        #endregion Hosts File Redirect

        #region ASP Server

        /// <summary>
        /// Starts and stops the ASP HTTP server
        /// </summary>
        private async void StartAspServerBtn_Click(object sender, EventArgs e)
        {
            if (!HttpServer.IsRunning)
            {
                AspStatusPic.Image = Resources.loading;
                StartAspServerBtn.Enabled = false;
                StartWebserverBtn.Enabled = false;

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

                    // Enable buttons again, warn user
                    StartAspServerBtn.Enabled = true;
                    StartWebserverBtn.Enabled = true;
                    AspStatusPic.Image = Resources.warning;
                    Tipsy.SetToolTip(AspStatusPic, Message);
                    Notify.Show("Failed to start ASP Server!", Message, AlertType.Warning);
                }
                catch (Exception E)
                {
                    // Check for specific error
                    Program.ErrorLog.Write("[ASP Server] " + E.Message);
                    StartAspServerBtn.Enabled = true;
                    StartWebserverBtn.Enabled = true;
                    AspStatusPic.Image = Resources.warning;
                    Tipsy.SetToolTip(AspStatusPic, E.Message);

                    // Show the DB exception form if its a DB connection error
                    if (E is DbConnectException)
                        ExceptionForm.ShowDbConnectError(E as DbConnectException);
                    else
                        Notify.Show("Failed to start ASP Server!", E.Message, AlertType.Warning);
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
                StartAspServerBtn.Enabled = true;
                StartAspServerBtn.Text = "Shutdown ASP Server";
                ViewBf2sCloneBtn.Enabled = true;
                EditPlayerBtn.Enabled = true;
                EditASPDatabaseBtn.Enabled = false;
                ClearStatsBtn.Enabled = true;
                AspStatusLabel.Text = "Running";
                AspStatusLabel.ForeColor = Color.LimeGreen;
                StartWebserverBtn.Text = "Stop Webserver";
                StartWebserverBtn.Enabled = true;
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
                StartAspServerBtn.Text = "Start ASP Server";
                StartAspServerBtn.Enabled = true;
                ViewBf2sCloneBtn.Enabled = false;
                EditPlayerBtn.Enabled = false;
                EditASPDatabaseBtn.Enabled = true;
                ClearStatsBtn.Enabled = false;
                AspStatusLabel.Text = "Stopped";
                AspStatusLabel.ForeColor = SystemColors.ControlDark;
                StartWebserverBtn.Text = "Start Webserver";
                StartWebserverBtn.Enabled = true;
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
            SnapshotViewForm Form = new SnapshotViewForm();
            Form.ShowDialog();
            CountSnapshots();
        }

        /// <summary>
        /// Edit Stats Database Settings Button Click Event
        /// </summary>
        private void EditASPDatabaseBtn_Click(object sender, EventArgs e)
        {
            SetupManager.ShowDatabaseSetupForm(DatabaseMode.Stats);
        }

        /// <summary>
        /// Edit ASP Settings Button Click Event
        /// </summary>
        private void EditASPSettingsBtn_Click(object sender, EventArgs e)
        {
            ASPConfigForm Form = new ASPConfigForm();
            Form.ShowDialog();
        }

        /// <summary>
        /// Edit BF2sClone Config Button Click Event
        /// </summary>
        private void EditBf2sCloneBtn_Click(object sender, EventArgs e)
        {
            LeaderboardConfigForm F = new LeaderboardConfigForm();
            F.ShowDialog();
        }

        /// <summary>
        /// View Leaderboard Button Click Event
        /// </summary>
        private void ViewBf2sCloneBtn_Click(object sender, EventArgs e)
        {
            if (!MainForm.Config.BF2S_Enabled)
            {
                DialogResult Res = MessageBox.Show("The Battlefield 2 Leaderboard is currently disabled! Would you like to enable it now?.",
                    "Disabled Leaderboard", MessageBoxButtons.YesNo, MessageBoxIcon.Question
                );

                if (Res == DialogResult.Yes)
                {
                    MainForm.Config.BF2S_Enabled = true;
                    MainForm.Config.Save();
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
            PlayerSearchForm Form = new PlayerSearchForm();
            Form.ShowDialog();
        }

        /// <summary>
        /// Clear Stats Database Button Click Event
        /// </summary>
        private void ManageStatsDBBtn_Click(object sender, EventArgs e)
        {
            ManageStatsDBForm Form = new ManageStatsDBForm();
            Form.ShowDialog();
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
            Process.Start("https://github.com/BF2Statistics/ControlCenter/releases/latest");
        }

        /// <summary>
        /// Report Issue or Bug Button
        /// </summary>
        private void ReportBugBtn_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/BF2Statistics/ControlCenter/issues");
        }

        #endregion About Tab

        #region Closer Methods

        /// <summary>
        /// Destructor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            // Shutdown login servers
            if (GamespyEmulator.IsRunning)
                GamespyEmulator.Shutdown();

            // Shutdown ASP Server
            if (HttpServer.IsRunning)
                HttpServer.Stop();

            // Unlock the hosts file
            HostsFile.UnLock();
        }

        #endregion Closer Methods
    }
}
