using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using BF2Statistics.Logging;

namespace BF2Statistics.Gamespy
{
    class TcpClientStream
    {
        /// <summary>
        /// The current clients stream
        /// </summary>
        private TcpClient Client;

        /// <summary>
        /// Clients NetworkStream
        /// </summary>
        private NetworkStream Stream;

        /// <summary>
        /// Write all data sent/recieved to the stream log?
        /// </summary>
        private bool Debugging;

        /// <summary>
        /// StreamLog Object
        /// </summary>
        private static LogWritter StreamLog = new LogWritter(Path.Combine(MainForm.Root, "Logs", "Stream.log"));

        /// <summary>
        /// Our message buffer
        /// </summary>
        private byte[] buffer = new byte[2048];

        /// <summary>
        /// Our remote message from the buffer, converted to a string
        /// </summary>
        private StringBuilder Message = new StringBuilder();

        /// <summary>
        /// Event fired when a completed message has been recieved
        /// </summary>
        public event DataRecivedEvent DataReceived;

        /// <summary>
        /// Event fire when the remote connection is closed
        /// </summary>
        public event ConnectionClosed OnDisconnect;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client"></param>
        public TcpClientStream(TcpClient client)
        {
            this.Client = client;
            this.Stream = client.GetStream();
            this.Debugging = MainForm.Config.DebugStream;
            Stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(DoRead), Stream);
        }

        /// <summary>
        /// Callback for BeginRead. This method handles the message parsing
        /// </summary>
        /// <param name="ar"></param>
        private void DoRead(IAsyncResult ar)
        {
            // End the Async Read
            int bytesRead = 0;
            try
            {
                bytesRead = Stream.EndRead(ar);
            }
            catch (IOException e)
            {
                // If we got an IOException, client connection is lost
                if (Client.Client.IsConnected())
                    Log("ERROR: IOException Thrown during read: " + e.Message);
            }
            catch (ObjectDisposedException) { } // Fired when a the login server is shutown

            // Force disconnet (Specifically for Gpsp, whom will spam null strings)
            if (bytesRead == 0)
            {
                OnDisconnect();
                return;
            }

            // Add message to buffer
            Message.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

            // If we have no more data, then the message is complete
            if (!Stream.DataAvailable)
            {
                // Debugging
                if (Debugging)
                    Log("Port {0} Recieves: {1}", ((IPEndPoint)Client.Client.LocalEndPoint).Port, Message.ToString());

                // tell our parent that we recieved a message
                DataReceived(Message.ToString());
                Message = new StringBuilder();
            }

            // Begin a new Read
            Stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(DoRead), Stream);
        }

        /// <summary>
        /// Writes a message to the client stream
        /// </summary>
        /// <param name="message">The complete message to be sent to the client</param>
        public void Send(string message)
        {
            if (Debugging)
                Log("Port {0} Sends: {1}", ((IPEndPoint)Client.Client.LocalEndPoint).Port, message);

            this.Send(Encoding.ASCII.GetBytes(message));
        }

        /// <summary>
        /// Writes a message to the client stream
        /// </summary>
        /// <param name="message">The complete message to be sent to the client</param>
        public void Send(string message, params object[] items)
        {
            message = String.Format(message, items);
            if (Debugging)
                Log("Port {0} Sends: {1}", ((IPEndPoint)Client.Client.LocalEndPoint).Port, message);

            this.Send(Encoding.ASCII.GetBytes(message));
        }

        /// <summary>
        /// Writes a message to the client stream
        /// </summary>
        /// <param name="bytes">An array of bytes to send to the stream</param>
        public void Send(byte[] bytes)
        {
            Stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Writes a message to the stream log
        /// </summary>
        /// <param name="message"></param>
        private static void Log(string message)
        {
            StreamLog.Write(message);
        }

        /// <summary>
        /// Writes a message to the stream log
        /// </summary>
        /// <param name="message"></param>
        private static void Log(string message, params object[] items)
        {
            StreamLog.Write(String.Format(message, items));
        }
    }
}
