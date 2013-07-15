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
using BF2Statistics.Database.QueryBuilder;

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
            List<Dictionary<string, object>> Rows = Driver.Query("SELECT name, clantag, rank, permban FROM player WHERE id=" + Pid);
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
            Driver.Execute("UPDATE unlocks SET state = 'n' WHERE id = " + Pid);
            Driver.Execute("UPDATE player SET usedunlocks = 0 WHERE id = " + Pid);
            MessageBox.Show("Player unlocks have been reset", "Success");
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            bool Changes = false;
            UpdateQueryBuilder Query = new UpdateQueryBuilder();

            // Update clantag
            if (Player["clantag"].ToString() != ClanTag.Text.Trim())
            {
                Player["clantag"] = ClanTag.Text.Trim();
                Query.SetField("clantag", ClanTag.Text.Trim());
                Changes = true;
            }

            // Update Rank
            if (Int32.Parse(Player["rank"].ToString()) != Rank.SelectedIndex)
            {
                if (Int32.Parse(Player["rank"].ToString()) > Rank.SelectedIndex)
                {
                    Query.SetField("decr", 1);
                    Query.SetField("chng", 0);
                }
                else
                {
                    Query.SetField("decr", 0);
                    Query.SetField("chng", 1);
                }

                Player["rank"] = Rank.SelectedIndex;
                Query.SetField("rank", Rank.SelectedIndex);

                Changes = true;
            }

            // update perm ban status
            if (Int32.Parse(Player["permban"].ToString()) != PermBan.SelectedIndex)
            {
                Player["permban"] = PermBan.SelectedIndex;
                Query.SetField("permban", PermBan.SelectedIndex);
                Changes = true;
            }

            // If no changes made, just return
            if (!Changes)
            {
                MessageBox.Show("Unable to save player because no changes were made.", "Warning");
                return;
            }

            // Preform Query
            Query.AddWhere("id", Comparison.Equals, Pid);
            Query.Execute();
            this.Close();
        }

        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete player?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                DbTransaction Transaction = Driver.BeginTransaction();
                List<string> Tables = StatsDatabase.GetPlayerTables();
                TaskForm.Show(this, "Delete Player", "Deleting Player \"" + Player["name"] + "\"", false);

                try
                {
                    // Remove the player from each player table
                    foreach (string Table in Tables)
                    {
                        TaskForm.UpdateStatus("Removing player from \"" + Table + "\" table...");
                        if (Table == "kills")
                            Driver.Execute(String.Format("DELETE FROM {0} WHERE attacker={1} OR victim={1}", Table, Pid));
                        else
                            Driver.Execute(String.Format("DELETE FROM {0} WHERE id={1}", Table, Pid));
                    }

                    // Commit Transaction
                    TaskForm.UpdateStatus("Commiting Transaction");
                    Transaction.Commit();

                    // Close Task form and Show success toast message
                    TaskForm.CloseForm();
                    Notify.Show("Player deleted successfully!", Player["name"].ToString(),  AlertType.Success);
                    this.Close();
                }
                catch (Exception E)
                {
                    // Show exception error
                    ExceptionForm Form = new ExceptionForm(E, false);
                    Form.Message = String.Format("Failed to remove player from database!{1}{1}Error: {0}", E.Message, Environment.NewLine);
                    DialogResult Result = Form.ShowDialog();

                    // Rollback!
                    Transaction.Rollback();
                    TaskForm.CloseForm();
                }
            }
        }
    }
}
