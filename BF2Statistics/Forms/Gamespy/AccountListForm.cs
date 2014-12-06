using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BF2Statistics.Gamespy;
using BF2Statistics.Database;
using BF2Statistics.Database.QueryBuilder;

namespace BF2Statistics
{
    public partial class AccountListForm : Form
    {
        /// <summary>
        /// Current list page number
        /// </summary>
        private int ListPage = 1;

        /// <summary>
        /// Total pages in the current data set
        /// </summary>
        private int TotalPages = 1;

        /// <summary>
        /// Our current sorted column
        /// </summary>
        private DataGridViewColumn SortedCol;

        /// <summary>
        /// Sorted column sort direction
        /// </summary>
        private ListSortDirection SortDir = ListSortDirection.Ascending;

        public AccountListForm()
        {
            InitializeComponent();
            SortedCol = DataTable.Columns[0];

            // Try to connect to the database 
            try
            {
                using (GamespyDatabase Driver = new GamespyDatabase()) { }
            }
            catch (DbConnectException Ex)
            {
                ExceptionForm.ShowDbConnectError(Ex);
                Load += (s, e) => Close(); // Close form
                return;
            }

            // Setting the limit will build the inital list
            LimitSelect.SelectedIndex = 2;
        }

        /// <summary>
        /// Fills the DataGridView with a list of accounts
        /// </summary>
        private void BuildList()
        {
            // Define initial variables
            int Limit = Int32.Parse(LimitSelect.SelectedItem.ToString());
            string Like = SearchBox.Text.Replace("'", "").Trim();
            List<Dictionary<string, object>> Rows;
            WhereClause Where = null;

            // Start Record
            int Start = (ListPage == 1) ? 0 : (ListPage - 1) * Limit;

            // Build Query
            using (GamespyDatabase Driver = new GamespyDatabase())
            {
                SelectQueryBuilder Query = new SelectQueryBuilder(Driver);
                Query.SelectColumns("id", "name", "email", "country", "lastip", "session");
                Query.SelectFromTable("accounts");
                Query.AddOrderBy(SortedCol.Name, ((SortDir == ListSortDirection.Ascending) ? Sorting.Ascending : Sorting.Descending));
                Query.Limit(Limit, Start);

                // User entered search
                if (!String.IsNullOrWhiteSpace(Like))
                    Where = Query.AddWhere("name", Comparison.Like, "%" + Like + "%");

                // Online Accounts
                if (OnlineAccountsCheckBox.Checked)
                {
                    if (Where == null)
                        Where = Query.AddWhere("session", Comparison.NotEqualTo, 0);
                    else
                        Where.AddClause(LogicOperator.And, "session", Comparison.NotEqualTo, 0);
                }

                // Clear out old junk
                DataTable.Rows.Clear();

                // Add players to data grid
                int RowCount = 0;
                foreach (Dictionary<string, object> Row in Driver.QueryReader(Query.BuildCommand()))
                {
                    DataTable.Rows.Add(new string[] {
                    Row["id"].ToString(),
                    Row["name"].ToString(),
                    Row["email"].ToString(),
                    Row["country"].ToString(),
                    ((Row["session"].ToString() == "1") ? "Yes" : "No"),
                    Row["lastip"].ToString(),
                });
                    RowCount++;
                }

                // Get Filtered Rows
                Query = new SelectQueryBuilder(Driver);
                Query.SelectCount();
                Query.SelectFromTable("accounts");
                if (Where != null)
                    Query.AddWhere(Where);
                Rows = Driver.ExecuteReader(Query.BuildCommand());
                int TotalFilteredRows = Int32.Parse(Rows[0]["count"].ToString());

                // Get Total Player Count
                Query = new SelectQueryBuilder(Driver);
                Query.SelectCount();
                Query.SelectFromTable("accounts");
                Rows = Driver.ExecuteReader(Query.BuildCommand());
                int TotalRows = Int32.Parse(Rows[0]["count"].ToString());

                // Stop Count
                int Stop = (ListPage == 1) ? RowCount : ((ListPage - 1) * Limit) + RowCount;

                // First / Previous button
                if (ListPage == 1)
                {
                    FirstBtn.Enabled = false;
                    PreviousBtn.Enabled = false;
                }
                else
                {
                    FirstBtn.Enabled = true;
                    PreviousBtn.Enabled = true;
                }

                // Next / Last Button
                LastBtn.Enabled = false;
                NextBtn.Enabled = false;

                // Get total number of pages
                if (TotalFilteredRows / (ListPage * Limit) > 0)
                {
                    float total = float.Parse(TotalFilteredRows.ToString()) / float.Parse(Limit.ToString());
                    TotalPages = Int32.Parse(Math.Floor(total).ToString());
                    if (TotalFilteredRows % Limit != 0)
                        TotalPages++;

                    LastBtn.Enabled = true;
                    NextBtn.Enabled = true;
                }

                // Set page number
                PageNumber.Maximum = TotalPages;
                PageNumber.Value = ListPage;

                // Update Row Count Information
                RowCountDesc.Text = String.Format(
                    "Showing {0} to {1} of {2} account{3}",
                    ++Start,
                    Stop,
                    TotalFilteredRows,
                    ((TotalFilteredRows > 1) ? "s " : " ")
                );

                // Add Total row count
                if (!String.IsNullOrWhiteSpace(Like))
                    RowCountDesc.Text += String.Format("(filtered from " + TotalRows + " total account{0})", ((TotalRows > 1) ? "s" : ""));

                // Update
                DataTable.Update();
            }
        }

        /// <summary>
        /// Search OnKey Down function. Filters the List of accounts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            BuildList();
        }

        /// <summary>
        /// Re-Filters the results when the limit is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LimitSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListPage = 1;
            BuildList();
            DataTable.Focus();
        }

        /// <summary>
        /// When a row is double clicked, this method is called, opening the Account Edit Form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataTable_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            int Id = Int32.Parse(DataTable.Rows[e.RowIndex].Cells[0].Value.ToString());
            AccountEditForm Form = new AccountEditForm(Id);
            Form.ShowDialog();
            BuildList();
        }

        /// <summary>
        /// Sets the current page to 1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FirstBtn_Click(object sender, EventArgs e)
        {
            ListPage = 1;
            BuildList();
            DataTable.Focus();
        }

        /// <summary>
        /// Decrements the current page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviousBtn_Click(object sender, EventArgs e)
        {
            ListPage -= 1;
            BuildList();
            DataTable.Focus();
        }

        /// <summary>
        /// Increments the current page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextBtn_Click(object sender, EventArgs e)
        {
            ListPage++;
            BuildList();
            DataTable.Focus();
        }

        /// <summary>
        /// Sets the current page to the last page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LastBtn_Click(object sender, EventArgs e)
        {
            ListPage = TotalPages;
            BuildList();
            DataTable.Focus();
        }

        /// <summary>
        /// Maunal Sorting of columns (Since Auto Sort sucks!)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataTable_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewColumn SelectedCol = DataTable.Columns[e.ColumnIndex];

            // Sort the same column again, reversing the SortOrder. 
            if (SortedCol == SelectedCol)
            {
                SortDir = (SortDir == ListSortDirection.Ascending)
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }
            else
            {
                // Sort a new column and remove the old SortGlyph.
                SortDir = ListSortDirection.Ascending;
                SortedCol.HeaderCell.SortGlyphDirection = SortOrder.None;
                SortedCol = SelectedCol;
            }

            // Set new Sort Glyph Direction
            SortedCol.HeaderCell.SortGlyphDirection = ((SortDir == ListSortDirection.Ascending)
                ? SortOrder.Ascending
                : SortOrder.Descending);

            // Build new List with database sort!
            BuildList();
        }

        private void PageNumber_ValueChanged(object sender, EventArgs e)
        {
            int Val = Int32.Parse(PageNumber.Value.ToString());
            if (Val != ListPage)
            {
                ListPage = Val;
                BuildList();
            }
        }

        private void OnlineAccountsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            BuildList();
        }

        private void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            menuItemDelete.Enabled = (DataTable.SelectedRows.Count != 0);
        }

        /// <summary>
        /// Delete Account menu item click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemDelete_Click(object sender, System.EventArgs e)
        {
            int Id = Int32.Parse(DataTable.SelectedRows[0].Cells[0].Value.ToString());
            string Name = DataTable.SelectedRows[0].Cells[1].Value.ToString();

            if (MessageBox.Show("Are you sure you want to delete account \"" + Name + "\"?", 
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                using (GamespyDatabase Database = new GamespyDatabase())
                {
                    if (Database.DeleteUser(Id) == 1)
                        Notify.Show("Account deleted successfully!", "Operation Successful", AlertType.Success);
                    else
                        Notify.Show("Failed to remove account from database!", "Operation Failed", AlertType.Warning);
                }

                BuildList();
            }
        }
    }
}
