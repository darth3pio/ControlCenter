using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace BF2Statistics
{
    public partial class ServerSettings : Form
    {
        /// <summary>
        /// Our Settings Object, which contains all of our settings
        /// </summary>
        private ServerSettingsParser Settings;

        /// <summary>
        /// Full path to our ServerSettings.con file
        /// </summary>
        private string FileName;

        public ServerSettings(string File)
        {
            InitializeComponent();
            this.FileName = File;

            bool StartUpError = false;

            // First, we try to parse the Settings file
            try {
                Settings = new ServerSettingsParser(File);
            }
            catch(Exception e) {
                MessageBox.Show(e.Message.ToString(), "Server Settings File Error");
                StartUpError = true;
            }

            if (StartUpError)
                this.Close();
            else
                Init();
        }

        #region Methods

        private void Init()
        {
            bool hadError = false;

            // Set all the box and slider values
            try
            {
                // General
                ServerNameBox.Text = Settings.GetValue("serverName");
                ServerPasswordBox.Text = Settings.GetValue("password");
                ServerPortBox.Text = Settings.GetValue("serverPort");
                GamespyPortBox.Text = Settings.GetValue("gameSpyPort");
                ServerWelcomeBox.Text = Settings.GetValue("welcomeMessage");
                SponserLogoBox.Text = Settings.GetValue("communityLogoURL");
                EnablePublicServerBox.Checked = (Int32.Parse(Settings.GetValue("internet")) == 1);
                RoundsPerMapBox.Text = Settings.GetValue("roundsPerMap");
                PlayersToStartSlider.Value = Int32.Parse(Settings.GetValue("numPlayersNeededToStart"));
                PlayersToStartValueBox.Text = PlayersToStartSlider.Value.ToString();

                // Voip
                EnableVoip.Checked = (Int32.Parse(Settings.GetValue("voipEnabled")) == 1);
                VoipClientPort.Text = Settings.GetValue("voipBFClientPort");
                VoipServerPort.Text = Settings.GetValue("voipBFServerPort");
                VoipQualityBar.Value = Int32.Parse(Settings.GetValue("voipQuality"));
                VoipQualityBox.Text = Settings.GetValue("voipQuality");

                // Voting Settings
                EnableVotingBox.Checked = (Int32.Parse(Settings.GetValue("votingEnabled")) == 1);
                EnableTeamVotingBox.Checked = (Int32.Parse(Settings.GetValue("teamVoteOnly")) == 1);
                VoteTimeBar.Value = Int32.Parse(Settings.GetValue("voteTime"));
                VoteTimeBox.Text = Settings.GetValue("voteTime");
                PlayersVotingBar.Value = Int32.Parse(Settings.GetValue("minPlayersForVoting"));
                PlayersVotingBox.Text = Settings.GetValue("minPlayersForVoting");

                // Ratio's and Time settings
                TimeLimitBar.Value = Int32.Parse(Settings.GetValue("timeLimit"));
                TimeLimitBox.Text = Settings.GetValue("timeLimit");
                TicketRatioBar.Value = Int32.Parse(Settings.GetValue("ticketRatio"));
                TicketRatioBox.Text = Settings.GetValue("ticketRatio");
                ScoreLimitBar.Value = Int32.Parse(Settings.GetValue("scoreLimit"));
                ScoreLimitBox.Text = Settings.GetValue("scoreLimit");
                SpawnTimeBar.Value = Int32.Parse(Settings.GetValue("spawnTime"));
                SpawnTimeBox.Text = Settings.GetValue("spawnTime");
                ManDownBar.Value = Int32.Parse(Settings.GetValue("manDownTime"));
                ManDownBox.Text = Settings.GetValue("manDownTime");
                TeamRatioBar.Value = Int32.Parse(Settings.GetValue("teamRatioPercent"));
                TeamRatioBox.Text = Settings.GetValue("teamRatioPercent");

                // Friendly Fire Settigns
                PunishTeamKillsBox.Checked = (Int32.Parse(Settings.GetValue("tkPunishEnabled")) == 1);
                FriendlyFireBox.Checked = (Int32.Parse(Settings.GetValue("friendlyFireWithMines")) == 1);
                SoldierFFBar.Value = Int32.Parse(Settings.GetValue("soldierFriendlyFire"));
                SoldierFFBox.Text = Settings.GetValue("soldierFriendlyFire");
                SoldierSplashFFBar.Value = Int32.Parse(Settings.GetValue("soldierSplashFriendlyFire"));
                SoldierSplashFFBox.Text = Settings.GetValue("soldierSplashFriendlyFire");
                VehicleFFBar.Value = Int32.Parse(Settings.GetValue("vehicleFriendlyFire"));
                VehicleFFBox.Text = Settings.GetValue("vehicleFriendlyFire");
                VehicleSplashFFBar.Value = Int32.Parse(Settings.GetValue("vehicleSplashFriendlyFire"));
                VehicleSplashFFBox.Text = Settings.GetValue("vehicleSplashFriendlyFire");

                // Bot Settings
                BotRatioBar.Value = Int32.Parse(Settings.GetValue("coopBotRatio"));
                BotRatioBox.Text = Settings.GetValue("coopBotRatio");
                BotCountBar.Value = Int32.Parse(Settings.GetValue("coopBotCount"));
                BotCountBox.Text = Settings.GetValue("coopBotCount");
                BotDifficultyBar.Value = Int32.Parse(Settings.GetValue("coopBotDifficulty"));
                BotDifficultyBox.Text = Settings.GetValue("coopBotDifficulty");

                // Misc Settings
                EnablePunkBuster.Checked = (Int32.Parse(Settings.GetValue("punkBuster")) == 1);
                AutoBalanceTeams.Checked = (Int32.Parse(Settings.GetValue("autoBalanceTeam")) == 1);
            }
            catch (Exception e)
            {
                hadError = true;
                MessageBox.Show(e.Message, "Server Settings Format Error");
            }

            if (hadError)
                this.Close();
        }

        #endregion

        #region Events

        private void PlayersToStartSlider_Scroll(object sender, EventArgs e)
        {
            PlayersToStartValueBox.Text = PlayersToStartSlider.Value.ToString();
        }

        private void VoipQualityBar_Scroll(object sender, EventArgs e)
        {
            VoipQualityBox.Text = VoipQualityBar.Value.ToString();
        }

        private void VoteTimeBar_Scroll(object sender, EventArgs e)
        {
            VoteTimeBox.Text = VoteTimeBar.Value.ToString();
        }

        private void PlayersVotingBar_Scroll(object sender, EventArgs e)
        {
            PlayersVotingBox.Text = PlayersVotingBar.Value.ToString();
        }

        private void TimeLimitBar_Scroll(object sender, EventArgs e)
        {
            TimeLimitBox.Text = TimeLimitBar.Value.ToString();
        }

        private void TicketRatioBar_Scroll(object sender, EventArgs e)
        {
            TicketRatioBox.Text = TicketRatioBar.Value.ToString();
        }

        private void ScoreLimitBar_Scroll(object sender, EventArgs e)
        {
            ScoreLimitBox.Text = ScoreLimitBar.Value.ToString();
        }

        private void SpawnTimeBar_Scroll(object sender, EventArgs e)
        {
            SpawnTimeBox.Text = SpawnTimeBar.Value.ToString();
        }

        private void ManDownBar_Scroll(object sender, EventArgs e)
        {
            ManDownBox.Text = ManDownBar.Value.ToString();
        }

        private void TeamRatioBar_Scroll(object sender, EventArgs e)
        {
            TeamRatioBox.Text = TeamRatioBar.Value.ToString();
        }

        private void SoldierFFBar_Scroll(object sender, EventArgs e)
        {
            SoldierFFBox.Text = SoldierFFBar.Value.ToString();
        }

        private void SoldierSplashFFBar_Scroll(object sender, EventArgs e)
        {
            SoldierSplashFFBox.Text = SoldierSplashFFBar.Value.ToString();
        }

        private void VehicleFFBar_Scroll(object sender, EventArgs e)
        {
            VehicleFFBox.Text = VehicleFFBar.Value.ToString();
        }

        private void VehicleSplashFFBar_Scroll(object sender, EventArgs e)
        {
            VehicleSplashFFBox.Text = VehicleSplashFFBar.Value.ToString();
        }

        private void BotRatioBar_Scroll(object sender, EventArgs e)
        {
            BotRatioBox.Text = BotRatioBar.Value.ToString();
        }

        private void BotCountBar_Scroll(object sender, EventArgs e)
        {
            BotCountBox.Text = BotCountBar.Value.ToString();
        }

        private void BotDifficultyBar_Scroll(object sender, EventArgs e)
        {
            BotDifficultyBox.Text = BotDifficultyBar.Value.ToString();
        }

        #endregion

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Event fired when the user wants to save his settings
        /// </summary>
        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveValues();

            string[] lines = new string[Settings.ItemCount()];
            int i = 0;
            int dummy;

            // Write the lines one by one into an array
            Dictionary<string, string> Items = Settings.GetAllSettings();
            foreach (KeyValuePair<string, string> Item in Items)
            {
                string Value = Item.Value.Trim();

                // Determine if the value is a string or number. Strings need wrapped in quotes
                if(!String.IsNullOrEmpty(Value) && Int32.TryParse(Value, out dummy))
                    lines[i] = String.Format("sv.{0} {1}", Item.Key, Value);
                else
                    lines[i] = String.Format("sv.{0} \"{1}\"", Item.Key, Value.Replace(System.Environment.NewLine, "|"));

                i++;
            }

            try
            {
                // Save the file
                File.WriteAllLines(FileName, lines);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Settings File Save Error");
            }

            this.Close();
        }

        /// <summary>
        /// Save's all the forms settings into the ServerSettings.con file
        /// </summary>
        private void SaveValues()
        {
            // General
            Settings.SetValue("serverName", ServerNameBox.Text);
            Settings.SetValue("password", ServerPasswordBox.Text);
            Settings.SetValue("serverPort", ServerPortBox.Text);
            Settings.SetValue("gameSpyPort", GamespyPortBox.Text);
            Settings.SetValue("welcomeMessage", ServerWelcomeBox.Text);
            Settings.SetValue("sponsorText", ServerWelcomeBox.Text);
            Settings.SetValue("communityLogoURL", SponserLogoBox.Text);
            Settings.SetValue("internet", (EnablePublicServerBox.Checked) ? "1" : "0");
            Settings.SetValue("roundsPerMap", RoundsPerMapBox.Text);
            Settings.SetValue("numPlayersNeededToStart", PlayersToStartSlider.Value.ToString());

            // Voip
            Settings.SetValue("voipEnabled", (EnableVoip.Checked) ? "1" : "0");
            Settings.SetValue("voipBFClientPort", VoipClientPort.Text);
            Settings.SetValue("voipBFServerPort", VoipServerPort.Text);
            Settings.SetValue("voipQuality", VoipQualityBar.Value.ToString());

            // Voting
            Settings.SetValue("votingEnabled", (EnableVotingBox.Checked) ? "1" : "0");
            Settings.SetValue("teamVoteOnly", (EnableTeamVotingBox.Checked) ? "1" : "0");
            Settings.SetValue("voteTime", VoteTimeBar.Value.ToString());
            Settings.SetValue("minPlayersForVoting", PlayersVotingBar.Value.ToString());

            // Time limits and ratio's
            Settings.SetValue("timeLimit", TimeLimitBar.Value.ToString());
            Settings.SetValue("ticketRatio", TicketRatioBar.Value.ToString());
            Settings.SetValue("scoreLimit", ScoreLimitBar.Value.ToString());
            Settings.SetValue("spawnTime", SpawnTimeBar.Value.ToString());
            Settings.SetValue("manDownTime", ManDownBar.Value.ToString());
            Settings.SetValue("teamRatioPercent", TeamRatioBar.Value.ToString());

            // Friendly Fire
            Settings.SetValue("tkPunishEnabled", (PunishTeamKillsBox.Checked) ? "1" : "0");
            Settings.SetValue("friendlyFireWithMines", (FriendlyFireBox.Checked) ? "1" : "0");
            Settings.SetValue("soldierFriendlyFire", SoldierFFBar.Value.ToString());
            Settings.SetValue("soldierSplashFriendlyFire", SoldierSplashFFBar.Value.ToString());
            Settings.SetValue("vehicleFriendlyFire", VehicleFFBar.Value.ToString());
            Settings.SetValue("vehicleSplashFriendlyFire", VehicleSplashFFBar.Value.ToString());

            // Bot Settings
            Settings.SetValue("coopBotRatio", BotRatioBar.Value.ToString());
            Settings.SetValue("coopBotCount", BotCountBar.Value.ToString());
            Settings.SetValue("coopBotDifficulty", BotDifficultyBar.Value.ToString());

            // Misc
            Settings.SetValue("punkBuster", (EnablePunkBuster.Checked) ? "1" : "0");
            Settings.SetValue("autoBalanceTeam", (AutoBalanceTeams.Checked) ? "1" : "0");
        }

        #region KeyPress Events

        // Methods below this line prevent any character except numbers from being entered
        // In fields that REQUIRE numbers only, such as ports

        private void ServerPortBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void GamespyPortBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void RoundsPerMapBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void VoipClientPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void VoipServerPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        #endregion
    }
}
