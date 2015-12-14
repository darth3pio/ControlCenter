using System;
using System.Net;
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
        /// Indicates whether the response object has written to the output stream
        /// </summary>
        public bool ResponseSent 
        {
            get { return this.Response.ResponseSent; }
        }

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
                Response = new ASPResponse(Client.Response, this);
            else
                Response = new HttpResponse(Client.Response, this);

            // Add BF2Statistics Header
            Client.Response.AddHeader("X-Powered-By", "BF2Statistics Control Center v" + Program.Version);
        }

        public void Dispose()
        {

        }
    }
}
