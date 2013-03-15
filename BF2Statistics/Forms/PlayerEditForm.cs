using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;
using BF2Statistics.ASP;
using BF2Statistics.Database;

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
        /// Stats Database Driver
        /// </summary>
        private DatabaseDriver Driver = ASPServer.Database.Driver;

        public PlayerEditForm(int Pid)
        {
            InitializeComponent();
            this.Pid = Pid;

            // Fetch Player from database
            List<Dictionary<string, object>> Rows = Driver.Query("SELECT name, clantag, rank, permban FROM player WHERE id={0}", Pid);
            Player = Rows[0];

            // Set window title
            this.Text = Player["name"].ToString();

            // Set form values
            PlayerId.Value = Pid;
            PlayerNick.Text = Player["name"].ToString();
            ClanTag.Text = Player["clantag"].ToString();
            Rank.SelectedIndex = Int32.Parse(Player["rank"].ToString());
            PermBan.SelectedIndex = Int32.Parse(Player["permban"].ToString());
        }

        private void ResetBtn_Click(object sender, EventArgs e)
        {
            Driver.Execute("UPDATE unlocks SET state = 'n' WHERE id = {0}", Pid);
            Driver.Execute("UPDATE player SET usedunlocks = 0 WHERE id = {0}", Pid);
            MessageBox.Show("Player unlocks have been reset", "Success");
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            bool Changes = false;
            SqlUpdateDictionary Update = new SqlUpdateDictionary();

            // Update clantag
            if (Player["clantag"].ToString() != ClanTag.Text.Trim())
            {
                Player["clantag"] = ClanTag.Text.Trim();
                Update.Add("clantag", ClanTag.Text.Trim(), true);
                Changes = true;
            }

            // Update Rank
            if (Int32.Parse(Player["rank"].ToString()) != Rank.SelectedIndex)
            {
                if (Int32.Parse(Player["rank"].ToString()) > Rank.SelectedIndex)
                {
                    Update.Add("decr", 1, false);
                    Update.Add("chng", 0, false);
                }
                else
                {
                    Update.Add("decr", 0, false);
                    Update.Add("chng", 1, false);
                }

                Player["rank"] = Rank.SelectedIndex;
                Update.Add("rank", Rank.SelectedIndex, false);

                Changes = true;
            }

            // update perm ban status
            if (Int32.Parse(Player["permban"].ToString()) != PermBan.SelectedIndex)
            {
                Player["permban"] = PermBan.SelectedIndex;
                Update.Add("permban", PermBan.SelectedIndex, false);
                Changes = true;
            }

            // If no changes made, just return
            if (!Changes)
            {
                MessageBox.Show("Unable to save player because no changes were made.", "Warning");
                return;
            }

            Driver.Update("player", Update, "id=" + Pid);
            this.Close();
        }

        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete player?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DbTransaction Transaction = Driver.BeginTransaction();
                List<string> Tables = StatsDatabase.GetPlayerTables();
                UpdateProgressForm.ShowScreen("Deleting Player...", this);

                try
                {
                    // Remove the player from each player table
                    foreach (string Table in Tables)
                    {
                        UpdateProgressForm.Status("Removing player from \"" + Table + "\" table...");
                        if (Table == "kills")
                            Driver.Execute("DELETE FROM {0} WHERE attacker={1} OR victim={1}", Table, Pid);
                        else
                            Driver.Execute("DELETE FROM {0} WHERE id={1}", Table, Pid);
                    }

                    Transaction.Commit();
                    UpdateProgressForm.CloseForm();
                    MessageBox.Show("Player deleted successfully!", "Success");
                    this.Close();
                }
                catch (Exception E)
                {
                    Transaction.Rollback();
                    UpdateProgressForm.CloseForm();
                    MessageBox.Show("Failed to remove player from database!\r\n\r\nMessage: " + E.Message, "Error");
                }
            }
        }
    }
}
