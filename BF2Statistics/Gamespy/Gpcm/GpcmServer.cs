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
        private TcpListener Listener;

        /// <summary>
        /// List of connected clients
        /// </summary>
        private List<GpcmClient> Clients = new List<GpcmClient>();

        public event EventHandler OnUpdate;

        public GpcmServer()
        {
            // Attempt to bind to port 29900
            Listener = new TcpListener(IPAddress.Any, 29900);
            Listener.Start();

            // Create a new thread to accept the connection
            Listener.BeginAcceptTcpClient(new AsyncCallback(AcceptClient), null);

            // Enlist for events
            GpcmClient.OnSuccessfulLogin += new ConnectionUpdate(Client_OnSuccessfulLogin);
            GpcmClient.OnDisconnect += new ConnectionUpdate(GpcmClient_OnDisconnect);
        }

        /// <summary>
        /// Shutsdown the GPSP server and socket
        /// </summary>
        public void Shutdown()
        {
            // Stop updating client checks
            Listener.Stop();
            GpcmClient.OnDisconnect -= new ConnectionUpdate(GpcmClient_OnDisconnect);

            // Disconnected all connected clients
            foreach (GpcmClient C in Clients)
                C.Dispose();

            // Update Connected Clients in the Database
            LoginServer.Database.Driver.Execute("UPDATE accounts SET session=0");
        }

        /// <summary>
        /// Returns the number of connected clients
        /// </summary>
        /// <returns></returns>
        public int NumClients()
        {
            return Clients.Count;
        }

        public bool LogClientOut(int Pid)
        {
            foreach (GpcmClient C in Clients)
            {
                if (C.ClientPID == Pid)
                {
                    C.LogOut();
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
            // End the operation and display the received data on  
            // the console.
            try
            {
                // Hurry up and get ready to accept another client
                TcpClient Client = Listener.EndAcceptTcpClient(ar);
                Listener.BeginAcceptTcpClient(new AsyncCallback(AcceptClient), null);

                // Convert the TcpClient to a GpcmClient, which will handle the client login info
                Clients.Add(new GpcmClient(Client));
            }
            catch { }
        }

        private void Client_OnSuccessfulLogin(object sender)
        {
            OnUpdate(this, new ClientList(Clients));
        }

        private void GpcmClient_OnDisconnect(object sender)
        {
            // Remove client, and call OnUpdate Event
            Clients.Remove((GpcmClient)sender);
            OnUpdate(this, new ClientList(Clients));
        }
    }
}
