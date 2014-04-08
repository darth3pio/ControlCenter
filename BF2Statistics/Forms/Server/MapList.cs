using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Xml;
using FreeImageAPI;

namespace BF2Statistics
{
    public partial class MapList : Form
    {
        /// <summary>
        /// An array of map names found in the clients "levels" folder
        /// </summary>
        private List<string> ClientMaps;

        /// <summary>
        /// The selected BF2 Map object
        /// </summary>
        private BF2Map SelectedMap;

        /// <summary>
        /// Specifies whether a map is currently selected in the form
        /// </summary>
        private bool isMapSelected = false;

        /// <summary>
        /// Contains the full sized bitmap image of the selected map
        /// </summary>
        protected Bitmap MapImage;

        /// <summary>
        /// Constructor
        /// </summary>
        public MapList()
        {
            InitializeComponent();

            // Define vars
            ClientMaps = new List<string>();
            BF2Mod Mod = MainForm.SelectedMod;

            // Fetch all maps for the selected mod
            foreach (string Map in Mod.Levels)
                MapListSelect.Items.Add(Map);

            // Get Client maps
            if (!String.IsNullOrEmpty(MainForm.Config.ClientPath))
            {
                string P = Path.Combine(MainForm.Config.ClientPath, "mods", Mod.Name, "levels");
                if (Directory.Exists(P))
                {
                    foreach (string Map in Directory.GetDirectories(P))
                        ClientMaps.Add(Map.Substring(P.Length + 1));
                }
            }

            // Get the current maplist and display it
            MapListBox.Lines = Mod.MapList;
        }

        #region Events

        /// <summary>
        /// Called when a map is selected or changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapListSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reset image
            MapPictureBox.Image = null;

            // If an error occurs while loading the maps .desc file, then just return
            if (MapListSelect.SelectedIndex == -1)
                return;

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
                GameModeSelect.Items.Clear();
                MapSizeSelect.Items.Clear();

                // Disable MapList select
                MapSizeSelect.Enabled = false;
                AddToMapList.Enabled = false;
            }

            // Try and load the map object... a common error here is that the
            // Descriptor file containing illegal XML characters
            try
            {
                string map = MapListSelect.SelectedItem.ToString();
                SelectedMap = MainForm.SelectedMod.LoadMap(map);
                isMapSelected = true;
            }
            catch (Exception E)
            {
                // Get our Inner exception message if its an InvalidMapException
                // We do this because InvalidMapException doesnt really tell us the issue,
                // but if there is an inner exception, we will have the issue.
                string mess = (E is InvalidMapException && E.InnerException != null) 
                    ? E.InnerException.Message 
                    : E.Message;
                MessageBox.Show("There was an error loading the map descriptor file"
                    + Environment.NewLine + Environment.NewLine
                    + "Message: " + mess,
                    "Map Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Warning
                );

                // Reset the GUI
                MapListSelect.SelectedIndex = -1;
                GameModeSelect.Enabled = false;
                isMapSelected = false;
            }

            // If we have no map loaded, it failed
            if (!isMapSelected)
                return;

            // Add all map supported game modes to the GameMode select list
            foreach (KeyValuePair<string, List<string>> Mode in SelectedMap.GameModes)
                GameModeSelect.Items.Add(new KeyValueItem(Mode.Key, GameModeToString(Mode.Key)));

            // Set the default map gamemode
            GameModeSelect.SelectedIndex = 0;
        }

        /// <summary>
        /// Adds completed map selections to the Maplist
        /// </summary>
        private void AddToMapList_Click(object sender, EventArgs e)
        {
            // Get Values
            string map = MapListSelect.SelectedItem.ToString();
            string mode = ((KeyValueItem) GameModeSelect.SelectedItem).Key;
            string size = MapSizeSelect.SelectedItem.ToString();

            // Warn user if client doesnt support map
            if (ClientMaps.Count > 0 && !ClientMaps.Contains(map, StringComparer.OrdinalIgnoreCase))
            {
                if (MessageBox.Show(
                    "The Battlfield 2 Client does not contain this map! Are you sure you want to add it to the maplist?",
                    "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                    return;
            }

            // Add to maplist
            if (!String.IsNullOrEmpty(MapListBox.Text))
                MapListBox.Text += Environment.NewLine;
            MapListBox.Text += String.Format("mapList.append {0} {1} {2}", map, mode, size);
        }

        /// <summary>
        /// Event fired when the client selects a Map Gamemode
        /// </summary>
        private void GameModeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Remove all Mapsize selects
            MapSizeSelect.Items.Clear();
            MapSizeSelect.Enabled = false;
            AddToMapList.Enabled = false;

            // Add new map sizes for the selected game mode
            string mode = ((KeyValueItem) GameModeSelect.SelectedItem).Key;

            // Add all supported map sizes. If we donot have mapsize support, I assume
            // we are in a Sp1 mod.
            if (SelectedMap.GameModes[mode].Count > 0)
            {
                foreach (string size in SelectedMap.GameModes[mode])
                    MapSizeSelect.Items.Add(size);

                MapSizeSelect.Enabled = true;
            }
            else
            {
                switch (mode)
                {
                    case "sp1":
                        MapSizeSelect.Items.Add("16");
                        break;
                    case "sp2":
                        MapSizeSelect.Items.Add("32");
                        break;
                    case "sp3":
                        MapSizeSelect.Items.Add("64");
                        break;
                    default:
                        MapSizeSelect.Items.Add("16");
                        MapSizeSelect.Items.Add("32");
                        MapSizeSelect.Items.Add("64");
                        MapSizeSelect.Enabled = true;
                        break;
                }
            }

            // Set default index
            MapSizeSelect.SelectedIndex = 0;
        }

        /// <summary>
        /// Event fired when a mapsize is selected
        /// </summary>
        private void MapSizeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reset image
            if (MapPictureBox.Image != null)
            {
                Bitmap Img = MapPictureBox.Image as Bitmap;
                Img.Dispose();
            }

            // Dispose old image
            if (MapImage != null)
                MapImage.Dispose();

            // Enable add button
            AddToMapList.Enabled = true;

            // Load map image
            // Get Values
            string map = MapListSelect.SelectedItem.ToString();
            string mode = ((KeyValueItem)GameModeSelect.SelectedItem).Key;
            string size = MapSizeSelect.SelectedItem.ToString();
            string ImgPath = Path.Combine(SelectedMap.RootPath, "Info", mode + "_" + size + "_menumap.png");

            // Alot of server files dont contain the map image files, so search the client if we need to
            if (!File.Exists(ImgPath))
            {
                if (!String.IsNullOrWhiteSpace(MainForm.Config.ClientPath))
                {
                    ImgPath = Path.Combine(SelectedMap.RootPath, "Info", mode + "_" + size + "_menumap.png");

                    // If the client doesnt have the image either, then Oh well :(
                    if (!File.Exists(ImgPath))
                        ImgPath = null;
                }
                else
                    ImgPath = null;
            }

            // Load the image if we have one
            if (ImgPath != null)
            {
                // Attempt to load image as a DDS file
                FREE_IMAGE_FORMAT Format = FREE_IMAGE_FORMAT.FIF_UNKNOWN;
                MapImage = FreeImage.LoadBitmap(ImgPath, FREE_IMAGE_LOAD_FLAGS.DEFAULT, ref Format);

                // If we have an image bitmap, display it :D
                if (MapImage != null)
                {
                    MapPictureBox.Image = new Bitmap(MapImage, 250, 250);
                }
            }
        }

        /// <summary>
        /// Event fired when the map image is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapPictureBox_DoubleClick(object sender, EventArgs e)
        {
            if (MapListSelect.SelectedIndex != -1 && MapPictureBox.Image != null)
            {
                ImageForm F = new ImageForm(MapImage);
                F.ShowDialog();
            }
        }

        /// <summary>
        /// Clears the current Maplist
        /// </summary>
        private void ClearButton_Click(object sender, EventArgs e)
        {
            MapListBox.Text = "";
        }

        /// <summary>
        /// Cancels any changes made to the maplist, and closes the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Shuffles the maps listed in the MapList box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RandomizeBtn_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            MapListBox.Lines = MapListBox.Lines.OrderBy(line => rnd.Next()).ToArray();
        }

        /// <summary>
        /// Saves the maplist to the maplist.com file
        /// </summary>
        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Make sure we have something in the maplist!
            if (String.IsNullOrWhiteSpace(MapListBox.Text))
            {
                MessageBox.Show("You must at least have 1 map before saving the maplist!", "User Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Save and close
            MainForm.SelectedMod.MapList = MapListBox.Lines;
            this.Close();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Parses a maplist.con game mode variable into a human readable one.
        /// </summary>
        /// <param name="mode">The gamemode from the maplist.con</param>
        /// <returns></returns>
        protected string GameModeToString(string mode)
        {
            switch (mode)
            {
                case "gpm_coop":
                    return "Co-op";
                case "gpm_cq":
                    return "Conquest";
                case "sp1":
                    return "Singleplayer 16";
                case "sp2":
                    return "Singleplayer 32";
                case "sp3":
                    return "Singleplayer 64";
                default:
                    return mode;
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
