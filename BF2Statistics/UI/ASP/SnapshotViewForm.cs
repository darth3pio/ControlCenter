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
        /// Builds a list of SnapshotFile objects found within the specified file path
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        private List<SnapshotFile> GetSnapshotFileInfos(string FilePath)
        {
            List<SnapshotFile> Files = new List<SnapshotFile>();

            // Grab all files with the .txt extension
            foreach (string File in Directory.EnumerateFiles(FilePath, "*.txt"))
            {
                try
                {
                    // Get filename
                    string fileName = Path.GetFileName(File);

                    // Check Formatting
                    if (!fileName.Contains('-') || !fileName.Contains('_'))
                        continue;

                    // Create new snapshot file into
                    Files.Add(new SnapshotFile(fileName));
                }
                catch
                {
                    continue;
                }
            }

            return Files;
        }

        /// <summary>
        /// Builds the snapshot file list, based on the snapshot files found within
        /// the Temp and Processed folders
        /// </summary>
        private void BuildList(string MapName = "", string Prefix = "", DateTime? StartDate = null, DateTime? EndDate = null)
        {
            // Remove old junk from the list
            SnapshotView.Items.Clear();
            SnapshotView.SuspendLayout();

            // Define what we are filtering
            bool filterMap = !String.IsNullOrWhiteSpace(MapName);
            bool filterPrefix = !String.IsNullOrWhiteSpace(Prefix);
            bool filterDate = (EndDate != null && StartDate != null);

            try
            {
                // Add each found snapshot to the snapshot view
                string path = (ViewSelect.SelectedIndex == 0) ? Paths.SnapshotTempPath : Paths.SnapshotProcPath;
                foreach (SnapshotFile file in GetSnapshotFileInfos(path).OrderByDescending(o => o.ProcessedDate))
                {
                    // Filtering of Mapname
                    if (filterMap && !MapName.Equals(file.MapName, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    // Filtering of Server Prefix
                    if (filterPrefix && !Prefix.Equals(file.ServerPrefix, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    // Filtering of Dates
                    if (filterDate && (file.ProcessedDate >= EndDate || file.ProcessedDate <= StartDate))
                        continue;

                    // Add item
                    ListViewItem Row = new ListViewItem();
                    Row.Tag = file;
                    Row.SubItems.Add(file.MapName);
                    Row.SubItems.Add(file.ServerPrefix);
                    Row.SubItems.Add(file.ProcessedDate.ToString());
                    SnapshotView.Items.Add(Row);
                }

                // If we have no items, disable a few things...
                if (SnapshotView.Items.Count == 0)
                {
                    ImportBtn.Enabled = false;
                    SnapshotView.CheckBoxes = false;

                    ListViewItem Row = new ListViewItem();
                    Row.Tag = String.Empty;
                    Row.SubItems.Add(String.Format("There are no {0}processed snapshots!", (ViewSelect.SelectedIndex == 0) ? "un" : ""));
                    Row.SubItems.Add("");
                    Row.SubItems.Add("");
                    SnapshotView.Items.Add(Row);
                }
                else
                    SnapshotView.CheckBoxes = true;
            }
            catch
            {

            }

            SnapshotView.ResumeLayout();
            SnapshotView.Update();
        }

        /// <summary>
        /// Event is fired when the Import Button is clicked
        /// </summary>
        private async void ImportBtn_Click(object sender, EventArgs e)
        {
            // List of files to process
            List<SnapshotFile> Files = new List<SnapshotFile>();
            foreach (ListViewItem I in SnapshotView.Items)
            {
                if (I.Checked)
                    Files.Add(I.Tag as SnapshotFile);
            }

            // Make sure we have a snapshot selected
            if (Files.Count == 0)
            {
                MessageBox.Show("You must select at least 1 snapshot to process.", "Error");
                return;
            }

            // Disable this form, and show the TaskForm
            this.Enabled = false;
            TaskForm.Show(this, "Importing Snapshots", "Importing Snapshots", true, ProgressBarStyle.Continuous, Files.Count);
            TaskForm.Cancelled += (s, ev) =>
            {
                TaskForm.Progress.Report(new TaskProgressUpdate("Cancelling...."));
                ImportTaskSource.Cancel();
            };

            // Setup cancellation
            ImportTaskSource = new CancellationTokenSource();
            CancellationToken CancelToken = ImportTaskSource.Token;

            // Wrap in a Task so we dont lock the GUI
            await Task.Run(() => ImportSnaphotFiles(Files, CancelToken), CancelToken);

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
        private void ImportSnaphotFiles(List<SnapshotFile> Files, CancellationToken CancelToken)
        {
            // Number of snapshots we have processed
            int processed = 0;

            // Do Work
            foreach (SnapshotFile SnapshotFile in Files)
            {
                // If we have a cancelation request
                if (CancelToken.IsCancellationRequested)
                    break;

                // Make sure we arent processing twice
                if (SnapshotFile.IsProcessed)
                    continue;

                // Process the snapshot
                try
                {
                    // Update status and run snapshot
                    TaskForm.Progress.Report(new TaskProgressUpdate(String.Format("Processing: \"{0}\"", SnapshotFile)));
                    Snapshot Snapshot = new Snapshot(File.ReadAllText(SnapshotFile.FilePath));

                    // Avoid processing exception
                    if (Snapshot.IsProcessed)
                        continue;
                    else // Do snapshot
                        Snapshot.ProcessData();

                    // Move the Temp snapshot to the Processed folder
                    File.Move(
                        Path.Combine(Paths.SnapshotTempPath, SnapshotFile.FileName), 
                        Path.Combine(Paths.SnapshotProcPath, SnapshotFile.FileName)
                    );
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
                
                // Whether we failed or succeeded, we are finished with this step 
                // and should move the progress bar 1 step
                TaskForm.Progress.Report(new TaskProgressUpdate(++processed));
            }

            // Let progress bar update to 100%
            TaskForm.Progress.Report(new TaskProgressUpdate("Done! Cleaning up..."));
            Thread.Sleep(250);
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
            try
            {
                // Get our snapshot file name
                SnapshotFile sFile = SnapshotView.SelectedItems[0].Tag as SnapshotFile;
                if (sFile == null) return;

                // Load up the snapshot, and display the Game Result Window
                Snapshot Snapshot = new Snapshot(File.ReadAllText(sFile.FilePath));
                using (GameResultForm F = new GameResultForm(Snapshot as GameResult, Snapshot.IsProcessed))
                {
                    F.ShowDialog();
                }
            }
            catch { }
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
            SnapshotFile sFile = SnapshotView.SelectedItems[0].Tag as SnapshotFile;
            if (sFile == null) return;

            // Move the selected snapshot to the opposite folder
            if (ViewSelect.SelectedIndex == 0)
            {
                File.Move(
                    Path.Combine(Paths.SnapshotTempPath, sFile.FileName), 
                    Path.Combine(Paths.SnapshotProcPath, sFile.FileName)
                );
            }
            else
            {
                File.Move(
                    Path.Combine(Paths.SnapshotProcPath, sFile.FileName),
                    Path.Combine(Paths.SnapshotTempPath, sFile.FileName)
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
            List<SnapshotFile> Files = new List<SnapshotFile>();
            foreach (ListViewItem I in SnapshotView.Items)
            {
                if (I.Checked)
                    Files.Add(I.Tag as SnapshotFile);
            }

            // Make sure we have a snapshot selected
            if (Files.Count == 0)
            {
                MessageBox.Show("You must select at least 1 snapshot to process.", "Error");
                return;
            }
            else
            {
                // Comfirm
                DialogResult Res = MessageBox.Show("Are you sure you want to delete these snapshots? This process cannot be reversed!", 
                    "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question
                );

                if (Res == DialogResult.No)
                    return;

                // Delete each snapshot file
                foreach (SnapshotFile sFile in Files)
                    File.Delete(sFile.FilePath);

                // Rebuild the list
                BuildList();
            }
        }

        private void linkLabelFilter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (SnapshotFilterForm f = new SnapshotFilterForm())
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    // Apply filtering
                    BuildList(f.MapNameBox.Text, f.PrefixBox.Text, f.StartDateTime.Value, f.EndDateTime.Value);
                }
            }
        }

        /// <summary>
        /// This class represents a snapshot file
        /// </summary>
        internal class SnapshotFile
        {
            /// <summary>
            /// Indicates whether this snapshot file exists in the Processed folder
            /// </summary>
            public bool IsProcessed
            {
                get
                {
                    string filePath = Path.Combine(Paths.SnapshotProcPath, FileName);
                    return File.Exists(filePath);
                }
            }

            /// <summary>
            /// Gets the full file path to this file
            /// </summary>
            public string FilePath
            {
                get
                {
                    string path = (IsProcessed) ? Paths.SnapshotProcPath : Paths.SnapshotTempPath;
                    return Path.Combine(path, FileName);
                }
            }

            /// <summary>
            /// Gets the filename of this snapshot file
            /// </summary>
            public string FileName { get; protected set; }

            /// <summary>
            /// Gets the Map name for this snapshot file
            /// </summary>
            public string MapName { get; protected set; }

            /// <summary>
            /// Gets the Server Prefix for this snapshot file
            /// </summary>
            public string ServerPrefix { get; protected set; }

            /// <summary>
            /// Gets the local time for when this snapshot file was saved by the server
            /// </summary>
            public DateTime ProcessedDate { get; protected set; }
            
            public SnapshotFile(string fileName)
            {
                // Seperate Server prefix from the snapshot data
                string[] data = Path.GetFileNameWithoutExtension(fileName).Split('-'); // Prefix-MapData

                // Parse Map data
                string[] parts = data[1].Split('_');
                string date = parts[parts.Length - 2] + "_" + parts[parts.Length - 1];

                // Create new snapshot file into
                FileName = fileName;
                ServerPrefix = data[0];
                MapName = String.Join("_", parts, 0, parts.Length - 2);
                ProcessedDate = DateTime.ParseExact(date, "yyyyMMdd_HHmm", CultureInfo.InvariantCulture);
            }
        }
    }
}
