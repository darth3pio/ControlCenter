using System;
using System.Collections.Generic;
using BF2Statistics.Database;

namespace BF2Statistics.Web.ASP
{
    /// <summary>
    /// /ASP/getawardsinfo.aspx
    /// </summary>
    class GetAwardsInfo
    {
        /// <summary>
        /// This request provides a list of awards for a particular player
        /// </summary>
        /// <queryParam name="pid" type="int">The unique player ID</queryParam>
        /// <param name="Client">The HttpClient who made the request</param>
        /// <param name="Driver">The Stats Database Driver. Connection errors are handled in the calling object</param>
        public GetAwardsInfo(HttpClient Client, StatsDatabase Driver)
        {
            int Pid;
            ASPResponse Response = Client.Response as ASPResponse;

            // make sure we have a valid player ID
            if (!Client.Request.QueryString.ContainsKey("pid") || !Int32.TryParse(Client.Request.QueryString["pid"], out Pid))
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

            try
            {
                // Fetch Player Awards
                List<Dictionary<string, object>> Awards = Driver.GetPlayerAwards(Pid);

                // Write each award as a new data line
                foreach (Dictionary<string, object> Award in Awards)
                    Response.WriteDataLine(Award["awd"], Award["level"], Award["earned"], Award["first"]);
            }
            catch { }

            // Send Response
            Response.Send();
        }
    }
}
