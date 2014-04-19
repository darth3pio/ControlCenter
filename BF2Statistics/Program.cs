/// <summary>
/// ---------------------------------------
/// Battlefield 2 Statistics Control Center
/// ---------------------------------------
/// Created By: Steven Wilson <Wilson212>
/// Copyright (C) 2013, Steven Wilson. All Rights Reserved
/// ---------------------------------------
/// </summary>

using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Security.Principal;
using BF2Statistics.Logging;

namespace BF2Statistics
{
    static class Program
    {
        /// <summary>
        /// Specifies the Program Version
        /// </summary>
        public static Version Version = new Version(1, 8, 2);

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
        static void Main()
        {
            // Set exception Handler
            Application.ThreadException += new ThreadExceptionEventHandler(ExceptionHandler.OnThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler.OnUnhandledException);

            // Create ErrorLog file
            ErrorLog = new LogWritter(Path.Combine(Application.StartupPath, "Logs", "Error.log"), 3000);

            // Load the main form!
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
