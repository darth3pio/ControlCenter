using System;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BF2Statistics.Properties;
using BF2Statistics.Utilities;

namespace BF2Statistics
{
    public partial class HostsFileTestForm : Form
    {
        /// <summary>
        /// The list of services we are verifying
        /// </summary>
        protected static readonly string[] Services = 
        {
            "master.gamespy.com",
            "gpcm.gamespy.com",
            "gpsp.gamespy.com",
            "battlefield2.ms14.gamespy.com",
            "bf2web.gamespy.com"
        };

        /// <summary>
        /// Our cache verifying task
        /// </summary>
        protected Task ServiceTask;

        /// <summary>
        /// Our ServiceTask cancellation token source
        /// </summary>
        private CancellationTokenSource TaskSource;

        public HostsFileTestForm()
        {
            InitializeComponent();
            TaskSource = new CancellationTokenSource();
            ServiceTask = new Task(VerifyDnsCache, TaskSource.Token);
        }

        /// <summary>
        /// This method ping's the gamepsy services and verifies that the HOSTS
        /// file redirects are working correctly
        /// </summary>
        protected void VerifyDnsCache()
        {
            // Loop through each service
            for (int i = 0; i < Services.Length; i++)
            {
                // Quit on cancel
                if (TaskSource.IsCancellationRequested)
                    return;

                // Make sure this service exists in the hosts file
                if (!HostsFile.HasEntry(Services[i]))
                {
                    SetStatus(i, "Skipped", Resources.question_button, "Entry not found in HOSTS file. Assumed redirect was not desired by user");
                    continue;
                }

                // Prepare for next service
                SetStatus(i, "Checking, Please Wait...", Resources.loading);

                // Ping server to get the IP address in the dns cache
                try
                {
                    IPAddress HostsIp = IPAddress.Parse(HostsFile.Get(Services[i]));
                    IPAddress[] Entries = Dns.GetHostAddresses(Services[i]);

                    // Check for cancel before setting a form value
                    if (TaskSource.IsCancellationRequested)
                        return;

                    // Verify correct address 
                    if (Entries.Contains(HostsIp))
                        SetStatus(i, HostsIp.ToString(), Resources.check);
                    else
                        SetStatus(i, Entries[0].ToString(), Resources.warning, "Address expected: " + HostsIp.ToString());
                }
                catch(Exception e)
                {
                    // Check for cancel before setting a form value
                    if (TaskSource.IsCancellationRequested)
                        return;

                    // No such hosts is known?
                    if(e.InnerException != null)
                        SetStatus(i, "Error Occured", Resources.error, e.InnerException.Message);
                    else
                        SetStatus(i, "Error Occured", Resources.error, e.Message);
                }
            }

            // Enable refresh btn
            if(IsHandleCreated)
                Invoke((Action)delegate { RefreshBtn.Enabled = true; });
        }

        /// <summary>
        /// This method sets the address, image, and image balloon text for the services
        /// listed in this form by service index.
        /// </summary>
        /// <param name="i">The service index</param>
        /// <param name="address">The text to display in the IP address box</param>
        /// <param name="Img">The image to display in the image box for this service</param>
        /// <param name="ImgText">The mouse over balloon text to display</param>
        private void SetStatus(int i, string address, Bitmap Img, string ImgText = "")
        {
            // Prevent exception
            if (!IsHandleCreated) return;

            // Invoke this in the thread that created the handle
            BeginInvoke((Action)delegate
            {
                switch (i)
                {
                    case 0:
                        Address1.Text = address;
                        Status1.Image = Img;
                        Tipsy.SetToolTip(Status1, ImgText);
                        break;
                    case 1:
                        Address2.Text = address;
                        Status2.Image = Img;
                        Tipsy.SetToolTip(Status2, ImgText);
                        break;
                    case 2:
                        Address3.Text = address;
                        Status3.Image = Img;
                        Tipsy.SetToolTip(Status3, ImgText);
                        break;
                    case 3:
                        Address4.Text = address;
                        Status4.Image = Img;
                        Tipsy.SetToolTip(Status4, ImgText);
                        break;
                    case 4:
                        Address5.Text = address;
                        Status5.Image = Img;
                        Tipsy.SetToolTip(Status5, ImgText);
                        break;
                }
            });
        }

        /// <summary>
        /// Event fired when the refresh button is clicked
        /// </summary>
        private async void RefreshBtn_Click(object sender, EventArgs e)
        {
            // Disable refresh spam
            RefreshBtn.Enabled = false;

            // Call Cancel Token
            if (ServiceTask.Status == TaskStatus.Running)
            {
                TaskSource.Cancel();

                // Wait for cancel to kick in
                await ServiceTask;
            }

            // Dispose Task, and disable form actions
            ServiceTask.Dispose();
            this.Enabled = false;

            // Reset the controls on this form to default
            while (Controls.Count > 0)
                Controls[0].Dispose();

            // Redraw default controls
            InitializeComponent();

            // Enable this form
            this.Enabled = true;

            // Create new
            TaskSource = new CancellationTokenSource();
            ServiceTask = new Task(VerifyDnsCache, TaskSource.Token);
            ServiceTask.Start();
        }

        /// <summary>
        /// Event fired when the Close button is clicked
        /// </summary>
        private void CloseBtn_Click(object sender, EventArgs e)
        {
            // Disable close btn
            CloseBtn.Enabled = false;

            // Close form
            this.Close();
        }

        /// <summary>
        /// Event fired when the form begins to close
        /// </summary>
        private async void HostsFileTestForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Call Cancel Token
            if (ServiceTask.Status == TaskStatus.Running)
            {
                TaskSource.Cancel();

                // Wait for cancel to kick in
                await ServiceTask;
            }

            // Dispose Task
            ServiceTask.Dispose();
        }

        /// <summary>
        /// Event fired after the form has been displayed to the user
        /// </summary>
        private async void HostsFileTestForm_Shown(object sender, EventArgs e)
        {
            await Task.Delay(250);
            ServiceTask.Start();
        }
    }
}
