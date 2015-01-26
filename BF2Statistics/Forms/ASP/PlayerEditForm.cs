using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;
using System.Reflection;
using System.IO;
using BF2Statistics.ASP;
using BF2Statistics.Web;
using BF2Statistics.Database;
using BF2Statistics.Database.QueryBuilder;
using BF2Statistics.Utilities;

namespace BF2Statistics
{
    public partial class PlayerEditForm : Form
    {
        /// <summary>
        /// Current player ID
        /// </summary>
        private int Pid;

        /// <summary>
        /// Player information
        /// </summary>
        private Dictionary<string, object> Player;

        /// <summary>
        /// Current executing Assembly
        /// </summary>
        Assembly Me = Assembly.GetExecutingAssembly();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Pid">The Players ID</param>
        public PlayerEditForm(int Pid)
        {
            InitializeComponent();
            this.Pid = Pid;
            LabelPid.Text = Pid.ToString();
            LoadPlayer();
        }

        /// <summary>
        /// Loads the players stats from the database, and fills out the forms
        /// labels with the current information
        /// </summary>
        private void LoadPlayer()
        {
            StatsDatabase Driver;

            // Establish DB connection
            try
            {
                Driver = new StatsDatabase();
            }
            catch (DbConnectException Ex)
            {
                ExceptionForm.ShowDbConnectError(Ex);
                HttpServer.Stop();
                Load += (s, e) => Close(); // Close form
                return;
            }

            // Fetch Player from database
            SelectQueryBuilder Builder = new SelectQueryBuilder(Driver);
            Builder.SelectFromTable("player");
            Builder.SelectColumns(
                "name", "score", "cmdscore", "skillscore", "teamscore", "joined",
                "country", "rank", "wins", "losses", "permban", "clantag", "isbot");
            Builder.AddWhere("id", Comparison.Equals, Pid);
            List<Dictionary<string, object>> Rows = Driver.ExecuteReader(Builder.BuildCommand());
            Player = Rows[0];

            // Set window title
            this.Text = String.Concat(Player["name"].ToString().Trim(), " (", Pid, ")");

            // Set country flag
            try
            {
                string Country = String.IsNullOrEmpty(Player["country"].ToString()) ? "XX" : Player["country"].ToString();
                CountryPicture.Image = Image.FromStream(Me.GetManifestResourceStream("BF2Statistics.Resources." + Country.ToUpper() + ".png"));
            }
            catch { }

            // Joined Label
            int Joind = Int32.Parse(Player["joined"].ToString());
            DateTime D = DateTime.UtcNow.ToDateTime(Joind);
            LabelJoined.Text = String.Concat(D.ToString("yyyy-MM-dd HH:mm"), " GMT");
            Tipsy.SetToolTip(LabelJoined, String.Concat(D.ToLocalTime().ToString("yyyy-MM-dd HH:mm"), " Local Time."));

            // Fill out the rest of the labels
            LabelNick.Text = Player["name"].ToString().Trim();
            ClanTagBox.Text = Player["clantag"].ToString();
            RankSelect.SelectedIndex = Int32.Parse(Player["rank"].ToString());
            PermBanSelect.SelectedIndex = Int32.Parse(Player["permban"].ToString());
            LabelGlobalScore.Text = Player["score"].ToString();
            LabelWinLoss.Text = String.Concat(Player["wins"], " / ", Player["losses"]);
            LabelTeamScore.Text = Player["teamscore"].ToString();
            LabelCombatScore.Text = Player["skillscore"].ToString();
            LabelCommandScore.Text = Player["cmdscore"].ToString();

            // Get Leaderboard Position
            Rows = Driver.Query("SELECT COUNT(id) as count FROM player WHERE score > @P0", Int32.Parse(Player["score"].ToString()));
            int Position = Int32.Parse(Rows[0]["count"].ToString()) + 1;
            LabelPosition.Text = Position.ToString();
            SaveBtn.Enabled = false;

            // Lock unlocks button if player is Bot
            if (Int32.Parse(Player["isbot"].ToString()) > 0)
                ResetUnlocksBtn.Enabled = false;

            // Close Connection
            Driver.Dispose();
        }

        /// <summary>
        /// Reset Unlocks Button Click Event
        /// </summary>
        private void ResetUnlocksBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Create New Player Unlock Data
                StringBuilder Query = new StringBuilder("INSERT INTO unlocks VALUES ");

                // Normal unlocks
                for (int i = 11; i < 100; i += 11)
                    Query.AppendFormat("({0}, {1}, 'n'), ", Pid, i);

                // Sf Unlocks
                for (int i = 111; i < 556; i += 111)
                {
                    Query.AppendFormat("({0}, {1}, 'n')", Pid, i);
                    if (i != 555) 
                        Query.Append(", ");
                }

                // Do driver queries
                using (StatsDatabase Driver = new StatsDatabase())
                using (DbTransaction T = Driver.BeginTransaction())
                {
                    // Perform queries
                    Driver.Execute("DELETE FROM unlocks WHERE id = " + Pid);
                    Driver.Execute("UPDATE player SET usedunlocks = 0 WHERE id = " + Pid);
                    Driver.Execute(Query.ToString());
                    T.Commit();

                    // Notify user
                    Notify.Show("Player Unlocks Have Been Reset", "This player will be able to select his new unlocks upon logging in.", AlertType.Success);
                }
            }
            catch (DbConnectException Ex)
            {
                HttpServer.Stop();
                ExceptionForm.ShowDbConnectError(Ex);
                this.Close();
            }
        }

        /// <summary>
        /// Export Player Button Click Event
        /// </summary>
        private void ExportPlayerBtn_Click(object sender, EventArgs e)
        {
            // Create export directory if it doesnt exist yet
            string sPath = Path.Combine(Paths.DocumentsFolder, "Player Exports");
            if (!Directory.Exists(sPath))
                Directory.CreateDirectory(sPath);

            // Have user select folder
            FolderSelect.FolderSelectDialog Dialog = new FolderSelect.FolderSelectDialog();
            Dialog.InitialDirectory = sPath;
            Dialog.Title = "Select folder to export player to";
            if (Dialog.ShowDialog())
            {
                try
                {
                    StatsManager.ExportPlayerXml(sPath, Pid, Player["name"].ToString());
                    Notify.Show("Player Exported Successfully", String.Format("{0} ({1})", Player["name"].ToString(), Pid), AlertType.Success);
                }
                catch (DbConnectException Ex)
                {
                    HttpServer.Stop();
                    ExceptionForm.ShowDbConnectError(Ex);
                    this.Close();
                }
                catch (Exception E)
                {
                    ExceptionForm EForm = new ExceptionForm(E, false);
                    EForm.Message = "Unable to export player because an exception was thrown!";
                    EForm.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Delete Player Button Click Event
        /// </summary>
        private void DeletePlayerBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete player?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    TaskForm.Show(this, "Delete Player", "Deleting Player \"" + Player["name"] + "\"", false);

                    // Delete the player
                    using (StatsDatabase Driver = new StatsDatabase())
                        Driver.DeletePlayer(Pid, true);

                    Notify.Show("Player Deleted Successfully!", "Operation Successful", AlertType.Success);
                }
                catch (DbConnectException Ex)
                {
                    HttpServer.Stop();
                    ExceptionForm.ShowDbConnectError(Ex);
                }
                catch (Exception E)
                {
                    // Show exception error
                    ExceptionForm Form = new ExceptionForm(E, false);
                    Form.Message = String.Format("Failed to remove player from database!{1}{1}Error: {0}", E.Message, Environment.NewLine);
                    Form.ShowDialog();
                }
                finally
                {
                    // Close task form
                    TaskForm.CloseForm();
                    this.Close();
                }
            }
        }

        /// <summary>
        /// Cancel Button Click Event
        /// </summary>
        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Save Button Click Event
        /// </summary>
        private void SaveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                using (StatsDatabase Driver = new StatsDatabase())
                {
                    bool Changes = false;
                    UpdateQueryBuilder Query = new UpdateQueryBuilder("player", Driver);
                    int Rank = Int32.Parse(Player["rank"].ToString());

                    // Update clantag
                    if (Player["clantag"].ToString() != ClanTagBox.Text.Trim())
                    {
                        Player["clantag"] = ClanTagBox.Text.Trim();
                        Query.SetField("clantag", ClanTagBox.Text.Trim());
                        Changes = true;
                    }

                    // Update Rank
                    if (Rank != RankSelect.SelectedIndex)
                    {
                        if (Rank > RankSelect.SelectedIndex)
                        {
                            Query.SetField("decr", 1);
                            Query.SetField("chng", 0);
                        }
                        else
                        {
                            Query.SetField("decr", 0);
                            Query.SetField("chng", 1);
                        }

                        Player["rank"] = RankSelect.SelectedIndex;
                        Query.SetField("rank", RankSelect.SelectedIndex);
                        Changes = true;
                    }

                    // update perm ban status
                    if (Int32.Parse(Player["permban"].ToString()) != PermBanSelect.SelectedIndex)
                    {
                        Player["permban"] = PermBanSelect.SelectedIndex;
                        Query.SetField("permban", PermBanSelect.SelectedIndex);
                        Changes = true;
                    }

                    // If no changes made, just return
                    if (!Changes)
                    {
                        MessageBox.Show("Unable to save player because no changes were made.",
                            "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Preform Query
                    Query.AddWhere("id", Comparison.Equals, Pid);
                    Query.Execute();
                    this.Close();
                }
            }
            catch (DbConnectException Ex)
            {
                HttpServer.Stop();
                ExceptionForm.ShowDbConnectError(Ex);
                return;
            }
        }

        /// <summary>
        /// Rank Select Index Change Event
        /// </summary>
        private void RankSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            int Rank = RankSelect.SelectedIndex;
            RankPictureBox.Image = Image.FromStream(Me.GetManifestResourceStream("BF2Statistics.Resources.rank_" + Rank + ".png"));
            LabelTitle.Text = MedalData.Rank.GetName(Rank);
            SaveBtn.Enabled = true;
        }

        /// <summary>
        /// Perm Ban Index Change Event
        /// </summary>
        private void PermBanSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveBtn.Enabled = true;
        }

        /// <summary>
        /// Clan Tag TextBox Text Changed Event
        /// </summary>
        private void ClanTagBox_TextChanged(object sender, EventArgs e)
        {
            SaveBtn.Enabled = true;
        }

        /// <summary>
        /// Reset stats button click event
        /// </summary>
        private void ResetStatsBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to reset players stats?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    TaskForm.Show(this, "Reset Player Stats", "Reseting Player \"" + Player["name"] + "\"'s Stats", false);

                    // Delete the players
                    using (StatsDatabase Driver = new StatsDatabase())
                    {
                        Driver.DeletePlayer(Pid, true);

                        // Insert a new player record
                        Driver.Execute(
                            "INSERT INTO player(id, name, country, joined, clantag, permban, isbot) VALUES(@P0, @P1, @P2, @P3, @P4, @P5, @P6)",
                            Pid, Player["name"], Player["country"], Player["joined"], Player["clantag"], Player["permban"], Player["isbot"]
                        );
                    }

                    LoadPlayer();
                    Notify.Show("Player Stats Reset Successfully!", "Operation Successful", AlertType.Success);
                }
                catch (DbConnectException Ex)
                {
                    HttpServer.Stop();
                    ExceptionForm.ShowDbConnectError(Ex);
                    TaskForm.CloseForm();
                    this.Close();
                    return;
                }
                catch (Exception E)
                {
                    // Show exception error
                    ExceptionForm Form = new ExceptionForm(E, false);
                    Form.Message = String.Format("Failed to reset player stats!{1}{1}Error: {0}", E.Message, Environment.NewLine);
                    Form.ShowDialog();
                }
                finally
                {
                    // Close task form
                    TaskForm.CloseForm();
                }
            }
        }
    }
}
