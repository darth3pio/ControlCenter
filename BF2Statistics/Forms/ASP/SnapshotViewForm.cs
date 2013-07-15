using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using BF2Statistics.ASP;
using BF2Statistics.ASP.Requests;

namespace BF2Statistics
{
    public partial class SnapshotViewForm : Form
    {
        private static BackgroundWorker bWorker;

        public SnapshotViewForm()
        {
            InitializeComponent();

            // Fill the list of unprocessed snapshots
            BuildList();
        }

        private void ImportBtn_Click(object sender, EventArgs e)
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

            this.Enabled = false;

            // Initialize Background worker
            bWorker = new BackgroundWorker();
            bWorker.WorkerSupportsCancellation = true;
            bWorker.DoWork += new DoWorkEventHandler(bWorker_DoWork);
            bWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bWorker_RunWorkerCompleted);
            bWorker.RunWorkerAsync(Files);
        }

        private void bWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BeginInvoke((Action)delegate
            {
                // Add each found snapshot to the snapshot view
                SnapshotView.Items.Clear();
                BuildList();

                TaskForm.CloseForm();
                this.Enabled = true;
                this.Focus();
            });
        }

        private void bWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Loop through each snapshot, and process it
            List<string> Files = e.Argument as List<string>;
            TaskForm.Show(this, "Importing Snapshots", "Importing Snapshots", true, ProgressBarStyle.Blocks, Files.Count); 
            int Selected = Files.Count;
            int i = 1;

            // Order snapshots by timestamp
            var Sorted = from _File in Files
                         let parts = _File.Split('_')
                         let date = int.Parse(parts[parts.Length - 2])
                         let time = int.Parse(parts[parts.Length - 1].Replace(".txt", ""))
                         orderby date, time ascending
                         select _File;

            // Do Work
            foreach (string Snapshot in Sorted)
            {
                // If we have a cancelation request
                if (bWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                try
                {
                    // Parse date of snapshot
                    string[] Parts = Snapshot.Split('_');
                    string D = Parts[Parts.Length - 2] + "_" + Parts[Parts.Length - 1].Replace(".txt", "");
                    DateTime Date = DateTime.ParseExact(D, "yyyyMMdd_HHmm", CultureInfo.InvariantCulture);

                    // Update status and run snapshot
                    TaskForm.UpdateStatus(String.Format("Processing: \"{0}\"", Snapshot));
                    Snapshot Snap = new Snapshot(File.ReadAllText(Path.Combine(Paths.SnapshotTempPath, Snapshot)), Date);

                    // Start Timer
                    Stopwatch Timer = new Stopwatch();
                    Timer.Start();

                    // Do snapshot
                    Snap.Process();

                    // Move the Temp snapshot to the Processed folder
                    File.Move(Path.Combine(Paths.SnapshotTempPath, Snapshot), Path.Combine(Paths.SnapshotProcPath, Snapshot));

                    // increment
                    i++;

                    // Update progress
                    TaskForm.ProgressBarStep();

                    // Slow thread to let progress update
                    Thread.Sleep(250);
                }
                catch (Exception E)
                {
                    ExceptionForm Form = new ExceptionForm(E, true);
                    Form.Message = "An exception was thrown while trying to import the snapshot."
                        + "If you click Continue, the application will continue proccessing the remaining "
                        + "snapshot files. If you click Quit, the operation will be aborted.";
                    DialogResult Result = Form.ShowDialog();

                    if (Result == System.Windows.Forms.DialogResult.Abort)
                        break;
                }
            }

            // Let progress bar update to 100%
            TaskForm.UpdateStatus("Done! Cleaning up...");
            Thread.Sleep(500);
        }

        private void BuildList()
        {
            // Add each found snapshot to the snapshot view
            foreach (string File in Directory.EnumerateFiles(Paths.SnapshotTempPath))
            {
                ListViewItem Row = new ListViewItem();
                Row.SubItems.Add(Path.GetFileName(File));
                SnapshotView.Items.Add(Row);
            }

            // If we have no items, disable a few things...
            if (SnapshotView.Items.Count == 0)
            {
                ImportBtn.Enabled = false;
                SnapshotView.CheckBoxes = false;

                ListViewItem Row = new ListViewItem();
                Row.SubItems.Add("There are no unprocessed snapshots!");
                SnapshotView.Items.Add(Row);
            }
        }

        private void SelectAllBtn_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem I in SnapshotView.Items)
                I.Checked = true;

            SnapshotView.Update();
        }

        private void SelectNoneBtn_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem I in SnapshotView.Items)
                I.Checked = false;

            SnapshotView.Update();
        }
    }
}
