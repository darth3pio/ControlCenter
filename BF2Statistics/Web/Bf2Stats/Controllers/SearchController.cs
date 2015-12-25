using System;
using System.Collections.Generic;
using BF2Statistics.Database;

namespace BF2Statistics.Web.Bf2Stats
{
    public class SearchController : Controller
    {
        /// <summary>
        /// Our Model, which holds the variables to be used in the View
        /// </summary>
        protected SearchModel Model;

        public SearchController(HttpClient Client) : base(Client)
        {
            Model = new SearchModel(Client);
        }

        public override void HandleRequest(MvcRoute Route)
        {
            // Get our POST'ed parameters
            Dictionary<string, string> postParams = Client.Request.GetFormUrlEncodedPostVars();

            // If we have a search value, run it
            if (postParams.ContainsKey("searchvalue"))
            {
                // NOTE: The HttpServer will handle the DbConnectException
                using (StatsDatabase Database = new StatsDatabase())
                {
                    Model.SearchValue = postParams["searchvalue"].Replace("+", " ");
                    List<Dictionary<string, object>> Rows;

                    // Do processing
                    if (Validator.IsNumeric(Model.SearchValue))
                    {
                        if (Validator.IsValidPID(Model.SearchValue))
                        {
                            // If player PID exists, redirect there
                            bool exists = Database.ExecuteScalar<bool>("SELECT COUNT(id) FROM player WHERE id=@P0", Model.SearchValue);
                            if (exists)
                            {
                                Client.Response.Redirect("/bf2stats/player/" + Model.SearchValue);
                                return;
                            }
                        }
                        
                        Rows = Database.Query(
                            "SELECT id, name, score, time, country, rank, lastonline, kills, deaths FROM player WHERE id LIKE @P0 LIMIT 50",
                            "%" + Model.SearchValue + "%"
                        );
                    }
                    else
                    {
                        // Check to see if player with this name exists
                        Rows = Database.Query("SELECT id FROM player WHERE name=@P0 LIMIT 1", Model.SearchValue);
                        if (Rows.Count > 0)
                        {
                            Client.Response.Redirect("/bf2stats/player/" + Rows[0]["id"]);
                            return;
                        }

                        Rows = Database.Query(
                            "SELECT id, name, score, time, country, rank, lastonline, kills, deaths FROM player WHERE name LIKE @P0 LIMIT 50",
                            Model.SearchValue
                        );
                    }

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
                        Model.SearchResults.Add(new PlayerResult
                        {
                            Pid = Int32.Parse(Row["id"].ToString()),
                            Name = Row["name"].ToString(),
                            Score = (int)Score,
                            Rank = Int32.Parse(Row["rank"].ToString()),
                            TimePlayed = FormatTime(Int32.Parse(Row["time"].ToString())),
                            LastOnline = FormatDate(Int32.Parse(Row["lastonline"].ToString())),
                            Country = Row["country"].ToString().ToUpperInvariant(),
                            Kdr = Kdr,
                            Spm = SPM
                        });
                    }
                }
            }

            // Send response
            base.SendTemplateResponse("search", typeof(SearchModel), Model);
        }
    }
}
