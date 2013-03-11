using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BF2Statistics.Database;

namespace BF2Statistics.ASP.Requests
{
    class GetLeaderBoard
    {
        private ASPResponse Response;
        private Dictionary<string, string> QueryString;
        private DatabaseDriver Driver;

        // Needed Params
        private string Id = "";
        private int Pid = 0;

        // Optional Params
        private int Before = 0;
        private int After = 0;
        private int Pos = 1;
        private int Min;
        private int Max;

        public GetLeaderBoard(ASPResponse Response, Dictionary<string, string> QueryString)
        {
            // Set internal variables
            this.Response = Response;
            this.QueryString = QueryString;
            this.Driver = ASPServer.Database.Driver;

            // We need a type!
            if (!QueryString.ContainsKey("type"))
            {
                FormattedOutput Output = new FormattedOutput("asof", "err");
                Output.AddRow(Utils.UnixTimestamp(), "Invalid Syntax!");
                Response.AddData(Output);
                Response.IsValidData(false);
                Response.Send();
                return;
            }

            // Setup Params
            if(QueryString.ContainsKey("pid"))
                Int32.TryParse(QueryString["pid"], out Pid);
            if (QueryString.ContainsKey("id"))
                Id = QueryString["id"];
            if (QueryString.ContainsKey("before"))
                Int32.TryParse(QueryString["before"], out Before);
            if(QueryString.ContainsKey("after"))
                Int32.TryParse(QueryString["after"], out After);
            if (QueryString.ContainsKey("pos"))
                Int32.TryParse(QueryString["pos"], out Pos);

            Min = (Pos - 1) - Before;
            Max = After + 1;

            // Do our requested Task
            switch (QueryString["type"])
            {
                case "score":
                    DoScore();
                    break;
                case "risingstar":
                    DoRisingStar();
                    break;
                case "kit":
                    DoKit();
                    break;
                case "vehicle":
                    DoVehicles();
                    break;
                case "weapon":
                    DoWeapons();
                    break;
                default:
                    //Response.HTTPStatusCode = ASPResponse.HTTPStatus.BadRequest;
                    Response.Send();
                    break;
            }
        }

        private void DoScore()
        {
            // Make sure we have a score sub type
            if (String.IsNullOrWhiteSpace(Id))
                return;

            // Prepare Output
            FormattedOutput Output = new FormattedOutput("size", "asof");
            List<Dictionary<string, object>> Rows;
            int Count;

            if (Id == "overall")
            {
                // Get Player count with a score
                Rows = Driver.Query("SELECT COUNT(id) AS count FROM player WHERE score > 0");
                Count = Int32.Parse(Rows[0]["count"].ToString());
                Output.AddRow(Count, Utils.UnixTimestamp());
                Response.AddData(Output);

                // Build New Header Output
                Output = new FormattedOutput("n", "pid", "nick", "score", "totaltime", "playerrank", "countrycode");
                if (Count == 0)
                {
                    Response.AddData(Output);
                    Response.Send();
                    return;
                }

                if (Pid == 0)
                {
                    string Query = "SELECT id, name, rank, country, time, score FROM player WHERE score > 0 ORDER BY score DESC, name DESC LIMIT {0}, {1}";
                    Rows = Driver.Query(Query, Min, Max);
                    foreach (Dictionary<string, object> Player in Rows)
                    {
                        Output.AddRow(
                            Pos++,
                            Player["id"],
                            Player["name"].ToString().Trim(),
                            Player["score"],
                            Player["time"],
                            Player["rank"],
                            Player["country"].ToString().ToUpper()
                        );
                    }
                }
                else
                {
                    // Get Player Position
                    string Query = "SELECT id, name, rank, country, time, score FROM player WHERE score > 0 ORDER BY score DESC, name DESC";
                    Rows = Driver.Query(Query);
                    foreach (Dictionary<string, object> Player in Rows)
                    {
                        if (Int32.Parse(Player["id"].ToString()) == Pid)
                        {
                            Output.AddRow(
                                Pos,
                                Player["id"],
                                Player["name"].ToString().Trim(),
                                Player["score"],
                                Player["time"],
                                Player["rank"],
                                Player["country"].ToString().ToUpper()
                            );
                            break;
                        }
                        Pos++;
                    }
                }

                Response.AddData(Output);
                Response.Send();
            }
            else if (Id == "commander")
            {
                Rows = Driver.Query("SELECT COUNT(id) AS count FROM player WHERE cmdscore > 0");
                Count = Int32.Parse(Rows[0]["count"].ToString());
                Output.AddRow(Count, Utils.UnixTimestamp());
                Response.AddData(Output);

                // Build New Header Output
                Output = new FormattedOutput("n", "pid", "nick", "coscore", "cotime", "playerrank", "countrycode");
                if (Count == 0)
                {
                    Response.AddData(Output);
                    Response.Send();
                    return;
                }

                if (Pid == 0)
                {
                    string Query = "SELECT id, name, rank, country, cmdtime, cmdscore FROM player WHERE cmdscore > 0 ORDER BY cmdscore DESC, name DESC LIMIT {0}, {1}";
                    Rows = Driver.Query(Query, Min, Max);
                    foreach (Dictionary<string, object> Player in Rows)
                    {
                        Output.AddRow(
                            Pos++,
                            Player["id"],
                            Player["name"].ToString().Trim(),
                            Player["cmdscore"],
                            Player["cmdtime"],
                            Player["rank"],
                            Player["country"].ToString().ToUpper()
                        );
                    }
                }
                else
                {
                    // Get Player Position
                    string Query = "SELECT id, name, rank, country, cmdtime, cmdscore FROM player WHERE cmdscore > 0 ORDER BY cmdscore DESC, name DESC";
                    Rows = Driver.Query(Query);
                    foreach (Dictionary<string, object> Player in Rows)
                    {
                        if (Int32.Parse(Player["id"].ToString()) == Pid)
                        {
                            Output.AddRow(
                                Pos,
                                Player["id"],
                                Player["name"].ToString().Trim(),
                                Player["cmdscore"],
                                Player["cmdtime"],
                                Player["rank"],
                                Player["country"].ToString().ToUpper()
                            );
                            break;
                        }
                        Pos++;
                    }
                }

                Response.AddData(Output);
                Response.Send();
            }
            else if (Id == "team")
            {
                Rows = Driver.Query("SELECT COUNT(id) AS count FROM player WHERE teamscore > 0");
                Count = Int32.Parse(Rows[0]["count"].ToString());
                Output.AddRow(Count, Utils.UnixTimestamp());
                Response.AddData(Output);

                // Build New Header Output
                Output = new FormattedOutput("n", "pid", "nick", "teamscore", "totaltime", "playerrank", "countrycode");
                if (Count == 0)
                {
                    Response.AddData(Output);
                    Response.Send();
                    return;
                }

                if (Pid == 0)
                {
                    string Query = "SELECT id, name, rank, country, time, teamscore FROM player WHERE teamscore > 0 ORDER BY teamscore DESC, name DESC LIMIT {0}, {1}";
                    Rows = Driver.Query(Query, Min, Max);
                    foreach (Dictionary<string, object> Player in Rows)
                    {
                        Output.AddRow(
                            Pos++,
                            Player["id"],
                            Player["name"].ToString().Trim(),
                            Player["teamscore"],
                            Player["time"],
                            Player["rank"],
                            Player["country"].ToString().ToUpper()
                        );
                    }
                }
                else
                {
                    // Get Player Position
                    string Query = "SELECT id, name, rank, country, time, teamscore FROM player WHERE teamscore > 0 ORDER BY teamscore DESC, name DESC";
                    Rows = Driver.Query(Query);
                    foreach (Dictionary<string, object> Player in Rows)
                    {
                        if (Int32.Parse(Player["id"].ToString()) == Pid)
                        {
                            Output.AddRow(
                                Pos,
                                Player["id"],
                                Player["name"].ToString().Trim(),
                                Player["teamscore"],
                                Player["time"],
                                Player["rank"],
                                Player["country"].ToString().ToUpper()
                            );
                            break;
                        }
                        Pos++;
                    }
                }

                Response.AddData(Output);
                Response.Send();
            }
            else if (Id == "combat")
            {
                Rows = Driver.Query("SELECT COUNT(id) AS count FROM player WHERE skillscore > 0");
                Count = Int32.Parse(Rows[0]["count"].ToString());
                Output.AddRow(Count, Utils.UnixTimestamp());
                Response.AddData(Output);

                // Build New Header Output
                Output = new FormattedOutput("n", "pid", "nick", "score", "totalkills", "totaltime", "playerrank", "countrycode");
                if (Count == 0)
                {
                    Response.AddData(Output);
                    Response.Send();
                    return;
                }

                if (Pid == 0)
                {
                    string Query = "SELECT id, name, rank, country, time, kills, skillscore FROM player WHERE skillscore > 0 ORDER BY skillscore DESC, name DESC LIMIT {0}, {1}";
                    Rows = Driver.Query(Query, Min, Max);
                    foreach (Dictionary<string, object> Player in Rows)
                    {
                        Output.AddRow(
                            Pos++,
                            Player["id"],
                            Player["name"].ToString().Trim(),
                            Player["skillscore"],
                            Player["kills"],
                            Player["time"],
                            Player["rank"],
                            Player["country"].ToString().ToUpper()
                        );
                    }
                }
                else
                {
                    // Get Player Position
                    string Query = "SELECT id, name, rank, country, time, kills, skillscore FROM player WHERE skillscore > 0 ORDER BY skillscore DESC, name DESC";
                    Rows = Driver.Query(Query);
                    foreach (Dictionary<string, object> Player in Rows)
                    {
                        if (Int32.Parse(Player["id"].ToString()) == Pid)
                        {
                            Output.AddRow(
                                Pos,
                                Player["id"],
                                Player["name"].ToString().Trim(),
                                Player["skillscore"],
                                Player["kills"],
                                Player["time"],
                                Player["rank"],
                                Player["country"].ToString().ToUpper()
                            );
                            break;
                        }
                        Pos++;
                    }
                }

                Response.AddData(Output);
                Response.Send();
            }
            else
            {
                //Response.HTTPStatusCode = ASPResponse.HTTPStatus.BadRequest;
                Response.Send();
            }
        }

        private void DoRisingStar()
        {
            // Fetch all players that made the rising star board
            int Timeframe = Utils.UnixTimestamp() - (60 * 60 * 24 * 7);
            string Query = "SELECT COUNT(DISTINCT(id)) AS count FROM player_history WHERE score > 0 AND timestamp >= {0}";
            List<Dictionary<string, object>> Rows = Driver.Query(Query, Timeframe);

            int Count = Int32.Parse(Rows[0]["count"].ToString());
            FormattedOutput Output = new FormattedOutput("size", "asof");
            Output.AddRow(Count, Utils.UnixTimestamp());
            Response.AddData(Output);

            // Start a new header
            Output = new FormattedOutput("n", "pid", "nick", "weeklyscore", "totaltime", "date", "playerrank", "countrycode");
            if (Count == 0)
            {
                Response.AddData(Output);
                Response.Send();
                return;
            }

            // Are we finding players position, or are we fetching the list?
            if (Pid == 0)
            {
                Query = "SELECT p.id, p.name, p.rank, p.country, p.time, sum(h.score) as weeklyscore, p.joined"
				    + " FROM player AS p JOIN player_history AS h ON p.id = h.id"
				    + " WHERE (h.score > 0 AND h.timestamp >= {0})"
				    + " GROUP BY p.id"
				    + " ORDER BY weeklyscore DESC, name DESC LIMIT {1}, {2}";
                Rows = Driver.Query(Query, Timeframe, Min, Max);
                foreach (Dictionary<string, object> Player in Rows)
                {
                    DateTime FromUnix = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Int32.Parse(Player["joined"].ToString())).ToLocalTime();
                    string DateString = FromUnix.ToString( "MM/dd/yy hh:mm:00 tt" );
                    Output.AddRow(
                        Pos++,
                        Player["id"],
                        Player["name"].ToString().Trim(),
                        Player["weeklyscore"],
                        Player["time"],
                        DateString,
                        Player["rank"],
                        Player["country"].ToString().ToUpper()
                    );
                }

                Response.AddData(Output);
                Response.Send();
            }
            else
            {
                // Find players position
                Query = @"SELECT p.id, p.name, p.rank, p.country, p.time, sum(h.score) as weeklyscore, p.joined"
                    + " FROM player AS p JOIN player_history AS h ON p.id = h.id"
                    + " WHERE (h.score > 0 AND h.timestamp >= {0})"
                    + " GROUP BY p.id"
                    + " ORDER BY weeklyscore DESC, name DESC";
                Rows = Driver.Query(Query, Timeframe);
                foreach (Dictionary<string, object> Player in Rows)
                {
                    if (Int32.Parse(Player["id"].ToString()) == Pid)
                    {
                        DateTime FromUnix = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Int32.Parse(Player["joined"].ToString())).ToLocalTime();
                        string DateString = FromUnix.ToString("MM/dd/yy hh:mm:00 tt");
                        Output.AddRow(
                            Pos,
                            Player["id"],
                            Player["name"].ToString().Trim(),
                            Player["weeklyscore"],
                            Player["time"],
                            DateString,
                            Player["rank"],
                            Player["country"].ToString().ToUpper()
                        );
                        break;
                    }
                    Pos++;
                }

                Response.AddData(Output);
                Response.Send();
            }
        }

        private void DoKit()
        {
            // Prepare Output
            FormattedOutput Output;

            // Make sure we have a score sub type
            int KitId = 0;
            if (String.IsNullOrWhiteSpace(Id) || !Int32.TryParse(Id, out KitId))
            {
                Output = new FormattedOutput("asof", "err");
                Output.AddRow(Utils.UnixTimestamp(), "Invalid Syntax!");
                Response.AddData(Output);
                Response.IsValidData(false);
                Response.Send();
                return;
            }

            // Prepare Output
            Output = new FormattedOutput("size", "asof");
            String Query;
            List<Dictionary<string, object>> Rows;
            int Count;

            // Get total number of players who have at least 1 kill in kit
            Rows = Driver.Query("SELECT COUNT(id) AS count FROM kits WHERE kills{0} > 0", Id);
            Count = Int32.Parse(Rows[0]["count"].ToString());
            Output.AddRow(Count, Utils.UnixTimestamp());
            Response.AddData(Output);

            // Build New Header Output
            Output = new FormattedOutput("n", "pid", "nick", "killswith", "deathsby", "timeplayed", "playerrank", "countrycode");

            // Get Leaderboard Positions
            Query = "SELECT player.id AS plid, name, rank, country, kills{0} AS kills, deaths{0} AS deaths, time{0} AS time"
                + " FROM player NATURAL JOIN kits WHERE kills{0} > 0 ORDER BY kills{0} DESC, name DESC";
            if (Pid == 0)
                Query += String.Format(" LIMIT {1}, {2}", Min, Max);

            Rows = Driver.Query(Query, KitId);
            foreach (Dictionary<string, object> Player in Rows)
            {
                if (Pid == 0 || Int32.Parse(Player["plid"].ToString()) == Pid)
                {
                    Output.AddRow(
                        Pos,
                        Player["plid"],
                        Player["name"].ToString().Trim(),
                        Player["kills"],
                        Player["deaths"],
                        Player["time"],
                        Player["rank"],
                        Player["country"].ToString().ToUpper()
                    );

                    if (Pid != 0)
                        break;
                }
                Pos++;
            }

            Response.AddData(Output);
            Response.Send();
        }

        private void DoVehicles()
        {
            // Prepare Output
            FormattedOutput Output;

            // Make sure we have a score sub type
            int KitId = 0;
            if (String.IsNullOrWhiteSpace(Id) || !Int32.TryParse(Id, out KitId))
            {
                Output = new FormattedOutput("asof", "err");
                Output.AddRow(Utils.UnixTimestamp(), "Invalid Syntax!");
                Response.AddData(Output);
                Response.IsValidData(false);
                Response.Send();
                return;
            }

            // Prepare Output
            Output = new FormattedOutput("size", "asof");
            String Query;
            List<Dictionary<string, object>> Rows;
            int Count;

            // Get total number of players who have at least 1 kill in kit
            Rows = Driver.Query("SELECT COUNT(id) AS count FROM vehicles WHERE kills{0} > 0", Id);
            Count = Int32.Parse(Rows[0]["count"].ToString());
            Output.AddRow(Count, Utils.UnixTimestamp());
            Response.AddData(Output);

            // Build New Header Output
            Output = new FormattedOutput("n", "pid", "nick", "killswith", "detahsby", "timeused", "playerrank", "countrycode");

            // Get Leaderboard Positions
            Query = "SELECT player.id AS plid, name, rank, country, kills{0} AS kills, deaths{0} AS deaths, time{0} AS time"
                + " FROM player NATURAL JOIN vehicles WHERE kills{0} > 0 ORDER BY kills{0} DESC, name DESC";
            if (Pid == 0)
                Query += String.Format(" LIMIT {1}, {2}", Min, Max);

            Rows = Driver.Query(Query, KitId);
            foreach (Dictionary<string, object> Player in Rows)
            {
                if (Pid == 0 || Int32.Parse(Player["plid"].ToString()) == Pid)
                {
                    Output.AddRow(
                        Pos,
                        Player["plid"],
                        Player["name"].ToString().Trim(),
                        Player["kills"],
                        Player["deaths"],
                        Player["time"],
                        Player["rank"],
                        Player["country"].ToString().ToUpper()
                    );

                    if (Pid != 0)
                        break;
                }
                Pos++;
            }

            Response.AddData(Output);
            Response.Send();
        }

        private void DoWeapons()
        {
            // Prepare Output
            FormattedOutput Output;

            // Make sure we have a score sub type
            int KitId = 0;
            if (String.IsNullOrWhiteSpace(Id) || !Int32.TryParse(Id, out KitId))
            {
                Output = new FormattedOutput("asof", "err");
                Output.AddRow(Utils.UnixTimestamp(), "Invalid Syntax!");
                Response.AddData(Output);
                Response.IsValidData(false);
                Response.Send();
                return;
            }

            // Prepare Output
            Output = new FormattedOutput("size", "asof");
            String Query;
            List<Dictionary<string, object>> Rows;
            int Count;

            // Get total number of players who have at least 1 kill in kit
            Rows = Driver.Query("SELECT COUNT(id) AS count FROM weapons WHERE kills{0} > 0", Id);
            Count = Int32.Parse(Rows[0]["count"].ToString());
            Output.AddRow(Count, Utils.UnixTimestamp());
            Response.AddData(Output);

            // Build New Header Output
            Output = new FormattedOutput("n", "pid", "nick", "killswith", "detahsby", "timeused", "accuracy", "playerrank", "countrycode");

            // Get Leaderboard Positions
            Query = "SELECT player.id AS plid, name, rank, country, kills{0} AS kills, deaths{0} AS deaths, time{0} AS time, "
                + "hit{0} AS hit, fired{0} AS fired FROM player NATURAL JOIN weapons WHERE kills{0} > 0 ORDER BY kills{0} DESC, name DESC";
            if (Pid == 0)
                Query += String.Format(" LIMIT {1}, {2}", Min, Max);

            Rows = Driver.Query(Query, KitId);
            foreach (Dictionary<string, object> Player in Rows)
            {
                if (Pid == 0 || Int32.Parse(Player["plid"].ToString()) == Pid)
                {
                    float Hit = float.Parse(Player["hit"].ToString());
                    float Fired = float.Parse(Player["fired"].ToString());
                    float Acc = (Hit / Fired) * 100;
                    Output.AddRow(
                        Pos,
                        Player["plid"],
                        Player["name"].ToString().Trim(),
                        Player["kills"],
                        Player["deaths"],
                        Player["time"],
                        Math.Round(Acc, 0),
                        Player["rank"],
                        Player["country"].ToString().ToUpper()
                    );

                    if (Pid != 0)
                        break;
                }
                Pos++;
            }

            Response.AddData(Output);
            Response.Send();
        }
    }
}
