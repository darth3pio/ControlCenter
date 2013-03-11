using System;
using System.Data.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.Common;
using MySql.Data.MySqlClient;

namespace BF2Statistics
{
    public partial class GamespyConfig : Form
    {
        public GamespyConfig()
        {
            InitializeComponent();

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
            Debug.Checked = MainForm.Config.DebugStream;
        }

        private void TypeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            Hostname.Enabled = Port.Enabled = Username.Enabled = Password.Enabled = TestBtn.Enabled = (TypeSelect.SelectedIndex == 1);
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
            catch(Exception E)
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
            MainForm.Config.GamespyDBEngine = (TypeSelect.SelectedIndex == 0) ? "Sqlite" : "Mysql";
            MainForm.Config.GamespyDBHost = Hostname.Text;
            MainForm.Config.GamespyDBPort = (int)Port.Value;
            MainForm.Config.GamespyDBUser = Username.Text;
            MainForm.Config.GamespyDBPass = Password.Text;
            MainForm.Config.GamespyDBName = DBName.Text;
            MainForm.Config.DebugStream = Debug.Checked;
            MainForm.Config.Save();
            this.Close();
        }
    }
}
