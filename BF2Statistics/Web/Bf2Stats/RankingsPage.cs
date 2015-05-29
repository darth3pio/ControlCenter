using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using BF2Statistics.Database;

namespace BF2Statistics.Web.Bf2Stats
{
    partial class RankingsPage
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
        /// Our cache file object
        /// </summary>
        protected FileInfo CacheFile;

        /// <summary>
        /// Indicates whether we need to cache this file after its generated
        /// </summary>
        protected bool NeedsCached = false;

        /// <summary>
        /// The array of parsed ranking objects
        /// </summary>
        protected List<RankingStats> Stats = new List<RankingStats>();

        /// <summary>
        /// A static array of all the different ranking sections queries
        /// </summary>
        protected static string[] Queries = new string[]
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
            "SELECT id, name, rank, (heals + revives) AS value FROM player WHERE value > 0 ORDER BY value DESC LIMIT 5",
            "SELECT id, name, rank, teamscore - (teamdamage + teamkills + teamvehicledamage) AS value FROM player WHERE value > 0 ORDER BY value DESC LIMIT 5",
            "SELECT id, name, rank, cmdscore AS value FROM player WHERE cmdscore > 0 ORDER BY cmdscore DESC LIMIT 5",
            "SELECT id, name, rank, (cmdscore * 1.0 / cmdtime) AS value FROM player WHERE (cmdtime > 3600 OR cmdscore > 1000) ORDER BY value DESC LIMIT 5"
        };

        public RankingsPage(HttpClient Client, StatsDatabase Database)
        {
            this.Client = Client;
            this.Root = "http://" + this.Client.Request.Url.DnsSafeHost + "/bf2stats";

            // Check cache
            if (Program.Config.BF2S_CacheEnabled)
            {
                CacheFile = new FileInfo(Path.Combine(Program.RootPath, "Web", "Bf2Stats", "Cache", "Rankings.html"));
                if (!CacheFile.Exists || DateTime.Now.CompareTo(CacheFile.LastWriteTime.AddMinutes(30)) > 0)
                    this.NeedsCached = true;
                else
                    return; // We are reading from cache, nothing to do here
            }

            // Score
            Stats.Add(new RankingStats
            {
                Name = "Score",
                Desc = "&nbsp;",
                TopPlayers = GetTopFromQuery(0, Database)
            });

            // SPM
            Stats.Add(new RankingStats
            {
                Name = "Score Per Minute",
                Desc = "&nbsp;",
                TopPlayers = GetTopFromQuery(1, Database)
            });

            // W/L Ratio
            Stats.Add(new RankingStats
            {
                Name = "Win-Loss Ratio",
                Desc = "&nbsp;",
                TopPlayers = GetTopFromQuery(2, Database)
            });

            // K/D Ratio
            Stats.Add(new RankingStats
            {
                Name = "Kill-Death Ratio",
                Desc = "&nbsp;",
                TopPlayers = GetTopFromQuery(3, Database)
            });

            // Knife KDR
            Stats.Add(new RankingStats
            {
                Name = "Knife KDR",
                Desc = "(Requires at least 1 kill with the Knife)",
                TopPlayers = GetTopFromQuery(4, Database)
            });

            // Sniper Accuracy
            Stats.Add(new RankingStats
            {
                Name = "Sniper Accuracy",
                Desc = "(Must have more than 100 shots with the Sniper Rifle)",
                TopPlayers = GetTopFromQuery(5, Database)
            });

            // Best Round Score
            Stats.Add(new RankingStats
            {
                Name = "Best Round Score",
                Desc = "&nbsp;",
                TopPlayers = GetTopFromQuery(6, Database)
            });

            // Flag Captures
            Stats.Add(new RankingStats
            {
                Name = "Flag Captures",
                Desc = "&nbsp;",
                TopPlayers = GetTopFromQuery(7, Database)
            });

            // Flag Work
            Stats.Add(new RankingStats
            {
                Name = "Flag Work",
                Desc = "(Defend, Capture, etc...)",
                TopPlayers = GetTopFromQuery(8, Database)
            });

            // Best Medic
            Stats.Add(new RankingStats
            {
                Name = "Top Medic",
                Desc = "(Revives, Heals)",
                TopPlayers = GetTopFromQuery(9, Database)
            });

            // Best Team Workers
            Stats.Add(new RankingStats
            {
                Name = "Best Teamworkers",
                Desc = "&nbsp;",
                TopPlayers = GetTopFromQuery(10, Database)
            });

            // Command Score
            Stats.Add(new RankingStats
            {
                Name = "Command Score",
                Desc = "&nbsp;",
                TopPlayers = GetTopFromQuery(11, Database)
            });

            // Relative Command Score
            Stats.Add(new RankingStats
            {
                Name = "Relative Command Score",
                Desc = "(Command score > 1000 OR Command time > 1 hour)",
                TopPlayers = GetTopFromQuery(12, Database)
            });
            
        }

        /// <summary>
        /// Using the Cache system, this method will return the Html contents of the generated page
        /// </summary>
        /// <returns></returns>
        public string TransformHtml()
        {
            if (Program.Config.BF2S_CacheEnabled)
            {
                if (this.NeedsCached)
                {
                    string page = TransformText();

                    try
                    {
                        using (FileStream Stream = CacheFile.Open(FileMode.OpenOrCreate, FileAccess.Write))
                        using (StreamWriter Writer = new StreamWriter(Stream))
                        {
                            Writer.BaseStream.SetLength(0);
                            Writer.Write(page);
                            Writer.Flush();
                        }

                        // Manually set write time!!!
                        CacheFile.LastWriteTime = DateTime.Now;
                    }
                    catch (Exception e)
                    {
                        Program.ErrorLog.Write("WARNING: [PlayerPage.CreateCacheFile] " + e.Message);
                    }

                    return page;
                }
                else
                {
                    try
                    {
                        using (FileStream Stream = CacheFile.Open(FileMode.Open, FileAccess.Read))
                        using (StreamReader Reader = new StreamReader(Stream))
                        {
                            // Read our page source
                            string source = Reader.ReadToEnd();

                            // Replace Http Link Addresses to match the request URL, preventing Localhost links with external requests
                            return Regex.Replace(source, "http://.*/Bf2stats", this.Root, RegexOptions.IgnoreCase);
                        }
                    }
                    catch (Exception e)
                    {
                        Program.ErrorLog.Write("ERROR: [PlayerPage.ReadCacheFile] " + e.Message);
                        return "Cache Read Error";
                    }
                }
            }

            return TransformText();
        }

        /// <summary>
        /// Processes the query ID and returns the results
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Database"></param>
        /// <returns></returns>
        protected List<Player> GetTopFromQuery(int id, StatsDatabase Database)
        {
            List<Player> Players = new List<Player>(5);
            var Rows = Database.Query( Queries[id] );
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
