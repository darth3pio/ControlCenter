using System;
using System.IO;
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
            // Trim input
            string Name = ProfileName.Text.Trim();

            // Make sure we dont have an empty string!
            if (String.IsNullOrEmpty(Name))
            {
                MessageBox.Show("Please enter a profile name.", "Error");
                return;
            }

            // Make sure the named profile doesnt exist!
            if (MedalDataEditor.Profiles.Contains(Name.ToLower()))
            {
                MessageBox.Show("This profile name already exists. Please try a different profile name.", "Error");
                return;
            }

            // Write default medal data
            try
            {
                File.WriteAllText(
                    Path.Combine(MedalDataEditor.PythonPath, "medal_data_" + Name + ".py"),
                    String.Concat(
                        Utils.GetResourceAsString("BF2Statistics.MedalData.PyFiles.functions.py"),
                        Utils.GetResourceAsString("BF2Statistics.MedalData.PyFiles.medal_data_xpack.py")
                    )
                );
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
            LastProfileName = Name;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
