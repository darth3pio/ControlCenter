using System;
using System.Collections.Generic;
using BF2Statistics.Database;

namespace BF2Statistics.Web.ASP
{
    /// <summary>
    /// /ASP/getawardsinfo.aspx
    /// </summary>
    /// <queryParam name="pid" type="int">The unique player ID</queryParam>
    /// <seealso cref="http://bf2tech.org/index.php/BF2_Statistics#Function:_getawardsinfo"/>
    public sealed class GetAwardsInfo : ASPController
    {
        /// <summary>
        /// This request provides a list of awards for a particular player
        /// </summary>
        /// <param name="Client">The HttpClient who made the request</param>
        /// <param name="Driver">The Stats Database Driver. Connection errors are handled in the calling object</param>
        public GetAwardsInfo(HttpClient Client) : base(Client) { }

        public override void HandleRequest()
        {
            int Pid;

            // make sure we have a valid player ID
            if (!Request.QueryString.ContainsKey("pid") || !Int32.TryParse(Request.QueryString["pid"], out Pid))
            {
                Response.WriteResponseStart(false);
                Response.WriteHeaderLine("asof", "err");
                Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp(), "Invalid Syntax!");
                Response.Send();
                return;
            }

            // Output header data
            Response.WriteResponseStart();
            Response.WriteHeaderLine("pid", "asof");
            Response.WriteDataLine(Pid, DateTime.UtcNow.ToUnixTimestamp());
            Response.WriteHeaderLine("award", "level", "when", "first");

            // Fetch Player Awards
            // NOTE: The HttpServer will handle the DbConnectException
            using (Database = new StatsDatabase())
            {
                List<Dictionary<string, object>> Awards = Database.GetPlayerAwards(Pid);

                // Write each award as a new data line
                foreach (Dictionary<string, object> Award in Awards)
                    Response.WriteDataLine(Award["awd"], Award["level"], Award["earned"], Award["first"]);

                // Send Response
                Response.Send();
            }
        }
    }
}
