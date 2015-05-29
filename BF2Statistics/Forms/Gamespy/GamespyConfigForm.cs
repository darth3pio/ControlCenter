using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;

namespace BF2Statistics
{
    public partial class GamespyConfigForm : Form
    {
        /// <summary>
        /// A list of services that provide IP Address
        /// checks
        /// </summary>
        private string[] IpServices = {
            "http://bot.whatismyipaddress.com",
            "http://icanhazip.com/",
            "http://checkip.dyndns.org/",
            "http://canihazip.com/s",
            "http://ipecho.net/plain",
            "http://ipinfo.io/ip"
        };

        public GamespyConfigForm()
        {
            InitializeComponent();

            // Load Settings
            EnableChkBox.Checked = Program.Config.GamespyEnableServerlist;
            AllowExtChkBox.Checked = Program.Config.GamespyAllowExtServers;
            AddressTextBox.Text = Program.Config.GamespyExtAddress;
            DebugChkBox.Checked = Program.Config.GamespyServerDebug;
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            // Save new settings
            Program.Config.GamespyEnableServerlist = EnableChkBox.Checked;
            Program.Config.GamespyAllowExtServers = AllowExtChkBox.Checked;
            Program.Config.GamespyExtAddress = AddressTextBox.Text;
            Program.Config.GamespyServerDebug = DebugChkBox.Checked;
            Program.Config.Save();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private async void FetchAddressBtn_Click(object sender, EventArgs e)
        {
            FetchAddressBtn.Enabled = false;
            StatusText.Show();
            StatusPic.Show();
            IPAddress addy = null;

            // WebClient can sometimes cause the GUI to lock being all slow and such, so task it up
            await Task.Run(() =>
            {
                // Use a web client to fetch our IP Address
                using (WebClient Web = new WebClient())
                {
                    // Disable Proxy to prevent slowness
                    Web.Proxy = null;

                    // Loop through each service and check for our IP
                    for (int i = 0; i < IpServices.Length; i++)
                    {
                        try
                        {
                            // Attempt to Fetch the IP address from this service
                            string result = Web.DownloadString(IpServices[i]);
                            Match match = Regex.Match(result, @"(?<IpAddress>\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})");
                            if (match.Success && IPAddress.TryParse(match.Groups["IpAddress"].Value, out addy))
                            {
                                break; // Success, we have an IP so stop here
                            }
                        }
                        catch
                        {
                            continue; // Error, Skip to next service
                        }
                    }
                }
            });

            // If we were unable to fetch the IP address, then alert the user
            if (addy == null)
            {
                StatusText.Text = "Unable to fetch external address!";
                StatusPic.Image = BF2Statistics.Properties.Resources.error;
            }
            else
            {
                StatusText.Text = "Address Fetched Successfully!";
                StatusPic.Image = BF2Statistics.Properties.Resources.check;
                AddressTextBox.Text = addy.ToString();
            }
        }
    }
}
