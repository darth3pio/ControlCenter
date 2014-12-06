using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;
using System.Threading;
using System.IO;
using BF2Statistics.Web.ASP;

namespace BF2Statistics.Web
{
    public class HttpClient : IDisposable
    {
        /// <summary>
        /// Returns the IP Endpoint for the connected client
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; protected set; }

        /// <summary>
        /// Gets the HttpRequest object for this client
        /// </summary>
        public HttpRequest Request { get; protected set; }

        /// <summary>
        /// Gets the response object for this client
        /// </summary>
        public HttpResponse Response { get; protected set; }

        /// <summary>
        /// Indicates whether this request is an ASP request
        /// </summary>
        public bool IsASPRequest { get; protected set; }

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
            RemoteEndPoint = Client.Request.RemoteEndPoint as IPEndPoint;
            Request = new HttpRequest(Client.Request, this);
            if (Client.Request.Url.AbsolutePath.ToLower().StartsWith("/asp/"))
            {
                IsASPRequest = true;
                Response = new ASPResponse(Client.Response, this);
            }
            else
            {
                IsASPRequest = false;
                Response = new HttpResponse(Client.Response, this);
            }

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
