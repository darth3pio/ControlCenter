using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using BF2Statistics.Properties;

namespace BF2Statistics
{
    class SetupManager
    {
        /// <summary>
        /// Entry point... this will check if we are at the initial setup
        /// phase, and show the installation forms
        /// </summary>
        /// <returns>Returns false if the user cancels the setup before the basic settings are setup, true otherwise</returns>
        public static bool Run()
        {
            // Load the program config
            Settings Config = Settings.Default;
            bool PromptDbSetup = false;

            // If this is the first time running a new update, we need to update the config file
            if (!Config.SettingsUpdated)
            {
                Config.Upgrade();
                Config.SettingsUpdated = true;
                Config.Save();
            }

            // If this is the first run, Get client and server install paths
            if (String.IsNullOrWhiteSpace(Config.ServerPath) || !File.Exists(Path.Combine(Config.ServerPath, "bf2_w32ded.exe")))
            {
                PromptDbSetup = true;
                if (!ShowInstallForm())
                    return false;
            }

            // Create the "My Documents/BF2Statistics" folder
            try
            {
                // Make sure documents folder exists
                if (!Directory.Exists(Paths.DocumentsFolder))
                    Directory.CreateDirectory(Paths.DocumentsFolder);

                // Backups folder
                if (!Directory.Exists(Path.Combine(Paths.DocumentsFolder, "Backups")))
                    Directory.CreateDirectory(Path.Combine(Paths.DocumentsFolder, "Backups"));
            }
            catch (Exception E)
            {
                string message = "Bf2Statistics encountered an error trying to create the required \"My Documents/BF2Statistics\" folder!";
                message += Environment.NewLine + Environment.NewLine + E.Message;
                MessageBox.Show(message, "Setup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Load server go.. If we fail to load a valid server, we will come back to here
            LoadServer:
            {
                // Load the BF2 Server
                try
                {
                    BF2Server.Load(Config.ServerPath);
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message, "Battlefield 2 Server Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Re-prompt
                    if (!ShowInstallForm())
                        return false;

                    goto LoadServer;
                }
            }

            // Fresh install? Show database config prompt
            if (PromptDbSetup)
            {
                string message = "In order to use the Private Stats feature of this program, we need to setup a database. "
                    + "You may choose to do this later by clicking \"Cancel\". Would you like to setup the database now?";
                DialogResult R = MessageBox.Show(message, "Stats Database Setup", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // Just return if the user doesnt want to set up the databases
                if (R == DialogResult.No)
                    return true;

                // Show Stats DB
                ShowDatabaseSetupForm(DatabaseMode.Stats);

                message = "In order to use the Gamespy Login Emulation feature of this program, we need to setup a database. "
                    + "You may choose to do this later by clicking \"Cancel\". Would you like to setup the database now?";
                R = MessageBox.Show(message, "Gamespy Database Setup", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // Just return if the user doesnt want to set up the databases
                if (R == DialogResult.No)
                    return true;

                ShowDatabaseSetupForm(DatabaseMode.Gamespy);
            }

            return true;
        }

        public static bool ShowInstallForm()
        {
            InstallForm IS = new InstallForm();
            return (IS.ShowDialog() == DialogResult.OK);
        }

        public static void ShowDatabaseSetupForm(DatabaseMode Mode)
        {
            DatabaseConfigForm F = new DatabaseConfigForm(Mode);
            F.ShowDialog();
        }
    }
}
