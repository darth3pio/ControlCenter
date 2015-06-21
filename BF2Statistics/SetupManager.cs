using System;
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

                // Create the database backups folder
                string bFolder = Path.Combine(Paths.DocumentsFolder, "Database Backups");
                if (!Directory.Exists(bFolder))
                {
                    // In 1.x.x versions, this folder was called Backups rather then Database Backups
                    string OldB = Path.Combine(Paths.DocumentsFolder, "Backups");
                    if(Directory.Exists(OldB))
                        Directory.Move(OldB, bFolder);
                    else
                        Directory.CreateDirectory(bFolder);
                }
            }
            catch (Exception E)
            {
                // Alert the user that there was an error
                MessageBox.Show("Bf2Statistics encountered an error trying to create the required \"My Documents/BF2Statistics\" folder!"
                    + Environment.NewLine.Repeat(1) + E.Message,
                    "Setup Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error
                );
                return false;
            }

            // Load server go.. If we fail to load a valid server, we will come back to here
            LoadServer:
            {
                // Load the BF2 Server
                try
                {
                    BF2Server.SetServerPath(Config.ServerPath);
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
                ShowDatabaseSetupForm(DatabaseMode.Stats, null);

                message = "In order to use the Gamespy Login Emulation feature of this program, we need to setup a database. "
                    + "You may choose to do this later by clicking \"Cancel\". Would you like to setup the database now?";
                R = MessageBox.Show(message, "Gamespy Database Setup", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // Just return if the user doesnt want to set up the databases
                if (R == DialogResult.No)
                    return true;

                ShowDatabaseSetupForm(DatabaseMode.Gamespy, null);
            }

            return true;
        }

        /// <summary>
        /// Displays the Install / Program configuration form
        /// </summary>
        /// <returns>Returns whether the config was saved, false otherwise</returns>
        public static bool ShowInstallForm()
        {
            using (InstallForm IS = new InstallForm())
            {
                return (IS.ShowDialog() == DialogResult.OK);
            }
        }

        /// <summary>
        /// Displays the Database configuration form for the selected database type (Stats or Gamespy)
        /// </summary>
        /// <param name="Mode">The database type</param>
        /// <param name="Parent">The parent window to Dialog over</param>
        public static void ShowDatabaseSetupForm(DatabaseMode Mode, Form Parent = null)
        {
            // Try and get the active form
            if (Parent == null) Parent = Form.ActiveForm;

            using (DatabaseConfigForm F = new DatabaseConfigForm(Mode))
            {
                if (Parent != null && Parent.IsHandleCreated && !Parent.InvokeRequired)
                    F.ShowDialog(Parent);
                else
                    F.ShowDialog();
            }
        }
    }
}
