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
    /// <summary>
    /// This is a dummy form used to display a full sized map image from the MapList
    /// Editor.
    /// </summary>
    public partial class ImageForm : Form
    {
        public ImageForm(Bitmap Image)
        {
            InitializeComponent();
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Image = Image;
            this.Height = Image.Height;
            this.Width = Image.Width;
        }
    }
}
