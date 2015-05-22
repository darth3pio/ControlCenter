using System;
using System.IO;
using System.Windows.Forms;
using BF2Statistics.Database;
using BF2Statistics.Logging;

namespace BF2Statistics.Gamespy
{
    /// <summary>
    /// The Gamespy Server is used to emulate the Official Gamespy Login servers,
    /// and provide players the ability to create fake "Online" accounts.
    /// </summary>
    public class GamespyEmulator
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
        /// Returns a list of all the connected clients
        /// </summary>
        public static GpcmClient[] ConnectedClients
        {
            get 
            { 
                return (IsRunning) ? CmServer.ConnectedClients : new GpcmClient[0]; 
            }
        }

        /// <summary>
        /// Returns the number of connected players that are logged in
        /// </summary>
        public static int NumClientsConencted
        {
            get { return (IsRunning) ? CmServer.NumClients : 0; }
        }

        /// <summary>
        /// The Number of servers that are currently online and actively
        /// reporting to this master server
        /// </summary>
        public static int ServersOnline
        {
            get { return MasterServer.Servers.Count;  }
        }

        /// <summary>
        /// Gamespy Client Manager Server Object
        /// </summary>
        private static GpcmServer CmServer;

        /// <summary>
        /// The Gamespy Search Provider Server Object
        /// </summary>
        private static GpspServer SpServer;

        /// <summary>
        /// The Gamespy Master Server
        /// </summary>
        private static MasterServer MstrServer;

        /// <summary>
        /// The Gamespy CDKey server
        /// </summary>
        private static CDKeyServer CDKeyServer;

        /// <summary>
        /// The Login Server Log Writter
        /// </summary>
        private static LogWriter Logger;

        /// <summary>
        /// The Gamespy Debug Log
        /// </summary>
        private static LogWriter DebugLog;

        /// <summary>
        /// Event that is fired when the login server is started
        /// </summary>
        public static event StartupEventHandler Started;

        /// <summary>
        /// Event that is fired when the login server is shutdown
        /// </summary>
        public static event ShutdownEventHandler Stopped;

        /// <summary>
        /// Event fires when a player logs in or disconnects from the login server
        /// </summary>
        public static event EventHandler OnClientsUpdate;

        /// <summary>
        /// Event fires when a server is added or removed from the online serverlist
        /// </summary>
        public static event EventHandler OnServerlistUpdate;

        static GamespyEmulator()
        {
            // Create our log files
            Logger = new LogWriter(Path.Combine(Program.RootPath, "Logs", "LoginServer.log"), true);
            DebugLog = new LogWriter(Path.Combine(Program.RootPath, "Logs", "GamespyDebug.log"));

            // Register for events
            GpcmServer.OnClientsUpdate += (s, e) => OnClientsUpdate(s, e);
            MasterServer.OnServerlistUpdate += (s, e) => OnServerlistUpdate(s, e);
        }

        /// <summary>
        /// Starts the Login Server listeners, and begins accepting new connections
        /// </summary>
        public static void Start()
        {
            // Make sure we arent already running!
            if (isRunning) return;

            // Start the DB Connection
            using (GamespyDatabase Database = new GamespyDatabase()) 
            {
                // First, make sure our account table exists
                if (!Database.TablesExist)
                {
                    string message = "In order to use the Gamespy Emulation feature of this program, we need to setup a database. "
                    + "You may choose to do this later by clicking \"Cancel\". Would you like to setup the database now?";
                    DialogResult R = MessageBox.Show(message, "Gamespy Database Setup", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (R == DialogResult.Yes)
                        SetupManager.ShowDatabaseSetupForm(DatabaseMode.Gamespy);

                    // Call the stoOnShutdown event to Re-enable the main forms buttons
                    Stopped();
                    return;
                }
                else if (Database.NeedsUpdated)
                {
                    // We cannot run an outdated database
                    DialogResult R = MessageBox.Show(
                        String.Format(
                            "The Gamespy database tables needs to be updated to version {0} before using this feature. Would you like to do this now?",
                            GamespyDatabase.LatestVersion
                        ) + Environment.NewLine.Repeat(1) + 
                        "NOTE: You should backup your gamespy account table if you are unsure as this update cannot be undone!", 
                        "Gamespy Database Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question
                    );

                    // If the user doesnt migrate the database tables, quit
                    if (R != DialogResult.Yes)
                    {
                        // Call the stoOnShutdown event to Re-enable the main forms buttons
                        Stopped();
                        return;
                    }
                    
                    // Do table migrations
                    Database.MigrateTables();
                }
            }

            // Bind gpcm server on port 29900
            int port = 29900;

            // Setup the DebugLog
            DebugLog.LoggingEnabled = MainForm.Config.GamespyServerDebug;
            if(MainForm.Config.GamespyServerDebug)
                DebugLog.ClearLog();

            try 
            {
                // Begin logging
                DebugLog.Write("=== Gamespy Emulator Initializing ===");
                DebugLog.Write("Starting Client Manager");

                // Start the client manager
                CmServer = new GpcmServer();

                // Begin logging
                DebugLog.Write("Bound to TCP port: " + port);
                DebugLog.Write("Starting Account Service Provider");

                // Start server provider server
                port++;
                SpServer = new GpspServer();

                // Begin logging
                DebugLog.Write("Bound to TCP port: " + port);
                DebugLog.Write("Starting Master Server");

                // Start then Master Server
                MstrServer = new MasterServer(ref port, DebugLog);

                // Start CDKey Server
                port = 29910;
                DebugLog.Write("Starting Cdkey Server");
                CDKeyServer = new CDKeyServer(DebugLog);

                // Begin logging
                DebugLog.Write("=== Gamespy Emulator Initialized ===");
            }
            catch (Exception E) 
            {
                Notify.Show(
                    "Failed to Start Gamespy Servers!", 
                    "Error binding to port " + port + ": " + Environment.NewLine + E.Message, 
                    AlertType.Warning
                );

                // Append log
                if (DebugLog != null)
                {
                    DebugLog.Write("=== Failed to Start Emulator Servers! ===");
                    DebugLog.Write("Error binding to port " + port + ": " + E.Message);
                }

                // Shutdown all started servers
                if (CmServer != null && CmServer.IsListening) CmServer.Shutdown();
                if (SpServer != null && SpServer.IsListening) SpServer.Shutdown();
                if (MstrServer != null && MstrServer.IsRunning) MstrServer.Shutdown();
                // Cdkey server must have throwm the exception at this point, since it starts last

                // Throw excpetion to parent
                throw;
            }

            // Let the client know we are ready for connections
            isRunning = true;
            Started();
        }

        /// <summary>
        /// Shutsdown the Login Server listeners and stops accepting new connections
        /// </summary>
        public static void Shutdown()
        {
            // Shutdown Login Servers
            CmServer.Shutdown();
            SpServer.Shutdown();
            MstrServer.Shutdown();
            CDKeyServer.Shutdown();

            // Trigger the OnShutdown Event
            Stopped();

            // Update status
            isRunning = false;
        }

        /// <summary>
        /// Forces the logout of a connected client
        /// </summary>
        public static bool ForceLogout(int Pid)
        {
            return (IsRunning) ? CmServer.ForceLogout(Pid) : false;
        }

        /// <summary>
        /// Returns whether the specified player is currently connected
        /// </summary>
        public static bool IsPlayerConnected(int Pid)
        {
            return (IsRunning) ? CmServer.IsConnected(Pid) : false;
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
