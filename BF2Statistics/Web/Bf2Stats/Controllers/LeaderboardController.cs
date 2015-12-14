using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BF2Statistics.Database;
using BF2Statistics.Gamespy;

namespace BF2Statistics.Web.Bf2Stats
{
    public class LeaderboardController : Controller
    {
        /// <summary>
        /// Our Model, which holds the variables to be used in the View
        /// </summary>
        protected LeaderboardModel Model;

        public LeaderboardController(HttpClient Client) : base(Client)
        {
            Model = new LeaderboardModel(Client);
        }

        public override void HandleRequest(MvcRoute Route)
        {
            // Get our POST variables
            HttpRequest Request = Client.Request;
            Dictionary<string, string> postParams = Request.GetFormUrlEncodedPostVars();
            int[] pids = new int[0];

            // Fetch our cookie, which contains our PiD's
            Cookie C = Request.Request.Cookies["leaderboard"] ?? new Cookie("leaderboard", "");

            // Convert cookie format into a readable one, and make sure we end with a comma!
            Model.CookieValue = C.Value.Trim().Replace('|', ',');
            if (!Model.CookieValue.EndsWith(","))
                Model.CookieValue += ",";

            // Save Leaderboard
            if (postParams.ContainsKey("set") && postParams.ContainsKey("leaderboard")) // Save cookie
            {
                Model.CookieValue = postParams["leaderboard"];
            }
            else if (Route.Action != "index" && Route.Params.Length != 0)
            {
                switch (Route.Action)
                {
                    case "add":
                        if (Validator.IsValidPID(Route.Params[0]))
                            Model.CookieValue += $"{Route.Params[0]},";
                        break;
                    case "remove":
                        if (Validator.IsValidPID(Route.Params[0]))
                            Model.CookieValue = Model.CookieValue.Replace($"{Route.Params[0]},", "");
                        break;
                    case "list":
                        Model.CookieValue = Route.Params[0];
                        break;
                }
            }

            // Read pids from the cookie
            try
            {
                // Pids are stored as xxxx,yyyyy,zzzz in the cookie
                if (Model.CookieValue.Length > 0)
                {
                    string[] players = Model.CookieValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (players.Length > 0)
                    {
                        pids = Array.ConvertAll(players, Int32.Parse).Distinct().ToArray();
                    }
                }
            }
            catch
            {
                // Bad Cookie value, so flush it!
                Model.CookieValue = "";
                C.Value = String.Empty;
                Client.Response.SetCookie(C);
            }

            // if "get" is POSTED, that means we are generated a URL instead of a cookie
            if (postParams.ContainsKey("get"))
            {
                Client.Response.Redirect(Model.Root + "/myleaderboard/list/" + String.Join(",", pids));
                return;
            }

            // If we have some player ID's, then the leaderboard is not empty
            if (pids.Length > 0)
            {
                // NOTE: The HttpServer will handle the DbConnectException
                using (StatsDatabase Database = new StatsDatabase())
                {
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
                        Model.Players.Add(new PlayerResult
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
                }
            }

            // Finally, set the cookie if we arent viewing from a List
            if (Route.Action != "list")
            {
                Model.CookieValue = String.Join(",", pids);
                C.Value = String.Join("|", pids);
                C.Expires = DateTime.Now.AddYears(1);
                C.Path = "/";
                Client.Response.SetCookie(C);
            }

            // TO prevent null expception in the template
            if (Model.CookieValue == null)
                Model.CookieValue = String.Empty;

            // Set content type
            base.SendTemplateRepsonse("myleaderboard", typeof(LeaderboardModel), Model);
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
