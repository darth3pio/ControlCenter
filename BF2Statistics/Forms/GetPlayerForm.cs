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
    public partial class GetPlayerForm : Form
    {
        protected AutoCompleteStringCollection NamesCollection = new AutoCompleteStringCollection();

        public GetPlayerForm()
        {
            InitializeComponent();
            Input.AutoCompleteCustomSource = NamesCollection;
        }

        /// <summary>
        /// Event fired when the form is submitted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitBtn_Click(object sender, EventArgs e)
        {
            if (Input.InvokeRequired)
            {
                this.Invoke(new Action<object, EventArgs>(SubmitBtn_Click), new object[] { sender, e });
            }
            else
            {
                // We still need to make sure the user exists!
                if (LoginServer.Database.UserExists(Input.Text))
                {
                    EditAcctForm.AccountName = Input.Text;
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("Account \"" + Input.Text + "\" does not exist!");
                    return;
                }
            }
        }

        /// <summary>
        /// Whenever the text changes in the input box, we display a suggestion box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Input_TextChanged(object sender, EventArgs e)
        {
            // Ignore if we have less then 3 characters
            if (Input.Text.Length < 3)
                return;

            // Get our "Like" names from the database
            List<string> Names = LoginServer.Database.GetUsersLike(Input.Text);
            NamesCollection.Clear();
            foreach (string Name in Names)
                NamesCollection.Add(Name);
        }

        /// <summary>
        /// Here we allow the enter key press to submit the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SubmitBtn_Click(this, new EventArgs());
        }
    }
}
