using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;
using BF2Statistics.Database;
using BF2Statistics.Logging;
using BF2Statistics.ASP.Requests;

namespace BF2Statistics.ASP
{
    /// <summary>
    /// The ASP Server is used to emulate the official Gamespy BF2 Stat
    /// Server HTTP Requests, and provide players with the ability to run 
    /// thier own BF2 Ranking system on thier personal PC's.
    /// </summary>
    class ASPServer
    {
        /// <summary>
        /// The HTTPListner for the webserver
        /// </summary>
        private static HttpListener Listener = new HttpListener();

        /// <summary>
        /// The stats database object
        /// </summary>
        public static StatsDatabase Database;

        /// <summary>
        /// ASP server log writter
        /// </summary>
        private static LogWritter ServerLog;

        /// <summary>
        /// Our Access log
        /// </summary>
        public static LogWritter AccessLog { get; protected set; }

        /// <summary>
        /// Occurs when the server is started.
        /// </summary>
        public static event EventHandler Started;

        /// <summary>
        /// Occurs when the server is about to stop.
        /// </summary>
        public static event EventHandler Stopping;

        /// <summary>
        /// Occurs when the server is stopped.
        /// </summary>
        public static event EventHandler Stopped;

        /// <summary>
        /// Event fired when ASP server recieves a connection
        /// </summary>
        public static event AspRequest RequestRecieved;

        /// <summary>
        /// A List of local IP addresses for this machine
        /// </summary>
        public static readonly List<IPAddress> LocalIPs;

        /// <summary>
        /// Number of session web requests
        /// </summary>
        public static int SessionRequests { get; protected set; }

        /// <summary>
        /// Static constructor
        /// </summary>
        static ASPServer()
        {
            ServerLog = new LogWritter(Path.Combine(MainForm.Root, "Logs", "AspServer.log"), 3000);
            AccessLog = new LogWritter(Path.Combine(MainForm.Root, "Logs", "AspAccess.log"), 3000);
            LocalIPs = Dns.GetHostAddresses(Dns.GetHostName()).ToList();
            SessionRequests = 0;
        }

        /// <summary>
        /// Is the webserver running?
        /// </summary>
        public static bool IsRunning
        {
            get 
            {
                try {
                    return (Listener == null) ? false : Listener.IsListening;
                }
                catch (ObjectDisposedException) {
                    return false;
                }
            }
        }

        /// <summary>
        /// Start the ASP listener, and Connects to the stats database
        /// </summary>
        public static void Start()
        {
            if (!IsRunning)
            {
                // Try to connect to the database
                if (Database == null)
                    Database = new StatsDatabase();
                else
                    Database.CheckConnection();
                Database.Driver.ConnectionClosed += new StateChangeEventHandler(Driver_ConnectionClosed);

                // Make sure we have the ASP prefix set
                Listener = new HttpListener();
                Listener.Prefixes.Add("http://*/ASP/");

                // Start the Listener and accept new connections
                Listener.Start();
                Listener.BeginGetContext(new AsyncCallback(DoAcceptClientCallback), Listener);
                
                // Fire Startup Event
                Started(null, null);
            }
        }

        /// <summary>
        /// Stops the ASP listener, and unbinds from the port.
        /// </summary>
        public static void Stop()
        {
            if (IsRunning)
            {
                // Call Stopping Event to disconnect clients
                if (Stopping != null)
                    Stopping(null, null);

                try
                {
                    Listener.Stop();
                    Database.Driver.ConnectionClosed -= new StateChangeEventHandler(Driver_ConnectionClosed);
                    Database.Close();
                    Listener = null;
                    Stopped(null, null);
                }
                catch(Exception E)
                {
                    ServerLog.Write(E.Message);
                }
            }
        }

        /// <summary>
        /// Accepts the connection
        /// </summary>
        private static void DoAcceptClientCallback(IAsyncResult Sync)
        {
            try
            {
                HttpListenerContext Client = Listener.EndGetContext(Sync);
                ThreadPool.QueueUserWorkItem(HandleRequest, Client);
            }
            catch (HttpListenerException E)
            {
                // Thread abort, or application abort request
                if (E.ErrorCode == 995)
                    return;

                ServerLog.Write("ERROR: [DoAcceptClientCallback] \r\n\t - {0}\r\n\t - ErrorCode: {1}", E.Message, E.ErrorCode);
            }

            Listener.BeginGetContext(new AsyncCallback(DoAcceptClientCallback), Listener);
        }

        /// <summary>
        /// Handles the Http Connecting client in a new thread
        /// </summary>
        private static void HandleRequest(object Sync)
        {
            // Finish accepting the client
            HttpClient Client = new HttpClient(Sync as HttpListenerContext);

            // Update client count, and fire connection event
            SessionRequests++;
            RequestRecieved();

            // Make sure our stats Database is online
            try
            {
                // If we arent suppossed to be running, show a 503
                if (!IsRunning)
                    throw new Exception("Server is not running");

                // If database is offline, Try to re-connect
                if (!Database.Driver.IsConnected)
                {
                    try { Database.CheckConnection(); }
                    catch { }
                    if (!Database.Driver.IsConnected)
                    {
                        string Message = "ERROR: Unable to establish database connection";
                        ServerLog.Write(Message);
                        throw new Exception(Message);
                    }
                }
            }
            catch
            {
                // Set service is unavialable
                Client.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                Client.Response.Send();
                return;
            }

            // Make sure request method is supported
            if (Client.Request.HttpMethod != "GET" 
                && Client.Request.HttpMethod != "POST"
                && Client.Request.HttpMethod != "HEAD")
            {
                Client.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                Client.Response.Send();
                return;
            }

            // Process Request
            try
            {
                // Get our requested document
                string Document = Client.Request.Url.AbsolutePath.ToLower();
                switch (Document.Replace("/asp/", ""))
                {
                    case "bf2statistics.php":
                        new SnapshotPost(Client);
                        break;
                    case "createplayer.aspx":
                        new CreatePlayer(Client);
                        break;
                    case "getbackendinfo.aspx":
                        new GetBackendInfo(Client);
                        break;
                    case "getawardsinfo.aspx":
                        new GetAwardsInfo(Client);
                        break;
                    case "getclaninfo.aspx":
                        new GetClanInfo(Client);
                        break;
                    case "getleaderboard.aspx":
                        new GetLeaderBoard(Client);
                        break;
                    case "getmapinfo.aspx":
                        new GetMapInfo(Client);
                        break;
                    case "getplayerid.aspx":
                        new GetPlayerID(Client);
                        break;
                    case "getplayerinfo.aspx":
                        new GetPlayerInfo(Client);
                        break;
                    case "getrankinfo.aspx":
                        new GetRankInfo(Client);
                        break;
                    case "getunlocksinfo.aspx":
                        new GetUnlocksInfo(Client);
                        break;
                    case "ranknotification.aspx":
                        new RankNotification(Client);
                        break;
                    case "searchforplayers.aspx":
                        new SearchForPlayers(Client);
                        break;
                    case "selectunlock.aspx":
                        new SelectUnlock(Client);
                        break;
                    default:
                        Client.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        Client.Response.Send();
                        break;
                }
            }
            catch (Exception E)
            {
                ServerLog.Write("ERROR: " + E.Message);
                if (!Client.ResponseSent)
                {
                    // Internal service error
                    Client.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    Client.Response.Send();
                }
            }
            finally
            {
                // Make sure a response is sent to prevent client hang
                if (!Client.ResponseSent)
                    Client.Response.Send();
            }
        }

        private static void Driver_ConnectionClosed(object sender, StateChangeEventArgs e)
        {
            // Try to reconnect
            try {
                Database.CheckConnection();
            }
            catch {
                Stop();
            }
        }

        /// <summary>
        /// Writes a message to the stream log
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message)
        {
            ServerLog.Write(message);
        }

        /// <summary>
        /// Writes a message to the stream log
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message, params object[] items)
        {
            ServerLog.Write(message, items);
        }
    }
}
