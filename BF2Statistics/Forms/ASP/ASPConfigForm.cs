using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BF2Statistics
{
    public partial class ASPConfigForm : Form
    {
        public ASPConfigForm()
        {
            InitializeComponent();

            // Update for 1.7.0 .. to fix an issue with older versions
            if (MainForm.Config.ASP_DebugLevel == 0)
            {
                MainForm.Config.ASP_DebugLevel = 1;
                MainForm.Config.Save();
            }

            // Set Form Values
            IgnoreAi.SelectedIndex = (MainForm.Config.ASP_IgnoreAI) ? 1 : 0;
            MinRoundTime.Value = MainForm.Config.ASP_MinRoundTime;
            MinRoundPlayers.Value = MainForm.Config.ASP_MinRoundPlayers;
            RankChecking.SelectedIndex = (MainForm.Config.ASP_StatsRankCheck) ? 1 : 0;
            RankTenure.Value = MainForm.Config.ASP_SpecialRankTenure;
            SmocProcessing.SelectedIndex = MainForm.Config.ASP_SmocCheck ? 1 : 0;
            GeneralProcessing.SelectedIndex = MainForm.Config.ASP_GeneralCheck ? 1 : 0;
            AwdRoundComplete.SelectedIndex = MainForm.Config.ASP_AwardsReqComplete ? 1 : 0;
            AuthGameServers.Lines = MainForm.Config.ASP_GameHosts.Split(',');
            OfflinePid.Value = MainForm.Config.ASP_DefaultPID;
            UnlocksOption.SelectedIndex = MainForm.Config.ASP_UnlocksMode;
            DebugLvl.SelectedIndex = MainForm.Config.ASP_DebugLevel - 1;
        }

        /// <summary>
        /// Save Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveBtn_Click(object sender, EventArgs e)
        {
            // Set Values
            MainForm.Config.ASP_IgnoreAI = (IgnoreAi.SelectedIndex == 1);
            MainForm.Config.ASP_MinRoundTime = Int32.Parse(MinRoundTime.Value.ToString());
            MainForm.Config.ASP_MinRoundPlayers = Int32.Parse(MinRoundPlayers.Value.ToString());
            MainForm.Config.ASP_StatsRankCheck = (RankChecking.SelectedIndex == 1);
            MainForm.Config.ASP_SpecialRankTenure = Int32.Parse(RankTenure.Value.ToString());
            MainForm.Config.ASP_SmocCheck = (SmocProcessing.SelectedIndex == 1);
            MainForm.Config.ASP_GeneralCheck = (GeneralProcessing.SelectedIndex == 1);
            MainForm.Config.ASP_AwardsReqComplete = (AwdRoundComplete.SelectedIndex == 1);
            MainForm.Config.ASP_GameHosts = String.Join(",", AuthGameServers.Lines);
            MainForm.Config.ASP_DefaultPID = Int32.Parse(OfflinePid.Value.ToString());
            MainForm.Config.ASP_UnlocksMode = UnlocksOption.SelectedIndex;
            MainForm.Config.ASP_DebugLevel = DebugLvl.SelectedIndex + 1;

            // Save Config
            MainForm.Config.Save();

            // Close the Form
            this.Close();
        }

        /// <summary>
        /// Cancel Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
