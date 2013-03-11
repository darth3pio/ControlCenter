using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BF2Statistics.Database;

namespace BF2Statistics.ASP.Requests
{
    class GetRankInfo
    {
        public GetRankInfo(ASPResponse Response, Dictionary<string, string> QueryString)
        {
            int Pid = 0;
            DatabaseDriver Driver = ASPServer.Database.Driver;
            List<Dictionary<string, object>> Rows;
            FormattedOutput Output;

            // Setup Params
            if (QueryString.ContainsKey("pid"))
                Int32.TryParse(QueryString["pid"], out Pid);

            // Fetch Player
            Rows = Driver.Query("SELECT rank, chng, decr FROM player WHERE id={0}", Pid);
            if (Rows.Count == 0)
            {
                Output = new FormattedOutput("asof", "err");
                Output.AddRow(Utils.UnixTimestamp(), "Player Doesnt Exist!");
                Response.AddData(Output);
                Response.IsValidData(false);
                Response.Send();
                return;
            }

            // Output status
            Output = new FormattedOutput("rank", "chng", "decr");
            Output.AddRow(Rows[0]["rank"], Rows[0]["chng"], Rows[0]["decr"]);
            Response.AddData(Output);
            Response.Send();

            // Reset
            Driver.Execute("UPDATE player SET chng=0, decr=0 WHERE id={0}", Pid);
        }
    }
}
