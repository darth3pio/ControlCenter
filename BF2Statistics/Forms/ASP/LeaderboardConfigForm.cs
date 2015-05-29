using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace BF2Statistics
{
    public partial class LeaderboardConfigForm : Form
    {
        public LeaderboardConfigForm()
        {
            InitializeComponent();

            EnableChkBox.Checked = Program.Config.BF2S_Enabled;
            TitleTextBox.Text = Program.Config.BF2S_Title;
            PlayerCount.Value = Program.Config.BF2S_LeaderCount;
            CacheChkBox.Checked = Program.Config.BF2S_CacheEnabled;
        }

        private void EnableChkBox_CheckedChanged(object sender, EventArgs e)
        {
            TitleTextBox.Enabled = PlayerCount.Enabled = CacheChkBox.Enabled = EnableChkBox.Checked;
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            // Cant have an empty title... or there will be errors
            if (String.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Leaderboard title must be at least 1 character!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Save the config
            Program.Config.BF2S_Enabled = EnableChkBox.Checked;
            Program.Config.BF2S_Title = TitleTextBox.Text;
            Program.Config.BF2S_LeaderCount = (int) PlayerCount.Value;
            Program.Config.BF2S_CacheEnabled = CacheChkBox.Checked;
            Program.Config.Save();
            this.Close();
        }

        private void ClearBtn_Click(object sender, EventArgs e)
        {
            int Counter = 0;

            string[] fileNames = Directory.GetFiles(Path.Combine(Program.RootPath, "Web", "Bf2Stats", "Cache"));
            foreach (string fileName in fileNames)
            {
                try
                {
                    File.Delete(fileName);
                    Counter++;
                }
                catch { }
            }

            MessageBox.Show(
                String.Format("Successfully cleared {0} of {1} cached files.", Counter, fileNames.Length),
                "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information
            );
        }
    }
}
