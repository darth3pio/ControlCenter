using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BF2Statistics.MedalData
{
    public partial class NewProfilePrompt : Form
    {
        public static string LastProfileName = null;

        public NewProfilePrompt()
        {
            InitializeComponent();
        }

        private void CreateBtn_Click(object sender, EventArgs e)
        {
            // Make sure the named profile doesnt exist!
            if (MedalDataEditor.Profiles.Contains(ProfileName.Text))
            {
                MessageBox.Show("This profile name already exists. Please try a different profile name.", "Error");
                return;
            }

            if (String.IsNullOrWhiteSpace(ProfileName.Text))
            {
                MessageBox.Show("Please enter a profile name.", "Error");
                return;
            }

            // Define paths
            string file = Path.Combine(MedalDataEditor.PythonPath, "medal_data_" + ProfileName.Text + ".py");
            string sfFile = Path.Combine(MedalDataEditor.PythonPath, "medal_data_" + ProfileName.Text + "_xpack.py");
            string Functions = Utils.GetResourceString("BF2Statistics.MedalData.PyFiles.functions.py");

            // Write default medal data
            try
            {
                File.WriteAllText(file, Functions + Utils.GetResourceString("BF2Statistics.MedalData.PyFiles.medal_data.py"));
                File.WriteAllText(sfFile, Functions + Utils.GetResourceString("BF2Statistics.MedalData.PyFiles.medal_data_xpack.py"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An exception was thrown while trying to create a medal data file!"
                        + Environment.NewLine + Environment.NewLine
                        + "Message: " + ex.Message,
                    "Error"
                );
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }

            // and close!
            LastProfileName = ProfileName.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
