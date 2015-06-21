using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BF2Statistics.Properties;
using BF2Statistics.Utilities;

namespace BF2Statistics
{
    public enum StepDirection { Forward, Backwards }

    public partial class GamespyRedirectForm : Form
    {
        /// <summary>
        /// The current step of the form's progress
        /// </summary>
        protected int Step = 0;

        /// <summary>
        /// Gets or Sets the direction of the last step
        /// </summary>
        protected StepDirection Direction;

        /// <summary>
        /// The list of services we are verifying on the Diagnostic tab
        /// </summary>
        protected static readonly string[] Services = 
        {
            "master.gamespy.com",
            "gpcm.gamespy.com",
            "battlefield2.ms14.gamespy.com",
            "bf2web.gamespy.com"
        };

        /// <summary>
        /// The IPAddress of the Stats Server
        /// </summary>
        private IPAddress StatsServerAddress = IPAddress.Loopback;

        /// <summary>
        /// The IPAddress of the Gamespy Server
        /// </summary>
        private IPAddress GamespyServerAddress = IPAddress.Loopback;

        /// <summary>
        /// The result of the Diagnostic
        /// </summary>
        protected bool DiagnosticResult = false;

        public GamespyRedirectForm()
        {
            InitializeComponent();

            // Register for Events
            pageControl1.SelectedIndexChanged += (s, e) => AfterSelectProcessing();
        }

        /// <summary>
        /// When the Step is changed, this method handles the processing of
        /// the next step
        /// </summary>
        protected async void AfterSelectProcessing()
        {
            // Disable buttons until processing is complete
            NextBtn.Enabled = false;
            PrevBtn.Enabled = false;
            bool IsErrorFree;

            // Do processing
            // Get our previous step
            switch (pageControl1.SelectedTab.Name)
            {
                case "tabPageSelect":
                    // We dont do anything here
                    NextBtn.Enabled = true;
                    break;
                case "tabPageRedirectType":
                    // We dont do much here
                    PrevBtn.Enabled = NextBtn.Enabled = true;
                    break;
                case "tabPageVerifyHosts":
                    IsErrorFree = await VerifyHosts();
                    if (IsErrorFree)
                        NextBtn.Enabled = true;
                    break;
                case "tabPageVerifyIcs":
                    IsErrorFree = await VerifyIcs();
                    if (IsErrorFree)
                        NextBtn.Enabled = true;
                    break;
                case "tabPageDiagnostic":
                    // Run in a new thread of course
                    await Task.Run(() => VerifyDnsCache());

                    // Switch page
                    if (DiagnosticResult)
                        NextBtn.Enabled = true;
                    break;
                case "tabPageSuccess":
                    PrevBtn.Visible = false;
                    CancelBtn.Visible = false;
                    NextBtn.Text = "Finish";
                    NextBtn.Enabled = true;
                    return;
                case "tabPageError":
                    break;
            }

            // Unlock the previos button
            if (pageControl1.SelectedTab != tabPageSelect)
                PrevBtn.Enabled = true;
        }

        #region Host Verification Tasks

        private Task<bool> VerifyIcs()
        {
            return Task.Run(() =>
            {
                for (int i = 8; i < 13; i++ )
                {
                    SetHostStatus(i, "", Resources.loading);
                    Thread.Sleep(1000);
                    SetHostStatus(i, "Success", Resources.check);
                }

                return true;
            });
        }


        private Task<bool> VerifyHosts()
        {
            return Task.Run(() =>
            {
                for (int i = 1; i < 8; i++)
                {
                    SetHostStatus(i, "", Resources.loading);
                    Thread.Sleep(1000);
                    SetHostStatus(i, "Success", Resources.check);
                }

                return true;
            });
        }

        #endregion

        #region Main Button Events

        /// <summary>
        /// Event fired when the Previous button is pushed
        /// </summary>
        private void PrevBtn_Click(object sender, EventArgs e)
        {
            // Get our previous step
            switch (pageControl1.SelectedTab.Name)
            {
                case "tabPageSelect": 
                    // Cannot go backwards from here
                    break;
                case "tabPageRedirectType":
                case "tabPageVerifyHosts":
                    Step--;
                    break;
                case "tabPageDiagnostic":
                case "tabPageSuccess":
                case "tabPageError":
                    Step = pageControl1.TabPages.IndexOf(tabPageRedirectType);
                    break;
            }

            // Make sure we have a change before changing our index
            if (Step == pageControl1.SelectedIndex)
                return;

            // Set the new direction
            Direction = StepDirection.Backwards;

            // Set the new index
            pageControl1.SelectedIndex = Step;
        }

        /// <summary>
        /// Event fired when the Next button is pushed
        /// </summary>
        private async void NextBtn_Click(object sender, EventArgs e)
        {
            // Get our next step
            switch (pageControl1.SelectedTab.Name)
            {
                case "tabPageSelect":
                    // Make sure we are going to redirect something...
                    if (!Bf2webCheckbox.Checked && !GpcmCheckbox.Checked)
                    {
                        MessageBox.Show(
                            "Please select at least 1 redirect option",
                            "Select an Option", MessageBoxButtons.OK, MessageBoxIcon.Information
                        );
                        return;
                    }

                    // Show loading status so the user sees progress
                    StatusPic.Visible = StatusText.Visible = true;
                    NextBtn.Enabled = false;

                    // Validate hostnames, and convert them to IPAddresses
                    bool IsValid = await GetRedirectAddressesAsync();
                    if (IsValid)
                        Step = pageControl1.TabPages.IndexOf(tabPageRedirectType);

                    // Hide status icon and next
                    StatusPic.Visible = StatusText.Visible = false;
                    break;
                case "tabPageRedirectType":
                    if (IcsRadio.Checked)
                        Step = pageControl1.TabPages.IndexOf(tabPageVerifyIcs);
                    else if (HostsRadio.Checked)
                        Step = pageControl1.TabPages.IndexOf(tabPageVerifyHosts);
                    else
                        Step = pageControl1.TabPages.IndexOf(tabPageDiagnostic);
                    break;
                case "tabPageVerifyIcs":
                case "tabPageVerifyHosts":
                    Step = pageControl1.TabPages.IndexOf(tabPageDiagnostic);
                    break;
                case "tabPageDiagnostic":
                    // All processing is done in the after select
                    Step = pageControl1.TabPages.IndexOf(tabPageSuccess);
                    break;
                case "tabPageError":
                case "tabPageSuccess":
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    break;
            }

            // Make sure we have a change before changing our index
            if (Step == pageControl1.SelectedIndex)
                return;

            // Set direction
            Direction = StepDirection.Forward;

            // Set the new index
            pageControl1.SelectedIndex = Step;
        }

        /// <summary>
        /// Event fired if the Cancel Button is pressed
        /// </summary>
        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Select Redirects

        /// <summary>
        /// Fetches the IP addresses of the hostnames provided in the redirect address boxes,
        /// and returns whether the provided hostnames were valid
        /// </summary>
        /// <returns>returns whether the provided hostnames were valid and an IP address could be fetched</returns>
        private Task<bool> GetRedirectAddressesAsync()
        {
            string Bf2Web = Bf2webAddress.Text.Trim().ToLower();
            string Gamespy = GpcmAddress.Text.Trim().ToLower();

            // Return this processing task to be awaited on
            return Task.Run<bool>(() =>
            {
                // Stats Server
                if (Bf2webCheckbox.Checked && Bf2Web != "localhost")
                {
                    // Need at least 8 characters
                    if (Bf2Web.Length < 8)
                    {
                        MessageBox.Show(
                            "You must enter an valid IP address or Hostname in the Address box!",
                            "Invalid Address", MessageBoxButtons.OK, MessageBoxIcon.Warning
                        );
                        return false;
                    }

                    // Reslove hostname if we were not provided an IP address
                    if (!Networking.TryGetIpAddress(Bf2Web, out StatsServerAddress))
                    {
                        MessageBox.Show(
                            "Stats server redirect address is invalid, or doesnt exist. Please enter a valid, and existing IPv4/6 or Hostname.",
                            "Invalid Address", MessageBoxButtons.OK, MessageBoxIcon.Warning
                        );

                        return false;
                    }
                }

                // Gamespy Server
                if (GpcmCheckbox.Checked && Gamespy != "localhost")
                {
                    // Need at least 8 characters
                    if (Gamespy.Length < 8)
                    {
                        MessageBox.Show(
                            "You must enter an valid IP address or Hostname in the Address box!",
                            "Invalid Address", MessageBoxButtons.OK, MessageBoxIcon.Warning
                        );
                        return false;
                    }

                    // Reslove hostname if we were not provided an IP address
                    if (!Networking.TryGetIpAddress(Gamespy, out GamespyServerAddress))
                    {
                        MessageBox.Show(
                            "Gamespy redirect address is invalid, or doesnt exist. Please enter a valid, and existing IPv4/6 or Hostname.",
                            "Invalid Address", MessageBoxButtons.OK, MessageBoxIcon.Warning
                        );

                        return false;
                    }
                }

                return true;
            });
        }

        #endregion

        #region Diagnostics

        /// <summary>
        /// This method ping's the gamepsy services and verifies that the HOSTS
        /// file redirects are working correctly
        /// </summary>
        protected void VerifyDnsCache()
        {
            // Set default as success
            DiagnosticResult = true;

            // Loop through each service
            for (int i = 0; i < Services.Length; i++)
            {
                // Make sure this service is enabled
                if ((!Bf2webCheckbox.Checked && i == 3) || (!GpcmCheckbox.Checked && i != 3))
                {
                    SetStatus(i, "Skipped", Resources.question_button, "Redirect was not enabled by user");
                    continue;
                }

                // Prepare for next service
                SetStatus(i, "Checking, Please Wait...", Resources.loading);
                Thread.Sleep(500);

                // Ping server to get the IP address in the dns cache
                try
                {
                    IPAddress HostsIp = (i == 3) ? StatsServerAddress : GamespyServerAddress;
                    IPAddress[] Entries = Dns.GetHostAddresses(Services[i]);

                    // Verify correct address 
                    if (!Entries.Contains(HostsIp))
                    {
                        SetStatus(i, Entries[0].ToString(), Resources.warning, "Address expected: " + HostsIp.ToString());
                        DiagnosticResult = false;
                    }
                    else
                        SetStatus(i, HostsIp.ToString(), Resources.check);
                }
                catch (Exception e)
                {
                    // No such hosts is known?
                    if (e.InnerException != null)
                        SetStatus(i, "Error Occured", Resources.error, e.InnerException.Message);
                    else
                        SetStatus(i, "Error Occured", Resources.error, e.Message);
                }
            }
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
            Invoke((Action)delegate
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
                        Address4.Text = address;
                        Status4.Image = Img;
                        Tipsy.SetToolTip(Status4, ImgText);
                        break;
                    case 3:
                        Address5.Text = address;
                        Status5.Image = Img;
                        Tipsy.SetToolTip(Status5, ImgText);
                        break;
                }
            });
        }

        #endregion

        private void SetHostStatus(int i, string ImgText, Bitmap Img)
        {
            // Prevent exception
            if (!IsHandleCreated) return;

            // Invoke this in the thread that created the handle
            Invoke((Action)delegate
            {
                Control[] cons = this.Controls.Find("pictureBox" + i, true);
                PictureBox pic = cons[0] as PictureBox;

                pic.Image = Img;

                if (!String.IsNullOrWhiteSpace(ImgText))
                    Tipsy.SetToolTip(pic, ImgText);
            });
        }

        /// <summary>
        /// Event fired when the form begins to close
        /// </summary>
        private void GamespyRedirectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }
    }
}
