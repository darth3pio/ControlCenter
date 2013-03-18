using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BF2Statistics.Gamespy;
using BF2Statistics.Database;

namespace BF2Statistics
{
    public partial class AccountListForm : Form
    {
        /// <summary>
        /// The Gamespy database driver
        /// </summary>
        private DatabaseDriver Driver = LoginServer.Database.Driver;

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
            LimitSelect.SelectedIndex = 2;
        }

        /// <summary>
        /// Fills the DataGridView with a list of accounts
        /// </summary>
        private void BuildList()
        {
            // Define initial variables
            int Start = 0;
            int Stop = 0;
            int Limit = Int32.Parse(LimitSelect.SelectedItem.ToString());
            string Like = " ";
            List<Dictionary<string, object>> Rows;

            // Sorting
            string OrderBy = String.Format("ORDER BY {0} {1}", SortedCol.Name, ((SortDir == ListSortDirection.Ascending) ? "ASC" : "DESC"));

            // Start Record
            if (ListPage == 1)
                Start = 0;
            else
                Start = (ListPage - 1) * Limit;

            // User entered search
            if (!String.IsNullOrWhiteSpace(SearchBox.Text.Replace("'", "")))
                Like = String.Format(" WHERE name LIKE '%{0}%' ", SearchBox.Text.Trim().Replace("'", ""));

            // Clear out old junk
            DataTable.Rows.Clear();

            // Add players to data grid
            Rows = Driver.Query("SELECT id, name, email, country, lastip, session FROM accounts{0}{1} LIMIT {2}, {3}", Like, OrderBy, Start, Limit);
            int RowCount = Rows.Count;
            int i = 0;
            foreach (Dictionary<string, object> P in Rows)
            {
                DataTable.Rows.Add(new string[] { 
                    Rows[i]["id"].ToString(),
                    Rows[i]["name"].ToString(),
                    Rows[i]["email"].ToString(),
                    Rows[i]["country"].ToString(),
                    ((Rows[i]["session"].ToString() == "1") ? "Yes" : "No"),
                    Rows[i]["lastip"].ToString(),
                });
                i++;
            }

            // Get Filtered Rows
            Rows = Driver.Query("SELECT COUNT(id) AS count FROM accounts{0}", Like);
            int TotalFilteredRows = Int32.Parse(Rows[0]["count"].ToString());

            // Get Total Player Count
            Rows = Driver.Query("SELECT COUNT(id) AS count FROM accounts");
            int TotalRows = Int32.Parse(Rows[0]["count"].ToString());

            // Stop Count
            if (ListPage == 1)
                Stop = RowCount;
            else
                Stop = (ListPage - 1) * Limit + RowCount;

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
            if (TotalRows / (ListPage * Limit) > 0)
            {
                float total = float.Parse(TotalRows.ToString()) / float.Parse(Limit.ToString());
                TotalPages = Int32.Parse(Math.Floor(total).ToString());
                if (TotalRows % Limit != 0)
                    TotalPages += 1;

                LastBtn.Enabled = true;
                NextBtn.Enabled = true;
            }

            // Set page number
            PageNumber.Maximum = TotalPages;
            PageNumber.Value = ListPage;

            // Update Row Count Information
            RowCountDesc.Text = String.Format("Showing {0} to {1} of {2} account(s)", ++Start, Stop, TotalFilteredRows);
            if (!String.IsNullOrWhiteSpace(Like))
                RowCountDesc.Text += " (filtered from " + TotalRows + " total account(s))";

            DataTable.Update();
        }

        /// <summary>
        /// Search OnKey Down function. Filters the List of accounts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
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
            BuildList();
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
    }
}
