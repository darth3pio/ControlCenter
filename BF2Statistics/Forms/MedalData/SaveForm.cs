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
    public partial class SaveForm : Form
    {
        public static bool IncludeSFData = false;

        public SaveForm()
        {
            InitializeComponent();
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            IncludeSFData = YesRadio.Checked;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
