using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BF2Statistics.Database;
using BF2Statistics.Gamespy;

namespace BF2Statistics.Web.Bf2Stats
{
    public class IndexController : Controller
    {
        /// <summary>
        /// Our Model, which holds the variables to be used in the View
        /// </summary>
        protected IndexModel Model;

        public IndexController(HttpClient Client) : base(Client)
        {
            Model = new IndexModel(Client);
        }

        public override void HandleRequest(MvcRoute Route)
        {
            // NOTE: The HttpServer will handle the DbConnectException
            using (StatsDatabase Database = new StatsDatabase())
            {
                if (Program.Config.BF2S_HomePageType == HomePageType.Leaderboard)
                {
                    // Fetch our player list
                    Model.Players = Database.Query(
                        "SELECT id, name, rank, score, kills, country, time FROM player WHERE score > 0 ORDER BY score DESC LIMIT "
                        + Program.Config.BF2S_LeaderCount
                    );

                    // Send response
                    base.SendTemplateRepsonse("index_leaderboard", typeof(IndexModel), Model);
                }
                else
                {
                    // Fetch our top 10 player list
                    Model.Players = Database.Query("SELECT id, name, score FROM player WHERE score > 0 ORDER BY score DESC LIMIT 10");

                    // Fetch our cookie, which contains our personal leaderboard Pid's
                    int[] pids = new int[0];
                    HttpRequest Request = Client.Request;
                    Cookie C = Request.Request.Cookies["leaderboard"] ?? new Cookie("leaderboard", "");

                    // Read pids from the cookie
                    try
                    {
                        // Pids are stored as xxxx|yyyyy|zzzz in the cookie
                        if (C.Value.Length > 0)
                        {
                            string[] players = C.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                            if (players.Length > 0)
                            {
                                pids = Array.ConvertAll(players, Int32.Parse).Distinct().ToArray();
                            }
                        }
                    }
                    catch
                    {
                        // Bad Cookie value, so flush it!
                        C.Value = String.Empty;
                        C.Path = "/";
                        Client.Response.SetCookie(C);
                    }

                    var Rows = Database.Query(
                        String.Format("SELECT id, name, score, time, country, rank, lastonline, kills, deaths FROM player WHERE id IN ({0})", String.Join(",", pids)
                    ));

                    // Loop through each result, and process
                    foreach (Dictionary<string, object> Row in Rows)
                    {
                        // DO Kill Death Ratio
                        double Kills = Int32.Parse(Row["kills"].ToString());
                        double Deaths = Int32.Parse(Row["deaths"].ToString());
                        double Kdr = (Deaths > 0) ? Math.Round(Kills / Deaths, 3) : Kills;

                        // Get Score Per Min
                        double Score = Int32.Parse(Row["score"].ToString());
                        double Mins = Int32.Parse(Row["time"].ToString()) / 60;
                        double SPM = (Mins > 0) ? Math.Round(Score / Mins, 4) : Score;

                        int Pid = Int32.Parse(Row["id"].ToString());

                        // Add Result
                        Model.MyLeaderboardPlayers.Add(new PlayerResult
                        {
                            Pid = Pid,
                            Name = Row["name"].ToString(),
                            Score = (int)Score,
                            Rank = Int32.Parse(Row["rank"].ToString()),
                            TimePlayed = FormatTime(Int32.Parse(Row["time"].ToString())),
                            LastOnline = FormatDate(Int32.Parse(Row["lastonline"].ToString())),
                            Country = Row["country"].ToString().ToUpperInvariant(),
                            Kdr = Kdr,
                            Spm = SPM,
                            Status = GetOnlineStatus(Pid)
                        });
                    }

                    // Send response
                    base.SendTemplateRepsonse("index", typeof(IndexModel), Model);
                }
            }
        }

        private string GetOnlineStatus(int Pid)
        {
            if (!GamespyEmulator.IsRunning)
                return "offline";
            else
                return (GamespyEmulator.IsPlayerConnected(Pid)) ? "online" : "offline";
        }
    }
}
