using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BF2Statistics.Net
{
    /// <summary>
    /// This object is used as a Network Stream wrapper for Gamespy TCP protocol,
    /// </summary>
    public class GamespyTcpStream : IDisposable
    {
        /// <summary>
        /// Our message recieved from the client connection. If the message is too long,
        /// it will be sent over multiple receive operations, so we store the message parts
        /// here until we recieve the \final\ delimiter.
        /// </summary>
        protected StringBuilder RecvMessage = new StringBuilder(256);

        /// <summary>
        /// Our message to send to the client. If the message is too long, it will be sent
        /// over multiple write operations, so we store the message here until its all sent
        /// </summary>
        protected List<byte> SendMessage = new List<byte>(256);

        /// <summary>
        /// The current send offset when sending asynchronously
        /// </summary>
        protected int SendBytesOffset = 0;

        /// <summary>
        /// Indicates whether we are sending a message currently
        /// </summary>
        protected bool IsSending = false;

        /// <summary>
        /// Our connected socket
        /// </summary>
        public Socket Connection;

        /// <summary>
        /// Our AsycnEventArgs object for reading data
        /// </summary>
        public SocketAsyncEventArgs ReadEventArgs { get; protected set; }

        /// <summary>
        /// Our AsyncEventArgs object for sending data
        /// </summary>
        public SocketAsyncEventArgs WriteEventArgs { get; protected set; }

        /// <summary>
        /// Gets the remote endpoint
        /// </summary>
        public EndPoint RemoteEndPoint
        {
            get { return ReadEventArgs.AcceptSocket.RemoteEndPoint; }
        }

        /// <summary>
        /// Indicates whether the underlying socket connection has been closed,
        /// and cleaned up properly
        /// </summary>
        public bool SocketClosed { get; protected set; }

        /// <summary>
        /// Indicates whether the OnDisconnect event has been called
        /// </summary>
        protected bool DisconnectEventCalled = false;

        /// <summary>
        /// Indicates whether the EventArgs objects were disposed by request
        /// <remarks>This should NEVER be true unless we are shutting down the server!!!</remarks>
        /// </summary>
        public bool DisposedEventArgs { get; protected set; }

        /// <summary>
        /// Event fired when a completed message has been received
        /// </summary>
        public event DataRecivedEvent DataReceived;

        /// <summary>
        /// Event fire when the remote connection is closed
        /// </summary>
        public event ConnectionClosed OnDisconnect; 

        /// <summary>
        /// Creates a new instance of GamespyTcpStream
        /// </summary>
        /// <param name="ReadAsyncEventArgs"></param>
        public GamespyTcpStream(SocketAsyncEventArgs ReadAsyncEventArgs, SocketAsyncEventArgs WriteAsyncEventArgs)
        {
            // Store our connection
            Connection = ReadAsyncEventArgs.AcceptSocket;

            // Create our IO event callbacks
            ReadAsyncEventArgs.Completed += IOComplete;
            WriteAsyncEventArgs.Completed += IOComplete;

            // Set our internal variables
            ReadEventArgs = ReadAsyncEventArgs;
            WriteEventArgs = WriteAsyncEventArgs;
            SocketClosed = false;
            DisposedEventArgs = false;
        }

        ~GamespyTcpStream()
        {
            if (!SocketClosed)
                Close();
        }

        public void Dispose()
        {
            if(!SocketClosed)
                Close();
        }

        /// <summary>
        /// Begins the process of receiving a message from the client.
        /// This method must manually be called to Begin receiving data
        /// </summary>
        public void BeginReceive()
        {
            try
            {
                if (Connection != null)
                {
                    // Reset Buffer offset back to the original allocated offset
                    BufferDataToken token = ReadEventArgs.UserToken as BufferDataToken;
                    ReadEventArgs.SetBuffer(token.BufferOffset, token.BufferBlockSize);

                    // Begin Receiving
                    if(!Connection.ReceiveAsync(ReadEventArgs))
                        ProcessReceive();
                }
            }
            catch(ObjectDisposedException e)
            {
                if(!DisconnectEventCalled)
                {
                    // Uh-Oh. idk how we got here
                    Program.ErrorLog.Write("WARNING: [GamespyStream.BeginReceive] ObjectDisposedException was thrown: " + e.Message);

                    // Disconnect user
                    DisconnectEventCalled = true;
                    if(OnDisconnect != null)
                        OnDisconnect();
                }
            }
            catch(SocketException e)
            {
                HandleSocketError(e.SocketErrorCode);
            }
        }

        /// <summary>
        /// Closes the underlying socket
        /// </summary>
        /// <param name="DisposeEventArgs">
        /// If true, the EventArg objects will be disposed instead of being re-added to 
        /// the IO pool. This should NEVER be set to true unless we are shutting down the server!
        /// </param>
        public void Close(bool DisposeEventArgs = false)
        {
            // If we've done this before
            if (SocketClosed) return;

            // Set that the socket is being closed properly
            SocketClosed = true;

            // Do a shutdown before you close the socket
            try
            {
                Connection.Shutdown(SocketShutdown.Both); 
            }
            catch(Exception) { }
            finally
            {
                // Unregister for vents
                ReadEventArgs.Completed -= IOComplete;
                WriteEventArgs.Completed -= IOComplete;

                // Close the connection
                Connection.Close();
                Connection = null;
            }

            // If we need to dispose out EventArgs
            if(DisposeEventArgs)
            {
                ReadEventArgs.Dispose();
                WriteEventArgs.Dispose();
                DisposedEventArgs = true;
            }

            // Call Disconnect Event
            if (!DisconnectEventCalled && OnDisconnect != null)
            {
                DisconnectEventCalled = true;
                OnDisconnect();
            }
        }

        /// <summary>
        /// Once data has been recived from the client, this method is called
        /// to process the data. Once a message has been completed, the OnDataReceived
        /// event will be called
        /// </summary>
        private void ProcessReceive()
        {
            // If we do not get a success code here, we have a bad socket
            if (ReadEventArgs.SocketError != SocketError.Success)
            {
                HandleSocketError(ReadEventArgs.SocketError);
                return;
            }

            // Force disconnect (Specifically for Gpsp, whom will spam empty connections)
            if (ReadEventArgs.BytesTransferred == 0)
            {
                if (!DisconnectEventCalled && OnDisconnect != null)
                {
                    DisconnectEventCalled = true;
                    OnDisconnect(); // Parent is responsible for closing the connection
                }
                return;
            }
            else
            {
                // Fetch our message as a string from the Buffer
                BufferDataToken token = ReadEventArgs.UserToken as BufferDataToken;
                RecvMessage.Append(
                    Encoding.UTF8.GetString(
                        ReadEventArgs.Buffer, 
                        token.BufferOffset, 
                        ReadEventArgs.BytesTransferred
                    )
                );

                // Process Message
                string received = RecvMessage.ToString();
                if (received.EndsWith("final\\") || received.EndsWith("\x00\x00\x00\x00"))
                {
                    // tell our parent that we recieved a message
                    RecvMessage.Clear(); // Clear old junk
                    DataReceived(received);
                }
            }

            // Begin receiving again
            BeginReceive();
        }

        /// <summary>
        /// Writes a message to the client stream asynchronously
        /// </summary>
        /// <param name="message">The complete message to be sent to the client</param>
        public void SendAsync(string message)
        {
            // Create a new message
            lock (SendMessage)
                SendMessage.AddRange(Encoding.UTF8.GetBytes(message));

            // Send if we aren't already in the middle of an Async send
            if (!IsSending) ProcessSend();
        }

        /// <summary>
        /// Writes a message to the client stream asynchronously
        /// </summary>
        /// <param name="message">The complete message to be sent to the client</param>
        /// <param name="items"></param>
        public void SendAsync(string message, params object[] items)
        {
            SendAsync(String.Format(message, items));
        }

        /// <summary>
        /// Writes a message to the client stream asynchronously
        /// </summary>
        /// <param name="message">The complete message to be sent to the client</param>
        public void SendAsync(byte[] message)
        {
            // Create a new message
            lock (SendMessage)
                SendMessage.AddRange(message);

            // Send if we aren't already in the middle of an Async send
            if (!IsSending) ProcessSend();
        }

        /// <summary>
        /// Sends a message Asynchronously to the client connection
        /// </summary>
        private void ProcessSend()
        {
            // Prevent race conditions by locking the Send Message
            lock (SendMessage)
            {
                // If we have finished sending our message, then reset
                if (SendMessage.Count <= SendBytesOffset)
                {
                    SendBytesOffset = 0;
                    SendMessage.Clear();
                    IsSending = false;
                    return;
                }

                // Signify that we are sending data
                IsSending = true;
            }

            // Get our data token
            BufferDataToken Token = WriteEventArgs.UserToken as BufferDataToken;
            int NumBytesToSend = SendMessage.Count - SendBytesOffset;

            // Make sure we arent sending more then we have space for
            if (NumBytesToSend > Token.BufferBlockSize)
                NumBytesToSend = Token.BufferBlockSize;

            // Convert our message to bytes, and copy to the buffer
            SendMessage.CopyTo(SendBytesOffset, WriteEventArgs.Buffer, Token.BufferOffset, NumBytesToSend);
            WriteEventArgs.SetBuffer(Token.BufferOffset, NumBytesToSend);

            // Send the message to the client
            if (!Connection.SendAsync(WriteEventArgs))
            {
                // Remember, if we are here, data was sent Synchronously... IOComplete event is not called! 
                // Manually set the BytesSent
                SendBytesOffset += WriteEventArgs.BytesTransferred;
                ProcessSend();
            }
        }

        /// <summary>
        /// If there was a socket error, it can be handled propery here
        /// </summary>
        /// <param name="socketError"></param>
        private void HandleSocketError(SocketError socketError)
        {
            // Error Handle Here
            switch (socketError)
            {
                case SocketError.TooManyOpenSockets:
                case SocketError.Shutdown:
                case SocketError.Disconnecting:
                case SocketError.ConnectionReset:
                case SocketError.NotConnected:
                case SocketError.TimedOut:
                    if (!SocketClosed)
                        Close();
                    break;
            }
        }

        /// <summary>
        /// Event called when data has been recived from the client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IOComplete(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive();
                    break;
                case SocketAsyncOperation.Send:
                    SendBytesOffset += e.BytesTransferred;
                    ProcessSend();
                    break;
            }
        }
    }
}
