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
    public partial class RandomizeForm : Form
    {
        public RandomizeForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event fired when the Generate Button is clicked
        /// Does the random Generating
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateBtn_Click(object sender, EventArgs e)
        {
            // Initialize internal variables
            BF2Mod Mod = MainForm.SelectedMod;
            Random Rnd = new Random();
            List<string> Modes = new List<string>();
            List<string> Sizes = new List<string>();
            int Num = (int) NumMaps.Value;
            int MapCount = Mod.Levels.Count;
            StringBuilder Sb = new StringBuilder();

            // Get list of supported Game Modes the user wants
            if (ConquestBox.Checked)
                Modes.Add("gpm_cq");
            if (CoopBox.Checked)
                Modes.Add("gpm_coop");

            // Get list of sizes the user wants
            if (s16Box.Checked)
                Sizes.Add("16");
            if (s32Box.Checked)
                Sizes.Add("32");
            if (s64Box.Checked)
                Sizes.Add("64");

            if (Modes.Count == 0)
            {
                // Handle Message
                MessageBox.Show("You must select at least 1 GameMode!", "User Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (Sizes.Count == 0)
            {
                // Handle Message
                MessageBox.Show("You must select at least 1 Map Size!", "User Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Loop through, the number of times the user specified, adding a map
            for (int i = 0; i < Num; i++)
            {
                // Grab a random map from the levels array
                string map = Mod.Levels[Rnd.Next(MapCount)];

                try
                {
                    // Try and load the map... if an exception is thrown, this loop doesnt count
                    BF2Map Map = Mod.LoadMap(map);

                    // Get a random gamemode key
                    string Key = Modes[Rnd.Next(Modes.Count)];
                    if (Map.GameModes.ContainsKey(Key))
                    {
                        // Get the common map sizes between what the user wants, and what the map supports
                        string[] Common = Map.GameModes[Key].Intersect(Sizes).ToArray();

                        // If there are no common sizes, try another map
                        if (Common.Length == 0)
                        {
                            ++Num;
                            continue;
                        }

                        // Get a random size, and add the map
                        string Size = Common[Rnd.Next(Common.Length)];
                        Sb.AppendLine(Map.Name + " " + Key + " " + Size);
                    }
                    else
                        ++Num;
                    
                }
                catch (InvalidMapException) 
                {
                    Num++;
                }
            }

            // Add new maplist to the maplist box
            MapListBox.Text = Sb.ToString();

        }

        /// <summary>
        /// Event fired when the Cancel button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Saves the current generated maplist into the maplist.con file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveBtn_Click(object sender, EventArgs e)
        {
            // Make sure we have at least 1 map :/
            int Len = MapListBox.Lines.Length - 1;
            if (Len == 0 || String.IsNullOrWhiteSpace(MapListBox.Text))
            {
                MessageBox.Show("There must be at least 1 map before saving!", "User Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Append the prefix to each map line
            string[] Lines = new string[Len];
            for (int i = 0; i < Len; i++)
                Lines[i] = "mapList.append " + MapListBox.Lines[i];

            // Save and close
            MainForm.SelectedMod.MapList = Lines;
            this.Close();
        }
    }
}
