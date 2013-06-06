using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;

namespace BF2Statistics
{
    public partial class BF2sConfig : Form
    {
        /// <summary>
        /// Path to the BF2StatisticsConfig.py file
        /// </summary>
        private string CFile = Path.Combine(MainForm.Config.ServerPath, "python", "bf2", "BF2StatisticsConfig.py");

        /// <summary>
        /// Path to the python stats folder
        /// </summary>
        private string PythonPath = Path.Combine(MainForm.Config.ServerPath, "python", "bf2", "stats");

        /// <summary>
        /// Array of medal data files
        /// </summary>
        private List<string> MedalList = new List<string>();

        /// <summary>
        /// Contents of the BF2StatisticsConfig.py
        /// </summary>
        private string FileContents;

        public BF2sConfig()
        {
            InitializeComponent();

            // Make sure path exists!
            if (!File.Exists(CFile))
            {
                this.Load += new EventHandler(CloseOnStart);
                MessageBox.Show("Bf2Statistics Config python file is missing! Please re-install the stats python", "Error");
                return;
            }

            // Make sure the stats folder exists!
            if (!Directory.Exists(PythonPath))
            {
                this.Load += new EventHandler(CloseOnStart);
                MessageBox.Show("The 'python/stats' folder is missing. Please re-install the stats enabled python before continuing", "Error");
                return;
            }

            // Get a list of all medal data
            int i = 0;
            string[] medalList = Directory.GetFiles(PythonPath, "medal_data_*.py");
            foreach (string file in medalList)
            {
                // Remove the path to the file
                string fileF = file.Remove(0, PythonPath.Length + 1);

                // Dont Add special forces ones
                if (fileF.Contains("_xpack"))
                    continue;

                // Remove .py extension, and add it to the list of files
                fileF = fileF.Remove(fileF.Length - 3, 3).Replace("medal_data_", "");
                MedalData.Items.Add(new Item(fileF, i));
                MedalList.Add(fileF);
                i++;
            }

            LoadConfig();
        }

        /// <summary>
        /// Loads the config file, and parses it for all the variables and values
        /// </summary>
        private void LoadConfig()
        {
            FileContents = File.ReadAllText(CFile);
            Match Match;
            int dummy;

            // Debug Enabled
            Match = Regex.Match(FileContents, @"debug_enable = (?<value>[0-1])");
            if (!Int32.TryParse(Match.Groups["value"].Value, out dummy))
            {
                MessageBox.Show("The config key \"debug_enabled\" was not formated correctly.", "Config Parse Error");
                throw new Exception("");
            }
            Debugging.SelectedIndex = dummy;

            // Snapshot logging
            Match = Regex.Match(FileContents, "snapshot_logging = (?<value>[0-2])");
            if (!Int32.TryParse(Match.Groups["value"].Value, out dummy))
            {
                MessageBox.Show("The config key \"snapshot_logging\" was not formated correctly.", "Config Parse Error");
                throw new Exception("");
            }
            Logging.SelectedIndex = dummy;

            // Snapshot prefix
            Match = Regex.Match(FileContents, @"snapshot_prefix = '(?<value>[A-Za-z0-9_]+)?'");
            if (!Match.Success)
            {
                MessageBox.Show("The config key \"snapshot_prefix\" was not formated correctly.", "Config Parse Error");
                throw new Exception("");
            }
            SnapshotPrefix.Text = Match.Groups["value"].Value;

            // Medal Data
            Match = Regex.Match(FileContents, @"medals_custom_data = '(?<value>[A-Za-z0-9_]*)'");
            if (!Match.Success)
            {
                MessageBox.Show("The config key \"medals_custom_data\" was not formated correctly.", "Config Parse Error");
                throw new Exception("");
            }

            // Determine the selected index based on what the config settings says
            int i = 1;
            string selected = Match.Groups["value"].Value;
            if (String.IsNullOrWhiteSpace(selected))
            {
                MedalData.SelectedIndex = 0;
            }
            else
            {
                foreach (string file in MedalList)
                {
                    if (file == selected)
                    {
                        MedalData.SelectedIndex = i;
                        break;
                    }
                    i++;
                }
            }

            // Force Medal Keystring
            Match = Regex.Match(FileContents, @"medals_force_keystring = (?<value>[0-1])");
            if (!Int32.TryParse(Match.Groups["value"].Value, out dummy))
            {
                MessageBox.Show("The config key \"medals_force_keystring\" was not formated correctly.", "Config Parse Error");
                throw new Exception("");
            }
            ForceKeyString.SelectedIndex = dummy;

            // ASP Address
            Match = Regex.Match(FileContents, @"http_backend_addr = '(?<value>.*)'");
            if (!Match.Success)
            {
                MessageBox.Show("The config key \"http_backend_addr\" was not formated correctly.", "Config Parse Error");
                throw new Exception("");
            }
            AspAddress.Text = Match.Groups["value"].Value;

            // ASP Port
            Match = Regex.Match(FileContents, @"http_backend_port = (?<value>[0-9]+)");
            if (!Match.Success)
            {
                MessageBox.Show("The config key \"http_backend_port\" was not formated correctly.", "Config Parse Error");
                throw new Exception("");
            }
            AspPort.Value = Int32.Parse(Match.Groups["value"].Value);

            // ASP Callback
            Match = Regex.Match(FileContents, @"http_backend_asp = '(?<value>.*)'");
            if (!Match.Success)
            {
                MessageBox.Show("The config key \"http_backend_asp\" was not formated correctly.", "Config Parse Error");
                throw new Exception("");
            }
            AspCallback.Text = Match.Groups["value"].Value;

            // Central ASP Address
            Match = Regex.Match(FileContents, @"http_central_addr = '(?<value>.*)'");
            if (!Match.Success)
            {
                MessageBox.Show("The config key \"http_central_addr\" was not formated correctly.", "Config Parse Error");
                throw new Exception("");
            }
            CentralAddress.Text = Match.Groups["value"].Value;

            // Central ASP Port
            Match = Regex.Match(FileContents, @"http_central_port = (?<value>[0-9]+)");
            if (!Match.Success)
            {
                MessageBox.Show("The config key \"http_central_port\" was not formated correctly.", "Config Parse Error");
                throw new Exception("");
            }
            CentralPort.Value = Int32.Parse(Match.Groups["value"].Value);

            // Central Callback
            Match = Regex.Match(FileContents, @"http_central_asp = '(?<value>.*)'");
            if (!Match.Success)
            {
                MessageBox.Show("The config key \"http_central_asp\" was not formated correctly.", "Config Parse Error");
                throw new Exception("");
            }
            CentralCallback.Text = Match.Groups["value"].Value;

            // Central Database Enabled
            Match = Regex.Match(FileContents, @"http_central_enable = (?<value>[0-2])");
            if (!Int32.TryParse(Match.Groups["value"].Value, out dummy))
            {
                MessageBox.Show("The config key \"http_central_enable\" was not formated correctly.", "Config Parse Error");
                throw new Exception("");
            }
            CentralDatabase.SelectedIndex = dummy;


            // CLAN MANAGER
            Match = Regex.Match(FileContents, @"enableClanManager = (?<value>[0-1])");
            if (!Int32.TryParse(Match.Groups["value"].Value, out dummy))
            {
                MessageBox.Show("The config key \"enableClanManager\" was not formated correctly.", "Config Parse Error");
                throw new Exception("");
            }
            ClanManager.SelectedIndex = dummy;

            // Server Mode
            Match = Regex.Match(FileContents, @"serverMode = (?<value>[0-4])");
            if (!Int32.TryParse(Match.Groups["value"].Value, out dummy))
            {
                MessageBox.Show("The config key \"serverMode\" was not formated correctly.", "Config Parse Error");
                throw new Exception("");
            }
            CmServerMode.SelectedIndex = dummy;

            // Clan manager array values

            // Clan Tag
            Match = Regex.Match(FileContents, @"'clantag',[\s|\t]+'(?<value>[A-Za-z0-9_=-\|\s\[\]]*)'");
            if (!Match.Success)
            {
                MessageBox.Show("The config key \"criteria_data => clantag\" was not formated correctly.", "Config Parse Error");
                throw new Exception("");
            }
            CmClanTag.Text = Match.Groups["value"].Value;

            // Score
            Match = Regex.Match(FileContents, @"'score',[\s|\t]+(?<value>[0-9]+)");
            if (!Match.Success)
            {
                MessageBox.Show("The config key \"criteria_data => score\" was not formated correctly.", "Config Parse Error");
                throw new Exception("");
            }
            CmGlobalScore.Value = Int32.Parse(Match.Groups["value"].Value);

            // time
            Match = Regex.Match(FileContents, @"'time',[\s|\t]+(?<value>[0-9]+)");
            if (!Match.Success)
            {
                MessageBox.Show("The config key \"criteria_data => time\" was not formated correctly.", "Config Parse Error");
                throw new Exception("");
            }
            CmGlobalTime.Value = Int32.Parse(Match.Groups["value"].Value);

            // K/D Ratio
            Match = Regex.Match(FileContents, @"'kdratio',[\s|\t]+(?<value>[0-9.]+)");
            if (!Match.Success)
            {
                MessageBox.Show("The config key \"criteria_data => kdratio\" was not formated correctly.", "Config Parse Error");
                throw new Exception("");
            }
            CmKDRatio.Value = decimal.Parse(Match.Groups["value"].Value);

            // Banned
            Match = Regex.Match(FileContents, @"'banned',[\s|\t]+(?<value>[0-9]+)");
            if (!Match.Success)
            {
                MessageBox.Show("The config key \"criteria_data => banned\" was not formated correctly.", "Config Parse Error");
                throw new Exception("");
            }
            CmBanCount.Value = Int32.Parse(Match.Groups["value"].Value);

            // Country
            Match = Regex.Match(FileContents, @"'country',[\s|\t]+'(?<value>[A_Za-z]*)'");
            if (!Match.Success)
            {
                MessageBox.Show("The config key \"criteria_data => country\" was not formated correctly.", "Config Parse Error");
                throw new Exception("");
            }
            CmCountry.Text = Match.Groups["value"].Value;

            // Rank
            Match = Regex.Match(FileContents, @"'rank',[\s|\t]+(?<value>[0-9]+)");
            if (!Int32.TryParse(Match.Groups["value"].Value, out dummy))
            {
                MessageBox.Show("The config key \"criteria_data => rank\" was not formated correctly.", "Config Parse Error");
                throw new Exception("");
            }
            CmMinRank.SelectedIndex = dummy;
        }

        #region Events

        /// <summary>
        /// Closes the form
        /// </summary>
        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Saves the current settings to the BF2Statistics.py file
        /// </summary>
        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Medal Data parsing
            string data = "";
            if (MedalData.Text != "Default")
                data = MedalData.Text;

            // Do replacements
            FileContents = Regex.Replace(FileContents, @"debug_enable = ([0-1])", "debug_enable = " + Debugging.SelectedIndex);
            FileContents = Regex.Replace(FileContents, @"snapshot_logging = ([0-2])", "snapshot_logging = " + Logging.SelectedIndex);
            FileContents = Regex.Replace(FileContents, @"snapshot_prefix = '([A-Za-z0-9_]*)'", String.Format("snapshot_prefix = '{0}'", SnapshotPrefix.Text));
            FileContents = Regex.Replace(FileContents, @"medals_custom_data = '([A-Za-z0-9_]*)'", String.Format("medals_custom_data = '{0}'", data));
            FileContents = Regex.Replace(FileContents, @"medals_force_keystring = ([0-1])", "medals_force_keystring = " + ForceKeyString.SelectedIndex);
            FileContents = Regex.Replace(FileContents, @"http_backend_addr = '(.*)'", String.Format("http_backend_addr = '{0}'", AspAddress.Text));
            FileContents = Regex.Replace(FileContents, @"http_central_addr = '(.*)'", String.Format("http_central_addr = '{0}'", CentralAddress.Text));
            FileContents = Regex.Replace(FileContents, @"http_backend_port = ([0-9]+)", "http_backend_port = " + AspPort.Value);
            FileContents = Regex.Replace(FileContents, @"http_central_port = ([0-9]+)", "http_central_port = " + CentralPort.Value);
            FileContents = Regex.Replace(FileContents, @"http_backend_asp = '(.*)'", String.Format("http_backend_asp = '{0}'", AspCallback.Text));
            FileContents = Regex.Replace(FileContents, @"http_central_asp = '(.*)'", String.Format("http_central_asp = '{0}'", CentralCallback.Text));
            FileContents = Regex.Replace(FileContents, @"enableClanManager = ([0-1])", "enableClanManager = " + ClanManager.SelectedIndex);
            FileContents = Regex.Replace(FileContents, @"serverMode = ([0-4])", "serverMode = " + CmServerMode.SelectedIndex);
            FileContents = Regex.Replace(FileContents, @"'clantag',[\s|\t]+'([A-Za-z0-9_=-\|\s\[\]]*)'", String.Format("'clantag', '{0}'", CmClanTag.Text));
            FileContents = Regex.Replace(FileContents, @"'score',[\s|\t]+([0-9]+)", String.Format("'score', {0}", CmGlobalScore.Value));
            FileContents = Regex.Replace(FileContents, @"'time',[\s|\t]+([0-9]+)", String.Format("'time', {0}", CmGlobalTime.Value));
            FileContents = Regex.Replace(FileContents, @"'kdratio',[\s|\t]+([0-9.]+)", String.Format("'kdratio', {0}", CmKDRatio.Value));
            FileContents = Regex.Replace(FileContents, @"'banned',[\s|\t]+([0-9]+)", String.Format("'banned', {0}", CmGlobalTime.Value));
            FileContents = Regex.Replace(FileContents, @"'country',[\s|\t]+'([A_Za-z]*)'", String.Format("'country', '{0}'", CmCountry.Text));
            FileContents = Regex.Replace(FileContents, @"'rank',[\s|\t]+([0-9]+)", String.Format("'rank', {0}", CmMinRank.SelectedIndex));
            File.WriteAllText(CFile, FileContents);
            this.Close();
        }

        private void CentralDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CentralDatabase.SelectedIndex == 0)
            {
                CentralAddress.Enabled = false;
                CentralCallback.Enabled = false;
                CentralPort.Enabled = false;
            }
            else
            {
                CentralAddress.Enabled = true;
                CentralCallback.Enabled = true;
                CentralPort.Enabled = true;
            }
        }

        #endregion

        private void ClanManager_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ClanManager.SelectedIndex == 0)
            {
                CmBanCount.Enabled = false;
                CmClanTag.Enabled = false;
                CmCountry.Enabled = false;
                CmGlobalScore.Enabled = false;
                CmGlobalTime.Enabled = false;
                CmKDRatio.Enabled = false;
                CmMinRank.Enabled = false;
                CmServerMode.Enabled = false;
            }
            else
            {
                CmBanCount.Enabled = true;
                CmClanTag.Enabled = true;
                CmCountry.Enabled = true;
                CmGlobalScore.Enabled = true;
                CmGlobalTime.Enabled = true;
                CmKDRatio.Enabled = true;
                CmMinRank.Enabled = true;
                CmServerMode.Enabled = true;
            }
        }

        #region Validations

        private void CmClanTag_Validating(object sender, CancelEventArgs e)
        {
            if (!Validator.IsValidClanTag(CmClanTag.Text.Trim()))
            {
                MessageBox.Show("Invalid format for Clan Tag. Must only contain characters( A-Z 0-9 _-=|[] )!", "Validation Error");
            }
        }

        private void CmCountry_Validating(object sender, CancelEventArgs e)
        {
            if (!Validator.IsAlphaOnly(CmCountry.Text))
            {
                MessageBox.Show("Invalid format for Criteria > Country. Must contain letters only!", "Validation Error");
            }
        }

        private void SnapshotPrefix_Validating(object sender, CancelEventArgs e)
        {
            if (!Validator.IsValidPrefix(SnapshotPrefix.Text))
            {
                MessageBox.Show("Invalid format for Snapshot Prefix. Must only characters: ( a-z0-9._-=[] )!", "Validation Error");
            }
        }

        #endregion

        /// <summary>
        /// Event closes the form when fired
        /// </summary>
        private void CloseOnStart(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
