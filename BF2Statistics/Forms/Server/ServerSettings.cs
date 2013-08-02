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
        /// Constructor
        /// </summary>
        /// <param name="File">The full path to the ServerSettings.con</param>
        public ServerSettings(string File)
        {
            InitializeComponent();

            // Parse settings file, and fill in form values
            try 
            {
                // Load Settings
                this.Settings = new ServerSettingsParser(File);

                // General
                ServerNameBox.Text = Settings.GetValue("serverName");
                ServerPasswordBox.Text = Settings.GetValue("password");
                ServerIpBox.Text = Settings.GetValue("serverIP");
                ServerPortBox.Value = Int32.Parse(Settings.GetValue("serverPort"));
                GamespyPortBox.Value = Int32.Parse(Settings.GetValue("gameSpyPort"));
                ServerWelcomeBox.Text = Settings.GetValue("welcomeMessage");
                AutoBalanceBox.Checked = (Int32.Parse(Settings.GetValue("autoBalanceTeam")) == 1);
                EnablePublicServerBox.Checked = (Int32.Parse(Settings.GetValue("internet")) == 1);
                EnablePunkBuster.Checked = (Int32.Parse(Settings.GetValue("punkBuster")) == 1);
                RoundsPerMapBox.Value = Int32.Parse(Settings.GetValue("roundsPerMap"));
                PlayersToStartSlider.Value = Int32.Parse(Settings.GetValue("numPlayersNeededToStart"));
                MaxPlayersBar.Value = Int32.Parse(Settings.GetValue("maxPlayers"));
                TicketRatioBar.Value = Int32.Parse(Settings.GetValue("ticketRatio"));
                ScoreLimitBar.Value = Int32.Parse(Settings.GetValue("scoreLimit"));
                TeamRatioBar.Value = Int32.Parse(Settings.GetValue("teamRatioPercent"));

                // Time settings
                TimeLimitBar.Value = Int32.Parse(Settings.GetValue("timeLimit"));
                SpawnTimeBar.Value = Int32.Parse(Settings.GetValue("spawnTime"));
                ManDownBar.Value = Int32.Parse(Settings.GetValue("manDownTime"));
                StartDelayBar.Value = Int32.Parse(Settings.GetValue("startDelay"));
                EndDelayBar.Value = Int32.Parse(Settings.GetValue("endDelay"));
                EORBar.Value = Int32.Parse(Settings.GetValue("endOfRoundDelay"));
                NotEnoughPlayersBar.Value = Int32.Parse(Settings.GetValue("notEnoughPlayersRestartDelay"));
                TimeB4RestartMapBar.Value = Int32.Parse(Settings.GetValue("timeBeforeRestartMap"));

                // Friendly Fire Settings
                PunishTeamKillsBox.Checked = (Int32.Parse(Settings.GetValue("tkPunishEnabled")) == 1);
                FriendlyFireBox.Checked = (Int32.Parse(Settings.GetValue("friendlyFireWithMines")) == 1);
                PunishDefaultBox.Checked = (Int32.Parse(Settings.GetValue("tkPunishByDefault")) == 1);
                TksBeforeKickBox.Value = Int32.Parse(Settings.GetValue("tkNumPunishToKick"));
                SoldierFFBar.Value = Int32.Parse(Settings.GetValue("soldierFriendlyFire"));
                VehicleFFBar.Value = Int32.Parse(Settings.GetValue("vehicleFriendlyFire"));
                SoldierSplashFFBar.Value = Int32.Parse(Settings.GetValue("soldierSplashFriendlyFire"));
                VehicleSplashFFBar.Value = Int32.Parse(Settings.GetValue("vehicleSplashFriendlyFire"));

                // Bot Settings
                int BotCount = Int32.Parse(Settings.GetValue("coopBotCount"));
                BotRatioBar.Value = Int32.Parse(Settings.GetValue("coopBotRatio"));
                BotCountBar.Value = ((BotCount > BotCountBar.Maximum) ? 64 : BotCount);
                BotDifficultyBar.Value = Int32.Parse(Settings.GetValue("coopBotDifficulty"));

                // Voip
                EnableVoip.Checked = (Int32.Parse(Settings.GetValue("voipEnabled")) == 1);
                EnableRemoteVoip.Checked = (Int32.Parse(Settings.GetValue("voipServerRemote")) == 1);
                VoipBF2ClientPort.Value = Int32.Parse(Settings.GetValue("voipBFClientPort"));
                VoipBF2ServerPort.Value = Int32.Parse(Settings.GetValue("voipBFServerPort"));
                VoipServerPort.Value = Int32.Parse(Settings.GetValue("voipServerPort"));
                RemoteVoipIpBox.Text = Settings.GetValue("voipServerRemoteIP");
                VoipPasswordBox.Text = Settings.GetValue("voipSharedPassword");
                VoipQualityBar.Value = Int32.Parse(Settings.GetValue("voipQuality"));

                // Voting Settings
                EnableVotingBox.Checked = (Int32.Parse(Settings.GetValue("votingEnabled")) == 1);
                EnableTeamVotingBox.Checked = (Int32.Parse(Settings.GetValue("teamVoteOnly")) == 1);
                VoteTimeBar.Value = Int32.Parse(Settings.GetValue("voteTime"));
                PlayersVotingBar.Value = Int32.Parse(Settings.GetValue("minPlayersForVoting"));

                // Demo & Urls
                EnableAutoRecord.Checked = (Int32.Parse(Settings.GetValue("autoRecord", "0")) == 1);
                DemoQualityBar.Value = Int32.Parse(Settings.GetValue("demoQuality", "5"));
                DemoIndexUrlBox.Text = Settings.GetValue("demoIndexURL", "http://");
                DemoDownloadBox.Text = Settings.GetValue("demoDownloadURL", "http://");
                DemoHookBox.Text = Settings.GetValue("autoDemoHook", "");
                CLogoUrlBox.Text = Settings.GetValue("communityLogoURL", "");
                SLogoUrlBox.Text = Settings.GetValue("sponsorLogoURL", "");

                // Misc Settings
                AllowNATNagotiation.Checked = (Int32.Parse(Settings.GetValue("allowNATNegotiation", "0")) == 1);
                AllowFreeCam.Checked = (Int32.Parse(Settings.GetValue("allowFreeCam", "0")) == 1);
                AllowNoseCam.Checked = (Int32.Parse(Settings.GetValue("allowNoseCam", "1")) == 1);
                AllowExtViews.Checked = (Int32.Parse(Settings.GetValue("allowExternalViews", "1")) == 1);
                HitIndicatorEnabled.Checked = (Int32.Parse(Settings.GetValue("hitIndicator", "1")) == 1);
                RadioSpamIntBox.Value = Int32.Parse(Settings.GetValue("radioSpamInterval", "6"));
                RadioMaxSpamBox.Value = Int32.Parse(Settings.GetValue("radioMaxSpamFlagCount", "6"));
                RadioBlockTimeBar.Value = Int32.Parse(Settings.GetValue("radioBlockedDurationTime", "30"));
            }
            catch(Exception e) 
            {
                this.Load += new EventHandler(CloseOnStart);
                MessageBox.Show(e.Message, "Server Settings File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Event fired when the user wants to save his settings
        /// </summary>
        private void SaveButton_Click(object sender, EventArgs e)
        {
            // General
            Settings.SetValue("serverName", ServerNameBox.Text);
            Settings.SetValue("password", ServerPasswordBox.Text);
            Settings.SetValue("serverIP", ServerIpBox.Text);
            Settings.SetValue("serverPort", ServerPortBox.Value.ToString());
            Settings.SetValue("gameSpyPort", GamespyPortBox.Value.ToString());
            Settings.SetValue("welcomeMessage", ServerWelcomeBox.Text);
            Settings.SetValue("sponsorText", ServerWelcomeBox.Text);
            Settings.SetValue("autoBalanceTeam", (AutoBalanceBox.Checked) ? "1" : "0");
            Settings.SetValue("punkBuster", (EnablePunkBuster.Checked) ? "1" : "0");
            Settings.SetValue("internet", (EnablePublicServerBox.Checked) ? "1" : "0");
            Settings.SetValue("roundsPerMap", RoundsPerMapBox.Value.ToString());
            Settings.SetValue("numPlayersNeededToStart", PlayersToStartSlider.Value.ToString());
            Settings.SetValue("maxPlayers", MaxPlayersBar.Value.ToString());
            Settings.SetValue("ticketRatio", TicketRatioBar.Value.ToString());
            Settings.SetValue("teamRatioPercent", TeamRatioBar.Value.ToString());
            Settings.SetValue("scoreLimit", ScoreLimitBar.Value.ToString());

            // Time limits
            Settings.SetValue("timeLimit", TimeLimitBar.Value.ToString());
            Settings.SetValue("spawnTime", SpawnTimeBar.Value.ToString());
            Settings.SetValue("manDownTime", ManDownBar.Value.ToString());
            Settings.SetValue("startDelay", StartDelayBar.Value.ToString());
            Settings.SetValue("endDelay", EndDelayBar.Value.ToString());
            Settings.SetValue("endOfRoundDelay", EORBar.Value.ToString());
            Settings.SetValue("notEnoughPlayersRestartDelay", NotEnoughPlayersBar.Value.ToString());
            Settings.SetValue("timeBeforeRestartMap", TimeB4RestartMapBar.Value.ToString());

            // Friendly Fire
            Settings.SetValue("tkPunishEnabled", (PunishTeamKillsBox.Checked) ? "1" : "0");
            Settings.SetValue("friendlyFireWithMines", (FriendlyFireBox.Checked) ? "1" : "0");
            Settings.SetValue("tkPunishByDefault", (PunishDefaultBox.Checked) ? "1" : "0");
            Settings.SetValue("tkNumPunishToKick", TksBeforeKickBox.Value.ToString());
            Settings.SetValue("soldierFriendlyFire", SoldierFFBar.Value.ToString());
            Settings.SetValue("soldierSplashFriendlyFire", SoldierSplashFFBar.Value.ToString());
            Settings.SetValue("vehicleFriendlyFire", VehicleFFBar.Value.ToString());
            Settings.SetValue("vehicleSplashFriendlyFire", VehicleSplashFFBar.Value.ToString());

            // Bot Settings
            Settings.SetValue("coopBotRatio", BotRatioBar.Value.ToString());
            Settings.SetValue("coopBotCount", BotCountBar.Value.ToString());
            Settings.SetValue("coopBotDifficulty", BotDifficultyBar.Value.ToString());

            // Voip
            Settings.SetValue("voipEnabled", (EnableVoip.Checked) ? "1" : "0");
            Settings.SetValue("voipServerRemote", (EnableRemoteVoip.Checked) ? "1" : "0");
            Settings.SetValue("voipBFClientPort", VoipBF2ClientPort.Value.ToString());
            Settings.SetValue("voipBFServerPort", VoipBF2ServerPort.Value.ToString());
            Settings.SetValue("voipServerPort", VoipServerPort.Value.ToString());
            Settings.SetValue("voipQuality", VoipQualityBar.Value.ToString());
            Settings.SetValue("voipServerRemoteIP", RemoteVoipIpBox.Text);
            Settings.SetValue("voipSharedPassword", VoipPasswordBox.Text);

            // Voting
            Settings.SetValue("votingEnabled", (EnableVotingBox.Checked) ? "1" : "0");
            Settings.SetValue("teamVoteOnly", (EnableTeamVotingBox.Checked) ? "1" : "0");
            Settings.SetValue("voteTime", VoteTimeBar.Value.ToString());
            Settings.SetValue("minPlayersForVoting", PlayersVotingBar.Value.ToString());

            // Demo & Urls
            Settings.SetValue("autoRecord", (EnableAutoRecord.Checked) ? "1" : "0");
            Settings.SetValue("demoQuality", DemoQualityBar.Value.ToString());
            Settings.SetValue("demoIndexURL", DemoIndexUrlBox.Text);
            Settings.SetValue("demoDownloadURL", DemoDownloadBox.Text);
            Settings.SetValue("autoDemoHook", DemoHookBox.Text);
            Settings.SetValue("communityLogoURL", CLogoUrlBox.Text);
            Settings.SetValue("sponsorLogoURL", SLogoUrlBox.Text);

            // Misc
            Settings.SetValue("allowNATNegotiation", (AllowNATNagotiation.Checked) ? "1" : "0");
            Settings.SetValue("allowFreeCam", (AllowFreeCam.Checked) ? "1" : "0");
            Settings.SetValue("allowNoseCam", (AllowNoseCam.Checked) ? "1" : "0");
            Settings.SetValue("allowExternalViews", (AllowExtViews.Checked) ? "1" : "0");
            Settings.SetValue("hitIndicator", (HitIndicatorEnabled.Checked) ? "1" : "0");
            Settings.SetValue("radioSpamInterval", RadioSpamIntBox.Value.ToString());
            Settings.SetValue("radioMaxSpamFlagCount", RadioMaxSpamBox.Value.ToString());
            Settings.SetValue("radioBlockedDurationTime", RadioBlockTimeBar.Value.ToString());

            // Save to the file
            try
            {
                Settings.Save();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server Settings File Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event closes the form when fired
        /// </summary>
        private void CloseOnStart(object sender, EventArgs e)
        {
            this.Close();
        }

        #region Events

        private void PlayersToStartSlider_ValueChanged(object sender, EventArgs e)
        {
            PlayersToStartValueBox.Text = PlayersToStartSlider.Value.ToString();
        }

        private void VoipQualityBar_ValueChanged(object sender, EventArgs e)
        {
            VoipQualityBox.Text = VoipQualityBar.Value.ToString();
        }

        private void VoteTimeBar_ValueChanged(object sender, EventArgs e)
        {
            VoteTimeBox.Text = VoteTimeBar.Value.ToString();
        }

        private void PlayersVotingBar_ValueChanged(object sender, EventArgs e)
        {
            PlayersVotingBox.Text = PlayersVotingBar.Value.ToString();
        }

        private void TimeLimitBar_ValueChanged(object sender, EventArgs e)
        {
            TimeLimitBox.Text = TimeLimitBar.Value.ToString();
        }

        private void TicketRatioBar_ValueChanged(object sender, EventArgs e)
        {
            TicketRatioBox.Text = TicketRatioBar.Value.ToString() + "%";
        }

        private void ScoreLimitBar_ValueChanged(object sender, EventArgs e)
        {
            ScoreLimitBox.Text = ScoreLimitBar.Value.ToString();
        }

        private void SpawnTimeBar_ValueChanged(object sender, EventArgs e)
        {
            SpawnTimeBox.Text = SpawnTimeBar.Value.ToString();
        }

        private void ManDownBar_ValueChanged(object sender, EventArgs e)
        {
            ManDownBox.Text = ManDownBar.Value.ToString();
        }

        private void TeamRatioBar_ValueChanged(object sender, EventArgs e)
        {
            TeamRatioBox.Text = TeamRatioBar.Value.ToString() + "%";
        }

        private void SoldierFFBar_ValueChanged(object sender, EventArgs e)
        {
            SoldierFFBox.Text = SoldierFFBar.Value.ToString() + "%";
        }

        private void SoldierSplashFFBar_ValueChanged(object sender, EventArgs e)
        {
            SoldierSplashFFBox.Text = SoldierSplashFFBar.Value.ToString() + "%";
        }

        private void VehicleFFBar_ValueChanged(object sender, EventArgs e)
        {
            VehicleFFBox.Text = VehicleFFBar.Value.ToString() + "%";
        }

        private void VehicleSplashFFBar_ValueChanged(object sender, EventArgs e)
        {
            VehicleSplashFFBox.Text = VehicleSplashFFBar.Value.ToString() + "%";
        }

        private void BotRatioBar_ValueChanged(object sender, EventArgs e)
        {
            BotRatioBox.Text = BotRatioBar.Value.ToString() + "%";
        }

        private void BotCountBar_ValueChanged(object sender, EventArgs e)
        {
            BotCountBox.Text = BotCountBar.Value.ToString();
        }

        private void BotDifficultyBar_ValueChanged(object sender, EventArgs e)
        {
            BotDifficultyBox.Text = BotDifficultyBar.Value.ToString();
        }

        private void MaxPlayersBar_ValueChanged(object sender, EventArgs e)
        {
            MaxPlayersBox.Text = MaxPlayersBar.Value.ToString();
        }

        private void StartDelayBar_ValueChanged(object sender, EventArgs e)
        {
            StartDelayBox.Text = StartDelayBar.Value.ToString();
        }

        private void EndDelayBar_ValueChanged(object sender, EventArgs e)
        {
            EndDelayBox.Text = EndDelayBar.Value.ToString();
        }

        private void EORBar_ValueChanged(object sender, EventArgs e)
        {
            EORBox.Text = EORBar.Value.ToString();
        }

        private void NotEnoughPlayersBar_ValueChanged(object sender, EventArgs e)
        {
            NotEnoughPlayersBox.Text = NotEnoughPlayersBar.Value.ToString();
        }

        private void TimeB4RestartMapBar_ValueChanged(object sender, EventArgs e)
        {
            TimeB4RestartMapBox.Text = TimeB4RestartMapBar.Value.ToString();
        }

        private void PunishTeamKillsBox_CheckedChanged(object sender, EventArgs e)
        {
            PunishDefaultBox.Enabled = PunishTeamKillsBox.Checked;
            TksBeforeKickBox.Enabled = PunishTeamKillsBox.Checked;
        }

        private void EnableRemoteVoip_CheckedChanged(object sender, EventArgs e)
        {
            if (EnableRemoteVoip.Checked)
            {
                if (!EnableVoip.Checked)
                {
                    EnableVoip.Checked = true;
                    EnableRemoteVoip.Checked = true;
                }

                RemoteVoipIpBox.Enabled = true;
                VoipPasswordBox.Enabled = true;
                VoipServerPort.Enabled = true;
                VoipQualityBar.Enabled = false;
            }
            else
            {
                RemoteVoipIpBox.Enabled = false;
                VoipPasswordBox.Enabled = false;
                VoipServerPort.Enabled = false;
                VoipQualityBar.Enabled = true;
            }
        }

        private void EnableVoip_CheckedChanged(object sender, EventArgs e)
        {
            if (!EnableVoip.Checked && EnableRemoteVoip.Checked)
                EnableRemoteVoip.Checked = false;
        }

        private void DemoQualityBar_ValueChanged(object sender, EventArgs e)
        {
            DemoQualityBox.Text = DemoQualityBar.Value.ToString();
        }

        private void RadioBlockTimeBar_ValueChanged(object sender, EventArgs e)
        {
            RadioBlockTimeBox.Text = RadioBlockTimeBar.Value.ToString();
        }

        #endregion
    }
}
