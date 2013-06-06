using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using BF2Statistics.Logging;

namespace BF2Statistics.ASP
{
    class ASPResponse
    {
        /// <summary>
        /// This is the contents response code "O" or "E"
        /// that is sent by the official Gamespy ASP.
        /// </summary>
        public enum DataType
        {
            OK = 'O',
            Error = 'E'
        }

        /// <summary>
        /// This is the response code
        /// </summary>
        private DataType ResponseType = DataType.OK;

        /// <summary>
        /// The Response Body to send to the client
        /// </summary>
        private string ResponseBody = "";

        /// <summary>
        /// The Http Response Object
        /// </summary>
        private HttpListenerResponse Response;

        /// <summary>
        /// The Http Request Object
        /// </summary>
        private HttpListenerRequest Request;

        /// <summary>
        /// Our Access log
        /// </summary>
        private static LogWritter AccessLog = new LogWritter(Path.Combine(MainForm.Root, "Logs", "AspAccess.log"), 3000);

        /// <summary>
        /// The HTTP Status Code to Send
        /// </summary>
        public int StatusCode
        {
            get { return this.Response.StatusCode; }
            set 
            {
                this.Response.StatusCode = value;
                this.Response.StatusDescription = GetStatusMessage(value);
            }
        }

        /// <summary>
        /// Do we format the ASP data with the O/E and $ length $ ?
        /// </summary>
        public bool FormatData = true;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Stream"></param>
        public ASPResponse(HttpListenerRequest Request, HttpListenerResponse Response)
        {
            this.Response = Response;
            this.Request = Request;
        }

        /// <summary>
        /// Defines whether the output content is Valid Data, or Error Data
        /// </summary>
        /// <param name="Is"></param>
        public void IsValidData(bool Is)
        {
            ResponseType = (Is) ? DataType.OK : DataType.Error;
        }

        /// <summary>
        /// Adds HeaderData to the current output
        /// </summary>
        /// <param name="Data"></param>
        public void AddData(FormattedOutput Data)
        {
            ResponseBody += "\n" + Data.ToString().Trim();
        }

        /// <summary>
        /// Adds HeaderData to the current output
        /// </summary>
        /// <param name="Data"></param>
        public void AddData(Dictionary<string, object> Data)
        {
            List<string> Head = new List<string>(Data.Count);
            List<string> Body = new List<string>(Data.Count);
            foreach (KeyValuePair<string, object> Item in Data)
            {
                Head.Add(Item.Key);
                Body.Add(Item.Value.ToString());
            }

            FormattedOutput Output = new FormattedOutput(Head);
            Output.AddRow(Body);
            ResponseBody += "\n" + Output.ToString().Trim();
        }

        /// <summary>
        /// Write's clean data to the stream
        /// </summary>
        /// <param name="Message"></param>
        public void WriteLine(string Message)
        {
            ResponseBody += "\n" + Message;
        }

        /// <summary>
        /// Write's clean data to the stream
        /// </summary>
        /// <param name="Message"></param>
        public void AddString(string Message, params object[] Items)
        {
            ResponseBody += "\n" + String.Format(Message, Items);
        }

        /// <summary>
        /// Sends all output to the browser
        /// </summary>
        public void Send()
        {
            // Just send response if we are not a valid response
            if (StatusCode != 200)
            {
                ShowError();
                return;
            }

            // Format data into ASP format?
            if (FormatData)
            {
                ResponseBody = ((ResponseType == DataType.OK) ? "O" : "E") + ResponseBody;
                ResponseBody += String.Format("\n$\t{0}\t$", (Regex.Replace(ResponseBody, "[\t\n]", "")).Length);
            }

            // Write the data and close the stream
            byte[] Final = Encoding.UTF8.GetBytes(ResponseBody);
            Response.ContentLength64 = Final.Length;
            Response.ContentEncoding = Encoding.UTF8;
            Response.KeepAlive = false;

            // Log Request
            AccessLog.Write("{0} - \"{1}\" {2} {3}", 
                Request.RemoteEndPoint.Address, 
                Request.HttpMethod + " " + Request.Url.PathAndQuery + " HTTP/" + Request.ProtocolVersion.ToString(),
                StatusCode,
                Final.Length
            );

            // Send Response
            Response.OutputStream.Write(Final, 0, Final.Length);
            Response.Close();
        }

        /// <summary>
        /// Gets the correct HTTP Status message from the StatusCode
        /// </summary>
        /// <returns></returns>
        public string GetStatusMessage(int StatusCode)
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
                case 501:
                   return "Not Implemented";
                default:
                   return "Internal Server Error";
            }
        }

        private void ShowError()
        {
            // Prepare Output Message
            byte[] Message = Encoding.UTF8.GetBytes(Utils.GetResourceString("BF2Statistics.ASP.Errors." + StatusCode + ".tpl"));
            int Len = Message.Length;

            // Log Request
            AccessLog.Write("{0} - \"{1}\" {2} {3}",
                Request.RemoteEndPoint.Address,
                Request.HttpMethod + " " + Request.Url.PathAndQuery + " HTTP/" + Request.ProtocolVersion.ToString(),
                StatusCode,
                Len
            );

            // Send 404
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentLength64 = Len;
            Response.OutputStream.Write(Message, 0, Len);
            Response.Close();
        }
    }
}
