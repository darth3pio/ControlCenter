using System;
using System.Collections.Generic;
using BF2Statistics.Database;

namespace BF2Statistics.Web.ASP
{
    class SearchForPlayers
    {
        public SearchForPlayers(HttpClient Client, StatsDatabase Driver)
        {
            string Nick;
            List<Dictionary<string, object>> Rows;
            ASPResponse Response = Client.Response as ASPResponse;

            // Setup Params
            if (!Client.Request.QueryString.ContainsKey("nick"))
            {
                Response.WriteResponseStart(false);
                Response.WriteHeaderLine("asof", "err");
                Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp(), "Invalid Syntax!");
                Response.Send();
                return;
            }
            else
                Nick = Client.Request.QueryString["nick"];

            // Timestamp Header
            Response.WriteResponseStart();
            Response.WriteHeaderLine("asof");
            Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp());

            // Output status
            int i = 0;
            Response.WriteHeaderLine("n", "pid", "nick", "score");
            Rows = Driver.Query("SELECT id, name, score FROM player WHERE name LIKE @P0 LIMIT 20", "%" + Nick + "%");
            foreach (Dictionary<string, object> Player in Rows)
            {
                Response.WriteDataLine(i + 1, Rows[i]["id"], Rows[i]["name"].ToString().Trim(), Rows[i]["score"]);
                i++;
            }

            // Send Response
            Response.Send();
        }
    }
}
