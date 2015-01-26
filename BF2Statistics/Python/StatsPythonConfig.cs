using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace BF2Statistics
{
    public class StatsPythonConfig
    {
        /// <summary>
        /// Our Bf2StatisticsConfig.py file object
        /// </summary>
        protected FileInfo SettingsFile;

        /// <summary>
        /// The parsable file contents within the SettingsFile
        /// </summary>
        protected string FileContents;

        #region Misc Settings

        /// <summary>
        /// Enables or Disables the debug for the BF2Statistics python scripts
        /// </summary>
        public bool DebugEnabled;

        /// <summary>
        /// Gets or Sets the Snapshot logging mode for the server
        /// </summary>
        public int SnapshotLogging;

        /// <summary>
        /// Gets or sets the Snapshot prefix
        /// </summary>
        public string SnapshotPrefix;

        /// <summary>
        /// Gets or sets the Medal Data Profile
        /// </summary>
        public string MedalDataProfile;

        /// <summary>
        /// Enabes or Disables the forced medal data keystring
        /// </summary>
        public bool ForceMedalKeystring;

        #endregion Misc Settings

        #region ASP

        /// <summary>
        /// Gets or Sets the ASP stats server IP address
        /// </summary>
        public IPAddress AspAddress;

        /// <summary>
        /// Gets or Sets the ASP server port
        /// </summary>
        public int AspPort = 80;

        /// <summary>
        /// Gets or Sets the ASP callback script to post Snapshots to
        /// </summary>
        public string AspFile = "/ASP/bf2statistics.php";

        /// <summary>
        /// Gets or Sets the Central Stats Server Mode
        /// </summary>
        public int CentralStatsMode;

        /// <summary>
        /// Gets or Sets the Central Stats Server Ip Address
        /// </summary>
        public IPAddress CentralAspAddress;

        /// <summary>
        /// Gets or Sets the Central Stats Server Port
        /// </summary>
        public int CentralAspPort = 80;

        /// <summary>
        /// Gets or Sets the Central Stats Server Callback Script to post snapshots to
        /// </summary>
        public string CentralAspFile = "/ASP/bf2statistics.php";

        #endregion ASP

        #region Clan Manager

        /// <summary>
        /// Contains all settings related to the clan manager
        /// </summary>
        public ClanManagerSettings ClanManager = new ClanManagerSettings();

        /// <summary>
        /// A Container for all clan manager settings
        /// </summary>
        public struct ClanManagerSettings
        {
            public bool Enabled;

            public int ServerMode;

            public string ClanTagRequirement;

            public string CountryRequirement;

            public decimal KDRatioRequirement;

            public int ScoreRequirement;

            public int TimeRequirement;

            public int RankRequirement;

            public int MaxBanCount;
        }

        #endregion Clan Manager

        public StatsPythonConfig()
        {
            // Create our file object
            SettingsFile = new FileInfo(Path.Combine(MainForm.Config.ServerPath, "python", "bf2", "BF2StatisticsConfig.py"));

            // Fetch file contents for parsing
            using (Stream Str = SettingsFile.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            using (StreamReader Rdr = new StreamReader(Str))
                FileContents = Rdr.ReadToEnd();

            // Load the config values into the objects variables
            ParseSettings();
        }

        /// <summary>
        /// Parses all the config values into this objects internal variables
        /// </summary>
        protected void ParseSettings()
        {
            Match Match;
            int dummy;

            // Debug Enabled
            Match = Regex.Match(FileContents, @"debug_enable = (?<value>[0-1])");
            if (!Int32.TryParse(Match.Groups["value"].Value, out dummy))
                throw new Exception("The config key \"debug_enabled\" was not formated correctly.");

            DebugEnabled = (dummy == 1);

            // Snapshot logging
            Match = Regex.Match(FileContents, "snapshot_logging = (?<value>[0-2])");
            if (!Int32.TryParse(Match.Groups["value"].Value, out SnapshotLogging))
                throw new Exception("The config key \"snapshot_logging\" was not formated correctly.");

            // Snapshot prefix
            Match = Regex.Match(FileContents, @"snapshot_prefix = '(?<value>[A-Za-z0-9_]+)?'");
            if (!Match.Success)
                throw new Exception("The config key \"snapshot_prefix\" was not formated correctly.");
            
            SnapshotPrefix = Match.Groups["value"].Value;

            // Medal Data
            Match = Regex.Match(FileContents, @"medals_custom_data = '(?<value>[A-Za-z0-9_]*)'");
            if (!Match.Success)
                throw new Exception("The config key \"medals_custom_data\" was not formated correctly.");

            MedalDataProfile = Match.Groups["value"].Value;

            // Force Medal Keystring
            Match = Regex.Match(FileContents, @"medals_force_keystring = (?<value>[0-1])");
            if (!Int32.TryParse(Match.Groups["value"].Value, out dummy))
                throw new Exception("The config key \"medals_force_keystring\" was not formated correctly.");

            ForceMedalKeystring = (dummy == 1);

            // ASP Address
            Match = Regex.Match(FileContents, @"http_backend_addr = '(?<value>.*)'");
            if (!Match.Success)
                throw new Exception("The config key \"http_backend_addr\" was not formated correctly.");
            
            AspAddress = IPAddress.Parse(Match.Groups["value"].Value);

            // ASP Port
            Match = Regex.Match(FileContents, @"http_backend_port = (?<value>[0-9]+)");
            if (!Match.Success || !Int32.TryParse(Match.Groups["value"].Value, out AspPort))
                throw new Exception("The config key \"http_backend_port\" was not formated correctly.");

            // ASP Callback
            Match = Regex.Match(FileContents, @"http_backend_asp = '(?<value>.*)'");
            if (!Match.Success)
                throw new Exception("The config key \"http_backend_asp\" was not formated correctly.");
            
            AspFile = Match.Groups["value"].Value;

            // Central ASP Address
            Match = Regex.Match(FileContents, @"http_central_addr = '(?<value>.*)'");
            if (!Match.Success)
                throw new Exception("The config key \"http_central_addr\" was not formated correctly.");
            
            CentralAspAddress = IPAddress.Parse(Match.Groups["value"].Value);

            // Central ASP Port
            Match = Regex.Match(FileContents, @"http_central_port = (?<value>[0-9]+)");
            if (!Match.Success || !Int32.TryParse(Match.Groups["value"].Value, out CentralAspPort))
                throw new Exception("The config key \"http_central_port\" was not formated correctly.");

            // Central Callback
            Match = Regex.Match(FileContents, @"http_central_asp = '(?<value>.*)'");
            if (!Match.Success)
                throw new Exception("The config key \"http_central_asp\" was not formated correctly.");
            
            CentralAspFile = Match.Groups["value"].Value;

            // Central Database Enabled
            Match = Regex.Match(FileContents, @"http_central_enable = (?<value>[0-2])");
            if (!Int32.TryParse(Match.Groups["value"].Value, out CentralStatsMode))
                throw new Exception("The config key \"http_central_enable\" was not formated correctly.");


            // CLAN MANAGER
            Match = Regex.Match(FileContents, @"enableClanManager = (?<value>[0-1])");
            if (!Int32.TryParse(Match.Groups["value"].Value, out dummy))
                throw new Exception("The config key \"enableClanManager\" was not formated correctly.");
            
            ClanManager.Enabled = (dummy == 1);

            // Server Mode
            Match = Regex.Match(FileContents, @"serverMode = (?<value>[0-4])");
            if (!Int32.TryParse(Match.Groups["value"].Value, out ClanManager.ServerMode))
                throw new Exception("The config key \"serverMode\" was not formated correctly.");

            // Clan manager array values

            // Clan Tag
            Match = Regex.Match(FileContents, @"'clantag',[\s|\t]+'(?<value>[A-Za-z0-9_=-\|\s\[\]]*)'");
            if (!Match.Success)
                throw new Exception("The config key \"criteria_data => clantag\" was not formated correctly.");
            
            ClanManager.ClanTagRequirement = Match.Groups["value"].Value;

            // Score
            Match = Regex.Match(FileContents, @"'score',[\s|\t]+(?<value>[0-9]+)");
            if (!Match.Success)
                throw new Exception("The config key \"criteria_data => score\" was not formated correctly.");
            
            ClanManager.ScoreRequirement = Int32.Parse(Match.Groups["value"].Value);

            // time
            Match = Regex.Match(FileContents, @"'time',[\s|\t]+(?<value>[0-9]+)");
            if (!Match.Success)
                throw new Exception("The config key \"criteria_data => time\" was not formated correctly.");
            
            ClanManager.TimeRequirement = Int32.Parse(Match.Groups["value"].Value);

            // K/D Ratio
            Match = Regex.Match(FileContents, @"'kdratio',[\s|\t]+(?<value>[0-9.]+)");
            if (!Match.Success)
                throw new Exception("The config key \"criteria_data => kdratio\" was not formated correctly.");
            
            ClanManager.KDRatioRequirement = Decimal.Parse(Match.Groups["value"].Value);

            // Banned
            Match = Regex.Match(FileContents, @"'banned',[\s|\t]+(?<value>[0-9]+)");
            if (!Match.Success)
                throw new Exception("The config key \"criteria_data => banned\" was not formated correctly.");
            
            ClanManager.MaxBanCount = Int32.Parse(Match.Groups["value"].Value);

            // Country
            Match = Regex.Match(FileContents, @"'country',[\s|\t]+'(?<value>[A_Za-z]*)'");
            if (!Match.Success)
                throw new Exception("The config key \"criteria_data => country\" was not formated correctly.");
            
            ClanManager.CountryRequirement = Match.Groups["value"].Value;

            // Rank
            Match = Regex.Match(FileContents, @"'rank',[\s|\t]+(?<value>[0-9]+)");
            if (!Int32.TryParse(Match.Groups["value"].Value, out ClanManager.RankRequirement))
                throw new Exception("The config key \"criteria_data => rank\" was not formated correctly.");
        }

        /// <summary>
        /// Saves the current settings to the BF2Statistics.py file
        /// </summary>
        public void Save()
        {
            // Do replacements
            FileContents = Regex.Replace(FileContents, @"debug_enable = ([0-1])", "debug_enable = " + (DebugEnabled ? 1 : 0));
            FileContents = Regex.Replace(FileContents, @"snapshot_logging = ([0-2])", "snapshot_logging = " + SnapshotLogging);
            FileContents = Regex.Replace(FileContents, @"snapshot_prefix = '([A-Za-z0-9_]*)'", String.Format("snapshot_prefix = '{0}'", SnapshotPrefix));
            FileContents = Regex.Replace(FileContents, @"medals_custom_data = '([A-Za-z0-9_]*)'", String.Format("medals_custom_data = '{0}'", MedalDataProfile));
            FileContents = Regex.Replace(FileContents, @"medals_force_keystring = ([0-1])", "medals_force_keystring = " + (ForceMedalKeystring ? 1 : 0));
            FileContents = Regex.Replace(FileContents, @"http_backend_addr = '(.*)'", String.Format("http_backend_addr = '{0}'", AspAddress));
            FileContents = Regex.Replace(FileContents, @"http_central_addr = '(.*)'", String.Format("http_central_addr = '{0}'", CentralAspAddress));
            FileContents = Regex.Replace(FileContents, @"http_backend_port = ([0-9]+)", "http_backend_port = " + AspPort);
            FileContents = Regex.Replace(FileContents, @"http_central_port = ([0-9]+)", "http_central_port = " + CentralAspPort);
            FileContents = Regex.Replace(FileContents, @"http_backend_asp = '(.*)'", String.Format("http_backend_asp = '{0}'", AspFile));
            FileContents = Regex.Replace(FileContents, @"http_central_asp = '(.*)'", String.Format("http_central_asp = '{0}'", CentralAspFile));
            FileContents = Regex.Replace(FileContents, @"enableClanManager = ([0-1])", "enableClanManager = " + (ClanManager.Enabled ? 1 : 0));
            FileContents = Regex.Replace(FileContents, @"serverMode = ([0-4])", "serverMode = " + ClanManager.ServerMode);
            FileContents = Regex.Replace(FileContents, @"'clantag',[\s|\t]+'([A-Za-z0-9_=-\|\s\[\]]*)'", String.Format("'clantag', '{0}'", ClanManager.ClanTagRequirement));
            FileContents = Regex.Replace(FileContents, @"'score',[\s|\t]+([0-9]+)", String.Format("'score', {0}", ClanManager.ScoreRequirement));
            FileContents = Regex.Replace(FileContents, @"'time',[\s|\t]+([0-9]+)", String.Format("'time', {0}", ClanManager.TimeRequirement));
            FileContents = Regex.Replace(FileContents, @"'kdratio',[\s|\t]+([0-9.]+)", String.Format("'kdratio', {0}", ClanManager.KDRatioRequirement));
            FileContents = Regex.Replace(FileContents, @"'banned',[\s|\t]+([0-9]+)", String.Format("'banned', {0}", ClanManager.MaxBanCount));
            FileContents = Regex.Replace(FileContents, @"'country',[\s|\t]+'([A_Za-z]*)'", String.Format("'country', '{0}'", ClanManager.CountryRequirement));
            FileContents = Regex.Replace(FileContents, @"'rank',[\s|\t]+([0-9]+)", String.Format("'rank', {0}", ClanManager.RankRequirement));

            // Save File
            using (Stream Str = SettingsFile.Open(FileMode.Truncate, FileAccess.Write))
            using (StreamWriter Wtr = new StreamWriter(Str))
            {
                Wtr.Write(FileContents);
                Wtr.Flush();
            }
        }
    }
}
