/// <summary>
/// ---------------------------------------
/// Battlefield 2 Statistics Control Center
/// ---------------------------------------
/// Created By: Steven Wilson <Wilson212>
/// Copyright (C) 2013-2014, Steven Wilson. All Rights Reserved
/// ---------------------------------------
/// </summary>

using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Security.Principal;
using BF2Statistics.Logging;
using BF2Statistics.Properties;

namespace BF2Statistics
{
    static class Program
    {
        /// <summary>
        /// Specifies the Program Version
        /// </summary>
        public static readonly Version Version = new Version(1, 9, 2);

        /// <summary>
        /// Specifies the installation directory of this program
        /// </summary>
        public static readonly string RootPath = Application.StartupPath;

        /// <summary>
        /// The main form log file
        /// </summary>
        public static LogWritter ErrorLog;

        /// <summary>
        /// Returns whether the app is running in administrator mode.
        /// </summary>
        public static bool IsAdministrator
        {
            get
            {
                WindowsPrincipal wp = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                return wp.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Set exception Handler
            Application.ThreadException += new ThreadExceptionEventHandler(ExceptionHandler.OnThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler.OnUnhandledException);

            // Create ErrorLog file
            ErrorLog = new LogWritter(Path.Combine(Application.StartupPath, "Logs", "Error.log"));

            // Are we Uninstalling?
            /* DOES NOT WORK WITH SETUP WIZARD
            if (args.Length > 0 && args[0] == "-u")
            {
                // Ask the user, if we have a backup created, if they want to restore back to original python
                string ServerPath = Settings.Default.ServerPath;
                if(!String.IsNullOrWhiteSpace(ServerPath) && Directory.Exists(Path.Combine(ServerPath, "python", "_backup_")))
                {
                    DialogResult R = MessageBox.Show(
                        "Would you like to remove the Bf2Statistics python, and restore the original python files?", 
                        "Uninstall", MessageBoxButtons.YesNo, MessageBoxIcon.Question
                    );

                    // IF user wants python restored
                    if (R == DialogResult.Yes)
                    {
                        // Define paths
                        string BackupPath = Path.Combine(ServerPath, "python", "_backup_");
                        string RankedPath = Path.Combine(ServerPath, "python", "bf2");

                        // Delete the current python directory
                        Directory.Delete(RankedPath, true);
                        System.Threading.Thread.Sleep(750);

                        // Copy back the original contents
                        Directory.Move(BackupPath, RankedPath);
                    }
                }

                return;
            }
            */

            // Load the main form!
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
