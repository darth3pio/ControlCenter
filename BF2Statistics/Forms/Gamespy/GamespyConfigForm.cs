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
            EnableChkBox.Checked = MainForm.Config.GamespyEnableServerlist;
            AllowExtChkBox.Checked = MainForm.Config.GamespyAllowExtServers;
            AddressTextBox.Text = MainForm.Config.GamespyExtAddress;
            DebugChkBox.Checked = MainForm.Config.GamespyServerDebug;
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            // Save new settings
            MainForm.Config.GamespyEnableServerlist = EnableChkBox.Checked;
            MainForm.Config.GamespyAllowExtServers = AllowExtChkBox.Checked;
            MainForm.Config.GamespyExtAddress = AddressTextBox.Text;
            MainForm.Config.GamespyServerDebug = DebugChkBox.Checked;
            MainForm.Config.Save();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private async void FetchAddressBtn_Click(object sender, EventArgs e)
        {
            FetchAddressBtn.Enabled = false;
            StatusText.Show();
            StatusPic.Show();
            IPAddress addy = null;

            // Loop through each service and check for our IP
            for (int i = 0; i < IpServices.Length; i++)
            {
                try
                {
                    string result = await (new WebClient()).DownloadStringTaskAsync(IpServices[i]);
                    Match match = Regex.Match(result, @"(?<IpAddress>\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})");
                    if (match.Success && IPAddress.TryParse(match.Groups["IpAddress"].Value, out addy))
                    {
                        StatusText.Text = "Address Fetched Successfully!";
                        StatusPic.Image = BF2Statistics.Properties.Resources.check;
                        AddressTextBox.Text = addy.ToString();
                        break;
                    }
                }
                catch
                {
                    continue;
                }
            }

            // If we were unable to fetch the IP address, then alert the user
            if(addy == null)
            {
                StatusText.Text = "Unable to fetch external address!";
                StatusPic.Image = BF2Statistics.Properties.Resources.error;
            }
        }
    }
}
