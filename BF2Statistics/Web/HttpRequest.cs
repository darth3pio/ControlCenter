using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace BF2Statistics.Web
{
    public class HttpRequest
    {
        /// <summary>
        /// The Parent Http Request
        /// </summary>
        protected HttpClient Client;

        /// <summary>
        /// The HttpListener Request
        /// </summary>
        public HttpListenerRequest Request { get; protected set; }

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

        protected Dictionary<string, string> PostVars = new Dictionary<string, string>();

        /// <summary>
        /// Creates a new instance of HttpRequest
        /// </summary>
        /// <param name="Client">The HttpClient creating this response</param>
        public HttpRequest(HttpListenerRequest Request, HttpClient Client)
        {
            // Create a better QueryString object
            this.QueryString = Request.QueryString.Cast<string>().ToDictionary(p => p, p => Request.QueryString[p]);
            this.Request = Request;
            this.Client = Client;
        }

        /// <summary>
        /// If a form was submitted using the POST http method, and the entity body is UrlFormEncoded,
        /// the post variables can be fetched from this method
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetFormUrlEncodedPostVars()
        {
            if (Request.HasEntityBody && PostVars.Count == 0)
            {
                try
                {
                    using (StreamReader Rdr = new StreamReader(Request.InputStream))
                    {
                        string[] rawParams = Rdr.ReadToEnd().Split('&');
                        foreach (string param in rawParams)
                        {
                            string[] kvPair = param.Split('=');
                            if (kvPair.Length == 2)
                                PostVars.Add(kvPair[0], Uri.UnescapeDataString(kvPair[1]));
                        }
                    }
                }
                catch { }
            }

            return PostVars;
        }
    }
}
