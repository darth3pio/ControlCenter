/// <summary>
/// ---------------------------------------
/// Battlefield 2 Statistics Control Center
/// ---------------------------------------
/// Created By: Steven Wilson <Wilson212>
/// Copyright (C) 2013, Steven Wilson. All Rights Reserved
/// ---------------------------------------
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BF2Statistics
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            catch (Exception E)
            {
                MessageBox.Show("A Startup error has occured!" 
                    + Environment.NewLine 
                    + Environment.NewLine 
                    + "Exception Message: " + E.Message 
                    + "Target Method: " + E.TargetSite.ToString() 
                    + "Stack Trace: " + E.StackTrace.ToString(),
                    "Startup Error");
            }
        }
    }
}
