using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using BF2Statistics.Logging;

namespace BF2Statistics
{
    class ClientStream
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
        private static LogWritter StreamLog = new LogWritter(Path.Combine(MainForm.Root, "Logs", "Stream.log"), 3000);

        public ClientStream(TcpClient client)
        {
            this.Client = client;
            this.Stream = client.GetStream();
            this.Debugging = MainForm.Config.DebugStream;
        }

        /// <summary>
        /// Returns a bool based off on wether there is data available to be read
        /// </summary>
        /// <returns></returns>
        public bool HasData()
        {
            return Stream.DataAvailable;
        }

        /// <summary>
        /// Reads from the client stream. Will rest until data is recieved
        /// </summary>
        /// <returns>The completed data from the client</returns>
        public string Read()
        {
            int bytesRead = 0;
            int bufferSize = Client.ReceiveBufferSize;
            byte[] buffer = new byte[bufferSize];
            string message = "";

            do
            {
                bytesRead += Stream.Read(buffer, 0, bufferSize);
                int Counter = 0;

                foreach (byte b in buffer)
                {
                    if (b == 0x00)
                        break;

                    ++Counter;
                }

                //Trim off the null bytes.
                Array.Resize(ref buffer, Counter);
                message += Encoding.UTF8.GetString(buffer);
                if (Debugging)
                    Log("Port {0} Recieves: {1}", ((IPEndPoint)Client.Client.LocalEndPoint).Port, message);

            } while (Stream.DataAvailable);

            if (bytesRead == 0)
                return "";

            return message.ToString();
        }

        /// <summary>
        /// Writes a message to the client stream
        /// </summary>
        /// <param name="message">The complete message to be sent to the client</param>
        public void Write(string message)
        {
            if (Debugging)
                Log("Port {0} Sends: {1}", ((IPEndPoint)Client.Client.LocalEndPoint).Port, message);

            this.Write(Encoding.ASCII.GetBytes(message));
        }

        /// <summary>
        /// Writes a message to the client stream
        /// </summary>
        /// <param name="message">The complete message to be sent to the client</param>
        public void Write(string message, params object[] items)
        {
            message = String.Format(message, items);
            if (Debugging)
                Log("Port {0} Sends: {1}", ((IPEndPoint)Client.Client.LocalEndPoint).Port, message);

            this.Write(Encoding.ASCII.GetBytes(message));
        }

        /// <summary>
        /// Writes a message to the client stream
        /// </summary>
        /// <param name="bytes">An array of bytes to send to the stream</param>
        public void Write(byte[] bytes)
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
