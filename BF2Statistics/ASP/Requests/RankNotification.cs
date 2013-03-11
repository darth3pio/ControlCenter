using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Rows = Driver.Query("SELECT rank FROM player WHERE id={0}", Pid);
            if (Rows.Count == 0)
            {
                Response.WriteLine("Player Doesnt Exist!");
                Response.IsValidData(false);
                Response.Send();
                return;
            }

            // Reset
            Driver.Execute("UPDATE player SET chng=0, decr=0 WHERE id={0}", Pid);
            Response.AddString("Cleared rank notification {0}", Pid);
            Response.Send();
        }
    }
}
