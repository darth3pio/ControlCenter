using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BF2Statistics.Database;

namespace BF2Statistics.Web.Bf2Stats
{
    partial class SearchPage
    {
        /// <summary>
        /// The page title
        /// </summary>
        public string Title = MainForm.Config.BF2S_Title;

        /// <summary>
        /// The HttpClient Object
        /// </summary>
        public HttpClient Client;

        /// <summary>
        /// The Root URL used for links
        /// </summary>
        protected string Root;

        /// <summary>
        /// Our search string if one is posted
        /// </summary>
        protected string SearchValue = "";

        /// <summary>
        /// If we have a search string. The results are stored here
        /// </summary>
        protected List<PlayerResult> SearchResults = new List<PlayerResult>();

        public SearchPage(HttpClient Client, StatsDatabase Database)
        {
            this.Client = Client;
            this.Root = "http://" + this.Client.Request.Url.DnsSafeHost + "/bf2stats";
            Dictionary<string, string> postParams = Client.Request.GetFormUrlEncodedPostVars();

            // If we have a search value, run it
            if (postParams.ContainsKey("searchvalue"))
            {
                this.SearchValue = postParams["searchvalue"];
                List<Dictionary<string, object>> Rows;

                // Do processing
                if (Validator.IsNumeric(SearchValue))
                {
                    Rows = Database.Query(
                        "SELECT id, name, score, time, country, rank, lastonline, kills, deaths FROM player WHERE id LIKE @P0 LIMIT 50",
                        "%" + SearchValue + "%"
                    );
                }
                else
                {
                    Rows = Database.Query(
                        "SELECT id, name, score, time, country, rank, lastonline, kills, deaths FROM player WHERE name LIKE @P0 LIMIT 50",
                        SearchValue
                    );
                }

                // Loop through each result, and process
                foreach (Dictionary<string, object> Row in Rows)
                {
                    // DO Kill Death Ratio
                    double Kills = Int32.Parse(Row["kills"].ToString());
                    double Deaths = Int32.Parse(Row["deaths"].ToString());
                    double Kdr = (Deaths > 0) ? Math.Round(Kills / Deaths, 3): Kills;

                    // Get Score Per Min
                    double Score = Int32.Parse(Row["score"].ToString());
                    double Mins = Int32.Parse(Row["time"].ToString()) / 60;
                    double SPM = (Mins > 0) ? Math.Round(Score / Mins, 4) : Score;

                    // Add Result
                    SearchResults.Add(new PlayerResult
                    {
                        Pid = Int32.Parse(Row["id"].ToString()),
                        Name = Row["name"].ToString(),
                        Score = (int) Score,
                        Rank = Int32.Parse(Row["rank"].ToString()),
                        TimePlayed = FormatTime(Int32.Parse(Row["time"].ToString())), 
                        LastOnline = FormatDate(Row["lastonline"]),
                        Country = Row["country"].ToString().ToUpperInvariant(),
                        Kdr = Kdr,
                        Spm = SPM
                    });
                }
            }

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
