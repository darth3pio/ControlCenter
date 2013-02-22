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
        protected string bf2InstallPath = "";
        protected string serverInstallPath = "";

        public InstallForm()
        {
            InitializeComponent();

            // Remove gray text from intro text box
            IntroTextBox.SelectAll();
            IntroTextBox.SelectionColor = Color.Black;

            // Check for BF2 Installation (32 bit)
            try
            {
                bf2InstallPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Electronic Arts\EA Games\Battlefield 2", "InstallDir", "").ToString();
                ClientPath.Text = bf2InstallPath;
            }
            catch(IOException) // Doesnt Exist
            {
                try
                {
                    bf2InstallPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Electronic Arts\EA Games\Battlefield 2", "InstallDir", "").ToString();
                    ClientPath.Text = bf2InstallPath;
                }
                catch (IOException)
                {
                    // Doesnt Exist. Do nothing for now i suppose...
                }
                catch (SecurityException)
                {
                    // We dont have registry permissions :(
                }
            }
            catch (SecurityException)
            {
                // We dont have registry permissions :(
            }

            // Check for BF2 Server Installation (32 bit)
            try
            {
                serverInstallPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\EA Games\Battlefield 2 Server", "GAMEDIR", "").ToString();
                ServerPath.Text = serverInstallPath;
            }
            catch (IOException) // Doesnt Exist
            {
                try
                {
                    serverInstallPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\EA Games\Battlefield 2 Server", "GAMEDIR", "").ToString();
                    ServerPath.Text = serverInstallPath;
                }
                catch (IOException)
                {
                    // Doesnt Exist. Do nothing for now i suppose...
                }
                catch (SecurityException)
                {
                    // We dont have registry permissions :(
                }
            }
            catch (SecurityException)
            {
                // We dont have registry permissions :(
            }
        }

        private void ClientBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.FileName = "bf2.exe";
            Dialog.Filter = "BF2 Executable|bf2.exe";
            if (!String.IsNullOrWhiteSpace(bf2InstallPath))
                Dialog.InitialDirectory = bf2InstallPath;

            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                ClientPath.Text = Path.GetDirectoryName(Dialog.FileName);
            }
        }

        private void ServerBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.FileName = "bf2_w32ded.exe";
            Dialog.Filter = "BF2 Server Executable|bf2_w32ded.exe";
            if (!String.IsNullOrWhiteSpace(serverInstallPath))
                Dialog.InitialDirectory = serverInstallPath;

            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                ServerPath.Text = Path.GetDirectoryName(Dialog.FileName);
            }
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(ClientPath.Text) || String.IsNullOrWhiteSpace(ServerPath.Text))
            {
                MessageBox.Show("You must set client and server paths before continuing.");
                return;
            }

            // Save config
            MainForm.Config.ClientPath = ClientPath.Text;
            MainForm.Config.ServerPath = ServerPath.Text;
            MainForm.Config.Save();

            this.DialogResult = DialogResult.OK;
        }
    }
}
