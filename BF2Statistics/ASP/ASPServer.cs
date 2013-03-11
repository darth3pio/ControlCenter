using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading;
using BF2Statistics.Database;
using BF2Statistics.Logging;
using BF2Statistics.ASP.Requests;

namespace BF2Statistics.ASP
{
    class ASPServer
    {
        /// <summary>
        /// The TCP Listner for the webserver
        /// </summary>
        private static HttpListener Listener = new HttpListener();

        /// <summary>
        /// The stats database
        /// </summary>
        public static StatsDatabase Database;

        /// <summary>
        /// ASP server log object
        /// </summary>
        private static LogWritter ServerLog = new LogWritter(Path.Combine(MainForm.Root, "Logs", "AspServer.log"));

        /// <summary>
        /// Event fired when ASP server is shutdown
        /// </summary>
        public static event ShutdownEventHandler OnShutdown;

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
        /// The status box
        /// </summary>
        private static TextBox StatusBox;

        /// <summary>
        /// Start the ASP listener, and Connects to the stats database
        /// </summary>
        public static void Start()
        {
            if (!IsRunning)
            {
                // Update Status
                StatusBox.Text = String.Format("Connecting to Stats Database ({0})\r\n", MainForm.Config.StatsDBEngine);

                // Try to connect to the database
                Database = new StatsDatabase();
                Database.Driver.ConnectionClosed += new StateChangeEventHandler(Driver_ConnectionClosed);

                // Make sure we have the ASP prefix set
                Listener = new HttpListener();
                Listener.Prefixes.Add("http://*/ASP/");

                // Start the Listener and accept new connections
                StatusBox.Text += "Binding on port 80\r\n";
                Listener.Start();
                Listener.BeginGetContext(new AsyncCallback(AcceptClient), Listener);
                StatusBox.Text += "\r\nReady for Connections!\r\n";
            }
        }

        /// <summary>
        /// Stops the ASP listener, and unbinds from the port.
        /// </summary>
        public static void Stop()
        {
            if (IsRunning)
            {
                try
                {
                    Listener.Stop();
                    Database.Driver.ConnectionClosed -= new StateChangeEventHandler(Driver_ConnectionClosed);
                    Database.Close();
                    Listener = null;

                    // Update Status
                    UpdateStatus("\r\nServer Shutdown Successfully");
                    OnShutdown();
                }
                catch(Exception E)
                {
                    ServerLog.Write(E.Message);
                    UpdateStatus("\r\nError Shutting Down Server!");
                    UpdateStatus("\r\n" + E.Message);
                }
            }
        }

        /// <summary>
        /// Invoking method to update the status window
        /// </summary>
        /// <param name="Message"></param>
        public static void UpdateStatus(string Message)
        {
            if (StatusBox.InvokeRequired)
                StatusBox.Invoke((MethodInvoker)delegate { StatusBox.Text = Message; });
            else
                StatusBox.Text += Message + "\r\n";
        }

        /// <summary>
        /// Accepts a new Connecting client in a new thread.
        /// </summary>
        /// <param name="Sync"></param>
        private static void AcceptClient(IAsyncResult Sync)
        {
            try
            {
                // Grab our connecting client
                HttpListenerContext Client = Listener.EndGetContext(Sync);

                // Make sure our stats Database is online
                if (!Database.Driver.IsConnected)
                {
                    try
                    {
                        // Try to reconnect
                        Database.CheckConnection();
                        if (!Database.Driver.IsConnected)
                        {
                            string Message = "Unable to establish database connection";
                            UpdateStatus(Message);
                            ServerLog.Write(Message);
                            throw new Exception();
                        }
                    }
                    catch(Exception)
                    {
                        // Set service is unavialable
                        Client.Response.StatusCode = 503;
                        Client.Response.Close();
                        return;
                    }
                }

                // Tell the listener to continue
                Listener.BeginGetContext(new AsyncCallback(AcceptClient), Listener);

                // Create an ASP Response
                ASPResponse Response = new ASPResponse(Client.Request, Client.Response);

                // Make sure request method is supported
                if (Client.Request.HttpMethod != "GET" && Client.Request.HttpMethod != "POST")
                {
                    Response.StatusCode = 501;
                    Response.Send();
                }

                // Create a better QueryString object
                Dictionary<string, string> QueryString = Client.Request.QueryString.Cast<string>()
                         .Select(s => new { Key = s, Value = Client.Request.QueryString[s] })
                         .ToDictionary(p => p.Key, p => p.Value);

                // Process Request
                string Doc = Client.Request.Url.AbsolutePath.Replace("/ASP/", "").ToLower();
                switch (Doc)
                {
                    case "bf2statistics.php":
                        new SnapshotPost(Client.Request, Response);
                        break;
                    case "getbackendinfo.aspx":
                        new GetBackendInfo(Response);
                        break;
                    case "getawardsinfo.aspx":
                        new GetAwardsInfo(Response, QueryString);
                        break;
                    case "getclaninfo.aspx":
                        new GetClanInfo(Response, QueryString);
                        break;
                    case "getleaderboard.aspx":
                        new GetLeaderBoard(Response, QueryString);
                        break;
                    case "getmapinfo.aspx":
                        new GetMapInfo(Response, QueryString);
                        break;
                    case "getplayerid.aspx":
                        new GetPlayerID(Response, QueryString);
                        break;
                    case "getplayerinfo.aspx":
                        new GetPlayerInfo(Response, QueryString);
                        break;
                    case "getrankinfo.aspx":
                        new GetRankInfo(Response, QueryString);
                        break;
                    case "getunlocksinfo.aspx":
                        new GetUnlocksInfo(Response, QueryString);
                        break;
                    case "ranknotification.aspx":
                        new RankNotification(Response, QueryString);
                        break;
                    case "searchforplayers.aspx":
                        new SearchForPlayers(Response, QueryString);
                        break;
                    default:
                        Response.StatusCode = 404;
                        Response.Send();
                        break;
                }
            }
            catch {}
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
        /// Sets the status window box
        /// </summary>
        /// <param name="statusBox"></param>
        public static void SetStatusBox(TextBox statusBox)
        {
            StatusBox = statusBox;
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
