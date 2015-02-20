using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using BF2Statistics.Logging;

namespace BF2Statistics.Gamespy
{
    /// <summary>
    /// This object is used as a TcpClient Network Stream wrapper,
    /// made to specifically handle Gamespy formated messages and 
    /// protocol
    /// </summary>
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
        /// If set to true, we will not contiue listening anymore
        /// </summary>
        public bool IsClosing = false;

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
            Stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, Stream);
        }

        /// <summary>
        /// Callback for BeginRead. This method handles the message parsing
        /// </summary>
        /// <param name="ar"></param>
        private void ReadCallback(IAsyncResult ar)
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
                    Program.ErrorLog.Write("ERROR: [TcpClientStream.ReadCallback] IOException Thrown during read: " + e.Message);
            }
            catch (ObjectDisposedException) { } // Fired when a the login server is shutown

            // Force disconnect (Specifically for Gpsp, whom will spam empty connections)
            if (bytesRead == 0)
            {
                if(OnDisconnect != null)
                    OnDisconnect(); // Parent is responsible for closing the connection
                return;
            }

            // Add message to buffer
            Message.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

            // If message is complete
            if (!Stream.DataAvailable && Message.ToString().EndsWith("final\\"))
            {
                // tell our parent that we recieved a message
                DataReceived(Message.ToString());
                Message = new StringBuilder();
            }

            // Begin a new Read if we are not closing the connection forcibly
            try
            {
                if (!IsClosing)
                    Stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, Stream);
            }
            catch
            {
                if (OnDisconnect != null)
                    OnDisconnect(); // Parent is responsible for closing the connection
                return;
            }
        }

        /// <summary>
        /// Writes a message to the client stream
        /// </summary>
        /// <param name="message">The complete message to be sent to the client</param>
        public void Send(string message)
        {
            this.Send(Encoding.ASCII.GetBytes(message));
        }

        /// <summary>
        /// Writes a message to the client stream
        /// </summary>
        /// <param name="message">The complete message to be sent to the client</param>
        public void Send(string message, params object[] items)
        {
            this.Send(Encoding.ASCII.GetBytes(String.Format(message, items)));
        }

        /// <summary>
        /// Writes a message to the client stream
        /// </summary>
        /// <param name="bytes">An array of bytes to send to the stream</param>
        public void Send(byte[] bytes)
        {
            Stream.Write(bytes, 0, bytes.Length);
        }
    }
}
