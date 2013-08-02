using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BF2Statistics.ASP;

namespace BF2Statistics
{
    public partial class EAStatsImportForm : Form
    {
        /// <summary>
        /// Background worker for importing player stats
        /// </summary>
        protected BackgroundWorker bWorker;

        /// <summary>
        /// Constructor
        /// </summary>
        public EAStatsImportForm()
        {
            InitializeComponent();
            bWorker = new BackgroundWorker();

            // Hide alert form if redirects are disabled
            if (!MainForm.RedirectsEnabled)
            {
                PanelAlert.Hide();
                ImportBtn.Enabled = true;
            }
        }

        /// <summary>
        /// Import Stats Button Click Event
        /// </summary>
        private void ImportBtn_Click(object sender, EventArgs e)
        {
            // Make sure PID text box is a valid PID
            if (!Validator.IsValidPID(PidTextBox.Text))
            {
                MessageBox.Show("The player ID entered is NOT a valid PID. Please try again.", 
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Make sure the PID doesnt exist already
            int Pid = Int32.Parse(PidTextBox.Text);
            if (ASPServer.Database.PlayerExists(Pid))
            {
                MessageBox.Show("The player ID entered already exists.",
                    "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Show Task Form
            TaskForm.Show(this, "Import EA Stats", "Importing EA Stats...", ProgressBarStyle.Blocks, 13);

            // Setup the worker
            bWorker.WorkerSupportsCancellation = false;
            bWorker.WorkerReportsProgress = true;

            // Run Worker
            bWorker.DoWork += new DoWorkEventHandler(ASPServer.Database.ImportEaStats);
            bWorker.ProgressChanged += new ProgressChangedEventHandler(bWorker_ProgressChanged);
            bWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bWorker_RunWorkerCompleted);
            bWorker.RunWorkerAsync(PidTextBox.Text);
        }

        /// <summary>
        /// Finishes the import process
        /// </summary>
        private void bWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Close the task form
            TaskForm.CloseForm();

            // Close this form!
            if (e.Error != null)
            {
                Exception E = e.Error as Exception;
                MessageBox.Show(
                    "An error occured while trying to import player stats."
                    + Environment.NewLine + Environment.NewLine + E.Message,
                    "Import Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error
                );
                this.Close();
                return;
            }

            Notify.Show("Player Imported Successfully!", AlertType.Success);
            this.Close();
        }

        /// <summary>
        /// Updates the Task Form Progress
        /// </summary>
        private void bWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            TaskForm.ProgressBarStep();
            TaskForm.UpdateStatus(e.UserState.ToString());
        }
    }
}
