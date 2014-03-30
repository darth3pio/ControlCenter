using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security;
using Microsoft.Win32;

namespace BF2Statistics
{
    public partial class InstallForm : Form
    {
        /// <summary>
        /// The installation path for the bf2 client
        /// </summary>
        protected string ClientInstallPath = "";

        /// <summary>
        /// The installation path for the dedicated server
        /// </summary>
        protected string ServerInstallPath = "";

        public InstallForm()
        {
            InitializeComponent();

            // Check for BF2 Client Installation (32 bit)
            try
            {
                ClientInstallPath = Registry.GetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\Electronic Arts\EA Games\Battlefield 2", 
                    "InstallDir", 
                    String.Empty).ToString();
                ClientPath.Text = ClientInstallPath;
            }
            catch(IOException) // Entry Doesnt Exist, Try 64 Bit Installation
            {
                try
                {
                    ClientInstallPath = Registry.GetValue(
                        @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Electronic Arts\EA Games\Battlefield 2", 
                        "InstallDir", 
                        String.Empty).ToString();
                    ClientPath.Text = ClientInstallPath;
                }
                catch { }
            }
            catch { }

            // Check for BF2 Server Installation (32 bit)
            try
            {
                ServerInstallPath = Registry.GetValue(
                    @"HKEY_LOCAL_MACHINE\SOFTWARE\EA Games\Battlefield 2 Server", 
                    "GAMEDIR", 
                    String.Empty).ToString();
                ServerPath.Text = ServerInstallPath;
            }
            catch (IOException) // Entry Doesnt Exist, Try 64 Bit Installation
            {
                try
                {
                    ServerInstallPath = Registry.GetValue(
                        @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\EA Games\Battlefield 2 Server", 
                        "GAMEDIR", 
                        String.Empty).ToString();
                    ServerPath.Text = ServerInstallPath;
                }
                catch { }
            }
            catch { }
        }

        /// <summary>
        /// Event fired when the Client Path button is clicked... Opens the Dialog to select
        /// the Client executable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.FileName = "bf2.exe";
            Dialog.Filter = "BF2 Executable|bf2.exe";

            // Set the initial search directory if we found an install path via registry
            if (!String.IsNullOrWhiteSpace(ClientInstallPath))
                Dialog.InitialDirectory = ClientInstallPath;

            if (Dialog.ShowDialog() == DialogResult.OK)
                ClientPath.Text = Path.GetDirectoryName(Dialog.FileName);
        }

        /// <summary>
        /// Event fired when the Server Path button is clicked... Opens the Dialog to select
        /// the Server executable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.FileName = "bf2_w32ded.exe";
            Dialog.Filter = "BF2 Server Executable|bf2_w32ded.exe";

            // Set the initial search directory if we found an install path via registry
            if (!String.IsNullOrWhiteSpace(ServerInstallPath))
                Dialog.InitialDirectory = ServerInstallPath;

            if (Dialog.ShowDialog() == DialogResult.OK)
                ServerPath.Text = Path.GetDirectoryName(Dialog.FileName);
        }

        /// <summary>
        /// Event fired when the Save button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveBtn_Click(object sender, EventArgs e)
        {
            // Make sure the server path is not empty
            if (String.IsNullOrWhiteSpace(ServerPath.Text))
            {
                MessageBox.Show("You must set the server path before proceeding.");
                return;
            }

            // Save config
            MainForm.Config.ClientPath = ClientPath.Text;
            MainForm.Config.ServerPath = ServerPath.Text;
            MainForm.Config.Save();

            // Tell the main form we are OK
            this.DialogResult = DialogResult.OK;
        }
    }
}
