using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BF2Statistics.ASP.StatsProcessor;
using BF2Statistics.Database;
using BF2Statistics.Web;

namespace BF2Statistics
{
    public partial class SnapshotViewForm : Form
    {
        /// <summary>
        /// Our cancellation token used if we want to cancel the import
        /// </summary>
        private CancellationTokenSource ImportTaskSource;

        public SnapshotViewForm()
        {
            InitializeComponent();

            // Show / Hide warning
            if (HttpServer.IsRunning)
                ServerOfflineWarning.Hide();

            // Fill the list of unprocessed snapshots
            ViewSelect.SelectedIndex = 0;
        }

        /// <summary>
        /// Event is fired when the Import Button is clicked
        /// </summary>
        private async void ImportBtn_Click(object sender, EventArgs e)
        {
            // List of files to process
            List<string> Files = new List<string>();
            foreach (ListViewItem I in SnapshotView.Items)
            {
                if (I.Checked)
                    Files.Add(I.SubItems[1].Text);
            }

            // Make sure we have a snapshot selected
            if (Files.Count == 0)
            {
                MessageBox.Show("You must select at least 1 snapshot to process.", "Error");
                return;
            }

            // Disable this form, and show the TaskForm
            this.Enabled = false;
            TaskForm.Show(this, "Importing Snapshots", "Importing Snapshots", true, ProgressBarStyle.Blocks, Files.Count);
            TaskForm.Cancelled += (s, ev) =>
            {
                TaskForm.UpdateStatus("Cancelling....");
                ImportTaskSource.Cancel();
            };

            // Setup cancellation
            ImportTaskSource = new CancellationTokenSource();
            CancellationToken CancelToken = ImportTaskSource.Token;

            // Wrap in a Task so we dont lock the GUI
            await Task.Factory.StartNew(() => ImportSnaphotFiles(Files, CancelToken), CancelToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            // Let progress bar update to 100%
            TaskForm.UpdateStatus("Done! Cleaning up...");

            // Update the snapshots found close task form
            BuildList();
            TaskForm.CloseForm();
            this.Enabled = true;
            this.Focus();
            
        }

        /// <summary>
        /// Imports the provided snapshot files into the stats database
        /// </summary>
        /// <param name="Files">List of snapshot file paths to import</param>
        /// <param name="CancelToken">Cancellation token, to cancel the import</param>
        private void ImportSnaphotFiles(List<string> Files, CancellationToken CancelToken)
        {
            // Order snapshots by timestamp
            var Sorted = from _File in Files
                         let parts = _File.Split('_')
                         let date = int.Parse(parts[parts.Length - 2])
                         let time = int.Parse(parts[parts.Length - 1].Replace(".txt", ""))
                         orderby date, time ascending
                         select _File;

            // Do Work
            foreach (string SnapshotFile in Sorted)
            {
                // If we have a cancelation request
                if (CancelToken.IsCancellationRequested)
                    break;

                // Process the snapshot
                try
                {
                    // Update status and run snapshot
                    TaskForm.UpdateStatus(String.Format("Processing: \"{0}\"", SnapshotFile));
                    Snapshot Snapshot = new Snapshot(File.ReadAllText(Path.Combine(Paths.SnapshotTempPath, SnapshotFile)));

                    // Do snapshot
                    Snapshot.ProcessData();

                    // Move the Temp snapshot to the Processed folder
                    File.Move(Path.Combine(Paths.SnapshotTempPath, SnapshotFile), Path.Combine(Paths.SnapshotProcPath, SnapshotFile));
                }
                catch (Exception E)
                {
                    using (ExceptionForm Form = new ExceptionForm(E, true))
                    {
                        Form.Message = "An exception was thrown while trying to import the snapshot."
                            + "If you click Continue, the application will continue proccessing the remaining "
                            + "snapshot files. If you click Quit, the operation will be aborted.";
                        DialogResult Result = Form.ShowDialog();

                        // User Abort
                        if (Result == DialogResult.Abort)
                            break;
                    }
                }
                finally
                {
                    // Whether we failed or succeeded, we are finished with this step
                    TaskForm.ProgressBarStep();
                }
            }
        }

        /// <summary>
        /// Builds the snapshot file list, based on the snapshot files found within
        /// the Temp and Processed folders
        /// </summary>
        private void BuildList()
        {
            SnapshotView.Items.Clear();

            // Add each found snapshot to the snapshot view
            string path = (ViewSelect.SelectedIndex == 0) ? Paths.SnapshotTempPath : Paths.SnapshotProcPath;
            foreach (string File in Directory.EnumerateFiles(path, "*.txt"))
            {
                ListViewItem Row = new ListViewItem();
                Row.Tag = Path.GetFileName(File);
                Row.SubItems.Add(Path.GetFileName(File));
                SnapshotView.Items.Add(Row);
            }

            // If we have no items, disable a few things...
            if (SnapshotView.Items.Count == 0)
            {
                ImportBtn.Enabled = false;
                SnapshotView.CheckBoxes = false;

                ListViewItem Row = new ListViewItem();
                Row.Tag = String.Empty;
                Row.SubItems.Add(String.Format("There are no {0}processed snapshots!", (ViewSelect.SelectedIndex == 1) ? "un" : ""));
                SnapshotView.Items.Add(Row);
            }
            else
                SnapshotView.CheckBoxes = true;

            SnapshotView.Update();
        }

        /// <summary>
        /// Event fired when the Select All button is clicked
        /// </summary>
        private void SelectAllBtn_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem I in SnapshotView.Items)
                I.Checked = true;

            SnapshotView.Update();
        }

        /// <summary>
        /// Event fired when the Select None button is clicked
        /// </summary>
        private void SelectNoneBtn_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem I in SnapshotView.Items)
                I.Checked = false;

            SnapshotView.Update();
        }

        /// <summary>
        /// Event fired when the user right clicks, to open the context menu
        /// </summary>
        private void SnapshotView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && SnapshotView.FocusedItem.Bounds.Contains(e.Location))
            {
                MenuStrip.Show(Cursor.Position);
            }
        }

        /// <summary>
        /// Event fire when the Details item menu is selected from the
        /// context menu
        /// </summary>
        private void Details_MenuItem_Click(object sender, EventArgs e)
        {
            // Get our snapshot file name
            string Name = SnapshotView.SelectedItems[0].Tag.ToString();
            if (String.IsNullOrEmpty(Name))
                return;

            // Parse date of snapshot, and build the file file location
            string _File = (ViewSelect.SelectedIndex == 0) 
                ? Path.Combine(Paths.SnapshotTempPath, Name) 
                : Path.Combine(Paths.SnapshotProcPath, Name);

            // Load up the snapshot, and display the Game Result Window
            Snapshot Snapshot = new Snapshot(File.ReadAllText(_File));
            using (GameResultForm F = new GameResultForm(Snapshot as GameResult, Snapshot.IsProcessed))
            {
                F.ShowDialog();
            }
        }

        /// <summary>
        /// Event fired when the snapshot view type is changed
        /// </summary>
        private void ViewSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Enable/Disable features based in mode
            bool Enable = (ViewSelect.SelectedIndex == 0 && HttpServer.IsRunning);
            SnapshotView.CheckBoxes = Enable;
            ImportBtn.Enabled = Enable;
            SelectAllBtn.Enabled = Enable;
            SelectNoneBtn.Enabled = Enable;
            BuildList();

            // Set menu item text
            if (Enable)
                MoveSnapshotMenuItem.Text = "Move to Processed";
            else
                MoveSnapshotMenuItem.Text = "Move to UnProcessed";

            // Set textbox text
            if (ViewSelect.SelectedIndex == 0)
            {
                textBox1.Text = "Below is a list of  snapshots that have not been imported into the database. "
                    + "You can select which snapshots you wish to try and import below";
            }
            else
            {
                textBox1.Text = "Below is a list of  snapshots that have been successfully imported into the database. ";
            }
        }

        /// <summary>
        /// Event fired when the Move Snapshot menu item is clicked from the drop down menu (right click)
        /// </summary>
        private void MoveSnapshotMenuItem_Click(object sender, EventArgs e)
        {
            // Get our snapshot file name
            string FileName = SnapshotView.SelectedItems[0].Tag.ToString();
            if (String.IsNullOrEmpty(FileName))
                return;

            // Move the selected snapshot to the opposite folder
            if (ViewSelect.SelectedIndex == 0)
            {
                File.Move(
                    Path.Combine(Paths.SnapshotTempPath, FileName), 
                    Path.Combine(Paths.SnapshotProcPath, FileName)
                );
            }
            else
            {
                File.Move(
                    Path.Combine(Paths.SnapshotProcPath, FileName), 
                    Path.Combine(Paths.SnapshotTempPath, FileName)
                );
            }

            BuildList();
        }

        /// <summary>
        /// Event fired when the Delete button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            // List of files to process
            List<string> Files = new List<string>();
            foreach (ListViewItem I in SnapshotView.Items)
            {
                if (I.Checked)
                    Files.Add(I.SubItems[1].Text);
            }

            // Make sure we have a snapshot selected
            if (Files.Count == 0)
            {
                MessageBox.Show("You must select at least 1 snapshot to process.", "Error");
                return;
            }
            else
            {
                DialogResult Res = MessageBox.Show("Are you sure you want to delete these snapshots? This process cannot be reversed!", 
                    "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question
                );

                if (Res == DialogResult.No)
                    return;

                foreach (string Name in Files)
                {
                    string fName = (ViewSelect.SelectedIndex == 0)
                        ? Path.Combine(Paths.SnapshotTempPath, Name)
                        : Path.Combine(Paths.SnapshotProcPath, Name);

                    File.Delete(fName);
                }

                BuildList();
            }
        }
    }
}
