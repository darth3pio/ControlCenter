using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BF2Statistics
{
    public partial class SnapshotFilterForm : Form
    {
        /// <summary>
        /// A list of all found maps, between all found mods, in lower case format
        /// </summary>
        private List<string> MapNames = new List<string>();

        public SnapshotFilterForm()
        {
            InitializeComponent();

            // Set auto complete data
            this.MapNameBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            this.MapNameBox.AutoCompleteSource = AutoCompleteSource.CustomSource;

            // Get our list of maps
            foreach (BF2Mod Mod in BF2Server.Mods)
            {
                MapNames.AddRange(
                    from x in Mod.Levels where !MapNames.Contains(x.ToLowerInvariant()) 
                    select x.ToLowerInvariant()
                );
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TextBox t = sender as TextBox;
            if (t != null)
            {
                // say you want to do a search when user types 3 or more chars
                if (t.Text.Length >= 1)
                {
                    AutoCompleteStringCollection Collection = new AutoCompleteStringCollection();
                    Collection.AddRange((from x in MapNames where x.StartsWith(t.Text) select x).ToArray());
                    this.MapNameBox.AutoCompleteCustomSource = Collection;
                }
            }
        }
    }
}
