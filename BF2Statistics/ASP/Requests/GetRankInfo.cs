using System;
using System.Collections.Generic;
using BF2Statistics.Database;

namespace BF2Statistics.ASP.Requests
{
    class GetRankInfo
    {
        public GetRankInfo(HttpClient Client, StatsDatabase Driver)
        {
            int Pid = 0;
            List<Dictionary<string, object>> Rows;
            Dictionary<string, string> QueryString = Client.Request.QueryString;

            // Setup Params
            if (QueryString.ContainsKey("pid"))
                Int32.TryParse(QueryString["pid"], out Pid);

            // Fetch Player
            Rows = Driver.Query("SELECT rank, chng, decr FROM player WHERE id=@P0", Pid);
            if (Rows.Count == 0)
            {
                Client.Response.WriteResponseStart(false);
                Client.Response.WriteHeaderLine("asof", "err");
                Client.Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp(), "Player Doesnt Exist");
                Client.Response.Send();
                return;
            }

            // Output status
            Client.Response.WriteResponseStart();
            Client.Response.WriteHeaderLine("rank", "chng", "decr");
            Client.Response.WriteDataLine(Rows[0]["rank"], Rows[0]["chng"], Rows[0]["decr"]);
            Client.Response.Send();

            // Reset
            Driver.Execute("UPDATE player SET chng=0, decr=0 WHERE id=@P0", Pid);
        }
    }
}
