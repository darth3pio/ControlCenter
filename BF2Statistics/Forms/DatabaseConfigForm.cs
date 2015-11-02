using System;
using System.Data.SQLite;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using BF2Statistics.Database;
using MySql.Data.MySqlClient;

namespace BF2Statistics
{
    /// <summary>
    /// Specifies what type of database we are working with
    /// </summary>
    public enum DatabaseMode
    {
        Stats, 
        Gamespy
    }

    public partial class DatabaseConfigForm : NativeForm
    {
        /// <summary>
        /// The database type we are working with (Stats or Gamespy)
        /// </summary>
        protected DatabaseMode DbMode;

        public DatabaseConfigForm(DatabaseMode Mode)
        {
            InitializeComponent();
            this.DbMode = Mode;

            // Our connection string temp variable
            string ConnString = (Mode == DatabaseMode.Stats) ? Program.Config.StatsDBConnectionString : Program.Config.GamespyDBConnectionString;
            string Engine = (Mode == DatabaseMode.Stats) ? Program.Config.StatsDBEngine : Program.Config.GamespyDBEngine;

            // Fill values for config boxes
            if (Engine == "Sqlite")
            {
                TypeSelect.SelectedIndex = 0;
                SQLiteConnectionStringBuilder Builder = new SQLiteConnectionStringBuilder(ConnString);
                if (!String.IsNullOrWhiteSpace(Builder.DataSource))
                    DBName.Text = Path.GetFileNameWithoutExtension(Builder.DataSource);
                else
                    DBName.Text = (Mode == DatabaseMode.Stats) ? "bf2stats" : "gamespy";
            }
            else
            {
                TypeSelect.SelectedIndex = 1;
                MySqlConnectionStringBuilder Builder = new MySqlConnectionStringBuilder(ConnString);
                Hostname.Text = Builder.Server;
                Port.Value = Builder.Port;
                Username.Text = Builder.UserID;
                Password.Text = Builder.Password;
                DBName.Text = Builder.Database;
            }

            // === Gamespy Texts
            if (Mode == DatabaseMode.Gamespy)
            {
                // Set header texts
                TitleLabel.Text = "Gamespy Database Configuration";
                DescLabel.Text = "Which database should gamespy accounts be saved to?";
            }
        }

        /// <summary>
        /// Sets the current config settings, but does not save the settings to the app.config file
        /// </summary>
        private void SetConfigSettings()
        {
            // Our connection string temp variable
            string ConnString;

            // Sqlite or Mysql
            if (TypeSelect.SelectedIndex == 0)
            {
                SQLiteConnectionStringBuilder Builder = new SQLiteConnectionStringBuilder();
                Builder.DataSource = Path.Combine(Program.RootPath, DBName.Text + ".sqlite3");
                Builder.Version = 3;
                Builder.PageSize = 4096; // Set page size to NTFS cluster size = 4096 bytes
                Builder.CacheSize = 10000;
                Builder.JournalMode = SQLiteJournalModeEnum.Wal;
                Builder.LegacyFormat = false;
                Builder.DefaultTimeout = 500;
                ConnString = Builder.ConnectionString;
            }
            else
            {
                MySqlConnectionStringBuilder Builder = new MySqlConnectionStringBuilder();
                Builder.Server = Hostname.Text;
                Builder.Port = (uint)Port.Value;
                Builder.UserID = Username.Text;
                Builder.Password = Password.Text;
                Builder.Database = DBName.Text;
                Builder.ConvertZeroDateTime = true;
                ConnString = Builder.ConnectionString;
            }

            // Set config vars based on the database mode
            if (DbMode == DatabaseMode.Stats)
            {
                // Make sure we arent sharing a database
                if (ConnString.Equals(Program.Config.GamespyDBConnectionString, StringComparison.InvariantCultureIgnoreCase))
                {
                    MessageBox.Show(
                        "The Stats database cannot be shared in the same database as the Gamespy database. Please use a different database name.",
                        "Ambiguity Error", MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );

                    return;
                }

                Program.Config.StatsDBEngine = (TypeSelect.SelectedIndex == 0) ? "Sqlite" : "Mysql";
                Program.Config.StatsDBConnectionString = ConnString;
            }
            else
            {
                // Make sure we arent sharing a database
                if (ConnString.Equals(Program.Config.StatsDBConnectionString, StringComparison.InvariantCultureIgnoreCase))
                {
                    MessageBox.Show(
                        "The Gamespy database cannot be shared in the same database as the Stats database. Please use a different database name.",
                        "Ambiguity Error", MessageBoxButtons.OK, MessageBoxIcon.Warning
                    );

                    return;
                }

                Program.Config.GamespyDBEngine = (TypeSelect.SelectedIndex == 0) ? "Sqlite" : "Mysql";
                Program.Config.GamespyDBConnectionString = ConnString;
            }
        }

        /// <summary>
        /// Hides or shows the password characters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowHideBtn_Click(object sender, EventArgs e)
        {
            Password.UseSystemPasswordChar = !Password.UseSystemPasswordChar;
        }

        /// <summary>
        /// Event fired when the cancel button is clicked
        /// </summary>
        private void CancelBtn_Click(object sender, EventArgs e)
        {
            Program.Config.Reload();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Event fired when the Test button is clicked
        /// </summary>
        private async void TestBtn_Click(object sender, EventArgs e)
        {
            // Build Connection String
            MySqlConnectionStringBuilder Builder = new MySqlConnectionStringBuilder();
            Builder.Server = Hostname.Text;
            Builder.Port = (uint)Port.Value;
            Builder.UserID = Username.Text;
            Builder.Password = Password.Text;
            Builder.Database = DBName.Text;

            // Attempt to connect, reporting any and all errors
            try
            {
                // Show loading form
                LoadingForm.ShowScreen(this, true, "Connecting to MySQL Database...");
                SetNativeEnabled(false);

                // Dont lock up the program
                await Task.Run(() =>
                {
                    using (MySqlConnection Connection = new MySqlConnection(Builder.ConnectionString))
                    {
                        Connection.Open();
                        Connection.Close();
                    }
                });
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message, "Connection Error", MessageBoxButtons.OK, 
                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1,
                    (MessageBoxOptions)0x40000 // Force window on top
                );
                return;
            }
            finally
            {
                LoadingForm.CloseForm();
                SetNativeEnabled(true);
            }

            // Show success after loading form has been called to close
            MessageBox.Show("Connection Successful!", "Success", MessageBoxButtons.OK,
                MessageBoxIcon.Information, MessageBoxDefaultButton.Button1,
                (MessageBoxOptions)0x40000 // Force window on top
            );
        }

        /// <summary>
        /// Event fired when the save button is clicked
        /// </summary>
        private void SaveBtn_Click(object sender, EventArgs e)
        {
            SetConfigSettings();
            Program.Config.Save();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Event fired when the database type selection has changed
        /// </summary>
        private void TypeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            Hostname.Enabled = Port.Enabled = Username.Enabled = Password.Enabled = TestBtn.Enabled = (TypeSelect.SelectedIndex == 1);
        }

        /// <summary>
        /// Event fired when the next button is clicked
        /// </summary>
        private async void NextBtn_Click(object sender, EventArgs e)
        {
            // Disable this form
            this.Enabled = false;
            string Message1 = "";

            // Initiate the Task Form
            if (TypeSelect.SelectedIndex == 1)
            {
                TaskForm.Show(this, "Create Database", "Connecting to MySQL Database...", false);
                Message1 = "Successfully Connected to MySQL Database! We will attempt to create the necessary tables into the specified database. Continue?";
            }
            else
            {
                TaskForm.Show(this, "Create Database", "Creating SQLite Database...", false);
                Message1 = "Successfully Created the SQLite Database! We will attempt to create the necessary tables into the specified database. Continue?";
            }

            // Temporarily set settings
            SetConfigSettings();

            // Try and install the SQL
            try
            {
                bool PreviousInstall = true;

                // Run in a seperate thread, dont wanna lock up the GUI thread
                await Task.Run(() =>
                {
                    // Switch mode
                    if (DbMode == DatabaseMode.Stats)
                    {
                        // Open up the database connetion
                        using (StatsDatabase Db = new StatsDatabase())
                        {
                            // We only have work to do if the tables are not installed
                            if (!Db.TablesExist)
                            {
                                PreviousInstall = false;

                                // Verify that the user wants to install DB tables
                                DialogResult Res = MessageBox.Show(Message1, "Verify Installation", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                    MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000 // Force window on top
                                );

                                // If we dont want to install tables, back out!
                                if (Res == DialogResult.No) return;

                                // Create our tables
                                TaskForm.Progress.Report(new TaskProgressUpdate("Creating Stats Tables"));
                                Db.CreateSqlTables(TaskForm.Progress);
                            }
                        }
                    }
                    else // Gamespy Mode
                    {
                        // Open up the database connetion
                        using (GamespyDatabase Db = new GamespyDatabase())
                        {
                            // We only have work to do if the tables are not installed
                            if (!Db.TablesExist)
                            {
                                PreviousInstall = false;

                                // Verify that the user wants to install DB tables
                                DialogResult Res = MessageBox.Show(Message1, "Verify Installation", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                    MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000 // Force window on top
                                );

                                // If we dont want to install tables, back out!
                                if (Res == DialogResult.No)  return;

                                // Create our tables
                                TaskForm.Progress.Report(new TaskProgressUpdate("Creating Gamespy Tables"));
                                Db.CreateSqlTables();
                            }
                        }
                    }
                });

                // No errors, so save the config file
                Program.Config.Save();

                // Close the task form
                TaskForm.CloseForm();

                // Show Success Form
                if (!PreviousInstall)
                    MessageBox.Show("Successfully installed the database tables!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000 // Force window on top
                    );
                else
                    MessageBox.Show(
                        "We've detected that the database was already installed here. Your database settings have been saved and no further setup is required.",
                        "Existing Installation Found", MessageBoxButtons.OK, MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000 // Force window on top
                        );

                // Close this form, as we are done now
                this.Close();
            }
            catch (Exception Ex)
            {
                // Close the task form and re-enable this form
                TaskForm.CloseForm();
                this.Enabled = true;

                // Revert the temporary config settings and show the error to the user
                Program.Config.Reload();
                MessageBox.Show(Ex.Message, "Database Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000 // Force window on top
                );
                return;
            }
            finally
            {
                if (TaskForm.IsOpen)
                    TaskForm.CloseForm();

                this.Enabled = true;
            }
        }
    }
}
