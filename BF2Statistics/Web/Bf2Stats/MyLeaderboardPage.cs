using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BF2Statistics.Database;

namespace BF2Statistics.Web.Bf2Stats
{
    partial class MyLeaderboardPage
    {
        /// <summary>
        /// The page title
        /// </summary>
        public string Title = Program.Config.BF2S_Title;

        /// <summary>
        /// The HttpClient Object
        /// </summary>
        public HttpClient Client;

        /// <summary>
        /// The Root URL used for links
        /// </summary>
        protected string Root;

        /// <summary>
        /// List of leaderboard players
        /// </summary>
        protected List<PlayerResult> Players = new List<PlayerResult>();

        /// <summary>
        /// The value of the cookie
        /// </summary>
        protected string CookieValue = String.Empty;

        public MyLeaderboardPage(HttpClient Client, StatsDatabase Database)
        {
            // Fetch cookie
            this.Client = Client;
            this.Root = "http://" + this.Client.Request.Url.DnsSafeHost + "/bf2stats";

            // Get our POST variables
            Dictionary<string, string> postParams = Client.Request.GetFormUrlEncodedPostVars();
            int[] pids = new int[0]; 

            // Fetch our cookie, which contains our PiD's
            Cookie C = Client.Request.Request.Cookies["leaderboard"];
            if (C == null) C = new Cookie("leaderboard", "");

            // Convert cookie format into a readable one, and make sure we end with a comma!
            CookieValue = C.Value.Trim().Replace('|', ',');
            if (!CookieValue.EndsWith(","))
                CookieValue += ",";

            // Adding player IDS
            if (Client.Request.QueryString.ContainsKey("add"))
            {
                CookieValue += Client.Request.QueryString["add"] + ",";
            }

            // Removing Player IDS
            if (Client.Request.QueryString.ContainsKey("remove"))
            {
                CookieValue = CookieValue.Replace(Client.Request.QueryString["remove"] + ",", "");
            }

            // Generating a share list (Does not use cookies)
            if (Client.Request.QueryString.ContainsKey("list"))
            {
                CookieValue = Client.Request.QueryString["list"];
            }

            // Save Leaderboard
            if (postParams.ContainsKey("set") && postParams.ContainsKey("leaderboard")) // Save cookie
            {
                CookieValue = postParams["leaderboard"];
            }

            // Read pids from the cookie
            try
            {
                // Pids are stored as xxxx,yyyyy,zzzz in the cookie
                if (CookieValue.Length > 0)
                {
                    string[] players = CookieValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (players.Length > 0)
                    {
                        pids = Array.ConvertAll(players, Int32.Parse).Distinct().ToArray();
                    }
                }
            }
            catch { }

            // if "get" is POSTED, that means we are generated a URL instead of a cookie
            if (postParams.ContainsKey("get"))
            {
                Client.Response.Redirect(this.Root + "/myleaderboard?list=" + String.Join(",", pids));
                return;
            }

            // If we have some player ID's, then the leaderboard is not empty
            if (pids.Length > 0)
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

                    // Add Result
                    Players.Add(new PlayerResult
                    {
                        Pid = Int32.Parse(Row["id"].ToString()),
                        Name = Row["name"].ToString(),
                        Score = (int)Score,
                        Rank = Int32.Parse(Row["rank"].ToString()),
                        TimePlayed = FormatTime(Int32.Parse(Row["time"].ToString())),
                        LastOnline = FormatDate(Row["lastonline"]),
                        Country = Row["country"].ToString().ToUpperInvariant(),
                        Kdr = Kdr,
                        Spm = SPM
                    });
                }
            }

            // Finally, set the cookie if we arent viewing from a List
            if (!Client.Request.QueryString.ContainsKey("list"))
            {
                CookieValue = String.Join(",", pids);
                C.Value = String.Join("|", pids);
                C.Expires = DateTime.Now.AddYears(1);
                //C.Domain = this.Root;
                Client.Response.SetCookie(C);
            }

            // TO prevent null expception in the template
            if (CookieValue.Length == 0)
                CookieValue = " ";

            // Set content type
            Client.Response.ContentType = "text/html";
        }

        public string FormatTime(int Time)
        {
            TimeSpan Span = TimeSpan.FromSeconds(Time);
            return String.Format("{0:00}:{1:00}:{2:00}", Span.TotalHours, Span.Minutes, Span.Seconds);
        }

        public string FormatDate(object Time)
        {
            DateTime T = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (T.AddSeconds(Int32.Parse(Time.ToString()))).ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
