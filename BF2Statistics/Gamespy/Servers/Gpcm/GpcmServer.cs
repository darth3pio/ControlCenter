using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using BF2Statistics.Database;
using BF2Statistics.Gamespy.Net;
using BF2Statistics.Logging;
using BF2Statistics.Net;

namespace BF2Statistics.Gamespy
{
    /// <summary>
    /// This server emulates the Gamespy Client Manager Server on port 29900.
    /// This class is responsible for managing the players logged into Battlefield 2.
    /// </summary>
    public class GpcmServer : GamespyTcpSocket
    {
        /// <summary>
        /// Max number of concurrent open and active connections.
        /// </summary>
        /// <remarks>
        ///   Connections to the Gpcm server are active for the entire duration
        ///   that a client is logged in. Max conenctions is essentially the max number 
        ///   of players that can be logged in at the same time.
        ///   
        ///   I decided to keep this value smaller, because this application isn't built
        ///   for windows servers, and I personally dont want people using this control
        ///   center as a mass service for everyone
        /// </remarks>
        public const int MaxConnections = 64;

        /// <summary>
        /// List of sucessfully logged in clients (Pid => Client Obj)
        /// </summary>
        private static ConcurrentDictionary<int, GpcmClient> Clients = new ConcurrentDictionary<int, GpcmClient>();

        /// <summary>
        /// Returns a list of all the connected clients
        /// </summary>
        public GpcmClient[] ConnectedClients
        {
            get { return Clients.Values.ToArray(); }
        }

        /// <summary>
        /// Returns the number of connected clients
        /// </summary>
        /// <returns></returns>
        public int NumClients
        {
            get { return Clients.Count; }
        }

        /// <summary>
        /// A timer that is used to Poll all connections, and removes dropped connections
        /// </summary>
        public static Timer PollTimer { get; protected set; }

        /// <summary>
        /// The Login Server Log Writter
        /// </summary>
        private static LogWriter Logger = new LogWriter(Path.Combine(Program.RootPath, "Logs", "LoginServer.log"), true);

        /// <summary>
        /// An event called everytime a client connects, or disconnects from the server
        /// </summary>
        public static event EventHandler OnClientsUpdate;

        public GpcmServer() : base(29900, MaxConnections)
        {
            // Register for events
            GpcmClient.OnSuccessfulLogin += GpcmClient_OnSuccessfulLogin;
            GpcmClient.OnDisconnect += GpcmClient_OnDisconnect;

            // Setup timer. Every 15 seconds should be sufficient
            PollTimer = new Timer(15000);
            PollTimer.Elapsed += (s, e) => Parallel.ForEach(Clients.Values, client => client.SendKeepAlive());
            PollTimer.Start();

            // Begin accepting connections
            base.StartAcceptAsync();
        }

        /// <summary>
        /// Shutsdown the ClientManager server and socket
        /// </summary>
        public void Shutdown()
        {
            // Discard the poll timer
            PollTimer.Stop();
            PollTimer.Dispose();

            // Stop accepting new connections
            base.IgnoreNewConnections = true;

            // Unregister events so we dont get a shit ton of calls
            GpcmClient.OnSuccessfulLogin -= GpcmClient_OnSuccessfulLogin;
            GpcmClient.OnDisconnect -= GpcmClient_OnDisconnect;

            // Disconnected all connected clients
            Parallel.ForEach(Clients.Values, client => client.Disconnect(9));

            // Shutdown the listener socket
            base.ShutdownSocket();

            // Update the database
            try
            {
                // Set everyone's online session to 0
                using (GamespyDatabase Conn = new GamespyDatabase())
                    Conn.Execute("UPDATE accounts SET session=0 WHERE session != 0");
            }
            catch (Exception e)
            {
                Program.ErrorLog.Write("WARNING: [Gpcm.Shutdown] Failed to update client database: " + e.Message);
            }

            // Update Connected Clients in the Database
            Clients.Clear();

            // Tell the base to dispose all free objects
            base.Dispose();
        }

        /// <summary>
        /// When a new connection is established, we the parent class are responsible
        /// for handling the processing
        /// </summary>
        /// <param name="Stream">A GamespyTcpStream object that wraps the I/O AsyncEventArgs and socket</param>
        protected override void ProcessAccept(GamespyTcpStream Stream)
        {
            try
            {
                // Create a new GpcmClient, passing the IO object for the TcpClientStream
                GpcmClient client = new GpcmClient(Stream);

                // Begin the asynchronous login process
                client.SendServerChallenge();
            }
            catch (Exception e)
            {
                Program.ErrorLog.Write("WARNING: An Error occured at [Gpcm.ProcessAccept] : Generating Exception Log");
                ExceptionHandler.GenerateExceptionLog(e);
                base.Release(Stream);
            }
        }

        /// <summary>
        /// Returns whether the specified player is currently connected
        /// </summary>
        /// <param name="Pid">The players ID</param>
        /// <returns></returns>
        public bool IsConnected(int Pid)
        {
            return Clients.ContainsKey(Pid);
        }

        /// <summary>
        /// Forces the logout of a connected client
        /// </summary>
        /// <param name="Pid">The players ID</param>
        /// <returns>Returns whether the client was connected, and disconnect was called</returns>
        public bool ForceLogout(int Pid)
        {
            GpcmClient client;
            if (Clients.TryGetValue(Pid, out client))
            {
                client.Disconnect(1);
                return true;
            }
            return false;
        }

        /// <summary>
        /// This method is used to store a message in the LoginServer.log file
        /// </summary>
        /// <param name="message">The message to be written to the log file</param>
        public static void Log(string message)
        {
            Logger.Write(message);
        }

        /// <summary>
        /// This method is used to store a message in the LoginServer.log file
        /// </summary>
        /// <param name="message">The message to be written to the log file</param>
        public static void Log(string message, params object[] items)
        {
            Logger.Write(String.Format(message, items));
        }

        /// <summary>
        /// Callback for when a connection had disconnected
        /// </summary>
        /// <param name="client">The client object whom is disconnecting</param>
        private void GpcmClient_OnDisconnect(GpcmClient client)
        {
            // Remove client, and call OnUpdate Event
            try
            {
                // Release this stream's AsyncEventArgs to the object pool
                base.Release(client.Stream);

                // Remove client from online list
                if (Clients.TryRemove(client.PlayerId, out client) && !client.Disposed)
                    client.Dispose();

                // Call Event
                OnClientsUpdate(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                Program.ErrorLog.Write("An Error occured at [GpcmServer.GpcmClient_OnDisconnect] : Generating Exception Log");
                ExceptionHandler.GenerateExceptionLog(e);
            }
        }

        /// <summary>
        /// Callback for a successful login
        /// </summary>
        /// <param name="sender">The GpcmClient that is logged in</param>
        private void GpcmClient_OnSuccessfulLogin(object sender)
        {
            // Wrap this in a try/catch
            try
            {
                // Check to see if the client is already logged in, if so disconnect the old user
                GpcmClient client = sender as GpcmClient;
                if (Clients.ContainsKey(client.PlayerId))
                {
                    // Kick old connection
                    GpcmClient oldC;
                    if (!Clients.TryRemove(client.PlayerId, out oldC))
                    {
                        Program.ErrorLog.Write("ERROR: [GpcmClient_OnSuccessfulLogin] Unable to remove previous client entry.");
                        client.Disconnect(1);
                        return;
                    }

                    oldC.Disconnect(1);
                }

                // Add current client to the dictionary
                if (!Clients.TryAdd(client.PlayerId, client))
                {
                    Program.ErrorLog.Write("ERROR: [GpcmClient_OnSuccessfulLogin] Unable to add client to HashSet.");
                    return;
                }

                // Fire event
                OnClientsUpdate(this, EventArgs.Empty);
            }
            catch (Exception E)
            {
                Program.ErrorLog.Write("ERROR: [GpcmClient_OnSuccessfulLogin] Exception was thrown, Generating exception log.");
                ExceptionHandler.GenerateExceptionLog(E);
            }
        }
    }
}
