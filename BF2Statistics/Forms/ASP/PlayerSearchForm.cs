using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;
using System.IO;
using System.Xml;
using System.Reflection;
using BF2Statistics.ASP;
using BF2Statistics.Database;
using BF2Statistics.Database.QueryBuilder;

namespace BF2Statistics
{
    public partial class PlayerSearchForm : Form
    {
        /// <summary>
        /// Our database connection
        /// </summary>
        private StatsDatabase Driver;

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
        /// <summary>
        /// Current executing Assembly
        /// </summary>
        Assembly Me = Assembly.GetExecutingAssembly();

        /// <summary>
        /// Constructor
        /// </summary>
        public PlayerSearchForm()
        {
            InitializeComponent();

            // Establish DB connection
            try
            {
                Driver = new StatsDatabase();
            }
            catch (DbConnectException Ex)
            {
                ASPServer.Stop();
                ExceptionForm.ShowDbConnectError(Ex);
                Load += (s, e) => Close(); // Close form
                return;
            }

            // Initialize sorting
            SortedCol = DataTable.Columns[1];
            SortedCol.HeaderCell.SortGlyphDirection = SortOrder.Ascending;
            LimitSelect.SelectedIndex = 2;
        }

        /// <summary>
        /// Fills the DataGridView with a list of players
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
            SelectQueryBuilder Query = new SelectQueryBuilder(Driver);
            Query.SelectColumns("id", "name", "clantag", "rank", "score", "country", "permban");
            Query.SelectFromTable("player");
            Query.AddOrderBy(SortedCol.Name, ((SortDir == ListSortDirection.Ascending) ? Sorting.Ascending : Sorting.Descending));
            Query.Limit(Limit, Start);

            // User entered search
            if (!String.IsNullOrWhiteSpace(Like))
                Where = Query.AddWhere("name", Comparison.Like, "%" + Like + "%");

            // Clear out old junk
            DataTable.Rows.Clear();

            // Add players to data grid
            int RowCount = 0;
            foreach (Dictionary<string, object> Row in Driver.QueryReader(Query.BuildCommand()))
            {
                DataTable.Rows.Add(new object[] {
                    Image.FromStream(Me.GetManifestResourceStream("BF2Statistics.Resources.rank_" + Row["rank"].ToString() + "icon.gif")),
                    Row["id"].ToString(),
                    Row["name"].ToString(),
                    Row["clantag"].ToString(),
                    Row["score"].ToString(),
                    Row["country"].ToString(),
                    Row["permban"].ToString(),
                });
                RowCount++;
            }

            // Get Filtered Rows
            Query = new SelectQueryBuilder(Driver);
            Query.SelectCount();
            Query.SelectFromTable("player");
            if (Where != null)
                Query.AddWhere(Where);
            Rows = Driver.ExecuteReader(Query.BuildCommand());
            int TotalFilteredRows = Int32.Parse(Rows[0]["count"].ToString());

            // Get Total Player Count
            Query = new SelectQueryBuilder(Driver);
            Query.SelectCount();
            Query.SelectFromTable("player");
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
                "Showing {0} to {1} of {2} player{3}",
                ++Start,
                Stop,
                TotalFilteredRows,
                ((TotalFilteredRows > 1) ? "s " : " ")
            );

            // Add Total row count
            if (!String.IsNullOrWhiteSpace(Like))
                RowCountDesc.Text += String.Format("(filtered from " + TotalRows + " total player{0})", ((TotalRows > 1) ? "s" : ""));

            // Update
            DataTable.Update();
        }

        /// <summary>
        /// Search OnKey Down function. Filters the List of players
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
        /// Event fired when a player is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataTable_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            int Pid = Int32.Parse(DataTable.Rows[e.RowIndex].Cells[1].Value.ToString());
            PlayerEditForm Form = new PlayerEditForm(Pid);
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

        /// <summary>
        /// Builds the list when the PageNumber numeric upDown is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PageNumber_ValueChanged(object sender, EventArgs e)
        {
            int Val = Int32.Parse(PageNumber.Value.ToString());
            if (Val != ListPage)
            {
                ListPage = Val;
                BuildList();
            }
        }

        #region Context Menu

        /// <summary>
        /// Fired everytime the context menu is opening. It disables some options
        /// if there is no player selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (DataTable.SelectedRows.Count == 0)
            {
                deletePlayerToolStripMenuItem.Enabled = false;
                exportPlayerToolStripMenuItem.Enabled = false;
            }
            else
            {
                deletePlayerToolStripMenuItem.Enabled = true;
                exportPlayerToolStripMenuItem.Enabled = true;
            }
        }

        /// <summary>
        /// Delete Player Menu Item Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deletePlayerMenuItem_Click(object sender, EventArgs e)
        {
            // Get players ID and Nick
            int Pid = Int32.Parse(DataTable.SelectedRows[0].Cells[1].Value.ToString());
            string Name = DataTable.SelectedRows[0].Cells[2].Value.ToString();

            // Show confirm box before deleting
            DialogResult Result = MessageBox.Show(
                String.Format("Are you sure you want to permanently delete player \"{0}\" ({1})?", Name, Pid),
                "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning
            );

            // If confirmed
            if (Result == DialogResult.OK)
            {
                try
                {
                    TaskForm.Show(this, "Delete Player", "Deleting Player \"" + Name + "\"", false);
                    Driver.DeletePlayer(Pid, true);
                    BuildList();
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
                }
            }
        }

        /// <summary>
        /// Export player menu item click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportPlayerMenuItem_Click(object sender, EventArgs e)
        {
            // Get players ID and Nick
            int Pid = Int32.Parse(DataTable.SelectedRows[0].Cells[1].Value.ToString());
            string Name = DataTable.SelectedRows[0].Cells[2].Value.ToString();

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
                    Driver.ExportPlayerXml(sPath, Pid, Name);
                    Notify.Show("Player Exported Successfully", String.Format("{0} ({1})", Name, Pid), AlertType.Success);
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
        /// Exports player XML sheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fromPlayerExportSheetMenuItem_Click(object sender, EventArgs e)
        {
            // Create export directory if it doesnt exist yet
            string sPath = Path.Combine(Paths.DocumentsFolder, "Player Exports");
            if (!Directory.Exists(sPath))
                Directory.CreateDirectory(sPath);

            // Show dialog
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.InitialDirectory = sPath;
            Dialog.Title = "Select Player Import File";
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Driver.ImportPlayerXml(Dialog.FileName);
                    Notify.Show("Player Imported Successfully", "Operation Successful", AlertType.Success);
                    BuildList();
                }
                catch (Exception E)
                {
                    ExceptionForm EForm = new ExceptionForm(E, false);
                    EForm.Message = "Unable to import player because an exception was thrown!";
                    EForm.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Imports players EA stats
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fromEAStatsServerMenuItem_Click(object sender, EventArgs e)
        {
            EAStatsImportForm Form = new EAStatsImportForm();
            Form.ShowDialog();
            BuildList();
        }

        #endregion

        /// <summary>
        /// Closes the database connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerSearchForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Driver.Dispose();
            }
            catch (ObjectDisposedException) { }
        }
    }
}
