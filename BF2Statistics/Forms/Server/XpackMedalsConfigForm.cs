using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using BF2Statistics.Utilities;

namespace BF2Statistics
{
    public partial class XpackMedalsConfigForm : Form
    {
        public XpackMedalsConfigForm()
        {
            InitializeComponent();
            int controlsAdded = 0;

            // Add each valid mod found in the servers Mods directory
            for(int i = 0; i < BF2Server.Mods.Count; i++)
            {
                // Get our current working mod
                BF2Mod Mod = BF2Server.Mods[i];

                // Bf2 is predefined
                if (Mod.Name.Equals("bf2", StringComparison.InvariantCultureIgnoreCase))
                {
                    checkBox1.Tag = Mod.Name;
                    checkBox1.Checked = StatsPython.Config.XpackMedalMods.Contains(Mod.Name.ToLowerInvariant());
                    continue;
                }

                // Xpack is predefined
                if (Mod.Name.Equals("xpack", StringComparison.InvariantCultureIgnoreCase))
                {
                    checkBox2.Tag = Mod.Name;
                    checkBox2.Checked = StatsPython.Config.XpackMedalMods.Contains(Mod.Name.ToLowerInvariant());
                    continue;
                }

                // Stop if over 10 added mods. I chose to use continue here instead of break
                // So that Xpack can get ticked if it may be at the bottom of the list
                if (controlsAdded >= 10) continue;

                try
                {
                    // Fetch our checkbox
                    int index = controlsAdded + 3;
                    CheckBox ChkBox = panel2.Controls["checkBox" + index] as CheckBox;

                    // Configure the Checkbox
                    string title = (Mod.Title.Length > 32) ? Mod.Title.CutTolength(29) + "..." : Mod.Title;
                    ChkBox.Text = String.Format("{0} [{1}]", title, Mod.Name);
                    ChkBox.Checked = StatsPython.Config.XpackMedalMods.Contains(Mod.Name.ToLowerInvariant());
                    ChkBox.Tag = Mod.Name;
                    ChkBox.Show();

                    // Add tooltip to checkbox with the full title
                    Tipsy.SetToolTip(ChkBox, Mod.Title);
                }
                catch 
                {
                    continue;
                }

                // Increment
                controlsAdded++;
            }
        }

        /// <summary>
        /// Event called when the Cancel button is clicked
        /// </summary>
        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Event fired when the save button is clicked
        /// </summary>
        private void SaveBtn_Click(object sender, EventArgs e)
        {
            // Clear old data
            StatsPython.Config.XpackMedalMods.Clear();

            // Loop through each control and grab the checkboxes
            foreach(Control C in panel2.Controls)
            {
                // Make sure the check box is visible
                if (C is CheckBox && (C as CheckBox).Checked)
                {
                    // Get our mod name
                    string modName = C.Tag as String;
                    if (String.IsNullOrWhiteSpace(modName))
                        continue;

                    // Add mod
                    StatsPython.Config.XpackMedalMods.Add(modName.ToLowerInvariant());
                }
            }

            // Save Config
            StatsPython.Config.Save();
            Notify.Show("Config saved successfully!", "Xpack Enabled Medals Updated Sucessfully");
            this.Close();
        }
    }
}
