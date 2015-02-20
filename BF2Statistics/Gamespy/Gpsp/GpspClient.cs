using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using BF2Statistics.Database;

namespace BF2Statistics.Gamespy
{
    public class GpspClient : IDisposable
    {
        /// <summary>
        /// Indicates whether this object is disposed
        /// </summary>
        public bool Disposed { get; protected set; }

        /// <summary>
        /// Connection TcpClient Stream
        /// </summary>
        private TcpClientStream Stream;

        /// <summary>
        /// The Tcp Client
        /// </summary>
        private TcpClient Client;

        /// <summary>
        /// The TcpClient's Endpoint
        /// </summary>
        private EndPoint ClientEP;

        /// <summary>
        /// Event fired when the connection is closed
        /// </summary>
        public static event GpspConnectionClosed OnDisconnect;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client"></param>
        public GpspClient(TcpClient client)
        {
            // Set disposed to false!
            this.Disposed = false;

            // Set the client variable
            this.Client = client;
            this.ClientEP = client.Client.RemoteEndPoint;
            //LoginServer.Log("[GPSP] Client Connected: {0}", ClientEP);

            // Init a new client stream class
            Stream = new TcpClientStream(client);
            Stream.OnDisconnect += Stream_OnDisconnect;
            Stream.DataReceived += Stream_DataReceived;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~GpspClient()
        {
            if (!Disposed)
                this.Dispose();
        }

        /// <summary>
        /// Dispose method to be called by the server
        /// </summary>
        public void Dispose()
        {
            // If connection is still alive, disconnect user
            if (Client.Client.IsConnected())
            {
                Stream.IsClosing = true;
                Client.Close();
            }

            // Call disconnect event
            if (OnDisconnect != null)
                OnDisconnect(this);

            // Preapare to be unloaded from memory
            this.Disposed = true;

            // Log
            //LoginServer.Log("[GPSP] Client Disconnected: {0}", ClientEP);
        }

        /// <summary>
        /// ECallback for when when the client stream is disconnected
        /// </summary>
        protected void Stream_OnDisconnect()
        {
            Dispose();
        }

        /// <summary>
        /// Callback for when a message has been recieved by the connected client
        /// </summary>
        public void Stream_DataReceived(string message)
        {
            // Parse input message
            string[] recv = message.Split('\\');
            if (recv.Length == 1)
                return;

            switch (recv[1])
            {
                case "nicks":
                    SendGPSP(recv);
                    break;
                case "check":
                    SendCheck(recv);
                    break;
            }
        }

        /// <summary>
        /// This method is requested by the client whenever an accounts existance needs validated
        /// </summary>
        /// <param name="recv"></param>
        private void SendGPSP(string[] recv)
        {
            // Try to get user data from database
            Dictionary<string, object> ClientData;
            try
            {
                using (GamespyDatabase Db = new GamespyDatabase())
                {
                    ClientData = Db.GetUser(GetParameterValue(recv, "email"), GetParameterValue(recv, "pass"));
                    if (ClientData == null)
                    {
                        Stream.Send("\\nr\\0\\ndone\\\\final\\");
                        return;
                    }
                }
            }
            catch
            {
                Dispose();
                return;
            }

            Stream.Send("\\nr\\1\\nick\\{0}\\uniquenick\\{0}\\ndone\\\\final\\", ClientData["name"]);
        }

        /// <summary>
        /// This is the primary method for fetching an accounts BF2 PID
        /// </summary>
        /// <param name="recv"></param>
        private void SendCheck(string[] recv)
        {
            try
            {
                using (GamespyDatabase Db = new GamespyDatabase())
                {
                    Stream.Send("\\cur\\0\\pid\\{0}\\final\\", Db.GetPID(GetParameterValue(recv, "nick")));
                }
            }
            catch
            {
                Dispose();
                return;
            }
        }

        /// <summary>
        /// A simple method of getting the value of the passed parameter key,
        /// from the returned array of data from the client
        /// </summary>
        /// <param name="parts">The array of data from the client</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>The value of the paramenter key</returns>
        private string GetParameterValue(string[] parts, string parameter)
        {
            bool next = false;
            foreach (string part in parts)
            {
                if (next)
                    return part;
                else if (part == parameter)
                    next = true;
            }
            return "";
        }
    }
}
