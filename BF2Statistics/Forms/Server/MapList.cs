using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Xml;

namespace BF2Statistics
{
    public partial class MapList : Form
    {
        private string[] Maps;
        private string ModPath;
        private string LevelsPath;
        private string MaplistFile;
        private bool isMapSelected = false;

        // Map Desc specific
        List<string> GameModes;
        Dictionary<string, List<string>> ModeSizes;

        public MapList()
        {
            InitializeComponent();

            // Define our paths
            ModPath = Path.Combine(MainForm.Config.ServerPath, "mods", MainForm.SelectedMod);
            LevelsPath = Path.Combine(ModPath, "levels");
            MaplistFile = Path.Combine(ModPath, "settings", "maplist.con");

            // Make sure maplist.con file exists!
            if (!File.Exists(MaplistFile))
            {
                this.Load += new EventHandler(CloseOnStart);
                MessageBox.Show("Maplist.con file is missing! Please make sure your server path is set properly", "Error");
                return;
            }

            // Make sure the levels folder exists!
            if (!Directory.Exists(LevelsPath))
            {
                this.Load += new EventHandler(CloseOnStart);
                MessageBox.Show("The current selected mod does not contain a 'levels' folder. Please select a valid mod.", "Error");
                return;
            }

            // Fetch all maps for the selected mod
            int i = 0;
            Maps = Directory.GetDirectories(LevelsPath);
            foreach (string M in Maps)
            {
                MapListSelect.Items.Add(new Item(M.Remove(0, LevelsPath.Length + 1), i));
                i++;
            }

            // Get the current maplist and display it
            MapListBox.Text = File.ReadAllText(MaplistFile).Trim();
        }

        #region Events

        private void MapListSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Dont Need to reset anything if its the first time loading a map
            if (!isMapSelected)
            {
                GameModeSelect.Enabled = true;
                isMapSelected = true;
            }
            else
            {
                // Reset select list's text
                MapSizeSelect.Text = "";
                GameModeSelect.Text = "";

                // Remove all items from the Mode select and MapSize select
                for (int i = 0; i < GameModeSelect.Items.Count; i++)
                    GameModeSelect.Items.RemoveAt(i);

                for (int i = 0; i < MapSizeSelect.Items.Count; i++)
                    MapSizeSelect.Items.RemoveAt(i);

                // Clear out all GameModeSelect items
                GameModeSelect.Items.Clear();

                // Disable MapList select
                MapSizeSelect.Enabled = false;
                AddToMapList.Enabled = false;
            }

            // Load our current selected map
            LoadMap();

            // Add all map supported game modes to the GameMode select list
            foreach (string mode in GameModes)
            {
                GameModeSelect.Items.Add(new Item(mode, 1));
            }
        }

        /// <summary>
        /// Adds completed map selections to the Maplist
        /// </summary>
        private void AddToMapList_Click(object sender, EventArgs e)
        {
            string map = MapListSelect.SelectedItem.ToString();
            string mode = GameModeSelect.SelectedItem.ToString();
            string size = MapSizeSelect.SelectedItem.ToString();
            if (!String.IsNullOrEmpty(MapListBox.Text))
                MapListBox.Text += Environment.NewLine;
            MapListBox.Text += String.Format("mapList.append {0} {1} {2}", map, mode, size);
        }

        /// <summary>
        /// Clears the current Maplist
        /// </summary>
        private void ClearButton_Click(object sender, EventArgs e)
        {
            MapListBox.Text = "";
        }

        /// <summary>
        /// Saves the maplist to the maplist.com file
        /// </summary>
        private void SaveButton_Click(object sender, EventArgs e)
        {
            File.WriteAllLines(MaplistFile, MapListBox.Lines);
            this.Close();
        }

        /// <summary>
        /// Event fired when the client selects a Map Gamemode
        /// </summary>
        private void GameModeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            object selected = GameModeSelect.SelectedItem;
            string mode = selected.ToString();

            // Remove all Mapsize selects
            MapSizeSelect.Items.Clear();

            foreach (string size in ModeSizes[mode])
            {
                MapSizeSelect.Items.Add(new Item(size, 1));
            }

            MapSizeSelect.Enabled = true;
        }

        /// <summary>
        /// Event fired when a mapsize is selected
        /// </summary>
        private void MapSizeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            AddToMapList.Enabled = true;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Method for loading the Map Desc file, which holds all the gamemode
        /// and map size information for the selected map
        /// </summary>
        private void LoadMap()
        {
            // Initialize new GameModes and MapSizes
            GameModes = new List<string>();
            ModeSizes = new Dictionary<string, List<string>>();

            string map = MapListSelect.SelectedItem.ToString();
            string dFile = Path.Combine(LevelsPath, map, "Info", map + ".desc");

            XmlDocument Doc = new XmlDocument();
            Doc.Load(dFile);

            XmlNodeList Modes = Doc.GetElementsByTagName("mode");
            foreach (XmlNode m in Modes)
            {
                string mode = m.Attributes["type"].InnerText;
                GameModes.Add(mode);

                List<string> temp = new List<string>();
                foreach (XmlNode c in m.ChildNodes)
                {
                    temp.Add(c.Attributes["players"].InnerText);
                }

                ModeSizes.Add(mode, temp);
            }
        }

        #endregion

        /// <summary>
        /// Event closes the form when fired
        /// </summary>
        private void CloseOnStart(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
