using System;
using System.Collections.Generic;
using BF2Statistics.Database;

namespace BF2Statistics.ASP.Requests
{
    class RankNotification
    {
        public RankNotification(ASPResponse Response, Dictionary<string, string> QueryString)
        {
            int Pid = 0;
            DatabaseDriver Driver = ASPServer.Database.Driver;
            List<Dictionary<string, object>> Rows;

            // Setup Params
            if (QueryString.ContainsKey("pid"))
                Int32.TryParse(QueryString["pid"], out Pid);

            // Fetch Player
            Rows = Driver.Query("SELECT rank FROM player WHERE id=@P0", Pid);
            if (Rows.Count == 0)
            {
                Response.WriteLine("Player Doesnt Exist!");
                Response.IsValidData(false);
                Response.Send();
                return;
            }

            // Reset
            Driver.Execute("UPDATE player SET chng=0, decr=0 WHERE id=@P0", Pid);
            Response.WriteLine(String.Format("Cleared rank notification {0}", Pid));
            Response.Send();
        }
    }
}
