using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BF2Statistics
{
    public partial class EditAcctForm : Form
    {
        public static string AccountName;

        protected int PID;

        public EditAcctForm()
        {
            InitializeComponent();
            GetPlayerForm Form = new GetPlayerForm();
            DialogResult Result = Form.ShowDialog();
            if (Result == DialogResult.OK)
                LoadUser();
            else
                this.Load += new EventHandler(MyForm_CloseOnStart);
        }

        /// <summary>
        /// This method fills the account information boxes
        /// </summary>
        private void LoadUser()
        {
            Dictionary<string, object> User = LoginServer.Database.GetUser(AccountName);
            PlayerID.Value = PID = Int32.Parse(User["id"].ToString());
            AccountNick.Text = User["name"].ToString();
            AccountPass.Text = User["password"].ToString();
            AccountEmail.Text = User["email"].ToString();
        }

        /// <summary>
        /// Event closes the form when fired
        /// </summary>
        private void MyForm_CloseOnStart(object sender, EventArgs e)
        {
            this.Close();
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
            else if(Pid != PID)
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

            LoginServer.Database.UpdateUser(PID, Pid, AccountNick.Text, AccountPass.Text, AccountEmail.Text);
            this.Close();
        }

        /// <summary>
        /// Event fired when the Delete button is pushed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete account?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string output = "";
                if (LoginServer.Database.DeleteUser(AccountName) == 1)
                    output = "Account deleted successfully";
                else
                    output = "Failed to remove account from database.";

                MessageBox.Show(output);
                this.Close();
            }
        }
    }
}
