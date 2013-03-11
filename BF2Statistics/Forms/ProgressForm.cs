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
    public partial class ProgressForm : Form
    {
        private const int WS_SYSMENU = 0x80000;

        private static ProgressForm Instance;

        public HorizontalAlignment TextAlign = HorizontalAlignment.Left;

        public ProgressForm()
        {
            InitializeComponent();
            Instance = this;
        }

        public ProgressForm(string WindowTitle)
        {
            InitializeComponent();
            Instance = this;
            this.Text = WindowTitle;
        }

        public void Update(int Percent, string Text)
        {
            // Update Progress Text
            Instance.StatusText.Text = Text;

            // Set the status text color to black instead of grey
            Instance.StatusText.SelectAll();
            Instance.StatusText.SelectionColor = Color.Black;

            // Align Text, and remove selection
            Instance.StatusText.SelectionAlignment = TextAlign;
            Instance.StatusText.SelectionLength = 0;

            // Push Update
            Instance.StatusText.Update();

            // Update progress bar precentage
            Instance.ProgBar.Value = Percent;
            Instance.ProgBar.Focus();
        }

        public void Update(int Percent)
        {
            // Update progress bar precentage
            Instance.ProgBar.Value = Percent;
            Instance.ProgBar.Focus();
        }

        public void SetTitle(string WindowTitle)
        {
            this.Text = WindowTitle;
        }

        /// <summary>
        /// Hides the Close, Minimize, and Maximize buttons
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~WS_SYSMENU;
                return cp;
            }
        }
    }
}
