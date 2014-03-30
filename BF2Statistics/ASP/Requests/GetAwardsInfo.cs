using System;
using System.Collections.Generic;
using BF2Statistics.Database;
using BF2Statistics.Database.QueryBuilder;

namespace BF2Statistics.ASP.Requests
{
    class GetAwardsInfo
    {
        public GetAwardsInfo(HttpClient Client, StatsDatabase Driver)
        {
            int Pid;

            // make sure we have a valid player ID
            if (!Client.Request.QueryString.ContainsKey("pid") || !Int32.TryParse(Client.Request.QueryString["pid"], out Pid))
            {
                Client.Response.WriteResponseStart(false);
                Client.Response.WriteHeaderLine("asof", "err");
                Client.Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp(), "Invalid Syntax!");
                Client.Response.Send();
                return;
            }

            // Output header data
            Client.Response.WriteResponseStart();
            Client.Response.WriteHeaderLine("pid", "asof");
            Client.Response.WriteDataLine(Pid, DateTime.UtcNow.ToUnixTimestamp());
            Client.Response.WriteHeaderLine("award", "level", "when", "first");

            try
            {
                // Fetch Player Awards
                List<Dictionary<string, object>> Awards = Driver.GetPlayerAwards(Pid);

                // Write each award as a new data line
                foreach (Dictionary<string, object> Award in Awards)
                    Client.Response.WriteDataLine(Award["awd"], Award["level"], Award["earned"], Award["first"]);
            }
            catch { }

            // Send Response
            Client.Response.Send();
        }
    }
}
