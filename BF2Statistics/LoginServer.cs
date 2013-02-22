using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BF2Statistics.Database;
using System.Windows.Forms;

namespace BF2Statistics
{
    public delegate void ShutdownEventHandler();

    public class LoginServer
    {
        protected static bool isRunning = false;

        public static bool IsRunning
        {
            get { return isRunning; }
        }

        private static GpcmServer CmServer;

        private static GpspServer SpServer;

        public static GamespyDatabase Database;

        private static StreamWriter LogFile = File.AppendText( Path.Combine(MainForm.Root, "Logs", "server.log") );

        public static TextBox StatusWindow;

        public static event ShutdownEventHandler OnShutdown;

        public static event EventHandler OnUpdate;

        public static void Start(TextBox StatusTextBox)
        {
            // Make sure we arent already running!
            if (isRunning)
                return;

            isRunning = true;

            // Clear old text
            StatusWindow = StatusTextBox;
            StatusWindow.Clear();

            // Start the DB Connection
            try {
                Database = new GamespyDatabase();
            }
            catch(Exception E) {
                isRunning = false;
                StatusTextBox.Text += E.Message;
                throw E;
            }

            // Bind gpcm server on port 29900
            try {
                StatusWindow.Text += "Binding to port 29900" + Environment.NewLine;
                CmServer = new GpcmServer();
                CmServer.OnUpdate += new EventHandler(CmServer_OnUpdate);
            }
            catch (Exception ex) {
                isRunning = false;
                StatusWindow.Text += "Error binding to port 29900! " + ex.Message + Environment.NewLine;
                throw ex;
            }

            // Bind gpsp server on port 29901
            try {
                StatusWindow.Text += "Binding to port 29901" + Environment.NewLine;
                SpServer = new GpspServer();
            }
            catch (Exception ex) {
                isRunning = false;
                StatusWindow.Text += "Error binding to port 29901! " + ex.Message + Environment.NewLine;
                throw ex;
            }

            // Let the client know we are ready for connections
            StatusWindow.Text += Environment.NewLine + "Ready for connections!" + Environment.NewLine;
        }

        static void CmServer_OnUpdate(object sender, EventArgs e)
        {
            OnUpdate(sender, e);
        }

        public static void Shutdown()
        {
            // Shutdown Login Servers
            CmServer.Shutdown();
            SpServer.Shutdown();

            // Close the database connection
            Database.Close();

            // Trigger the OnShutdown Event
            OnShutdown();

            // Update status
            isRunning = false;
            StatusWindow.Text += "Server shutdown Successfully";
        }

        /// <summary>
        /// This method is used to store a message in the console.log file
        /// </summary>
        /// <param name="message">The message to be written to the log file</param>
        public static void Log(string message)
        {
            DateTime datet = DateTime.Now;
            try
            {
                LogFile.WriteLine(datet.ToString("MM/dd hh:mm") + "> " + message);
                LogFile.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
            }
        }

        /// <summary>
        /// This method is used to store a message in the console.log file
        /// </summary>
        /// <param name="message">The message to be written to the log file</param>
        public static void Log(string message, params object[] items)
        {
            Log(String.Format(message, items));
        }
    }
}
