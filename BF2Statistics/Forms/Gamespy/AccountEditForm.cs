using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BF2Statistics.Gamespy;

namespace BF2Statistics
{
    public partial class AccountEditForm : Form
    {
        /// <summary>
        /// The Account ID
        /// </summary>
        protected int AccountId;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Pid">The player account ID</param>
        public AccountEditForm(int Pid)
        {
            InitializeComponent();
            this.AccountId = Pid;

            // Register for Events
            GpcmClient.OnSuccessfulLogin += new ConnectionUpdate(GpcmClient_OnSuccessfulLogin);
            GpcmClient.OnDisconnect += new ConnectionUpdate(GpcmClient_OnDisconnect);
            
            // Fill the account information boxes
            Dictionary<string, object> User = LoginServer.Database.GetUser(AccountId);
            PlayerID.Value = AccountId = Int32.Parse(User["id"].ToString());
            AccountNick.Text = User["name"].ToString();
            AccountPass.Text = User["password"].ToString();
            AccountEmail.Text = User["email"].ToString();

            // Disable options if user is online
            if (User["session"].ToString() != "0")
            {
                SatusLabel.Text = "Online (IP: " + User["lastip"].ToString() + ")";
                UpdateBtn.Enabled = false;
                DeleteBtn.Enabled = false;
                DisconnectBtn.Enabled = true;
            }
            else
            {
                SatusLabel.Text = "Offline";
            }
        }

        /// <summary>
        /// We cant modify connected client accounts. When the account goes offline,
        /// we re-enable the update button and status
        /// </summary>
        /// <param name="sender"></param>
        private void GpcmClient_OnDisconnect(object sender)
        {
            GpcmClient Client = (GpcmClient)sender;
            if (Client.ClientPID == AccountId)
            {
                // Since we are in a different thread, Invoke
                Invoke(new Action( () =>
                {
                    SatusLabel.Text = "Offline";
                    UpdateBtn.Enabled = true;
                    DeleteBtn.Enabled = true;
                    DisconnectBtn.Enabled = false;
                }));
            }
        }

        /// <summary>
        /// We cant modify connected client accounts. When the account logs in,
        /// we disable the update buttonn and update the status
        /// </summary>
        /// <param name="sender"></param>
        private void GpcmClient_OnSuccessfulLogin(object sender)
        {
            GpcmClient Client = (GpcmClient)sender;
            if (Client.ClientPID == AccountId)
            {
                // Since we are in a different thread, Invoke
                Invoke(new Action( () =>
                {
                    SatusLabel.Text = "Online (IP: " + Client.IpAddress.ToString() + ")";
                    UpdateBtn.Enabled = false;
                    DeleteBtn.Enabled = false;
                    DisconnectBtn.Enabled = true;
                }));
            }
        }

        /// <summary>
        /// Event fired when the Submit button is pushed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateBtn_Click(object sender, EventArgs e)
        {
            int Pid = (int)PlayerID.Value;

            // Make sure there is no empty fields!
            if (AccountNick.Text.Trim().Length < 3)
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
            else if(Pid != AccountId)
            {
                if (!Validator.IsValidPID(Pid.ToString()))
                {
                    MessageBox.Show("Invalid PID Format. A PID must be 8 or 9 digits in length", "Error");
                    return;
                }
                // Make sure the PID doesnt exist!
                else if (LoginServer.Database.UserExists(Pid))
                {
                    MessageBox.Show("Battlefield 2 PID is already taken. Please try a different PID.", "Error");
                    return;
                }
            }

            LoginServer.Database.UpdateUser(AccountId, Pid, AccountNick.Text, AccountPass.Text, AccountEmail.Text);
            this.Close();
        }

        /// <summary>
        /// Event fired when the Delete button is pushed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete account?", "Confirm", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (LoginServer.Database.DeleteUser(AccountId) == 1)
                    Notify.Show("Account deleted successfully!", "Operation Successful", AlertType.Success);
                else
                    Notify.Show("Failed to remove account from database!", "Operation failed", AlertType.Warning);
                this.Close();
            }
        }

        /// <summary>
        /// Event fire when disconnect button is pushed. Forces the account
        /// to log out.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisconnectBtn_Click(object sender, EventArgs e)
        {
            LoginServer.ForceLogout(AccountId);
        }

        /// <summary>
        /// When the form starts to close, we need to unregister for the OnDisconnect
        /// event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AccountEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            GpcmClient.OnSuccessfulLogin -= new ConnectionUpdate(GpcmClient_OnSuccessfulLogin);
            GpcmClient.OnDisconnect -= new ConnectionUpdate(GpcmClient_OnDisconnect);
        }
    }
}
