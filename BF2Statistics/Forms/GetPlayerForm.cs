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
    public partial class GetPlayerForm : Form
    {
        protected AutoCompleteStringCollection NamesCollection = new AutoCompleteStringCollection();

        public GetPlayerForm()
        {
            InitializeComponent();
            Input.AutoCompleteCustomSource = NamesCollection;
        }

        private void SubmitBtn_Click(object sender, EventArgs e)
        {
            if (Input.InvokeRequired)
            {
                this.Invoke(new Action<object, EventArgs>(SubmitBtn_Click), new object[] { sender, e });
            }
            else
            {
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

        private void Input_TextChanged(object sender, EventArgs e)
        {
            if (Input.Text.Length < 3)
                return;

            List<string> Names = LoginServer.Database.GetUsersLike(Input.Text);
            NamesCollection.Clear();
            foreach (string Name in Names)
                NamesCollection.Add(Name);
        }
    }
}
