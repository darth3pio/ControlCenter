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
    public partial class BotCountInfoForm : Form
    {
        public BotCountInfoForm()
        {
            InitializeComponent();
            richTextBox1.SelectAll();
            richTextBox1.SelectionColor = Color.Black;
        }
    }
}
