using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using BF2Statistics.Properties;
using BF2Statistics.ASP;
using BF2Statistics.Gamespy;
using BF2Statistics.Logging;
using BF2Statistics.Utilities;

namespace BF2Statistics
{
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
        /// Returns a bool stating whether the stats enabled python files are installed
        /// </summary>
        public static bool StatsEnabled { get; protected set; }

        /// <summary>
        /// Returns the NotifyIcon for on the main form
        /// </summary>
        public static NotifyIcon SysIcon { get; protected set; }

        /// <summary>
        /// The Battlefield 2 server process (when running)
        /// </summary>
        private Process ServerProcess;

        /// <summary>
        /// The Battlefield 2 Client process (when running)
        /// </summary>
        private Process ClientProcess;

        /// <summary>
        /// Indicates whether the hosts file redirects are active for the gamespy servers
        /// </summary>
        public static bool RedirectsEnabled { get; protected set; }

        /// <summary>
        /// A Background worker used for Hosts file redirects
        /// </summary>
        private BackgroundWorker HostsWorker = new BackgroundWorker();

        /// <summary>
        /// A Background worker uesd for starting the servers
        /// </summary>
        private BackgroundWorker ServerWorker = new BackgroundWorker();

        /// <summary>
        /// Constructor. Initializes and Displays the Applications main GUI
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            // Set instance
            Instance = this;

            // Check for needed settings upgrade
            if (!Config.SettingsUpdated)
            {
                Config.Upgrade();
                Config.SettingsUpdated = true;
                Config.Save();
            }

            // If this is the first run, Get client and server install paths
            if (String.IsNullOrWhiteSpace(Config.ServerPath) || !File.Exists(Path.Combine(Config.ServerPath, "bf2_w32ded.exe")))
            {
                InstallForm IS = new InstallForm();
                if (IS.ShowDialog() != DialogResult.OK)
                {
                    this.Load += new EventHandler(CloseOnStart);
                    return;
                }
            }

            // Make sure documents folder exists
            if (!Directory.Exists(Paths.DocumentsFolder))
                Directory.CreateDirectory(Paths.DocumentsFolder);

            // Backups folder
            if (!Directory.Exists(Path.Combine(Paths.DocumentsFolder, "Backups")))
                Directory.CreateDirectory(Path.Combine(Paths.DocumentsFolder, "Backups"));

            // Load installed Mods. If there is an error, a messagebox will be displayed, 
            // and the form closed automatically
            if (!LoadModList())
                return;

            // Set BF2Statistics Install Status
            SetInstallStatus();

            // Get snapshot counts
            CountSnapshots();

            // Check if the server is already running
            CheckServerProcess();

            // Try to access the hosts file
            DoHOSTSCheck();

            // Load Cross Session Settings
            ParamBox.Text = Config.ClientParams;
            GlobalServerSettings.Checked = Config.UseGlobalSettings;
            ShowConsole.Checked = Config.ShowServerConsole;
            MinimizeConsole.Checked = Config.MinimizeServerConsole;
            IgnoreAsserts.Checked = Config.ServerIgnoreAsserts;
            FileMoniter.Checked = Config.ServerFileMoniter;
            GpcmAddress.Text = (!String.IsNullOrWhiteSpace(Config.LastLoginServerAddress)) ? Config.LastLoginServerAddress : "localhost";
            Bf2webAddress.Text = (!String.IsNullOrWhiteSpace(Config.LastStatsServerAddress)) ? Config.LastStatsServerAddress : "localhost";
            labelTotalWebRequests.Text = Config.TotalASPRequests.ToString();

            // If we dont have a client path, disable the Launch Client button
            LaunchClientBtn.Enabled = (!String.IsNullOrWhiteSpace(Config.ClientPath) && File.Exists(Path.Combine(Config.ClientPath, "bf2.exe")));

            // Register for ASP events
            ASPServer.Started += new EventHandler(ASPServer_OnStart);
            ASPServer.Stopped += new EventHandler(ASPServer_OnShutdown);
            ASPServer.RequestRecieved += new AspRequest(ASPServer_ClientConnected);
            Snapshot.SnapshotProcessed += new SnapshotProccessed(Snapshot_SnapshotProccessed);
            ASP.Requests.SnapshotPost.SnapshotReceived += new SnapshotRecieved(SnapshotPost_SnapshotReceived);

            // Register for Login server events
            LoginServer.OnStart += new StartupEventHandler(LoginServer_OnStart);
            LoginServer.OnShutdown += new ShutdownEventHandler(LoginServer_OnShutdown);
            LoginServer.OnUpdate += new EventHandler(LoginServer_OnUpdate);

            // Add administrator title to program title bar
            if (Program.IsAdministrator)
                this.Text += " (Administrator)";

            // Set server tooltips
            Tipsy.SetToolTip(LoginStatusPic, "Login server is currently offline.");
            Tipsy.SetToolTip(AspStatusPic, "Asp server is currently offline");
            SysIcon = NotificationIcon;
        }


        #region Startup Methods

        /// <summary>
        /// This method sets the Install status if the BF2s python files
        /// </summary>
        private void SetInstallStatus()
        {
            if (File.Exists(Path.Combine(Config.ServerPath, "python", "bf2", "BF2StatisticsConfig.py")))
            {
                StatsEnabled = true;
                InstallBox.ForeColor = Color.Green;
                InstallBox.Text = "BF2 Statistics server files are currently installed.";
                BF2sInstallBtn.Text = "Uninstall BF2 Statistics Python";
                BF2sConfigBtn.Enabled = true;
                BF2sEditMedalDataBtn.Enabled = true;
                StatsStatusPic.Image = Resources.check;
                Tipsy.SetToolTip(StatsStatusPic, "BF2 Statistics server files are currently installed."); 
            }
            else
            {
                StatsEnabled = false;
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
        private bool LoadModList()
        {
            // Load the BF2 Server
            try
            {
                BF2Server.Load(Config.ServerPath);
            }
            catch(Exception E)
            {
                MessageBox.Show(E.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Add each valid mod to the mod selection list
            foreach (string Module in BF2Server.Mods)
            {
                try
                {
                    BF2Mod Mod = BF2Server.LoadMod(Module);
                    ModSelectList.Items.Add(Mod);
                    if (Mod.Name == "bf2")
                        ModSelectList.SelectedIndex = ModSelectList.Items.Count - 1;
                }
                catch (InvalidModException)
                {
                    continue;
                }
                catch (Exception e)
                {
                    Log(e.Message);
                }
            }

            // If we have no mods, we cant continue :(
            if (ModSelectList.Items.Count == 0)
            {
                MessageBox.Show("No battlefield 2 mods could be found! The application can no longer continue.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Load += new EventHandler(CloseOnStart);
                return false;
            }
            else if (ModSelectList.SelectedIndex == -1)
                ModSelectList.SelectedIndex = 0;

            return true;
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
                    HostsStatusPic.Image = Resources.check;
                    LockGroups();

                    RedirectButton.Enabled = true;
                    RedirectButton.Text = "Remove HOSTS Redirect";

                    UpdateHostFileStatus("- Locking HOSTS file");
                    HostsFile.Lock();
                    UpdateHostFileStatus("- All Done!");
                }
                else
                    Tipsy.SetToolTip(HostsStatusPic, "Gamespy redirects are not active.");
            }
        }

        /// <summary>
        /// Gets a count of processed and un processed snapshots
        /// </summary>
        private void CountSnapshots()
        {
            /// Unprocessed
            TotalUnProcSnapCount.Text = Directory.GetFiles(Paths.SnapshotTempPath).Length.ToString();
            // Processed
            TotalSnapCount.Text = Directory.GetFiles(Paths.SnapshotProcPath).Length.ToString();
        }

        /// <summary>
        /// Assigns the Server Process if the process is running
        /// </summary>
        private void CheckServerProcess()
        {
            Process[] processCollection = Process.GetProcessesByName("bf2_w32ded");
            foreach (Process P in processCollection)
            {
                if (Path.GetDirectoryName(P.MainModule.FileName) == Config.ServerPath)
                {
                    // Hook into the proccess so we know when its running, and register a closing event
                    ServerProcess = P;
                    ServerProcess.EnableRaisingEvents = true;
                    ServerProcess.Exited += new EventHandler(BF2Server_Exited);

                    // Set the status to online in the Status Overview
                    ServerStatusPic.Image = Resources.check;
                    LaunchServerBtn.Text = "Shutdown Server";

                    // Disable the Restore bf2s python files while server is running
                    BF2sRestoreBtn.Enabled = false;
                    BF2sInstallBtn.Enabled = false;
                    break;
                }
            }
        }

        #endregion Startup Methods

        #region Launcher Tab

        /// <summary>
        /// Event fired when the Launch emulator button is pushed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LaunchEmuBtn_Click(object sender, EventArgs e)
        {
            if (!LoginServer.IsRunning)
            {
                LaunchEmuBtn.Enabled = false;
                StartLoginserverBtn.Enabled = false;
                LoginStatusPic.Image = Resources.loading;

                // Start Servers in aBackground worker, Dont want MySQL locking up the GUI
                ServerWorker.DoWork += new DoWorkEventHandler(ServerWorker_StartLoginServers);
                ServerWorker.RunWorkerAsync();
            }
            else
            {
                LoginServer.Shutdown();
                Tipsy.SetToolTip(LoginStatusPic, "Login server us currently offline.");
            }
        }

        /// <summary>
        /// Background Operation for connecting to the login servers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerWorker_StartLoginServers(object sender, DoWorkEventArgs e)
        {
            // Unregister for work
            ServerWorker.DoWork -= new DoWorkEventHandler(ServerWorker_StartLoginServers);

            try
            {
                LoginServer.Start();
            }
            catch (Exception E)
            {
                // Exception message will already be there
                BeginInvoke((Action)delegate
                {
                    LoginStatusPic.Image = Resources.warning;
                    StartLoginserverBtn.Enabled = true;
                    LaunchEmuBtn.Enabled = true;
                    Tipsy.SetToolTip(LoginStatusPic, E.Message);

                    // Show the DB exception form if its a DB connection error
                    if (E is DbConnectException)
                        ExceptionForm.ShowDbConnectError(E as DbConnectException);
                });
            }
        }

        /// <summary>
        /// Client Launcher Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LaunchClientBtn_Click(object sender, EventArgs e)
        {
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
                LaunchClientBtn.Text = "Shutdown Battlefield 2";
            }
            else
            {
                try 
                {
                    ClientProcess.Kill();
                    this.Enabled = false;
                    LoadingForm.ShowScreen(this);
                }
                catch (Exception E)
                {
                    MessageBox.Show("Unable to stop Battlefield 2 client process!" + Environment.NewLine + Environment.NewLine +
                        E.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Event fired when Server has exited
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BF2Client_Exited(object sender, EventArgs e)
        {
            // Make this cross thread safe
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, EventArgs>(BF2Client_Exited), new object[] { sender, e });
            }
            else
            {
                ClientProcess.Close();
                LaunchClientBtn.Text = "Play Battlefield 2";
                ClientProcess = null;
                this.Enabled = true;
                LoadingForm.CloseForm();
            }
        }

        /// <summary>
        /// Server Launcher Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LaunchServerBtn_Click(object sender, EventArgs e)
        {
            if (ServerProcess == null)
            {
                // Start new BF2 proccess
                ProcessStartInfo Info = new ProcessStartInfo();
                Info.Arguments = String.Format(" +modPath mods/{0}", SelectedMod.Name.ToLower());

                // Use the global server settings file?
                if (GlobalServerSettings.Checked)
                    Info.Arguments += " +config " + Path.Combine(Program.RootPath, "Python", "GlobalServerSettings.con");

                // Moniter Con Files?
                if (FileMoniter.Checked)
                    Info.Arguments += " +fileMonitor 1";

                // Ignore Asserts? (Non-Fetal Startup Errors)
                if (IgnoreAsserts.Checked)
                    Info.Arguments += " +ignoreAsserts 1";

                // Hide window if user specifies this...
                if (!ShowConsole.Checked)
                    Info.WindowStyle = ProcessWindowStyle.Hidden;
                else if(Config.MinimizeServerConsole)
                    Info.WindowStyle = ProcessWindowStyle.Minimized;

                // Start process. Set working directory so we dont get errors!
                Info.FileName = "bf2_w32ded.exe";
                Info.WorkingDirectory = BF2Server.RootPath;
                ServerProcess = Process.Start(Info);

                // Hook into the proccess so we know when its running, and register a closing event
                ServerProcess.EnableRaisingEvents = true;
                ServerProcess.Exited += new EventHandler(BF2Server_Exited);

                // Set status to online
                ServerStatusPic.Image = Resources.check;
                LaunchServerBtn.Text = "Shutdown Server";

                // Disable the Restore bf2s python files while server is running
                BF2sRestoreBtn.Enabled = false;
                BF2sInstallBtn.Enabled = false;
            }
            else
            {
                try
                {
                    ServerProcess.Kill();
                    this.Enabled = false;
                    LoadingForm.ShowScreen(this);
                }
                catch(Exception E)
                {
                    MessageBox.Show("Unable to stop server process!" + Environment.NewLine + Environment.NewLine +
                        E.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Event fired when Server has exited
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BF2Server_Exited(object sender, EventArgs e)
        {
            // Make this cross thread safe
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, EventArgs>(BF2Server_Exited), new object[] { sender, e });
            }
            else
            {
                ServerProcess.Close();
                ServerStatusPic.Image = Resources.error;
                LaunchServerBtn.Text = "Launch Server";
                ServerProcess = null;
                BF2sRestoreBtn.Enabled = true;
                BF2sInstallBtn.Enabled = true;
                this.Enabled = true;
                LoadingForm.CloseForm();
            }
        }

        /// <summary>
        /// Event fired when the selected mod changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModSelectList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedMod = (BF2Mod) ModSelectList.SelectedItem;
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        private void LoginServer_OnStart()
        {
            // Make this cross thread safe
            BeginInvoke((Action)delegate
            {
                LoginStatusPic.Image = Resources.check;
                LaunchEmuBtn.Text = "Shutdown Login Server";
                LaunchEmuBtn.Enabled = true;
                StartLoginserverBtn.Text = "Shutdown Login Server";
                StartLoginserverBtn.Enabled = true;
                CreateAcctBtn.Enabled = true;
                EditAcctBtn.Enabled = true;
                LoginStatusLabel.Text = "Running";
                LoginStatusLabel.ForeColor = Color.LimeGreen;
            });
        }

        /// <summary>
        /// Event fired when the login emulator shutsdown
        /// </summary>
        private void LoginServer_OnShutdown()
        {
            // Make this cross thread safe
            BeginInvoke((Action)delegate
            {
                ConnectedClients.Clear();
                LoginStatusPic.Image = Resources.error;
                ClientCountLabel.Text = "0";
                LaunchEmuBtn.Text = "Start Login Server";
                StartLoginserverBtn.Text = "Start Login Server";
                CreateAcctBtn.Enabled = false;
                EditAcctBtn.Enabled = false;
                LoginStatusLabel.Text = "Stopped";
                LoginStatusLabel.ForeColor = SystemColors.ControlDark;
            });
        }

        /// <summary>
        /// This method updates the connected clients area of the login emulator tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginServer_OnUpdate(object sender, EventArgs e)
        {
            // DO processing in this thread
            StringBuilder SB = new StringBuilder();
            List<GpcmClient> Clients = ((ClientList)e).Clients;
            int PeakClients = Int32.Parse(labelPeakClients.Text);
            if (PeakClients < Clients.Count)
                PeakClients = Clients.Count;

            foreach (GpcmClient C in Clients)
                SB.AppendFormat(" {0} ({1}) - {2}{3}", C.ClientNick, C.ClientPID, C.IpAddress, Environment.NewLine);

            // Update connected clients count, and list, on main thread
            BeginInvoke((Action)delegate
            {
                ClientCountLabel.Text = Clients.Count.ToString();
                labelPeakClients.Text = PeakClients.ToString();
                ConnectedClients.Clear();
                ConnectedClients.Text = SB.ToString();
            });
        }

        private void StartLoginserverBtn_Click(object sender, EventArgs e)
        {
            LaunchEmuBtn_Click(sender, e);
        }

        private void CreateAcctBtn_Click(object sender, EventArgs e)
        {
            CreateAcctForm Form = new CreateAcctForm();
            Form.ShowDialog();
        }

        private void EditGamespyConfigBtn_Click(object sender, EventArgs e)
        {
            GamespyConfig Form = new GamespyConfig();
            Form.ShowDialog();
        }

        private void EditAcctBtn_Click(object sender, EventArgs e)
        {
            AccountListForm Form = new AccountListForm();
            Form.ShowDialog();
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

            try
            {
                // Install
                if (!StatsEnabled)
                {
                    // Remove current Python
                    Directory.Delete(Paths.ServerPythonPath, true);
                    System.Threading.Thread.Sleep(750);

                    // Make sure we dont have an empty backup folder
                    if (Directory.GetFiles(Paths.RankedPythonPath).Length == 0)
                        DirectoryExt.Copy(Path.Combine(Program.RootPath, "Python", "Ranked", "Original"), Paths.ServerPythonPath, true);
                    else
                        DirectoryExt.Copy(Paths.RankedPythonPath, Paths.ServerPythonPath, true);
                }
                else // Uninstall
                {
                    // Backup the users bf2s python files
                    Directory.Delete(Paths.RankedPythonPath, true);
                    System.Threading.Thread.Sleep(750);
                    DirectoryExt.Copy(Paths.ServerPythonPath, Paths.RankedPythonPath, true);

                    // Install default python files
                    Directory.Delete(Paths.ServerPythonPath, true);
                    System.Threading.Thread.Sleep(750);
                    DirectoryExt.Copy(Paths.NonRankedPythonPath, Paths.ServerPythonPath, true);
                }
            }
            catch (Exception E)
            {
                Program.ErrorLog.Write("ERROR: [BF2sPythonInstall] " + E.Message);
                throw;
            }
            finally
            {
                // Unlock now that we are done
                System.Threading.Thread.Sleep(300);
                SetInstallStatus();
                this.Enabled = true;
                LoadingForm.CloseForm();
            }
        }

        /// <summary>
        /// This button opens up the BF2Statistics config form
        /// </summary>
        private void BF2sConfig_Click(object sender, EventArgs e)
        {
            BF2sConfig Form = new BF2sConfig();
            Form.ShowDialog();
        }

        /// <summary>
        /// This button opens up the Medal Data Editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    if (StatsEnabled)
                    {
                        Directory.Delete(Paths.ServerPythonPath, true);
                        System.Threading.Thread.Sleep(750);
                        DirectoryExt.Copy(Path.Combine(Program.RootPath, "Python", "Ranked", "Original"), Paths.ServerPythonPath, true);
                    }
                    else
                    {
                        Directory.Delete(Paths.RankedPythonPath, true);
                        System.Threading.Thread.Sleep(750);
                        DirectoryExt.Copy(Path.Combine(Program.RootPath, "Python", "Ranked", "Original"), Paths.RankedPythonPath, true);
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    string text = Bf2webAddress.Text.Trim();
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
                    if (text.ToLower().Trim() == "localhost")
                        text = IPAddress.Loopback.ToString();

                    // Check if this is an IP address or hostname
                    IPAddress BF2Web;
                    try {
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

                    // Append line, and update status
                    HostsFile.Set("bf2web.gamespy.com", BF2Web.ToString());
                    Config.LastStatsServerAddress = Bf2webAddress.Text.Trim();
                    UpdateHostFileStatus("- Adding bf2web.gamespy.com redirect to hosts file");
                }

                // First, lets determine what the user wants to redirect
                if (GpcmCheckbox.Checked)
                {
                    // Make sure we have a valid IP address in the address box!
                    string text2 = GpcmAddress.Text.Trim();
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
                    if (text2.ToLower().Trim() == "localhost")
                        text2 = IPAddress.Loopback.ToString();

                    // Make sure the IP address is valid!
                    IPAddress GpcmA;
                    try {
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

                    // Update status
                    UpdateHostFileStatus("- Adding gpcm.gamespy.com redirect to hosts file");
                    UpdateHostFileStatus("- Adding gpsp.gamespy.com redirect to hosts file");

                    // Append lines to hosts file
                    HostsFile.Set("gpcm.gamespy.com", GpcmA.ToString());
                    HostsFile.Set("gpsp.gamespy.com", GpcmA.ToString());
                    Config.LastLoginServerAddress = GpcmAddress.Text.Trim();
                }

                // Save last used addresses
                Config.Save();

                // Create new instance of the background worker
                HostsWorker = new BackgroundWorker();
                HostsWorker.WorkerSupportsCancellation = true;

                // Write the lines to the hosts file
                UpdateHostFileStatus("- Writting to hosts file... ", false);
                try
                {
                    // Save lines to hosts file
                    HostsFile.Save();
                    UpdateHostFileStatus("Success!");

                    // Flush DNS Cache
                    FlushDNS();

                    // Do pings, And lock hosts file. We do this in
                    // a background worker so the User can imediatly start
                    // the BF2 client while the HOSTS redirect finishes
                    HostsWorker.DoWork += new DoWorkEventHandler(RebuildDNSCache);
                    HostsWorker.RunWorkerAsync();

                    // Set form data
                    RedirectsEnabled = true;
                    HostsStatusPic.Image = Resources.check;
                    RedirectButton.Text = "Remove HOSTS Redirect";
                    RedirectButton.Enabled = true;
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
                }
            }
            else
            {
                // Lock the button
                RedirectButton.Enabled = false;

                // Create new instance of the background worker
                if (HostsWorker == null)
                {
                    HostsWorker = new BackgroundWorker();
                    HostsWorker.WorkerSupportsCancellation = true;
                }

                // Stop worker if its busy
                if (HostsWorker.IsBusy)
                {
                    LoadingForm.ShowScreen(this);
                    this.Enabled = false;
                    HostsWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(HostsWorker_Completed);
                    HostsWorker.CancelAsync();
                    return;
                }

                UndoRedirects();
            }
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
            FlushDNS();

            // Update status
            UpdateHostFileStatus("- All Done!");
            Tipsy.SetToolTip(HostsStatusPic, "Gamespy redirects are NOT active.");

            // Reset form data
            RedirectsEnabled = false;
            HostsStatusPic.Image = Resources.error;
            RedirectButton.Text = "Begin HOSTS Redirect";
            UnlockGroups();
        }

        /// <summary>
        /// If the user cancels the redirects while the worker is currently building
        /// the DNS cache, this event will be registered. On completion, the redirects
        /// are reverted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HostsWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            // Unregister
            HostsWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(HostsWorker_Completed);
            UndoRedirects();
            LoadingForm.CloseForm();
            this.Enabled = true;
        }

        /// <summary>
        /// Method is used to unlock the input fields
        /// </summary>
        private void UnlockGroups()
        {
            RedirectButton.Enabled = true;
            GpcmGroupBox.Enabled = true;
            BF2webGroupBox.Enabled = true;
            Bf2AaGroupBox.Enabled = true;
        }

        /// <summary>
        /// Method is used to lock the input fields while redirect is active
        /// </summary>
        private void LockGroups()
        {
            RedirectButton.Enabled = false;
            GpcmGroupBox.Enabled = false;
            BF2webGroupBox.Enabled = false;
            Bf2AaGroupBox.Enabled = false;
        }

        /// <summary>
        /// Preforms the pings required to fill the dns cache, and locks the HOSTS file.
        /// The reason we ping, is because once the HOSTS file is locked, any request
        /// made to a url (when the DNS cache is empty), will skip the hosts file, because 
        /// it cant be read. If we ping first, then the DNS cache fills up with the IP 
        /// addresses in the hosts file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RebuildDNSCache(object sender, DoWorkEventArgs e)
        {
            // Update status window
            UpdateHostFileStatus("- Rebuilding DNS Cache... ", false);
            foreach (KeyValuePair<String, String> IP in HostsFile.GetLines())
            {
                // Cancel if we have a cancelation request
                if (HostsWorker.CancellationPending)
                {
                    UpdateHostFileStatus("Cancelled!");
                    e.Cancel = true;
                    return;
                }

                Ping p = new Ping();
                PingReply reply = p.Send(IP.Key);
            }
            UpdateHostFileStatus("Done");

            // Lock the hosts file
            UpdateHostFileStatus("- Locking HOSTS file");
            HostsFile.Lock();
            UpdateHostFileStatus("- All Done!");
            Tipsy.SetToolTip(HostsStatusPic, "Gamespy redirects are currently active.");
        }

        /// <summary>
        /// Adds a new line to the "status" window on the GUI
        /// </summary>
        /// <param name="message">The message to print</param>
        public void UpdateHostFileStatus(string message)
        {
            UpdateHostFileStatus(message, true);
        }

        /// <summary>
        /// Adds a new line to the "status" window on the GUI
        /// </summary>
        /// <param name="message">The message to print</param>
        /// <param name="newLine">Add a new line for the next message?</param>
        public void UpdateHostFileStatus(string message, bool newLine)
        {
            // Add new line
            if (newLine) message = message + Environment.NewLine;

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

        /// <summary>
        /// For external use... Displays a message box with the provided message
        /// </summary>
        /// <param name="message">The message to dispay to the client</param>
        public static void Show(string message) 
        {
            MessageBox.Show(message, "Error");
        }

        /// <summary>
        /// Flushes the Windows DNS cache
        /// </summary>
        public void FlushDNS()
        {
            UpdateHostFileStatus("- Flushing DNS Cache");
            DnsFlushResolverCache();
        }

        [DllImport("dnsapi.dll", EntryPoint = "DnsFlushResolverCache")]
        private static extern UInt32 DnsFlushResolverCache();

        #endregion Hosts File Redirect

        #region ASP Server

        /// <summary>
        /// Starts and stops the ASP HTTP server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartAspServerBtn_Click(object sender, EventArgs e)
        {
            if (!ASPServer.IsRunning)
            {
                AspStatusPic.Image = Resources.loading;
                StartAspServerBtn.Enabled = false;
                StartWebserverBtn.Enabled = false;

                // Start Server in a Background worker, Dont want MySQL locking up the GUI
                ServerWorker.DoWork += new DoWorkEventHandler(ServerWorker_StartAsp);
                ServerWorker.RunWorkerAsync();
            }
            else
            {
                try {
                    ASPServer.Stop();
                    Tipsy.SetToolTip(AspStatusPic, "Asp server is currently offline");  
                }
                catch(Exception E) {
                    Program.ErrorLog.Write(E.Message);
                }
            }
        }

        /// <summary>
        /// Background Operation for Starting the ASP Server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerWorker_StartAsp(object sender, DoWorkEventArgs e)
        {
            // Unregister for work
            ServerWorker.DoWork -= new DoWorkEventHandler(ServerWorker_StartAsp);

            try
            {
                // Start server
                ASPServer.Start();
            }
            catch (HttpListenerException E)
            {
                // Custom port 80 in use message
                string Message;
                if (E.ErrorCode == 32)
                    Message = "Port 80 is already in use by another program.";
                else
                    Message = E.Message;

                BeginInvoke((Action)delegate
                {
                    StartAspServerBtn.Enabled = true;
                    StartWebserverBtn.Enabled = true;
                    AspStatusPic.Image = Resources.warning;
                    Tipsy.SetToolTip(AspStatusPic, Message);
                    Notify.Show("Failed to start ASP Server!", Message, AlertType.Warning);
                });
            }
            catch (Exception E)
            {
                // Check for specific error
                Program.ErrorLog.Write("[ASP Server] " + E.Message);
                BeginInvoke((Action)delegate
                {
                    StartAspServerBtn.Enabled = true;
                    StartWebserverBtn.Enabled = true;
                    AspStatusPic.Image = Resources.warning;
                    Tipsy.SetToolTip(AspStatusPic, E.Message);

                    // Show the DB exception form if its a DB connection error
                    if (E is DbConnectException)
                        ExceptionForm.ShowDbConnectError(E as DbConnectException);
                    else
                        Notify.Show("Failed to start ASP Server!", E.Message, AlertType.Warning);
                });
            }
        }

        /// <summary>
        /// Starts the ASP Webserver
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                ViewSnapshotBtn.Enabled = true;
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
                ViewSnapshotBtn.Enabled = false;
                EditPlayerBtn.Enabled = false;
                EditASPDatabaseBtn.Enabled = true;
                ClearStatsBtn.Enabled = false;
                AspStatusLabel.Text = "Stopped";
                AspStatusLabel.ForeColor = SystemColors.ControlDark;
                StartWebserverBtn.Text = "Start Webserver";
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
                Config.TotalASPRequests++;
                labelTotalWebRequests.Text = Config.TotalASPRequests.ToString();
                int Sr = Int32.Parse(labelSessionWebRequests.Text);
                if (Sr < ASPServer.SessionRequests)
                    labelSessionWebRequests.Text = ASPServer.SessionRequests.ToString();
            });
        }

        /// <summary>
        /// Updates the GUI when a snapshot is proccessed
        /// </summary>
        private void Snapshot_SnapshotProccessed()
        {
            BeginInvoke((Action)delegate { CountSnapshots(); });
        }

        /// <summary>
        /// Updates the GUI when a snapshot is recieved successfully
        /// </summary>
        private void SnapshotPost_SnapshotReceived(bool Proccessed)
        {
            BeginInvoke((Action)delegate
            {
                string[] Parts = labelSnapshotsProc.Text.Split('/');
                int Total = Int32.Parse(Parts[1]) + 1;
                int Good = Int32.Parse(Parts[0]);
                if (Proccessed)
                    Good++;

                labelSnapshotsProc.Text = String.Concat(Good, " / ", Total);
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewAccessLogBtn_Click(object sender, EventArgs e)
        {
            Process.Start(Path.Combine(Program.RootPath, "Logs", "AspAccess.log"));
        }

        /// <summary>
        /// View ASP Error Log Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewErrorLogBtn_Click(object sender, EventArgs e)
        {
            Process.Start(Path.Combine(Program.RootPath, "Logs", "AspServer.log"));
        }

        /// <summary>
        /// View Snapshot Logs Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewSnapshotBtn_Click(object sender, EventArgs e)
        {
            SnapshotViewForm Form = new SnapshotViewForm();
            Form.ShowDialog();
            CountSnapshots();
        }

        /// <summary>
        /// Edit Stats Database Settings Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditASPDatabaseBtn_Click(object sender, EventArgs e)
        {
            StatsDbConfigForm Form = new StatsDbConfigForm();
            Form.ShowDialog();
        }

        /// <summary>
        /// Edit ASP Settings Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditASPSettingsBtn_Click(object sender, EventArgs e)
        {
            ASPConfigForm Form = new ASPConfigForm();
            Form.ShowDialog();
        }

        /// <summary>
        /// Edit Player Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditPlayerBtn_Click(object sender, EventArgs e)
        {
            PlayerSearchForm Form = new PlayerSearchForm();
            Form.ShowDialog();
        }

        /// <summary>
        /// Clear Stats Database Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManageStatsDBBtn_Click(object sender, EventArgs e)
        {
            ManageStatsDBForm Form = new ManageStatsDBForm();
            Form.ShowDialog();
        }

        #endregion ASP Server

        #region Status OnClick Events

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

        #endregion Status OnClick Events

        #region About Tab

        private void Bf2StatisticsLink_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.bf2statistics.com/");
        }

        /// <summary>
        /// Setup Button Click Event. Relaunches the setup client/server paths screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetupBtn_Click(object sender, EventArgs e)
        {
            InstallForm IS = new InstallForm();
            IS.ShowDialog();

            // Re-Init server if we need to
            if (Config.ServerPath != BF2Server.RootPath)
                BF2Server.Load(Config.ServerPath);
        }

        /// <summary>
        /// Open Program Folder Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenRootBtn_Click(object sender, EventArgs e)
        {
            Process.Start(Program.RootPath);
        }

        /// <summary>
        /// Check for Updates Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkUpdateBtn_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/BF2Statistics/ControlCenter/releases/latest");
        }

        #endregion About Tab

        #region Static Control Methods

        /// <summary>
        /// Static call that can disable the main form
        /// </summary>
        public static void Disable()
        {
            Instance.Invoke((Action)delegate
            {
                Instance.Enabled = false;
            });
        }

        /// <summary>
        /// Static call that can enable the main form
        /// </summary>
        public static void Enable()
        {
            Instance.Invoke((Action)delegate
            {
                Instance.Enabled = true;
            });
        }

        #endregion Static Control Methods

        #region Closer Methods

        /// <summary>
        /// Event closes the form when fired
        /// </summary>
        private void CloseOnStart(object sender, EventArgs e)
        {
            this.Close();
        }

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
            Config.ServerIgnoreAsserts = IgnoreAsserts.Checked;
            Config.ServerFileMoniter = FileMoniter.Checked;
            Config.Save();

            // Shutdown login servers
            if (LoginServer.IsRunning)
                LoginServer.Shutdown();

            // Shutdown ASP Server
            if (ASPServer.IsRunning)
                ASPServer.Stop();

            // Unlock the hosts file
            HostsFile.UnLock();
        }

        #endregion Closer Methods

        /// <summary>
        /// This method is used to store a message in the console.log file
        /// </summary>
        /// <param name="message">The message to be written to the log file</param>
        public static void Log(string message)
        {
            Program.ErrorLog.Write(message);
        }

        /// <summary>
        /// This method is used to store a message in the console.log file
        /// </summary>
        /// <param name="message">The message to be written to the log file</param>
        public static void Log(string message, params object[] items)
        {
            Program.ErrorLog.Write(String.Format(message, items));
        }
    }
}
