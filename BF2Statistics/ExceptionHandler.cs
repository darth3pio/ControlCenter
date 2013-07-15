using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;

namespace BF2Statistics
{
    /// <summary>
    /// A simple object to handle exceptions thrown during runtime
    /// </summary>
    public sealed class ExceptionHandler
    {
        private ExceptionHandler() { }

        /// <summary>
        /// Handles an exception on the main thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="t"></param>
        public static void OnThreadException(object sender, ThreadExceptionEventArgs t)
        {
            // Display the Exception Form
            ExceptionForm EForm = new ExceptionForm(t.Exception, true);
            EForm.Message = "An unhandled exception was thrown while trying to preform the requested task.\r\n"
                + "If you click Continue, the application will attempt to ignore this error, and continue. "
                + "If you click Quit, the application will close immediatly.";
            DialogResult Result = EForm.ShowDialog();

            // Kill the form on abort
            if (Result == DialogResult.Abort)
                Application.Exit();
        }

        /// <summary>
        /// Handles cross thread exceptions, that are unrecoverable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Create Trace Log
            string FileName = Path.Combine(Paths.DocumentsFolder, "traceLog_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".txt");
            Exception Ex = e.ExceptionObject as Exception;
            GenerateTraceLog(FileName, Ex);

            // Display the Exception Form
            ExceptionForm EForm = new ExceptionForm(Ex, false);
            EForm.Message = "An unhandled exception was thrown while trying to preform the requested task.\r\n"
                + "A trace log was generated under the \"My Documents/BF2Stastistics\" folder, to "
                + "assist with debugging, and getting help with this error.";
            EForm.LogFile = FileName;
            EForm.ShowDialog();
            Application.Exit();
        }

        /// <summary>
        /// Generates a trace log for an exception
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="E"></param>
        public static void GenerateTraceLog(string FileName, Exception E)
        {
            // Try to write to the log
            try
            {
                using (StreamWriter Log = new StreamWriter(File.Open(FileName, FileMode.Create, FileAccess.Write)))
                {
                    Win32Exception Ex = E as Win32Exception;
                    if (Ex == null && E.InnerException != null)
                        Ex = E.InnerException as Win32Exception;

                    Log.WriteLine("Date: " + DateTime.Now.ToString());
                    Log.WriteLine("Exception: " + E.Message.Replace("\n", "\n\t"));
                    if (Ex != null)
                        Log.WriteLine("Exception Code: " + Ex.ErrorCode);
                    Log.WriteLine("Target Method: " + E.TargetSite.ToString());
                    Log.WriteLine("StackTrace:");
                    Log.WriteLine(E.StackTrace);
                    Log.Close();
                }
            }
            catch { }
        }

        
    }
}
