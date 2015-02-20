using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Timers;

namespace BF2Statistics.Gamespy
{
    class GpcmServer
    {
        /// <summary>
        /// Our GPSP Server Listener Socket
        /// </summary>
        private static TcpListener Listener;

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
        /// Signifies whether we are shutting down or not
        /// </summary>
        private bool isShutingDown = false;

        /// <summary>
        /// An event called everytime a client connects, or disconnects from the server
        /// </summary>
        public static event EventHandler OnClientsUpdate;

        public GpcmServer()
        {
            // Attempt to bind to port 29900
            isShutingDown = false;
            Listener = new TcpListener(IPAddress.Any, 29900);
            Listener.Start();

            // Create a new thread to accept the connection
            Listener.BeginAcceptTcpClient(AcceptClient, null);

            // Enlist for events
            GpcmClient.OnSuccessfulLogin += GpcmClient_OnSuccessfulLogin;
            GpcmClient.OnDisconnect += GpcmClient_OnDisconnect;

            // Setup timer
            PollTimer = new Timer(3000);
            PollTimer.Elapsed += PollTimer_Elapsed;
            PollTimer.Start();
        }

        /// <summary>
        /// Shutsdown the GPSP server and socket
        /// </summary>
        public void Shutdown()
        {
            // Stop updating client checks
            isShutingDown = true;
            Listener.Stop();

            // Discard the poll timer
            PollTimer.Stop();
            PollTimer.Dispose();

            // Unregister events so we dont get a shit ton of calls
            GpcmClient.OnDisconnect -= GpcmClient_OnDisconnect;

            // Disconnected all connected clients
            foreach (KeyValuePair<int, GpcmClient> C in Clients)
            {
                C.Value.Disconnect(9);
                C.Value.Dispose();
            }

            // Update the database
            try
            {
                // Set everyone's online session to 0
                using (Database.GamespyDatabase Conn = new Database.GamespyDatabase())
                    Conn.Execute("UPDATE accounts SET session=0");
            }
            catch (Exception e)
            {
                Program.ErrorLog.Write("WARNING: [Gpcm.Shutdown] Failed to update client database: " + e.Message);
            }

            // Update Connected Clients in the Database
            Clients.Clear();
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
            if(Clients.ContainsKey(Pid))
            {
                Clients[Pid].Disconnect(1);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Accepts a TcpClient, and begin the login process of the client
        /// </summary>
        private async void AcceptClient(IAsyncResult ar)
        {
            bool Accepting = false;

            // End the operation and display the received data on  
            // the console.
            try
            {
                // Hurry up and get ready to accept another client
                TcpClient Client = Listener.EndAcceptTcpClient(ar);
                Listener.BeginAcceptTcpClient(AcceptClient, null);
                Accepting = true;

                // Convert the TcpClient to a GpcmClient, which will handle the client login info.
                // Process is ran in a new thread to prevent lockup here
                await Task.Run(async () =>
                {
                    // Start the login process by creating a new GpcmClient object
                    GpcmClient client = new GpcmClient(Client);
                    int i = 0;

                    // Wait for client to login, so we dont lose our client reference
                    while(client.Status == LoginStatus.Processing)
                    {
                        // Check every 1/10th second
                        await Task.Delay(100);

                        // Give the client 5 seconds to login or we quit on them
                        if(++i > 50)
                        {
                            client.Disconnect(1);
                            break;
                        }
                    }
                });
            }
            catch (ObjectDisposedException) { } // Ignore
            catch (Exception e)
            {
                Program.ErrorLog.Write("An Error occured at [GpcmServer.AcceptClient] : Generating Exception Log");
                ExceptionHandler.GenerateExceptionLog(e);
            }
            finally
            {
                // If we encountered an error before we started accepting again, and we arent shutting down
                if (!Accepting && !isShutingDown)
                    Listener.BeginAcceptTcpClient(AcceptClient, null);
            }
        }

        /// <summary>
        /// When called, each connected clients connection will be checked for any
        /// disconnections
        /// </summary>
        private void PollTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Loop through each connection and check for any drops
            foreach (KeyValuePair<int, GpcmClient> C in Clients)
                C.Value.PollConnection();
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
                if(Clients.TryRemove(client.ClientPID, out client) && !client.Disposed)
                    client.Dispose();
            }
            catch (Exception e)
            {
                Program.ErrorLog.Write("An Error occured at [GpcmServer.GpcmClient_OnDisconnect] : Generating Exception Log");
                ExceptionHandler.GenerateExceptionLog(e);
            }
            finally
            {
                OnClientsUpdate(this, EventArgs.Empty);
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
                GpcmClient client = sender as GpcmClient;

                // Check to see if the client is already logged in, if so disconnect the old user
                if (Clients.ContainsKey(client.ClientPID))
                {
                    // Kick old connection
                    GpcmClient oldC;
                    if (!Clients.TryRemove(client.ClientPID, out oldC))
                    {
                        Program.ErrorLog.Write("ERROR: [GpcmClient_OnSuccessfulLogin] Unable to remove previous client entry.");
                        client.Disconnect(1);
                        return;
                    }
                    
                    oldC.Disconnect(1);
                }

                // Add current client to the dictionary
                if (!Clients.TryAdd(client.ClientPID, client))
                {
                    // Shit
                    Program.ErrorLog.Write("ERROR: [GpcmClient_OnSuccessfulLogin] Unable to add client to HashSet.");
                    return;
                }

                // Fire event
                OnClientsUpdate(this, EventArgs.Empty);
            }
            catch(Exception E)
            {
                Program.ErrorLog.Write("ERROR: [GpcmClient_OnSuccessfulLogin] Exception was thrown, Generating exception log.");
                ExceptionHandler.GenerateExceptionLog(E);
            }
        }
    }
}
