using System;
using System.Collections.Generic;
using System.Globalization;
using BF2Statistics.Database;
using BF2Statistics.Database.QueryBuilder;

namespace BF2Statistics.Web.Bf2Stats
{
    public class RankingsController : Controller
    {
        /// <summary>
        /// The number of players that are shown per page
        /// </summary>
        public const int PlayersPerPage = 50;

        /// <summary>
        /// A static array of all the different ranking sections queries
        /// </summary>
        protected static readonly string[] IndexQueries = new string[]
        {
            "SELECT id, name, rank, score AS value FROM player WHERE score > 0 ORDER BY score DESC LIMIT 5",
            "SELECT id, name, rank, score / (time * 1.0 / 60) AS value FROM player ORDER BY value DESC LIMIT 5",
            "SELECT id, name, rank, (wins * 1.0 / losses) AS value FROM player ORDER BY value DESC LIMIT 5",
            "SELECT id, name, rank, (kills * 1.0 / deaths) AS value FROM player ORDER BY value DESC LIMIT 5",
            "SELECT p.id AS id, name, rank, (w.knifekills * 1.0 / w.knifedeaths) AS value FROM player p JOIN weapons w WHERE p.id = w.id AND w.knifekills > 0 ORDER BY value DESC LIMIT 5", // Knife KDR
            "SELECT p.id AS id, name, rank, (w.hit4 * 1.0 / w.fired4) AS value FROM player p JOIN weapons w WHERE p.id = w.id AND w.fired4 > 100 ORDER BY value DESC LIMIT 5", // SNiper accuracy
            "SELECT id, name, rank, rndscore AS value FROM player WHERE rndscore > 0 ORDER BY value DESC LIMIT 5",
            "SELECT id, name, rank, captures AS value FROM player WHERE captures > 0 ORDER BY value DESC LIMIT 5",
            "SELECT id, name, rank, (captureassists + captures + neutralizes + defends) AS value FROM player ORDER BY value DESC LIMIT 5",
            "SELECT id, name, rank, teamscore AS value FROM player WHERE value > 0 ORDER BY value DESC LIMIT 5",
            "SELECT id, name, rank, (time / 3600.0) / ((lastonline - joined) / 86400.0) AS value FROM player WHERE time > 3600 ORDER BY value DESC LIMIT 5",
            "SELECT id, name, rank, cmdscore AS value FROM player WHERE cmdscore > 0 ORDER BY cmdscore DESC LIMIT 5",
            "SELECT id, name, rank, COALESCE((cmdscore * 1.0 / (cmdtime  / 60)), 0.0) AS value FROM player WHERE (cmdtime > 3600 OR cmdscore > 1000) ORDER BY value DESC LIMIT 5"
        };

        /// <summary>
        /// A static array of stats catagories on the ranking index page
        /// </summary>
        protected static readonly List<KeyValuePair> IndexCatagories = new List<KeyValuePair>()
        {
            new KeyValuePair("Global Score", "&nbsp;"),
            new KeyValuePair("Score Per Minute", "&nbsp;"),
            new KeyValuePair("Win-Loss Ratio", "&nbsp;"),
            new KeyValuePair("Kill-Death Ratio", "&nbsp;"),
            new KeyValuePair("Knife KDR", "(Requires at least 1 kill with the Knife)"),
            new KeyValuePair("Sniper Accuracy", "(Must have more than 100 shots with the Sniper Rifle)"),
            new KeyValuePair("Best Round Score", "&nbsp;"),
            new KeyValuePair("Flag Captures", "&nbsp;"),
            new KeyValuePair("Flag Work", "&nbsp;"),
            new KeyValuePair("Best Teamworkers", "&nbsp;"),
            new KeyValuePair("Hours Per Day", "(Time played > 1 hour)"),
            new KeyValuePair("Command Score", "&nbsp;"),
            new KeyValuePair("Relative Command Score", "(Command score > 1000 OR Command time > 1 hour)")
        };

        /// <summary>
        /// An array of Action names in the URL that are accepted
        /// </summary>
        protected static readonly string[] ActionNames = new string[]
        {
            "score", "spm", "wlr", "kdr", "knife_kdr", "sniper_acc",
            "brs", "fc", "fw", "teamwork", "hpd", "command", "rcmds"
        };

        public RankingsController(HttpClient Client) : base(Client) { }

        public override void HandleRequest(MvcRoute Route)
        {
            // Get our Action
            if (Route.Action == "index" || Array.IndexOf(ActionNames, Route.Action) < 0)
                ShowIndex();
            else
                ShowRankingType(Route);
        }

        private void ShowIndex()
        {
            // Check the cache file
            if (!base.CacheFileExpired("rankings_index", 30))
            {
                base.SendCachedResponse("rankings_index");
                return;
            }

            // Create our model
            RankingsModel Model = new RankingsModel(Client);

            // NOTE: The HttpServer will handle the DbConnectException
            using (StatsDatabase Database = new StatsDatabase())
            {
                // Loop through and add each index catagories, and its players
                for (int i = 0; i < IndexCatagories.Count; i++)
                {
                    Model.Stats.Add(new RankingStats
                    {
                        Name = IndexCatagories[i].Key,
                        Desc = IndexCatagories[i].Value,
                        UrlName = ActionNames[i],
                        TopPlayers = GetTopFromQuery(i, Database)
                    });
                }
            }

            // Send response
            base.SendTemplateRepsonse("rankings_index", typeof(RankingsModel), Model, "Rankings");
        }

        private void ShowRankingType(MvcRoute Route)
        {
            // Create our model
            RankingsTypeModel Model = new RankingsTypeModel(Client);
            Model.UrlName = Route.Action;
            string CacheName = $"rankings_{Route.Action}_1";

            // Parse our country and page filters based on URL
            // Url formats:
            // - scoreType/country/pageNumber
            // - scoreType/pageNumber
            if (Route.Params.Length == 1)
            {
                if (Validator.IsNumeric(Route.Params[0]))
                {
                    // Just a page number provided
                    Model.CurrentPage = Int32.Parse(Route.Params[0]);
                    CacheName = $"rankings_{Route.Action}_{Model.CurrentPage}";
                }
                else if (Route.Params[0].Length == 2)
                {
                    // Just a country code provided, default to page 1
                    Model.Country = Route.Params[0];
                    CacheName = $"rankings_{Route.Action}_{Model.Country}_1";
                }
            }
            else if (Route.Params.Length == 2 && Validator.IsNumeric(Route.Params[1]))
            {
                Model.CurrentPage = Int32.Parse(Route.Params[1]);
                if (Route.Params[0].Length == 2) // Check valid country code
                {
                    Model.Country = Route.Params[0];
                    CacheName = $"rankings_{Route.Action}_{Model.Country}_{Model.CurrentPage}";
                }
                else
                    CacheName = $"rankings_{Route.Action}_{Model.CurrentPage}";
            }

            // Check the cache file
            if (!base.CacheFileExpired(CacheName, 30))
            {
                base.SendCachedResponse(CacheName);
                return;
            }

            // NOTE: The HttpServer will handle the DbConnectException
            using (StatsDatabase Database = new StatsDatabase())
            {
                // Get our DISTINCT country list from our player pool
                SelectQueryBuilder builder = new SelectQueryBuilder(Database);
                builder.SelectColumn("country");
                builder.Distinct = true;
                builder.SelectFromTable("player");
                builder.AddWhere("country", Comparison.NotEqualTo, "xx");
                foreach (var Row in builder.ExecuteQuery())
                    Model.CountryList.Add(Row["country"].ToString());

                // Start building our player query
                builder = new SelectQueryBuilder(Database);
                builder.SelectCount();
                builder.SelectFromTable("player");
                WhereClause Where = builder.AddWhere("score", Comparison.GreaterOrEquals, 1);

                // Add country filter
                if (Model.Country.Length == 2)
                    Where.AddClause(LogicOperator.And, "country", Comparison.Equals, Model.Country);

                // Hpd additional Where
                if (Route.Action.Equals("hpd", StringComparison.InvariantCultureIgnoreCase))
                    Where.AddClause(LogicOperator.And, "time", Comparison.GreaterOrEquals, 3600);

                // Get our total records
                Model.TotalRecords = builder.ExecuteScalar<int>();
                Model.TotalPages = 1 + (Model.TotalRecords / PlayersPerPage);
                Model.ScoreHeader = GetHeaderName(Route.Action);

                // Now, Build Query that will select the players, not just the count
                bool isDecimal = false;
                FinishQuery(Route.Action, builder, out isDecimal);

                // Get our players, limiting to 50 and starting by page
                builder.Limit(PlayersPerPage, (Model.CurrentPage * PlayersPerPage) - PlayersPerPage);
                var Rows = builder.ExecuteQuery();

                // Initialize records based on records returned from Database
                Model.Records = new List<RankingsTypeModel.PlayerRow>(Rows.Count);
                foreach (Dictionary<string, object> Player in Rows)
                {
                    Model.Records.Add(new RankingsTypeModel.PlayerRow()
                    {
                        Pid = Int32.Parse(Player["pid"].ToString()),
                        Name = Player["name"].ToString(),
                        Rank = Int32.Parse(Player["rank"].ToString()),
                        Country = Player["country"].ToString(),
                        Time = Int32.Parse(Player["time"].ToString()),
                        ScorePerMin = Double.Parse(Player["spm"].ToString()),
                        KillDeathRatio = Double.Parse(Player["kdr"].ToString()),
                        WinLossRatio = Double.Parse(Player["wlr"].ToString()),
                        ScoreValue = (isDecimal)
                            ? String.Format(CultureInfo.InvariantCulture, "{0:n4}", Player["value"])
                            : String.Format(CultureInfo.InvariantCulture, "{0:n0}", Player["value"])
                    });
                }
            }

            // Send response
            base.SendTemplateRepsonse("rankings_type", typeof(RankingsTypeModel), Model, CacheName);
        }

        /// <summary>
        /// Converts the url action to a string that is used in the table header
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private string GetHeaderName(string action)
        {
            switch (action.ToLowerInvariant())
            {
                case "score": return "Global Score";
                case "spm": return "Score Per Minute";
                case "wlr": return "Win-Loss Ratio";
                case "kdr": return "Kill-Death Ratio";
                case "knife_kdr": return "Knife KDR";
                case "sniper_acc": return "Sniper Accuracy";
                case "brs": return "Best Round Score";
                case "fc": return "Flag Captures";
                case "fw": return "Flag Work";
                case "btw": return "Best Teamworkers";
                case "command": return "Command Score";
                case "rcmds": return "Relative Command Score";
                case "hpd": return "Hours Per Day";
                default: return String.Empty; 
            }
        }

        /// <summary>
        /// This method is used to fetch an array of columns we need in our detailed rankings
        /// page. It also returns in 2 out variables additional data we need to adjust our query
        /// </summary>
        /// <param name="action">The Action from our MvcRoute</param>
        /// <param name="valueIsDecimal">Indicates to the calling method if the Type of return "value" is a decimal</param>
        private void FinishQuery(string action, SelectQueryBuilder builder, out bool valueIsDecimal)
        {
            // Generate our list of columns
            List<string> Cols = new List<string>()
            {
                "player.id AS pid",
                "name",
                "rank",
                "time",
                "country",
                "score / (time * 1.0 / 60) AS spm",
                "(wins * 1.0 / losses) AS wlr",
                "(kills * 1.0 / deaths) AS kdr",
            };

            // Value Type
            valueIsDecimal = false;

            // Additional function
            switch (action.ToLowerInvariant())
            {
                case "score":
                    Cols.Add("score AS value");
                    break;
                case "spm":
                    Cols.Add("score / (time * 1.0 / 60) AS value");
                    valueIsDecimal = true;
                    break;
                case "wlr":
                    Cols.Add("(wins * 1.0 / losses) AS value");
                    valueIsDecimal = true;
                    break;
                case "kdr":
                    Cols.Add("(kills * 1.0 / deaths) AS value");
                    valueIsDecimal = true;
                    break;
                case "knife_kdr":
                    Cols.Add("(weapons.knifekills * 1.0 / weapons.knifedeaths) AS value");
                    builder.AddJoin(JoinType.InnerJoin, "weapons", "id", Comparison.Equals, "player", "id");
                    valueIsDecimal = true;
                    break;
                case "sniper_acc":
                    Cols.Add("(weapons.hit4 * 1.0 / weapons.fired4) AS value");
                    valueIsDecimal = true;
                    builder.AddJoin(JoinType.InnerJoin, "weapons", "id", Comparison.Equals, "player", "id");
                    break;
                case "brs":
                    Cols.Add("rndscore AS value");
                    break;
                case "fc":
                    Cols.Add("captures AS value");
                    break;
                case "fw":
                    Cols.Add("(captureassists + captures + neutralizes + defends) AS value");
                    break;
                case "btw":
                    Cols.Add("teamscore AS value");
                    break;
                case "hpd":
                    // Hours Per Day 
                    // Timeframe: ((Last Online timstamp - Joined Timestamp) / 1 day (86400 seconds)) Gets the timespan of days played
                    // Divide Timeframe by: hours played (seconds played (`time` column) / 1 hr (3600 seconds))
                    Cols.Add("(time / 3600.0) / ((lastonline - joined) / 86400.0) AS value");
                    valueIsDecimal = true;
                    break;
                case "command":
                    Cols.Add("cmdscore AS value");
                    break;
                case "rcmds":
                    Cols.Add("COALESCE((cmdscore * 1.0 / (cmdtime  / 60)), 0.0) AS value");
                    valueIsDecimal = true;
                    break;
            }

            builder.SelectColumns(Cols.ToArray());
            //builder.WhereStatement.Add("value", Comparison.GreaterThan, 0);
            builder.AddOrderBy("value", Sorting.Descending);
        }

        /// <summary>
        /// Returns the page number that the players stat would appear
        /// </summary>
        /// <param name="index">The player position</param>
        /// <returns></returns>
        public int GetPage(int index) => 1 + (index / PlayersPerPage);

        /// <summary>
        /// Processes the query ID and returns the results
        /// </summary>
        /// <param name="id">The Query id</param>
        /// <param name="Database">The stats database connection</param>
        /// <returns></returns>
        protected List<Player> GetTopFromQuery(int id, StatsDatabase Database)
        {
            List<Player> Players = new List<Player>(5);
            var Rows = Database.Query(IndexQueries[id]);
            for (int i = 0; i < 5; i++)
            {
                if (i < Rows.Count)
                {
                    double ds = Double.Parse(Rows[i]["value"].ToString());
                    string Val = ((ds % 1) != 0) ? Math.Round(ds, 4).ToString() : FormatNumber(ds);

                    Players.Add(new Player
                    {
                        Pid = Int32.Parse(Rows[i]["id"].ToString()),
                        Name = Rows[i]["name"].ToString(),
                        Rank = Int32.Parse(Rows[i]["rank"].ToString()),
                        Value = Val
                    });
                }
                else
                {
                    Players.Add(new Player { Name = "" });
                }
            }

            return Players;
        }

        public string FormatNumber(object Num)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:n0}", Int32.Parse(Num.ToString()));
        }
    }
}
