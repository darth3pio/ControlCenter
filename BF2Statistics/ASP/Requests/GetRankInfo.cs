using System;
using System.Collections.Generic;
using BF2Statistics.Database;

namespace BF2Statistics.ASP.Requests
{
    class GetRankInfo
    {
        /// <summary>
        /// This request provides details on a particular players rank, and
        /// whether or not to show the user a promotion/demotion announcement
        /// </summary>
        /// <queryParam name="pid" type="int">The unique player ID</queryParam>
        /// <param name="Client">The HttpClient who made the request</param>
        /// <param name="Driver">The Stats Database Driver. Connection errors are handled in the calling object</param>
        public GetRankInfo(HttpClient Client, StatsDatabase Driver)
        {
            int Pid = 0;
            List<Dictionary<string, object>> Rows;

            // Setup Params
            if (Client.Request.QueryString.ContainsKey("pid"))
                Int32.TryParse(Client.Request.QueryString["pid"], out Pid);

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

            // Output status... chng set to 1 shows the Promotion Announcement, whereas decr shows the Demotion Announcement
            Client.Response.WriteResponseStart();
            Client.Response.WriteHeaderLine("rank", "chng", "decr");
            Client.Response.WriteDataLine(Rows[0]["rank"], Rows[0]["chng"], Rows[0]["decr"]);
            Client.Response.Send();

            // Reset
            Driver.Execute("UPDATE player SET chng=0, decr=0 WHERE id=@P0", Pid);
        }
    }
}
