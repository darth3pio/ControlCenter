using System;
using System.Collections.Generic;
using BF2Statistics.Database;

namespace BF2Statistics.ASP.Requests
{
    class SearchForPlayers
    {
        public SearchForPlayers(HttpClient Client, StatsDatabase Driver)
        {
            string Nick;
            List<Dictionary<string, object>> Rows;

            // Setup Params
            if (!Client.Request.QueryString.ContainsKey("nick"))
            {
                Client.Response.WriteResponseStart(false);
                Client.Response.WriteHeaderLine("asof", "err");
                Client.Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp(), "Invalid Syntax!");
                Client.Response.Send();
                return;
            }
            else
                Nick = Client.Request.QueryString["nick"];

            // Timestamp Header
            Client.Response.WriteResponseStart();
            Client.Response.WriteHeaderLine("asof");
            Client.Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp());

            // Output status
            int i = 0;
            Client.Response.WriteHeaderLine("n", "pid", "nick", "score");
            Rows = Driver.Query("SELECT id, name, score FROM player WHERE name LIKE @P0 LIMIT 20", "%" + Nick + "%");
            foreach (Dictionary<string, object> Player in Rows)
            {
                Client.Response.WriteDataLine(i + 1, Rows[i]["id"], Rows[i]["name"].ToString().Trim(), Rows[i]["score"]);
                i++;
            }

            // Send Response
            Client.Response.Send();
        }
    }
}
