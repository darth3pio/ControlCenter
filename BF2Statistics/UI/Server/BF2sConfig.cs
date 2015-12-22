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
        /// Path to the python stats folder
        /// </summary>
        private string PythonPath = Path.Combine(Program.Config.ServerPath, "python", "bf2", "stats");

        /// <summary>
        /// Array of medal data files
        /// </summary>
        private List<string> MedalList = new List<string>();

        public BF2sConfig()
        {
            InitializeComponent();

            // Get a list of all medal data
            string[] medalList = Directory.GetFiles(PythonPath, "medal_data_*.py");
            foreach (string file in medalList)
            {
                // Remove the path to the file
                string fileF = file.Remove(0, PythonPath.Length + 1);

                // Remove .py extension, and add it to the list of files
                fileF = fileF.Remove(fileF.Length - 3, 3).Replace("medal_data_", "");
                MedalData.Items.Add(fileF);
                MedalList.Add(fileF);
            }

            // Use the stream reader to load the config
            try
            {
                LoadConfig();
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "Unable to Read/Write to the Bf2Statistics Config python file:" + Environment.NewLine
                    + Environment.NewLine + "Error: " + e.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );

                // Close this form
                this.Load += (s, ev) => Close();
            }
        }

        /// <summary>
        /// Loads the config file, and parses it for all the variables and values
        /// </summary>
        private void LoadConfig()
        {
            // Stats Enabled
            RankMode.SelectedIndex = StatsPython.Config.StatsEnabled ? 1 : 0;

            // Debug Enabled
            Debugging.SelectedIndex = StatsPython.Config.DebugEnabled ? 1 : 0;

            // Snapshot logging
            Logging.SelectedIndex = StatsPython.Config.SnapshotLogging;

            // Snapshot prefix
            SnapshotPrefix.Text = StatsPython.Config.SnapshotPrefix;

            // === Medal Data
            // Determine the selected index based on what the config settings says
            int i = 1;
            string selected = StatsPython.Config.MedalDataProfile;
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

            // ASP Address
            AspAddress.Text = StatsPython.Config.AspAddress.ToString();

            // ASP Port
            AspPort.Value = StatsPython.Config.AspPort;

            // ASP Callback
            AspCallback.Text = StatsPython.Config.AspFile;

            // Central ASP Address
            CentralAddress.Text = StatsPython.Config.CentralAspAddress.ToString();

            // Central ASP Port
            CentralPort.Value = StatsPython.Config.CentralAspPort;

            // Central Callback
            CentralCallback.Text = StatsPython.Config.CentralAspFile;

            // Central Database Enabled
            CentralDatabase.SelectedIndex = StatsPython.Config.CentralStatsMode;

            // CLAN MANAGER
            ClanManager.SelectedIndex = StatsPython.Config.ClanManager.Enabled ? 1 : 0;

            // Server Mode
            CmServerMode.SelectedIndex = StatsPython.Config.ClanManager.ServerMode;

            // === Clan manager array values

            // Clan Tag
            CmClanTag.Text = StatsPython.Config.ClanManager.ClanTagRequirement;

            // Score
            CmGlobalScore.Value = StatsPython.Config.ClanManager.ScoreRequirement;

            // time
            CmGlobalTime.Value = StatsPython.Config.ClanManager.TimeRequirement;

            // K/D Ratio
            CmKDRatio.Value = StatsPython.Config.ClanManager.KDRatioRequirement;

            // Banned
            CmBanCount.Value = StatsPython.Config.ClanManager.MaxBanCount;

            // Country
            CmCountry.Text = StatsPython.Config.ClanManager.CountryRequirement;

            // Rank
            CmMinRank.SelectedIndex = StatsPython.Config.ClanManager.RankRequirement;
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
            StatsPython.Config.StatsEnabled = (RankMode.SelectedIndex == 1);
            StatsPython.Config.DebugEnabled = (Debugging.SelectedIndex == 1);
            StatsPython.Config.SnapshotLogging = Logging.SelectedIndex;
            StatsPython.Config.SnapshotPrefix = SnapshotPrefix.Text;
            StatsPython.Config.MedalDataProfile = data;
            StatsPython.Config.AspAddress = System.Net.IPAddress.Parse(AspAddress.Text);
            StatsPython.Config.CentralAspAddress = System.Net.IPAddress.Parse(CentralAddress.Text);
            StatsPython.Config.AspPort = (int) AspPort.Value;
            StatsPython.Config.CentralAspPort = (int) CentralPort.Value;
            StatsPython.Config.AspFile = AspCallback.Text;
            StatsPython.Config.CentralStatsMode = CentralDatabase.SelectedIndex;
            StatsPython.Config.CentralAspFile = CentralCallback.Text;
            StatsPython.Config.ClanManager.Enabled = (ClanManager.SelectedIndex == 1);
            StatsPython.Config.ClanManager.ServerMode = CmServerMode.SelectedIndex;
            StatsPython.Config.ClanManager.ClanTagRequirement = CmClanTag.Text;
            StatsPython.Config.ClanManager.ScoreRequirement = (int) CmGlobalScore.Value;
            StatsPython.Config.ClanManager.TimeRequirement = (int) CmGlobalTime.Value;
            StatsPython.Config.ClanManager.KDRatioRequirement = CmKDRatio.Value;
            StatsPython.Config.ClanManager.MaxBanCount = (int) CmBanCount.Value;
            StatsPython.Config.ClanManager.CountryRequirement = CmCountry.Text;
            StatsPython.Config.ClanManager.RankRequirement = CmMinRank.SelectedIndex;
            
            // Save File
            StatsPython.Config.Save();
            Notify.Show("Config saved successfully!", "The BF2Statistics config was sucessfully updated");
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

        #endregion

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

        private void XpackMedalsConfigBtn_Click(object sender, EventArgs e)
        {
            using (XpackMedalsConfigForm f = new XpackMedalsConfigForm())
            {
                f.ShowDialog();
            }
        }
    }
}
