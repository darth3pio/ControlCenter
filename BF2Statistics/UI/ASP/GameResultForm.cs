using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using BF2Statistics.ASP.StatsProcessor;

namespace BF2Statistics
{
    public partial class GameResultForm : Form
    {
        /// <summary>
        /// Current executing Assembly
        /// </summary>
        Assembly Me = Assembly.GetExecutingAssembly();

        /// <summary>
        /// An array of the supported army names [ArmyId => Name]
        /// </summary>
        protected string[] ArmyNames = new string[]
        {
            "United States Marine Corp.",
            "Middle Eastern Coalition",
            "People's Liberation Army",
            "United States Navy SEALS",
            "British SAS",
            "Russian Spetznas",
            "MEC Special Forces",
            "Rebel Forces",
            "Insurgent Forces",
            "European Union",
            "German Forces",
            "Ukrainian Forces",
            "United Nations",
            "Canadian Forces"
        };

        public GameResultForm(GameResult Game, bool isProcessed)
        {
            InitializeComponent();

            // Build Team Grid 1
            foreach (Player P in Game.GetPlayersByTeam(1).OrderByDescending(p => p.RoundScore))
            {
                DataGridViewRow Row = new DataGridViewRow();
                Row.CreateCells(Team1Grid);
                Row.SetValues(new object[] {
                    Image.FromStream(Me.GetManifestResourceStream("BF2Statistics.Resources.rank_" + P.Rank + "icon.gif")), 
                    P.Name, P.RoundScore, P.Stats.Kills, P.Stats.Deaths
                });
                Row.Tag = P.Pid;
                Team1Grid.Rows.Add(Row);
            }

            // Build Team Grid 2
            foreach (Player P in Game.GetPlayersByTeam(2).OrderByDescending(p => p.RoundScore))
            {
                DataGridViewRow Row = new DataGridViewRow();
                Row.CreateCells(Team2Grid);
                Row.SetValues(new object[] {
                    Image.FromStream(Me.GetManifestResourceStream("BF2Statistics.Resources.rank_" + P.Rank + "icon.gif")), 
                    P.Name, P.RoundScore, P.Stats.Kills, P.Stats.Deaths
                });
                Row.Tag = P.Pid;
                Team2Grid.Rows.Add(Row);
            }

            // Set flag images
            ArmyFlag1.Image = Image.FromStream(Me.GetManifestResourceStream("BF2Statistics.Resources.armyflag_" + GetArmyCode(Game.Team1ArmyId) + ".png"));
            ArmyFlag2.Image = Image.FromStream(Me.GetManifestResourceStream("BF2Statistics.Resources.armyflag_" + GetArmyCode(Game.Team2ArmyId) + ".png"));

            // Set team names
            TeamName1.Text = ArmyNames[Game.Team1ArmyId];
            TeamName2.Text = ArmyNames[Game.Team2ArmyId];

            // Set secondary team text
            if (Game.WinningTeam == 1)
            {
                TicketsLabel1.Text = "Has won the Round!";
            }
            else if (Game.WinningTeam == 2)
            {
                TicketsLabel2.Text = "Has won the Round!";
            }
            else
            {
                TicketsLabel1.Text = "Remaining Tickets: " + Game.Team1Tickets;
                TicketsLabel2.Text = "Remaining Tickets: " + Game.Team2Tickets;
            }

            // Set window title text
            this.Text = String.Format("Game Result: {0} [{1}] - {2} UTC", Game.MapName, Game.Mod, Game.RoundEndDate.ToString());
            this.MapNameLabel.Text = Game.MapName + " (ID: " + Game.MapId + ")";
            this.ModLabel.Text = Game.Mod;
            this.KillsLabel.Text = Game.MapKills.ToString();
            this.DeathsLabel.Text = Game.MapDeaths.ToString();
            this.StartTimeLabel.Text = DateTime.UtcNow.FromUnixTimestamp(Game.RoundStartTime).ToLocalTime().ToString();
            this.EndTimeLabel.Text = DateTime.UtcNow.FromUnixTimestamp(Game.RoundEndTime).ToLocalTime().ToString();
            this.RoundTimeLabel.Text = Game.RoundTime.ToString();
            this.ServerNameLabel.Text = Game.ServerName;
            this.ServerPortLabel.Text = Game.ServerPort.ToString();
            this.TotalPlayersLabel.Text = Game.PlayersConnected.ToString();
            this.Team1PlayersLabel.Text = Game.Team1Players.ToString();
            this.Team2PlayersLabel.Text = Game.Team2Players.ToString();
            this.EorPlayers1Label.Text = Game.Team1PlayersEnd.ToString();
            this.EorPlayers2Label.Text = Game.Team2PlayersEnd.ToString();
            this.RoundProcLabel.Text = isProcessed ? "True" : "False";

            // Set Gamemode
            this.GameModeLabel.Text = GetGameModeText(Game.GameMode);
        }

        private string GetArmyCode(int ArmyId)
        {
            switch (ArmyId)
            {
                case 0: return "us";
                case 1: return "mec";
                case 2: return "ch";
                case 3: return "us";
                case 4: return "sas";
                case 5: return "spetz";
                case 6: return "mec";
                case 7: return "chin";
                case 8: return "mecin";
                case 9: return "eu";
                case 10: return "ger";
                case 11: return "ukr";
                case 12: return "un";
                case 13: return "ca";
            }

            return "";
        }

        private string GetGameModeText(int GameMode)
        {
            switch (GameMode)
            {
                case 0: return "Conquest";
                case 1: return "Single Player";
                case 2: return "Coop";
            }
            return "Unknown";
        }

        private void GameResultForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ArmyFlag1.Image.Dispose();
            ArmyFlag2.Image.Dispose();
        }
    }
}
