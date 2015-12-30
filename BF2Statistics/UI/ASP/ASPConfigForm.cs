using System;
using System.Windows.Forms;

namespace BF2Statistics
{
    public partial class ASPConfigForm : Form
    {
        public ASPConfigForm()
        {
            InitializeComponent();

            // Update for 1.7.0 .. to fix an issue with older versions
            if (Program.Config.ASP_DebugLevel == 0)
            {
                Program.Config.ASP_DebugLevel = 1;
                Program.Config.Save();
            }

            // Set Form Values
            IgnoreAi.SelectedIndex = (Program.Config.ASP_IgnoreAI) ? 1 : 0;
            MinRoundTime.Value = Program.Config.ASP_MinRoundTime;
            MinRoundPlayers.Value = Program.Config.ASP_MinRoundPlayers;
            RankTenure.Value = Program.Config.ASP_SpecialRankTenure;
            SmocProcessing.SelectedIndex = Program.Config.ASP_SmocCheck ? 1 : 0;
            GeneralProcessing.SelectedIndex = Program.Config.ASP_GeneralCheck ? 1 : 0;
            AwdRoundComplete.SelectedIndex = Program.Config.ASP_AwardsReqComplete ? 1 : 0;
            AuthGameServers.Lines = Program.Config.ASP_GameHosts.Split(',');
            UnlocksOption.SelectedIndex = Program.Config.ASP_UnlocksMode;
            DebugLvl.SelectedIndex = Program.Config.ASP_DebugLevel - 1;
        }

        /// <summary>
        /// Save Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveBtn_Click(object sender, EventArgs e)
        {
            // Set Values
            Program.Config.ASP_IgnoreAI = (IgnoreAi.SelectedIndex == 1);
            Program.Config.ASP_MinRoundTime = Int32.Parse(MinRoundTime.Value.ToString());
            Program.Config.ASP_MinRoundPlayers = Int32.Parse(MinRoundPlayers.Value.ToString());
            Program.Config.ASP_SpecialRankTenure = Int32.Parse(RankTenure.Value.ToString());
            Program.Config.ASP_SmocCheck = (SmocProcessing.SelectedIndex == 1);
            Program.Config.ASP_GeneralCheck = (GeneralProcessing.SelectedIndex == 1);
            Program.Config.ASP_AwardsReqComplete = (AwdRoundComplete.SelectedIndex == 1);
            Program.Config.ASP_GameHosts = String.Join(",", AuthGameServers.Lines);
            Program.Config.ASP_UnlocksMode = UnlocksOption.SelectedIndex;
            Program.Config.ASP_DebugLevel = DebugLvl.SelectedIndex + 1;

            // Save Config
            Program.Config.Save();

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
