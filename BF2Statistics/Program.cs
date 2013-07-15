/// <summary>
/// ---------------------------------------
/// Battlefield 2 Statistics Control Center
/// ---------------------------------------
/// Created By: Steven Wilson <Wilson212>
/// Copyright (C) 2013, Steven Wilson. All Rights Reserved
/// ---------------------------------------
/// </summary>

using System;
using System.Windows.Forms;
using System.IO;
using System.Threading;

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
            // Set exception Handler
            Application.ThreadException += new ThreadExceptionEventHandler(ExceptionHandler.OnThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler.OnUnhandledException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
