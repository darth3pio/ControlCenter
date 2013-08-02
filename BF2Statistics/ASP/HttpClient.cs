using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;
using System.Threading;
using System.IO;

namespace BF2Statistics.ASP
{
    class HttpClient : IDisposable
    {
        /// <summary>
        /// Returns the IP Endpoint for the connected client
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; protected set; }

        /// <summary>
        /// Gets the HttpRequest object for this client
        /// </summary>
        public ASPRequest Request { get; protected set; }

        /// <summary>
        /// Gets the response object for this client
        /// </summary>
        public ASPResponse Response { get; protected set; }

        /// <summary>
        /// Indicates whether the response object has written to the output stream
        /// </summary>
        public bool ResponseSent { get; protected set; }

        /// <summary>
        /// Creates a new instance of HttpClinet
        /// </summary>
        /// <param name="Client">The TcpClient object for this connection</param>
        /// <param name="Reset">
        /// The ManualResetEvent object, used to stop the parent thread until the 
        /// request is fully recieved, and a response an be created
        /// </param>
        public HttpClient(HttpListenerContext Client)
        {
            // Fill Request / Response
            Response = new ASPResponse(Client.Response, this);
            Request = new ASPRequest(Client.Request, this);
            RemoteEndPoint = Client.Request.RemoteEndPoint as IPEndPoint;

            // Register for events
            Response.SentResponse += new EventHandler(Response_SentResponse);

            // Add BF2Statistics Header
            Client.Response.AddHeader("X-Powered-By", "BF2Statistics Control Center v" + Program.Version);
        }

        public void Dispose()
        {

        }

        /// <summary>
        /// Callback method for when the Response is sent to the client
        /// </summary>
        private void Response_SentResponse(object sender, EventArgs e)
        {
            ResponseSent = true;
        }
    }
}
