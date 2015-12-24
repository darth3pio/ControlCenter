using System;
using System.Collections.Generic;
using BF2Statistics.Database;

namespace BF2Statistics.Web.ASP
{
    /// <summary>
    /// /ASP/getrankinfo.aspx
    /// </summary>
    /// <queryParam name="pid" type="int">The unique player ID</queryParam>
    /// <seealso cref="http://bf2tech.org/index.php/BF2_Statistics#Function:_getrankinfo"/>
    public sealed class GetRankInfo : ASPController
    {
        /// <summary>
        /// This request provides details on a particular players rank, and
        /// whether or not to show the user a promotion/demotion announcement
        /// </summary>
        /// <queryParam name="pid" type="int">The unique player ID</queryParam>
        /// <param name="Client">The HttpClient who made the request</param>
        public GetRankInfo(HttpClient Client) : base(Client) { }

        public override void HandleRequest()
        {
            // NOTE: The HttpServer will handle the DbConnectException
            using (Database = new StatsDatabase())
            {
                int Pid = 0;

                // Setup Params
                if (Client.Request.QueryString.ContainsKey("pid"))
                    Int32.TryParse(Client.Request.QueryString["pid"], out Pid);

                // Fetch Player
                var Rows = Database.Query("SELECT rank, chng, decr FROM player WHERE id=@P0", Pid);
                if (Rows.Count == 0)
                {
                    Response.WriteResponseStart(false);
                    Response.WriteHeaderLine("asof", "err");
                    Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp(), "Player Doesnt Exist");
                    Response.Send();
                    return;
                }

                // Output status... chng set to 1 shows the Promotion Announcement, whereas decr shows the Demotion Announcement
                Response.WriteResponseStart();
                Response.WriteHeaderLine("rank", "chng", "decr");
                Response.WriteDataLine(Rows[0]["rank"], Rows[0]["chng"], Rows[0]["decr"]);
                Response.Send();

                // Reset
                Database.Execute("UPDATE player SET chng=0, decr=0 WHERE id=@P0", Pid);
            }
        }
    }
}
