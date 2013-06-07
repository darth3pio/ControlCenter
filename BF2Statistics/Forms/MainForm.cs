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
using System.Security.Principal;
using BF2Statistics.Properties;
using BF2Statistics.ASP;
using BF2Statistics.Gamespy;
using BF2Statistics.Logging;

namespace BF2Statistics
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// The User Config object
        /// </summary>
        public static Settings Config = Settings.Default;

        /// <summary>
        /// Startup root directory for this application
        /// </summary>
        public static string Root = Application.StartupPath;

        /// <summary>
        /// The instance of this form
        /// </summary>
        public static MainForm Instance { get; protected set; }

        /// <summary>
        /// The main form log file
        /// </summary>
        public static LogWritter ErrorLog { get; protected set; }

        /// <summary>
        /// An array of found mods
        /// </summary>
        public static Dictionary<string, string> InstalledMods = new Dictionary<string, string>();

        /// <summary>
        /// The current selected mod foldername
        /// </summary>
        public static string SelectedMod { get; protected set; }

        /// <summary>
        /// Returns a bool stating whether the stats enabled python files are installed
        /// </summary>
        public static bool StatsEnabled { get; protected set; }

        /// <summary>
        /// The Battlefield 2 server process (when running)
        /// </summary>
        private Process ServerProccess;

        /// <summary>
        /// Full path to the current selected mod's settings folder
        /// </summary>
        public static string SettingsPath { get; protected set; }

        /// <summary>
        /// The bf2 python path
        /// </summary>
        public static string ServerPythonPath { get; protected set; }

        /// <summary>
        /// Full path to the stats enabled python files
        /// </summary>
        public static string RankedPythonPath { get; protected set; }

        /// <summary>
        /// Full path to the Non-Ranked (default) python files
        /// </summary>
        public static string NonRankedPythonPath { get; protected set; }

        /// <summary>
        /// The HOSTS file object
        /// </summary>
        private HostsFile HostFile;

        /// <summary>
        /// Are hosts file redirects active?
        /// </summary>
        private bool RedirectsEnabled = false;

        /// <summary>
        /// A General Background worker used for various tasks
        /// </summary>
        private BackgroundWorker bWorker;

        /// <summary>
        /// Returns whether the app is running in administrator mode.
        /// </summary>
        public static bool IsAdministrator
        {
            get
            {
                WindowsPrincipal wp = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                return wp.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        /// <summary>
        /// Constructor
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

            // Create logs folder if it doesnt exist
            if (!Directory.Exists(Path.Combine(Root, "Logs")))
                Directory.CreateDirectory(Path.Combine(Root, "Logs"));

            // Make sure we have our snapshot dirs made
            if (!Directory.Exists(ASP.Requests.SnapshotPost.TempPath))
                Directory.CreateDirectory(ASP.Requests.SnapshotPost.TempPath);
            if (!Directory.Exists(ASP.Requests.SnapshotPost.ProcPath))
                Directory.CreateDirectory(ASP.Requests.SnapshotPost.ProcPath);

            // Create ErrorLog file
            ErrorLog = new LogWritter(Path.Combine(Root, "Logs", "Error.log"), 3000);

            // Define python paths
            ServerPythonPath = Path.Combine(Config.ServerPath, "python", "bf2");
            NonRankedPythonPath = Path.Combine(MainForm.Root, "Python", "NonRanked");
            RankedPythonPath = Path.Combine(MainForm.Root, "Python", "Ranked", "Backup");

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
            try {
                // Do hosts file check for existing redirects
                HostFile = new HostsFile();
                DoHOSTSCheck();
            }
            catch (Exception e) {
                HostsStatusPic.Image = Resources.warning;
                MessageBox.Show(e.Message, "Error");
            }

            // Load Cross Session Settings
            ParamBox.Text = Config.ClientParams;
            GlobalServerSettings.Checked = Config.UseGlobalSettings;
            ShowConsole.Checked = Config.ShowServerConsole;
            MinimizeConsole.Checked = Config.MinimizeServerConsole;
            IgnoreAsserts.Checked = Config.ServerIgnoreAsserts;
            FileMoniter.Checked = Config.ServerFileMoniter;
            GpcmAddress.Text = Config.LastLoginServerAddress;
            Bf2webAddress.Text = Config.LastStatsServerAddress;

            // If we dont have a client path, disable the Launch Client button
            LaunchClientBtn.Enabled = (!String.IsNullOrWhiteSpace(Config.ClientPath) && File.Exists(Path.Combine(Config.ClientPath, "bf2.exe")));

            // Register for ASP events
            ASPServer.OnStart += new StartupEventHandler(ASPServer_OnStart);
            ASPServer.OnShutdown += new ShutdownEventHandler(ASPServer_OnShutdown);

            // Register for Login server events
            LoginServer.OnStart += new StartupEventHandler(LoginServer_OnStart);
            LoginServer.OnShutdown += new ShutdownEventHandler(LoginServer_OnShutdown);
            LoginServer.OnUpdate += new EventHandler(LoginServer_OnUpdate);

            // Set the ASP and Login Server statusbox boxes
            ASPServer.SetStatusBox(AspStatusBox);
            LoginServer.SetStatusBox(EmuStatusWindow);

            // Add administrator title to program title bar
            if (IsAdministrator)
                this.Text += " (Administrator)";
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
                InstallButton.Text = "Uninstall BF2 Statistics Python";
                BF2sConfigBtn.Enabled = true;
                BF2sEditMedalDataBtn.Enabled = true;
                StatsStatusPic.Image = Resources.check;
            }
            else
            {
                StatsEnabled = false;
                InstallBox.ForeColor = Color.Red;
                InstallBox.Text = "BF2 Statistics server files are currently NOT installed";
                InstallButton.Text = "Install BF2 Statistics Python";
                BF2sConfigBtn.Enabled = false;
                BF2sEditMedalDataBtn.Enabled = false;
                StatsStatusPic.Image = Resources.error;
            }
        }

        /// <summary>
        /// Loads up all the supported mods, and adds them to the Mod select list
        /// </summary>
        private bool LoadModList()
        {
            string path = Path.Combine(Config.ServerPath, "mods");

            // Make sure the levels folder exists!
            if (!Directory.Exists(path))
            {
                this.Load += new EventHandler(CloseOnStart);
                MessageBox.Show("Unable to locate the 'mods' folder. Please make sure you have selected a valid "
                    + "battlefield 2 installation path before proceeding.", "Error");
                return false;
            }

            string[] Mods = Directory.GetDirectories(path);
            XmlDocument Desc = new XmlDocument();

            // Proccess each installed mod
            foreach (string D in Mods)
            {
                // Get just the mod folder name
                string ModName = D.Remove(0, path.Length + 1);

                // Make sure we have a mod description file
                string DescFile = Path.Combine(D, "mod.desc");
                if(!File.Exists(DescFile))
                    continue;

                // Make sure the server supports the mod as well...
                if (!Directory.Exists(Path.Combine(Config.ServerPath, "mods", ModName)))
                    continue;

                // Get the actual name of the mod
                try
                {
                    Desc.Load(DescFile);
                    XmlNodeList Node = Desc.GetElementsByTagName("title");
                    string Name = Node[0].InnerText.Trim();
                    if (Name == "MODDESC_BF2_TITLE")
                    {
                        ModSelectList.Items.Add(new KeyValueItem(ModName, "Battlefield 2"));
                        ModSelectList.SelectedIndex = ModSelectList.Items.Count - 1;
                        continue;
                    }
                    else if (Name == "MODDESC_XP_TITLE")
                        Name = "Battlefield 2: Special Forces";

                    ModSelectList.Items.Add(new KeyValueItem(ModName, Name));
                    InstalledMods.Add(D, Name);
                }
                catch (Exception E)
                {
                    Log(E.Message);
                }
            }

            // If we have no mods, we cant continue :(
            if (ModSelectList.Items.Count == 0)
            {
                MessageBox.Show("No battlefield 2 mods could be found!", "Error");
                this.Load += new EventHandler(CloseOnStart);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets a count of processed and un processed snapshots
        /// </summary>
        private void CountSnapshots()
        {
            string[] Files = Directory.GetFiles(ASP.Requests.SnapshotPost.TempPath);
            TotalUnProcSnapCount.Text = Files.Length.ToString();
            TotalUnProcSnapCount.Update();
            Files = Directory.GetFiles(ASP.Requests.SnapshotPost.ProcPath);
            TotalSnapCount.Text = Files.Length.ToString();
            TotalSnapCount.Update();
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
                    ServerProccess = P;

                    // Hook into the proccess so we know when its running, and register a closing event
                    ServerProccess.EnableRaisingEvents = true;
                    ServerProccess.Exited += new EventHandler(BF2_Exited);

                    // Set status to online
                    ServerStatusPic.Image = Resources.check;
                    LaunchServerBtn.Text = "Shutdown Server";

                    // Disable the Restore bf2s python files while server is running
                    BF2sRestoreBtn.Enabled = false;
                    break;
                }
            }
        }

        /// <summary>
        /// This method is used to store a message in the console.log file
        /// </summary>
        /// <param name="message">The message to be written to the log file</param>
        public static void Log(string message)
        {
            ErrorLog.Write(message);
        }

        /// <summary>
        /// This method is used to store a message in the console.log file
        /// </summary>
        /// <param name="message">The message to be written to the log file</param>
        public static void Log(string message, params object[] items)
        {
            ErrorLog.Write(String.Format(message, items));
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
                try
                {
                    LoginStatusPic.Image = Resources.loading;
                    LoginServer.Start();
                }
                catch
                {
                    LoginStatusPic.Image = Resources.warning;
                }
            }
            else
            {
                LoginServer.Shutdown();
            }
        }

        /// <summary>
        /// Client Launcher Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LaunchClientBtn_Click(object sender, EventArgs e)
        {
            // Get our current mod
            string mod = ((KeyValueItem)ModSelectList.SelectedItem).Key;

            // Start new BF2 proccess
            ProcessStartInfo Info = new ProcessStartInfo();
            Info.Arguments = String.Format(" +modPath mods/{0} {1}", mod.ToLower(), ParamBox.Text.Trim());
            Info.FileName = "bf2.exe";
            Info.WorkingDirectory = Config.ClientPath;
            Process BF2 = Process.Start(Info);
        }

        /// <summary>
        /// Server Launcher Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LaunchServerBtn_Click(object sender, EventArgs e)
        {
            if (ServerProccess == null)
            {
                // Get our current mod
                string mod = ((KeyValueItem)ModSelectList.SelectedItem).Key;

                // Start new BF2 proccess
                ProcessStartInfo Info = new ProcessStartInfo();
                Info.Arguments = String.Format(" +modPath mods/{0}", mod.ToLower());

                // Use the global server settings file?
                if (GlobalServerSettings.Checked)
                    Info.Arguments += " +config " + Path.Combine(MainForm.Root, "Python", "GlobalServerSettings.con");

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

                Info.FileName = "bf2_w32ded.exe";
                Info.WorkingDirectory = Config.ServerPath;
                ServerProccess = Process.Start(Info);

                // Hook into the proccess so we know when its running, and register a closing event
                ServerProccess.EnableRaisingEvents = true;
                ServerProccess.Exited += new EventHandler(BF2_Exited);

                // Set status to online
                ServerStatusPic.Image = Resources.check;
                LaunchServerBtn.Text = "Shutdown Server";

                // Disable the Restore bf2s python files while server is running
                BF2sRestoreBtn.Enabled = false;
            }
            else
            {
                try
                {
                    ServerProccess.Kill();
                }
                catch(Exception E)
                {
                    MessageBox.Show("Could not stop server!" + Environment.NewLine + Environment.NewLine +
                        "Reason: " + E.Message, "Error");
                }
            }
        }

        /// <summary>
        /// Event fired when Server has exited
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BF2_Exited(object sender, EventArgs e)
        {
            // Make this cross thread safe
            if (LaunchServerBtn.InvokeRequired)
            {
                this.Invoke(new Action<object, EventArgs>(BF2_Exited), new object[] { sender, e });
            }
            else
            {
                ServerStatusPic.Image = Resources.error;
                LaunchServerBtn.Text = "Launch Server";
                ServerProccess = null;
                BF2sRestoreBtn.Enabled = true;
            }
        }

        /// <summary>
        /// Event fired when the selected mod changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModSelectList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedMod = ((KeyValueItem)ModSelectList.SelectedItem).Key;
            SettingsPath = Path.Combine(Config.ServerPath, "mods", SelectedMod, "settings");
            string[] lines = File.ReadAllLines(Path.Combine(SettingsPath, "maplist.con"));

            if (lines.Length != 0)
            {
                Match M = Regex.Match(lines[0],
                    @"^maplist.append[\s|\t]+([""]*)(?<Mapname>[a-z0-9_]+)([""]*)[\s|\t]+([""]*)gpm_(?<Gamemode>[a-z]+)([""]*)[\s|\t]+(?<Size>[0-9]+)$", 
                    RegexOptions.IgnoreCase
                );
                if (M.Success)
                {
                    // First, convert mapname into an array, and capitalize each word
                    string Mapname = M.Groups["Mapname"].ToString().Replace('_', ' ');
                    string[] Parts = Mapname.Split(' ');
                    for (int i = 0; i < Parts.Length; i++)
                    {
                        // Ignore empty parts
                        if(String.IsNullOrWhiteSpace(Parts[i]))
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

                    // Convert gametype
                    string Type = M.Groups["Gamemode"].ToString();
                    switch (Type)
                    {
                        case "coop":
                            MapModeBox.Text = "Co-op";
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
                    MapSizeBox.Text = M.Groups["Size"].ToString();
                }
            }
        }

        #endregion Launcher Tab

        #region Login Emulator Tab

        /// <summary>
        /// Event fired when the login server starts
        /// </summary>
        private void LoginServer_OnStart()
        {
            // Make this cross thread safe
            if (LoginStatusPic.InvokeRequired)
            {
                this.Invoke(new Action(LoginServer_OnShutdown));
            }
            else
            {
                LoginStatusPic.Image = Resources.check;
                LaunchEmuBtn.Text = "Shutdown Login Server";
                CreateAcctBtn.Enabled = true;
                EditAcctBtn.Enabled = true;
            }
        }

        /// <summary>
        /// Event fired when the login emulator shutsdown
        /// </summary>
        private void LoginServer_OnShutdown()
        {
            // Make this cross thread safe
            if (LoginStatusPic.InvokeRequired)
            {
                this.Invoke(new Action(LoginServer_OnShutdown));
            }
            else
            {
                ConnectedClients.Clear();
                LoginStatusPic.Image = Resources.error;
                ClientCountLabel.Text = "Number of Connected Clients: 0";
                LaunchEmuBtn.Text = "Start Login Server";
                CreateAcctBtn.Enabled = false;
                EditAcctBtn.Enabled = false;
            }
        }

        /// <summary>
        /// This method updates the connected clients area of the login emulator tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginServer_OnUpdate(object sender, EventArgs e)
        {
            StringBuilder SB = new StringBuilder();
            ClientList Clients = (ClientList)e;
            foreach (GpcmClient C in Clients.Clients)
                SB.AppendFormat(" {0} ({1}) - {2}{3}", C.ClientNick, C.ClientPID, C.IpAddress, Environment.NewLine);

            // Update connected clients count, and list
            this.Invoke((MethodInvoker)delegate
            {
                ClientCountLabel.Text = "Number of Connected Clients: " + Clients.Clients.Count;
                ConnectedClients.Clear();
                ConnectedClients.Text = SB.ToString();
            });
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
            // Install
            if (!StatsEnabled)
            {
                // Lock the console to prevent errors!
                this.Enabled = false;

                // Remove current Python
                Directory.Delete(ServerPythonPath, true);

                // Make sure we dont have an empty backup folder
                if (Directory.GetFiles(RankedPythonPath).Length == 0)
                    DirectoryExt.Copy(Path.Combine(MainForm.Root, "Python", "Ranked", "Original"), ServerPythonPath, true);
                else
                    DirectoryExt.Copy(RankedPythonPath, ServerPythonPath, true);

                // unlock now that we are done
                this.Enabled = true;
            }

            // Uninstall
            else
            {
                // Lock the console to prevent errors!
                this.Enabled = false;

                // Backup the users bf2s python files
                Directory.Delete(RankedPythonPath, true);
                DirectoryExt.Copy(ServerPythonPath, RankedPythonPath, true);

                // Install default python files
                Directory.Delete(ServerPythonPath, true);
                DirectoryExt.Copy(NonRankedPythonPath, ServerPythonPath, true);

                // Unlock now that we are done
                this.Enabled = true;
            }

            SetInstallStatus();
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
            if (MessageBox.Show(
                    "Restoring the BF2Statistics python files will erase any and all modifications to the BF2Statistics " +
                    "python files. Are you sure you want to continue?",
                    "Confirmation",
                    MessageBoxButtons.OKCancel)
                == DialogResult.OK)
            {
                // Lock the console to prevent errors!
                this.Enabled = false;
                if (StatsEnabled)
                {
                    Directory.Delete(ServerPythonPath, true);
                    DirectoryExt.Copy(Path.Combine(MainForm.Root, "Python", "Ranked", "Original"), ServerPythonPath, true);
                }
                else
                {
                    Directory.Delete(RankedPythonPath, true);
                    DirectoryExt.Copy(Path.Combine(MainForm.Root, "Python", "Ranked", "Original"), RankedPythonPath, true);
                }

                // Show Success Message
                MessageBox.Show("Your Stats python files have been restored successfully.", "Bf2 Statistics Server Launcher");
                this.Enabled = true; // unlcok
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
        /// When the user requests to shuffle the map list, this method makes it happen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShuffleMapListBtn_Click(object sender, EventArgs e)
        {
            // Get our file path and Array of lines
            string MaplistFile = Path.Combine(Config.ServerPath, "mods", SelectedMod, "settings", "maplist.con");
            string[] lines = File.ReadAllLines(MaplistFile);

            // Randomize the array of lines
            new Random().Shuffle<string>(lines);

            // Save randomized lines
            File.WriteAllLines(MaplistFile, lines);
        }

        /// <summary>
        /// Opens the Edit Server Settings Form
        /// </summary>
        private void EditServerSettingsBtn_Click(object sender, EventArgs e)
        {
            string file;
            if (GlobalServerSettings.Checked)
                file = Path.Combine(MainForm.Root, "Python", "GlobalServerSettings.con");
            else
                file = Path.Combine(SettingsPath, "ServerSettings.con");

            try
            {
                ServerSettings SS = new ServerSettings(file);
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
            ScoreSettings SS;
            ScoreSelectForm SSF = new ScoreSelectForm();
            DialogResult R = SSF.ShowDialog();

            // Player
            if (R == DialogResult.Yes)
                SS = new ScoreSettings(true);
            else if (R == DialogResult.No)
                SS = new ScoreSettings(false);
            else
                return;

            try
            {
                // Show score form
                SS.ShowDialog();
            }
            catch { }
        }

        #endregion Server Settings Tab

        #region Hosts File Redirect

        /// <summary>
        /// Checks the HOSTS file on startup, detecting existing redirects to the bf2web.gamespy
        /// or gpcm/gpsp.gamespy urls
        /// </summary>
        private void DoHOSTSCheck()
        {
            bool MatchFound = false;

            if (HostFile.Lines.ContainsKey("gpcm.gamespy.com"))
            {
                MatchFound = true;
                GpcmCheckbox.Checked = true;
                //GpcmAddress.Text = HostFile.Lines["gpcm.gamespy.com"];
            }

            if (HostFile.Lines.ContainsKey("bf2web.gamespy.com"))
            {
                MatchFound = true;
                Bf2webCheckbox.Checked = true;
                //Bf2webAddress.Text = HostFile.Lines["bf2web.gamespy.com"];
            }

            // Did we find any matches?
            if (MatchFound)
            {
                UpdateHostFileStatus("- Found old redirect data in HOSTS file.");
                RedirectsEnabled = true;
                HostsStatusPic.Image = Resources.check;
                LockGroups();

                iButton.Enabled = true;
                iButton.Text = "Remove HOSTS Redirect";

                UpdateHostFileStatus("- Locking HOSTS file");
                HostFile.Lock();
                UpdateHostFileStatus("- All Done!");
            }
        }

        /// <summary>
        /// This is the main HOSTS file button event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void iButton_Click(object sender, EventArgs e)
        {
            // Clear the output window
            LogBox.Clear();

            // If we do not have a redirect in the hosts file...
            if (!RedirectsEnabled)
            {
                // Lines to add to the HOSTS file. [hostname, ipAddress]
                Dictionary<string, string> Lines = new Dictionary<string, string>();

                // Make sure we are going to redirect something...
                if (!Bf2webCheckbox.Checked && !GpcmCheckbox.Checked)
                {
                    MessageBox.Show("Please select at least 1 redirect option", "Error");
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
                            "Error"
                        );
                        UnlockGroups();
                        Bf2webAddress.Focus();
                        return;
                    }

                    // Convert Localhost to the Loopback Address
                    if (text.ToLower() == "localhost")
                        text = IPAddress.Loopback.ToString();

                    // Check if this is an IP address or hostname
                    IPAddress BF2Web;
                    try {
                        BF2Web = GetIpAddress(text);
                    }
                    catch
                    {
                        MessageBox.Show(
                            "Stats server redirect address is invalid, or doesnt exist. Please enter a valid, and existing IPv4/6 or Hostname.",
                            "Error"
                        );

                        UnlockGroups();
                        return;
                    }

                    Lines.Add("bf2web.gamespy.com", BF2Web.ToString());
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
                        MessageBox.Show("You must enter an IP address or Hostname in the Address box!", "Error");
                        UnlockGroups();
                        GpcmAddress.Focus();
                        return;
                    }

                    // Convert Localhost to the Loopback Address
                    if (text2.ToLower() == "localhost")
                        text2 = IPAddress.Loopback.ToString();

                    // Make sure the IP address is valid!
                    IPAddress GpcmA;
                    try {
                        GpcmA = GetIpAddress(text2);
                    }
                    catch
                    {
                        MessageBox.Show(
                            "Login Server redirect address is invalid, or doesnt exist. Please enter a valid, and existing IPv4/6 or Hostname.",
                            "Error"
                        );
                        UnlockGroups();
                        return;
                    }

                    UpdateHostFileStatus("- Adding gpcm.gamespy.com redirect to hosts file");
                    UpdateHostFileStatus("- Adding gpsp.gamespy.com redirect to hosts file");

                    Lines.Add("gpcm.gamespy.com", GpcmA.ToString());
                    Lines.Add("gpsp.gamespy.com", GpcmA.ToString());
                    Config.LastLoginServerAddress = GpcmAddress.Text.Trim();
                }

                // Save last used addresses
                Config.Save();

                // Create new instance of the background worker
                bWorker = new BackgroundWorker();

                // Write the lines to the hosts file
                UpdateHostFileStatus("- Writting to hosts file... ", false);
                bool error = false;
                try
                {
                    // Add lines to the hosts file
                    HostFile.AppendLines(Lines);
                    UpdateHostFileStatus("Success!");

                    // Flush the DNS!
                    FlushDNS();

                    // Do pings, And lock hosts file. We do this in
                    // a background worker so the User can imediatly start
                    // the BF2 client with the HOSTS redirect finishes
                    bWorker.DoWork += new DoWorkEventHandler(RebuildDNSCache);
                    bWorker.RunWorkerAsync();
                }
                catch
                {
                    UpdateHostFileStatus("Failed!");
                    error = true;
                }

                if (!error)
                {
                    // Set form data
                    RedirectsEnabled = true;
                    HostsStatusPic.Image = Resources.check;
                    iButton.Text = "Remove HOSTS Redirect";
                    iButton.Enabled = true;
                }
                else
                {
                    UnlockGroups();
                    MessageBox.Show(
                        "Unable to WRITE to HOSTS file! Please make sure to replace your HOSTS file with " +
                        "the one provided in the release package, or remove your current permissions from the HOSTS file. " +
                        "It may also help to run this program as an administrator.",
                        "Error"
                    );
                }
            }
            else
            {
                // Lock the button
                iButton.Enabled = false;

                // Tell the writter to restore the HOSTS file to its
                // original state
                UpdateHostFileStatus("- Unlocking HOSTS file");
                HostFile.UnLock();

                // Restore the original hosts file contents
                UpdateHostFileStatus("- Restoring HOSTS file... ", false);
                try
                {
                    HostFile.Revert();
                    UpdateHostFileStatus("Success!");
                }
                catch
                {
                    UpdateHostFileStatus("Failed!");
                    MessageBox.Show(
                        "Unable to RESTORE to HOSTS file! Unfortunatly this error can only be fixed by manually removing the HOSTS file,"
                        + " and replacing it with a new one :( . If possible, you may also try changing the permissions yourself.",
                        "Error"
                    );
                }

                // Remove lines
                if (Bf2webCheckbox.Checked)
                    HostFile.Lines.Remove("bf2web.gamespy.com");
                if (GpcmCheckbox.Checked)
                {
                    HostFile.Lines.Remove("gpcm.gamespy.com");
                    HostFile.Lines.Remove("gpsp.gamespy.com");
                }

                // Flush the DNS!
                FlushDNS();

                // Reset form data
                RedirectsEnabled = false;
                HostsStatusPic.Image = Resources.error;
                iButton.Text = "Begin HOSTS Redirect";
                UnlockGroups();

                UpdateHostFileStatus("- All Done!");
            }
        }

        /// <summary>
        /// Method is used to unlock the input fields
        /// </summary>
        private void UnlockGroups()
        {
            iButton.Enabled = true;
            GpcmGroupBox.Enabled = true;
            BF2webGroupBox.Enabled = true;
            Bf2AaGroupBox.Enabled = true;
        }

        /// <summary>
        /// Method is used to lock the input fields while redirect is active
        /// </summary>
        private void LockGroups()
        {
            iButton.Enabled = false;
            GpcmGroupBox.Enabled = false;
            BF2webGroupBox.Enabled = false;
            Bf2AaGroupBox.Enabled = false;
        }

        /// <summary>
        /// Takes a domain name, or IP address, and returns the Correct IP address.
        /// If multiple IP addresses are found, the first one is returned
        /// </summary>
        /// <param name="text">Domain name or IP Address</param>
        /// <returns></returns>
        private IPAddress GetIpAddress(string text)
        {
            // Make sure the IP address is valid!
            IPAddress Address;
            bool isValid = IPAddress.TryParse(text, out Address);

            if (!isValid)
            {
                // Try to get dns value
                IPAddress[] Addresses;
                try
                {
                    UpdateHostFileStatus("- Resolving Hostname: " + text);
                    Addresses = Dns.GetHostAddresses(text);
                }
                catch
                {
                    UpdateHostFileStatus("- Failed to Resolve Hostname!");
                    throw new Exception("Invalid Hostname or IP Address");
                }

                if (Addresses.Length == 0)
                {
                    UpdateHostFileStatus("- Failed to Resolve Hostname!");
                    throw new Exception("Invalid Hostname or IP Address");
                }

                UpdateHostFileStatus("- Found IP: " + Addresses[0]);
                return Addresses[0];
            }

            return Address;
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
            UpdateHostFileStatus("- Rebuilding DNS Cache... ", false);
            foreach (KeyValuePair<String, String> IP in HostFile.Lines)
            {
                Ping p = new Ping();
                PingReply reply = p.Send(IP.Key);
            }
            UpdateHostFileStatus("Done");

            // Lock the hosts file
            UpdateHostFileStatus("- Locking HOSTS file");
            HostFile.Lock();
            UpdateHostFileStatus("- All Done!");
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
                try
                {
                    // Clear out old messages, and set status to a blank light
                    AspStatusBox.Clear();
                    AspStatusPic.Image = Resources.loading;

                    // Start server, and enable the disabled buttons and vice versa
                    ASPServer.Start();
                }
                catch (HttpListenerException E)
                {
                    // Custom port 80 in use message
                    string Message = Environment.NewLine;
                    if (E.ErrorCode == 32)
                        Message += "Port 80 is already in use by another program.";
                    else
                        Message += E.Message;

                    AspStatusBox.Text += Message;
                    AspStatusPic.Image = Resources.warning;
                }
                catch (Exception E)
                {
                    // Check for specific error
                    AspStatusBox.Text += Environment.NewLine + E.Message;
                    AspStatusPic.Image = Resources.warning;
                    ErrorLog.Write(E.Message);
                }
            }
            else
            {
                try {
                    ASPServer.Stop();
                }
                catch(Exception E) {
                    ErrorLog.Write(E.Message);
                }
            }
        }

        /// <summary>
        /// Update the GUI when the ASP starts up
        /// </summary>
        private void ASPServer_OnStart()
        {
            AspStatusPic.Image = Resources.check;
            StartAspServerBtn.Text = "Shutdown ASP Server";
            ViewSnapshotBtn.Enabled = true;
            EditPlayerBtn.Enabled = true;
            EditASPDatabaseBtn.Enabled = false;
            ClearStatsBtn.Enabled = true;
        }

        /// <summary>
        /// Update the GUI when the ASP shutsdown
        /// </summary>
        private void ASPServer_OnShutdown()
        {
            AspStatusPic.Image = Resources.error;
            StartAspServerBtn.Text = "Start ASP Server";
            ViewSnapshotBtn.Enabled = false;
            EditPlayerBtn.Enabled = false;
            EditASPDatabaseBtn.Enabled = true;
            ClearStatsBtn.Enabled = false;
        }

        /// <summary>
        /// View ASP Access Log Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewAccessLogBtn_Click(object sender, EventArgs e)
        {
            Process.Start(Path.Combine(Root, "Logs", "AspAccess.log"));
        }

        /// <summary>
        /// View ASP Error Log Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewErrorLogBtn_Click(object sender, EventArgs e)
        {
            Process.Start(Path.Combine(Root, "Logs", "AspServer.log"));
        }

        /// <summary>
        /// View Snapshot Logs Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewSnapshotLogBtn_Click(object sender, EventArgs e)
        {
            // Make sure the log file exists... It doesnt get created on startup like the others
            string fPath = Path.Combine(Root, "Logs", "StatsDebug.log");
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
        private void ClearStatsBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear the stats database? This will ERASE ALL stats data, and cannot be recovered!",
                "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    ASPServer.Database.Truncate();
                    MessageBox.Show("Database successfully cleared", "Success");
                }
                catch (Exception E)
                {
                    MessageBox.Show("An error occured while clearing the stats database!\r\n\r\nMessage: " + E.Message, "Error");
                }
            }
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
        }

        /// <summary>
        /// Open Program Folder Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenRootBtn_Click(object sender, EventArgs e)
        {
            Process.Start(Root);
        }

        #endregion About Tab

        #region Static Control Methods

        /// <summary>
        /// Static call that can disable the main form
        /// </summary>
        public static void Disable()
        {
            Instance.Enabled = false;
        }

        /// <summary>
        /// Static call that can enable the main form
        /// </summary>
        public static void Enable()
        {
            Instance.Enabled = true;
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
        }

        #endregion Closer Methods

        /// <summary>
        /// Recounts the snapsots when the ASP Stats Server tab is opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 3)
                CountSnapshots();
        }
    }
}
