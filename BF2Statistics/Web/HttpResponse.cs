using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using BF2Statistics.ASP;

namespace BF2Statistics.Web
{
    public class HttpResponse
    {
        /// <summary>
        /// Contains the HttpClient object
        /// </summary>
        protected HttpClient Client;

        /// <summary>
        /// The Http Response Object
        /// </summary>
        protected HttpListenerResponse Response;

        /// <summary>
        /// Our connection timer
        /// </summary>
        protected Stopwatch Clock;

        /// <summary>
        /// The Response Body to send to the client
        /// </summary>
        public StringBuilder ResponseBody { get; protected set; }

        /// <summary>
        /// Gets or sets the HTTP status code to be returned to the client
        /// See the <see cref="System.Net.HttpStatusCode"/> Enumeration
        /// </summary>
        /// <remarks>
        ///     Setting the status code will also automatically update the
        ///     <see cref="StatusDescription"/>
        /// </remarks>
        public int StatusCode
        {
            get { return Response.StatusCode; }
            set
            {
                // Need to make sure its valid, or throw an exception
                Response.StatusCode = value;
                Response.StatusDescription = GetStatusMessage(value);
            }
        }

        /// <summary>
        /// Gets or sets a text description of the HTTP status code returned to the client.
        /// </summary>
        public string StatusDescription
        {
            get { return Response.StatusDescription; }
            set { Response.StatusDescription = value; }
        }

        /// <summary>
        /// Gets or sets the content type
        /// </summary>
        public string ContentType
        {
            get { return Response.ContentType; }
            set { Response.ContentType = value; }
        }

        /// <summary>
        /// Indicates whether the response object is finished
        /// </summary>
        public bool ResponseSent { get; protected set; }

        /// <summary>
        /// Event that is called when the response is being prepared to be sent
        /// </summary>
        public event EventHandler SendingResponse;

        /// <summary>
        /// Event that is called when the response has been written to the
        /// output stream.
        /// </summary>
        public event EventHandler SentResponse;

        /// <summary>
        /// Constructor
        /// </summary>
        public HttpResponse(HttpListenerResponse Response, HttpClient Client)
        {
            // Set internals
            this.Client = Client;
            this.Response = Response;
            this.ResponseBody = new StringBuilder();
            this.ResponseSent = false;

            // Start the stopwatch for response benchmarking
            this.Clock = new Stopwatch();
            this.Clock.Start();
        }

        /// <summary>
        /// Redirects the client to the new URL and closes the connection
        /// </summary>
        /// <param name="url">The full url to redirect the client to.</param>
        /// <exception cref="System.Exception">
        ///     An exception will be thrown if the response has already been sent. Use the 
        ///     <see cref="ResponseSent" /> parameter before attempting to redirect someone.
        /// </exception>
        public void Redirect(string url)
        {
            // Cant send a redirect if the response has been sent!
            if (ResponseSent)
                throw new Exception("Unable to redirect client, response has already been sent!");

            // Send redirect data
            Response.KeepAlive = false;
            Response.ContentLength64 = 0;
            Response.Redirect(url);
            Response.Close();
            Clock.Stop();

            // Log Request
            LogAccess();

            // Response is finished
            ResponseSent = true;

            // Fire Event
            if (SentResponse != null)
                SentResponse(this, null);
        }

        /// <summary>
        /// Sets a cookie to be sent to the client
        /// </summary>
        /// <param name="C">The cookie data to be set</param>
        /// <exception cref="System.Exception">
        ///     An exception will be thrown if the response has already been sent. Use the 
        ///     <see cref="ResponseSent" /> parameter before attempting to set a cookie.
        /// </exception>
        public void SetCookie(Cookie C)
        {
            // Cant send a cookie if the response has been sent!
            if (ResponseSent)
                throw new Exception("Unable to set cookie, response has already been sent!");

            Response.SetCookie(C);
        }

        /// <summary>
        /// Sends the specified bytes to the web browser
        /// </summary>
        /// <param name="Body"></param>
        /// <exception cref="System.Exception">
        ///     An exception will be thrown if the response has already been sent. Use the 
        ///     <see cref="ResponseSent" /> parameter before sending.
        /// </exception>
        public void Send(byte[] Body)
        {
            // Cant send a second response!
            if (ResponseSent)
                throw new Exception("Unable to send a response to the client, A response has already been sent!");

            // Fire Event
            if (SendingResponse != null)
                SendingResponse(this, null);

            // Set the contents length, Instruct to close the connection
            Response.ContentLength64 = Body.Length;
            Response.KeepAlive = false;

            // Send Response to the remote socket
            Response.OutputStream.Write(Body, 0, Body.Length);
            Response.Close();
            Clock.Stop();

            // Log Request
            LogAccess();

            // Response is finished
            ResponseSent = true;

            // Fire Event
            if (SentResponse != null)
                SentResponse(this, null);
        }

        /// <summary>
        /// Sends the ResponseBody to the output browser
        /// </summary>
        public void Send()
        {
            // Set content encoding, and send the response body
            Response.ContentEncoding = Encoding.UTF8;
            this.Send(Encoding.UTF8.GetBytes((StatusCode == 200) ? ResponseBody.ToString() : BuildErrorPage()));
        }

        /// <summary>
        /// Builds, and returns an HTML formated error page
        /// </summary>
        /// <returns></returns>
        protected string BuildErrorPage()
        {
            string Page = Utils.GetResourceAsString("BF2Statistics.Web.Error.html");
            return String.Format(Page, StatusCode, StatusDescription, GetErrorDesciption());
        }

        /// <summary>
        /// Writes the connection information to the Server Access log
        /// </summary>
        protected void LogAccess()
        {
            HttpServer.AccessLog.Write("{0} - \"{1}\" [Status: {2}, Len: {3}, Time: {4}ms]",
                Client.RemoteEndPoint.Address,
                String.Concat(
                    Client.Request.HttpMethod,
                    " ", Client.Request.Url.PathAndQuery,
                    " HTTP/",
                    Client.Request.ProtocolVersion
                ),
                Response.StatusCode,
                Response.ContentLength64,
                Clock.ElapsedMilliseconds
            );
        }

        /// <summary>
        /// Gets the correct HTTP Status message from the StatusCode
        /// </summary>
        /// <returns></returns>
        protected string GetStatusMessage(int StatusCode)
        {
            switch (StatusCode)
            {
                case 200:
                    return "OK";
                case 400:
                    return "Bad Request";
                case 403:
                    return "Forbidden";
                case 404:
                    return "Not Found";
                case 405:
                    return "Method Not Allowed";
                case 411:
                    return "Length Required";
                case 501:
                    return "Not Implemented";
                case 503:
                    return "Service Unavailable";
                default:
                    return "Internal Server Error";
            }
        }

        /// <summary>
        /// Gets the description text of an Http Error
        /// </summary>
        /// <returns></returns>
        protected string GetErrorDesciption()
        {
            switch (StatusCode)
            {
                case 400:
                    return "The server was unable to understand the request";
                case 403:
                    return "You don't have permission to access this page";
                case 404:
                    return "The requested resource is not found";
                case 405:
                    return "The Request Method Is Not Allowed";
                case 411:
                    return "The server refuses to accept the request without a defined Content- Length";
                case 500:
                    return "An Internal Server Error occurred during this request";
                case 501:
                    return "Request method is not supported";
                case 503:
                    return "The service is unavailable";
                default:
                    return "";
            }
        }
    }
}
