using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;
using MySql.Data;
using MySql.Data.Common;
using MySql.Data.MySqlClient;
using BF2Statistics.Database;

namespace BF2Statistics
{
    public enum DatabaseMode
    {
        Stats, 
        Gamespy
    }

    public partial class DatabaseConfigForm : Form
    {
        protected DatabaseMode DbMode;

        public DatabaseConfigForm(DatabaseMode Mode)
        {
            InitializeComponent();
            this.DbMode = Mode;

            if (Mode == DatabaseMode.Stats)
            {
                // Fill values for config boxes
                if (MainForm.Config.StatsDBEngine == "Sqlite")
                    TypeSelect.SelectedIndex = 0;
                else
                    TypeSelect.SelectedIndex = 1;

                Hostname.Text = MainForm.Config.StatsDBHost;
                Port.Value = MainForm.Config.StatsDBPort;
                Username.Text = MainForm.Config.StatsDBUser;
                Password.Text = MainForm.Config.StatsDBPass;
                DBName.Text = MainForm.Config.StatsDBName;
            }
            else
            {
                // Fill values for config boxes
                if (MainForm.Config.GamespyDBEngine == "Sqlite")
                    TypeSelect.SelectedIndex = 0;
                else
                    TypeSelect.SelectedIndex = 1;

                Hostname.Text = MainForm.Config.GamespyDBHost;
                Port.Value = MainForm.Config.GamespyDBPort;
                Username.Text = MainForm.Config.GamespyDBUser;
                Password.Text = MainForm.Config.GamespyDBPass;
                DBName.Text = MainForm.Config.GamespyDBName;

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
            if (DbMode == DatabaseMode.Stats)
            {
                MainForm.Config.StatsDBEngine = (TypeSelect.SelectedIndex == 0) ? "Sqlite" : "Mysql";
                MainForm.Config.StatsDBHost = Hostname.Text;
                MainForm.Config.StatsDBPort = (int)Port.Value;
                MainForm.Config.StatsDBUser = Username.Text;
                MainForm.Config.StatsDBPass = Password.Text;
                MainForm.Config.StatsDBName = DBName.Text;

            }
            else
            {
                MainForm.Config.GamespyDBEngine = (TypeSelect.SelectedIndex == 0) ? "Sqlite" : "Mysql";
                MainForm.Config.GamespyDBHost = Hostname.Text;
                MainForm.Config.GamespyDBPort = (int)Port.Value;
                MainForm.Config.GamespyDBUser = Username.Text;
                MainForm.Config.GamespyDBPass = Password.Text;
                MainForm.Config.GamespyDBName = DBName.Text;
                MainForm.Config.DebugStream = false;
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

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            MainForm.Config.Reload();
            this.Close();
        }

        private void TestBtn_Click(object sender, EventArgs e)
        {
            // Disable console
            this.Enabled = false;

            // Build Connection String
            DbConnectionStringBuilder Builder = new MySqlConnectionStringBuilder();
            Builder.Add("Server", Hostname.Text);
            Builder.Add("Port", (int)Port.Value);
            Builder.Add("User ID", Username.Text);
            Builder.Add("Password", Password.Text);
            Builder.Add("Database", DBName.Text);

            // Attempt to connect, reporting any and all errors
            try
            {
                MySqlConnection Connection = new MySqlConnection(Builder.ConnectionString);
                Connection.Open();
                Connection.Close();
            }
            catch (Exception E)
            {
                this.Enabled = true;
                MessageBox.Show(E.Message, "Connection Error");
                return;
            }

            MessageBox.Show("Connection Successful!", "Success");
            this.Enabled = true;
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            SetConfigSettings();
            MainForm.Config.Save();
            this.Close();
        }

        private void TypeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            Hostname.Enabled = Port.Enabled = Username.Enabled = Password.Enabled = TestBtn.Enabled = (TypeSelect.SelectedIndex == 1);
        }

        private void NextBtn_Click(object sender, EventArgs e)
        {
            // Disable this form
            this.Enabled = false;
            string Message1 = "";

            // Temporarily set settings
            SetConfigSettings();

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

            // Try and install the SQL
            try
            {
                bool PreviousInstall = true;

                if (DbMode == DatabaseMode.Stats)
                {
                    using (StatsDatabase Db = new StatsDatabase())
                    {
                        if (!Db.IsInstalled)
                        {
                            PreviousInstall = false;

                            // Verify that the user wants to install DB tables
                            TaskForm.CloseForm();
                            DialogResult Res = MessageBox.Show(Message1, "Verify Installation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            // If we dont want to install tables, back out!
                            if (Res == DialogResult.No)
                                return;

                            TaskForm.Show(this, "Create Database", "Creating Stats Tables", false);
                            Db.CreateSqlTables();
                        }
                    }
                }
                else
                {
                    using (GamespyDatabase Db = new GamespyDatabase())
                    {
                        if (!Db.IsInstalled)
                        {
                            PreviousInstall = false;

                            // Verify that the user wants to install DB tables
                            TaskForm.CloseForm();
                            DialogResult Res = MessageBox.Show(Message1, "Verify Installation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            // If we dont want to install tables, back out!
                            if (Res == DialogResult.No)
                                return;

                            TaskForm.Show(this, "Create Database", "Creating Gamespy Tables", false);
                            Db.CreateSqlTables();
                        }
                    }
                }

                // No errors, so save the config file
                MainForm.Config.Save();

                // Close the task form
                TaskForm.CloseForm();

                // Show Success Form
                if (!PreviousInstall)
                    MessageBox.Show("Successfully installed the database tables!", "Success", MessageBoxButtons.OK);
                else
                    MessageBox.Show(
                        "We've detected that the database was already installed here. Your database settings have been saved and no further setup is required.",
                        "Existing Installation Found", MessageBoxButtons.OK, MessageBoxIcon.Information
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
                MainForm.Config.Reload();
                MessageBox.Show(Ex.Message, "Database Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
