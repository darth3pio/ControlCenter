using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BF2Statistics.Database;
using BF2Statistics.Logging;

namespace BF2Statistics.Gamespy
{
    /// <summary>
    /// The Login Server is used to emulate the Official Gamespy Login servers,
    /// and provide players the ability to create fake "Online" accounts.
    /// </summary>
    public class LoginServer
    {
        /// <summary>
        /// Returns whether the login server is running or not
        /// </summary>
        protected static bool isRunning = false;

        /// <summary>
        /// Returns whether the login server is running or not
        /// </summary>
        public static bool IsRunning
        {
            get { return isRunning; }
        }

        /// <summary>
        /// Gamespy GpCm Server Object
        /// </summary>
        private static GpcmServer CmServer;

        /// <summary>
        /// The Gamespy GpSp Server Object
        /// </summary>
        private static GpspServer SpServer;

        /// <summary>
        /// The Gamespy Database Object
        /// </summary>
        public static GamespyDatabase Database;

        /// <summary>
        /// The Login Server Log Writter
        /// </summary>
        private static LogWritter Logger = new LogWritter(Path.Combine(MainForm.Root, "Logs", "LoginServer.log"), 3000);

        /// <summary>
        /// The status window for the login server to update status messages with
        /// </summary>
        public static TextBox StatusWindow;

        /// <summary>
        /// Event that is fired when the login server is started
        /// </summary>
        public static event StartupEventHandler OnStart;

        /// <summary>
        /// Event that is fired when the login server is shutdown
        /// </summary>
        public static event ShutdownEventHandler OnShutdown;

        /// <summary>
        /// Event fires to update the client list
        /// </summary>
        public static event EventHandler OnUpdate;

        /// <summary>
        /// Starts the Login Server listeners, and begins accepting new connections
        /// </summary>
        public static void Start()
        {
            // Make sure we arent already running!
            if (isRunning)
                return;

            // Clear old text
            StatusWindow.Clear();

            // Start the DB Connection
            try {
                Database = new GamespyDatabase();
            }
            catch(Exception E) {
                StatusWindow.Text += E.Message;
                throw E;
            }

            // Bind gpcm server on port 29900
            try {
                StatusWindow.Text += "Binding to port 29900" + Environment.NewLine;
                CmServer = new GpcmServer();
                CmServer.OnUpdate += new EventHandler(CmServer_OnUpdate);
            }
            catch (Exception ex) {
                StatusWindow.Text += "Error binding to port 29900! " + ex.Message + Environment.NewLine;
                throw ex;
            }

            // Bind gpsp server on port 29901
            try {
                StatusWindow.Text += "Binding to port 29901" + Environment.NewLine;
                SpServer = new GpspServer();
            }
            catch (Exception ex) {
                StatusWindow.Text += "Error binding to port 29901! " + ex.Message + Environment.NewLine;
                throw ex;
            }

            // Let the client know we are ready for connections
            isRunning = true;
            StatusWindow.Text += Environment.NewLine + "Ready for connections!" + Environment.NewLine;

            // Fire event
            OnStart();
        }

        /// <summary>
        /// Sets the status textbox for the login server to push messages to
        /// </summary>
        /// <param name="Window"></param>
        public static void SetStatusBox(TextBox Window)
        {
            StatusWindow = Window;
        }

        static void CmServer_OnUpdate(object sender, EventArgs e)
        {
            OnUpdate(sender, e);
        }

        /// <summary>
        /// Shutsdown the Login Server listeners and stops accepting new connections
        /// </summary>
        public static void Shutdown()
        {
            // Shutdown Login Servers
            CmServer.Shutdown();
            SpServer.Shutdown();

            // Unregister events
            CmServer.OnUpdate -= new EventHandler(CmServer_OnUpdate);

            // Close the database connection
            Database.Close();

            // Trigger the OnShutdown Event
            OnShutdown();

            // Update status
            isRunning = false;
            StatusWindow.Text += "Server shutdown Successfully";
        }

        /// <summary>
        /// Forces the logout of a connected client
        /// </summary>
        /// <param name="Pid"></param>
        /// <returns></returns>
        public static bool ForceLogout(int Pid)
        {
            return (IsRunning) ? CmServer.ForceLogout(Pid) : false;
        }

        /// <summary>
        /// This method is used to store a message in the console.log file
        /// </summary>
        /// <param name="message">The message to be written to the log file</param>
        public static void Log(string message)
        {
            Logger.Write(message);
        }

        /// <summary>
        /// This method is used to store a message in the console.log file
        /// </summary>
        /// <param name="message">The message to be written to the log file</param>
        public static void Log(string message, params object[] items)
        {
            Logger.Write(String.Format(message, items));
        }
    }
}
