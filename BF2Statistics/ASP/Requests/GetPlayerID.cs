using System;
using System.Collections.Generic;
using BF2Statistics.Database;

namespace BF2Statistics.ASP.Requests
{
    class GetPlayerID
    {
        /// <summary>
        /// To prevent the chance of 2 peeps getting the same PID
        /// we will just load this on startup
        /// </summary>
        public static int LowestPid;

        /// <summary>
        /// Grab lowest PID on startup
        /// </summary>
        static GetPlayerID()
        {
            // Get the lowest PID from the database
            int DefaultPid = MainForm.Config.ASP_DefaultPID;
            List<Dictionary<string, object>> Rows = ASPServer.Database.Driver.Query("SELECT MIN(id) AS min FROM player");
            if (Rows.Count == 0 || String.IsNullOrWhiteSpace(Rows[0]["min"].ToString()) || (Int32.Parse(Rows[0]["min"].ToString()) > DefaultPid))
                LowestPid = DefaultPid;
            else
                LowestPid = Int32.Parse(Rows[0]["min"].ToString()) - 1;
        }

        public GetPlayerID(HttpClient Client)
        {
            // Setup Variables
            DatabaseDriver Driver = ASPServer.Database.Driver;
            List<Dictionary<string, object>> Rows;
            Dictionary<string, string> QueryString = Client.Request.QueryString;

            // Querystring vars
            int IsAI = 0;
            int ListPlayers = 0;
            string PlayerNick = "";

            // Setup Params
            if (QueryString.ContainsKey("nick"))
                PlayerNick = QueryString["nick"].Replace("%20", " ");
            if (QueryString.ContainsKey("ai"))
                Int32.TryParse(QueryString["ai"], out IsAI);
            if (QueryString.ContainsKey("playerlist"))
                Int32.TryParse(QueryString["playerlist"], out ListPlayers);

            // Handle Request
            if (!String.IsNullOrWhiteSpace(PlayerNick))
            {
                int Pid;

                // Create player if they donot exist
                Rows = Driver.Query("SELECT id FROM player WHERE name = @P0 LIMIT 1", PlayerNick);
                if (Rows.Count == 0)
                {
                    // Grab new Player ID
                    Pid = LowestPid--;

                    // Create Player
                    Driver.Execute(
                        "INSERT INTO player(id, name, joined, isbot) VALUES(@P0, @P1, @P2, @P3)",
                        Pid, PlayerNick, DateTime.UtcNow.ToUnixTimestamp(), IsAI
                    );

                    // Create Player Unlock Data
                    string Query = "INSERT INTO unlocks VALUES ";
                    for (int i = 11; i < 100; i += 11)
                        Query += String.Format("({0}, {1}, 'n'), ", Pid, i);
                    for (int i = 111; i < 556; i += 111)
                        Query += String.Format("({0}, {1}, 'n'), ", Pid, i);
                    Driver.Execute(Query.TrimEnd(new char[] { ',', ' ' }));
                }
                else
                    Pid = Int32.Parse(Rows[0]["id"].ToString());

                // Send Response
                Client.Response.WriteResponseStart();
                Client.Response.WriteHeaderLine("pid");
                Client.Response.WriteDataLine(Pid);
            }
            else if (ListPlayers != 0)
            {
                // Prepare Response
                Client.Response.WriteResponseStart();
                Client.Response.WriteHeaderLine("pid");

                // Fetch Players
                Rows = Driver.Query("SELECT id FROM player WHERE ip <> '127.0.0.1'");
                foreach (Dictionary<string, object> Player in Rows)
                    Client.Response.WriteDataLine(Player["id"]);
            }
            else
            {
                Client.Response.WriteResponseStart(false);
                Client.Response.WriteHeaderLine("asof", "err");
                Client.Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp(), "Invalid Syntax!");
            }

            // Send Response
            Client.Response.Send();
        }
    }
}
