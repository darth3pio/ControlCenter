using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BF2Statistics.Database;

namespace BF2Statistics.ASP.Requests
{
    class GetPlayerID
    {
        public GetPlayerID(ASPResponse Response, Dictionary<string, string> QueryString)
        {
            // Setup Variables
            DatabaseDriver Driver = ASPServer.Database.Driver;
            List<Dictionary<string, object>> Rows;
            FormattedOutput Output = new FormattedOutput("pid");

            // Querystring vars
            int IsAI = 0;
            int ListPlayers = 0;
            string PlayerNick = "";

            // Setup Params
            if (QueryString.ContainsKey("nick"))
                PlayerNick = DatabaseDriver.Escape(QueryString["nick"].Replace("%20", " "));
            if (QueryString.ContainsKey("ai"))
                Int32.TryParse(QueryString["ai"], out IsAI);
            if (QueryString.ContainsKey("playerlist"))
                Int32.TryParse(QueryString["playerlist"], out ListPlayers);

            // Handle Request
            if (!String.IsNullOrWhiteSpace(PlayerNick))
            {
                int Pid;

                // Create player if they donot exist
                Rows = Driver.Query("SELECT id FROM player WHERE name = '{0}' LIMIT 1", PlayerNick);
                if (Rows.Count == 0)
                {
                    int DefaultPid = MainForm.Config.ASP_DefaultPID;
                    Rows = null;

                    // Get the lowest PID from the database
                    Rows = Driver.Query("SELECT MIN(id) AS min FROM player");
                    if (Rows.Count == 0 || (Int32.Parse(Rows[0]["min"].ToString()) > DefaultPid))
                        Pid = DefaultPid;
                    else
                        Pid = Int32.Parse(Rows[0]["min"].ToString()) - 1;

                    // Create Player
                    Driver.Execute(
                        "INSERT INTO player(id, name, joined, isbot) VALUES({0}, '{1}', {2}, {3})",
                        Pid, PlayerNick, Utils.UnixTimestamp(), IsAI
                    );

                    // Create Player Unlock Data
                    string Query = "INSERT INTO unlocks VALUES ";
                    for (int i = 11; i < 100; i += 11)
                        Query += String.Format("({0}, {1}, 'n'), ", Pid, i);
                    for (int i = 111; i < 556; i += 111)
                        Query += String.Format("({0}, {1}, 'n'), ", Pid, i);
                    Driver.Execute(Query.TrimEnd( new char[] { ',', ' '} ));
                }
                else
                    Pid = Int32.Parse(Rows[0]["id"].ToString());

                // Send Response
                Output.AddRow(Pid);
            }
            else if (ListPlayers != 0)
            {
                Rows = Driver.Query("SELECT id FROM player WHERE ip <> '127.0.0.1'");
                foreach (Dictionary<string, object> Player in Rows)
                    Output.AddRow(Player["id"]);
            }
            else
            {
                Output = new FormattedOutput("asof", "err");
                Output.AddRow(Utils.UnixTimestamp(), "Invalid Syntax");
                Response.IsValidData(false);
            }

            Response.AddData(Output);
            Response.Send();
        }
    }
}
