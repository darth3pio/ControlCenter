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
using BF2Statistics.Net;
using BF2Statistics.Properties;
using BF2Statistics.Utilities;

namespace BF2Statistics
{
    public partial class GamespyRedirectForm : Form
    {
        /// <summary>
        /// The amount of milliseconds to delay the display of the Error tab when an error occurs
        /// </summary>
        const int ERRORPAGE_DELAY = 1500;

        /// <summary>
        /// The current step of the form's progress
        /// </summary>
        /// <remarks>
        ///   Used to track which page ID we are moving to next
        /// </remarks>
        protected int Step = 0;

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
        private IPAddress StatsServerAddress = null;

        /// <summary>
        /// The IPAddress of the Gamespy Server
        /// </summary>
        private IPAddress GamespyServerAddress = null;

        /// <summary>
        /// The current selected redirect mode
        /// </summary>
        private RedirectMode SelectedMode;

        /// <summary>
        /// The TaskStep object if a step from applying redirects fails
        /// </summary>
        private TaskStep FailedStep;

        /// <summary>
        /// Creates a new instance of GamespyRedirectForm
        /// </summary>
        public GamespyRedirectForm()
        {
            // Create Form Controls
            InitializeComponent();

            // Force the description box to fill with text
            IcsRadio.Checked = false;
            switch (Program.Config.RedirectMode)
            {
                case RedirectMode.HostsIcsFile:
                    IcsRadio.Checked = true;
                    break;
                case RedirectMode.HostsFile:
                    HostsRadio.Checked = true;
                    break;
                case RedirectMode.DnsServer:
                    DnsRadio.Checked = true;
                    break;
            }

            // Load previous settings
            Bf2webAddress.Text = Program.Config.LastStatsServerAddress;
            GamespyAddress.Text = Program.Config.LastLoginServerAddress;

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
                case "tabPageVerifyIcs":
                    // Create new progress
                    SyncProgress<TaskStep> Myprogress = new SyncProgress<TaskStep>(RedirectStatusUpdate);

                    // Apply redirects
                    IsErrorFree = await Redirector.ApplyRedirectsAsync(Myprogress);
                    if (IsErrorFree)
                    {
                        NextBtn.Enabled = true;
                    }
                    else
                    {
                        // Remove redirect if there are errors
                        await Task.Delay(ERRORPAGE_DELAY);
                        ShowHostsErrorPage();
                    }
                    break;
                case "tabPageDiagnostic":
                    // Run in a new thread of course
                    bool DiagnosticResult = await Task.Run<bool>(() => VerifyDnsCache());

                    // Switch page
                    if (DiagnosticResult)
                    {
                        NextBtn.Enabled = true;
                    }
                    else
                    {
                        // Remove redirect if there are errors
                        Redirector.RemoveRedirects();
                        await Task.Delay(ERRORPAGE_DELAY);

                        // Show Error Page
                        ShowDnsErrorPage();
                    }
                    break;
                case "tabPageSuccess":
                    PrevBtn.Visible = false;
                    CancelBtn.Visible = false;
                    NextBtn.Text = "Finish";
                    NextBtn.Enabled = true;
                    NextBtn.Location = CancelBtn.Location;
                    return;
                case "tabPageError":
                    break;
            }

            // Unlock the previos button
            if (pageControl1.SelectedTab != tabPageSelect)
                PrevBtn.Enabled = true;
        }

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

                    // Set new redirect options
                    Redirector.SetRedirectMode(SelectedMode);
                    Redirector.StatsServerAddress = StatsServerAddress;
                    Redirector.GamespyServerAddress = GamespyServerAddress;
                    break;
                case "tabPageVerifyIcs":
                case "tabPageVerifyHosts":
                    // Reset diag page
                    Status1.Image = Status2.Image = Status4.Image = Status5.Image = null;
                    Address1.Text = Address2.Text = Address4.Text = Address5.Text = "Queued";

                    // Get our next page index
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
            // Get our user input
            string Bf2Web = Bf2webAddress.Text.Trim().ToLower();
            string Gamespy = GamespyAddress.Text.Trim().ToLower();

            // Reset values
            StatsServerAddress = null;
            GamespyServerAddress = null;

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
        /// This method ping's the gamepsy services and verifies that the Redirect
        /// services are working. This method effectivly replaces the <see cref="Redirector.VerifyDNSCache()"/>
        /// method.
        /// </summary>
        protected bool VerifyDnsCache()
        {
            // Set default as success
            bool DiagnosticResult = true;

            // Set the System.Net DNS Cache refresh timeout to 1 millisecond
            ServicePointManager.DnsRefreshTimeout = 1;

            // If using System Hosts, Wait 10 seconds to allow the Windows DNS Client to catch up
            if (SelectedMode == RedirectMode.HostsFile)
            {
                for (int i = 0; i < 10; i++)
                {
                    // Get second count
                    int o = 10 - i;

                    // Make sure form wasn't cancelled
                    if (!IsHandleCreated) return false;

                    // Tell the main form thread to update the second count
                    Invoke((Action)delegate
                    {
                        labelDnsText.Text = "Verifying DNS Cache Settings in " + o + " seconds...";
                    });

                    // Wait one
                    Thread.Sleep(1000);
                }

                // Make sure form wasn't cancelled
                if (!IsHandleCreated) return false;

                // Fix header to display no time
                Invoke((Action)delegate
                {
                    labelDnsText.Text = "Verifying DNS Cache Settings";
                });
            }

            // Clear Old Entries
            Redirector.DnsCacheReport.Entries.Clear();

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
                Thread.Sleep(300);

                // Ping server to get the IP address in the dns cache
                try
                {
                    IPAddress HostsIp = (i == 3) ? StatsServerAddress : GamespyServerAddress;
                    DnsCacheResult Result = new DnsCacheResult(Services[i], HostsIp);

                    // Update Gamespy Redirector Cache
                    Redirector.DnsCacheReport.AddOrUpdate(Result);

                    // Throw bad result
                    if (Result.IsFaulted)
                    {
                        // No such hosts is known?
                        if (Result.Error.InnerException != null)
                            SetStatus(i, "Error Occured", Resources.error, Result.Error.InnerException.Message);
                        else
                            SetStatus(i, "Error Occured", Resources.error, Result.Error.Message);

                        // Flag error
                        DiagnosticResult = false;
                        continue;
                    }

                    // Verify correct address 
                    if (!Result.GotExpectedResult)
                    {
                        SetStatus(i, Result.ResultAddresses[0].ToString(), Resources.warning, "Address expected: " + HostsIp.ToString());

                        // Flag error
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

                    // Flag error
                    DiagnosticResult = false;
                }
            }

            return DiagnosticResult;
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

        #region Hosts Files

        /// <summary>
        /// Updates the Verify Hosts(ics) tab UI steps with status icons
        /// </summary>
        /// <remarks> Callback method for the Redirector.ApplyRedirectsAsync Method's IProgress.</remarks>
        /// <param name="Step"></param>
        private void RedirectStatusUpdate(TaskStep Step)
        {
            // Get the picture box number
            int PicId = (SelectedMode == RedirectMode.HostsFile) ? 0 : 6;
            PicId += Step.StepId;

            // Update the status on the GUI
            if (Step.IsFaulted)
            {
                SetHostStatus(PicId, Step.Description, Resources.cross);

                // Set Internal
                FailedStep = Step;
            }
            else
            {
                SetHostStatus(PicId, Step.Description, Resources.check);

                // Set loading icon of the next step
                if (PicId < 11)
                    SetHostStatus(++PicId, "", Resources.loading);
            }
        }

        /// <summary>
        /// Fills in the details of a Hosts(Ics) tab progress step on the GUI
        /// </summary>
        /// <param name="i">The picturebox id</param>
        /// <param name="ImgText">The tooltip message for this step image</param>
        /// <param name="Img">The image to set</param>
        private void SetHostStatus(int i, string ImgText, Bitmap Img)
        {
            // Prevent exception
            if (!IsHandleCreated) return;

            // Fetch the picture box for this step
            Control[] cons = this.Controls.Find("pictureBox" + i, true);
            PictureBox pic = cons[0] as PictureBox;

            // Set the new image
            pic.Image = Img;

            // Set the tooltip for the image control if we have one
            if (!String.IsNullOrWhiteSpace(ImgText))
                Tipsy.SetToolTip(pic, ImgText);
        }

        #endregion Hosts Files

        #region Error Pages

        private void ShowHostsErrorPage()
        {
            // Set title
            textErrLabel.Text = FailedStep.Description;

            // Set detail text based on step
            switch (FailedStep.StepId)
            {
                case 0:
                    // Unlock
                case 1:
                    // Read
                case 2:
                    // Write
                case 4:
                    // Save Changes
                    textErrDetails.Text = "Please make sure this program is being ran as an administrator, or "
                        + "modify your HOSTS file permissions allowing this program to read/modify it.";
                    break;
                case 5:
                    textErrDetails.Text = FailedStep.Error.Message ?? "";
                    break;
            }

            // Get page index
            Step = pageControl1.TabPages.IndexOf(tabPageError);

            // Set the new index
            pageControl1.SelectedIndex = Step;
        }

        private void ShowDnsErrorPage()
        {
            // Set generic description
            textErrLabel.Text = "Failed to resolve the Gamespy entries in the Windows DNS Cache";

            // Set details text
            switch (Redirector.RedirectMethod)
            {
                case RedirectMode.DnsServer:
                    textErrDetails.Text = "The Windows Dns Server settings appear to be improperly configured. "
                      + "Please double check your internet adapter settings, and try again. If you are using an "
                      + "installed DNS software, ensure that it is running and the redirects are properly configured within it.";
                    break;
                case RedirectMode.HostsFile:
                    textErrDetails.Text = "It appears that the privilages set on the HOSTS file are too strict for Windows to open. "
                      + "Read permissions must be removed to prevent Battlefield 2 from detecting the Gamespy redirects. It may be a "
                      + "better option to try one of the other two redirect options.";
                    break;
                case RedirectMode.HostsIcsFile:
                    textErrDetails.Text = "The Hosts.ics file does not seem to be affecting the Windows DNS Client. Please make "
                      + "sure that you have Internet Connection Sharing enabled on your PC, as well as the DNS Client windows feature "
                      + "in the Control Panel.";
                    break;
            }

            // Get page index
            Step = pageControl1.TabPages.IndexOf(tabPageError);

            // Set the new index
            pageControl1.SelectedIndex = Step;
        }

        #endregion Error Pages

        #region Radio Events

        private void IcsRadio_CheckedChanged(object sender, EventArgs e)
        {
            // If we arent selected, return
            if (!IcsRadio.Checked) return;

            SelectedMode = RedirectMode.HostsIcsFile;
            descTextBox.Text = "Enabling this option will create and use the hosts.ics file. This is a better option "
                + "then using the system HOSTS file because Battlefield 2 will not check the hosts.ics file  for gamespy redirects";
        }

        private void HostsRadio_CheckedChanged(object sender, EventArgs e)
        {
            // If we arent selected, return
            if (!HostsRadio.Checked) return;

            SelectedMode = RedirectMode.HostsFile;
            descTextBox.Text = "By enabling this option, this program will attempt to store the Gamespy redirects inside the system HOSTS file. "
                + "Battlefield 2 will check the hosts file for Gamespy redirects, so we must also remove READ permissions to prevent this. "
                + "This option should be used as a last resort, since removing READ permissions from the HOSTS file can cause Windows DNS server "
                + "to undo the gamespy redirects when it refreshes itself.";
        }

        private void DnsRadio_CheckedChanged(object sender, EventArgs e)
        {
            // If we arent selected, return
            if (!DnsRadio.Checked) return;

            SelectedMode = RedirectMode.DnsServer;
            descTextBox.Text = "This option is for advanced users that have setup a DNS server on their machine to automatically redirect "
                + "Gamespy services to a configured Ip address.";
        }

        #endregion Radio Events
    }
}
