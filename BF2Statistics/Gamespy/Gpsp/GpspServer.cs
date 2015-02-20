using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace BF2Statistics.Gamespy
{
    class GpspServer
    {
        /// <summary>
        /// Our GPSP Server Listener Socket
        /// </summary>
        private TcpListener Listener;

        /// <summary>
        /// List of connected clients
        /// </summary>
        public List<GpspClient> Clients = new List<GpspClient>();

        /// <summary>
        /// Signifies whether we are shutting down or not
        /// </summary>
        private bool isShutingDown = false;

        public GpspServer()
        {
            // Attempt to bind to port 29901
            Listener = new TcpListener(IPAddress.Any, 29901);
            Listener.Start();

            // Register for disconnect
            GpspClient.OnDisconnect += GpspClient_OnDisconnect;

            // Create a new thread to accept the connection
            Listener.BeginAcceptTcpClient(AcceptClient, null);
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
            GpspClient.OnDisconnect -= GpspClient_OnDisconnect;

            // Disconnected all connected clients
            foreach (GpspClient C in Clients)
                C.Dispose();

            // clear clients
            Clients.Clear();
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
                TcpClient Client = Listener.EndAcceptTcpClient(ar);
                Listener.BeginAcceptTcpClient(AcceptClient, null);
                Accepting = true;

                // Process last so there is no delay in accepting connections
                Clients.Add(new GpspClient(Client));
            }
            catch { }
            finally
            {
                if (!Accepting && !isShutingDown)
                    Listener.BeginAcceptTcpClient(AcceptClient, null);
            }
        }

        /// <summary>
        /// Callback for when a connection had disconnected
        /// </summary>
        /// <param name="sender">The client object whom is disconnecting</param>
        private void GpspClient_OnDisconnect(GpspClient client)
        {
            Clients.Remove(client);
        }
    }
}
