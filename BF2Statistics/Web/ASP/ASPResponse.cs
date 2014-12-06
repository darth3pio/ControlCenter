using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;
using BF2Statistics.Web;

namespace BF2Statistics.Web.ASP
{
    class ASPResponse : HttpResponse
    {
        /// <summary>
        /// Indicates whether the data will be transposed
        /// </summary>
        public bool Transpose { get;  protected set; }

        /// <summary>
        /// Internal, Temporary Formatted Output class.
        /// We use this to easily transpose format
        /// </summary>
        protected FormattedOutput Formatted;

        /// <summary>
        /// Constructor
        /// </summary>
        public ASPResponse(HttpListenerResponse Response, HttpClient Client) : base(Response, Client)
        {
            // Transpose?
            Transpose = (
                    Client.Request.QueryString.ContainsKey("transpose")
                    && Client.Request.QueryString["transpose"] == "1"
                );
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
        public new void Send()
        {
            // Make sure our client didnt send a response already
            if (Client.ResponseSent)
                return;

            // Whats left of the transposed data
            if (StatusCode == 200)
            {
                if (Transpose && Formatted != null)
                    ResponseBody.Append(Formatted.ToString());

                WriteResponseEnd();
                Response.ContentType = "text/plain";
            }

            base.Send();
        }
    }
}
