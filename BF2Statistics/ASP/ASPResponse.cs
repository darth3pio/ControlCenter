using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace BF2Statistics.ASP
{
    class ASPResponse
    {
        /// <summary>
        /// Contains the HttpClient object
        /// </summary>
        private HttpClient Client;

        /// <summary>
        /// The Http Response Object
        /// </summary>
        private HttpListenerResponse Response;

        /// <summary>
        /// Our connection timer
        /// </summary>
        private Stopwatch Clock;

        /// <summary>
        /// The Response Body to send to the client
        /// </summary>
        private StringBuilder ResponseBody;

        /// <summary>
        /// The Http Status Code
        /// </summary>
        protected int statusCode = (int)HttpStatusCode.OK;

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
            get { return statusCode; }
            set
            {
                // Need to make sure its valid, or throw an exception
                statusCode = value;
                StatusDescription = GetStatusMessage(value);
            }
        }

        /// <summary>
        /// Gets or sets a text description of the HTTP status code returned to the client.
        /// </summary>
        public string StatusDescription = "OK";

        /// <summary>
        /// Indicates whether the data will be transposed
        /// </summary>
        public bool Transpose
        {
            get
            {
                return (
                    Client.Request.QueryString.ContainsKey("transpose")
                    && Client.Request.QueryString["transpose"] == "1"
                );
            }
        }

        /// <summary>
        /// Internal, Temporary Formatted Output class.
        /// We use this to easily transpose format
        /// </summary>
        protected FormattedOutput Formatted;

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
        public ASPResponse(HttpListenerResponse Response, HttpClient Client)
        {
            // Set Iternals
            this.Response = Response;
            this.Client = Client;

            // Start our Timer
            Clock = new Stopwatch();
            Clock.Start();

            // Create a new Response Body
            ResponseBody = new StringBuilder();
        }

        /// <summary>
        /// Adds HeaderData to the current output
        /// </summary>
        /// <param name="Data"></param>
        public void AddData(FormattedOutput Data)
        {
            Data.Transpose = Transpose;
            ResponseBody.Append(Data.ToString().Trim());
        }

        /// <summary>
        /// Adds HeaderData to the current output
        /// </summary>
        /// <param name="Data"></param>
        public void WriteHeaderDataPair(Dictionary<string, object> Data)
        {
            if (Transpose)
            {
                if (Formatted != null)
                    ResponseBody.Append(Formatted.ToString());

                List<string> Params = new List<string>(Data.Count);
                foreach (KeyValuePair<string, object> Item in Data)
                    Params.Add(Item.Key);

                Formatted = new FormattedOutput(Params);
                Formatted.Transpose = true;

                Params = new List<string>(Data.Count);
                foreach (KeyValuePair<string, object> Item in Data)
                    Params.Add(Item.Value.ToString());

                Formatted.AddRow(Params);
            }
            else
            {
                // Add Keys
                ResponseBody.Append("\nH");
                foreach (KeyValuePair<string, object> Item in Data)
                    ResponseBody.Append("\t" + Item.Key);

                // Add Data
                ResponseBody.Append("\nD");
                foreach (KeyValuePair<string, object> Item in Data)
                    ResponseBody.Append("\t" + Item.Value);
            }
        }

        /// <summary>
        /// Opens the ASP response with the Valid Data opening tag ( "O" )
        /// <remarks>Calling this method will reset all current running data.</remarks>
        /// </summary>
        public void WriteResponseStart()
        {
            ResponseBody = new StringBuilder("O");
        }

        /// <summary>
        /// Starts the ASP formatted response
        /// </summary>
        /// <param name="IsValid">
        /// Defines whether this response is valid data. If false,
        /// the opening tag will be an "E" rather then "O"
        /// <remarks>Calling this method will reset all current running data.</remarks>
        /// </param>
        public void WriteResponseStart(bool IsValid)
        {
            ResponseBody = new StringBuilder(((IsValid) ? "O" : "E"));
        }

        /// <summary>
        /// Writes a Header line with the specified parameters
        /// </summary>
        /// <param name="Params"></param>
        public void WriteHeaderLine(params object[] Params)
        {
            if (Transpose)
            {
                if (Formatted != null)
                    ResponseBody.Append(Formatted.ToString());

                Formatted = new FormattedOutput(Params);
                Formatted.Transpose = true;
            }
            else
                ResponseBody.AppendFormat("\nH\t{0}", String.Join("\t", Params));
        }

        /// <summary>
        /// Writes a header line with the items provided in the List
        /// </summary>
        /// <param name="Headers"></param>
        public void WriteHeaderLine(List<string> Headers)
        {
            if (Transpose)
            {
                if (Formatted != null)
                    ResponseBody.Append(Formatted.ToString());

                Formatted = new FormattedOutput(Headers);
                Formatted.Transpose = true;
            }
            else
                ResponseBody.AppendFormat("\nH\t{0}", String.Join("\t", Headers));
        }

        /// <summary>
        /// Writes a Data line with the specified parameters
        /// </summary>
        /// <param name="Params"></param>
        public void WriteDataLine(params object[] Params)
        {
            if (Transpose)
                Formatted.AddRow(Params);
            else
                ResponseBody.AppendFormat("\nD\t{0}", String.Join("\t", Params));
        }

        /// <summary>
        /// Writes a data line with the items provided in the List
        /// </summary>
        /// <param name="Params"></param>
        public void WriteDataLine(List<string> Params)
        {
            if (Transpose)
                Formatted.AddRow(Params);
            else
                ResponseBody.AppendFormat("\nD\t{0}", String.Join("\t", Params));
        }

        /// <summary>
        /// Write's clean data to the stream
        /// </summary>
        /// <param name="Message"></param>
        public void WriteFreeformLine(string Message)
        {
            ResponseBody.AppendFormat("\n{0}", Message);
        }

        /// <summary>
        /// Writes the closing ASP response tags
        /// </summary>
        protected void WriteResponseEnd()
        {
            ResponseBody.AppendFormat("\n$\t{0}\t$", (Regex.Replace(ResponseBody.ToString(), "[\t\n]", "")).Length);
        }

        /// <summary>
        /// Sends all output to the browser
        /// </summary>
        public void Send()
        {
            // Make sure our client didnt send a response already
            if (Client.ResponseSent)
                return;

            // Fire Event
            if (SendingResponse != null)
                SendingResponse(this, null);

            // Create body buffer
            byte[] Body = new byte[0];

            // Get body based on HttpStatus Code
            if (StatusCode == 200)
            {
                // Whats left of the transposed data
                if (Transpose && Formatted != null)
                    ResponseBody.Append(Formatted.ToString());

                // Write the data and close the stream
                WriteResponseEnd();
                Body = Encoding.UTF8.GetBytes(ResponseBody.ToString());
                Response.ContentType = "text/plain";
            }
            else
            {
                // Just send response if we are not a valid response
                Body = Encoding.UTF8.GetBytes(BuildErrorPage());
                Response.ContentType = "text/html";
            }

            // Set the contents length, and encoding. Instruct to close the connection
            Response.ContentLength64 = Body.Length;
            Response.ContentEncoding = Encoding.UTF8;
            Response.KeepAlive = false;

            // Send Response to the remote socket
            Response.OutputStream.Write(Body, 0, Body.Length);
            Response.Close();
            Clock.Stop();

            // Log Request
            ASPServer.AccessLog.Write("{0} - \"{1}\" [Status: {2}, Len: {3}, Time: {4}ms]",
                Client.RemoteEndPoint.Address,
                String.Concat(
                    Client.Request.HttpMethod,
                    " ", Client.Request.Url.PathAndQuery,
                    " HTTP/",
                    Client.Request.ProtocolVersion
                ),
                StatusCode,
                Body.Length,
                Clock.ElapsedMilliseconds
            );

            // Fire Event
            if (SentResponse != null)
                SentResponse(this, null);
        }

        /// <summary>
        /// Builds, and returns an HTML formated error page
        /// </summary>
        /// <returns></returns>
        protected string BuildErrorPage()
        {
            string Page = Utils.GetResourceAsString("BF2Statistics.ASP.Error.html");
            return String.Format(Page, StatusCode, StatusDescription, GetErrorDesciption());
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
