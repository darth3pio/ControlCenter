using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace BF2Statistics.ASP
{
    class ASPRequest
    {
        /// <summary>
        /// The Parent Http Request
        /// </summary>
        protected HttpClient Client;

        /// <summary>
        /// The HttpListener Request
        /// </summary>
        protected HttpListenerRequest Request;

        /// <summary>
        /// Contains the Key / Value pairs in the query string
        /// </summary>
        public Dictionary<string, string> QueryString;

        /// <summary>
        /// Gets the HTTP method specified by the client
        /// </summary>
        public string HttpMethod
        {
            get { return Request.HttpMethod; }
        }

        /// <summary>
        /// Returns the protocol version
        /// </summary>
        public Version ProtocolVersion
        {
            get { return Request.ProtocolVersion; }
        }

        /// <summary>
        /// Gets the MIME types accepted by the client.
        /// </summary>
        public string[] AcceptTypes
        {
            get { return Request.AcceptTypes; }
        }

        /// <summary>
        /// Gets the User Agent
        /// </summary>
        public string UserAgent
        {
            get { return Request.UserAgent; }
        }

        /// <summary>
        /// Indicates whether this Remote Http Client is Local or Remote
        /// </summary>
        public bool IsLocal 
        {
            get { return Request.IsLocal; }
        }

        /// <summary>
        /// Gets the content encoding that can be used with data sent with the request
        /// </summary>
        public Encoding ContentEncoding 
        {
            get { return Request.ContentEncoding; }
        }

        /// <summary>
        /// Indicates whether a body was included in the request
        /// </summary>
        public bool HasEntityBody
        {
            get { return Request.HasEntityBody; }
        }

        /// <summary>
        /// Gets a stream that contains the body data sent by the client.
        /// </summary>
        public Stream InputStream
        {
            get { return Request.InputStream; }
        }

        /// <summary>
        /// Returns the Request URI object
        /// </summary>
        public Uri Url 
        {
            get { return Request.Url; }
        }

        /// <summary>
        /// Creates a new instance of HttpRequest
        /// </summary>
        /// <param name="Client">The HttpClient creating this response</param>
        public ASPRequest(HttpListenerRequest Request, HttpClient Client)
        {
            // Create a better QueryString object
            QueryString = Request.QueryString.Cast<string>()
                .Select(s => new { Key = s, Value = Request.QueryString[s] })
                .ToDictionary(p => p.Key, p => p.Value);

            this.Request = Request;
            this.Client = Client;
        }
    }
}
