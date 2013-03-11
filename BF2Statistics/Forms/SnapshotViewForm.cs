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
using System.Windows.Forms;
using BF2Statistics.ASP;
using BF2Statistics.ASP.Requests;

namespace BF2Statistics
{
    public partial class SnapshotViewForm : Form
    {
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

            // Order snapshots by timestamp
            var Sorted = from _File in Files
                         let parts = _File.Split('_')
                         let date = int.Parse(parts[parts.Length - 2])
                         let time = int.Parse(parts[parts.Length - 1].Replace(".txt", ""))
                         orderby date, time ascending
                         select _File;

            // Loop through each snapshot, and process it
            UpdateProgressForm.ShowScreen("Importing Snapshots");
            this.Enabled = false;
            int Selected = Files.Count;
            int i = 1;

            // Do Work
            foreach (string Snapshot in Sorted)
            {
                try
                {
                    // Update status and run snapshot
                    UpdateProgressForm.Status( String.Format("Importing {0} of {1} snapshot(s)...", i, Selected));
                    Snapshot Snap = new Snapshot(File.ReadAllText(Path.Combine(SnapshotPost.TempPath, Snapshot)));

                    // Start Timer
                    Stopwatch Timer = new Stopwatch();
                    Timer.Start();

                    // Do snapshot
                    Snap.Process();

                    // Move the Temp snapshot to the Processed folder
                    File.Move(Path.Combine(SnapshotPost.TempPath, Snapshot), Path.Combine(SnapshotPost.ProcPath, Snapshot));

                    // increment
                    i++;

                    // Sleep to prevent snapshots from processing "too fast" and causing sql errors with timestamps not being unique
                    if (Timer.ElapsedMilliseconds < 1000)
                        Thread.Sleep(Convert.ToInt32((1001 - Timer.ElapsedMilliseconds)));
                }
                catch (Exception E)
                {
                    MessageBox.Show("An Error occurred!\r\n\r\nMessage: "+ E.Message, "Snapshot Error");
                    //UpdateProgressForm.CloseForm();
                    //this.Enabled = true;
                    //return;
                }
            }

            // Add each found snapshot to the snapshot view
            SnapshotView.Items.Clear();
            BuildList();

            UpdateProgressForm.CloseForm();
            this.Enabled = true;
            this.Focus();
        }

        private void BuildList()
        {
            // Add each found snapshot to the snapshot view
            foreach (string File in Directory.EnumerateFiles(SnapshotPost.TempPath))
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
