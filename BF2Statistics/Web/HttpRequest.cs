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

        /// <summary>
        /// A Key => Value collection of our formUrlEncoded POST Vars
        /// </summary>
        protected Dictionary<string, string> PostVars;

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
            // No post data, or processed already?
            if (!Request.HasEntityBody || PostVars != null)
            {
                // Initialize empty Dic
                if (PostVars == null)
                    PostVars = new Dictionary<string, string>(0);

                return PostVars;
            }

            // Always wrap handling user data in a Try-Catch
            try
            {
                using (StreamReader reader = new StreamReader(Request.InputStream, Encoding.UTF8))
                {
                    string[] rawParams = reader.ReadToEnd().Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                    PostVars = new Dictionary<string, string>(rawParams.Length);
                    foreach (string field in rawParams)
                    {
                        // Convert to Key/Value
                        string[] pair = field.Split(new[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);

                        // If we dont have a key/pair, skip
                        if (pair.Length != 2)
                            continue;

                        PostVars[pair[0]] = Uri.UnescapeDataString(pair[1]);
                    }
                }
            }
            catch { }

            return PostVars;
        }
    }
}
