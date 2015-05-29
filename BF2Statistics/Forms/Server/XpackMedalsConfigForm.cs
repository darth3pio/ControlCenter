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
                // get our working mod
                BF2Mod Mod = BF2Server.Mods[i];

                // Bf2 is predefined
                if (Mod.Name.Equals("bf2", StringComparison.InvariantCultureIgnoreCase))
                {
                    checkBox1.Checked = StatsPython.Config.XpackMedalMods.Contains(Mod.Name);
                    continue;
                }

                // Xpack is predefined
                if (Mod.Name.Equals("xpack", StringComparison.InvariantCultureIgnoreCase))
                {
                    checkBox2.Checked = StatsPython.Config.XpackMedalMods.Contains(Mod.Name);
                    continue;
                }

                // Stop if over 10 added mods. I chose to use continue here instead of break
                // So that Xpack can get ticked if it may be at the bottom of the list
                if (controlsAdded >= 10) continue;

                // Enable Control
                int index = controlsAdded + 3;
                Control[] Controls = this.Controls.Find("checkBox" + index, true);
                if (Controls.Length == 0)
                    throw new Exception("A crazy error happened, but a checkBox has seized to exist!");

                try
                {
                    // Configure Checkbox
                    CheckBox C = Controls[0] as CheckBox;
                    string title = (Mod.Title.Length > 32) ? Mod.Title.CutTolength(29) + "..." : Mod.Title;
                    C.Text = String.Format("{0} [{1}]", title, Mod.Name);
                    C.Checked = StatsPython.Config.XpackMedalMods.Contains(Mod.Name);
                    C.Tag = Mod.Name;
                    C.Show();

                    // Add tooltip to checkbox with the full title
                    Tipsy.SetToolTip(C, Mod.Title);
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
            Control Con = this.Controls.Find("panel2", true)[0];
            foreach(Control C in Con.Controls)
            {
                // Make sure the check box is visible
                if(C is CheckBox && (C as CheckBox).Checked)
                {
                    // Get our mod name
                    string modName = Regex.Match(C.Text, @"\[(?<value>[A-Za-z0-9_\s',]*)\]").Groups["value"].Value;
                    if (String.IsNullOrWhiteSpace(modName))
                        continue;

                    // Add mod
                    StatsPython.Config.XpackMedalMods.Add(modName);
                }
            }

            // Save Config
            StatsPython.Config.Save();
            Notify.Show("Config saved successfully!", "Xpack Enabled Medals Updated Sucessfully");
            this.Close();
        }
    }
}
