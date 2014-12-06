using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BF2Statistics.Gamespy
{
    class GpcmServer
    {
        /// <summary>
        /// Our GPSP Server Listener Socket
        /// </summary>
        private static TcpListener Listener;

        /// <summary>
        /// List of connected clients
        /// </summary>
        private static List<GpcmClient> Clients = new List<GpcmClient>();

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
            Listener.BeginAcceptTcpClient(new AsyncCallback(AcceptClient), null);

            // Enlist for events
            GpcmClient.OnSuccessfulLogin += new ConnectionUpdate(GpcmClient_OnSuccessfulLogin);
            GpcmClient.OnDisconnect += new GpcmConnectionClosed(GpcmClient_OnDisconnect);
        }

        /// <summary>
        /// Shutsdown the GPSP server and socket
        /// </summary>
        public void Shutdown()
        {
            // Stop updating client checks
            isShutingDown = true;
            Listener.Stop();

            // Unregister events so we dont get a shit ton of calls
            GpcmClient.OnDisconnect -= new GpcmConnectionClosed(GpcmClient_OnDisconnect);

            // Disconnected all connected clients
            foreach (GpcmClient C in Clients)
            {
                C.Disconnect(9);
                C.Dispose();
            }

            // Update Connected Clients in the Database
            Clients.Clear();
        }

        /// <summary>
        /// Returns the number of connected clients
        /// </summary>
        /// <returns></returns>
        public int NumClients()
        {
            return Clients.Count;
        }

        /// <summary>
        /// Forces the logout of a connected client
        /// </summary>
        /// <param name="Pid">The account ID</param>
        /// <returns></returns>
        public bool ForceLogout(int Pid)
        {
            foreach (GpcmClient C in Clients)
            {
                if (C.ClientPID == Pid)
                {
                    C.Disconnect(1);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Accepts a TcpClient
        /// </summary>
        /// <param name="ar"></param>
        private void AcceptClient(IAsyncResult ar)
        {
            bool Accepting = false;

            // End the operation and display the received data on  
            // the console.
            try
            {
                // Hurry up and get ready to accept another client
                TcpClient Client = Listener.EndAcceptTcpClient(ar);
                Listener.BeginAcceptTcpClient(new AsyncCallback(AcceptClient), null);
                Accepting = true;

                // Convert the TcpClient to a GpcmClient, which will handle the client login info
                // Process last so there is no delay in accepting connections
                Clients.Add(new GpcmClient(Client));
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
                    Listener.BeginAcceptTcpClient(new AsyncCallback(AcceptClient), null);
            }
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
                client.Dispose();
                Clients.Remove(client);
            }
            catch (Exception e)
            {
                Program.ErrorLog.Write("An Error occured at [GpcmServer.GpcmClient_OnDisconnect] : Generating Exception Log");
                ExceptionHandler.GenerateExceptionLog(e);
            }
            finally
            {
                OnClientsUpdate(this, new ClientList(Clients));
            }
        }

        /// <summary>
        /// Callback for a successful login
        /// </summary>
        /// <param name="sender"></param>
        private void GpcmClient_OnSuccessfulLogin(object sender)
        {
            OnClientsUpdate(this, new ClientList(Clients));
        }
    }
}
