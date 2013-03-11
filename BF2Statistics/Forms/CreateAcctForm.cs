using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;
using BF2Statistics.Gamespy;

namespace BF2Statistics
{
    public partial class CreateAcctForm : Form
    {
        public CreateAcctForm()
        {
            InitializeComponent();
            PidSelect.SelectedIndex = 0;
        }

        private void PidSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            PidBox.Enabled = (PidSelect.SelectedIndex != 0);
        }

        private void CreateBtn_Click(object sender, EventArgs e)
        {
            int Pid = (int)PidBox.Value;

            // Make sure there is no empty fields!
            if (AccountName.Text.Trim().Length < 3)
            {
                MessageBox.Show("Please enter a valid account name", "Error");
                return;
            }
            else if (AccountPass.Text.Trim().Length < 3)
            {
                MessageBox.Show("Please enter a valid account password", "Error");
                return;
            }
            else if (!Validator.IsValidEmail(AccountEmail.Text))
            {
                MessageBox.Show("Please enter a valid account email", "Error");
                return;
            }

            // Check if PID exists (for changing PID)
            if (PidSelect.SelectedIndex == 1)
            {
                if (!Validator.IsValidPID(Pid.ToString()))
                {
                    MessageBox.Show("Invalid PID Format. A PID must be 8 or 9 digits in length", "Error");
                    return;
                }
                else if(LoginServer.Database.UserExists(Pid))
                {
                    MessageBox.Show("PID is already in use. Please enter a different PID.", "Error");
                    return;
                }
            }

            // Check if the user exists
            if (LoginServer.Database.UserExists(AccountName.Text))
            {
                MessageBox.Show("Account name is already in use. Please select a different Account Name.", "Error");
                return;
            }

            bool Success;
            try
            {
                Success = LoginServer.Database.CreateUser(AccountName.Text, AccountPass.Text, AccountEmail.Text, "00");
                if (PidSelect.SelectedIndex == 1)
                    LoginServer.Database.SetPID(AccountName.Text, Pid);
            }
            catch(Exception E)
            {
                MessageBox.Show(E.Message, "Account Create Error");
            }

            this.Close();
        }
    }
}
