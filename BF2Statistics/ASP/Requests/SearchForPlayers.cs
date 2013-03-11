using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BF2Statistics.Database;

namespace BF2Statistics.ASP.Requests
{
    class SearchForPlayers
    {
        public SearchForPlayers(ASPResponse Response, Dictionary<string, string> QueryString)
        {
            string Nick;
            DatabaseDriver Driver = ASPServer.Database.Driver;
            List<Dictionary<string, object>> Rows;
            FormattedOutput Output;

            // Setup Params
            if (!QueryString.ContainsKey("nick"))
            {
                Output = new FormattedOutput("asof", "err");
                Output.AddRow(Utils.UnixTimestamp(), "Invalid Syntax!");
                Response.AddData(Output);
                Response.IsValidData(false);
                Response.Send();
                return;
            }
            else
                Nick = QueryString["nick"];

            // Timestamp Header
            Output = new FormattedOutput("asof");
            Output.AddRow(Utils.UnixTimestamp());
            Response.AddData(Output);

            // Output status
            Output = new FormattedOutput("pid", "nick", "score");
            Rows = Driver.Query("SELECT id, name, score FROM player WHERE name LIKE '%{0}%'", Nick);
            foreach(Dictionary<string, object> Player in Rows)
                Output.AddRow(Rows[0]["id"], Rows[0]["name"].ToString().Trim(), Rows[0]["score"]);

            Response.AddData(Output);
            Response.Send();
        }
    }
}
