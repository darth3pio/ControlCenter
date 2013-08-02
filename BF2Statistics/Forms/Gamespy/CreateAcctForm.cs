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
                MessageBox.Show("Please enter a valid account name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (AccountPass.Text.Trim().Length < 3)
            {
                MessageBox.Show("Please enter a valid account password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (!Validator.IsValidEmail(AccountEmail.Text))
            {
                MessageBox.Show("Please enter a valid account email", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if PID exists (for changing PID)
            if (PidSelect.SelectedIndex == 1)
            {
                if (!Validator.IsValidPID(Pid.ToString()))
                {
                    MessageBox.Show("Invalid PID Format. A PID must be 8 or 9 digits in length", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if(LoginServer.Database.UserExists(Pid))
                {
                    MessageBox.Show("PID is already in use. Please enter a different PID.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Check if the user exists
            if (LoginServer.Database.UserExists(AccountName.Text))
            {
                MessageBox.Show("Account name is already in use. Please select a different Account Name.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Attempt to create the account
                if (PidSelect.SelectedIndex == 1)
                    LoginServer.Database.CreateUser(Pid, AccountName.Text, AccountPass.Text, AccountEmail.Text, "00");
                else
                    LoginServer.Database.CreateUser(AccountName.Text, AccountPass.Text, AccountEmail.Text, "00");

                Notify.Show("Account Created Successfully!", AccountName.Text, AlertType.Success);
            }
            catch(Exception E)
            {
                MessageBox.Show(E.Message, "Account Create Error");
            }

            this.Close();
        }
    }
}
