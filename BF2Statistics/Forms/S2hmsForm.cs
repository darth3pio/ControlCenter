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
    public partial class S2hmsForm : Form
    {
        public static int LastValue = 0;

        public S2hmsForm()
        {
            InitializeComponent();
        }

        public void SetTime(int Seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(Seconds);
            int Hours = t.Hours;

            // If we have more then 24 hours, then we need to
            // convert the days to hours
            if (t.Days > 0)
                Hours += t.Days * 24;

            // Format
            HoursBox.Value = Hours;
            MinutesBox.Value = t.Minutes;
            SecondsBox.Value = t.Seconds;
        }

        private void ConvertBtn_Click(object sender, EventArgs e)
        {
            float Temp;
            int Hours = Int32.Parse(HoursBox.Value.ToString());
            int Min = Int32.Parse(MinutesBox.Value.ToString());
            int Sec = Int32.Parse(SecondsBox.Value.ToString());

            if (Min > 0)
            {
                Temp = float.Parse((Min * 60).ToString());
                Sec += Int32.Parse((Math.Round(Temp, 0)).ToString());
            }

            if (Hours > 0)
            {
                Temp = float.Parse((Hours * 3600).ToString());
                Sec += Int32.Parse((Math.Round(Temp, 0)).ToString());
            }

            LastValue = Sec;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
