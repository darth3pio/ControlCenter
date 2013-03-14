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

namespace BF2Statistics
{
    public partial class StatsDbConfigForm : Form
    {
        public StatsDbConfigForm()
        {
            InitializeComponent();

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
            MainForm.Config.StatsDBEngine = (TypeSelect.SelectedIndex == 0) ? "Sqlite" : "Mysql";
            MainForm.Config.StatsDBHost = Hostname.Text;
            MainForm.Config.StatsDBPort = (int)Port.Value;
            MainForm.Config.StatsDBUser = Username.Text;
            MainForm.Config.StatsDBPass = Password.Text;
            MainForm.Config.StatsDBName = DBName.Text;
            MainForm.Config.Save();
            this.Close();
        }

        private void TypeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            Hostname.Enabled = Port.Enabled = Username.Enabled = Password.Enabled = TestBtn.Enabled = (TypeSelect.SelectedIndex == 1);
        }
    }
}
