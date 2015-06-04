using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BF2Statistics
{
    public partial class RandomizeForm : NativeForm
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
        private async void GenerateBtn_Click(object sender, EventArgs e)
        {
            // Initialize lists
            List<string> Modes = new List<string>();
            List<string> Sizes = new List<string>();

            // Get list of supported Game Modes the user wants
            if (ConquestBox.Checked) Modes.Add("gpm_cq");
            if (CoopBox.Checked)     Modes.Add("gpm_coop");

            // Get list of sizes the user wants
            if (s16Box.Checked) Sizes.Add("16");
            if (s32Box.Checked) Sizes.Add("32");
            if (s64Box.Checked) Sizes.Add("64");

            // Make sure we have at least 1 mode and size
            if (Modes.Count == 0 || Sizes.Count == 0)
            {
                // Handle Message
                MessageBox.Show("You must select at least 1 GameMode and Map Size!", "User Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Initialize internal variables
            BF2Mod Mod = MainForm.SelectedMod;
            Random Rnd = new Random();
            int NumOfMapsToAdd = (int)NumMaps.Value;
            int MapCount = Mod.Levels.Count;
            string[] gModes = Modes.ToArray();
            StringBuilder Sb = new StringBuilder();

            // Shuffle the maplist
            Mod.Levels.Shuffle();

            // Show loading form
            LoadingForm.ShowScreen(this);
            SetNativeEnabled(false);

            // Don't lockup GUI, run in a new task
            await Task.Run(() =>
            {
                // Loop through, the number of times the user specified, adding a map
                for (int i = 0; i < NumOfMapsToAdd; i++)
                {
                    // Prevent infinite looping and/or quit if we have reached the map count
                    if (i > 255 || (noDupesBox.Checked && i == MapCount))
                        break;

                    // Grab a random map from the levels array
                    try
                    {
                        // Try and load the map... if an exception is thrown, this loop doesnt count
                        BF2Map Map = Mod.LoadMap((noDupesBox.Checked) ? Mod.Levels[i] : Mod.Levels[Rnd.Next(0, MapCount)]);

                        // Get the common intersected gamemodes that the map has in common with what the user wants
                        string[] Common = Map.GameModes.Keys.ToArray();
                        Common = gModes.Intersect(Common).ToArray();

                        // No common game modes
                        if (Common.Length == 0)
                        {
                            NumOfMapsToAdd++;
                            continue;
                        }

                        // Get a random gamemode key
                        string Mode = Common[Rnd.Next(0, Common.Length)];

                        // Get the common map sizes between what the user wants, and what the map supports
                        Common = Map.GameModes[Mode].Intersect(Sizes).ToArray();
                        if (Common.Length == 0)
                        {
                            // No common sizes, try another map
                            NumOfMapsToAdd++;
                            continue;
                        }

                        // Get a random size, and add the map
                        string Size = Common[Rnd.Next(0, Common.Length)];
                        Sb.AppendLine(Map.Name + " " + Mode + " " + Size);
                    }
                    catch (InvalidMapException)
                    {
                        NumOfMapsToAdd++;
                    }
                }
            });

            // Add new maplist to the maplist box
            MapListBox.Text = Sb.ToString();
            SetNativeEnabled(true);
            LoadingForm.CloseForm();
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
