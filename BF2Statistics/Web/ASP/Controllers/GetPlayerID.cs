using System;
using System.Collections.Generic;
using System.Text;
using BF2Statistics.ASP;
using BF2Statistics.Database;

namespace BF2Statistics.Web.ASP
{
    /// <summary>
    /// /ASP/getplayerid.aspx
    /// </summary>
    public sealed class GetPlayerID : ASPController
    {
        /// <summary>
        /// This request provides details on a particular players rank, and
        /// whether or not to show the user a promotion/demotion announcement
        /// </summary>
        /// <queryParam name="nick" type="string">The unique player's Name</queryParam>
        /// <queryParam name="ai" type="int">Defines whether the player is a bot (used by bf2server)</queryParam>
        /// <queryParam name="playerlist" type="int">Defines whether to list the players who's nick is similair to the Nick param</queryParam>
        /// <param name="Client">The HttpClient who made the request</param>
        public GetPlayerID(HttpClient Client) : base(Client) { }

        public override void HandleRequest()
        {
            // Setup Variables
            List<Dictionary<string, object>> Rows;
            Dictionary<string, string> QueryString = Request.QueryString;

            // Querystring vars
            int IsAI = 0;
            int ListPlayers = 0;
            string PlayerNick = "";

            // Setup Params
            if (QueryString.ContainsKey("nick"))
                PlayerNick = Uri.UnescapeDataString(QueryString["nick"].Replace("%20", " "));
            if (QueryString.ContainsKey("ai"))
                Int32.TryParse(QueryString["ai"], out IsAI);
            if (QueryString.ContainsKey("playerlist"))
                Int32.TryParse(QueryString["playerlist"], out ListPlayers);

            // NOTE: The HttpServer will handle the DbConnectException
            using (Database = new StatsDatabase())
            {
                // Handle Request
                if (!String.IsNullOrWhiteSpace(PlayerNick))
                {
                    int Pid;

                    // Create player if they donot exist
                    Rows = Database.Query("SELECT id FROM player WHERE name = @P0 LIMIT 1", PlayerNick);
                    if (Rows.Count == 0)
                    {
                        // Grab new Player ID using thread safe methods
                        Pid = (IsAI > 0) ? StatsManager.GenerateNewAIPid() : StatsManager.GenerateNewPlayerPid();

                        // Create Player
                        Database.Execute(
                            "INSERT INTO player(id, name, joined, isbot) VALUES(@P0, @P1, @P2, @P3)",
                            Pid, PlayerNick, DateTime.UtcNow.ToUnixTimestamp(), IsAI
                        );
                    }
                    else
                        Pid = Int32.Parse(Rows[0]["id"].ToString());

                    // Send Response
                    Response.WriteResponseStart();
                    Response.WriteHeaderLine("pid");
                    Response.WriteDataLine(Pid);
                }
                else if (ListPlayers != 0)
                {
                    // Prepare Response
                    Response.WriteResponseStart();
                    Response.WriteHeaderLine("pid");

                    // Fetch Players
                    Rows = Database.Query("SELECT id FROM player WHERE isbot=0 LIMIT 1000");
                    foreach (Dictionary<string, object> Player in Rows)
                        Response.WriteDataLine(Player["id"]);
                }
                else
                {
                    Response.WriteResponseStart(false);
                    Response.WriteHeaderLine("asof", "err");
                    Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp(), "Invalid Syntax!");
                }

                // Send Response
                Response.Send();
            }
        }
    }
}
