using System;
using System.Collections.Generic;
using BF2Statistics.Database;

namespace BF2Statistics.Web.ASP
{
    /// <summary>
    /// /ASP/ranknotification.aspx
    /// </summary>
    public sealed class RankNotification : ASPController
    {
        /// <summary>
        /// This request clears all rank announcements for a specific player
        /// </summary>
        /// <queryParam name="pid" type="int">The unique player ID</queryParam>
        /// <param name="Client">The HttpClient who made the request</param>
        public RankNotification(HttpClient Client) : base(Client) { }

        public override void HandleRequest()
        {
            // NOTE: The HttpServer will handle the DbConnectException
            using (Database = new StatsDatabase())
            {
                int Pid = 0;
                List<Dictionary<string, object>> Rows;

                // Setup Params
                if (Client.Request.QueryString.ContainsKey("pid"))
                    Int32.TryParse(Client.Request.QueryString["pid"], out Pid);

                // Fetch Player
                Rows = Database.Query("SELECT rank FROM player WHERE id=@P0", Pid);
                if (Rows.Count == 0)
                {
                    Response.WriteResponseStart(false);
                    Response.WriteFreeformLine("Player Doesnt Exist!");
                    Client.Response.Send();
                    return;
                }

                // Reset
                Database.Execute("UPDATE player SET chng=0, decr=0 WHERE id=@P0", Pid);
                Response.WriteResponseStart();
                Response.WriteFreeformLine(String.Format("Cleared rank notification {0}", Pid));
                Response.Send();
            }
        }
    }
}
