using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BF2Statistics.Properties;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace BF2Statistics
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// The User Config
        /// </summary>
        public static Settings Config = Settings.Default;

        /// <summary>
        /// Startup root directory
        /// </summary>
        public static string Root = Application.StartupPath;

        /// <summary>
        /// An array of found mods
        /// </summary>
        public static Dictionary<string, string> InstalledMods = new Dictionary<string, string>();

        /// <summary>
        /// The current selected mod foldername
        /// </summary>
        public static string SelectedMod = "";

        /// <summary>
        /// Is Stats Enabled?
        /// </summary>
        public static bool StatsEnabled;

        /// <summary>
        /// Is the BF2Stats folder existant in the server root?
        /// </summary>
        public static bool BF2StatsFolderExists;

        /// <summary>
        /// The Battlefield 2 server process (when running)
        /// </summary>
        private Process ServerProccess;

        /// <summary>
        /// Root path to the bf2statistics folder
        /// </summary>
        public static string Bf2statisticsPath { get; protected set; }

        /// <summary>
        /// Full path to the current selected mod's settings folder
        /// </summary>
        public static string SettingsPath { get; protected set; }

        /// <summary>
        /// The bf2 python path
        /// </summary>
        public static string PythonPath { get; protected set; }

        /// <summary>
        /// Full path to the stats python files
        /// </summary>
        public static string StatsPythonPath { get; protected set; }

        /// <summary>
        /// Full path to the backup python files
        /// </summary>
        public static string BackupPythonPath { get; protected set; }

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

        public MainForm()
        {
            InitializeComponent();

            // Check for needed settings upgrade
            if (!Config.SettingsUpdated)
            {
                Config.Upgrade();
                Config.SettingsUpdated = true;
                Config.Save();
            }

            // If this is the first run, Get client and server install paths
            if (String.IsNullOrWhiteSpace(Config.ClientPath))
            {
                InstallForm IS = new InstallForm();
                if (IS.ShowDialog() != DialogResult.OK)
                {
                    this.Load += new EventHandler(MyForm_CloseOnStart);
                    return;
                }
            }

            // Define BF2Statistics Path
            Bf2statisticsPath = Path.Combine(Config.ServerPath, "bf2statistics");

            // Define python paths
            PythonPath = Path.Combine(Config.ServerPath, "python", "bf2");
            BackupPythonPath = Path.Combine(Bf2statisticsPath, "python", "original");
            StatsPythonPath = Path.Combine(Bf2statisticsPath, "python", "bf2statistics");

            // Load installed Mods
            LoadModList();

            // Set BF2Statistics Install Status
            SetInstallStatus();

            // Set whether the bf2statistics system is detected
            BF2StatsFolderExists = Directory.Exists(Bf2statisticsPath);

            // If the BF2s folder doesnt exist, lock its install buttons and such
            if (!BF2StatsFolderExists)
            {
                InstallButton.Enabled = false;
                InstallBox.Text = "BF2Statistics folder is not found!";
                InstallBox.ForeColor = Color.Red;
                BF2sConfigBtn.Enabled = false;
                BF2sRestoreBtn.Enabled = false;
                GlobalServerSettings.Enabled = false;
                EditScoreSettingsBtn.Enabled = false;
            }

            // Check if the server is already running
            CheckServerProcess();

            // Try to access the hosts file
            try {
                // Do hosts file check for existing redirects
                HostFile = new HostsFile();
                DoHOSTSCheck();
            }
            catch (Exception e) {
                HostsStatusPic.Image = Properties.Resources.amber;
                MessageBox.Show(e.Message, "Error");
            }
            

            // Register for login server events
            LoginServer.OnShutdown += new ShutdownEventHandler(LoginServer_OnShutdown);
            LoginServer.OnUpdate += new EventHandler(LoginServer_OnUpdate);
        }

        /// <summary>
        /// Event closes the form when fired
        /// </summary>
        private void MyForm_CloseOnStart(object sender, EventArgs e)
        {
            this.Close();
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
                BF2sRestoreBtn.Enabled = false;
                StatsStatusPic.Image = Properties.Resources.green;
            }
            else
            {
                StatsEnabled = false;
                InstallBox.ForeColor = Color.Red;
                InstallBox.Text = "BF2 Statistics server files are currently NOT installed";
                InstallButton.Text = "Install BF2 Statistics Python";
                BF2sConfigBtn.Enabled = false;
                BF2sRestoreBtn.Enabled = true;
                StatsStatusPic.Image = Properties.Resources.red;
            }
        }

        /// <summary>
        /// Loads up all the supported mods, and adds them to the Mod select list
        /// </summary>
        private void LoadModList()
        {
            string path = Path.Combine(Config.ClientPath, "mods");
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
                this.Close();
            }
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
                    ServerStatusPic.Image = Properties.Resources.green;
                    LaunchServerBtn.Text = "Shutdown Server";
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
            DateTime datet = DateTime.Now;
            String logFile = Path.Combine(Root, "Logs", "error.log");
            if (!File.Exists(logFile))
            {
                FileStream files = File.Create(logFile);
                files.Close();
            }
            try
            {
                StreamWriter sw = File.AppendText(logFile);
                sw.WriteLine(datet.ToString("MM/dd hh:mm") + "> " + message);
                sw.Flush();
                sw.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
        }

        /// <summary>
        /// This method is used to store a message in the console.log file
        /// </summary>
        /// <param name="message">The message to be written to the log file</param>
        public static void Log(string message, params object[] items)
        {
            Log(String.Format(message, items));
        }

        #endregion Startup Methods

        #region Status OnClick Events

        private void HostsFileStatusLabel_DoubleClick(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
        }

        private void LoginStatusDesc_DoubleClick(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }

        private void StatsStatusDesc_DoubleClick(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 4;
        }

        private void ServerStatusDesc_DoubleClick(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        #endregion Status OnClick Events

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
                    LoginServer.Start(EmuStatusWindow);
                    LoginStatusPic.Image = Properties.Resources.green;
                    LaunchEmuBtn.Text = "Shutdown Login Server";
                    CreateAcctBtn.Enabled = true;
                    EditAcctBtn.Enabled = true;
                }
                catch
                {
                    LoginStatusPic.Image = Properties.Resources.amber;
                }
            }
            else
            {
                LoginServer.Shutdown();
                ConnectedClients.Clear();
                ClientCountLabel.Text = "Number of Connected Clients: 0";
                LaunchEmuBtn.Text = "Start Login Server";
                CreateAcctBtn.Enabled = false;
                EditAcctBtn.Enabled = false;
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
                LoginStatusPic.Image = Properties.Resources.red;
                LaunchEmuBtn.Text = "Start Login Server";
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
                if (GlobalServerSettings.Checked && BF2StatsFolderExists)
                    Info.Arguments += " +config " + Path.Combine(Config.ServerPath, "bf2statistics", "ServerSettings.con");

                // Moniter Con Files?
                if (FileMoniter.Checked)
                    Info.Arguments += " +fileMonitor 1";

                // Ignore Asserts? (Non-Fetal Startup Errors)
                if (IgnoreAsserts.Checked)
                    Info.Arguments += " +ignoreAsserts 1";

                // Hide window if user specifies this...
                if (!ShowConsole.Checked)
                    Info.WindowStyle = ProcessWindowStyle.Hidden;
                else
                    Info.WindowStyle = ProcessWindowStyle.Minimized;

                Info.FileName = "bf2_w32ded.exe";
                Info.WorkingDirectory = Config.ServerPath;
                ServerProccess = Process.Start(Info);

                // Hook into the proccess so we know when its running, and register a closing event
                ServerProccess.EnableRaisingEvents = true;
                ServerProccess.Exited += new EventHandler(BF2_Exited);

                // Set status to online
                ServerStatusPic.Image = Properties.Resources.green;
                LaunchServerBtn.Text = "Shutdown Server";
            }
            else
            {
                try
                {
                    ServerProccess.Kill();
                    ServerProccess = null;
                    ServerStatusPic.Image = Properties.Resources.red;
                    LaunchServerBtn.Text = "Launch Server";
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
                ServerStatusPic.Image = Properties.Resources.red;
                LaunchServerBtn.Text = "Launch Server";
                ServerProccess = null;
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

            UpdateClientCount("Number of Connected Clients: " + Clients.Clients.Count);
            UpdateClientList(SB.ToString());
        }

        /// <summary>
        /// Updates the client count label
        /// </summary>
        /// <param name="text"></param>
        public void UpdateClientCount(string text)
        {
            if (ClientCountLabel.InvokeRequired)
                this.Invoke(new Action<string>(UpdateClientCount), new object[] { text });
            else
                ClientCountLabel.Text = text;
        }

        /// <summary>
        /// Updates the connected  clients list
        /// </summary>
        /// <param name="list"></param>
        public void UpdateClientList(string list)
        {
            if (ConnectedClients.InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateClientList), new object[] { list });
            }
            else
            {
                ConnectedClients.Clear();
                ConnectedClients.Text = list;
            }
        }

        private void CreateAcctBtn_Click(object sender, EventArgs e)
        {
            CreateAcctForm Form = new CreateAcctForm();
            Form.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            GamespyConfig Form = new GamespyConfig();
            Form.ShowDialog();
        }

        private void EditAcctBtn_Click(object sender, EventArgs e)
        {
            EditAcctForm Form = new EditAcctForm();
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

                // Backup the original files
                Directory.Delete(BackupPythonPath, true);
                Directory.Move(PythonPath, BackupPythonPath);

                // Install new BF2s Python
                DirectoryHelper.Copy(StatsPythonPath, PythonPath, true);

                // unlock now that we are done
                this.Enabled = true;
            }

            // Uninstall
            else
            {
                // Lock the console to prevent errors!
                this.Enabled = false;

                // Backup the users bf2s python files
                Directory.Delete(StatsPythonPath, true);
                Directory.Move(PythonPath, StatsPythonPath);

                // Install default python files
                DirectoryHelper.Copy(BackupPythonPath, PythonPath, true);

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
                Directory.Delete(StatsPythonPath, true);
                DirectoryHelper.Copy(Path.Combine(Bf2statisticsPath, "python", "bf2statistics_original"), StatsPythonPath, true);
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
                file = Path.Combine(Bf2statisticsPath, "ServerSettings.con");
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
                GpcmAddress.Text = HostFile.Lines["gpcm.gamespy.com"];
            }

            if (HostFile.Lines.ContainsKey("bf2web.gamespy.com"))
            {
                MatchFound = true;
                Bf2webCheckbox.Checked = true;
                Bf2webAddress.Text = HostFile.Lines["bf2web.gamespy.com"];
            }

            // Did we find any matches?
            if (MatchFound)
            {
                UdpateStatus("- Found old redirect data in HOSTS file.");
                RedirectsEnabled = true;
                HostsStatusPic.Image = Properties.Resources.green;
                LockGroups();

                iButton.Enabled = true;
                iButton.Text = "Remove HOSTS Redirect";

                UdpateStatus("- Locking HOSTS file");
                HostFile.Lock();
                UdpateStatus("- All Done!");
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
                    try
                    {
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
                    UdpateStatus("- Adding bf2web.gamespy.com redirect to hosts file");
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
                    try
                    {
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

                    UdpateStatus("- Adding gpcm.gamespy.com redirect to hosts file");
                    UdpateStatus("- Adding gpsp.gamespy.com redirect to hosts file");

                    Lines.Add("gpcm.gamespy.com", GpcmA.ToString());
                    Lines.Add("gpsp.gamespy.com", GpcmA.ToString());
                }

                // Create new instance of the background worker
                bWorker = new BackgroundWorker();

                // Write the lines to the hosts file
                UpdateStatus("- Writting to hosts file... ", false);
                bool error = false;
                try
                {
                    // Add lines to the hosts file
                    HostFile.AppendLines(Lines);
                    UdpateStatus("Success!");

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
                    UdpateStatus("Failed!");
                    error = true;
                }

                if (!error)
                {
                    // Set form data
                    RedirectsEnabled = true;
                    HostsStatusPic.Image = Properties.Resources.green;
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
                UdpateStatus("- Unlocking HOSTS file");
                HostFile.UnLock();

                // Restore the original hosts file contents
                UpdateStatus("- Restoring HOSTS file... ", false);
                try
                {
                    HostFile.Revert();
                    UdpateStatus("Success!");
                }
                catch
                {
                    UdpateStatus("Failed!");
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
                HostsStatusPic.Image = Properties.Resources.red;
                iButton.Text = "Begin HOSTS Redirect";
                UnlockGroups();

                UdpateStatus("- All Done!");
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
                    UdpateStatus("- Resolving Hostname: " + text);
                    Addresses = Dns.GetHostAddresses(text);
                }
                catch
                {
                    UdpateStatus("- Failed to Resolve Hostname!");
                    throw new Exception("Invalid Hostname or IP Address");
                }

                if (Addresses.Length == 0)
                {
                    UdpateStatus("- Failed to Resolve Hostname!");
                    throw new Exception("Invalid Hostname or IP Address");
                }

                UdpateStatus("- Found IP: " + Addresses[0]);
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
        void RebuildDNSCache(object sender, DoWorkEventArgs e)
        {
            UpdateStatus("- Rebuilding DNS Cache... ", false);
            foreach (KeyValuePair<String, String> IP in HostFile.Lines)
            {
                Ping p = new Ping();
                PingReply reply = p.Send(IP.Key);
            }
            UdpateStatus("Done");

            // Lock the hosts file
            UdpateStatus("- Locking HOSTS file");
            HostFile.Lock();
            UdpateStatus("- All Done!");
        }

        /// <summary>
        /// Adds a new line to the "status" window on the GUI
        /// </summary>
        /// <param name="message">The message to print</param>
        public void UdpateStatus(string message)
        {
            UpdateStatus(message, true);
        }

        /// <summary>
        /// Adds a new line to the "status" window on the GUI
        /// </summary>
        /// <param name="message">The message to print</param>
        /// <param name="newLine">Add a new line for the next message?</param>
        public void UpdateStatus(string message, bool newLine)
        {
            if (LogBox.InvokeRequired)
            {
                this.Invoke(new Action<string, bool>(UpdateStatus), new object[] { message, newLine });
            }
            else
            {
                if (newLine)
                    message = message + Environment.NewLine;
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
            UdpateStatus("- Flushing DNS Cache");
            DnsFlushResolverCache();
        }

        [System.Runtime.InteropServices.DllImport("dnsapi.dll", EntryPoint = "DnsFlushResolverCache")]
        private static extern UInt32 DnsFlushResolverCache();

        #endregion Hosts File Redirect

        private void Bf2StatisticsLink_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.bf2statistics.com/");
        }

        private void SetupBtn_Click(object sender, EventArgs e)
        {
            InstallForm IS = new InstallForm();
            IS.ShowDialog();
        }
    }
}
