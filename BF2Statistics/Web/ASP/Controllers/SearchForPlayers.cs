using System;
using System.Collections.Generic;
using BF2Statistics.Database;

namespace BF2Statistics.Web.ASP
{
    /// <summary>
    /// /ASP/searchforplayers.aspx
    /// </summary>
    public sealed class SearchForPlayers : ASPController
    {
        public SearchForPlayers(HttpClient Client) : base(Client) { }

        public override void HandleRequest()
        {
            // Setup Params
            if (!Request.QueryString.ContainsKey("nick"))
            {
                Response.WriteResponseStart(false);
                Response.WriteHeaderLine("asof", "err");
                Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp(), "Invalid Syntax!");
                Response.Send();
                return;
            }

            // NOTE: The HttpServer will handle the DbConnectException
            using (Database = new StatsDatabase())
            {
                // Setup local vars
                int i = 0;
                string Nick = Request.QueryString["nick"];
                List<Dictionary<string, object>> Rows;

                // Timestamp Header
                Response.WriteResponseStart();
                Response.WriteHeaderLine("asof");
                Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp());

                // Output status
                Response.WriteHeaderLine("n", "pid", "nick", "score");
                Rows = Database.Query("SELECT id, name, score FROM player WHERE name LIKE @P0 LIMIT 20", "%" + Nick + "%");
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
}
