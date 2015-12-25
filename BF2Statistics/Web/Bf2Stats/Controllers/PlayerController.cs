using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using BF2Statistics.Database;
using BF2Statistics.Database.QueryBuilder;
using BF2Statistics.MedalData;
using BF2Statistics.Python;

namespace BF2Statistics.Web.Bf2Stats
{
    public class PlayerController : Controller
    {
        /// <summary>
        /// The player ID
        /// </summary>
        protected int Pid;

        /// <summary>
        /// Gets or Sets an open stats database connection
        /// </summary>
        protected StatsDatabase Database;

        /// <summary>
        /// Gets or Sets the working row set from the database
        /// </summary>
        protected List<Dictionary<string, object>> Rows;

        /// <summary>
        /// The array of queries for the players "Stats" section. Allows for
        /// looping, thus reducing code length
        /// </summary>
        protected string[] StatsQueries = new string[]
        {
            "SELECT COUNT(id) FROM awards WHERE awd IN(2051902, 2051919, 2051907) AND id=@P0",
            "SELECT COUNT(id) FROM awards WHERE id=@P0",
            "SELECT kills FROM player WHERE id=@P0",
            "SELECT deaths FROM player WHERE id=@P0",
            "SELECT time FROM player WHERE id=@P0",
            "SELECT teamkills FROM player WHERE id=@P0",
            "SELECT (captures + captureassists + neutralizes + neutralizeassists + defends) AS value FROM player WHERE id=@P0",
            "SELECT teamscore FROM player WHERE id=@P0",
            "SELECT (score - cmdscore - teamscore) FROM player WHERE id=@P0",
            "SELECT score FROM player WHERE id=@P0",
        };

        public PlayerController(HttpClient Client) : base(Client) {  }

        public override void HandleRequest(MvcRoute Route)
        {
            // Make sure we have a pid
            if (!Validator.IsValidPID(Route.Action))
            {
                Client.Response.Redirect("/bf2stats");
                return;
            }

            // Fetch Player ID (we already verified its an int above)
            Pid = Int32.Parse(Route.Action);

            // NOTE: The HttpServer will handle the DbConnectException
            using (Database = new StatsDatabase())
            {
                // Fetch Player
                Rows = Database.Query("SELECT * FROM player WHERE id=@P0", Pid);
                if (Rows.Count == 0)
                {
                    Client.Response.Redirect("/bf2stats/search");
                    return;
                }

                // Load our page based on the param passed
                if (Route.Params.Length > 0 && Route.Params[0] == "rankings")
                    ShowRankings();
                else
                    ShowStats();
            }
        }

        private void ShowStats()
        {
            // Check cache
            if (!base.CacheFileExpired(Pid.ToString(), 30))
            {
                base.SendCachedResponse(Pid.ToString());
                return;
            }

            // Load our Model
            PlayerModel Model = new PlayerModel(Client);
            Model.Player = Rows[0];
            Model.SearchBarValue = Pid.ToString();

            // Setup player variables
            int PlayerKills = Int32.Parse(Model.Player["kills"].ToString());
            double AcctsFor = 0;
            int j = 0;

            // My Leaderboard stuff
            Cookie C = Client.Request.Request.Cookies["leaderboard"] ?? new Cookie("leaderboard", "");
            Model.OnMyLeaderboard = C.Value.Contains(Pid.ToString());

            #region ArmyData
            // Fetch army data
            Rows = Database.Query("SELECT * FROM army WHERE id=@P0", Pid);
            foreach (KeyValuePair<int, string> Army in Bf2StatsData.Armies)
            {
                int Wins = Int32.Parse(Rows[0]["win" + Army.Key].ToString());
                int Losses = Int32.Parse(Rows[0]["loss" + Army.Key].ToString());
                Model.ArmyData.Add(new ArmyMapStat
                {
                    Id = Army.Key,
                    Time = Int32.Parse(Rows[0]["time" + Army.Key].ToString()),
                    Wins = Wins,
                    Losses = Losses,
                    Best = Int32.Parse(Rows[0]["best" + Army.Key].ToString()),
                });

                Model.ArmySummary.TotalWins += Wins;
                Model.ArmySummary.TotalLosses += Losses;
                Model.ArmySummary.TotalTime += Model.ArmyData[j].Time;
                Model.ArmySummary.TotalBest += Model.ArmyData[j].Best;
                j++;
            }
            #endregion ArmyData

            #region MapData
            // Fetch Map Data
            j = 0;
            Rows = Database.Query("SELECT * FROM maps WHERE id=@P0 ORDER BY mapid", Pid);
            foreach (Dictionary<string, object> Row in Rows)
            {
                // Do we support this map id?
                if (!Bf2StatsData.Maps.Keys.Contains(Int32.Parse(Row["mapid"].ToString())))
                    continue;

                int Wins = Int32.Parse(Row["win"].ToString());
                int Losses = Int32.Parse(Row["loss"].ToString());
                Model.MapData.Add(new ArmyMapStat
                {
                    Id = Int32.Parse(Row["mapid"].ToString()),
                    Time = Int32.Parse(Row["time"].ToString()),
                    Wins = Wins,
                    Losses = Losses,
                    Best = Int32.Parse(Row["best"].ToString()),
                });

                Model.MapSummary.TotalWins += Wins;
                Model.MapSummary.TotalLosses += Losses;
                Model.MapSummary.TotalTime += Model.MapData[j].Time;
                Model.MapSummary.TotalBest += Model.MapData[j].Best;
                j++;
            }
            #endregion MapData

            #region TheaterData
            // Do Theater Data
            foreach (KeyValuePair<string, int[]> t in Bf2StatsData.TheatreMapIds)
            {
                j = 0;
                string inn = String.Join(",", t.Value);
                Rows = Database.Query(
                    "SELECT COALESCE(sum(time), 0) as time, COALESCE(sum(win), 0) as win, COALESCE(sum(loss), 0) as loss, COALESCE(max(best), 0) as best "
                    + "FROM maps WHERE mapid IN (" + inn + ") AND id=@P0", Pid
                );

                // 
                Model.TheaterData.Add(new TheaterStat
                {
                    Name = t.Key,
                    Time = Int32.Parse(Rows[0]["time"].ToString()),
                    Wins = Int32.Parse(Rows[0]["win"].ToString()),
                    Losses = Int32.Parse(Rows[0]["loss"].ToString()),
                    Best = Int32.Parse(Rows[0]["best"].ToString()),
                });
            }
            #endregion TheaterData

            #region VehicleData
            // Fetch Vehicle Data
            Rows = Database.Query("SELECT * FROM vehicles WHERE id=@P0", Pid);
            for (int i = 0; i < 7; i++)
            {
                int Kills = Int32.Parse(Rows[0]["kills" + i].ToString());
                int Deaths = Int32.Parse(Rows[0]["deaths" + i].ToString());
                int RoadKills = Int32.Parse(Rows[0]["rk" + i].ToString());
                Model.VehicleData.Add(new ObjectStat
                {
                    Time = Int32.Parse(Rows[0]["time" + i].ToString()),
                    Kills = Kills,
                    Deaths = Deaths,
                    RoadKills = RoadKills,
                    KillsAcctFor = (PlayerKills == 0 || Kills == 0) ? 0.00 : Math.Round(100 * ((double)(RoadKills + Kills) / PlayerKills), 2)
                });

                Model.VehicleSummary.TotalKills += Kills;
                Model.VehicleSummary.TotalDeaths += Deaths;
                Model.VehicleSummary.TotalTime += Model.VehicleData[i].Time;
                Model.VehicleSummary.TotalRoadKills += Model.VehicleData[i].RoadKills;
                AcctsFor += Model.VehicleData[i].KillsAcctFor;
            }

            // Add para time
            Model.VehicleData.Add(new ObjectStat { Time = Int32.Parse(Rows[0]["timepara"].ToString()) });
            Model.VehicleSummary.KillsAcctFor = (AcctsFor > 0) ? Math.Round(AcctsFor / 7, 2) : 0.00;
            #endregion VehicleData

            #region XpackTime
            // Do Expansion time
            foreach (KeyValuePair<string, List<int>> t in Bf2StatsData.ModMapIds)
            {
                if (t.Value.Count > 0)
                {
                    string inn = String.Join(",", t.Value);
                    Rows = Database.Query("SELECT COALESCE(sum(time), 0) as time FROM maps WHERE mapid IN (" + inn + ") AND id=@P0", Pid);
                    if (Model.ExpackTime.ContainsKey(t.Key))
                        Model.ExpackTime[t.Key] = Int32.Parse(Rows[0]["time"].ToString());
                    else
                        Model.ExpackTime.Add(t.Key, Int32.Parse(Rows[0]["time"].ToString()));
                }
            }
            #endregion XpackTime

            #region KitData
            // Fetch Kit Data
            AcctsFor = 0;
            Rows = Database.Query("SELECT * FROM kits WHERE id=@P0", Pid);
            for (int i = 0; i < 7; i++)
            {
                int Kills = Int32.Parse(Rows[0]["kills" + i].ToString());
                int Deaths = Int32.Parse(Rows[0]["deaths" + i].ToString());
                Model.KitData.Add(new ObjectStat
                {
                    Time = Int32.Parse(Rows[0]["time" + i].ToString()),
                    Kills = Kills,
                    Deaths = Deaths,
                    KillsAcctFor = (PlayerKills == 0 || Kills == 0) ? 0.00 : Math.Round(100 * ((double)Kills / PlayerKills), 2)
                });

                Model.KitSummary.TotalKills += Kills;
                Model.KitSummary.TotalDeaths += Deaths;
                Model.KitSummary.TotalTime += Model.KitData[i].Time;
                AcctsFor += Model.KitData[i].KillsAcctFor;
            }
            Model.KitSummary.KillsAcctFor = (AcctsFor > 0) ? Math.Round(AcctsFor / 7, 2) : 0.00;
            #endregion KitData

            #region WeaponData
            // Fetch weapon Data
            AcctsFor = 0;
            double AcctsFor2 = 0;
            Rows = Database.Query("SELECT * FROM weapons WHERE id=@P0", Pid);
            for (int i = 0; i < 15; i++)
            {
                if (i < 9)
                {
                    int Kills = Int32.Parse(Rows[0]["kills" + i].ToString());
                    int Deaths = Int32.Parse(Rows[0]["deaths" + i].ToString());
                    int Hits = Int32.Parse(Rows[0]["hit" + i].ToString());
                    int Fired = Int32.Parse(Rows[0]["fired" + i].ToString());
                    Model.WeaponData.Add(new WeaponStat
                    {
                        Time = Int32.Parse(Rows[0]["time" + i].ToString()),
                        Kills = Kills,
                        Deaths = Deaths,
                        Hits = Hits,
                        Fired = Fired,
                        KillsAcctFor = (PlayerKills == 0 || Kills == 0) ? 0.00 : Math.Round(100 * ((double)Kills / PlayerKills), 2)
                    });
                }
                else
                {
                    string Pfx = GetWeaponTblPrefix(i);
                    int Kills = Int32.Parse(Rows[0][Pfx + "kills"].ToString());
                    int Deaths = Int32.Parse(Rows[0][Pfx + "deaths"].ToString());
                    int Hits = Int32.Parse(Rows[0][Pfx + "hit"].ToString());
                    int Fired = Int32.Parse(Rows[0][Pfx + "fired"].ToString());
                    Model.WeaponData.Add(new WeaponStat
                    {
                        Time = Int32.Parse(Rows[0][Pfx + "time"].ToString()),
                        Kills = Kills,
                        Deaths = Deaths,
                        Hits = Hits,
                        Fired = Fired,
                        KillsAcctFor = (PlayerKills == 0 || Kills == 0) ? 0.00 : Math.Round(100 * ((double)Kills / PlayerKills), 2)
                    });
                }
            }

            Model.WeaponData.Add(new WeaponStat
            {
                Time = Int32.Parse(Rows[0]["tacticaltime"].ToString()),
                Fired = Int32.Parse(Rows[0]["tacticaldeployed"].ToString())
            });

            Model.WeaponData.Add(new WeaponStat
            {
                Time = Int32.Parse(Rows[0]["grapplinghooktime"].ToString()),
                Deaths = Int32.Parse(Rows[0]["grapplinghookdeaths"].ToString()),
                Fired = Int32.Parse(Rows[0]["grapplinghookdeployed"].ToString())
            });

            Model.WeaponData.Add(new WeaponStat
            {
                Time = Int32.Parse(Rows[0]["ziplinetime"].ToString()),
                Deaths = Int32.Parse(Rows[0]["ziplinedeaths"].ToString()),
                Fired = Int32.Parse(Rows[0]["ziplinedeployed"].ToString())
            });

            for (int i = 0; i < 17; i++)
            {
                Model.WeaponSummary.TotalKills += Model.WeaponData[i].Kills;
                Model.WeaponSummary.TotalDeaths += Model.WeaponData[i].Deaths;
                Model.WeaponSummary.TotalTime += Model.WeaponData[i].Time;
                Model.WeaponSummary.TotalHits += Model.WeaponData[i].Hits;
                Model.WeaponSummary.TotalFired += Model.WeaponData[i].Fired;
                AcctsFor += Model.WeaponData[i].KillsAcctFor;

                if (i > 8)
                {
                    Model.EquipmentSummary.TotalKills += Model.WeaponData[i].Kills;
                    Model.EquipmentSummary.TotalDeaths += Model.WeaponData[i].Deaths;
                    Model.EquipmentSummary.TotalTime += Model.WeaponData[i].Time;
                    Model.EquipmentSummary.TotalHits += Model.WeaponData[i].Hits;
                    Model.EquipmentSummary.TotalFired += Model.WeaponData[i].Fired;
                    AcctsFor2 += Model.WeaponData[i].KillsAcctFor;
                }
            }
            Model.WeaponSummary.KillsAcctFor = (AcctsFor > 0) ? Math.Round(AcctsFor / 15, 2) : 0.00;
            Model.EquipmentSummary.KillsAcctFor = (AcctsFor > 0) ? Math.Round(AcctsFor / 6, 2) : 0.00;

            // Extra weapon data DEFIB
            Model.WeaponData2.Add(Model.WeaponData[13]);

            // Extra Weapon data Explosives
            Model.WeaponData2.Add(new WeaponStat
            {
                Time = Model.WeaponData[10].Time + Model.WeaponData[11].Time + Model.WeaponData[14].Time,
                Kills = Model.WeaponData[10].Kills + Model.WeaponData[11].Kills + Model.WeaponData[14].Kills,
                Deaths = Model.WeaponData[10].Deaths + Model.WeaponData[11].Deaths + Model.WeaponData[14].Deaths,
                Hits = Model.WeaponData[10].Hits + Model.WeaponData[11].Hits + Model.WeaponData[14].Hits,
                Fired = Model.WeaponData[10].Fired + Model.WeaponData[11].Fired + Model.WeaponData[14].Fired,
                KillsAcctFor = Model.WeaponData[10].KillsAcctFor + Model.WeaponData[11].KillsAcctFor + Model.WeaponData[14].KillsAcctFor
            });

            // Extra weapon data Grenade
            Model.WeaponData2.Add(Model.WeaponData[12]);

            #endregion WeaponData

            #region Favorites

            // Add Favorites
            Model.Favorites.Add("army", (from x in Model.ArmyData orderby x.Time select Model.ArmyData.IndexOf(x)).Last());
            Model.Favorites.Add("map", (Model.MapData.Count == 0)
                ? -1
                : (from x in Model.MapData orderby x.Time select Model.MapData.IndexOf(x)).Last());
            Model.Favorites.Add("theater", (from x in Model.TheaterData orderby x.Time select Model.TheaterData.IndexOf(x)).Last());
            Model.Favorites.Add("vehicle", (from x in Model.VehicleData orderby x.Time select Model.VehicleData.IndexOf(x)).Last());
            Model.Favorites.Add("kit", (from x in Model.KitData orderby x.Time select Model.KitData.IndexOf(x)).Last());
            Model.Favorites.Add("weapon", (from x in Model.WeaponData orderby x.Time select Model.WeaponData.IndexOf(x)).Last());
            Model.Favorites.Add("equipment",
            (
                from x in Model.WeaponData
                orderby x.Time
                let index = Model.WeaponData.IndexOf(x)
                where index > 8
                select index
            ).Last());
            Model.FavoriteMapId = Model.Favorites["map"] >= 0 ? Model.MapData[Model.Favorites["map"]].Id : -1;

            #endregion

            #region TopEnemy and Victim
            // Get top enemy's
            Rows = Database.Query(
                "SELECT attacker, count, t.name AS name, t.rank AS rank FROM kills JOIN player AS t WHERE t.id = attacker AND victim = @P0 ORDER BY count DESC LIMIT 11",
                Pid
            );
            if (Rows.Count > 0)
            {
                Model.TopEnemies.Add(new Player
                {
                    Pid = Int32.Parse(Rows[0]["attacker"].ToString()),
                    Name = Rows[0]["name"].ToString(),
                    Rank = Int32.Parse(Rows[0]["rank"].ToString()),
                    Count = Int32.Parse(Rows[0]["count"].ToString()),
                });

                if (Rows.Count > 1)
                {
                    for (int i = 1; i < Rows.Count; i++)
                    {
                        Model.TopEnemies.Add(new Player
                        {
                            Pid = Int32.Parse(Rows[i]["attacker"].ToString()),
                            Name = Rows[i]["name"].ToString(),
                            Rank = Int32.Parse(Rows[i]["rank"].ToString()),
                            Count = Int32.Parse(Rows[i]["count"].ToString()),
                        });
                    }
                }
            }

            // Get top victims's
            Rows = Database.Query(
                "SELECT victim, count, t.name AS name, t.rank AS rank FROM kills JOIN player AS t WHERE t.id = victim AND attacker = @P0 ORDER BY count DESC LIMIT 11",
                Pid
            );
            if (Rows.Count > 0)
            {
                Model.TopVictims.Add(new Player
                {
                    Pid = Int32.Parse(Rows[0]["victim"].ToString()),
                    Name = Rows[0]["name"].ToString(),
                    Rank = Int32.Parse(Rows[0]["rank"].ToString()),
                    Count = Int32.Parse(Rows[0]["count"].ToString()),
                });

                if (Rows.Count > 1)
                {
                    for (int i = 1; i < Rows.Count; i++)
                    {
                        Model.TopVictims.Add(new Player
                        {
                            Pid = Int32.Parse(Rows[i]["victim"].ToString()),
                            Name = Rows[i]["name"].ToString(),
                            Rank = Int32.Parse(Rows[i]["rank"].ToString()),
                            Count = Int32.Parse(Rows[i]["count"].ToString()),
                        });
                    }
                }
            }

            #endregion TopEnemy and Victim

            #region Unlocks
            j = 0;
            Rows = Database.Query("SELECT kit, state FROM unlocks WHERE id=@P0 ORDER BY kit ASC", Pid);
            if (Rows.Count > 0)
            {
                foreach (Dictionary<string, object> Row in Rows)
                {
                    Model.PlayerUnlocks[j] = new WeaponUnlock
                    {
                        Id = Int32.Parse(Row["kit"].ToString()),
                        Name = Bf2Constants.Unlocks[Row["kit"].ToString()],
                        State = Row["state"].ToString()
                    };
                    j++;
                }
            }
            else
            {
                // Normal unlocks
                for (int i = 11; i < 100; i += 11)
                {
                    // 88 and above are Special Forces unlocks, and wont display at all if the base unlocks are not earned
                    Model.PlayerUnlocks[i] = new WeaponUnlock
                    {
                        Id = i,
                        Name = Bf2Constants.Unlocks[i.ToString()],
                        State = "n"
                    };
                    j++;
                }

                // Sf Unlocks, Dont display these because thats how Gamespy does it
                for (int i = 111; i < 556; i += 111)
                {
                    Model.PlayerUnlocks[i] = new WeaponUnlock
                    {
                        Id = i,
                        Name = Bf2Constants.Unlocks[i.ToString()],
                        State = "n"
                    };
                    j++;
                }
            }
            #endregion Unlocks

            #region Badges
            // Fetch player badges
            foreach (KeyValuePair<string, string> Awd in StatsConstants.Badges)
            {
                int AwdId = Int32.Parse(Awd.Key);
                List<Award> Badges = new List<Award>(4);
                for (int i = 0; i < 4; i++)
                    Badges.Add(new Award { Id = AwdId });

                Rows = Database.Query("SELECT * FROM awards WHERE id=@P0 AND awd=@P1 ORDER BY level ASC", Pid, AwdId);
                if (Rows.Count > 0)
                {
                    int max = Rows.Count - 1;
                    int Maxlevel = Int32.Parse(Rows[max]["level"].ToString());
                    Badges[0] = new Award
                    {
                        Id = AwdId,
                        Level = Maxlevel,
                        Earned = Int32.Parse(Rows[max]["earned"].ToString())
                    };

                    for (int i = 1; i < 4; i++)
                    {
                        if (Rows.Count >= i)
                        {
                            Badges[i] = new Award
                            {
                                Id = AwdId,
                                Level = Int32.Parse(Rows[i - 1]["level"].ToString()),
                                Earned = Int32.Parse(Rows[i - 1]["earned"].ToString())
                            };
                        }
                    }
                }

                Model.PlayerBadges.Add(Awd.Key, Badges);
            }

            // Fetch player badges
            foreach (KeyValuePair<string, string> Awd in StatsConstants.SfBadges)
            {
                int AwdId = Int32.Parse(Awd.Key);
                List<Award> Badges = new List<Award>(4);
                for (int i = 0; i < 4; i++)
                    Badges.Add(new Award { Id = AwdId });

                Rows = Database.Query("SELECT * FROM awards WHERE id=@P0 AND awd=@P1 ORDER BY level ASC", Pid, AwdId);
                if (Rows.Count > 0)
                {
                    int max = Rows.Count - 1;
                    int Maxlevel = Int32.Parse(Rows[max]["level"].ToString());
                    Badges[0] = new Award
                    {
                        Id = AwdId,
                        Level = Maxlevel,
                        Earned = Int32.Parse(Rows[max]["earned"].ToString())
                    };

                    for (int i = 1; i < 4; i++)
                    {
                        if (Rows.Count >= i)
                        {
                            Badges[i] = new Award
                            {
                                Id = AwdId,
                                Level = Int32.Parse(Rows[i - 1]["level"].ToString()),
                                Earned = Int32.Parse(Rows[i - 1]["earned"].ToString())
                            };
                        }
                    }
                }

                Model.PlayerSFBadges.Add(Awd.Key, Badges);
            }
            #endregion Badges

            #region Medals

            // Fetch player medals
            foreach (KeyValuePair<string, string> Awd in StatsConstants.Medals)
            {
                int AwdId = Int32.Parse(Awd.Key);
                Rows = Database.Query("SELECT * FROM awards WHERE id=@P0 AND awd=@P1 LIMIT 1", Pid, AwdId);
                if (Rows.Count > 0)
                {
                    Model.PlayerMedals.Add(Awd.Key, new Award
                    {
                        Id = AwdId,
                        Level = Int32.Parse(Rows[0]["level"].ToString()),
                        Earned = Int32.Parse(Rows[0]["earned"].ToString()),
                        First = Int32.Parse(Rows[0]["first"].ToString()),
                    });
                }
                else
                    Model.PlayerMedals.Add(Awd.Key, new Award { Id = AwdId });

            }

            foreach (KeyValuePair<string, string> Awd in StatsConstants.SfMedals)
            {
                int AwdId = Int32.Parse(Awd.Key);
                Rows = Database.Query("SELECT * FROM awards WHERE id=@P0 AND awd=@P1 LIMIT 1", Pid, AwdId);
                if (Rows.Count > 0)
                {
                    Model.PlayerSFMedals.Add(Awd.Key, new Award
                    {
                        Id = AwdId,
                        Level = Int32.Parse(Rows[0]["level"].ToString()),
                        Earned = Int32.Parse(Rows[0]["earned"].ToString()),
                        First = Int32.Parse(Rows[0]["first"].ToString()),
                    });
                }
                else
                    Model.PlayerSFMedals.Add(Awd.Key, new Award { Id = AwdId });

            }

            #endregion Medals

            #region Ribbons

            // Fetch player ribbons
            foreach (KeyValuePair<string, string> Awd in StatsConstants.Ribbons)
            {
                int AwdId = Int32.Parse(Awd.Key);
                Rows = Database.Query("SELECT * FROM awards WHERE id=@P0 AND awd=@P1 LIMIT 1", Pid, AwdId);
                if (Rows.Count > 0)
                {
                    Model.PlayerRibbons.Add(Awd.Key, new Award
                    {
                        Id = AwdId,
                        Level = Int32.Parse(Rows[0]["level"].ToString()),
                        Earned = Int32.Parse(Rows[0]["earned"].ToString()),
                        First = Int32.Parse(Rows[0]["first"].ToString()),
                    });
                }
                else
                    Model.PlayerRibbons.Add(Awd.Key, new Award { Id = AwdId });

            }

            // Fetch SF Ribbons
            foreach (KeyValuePair<string, string> Awd in StatsConstants.SfRibbons)
            {
                int AwdId = Int32.Parse(Awd.Key);
                Rows = Database.Query("SELECT * FROM awards WHERE id=@P0 AND awd=@P1 LIMIT 1", Pid, AwdId);
                if (Rows.Count > 0)
                {
                    Model.PlayerSFRibbons.Add(Awd.Key, new Award
                    {
                        Id = AwdId,
                        Level = Int32.Parse(Rows[0]["level"].ToString()),
                        Earned = Int32.Parse(Rows[0]["earned"].ToString()),
                        First = Int32.Parse(Rows[0]["first"].ToString()),
                    });
                }
                else
                    Model.PlayerSFRibbons.Add(Awd.Key, new Award { Id = AwdId });

            }

            #endregion Ribbons

            #region Time To Advancement

            // Fetch all of our awards, so we can determine our qualified rank ups
            Rows = Database.Query("SELECT awd, level FROM awards WHERE id=@P0 ORDER BY level", Pid);
            Dictionary<string, int> Awds = new Dictionary<string, int>();
            foreach (Dictionary<string, object> Row in Rows)
            {
                if (!Awds.ContainsKey(Row["awd"].ToString()))
                    Awds.Add(Row["awd"].ToString(), Int32.Parse(Row["level"].ToString()));
            }

            // Debugging ranks? (personal uses)
            int RanksToShow = 3;
            if (Client.Request.QueryString.ContainsKey("nextranks"))
                Int32.TryParse(Client.Request.QueryString["nextranks"], out RanksToShow);

            // 3 min.
            if (RanksToShow < 3) RanksToShow = 3;

            // Get our next ranks
            int Score = Int32.Parse(Model.Player["score"].ToString());
            Model.NextPlayerRanks = RankCalculator.GetNextRanks(
                Score,
                Int32.Parse(Model.Player["rank"].ToString()),
                Awds,
                RanksToShow
            );

            foreach (Rank Rnk in Model.NextPlayerRanks)
            {
                // Get Needed Points for this next rank
                int NP = Rnk.MinPoints - Score;
                Rnk.PointsNeeded = (NP > 0) ? NP : 0;

                // Get our percentage to this next rank based on needed points
                double Perc = Math.Round(((double)Score / Rnk.MinPoints) * 100, 0);
                Rnk.PercentComplete = (Perc > 100) ? 100 : Perc;

                // Get the time to completion, based on our score per minute
                double t = NP / (Model.ScorePerMin / 60);
                if (t < 0) t = 0;
                Rnk.TimeToComplete = (int)t;

                // Get our days to completion time, based on our Join date, Last battle, and average Points per day
                TimeSpan Span = TimeSpan.FromSeconds(
                    Int32.Parse(Model.Player["lastonline"].ToString()) - Int32.Parse(Model.Player["joined"].ToString())
                );
                double SPD = Math.Round(Score / Span.TotalDays, 0);
                Rnk.DaysToComplete = (int)Math.Round(NP / SPD, 0);
            }

            #endregion Time To Advancement

            // Get player image
            string PlayerImage = Path.Combine(
                Program.RootPath, "Web", "Bf2Stats", "Resources", "images", "soldiers",
                Model.Favorites["army"] + "_" + Model.Favorites["kit"] + "_" + Model.Favorites["weapon"] + ".jpg"
            );

            // Convert fav into a URL
            Model.PlayerImage = (File.Exists(PlayerImage))
                ? Model.Root + "/images/soldiers/" + Model.Favorites["army"] + "_" + Model.Favorites["kit"] + "_" + Model.Favorites["weapon"] + ".jpg"
                : Model.Root + "/images/soldiers/" + Model.Favorites["army"] + "_" + Model.Favorites["kit"] + "_5.jpg";

            // Send the response
            base.SendTemplateResponse("player", typeof(PlayerModel), Model, Pid.ToString());
        }

        private void ShowRankings()
        {
            // Check cache
            if (!base.CacheFileExpired(Pid + "_rankings", 30))
            {
                base.SendCachedResponse(Pid + "_rankings");
                return;
            }

            // Load our Model
            PlayerRankingsModel Model = new PlayerRankingsModel(Client);
            Model.Player = Rows[0];
            Model.SearchBarValue = Pid.ToString();

            #region Player Rankings

            // Global Score
            RankingPosition Score = new RankingPosition() { RankingType = "score" };
            string query = "SELECT COUNT(id) FROM player WHERE score > @P0";
            Score.Value = Model.FormatNumber(Int32.Parse(Model.Player["score"].ToString()));
            Score.Position = Database.ExecuteScalar<int>(query, Model.Player["score"]) + 1;
            Score.CtrPosition = Database.ExecuteScalar<int>(query + " AND country=@P1", Model.Player["score"], Model.Player["country"]) + 1;
            Score.PageNumber = GetPage(Score.Position);
            Score.CtrPageNumber = GetPage(Score.CtrPosition);
            Model.Rankings.Add(Score);

            // Score Per Min
            Score = new RankingPosition() { RankingType = "spm" };
            query = "SELECT score / (time / 60.0) AS spm FROM player WHERE id=@P0";
            Score.Value = Math.Round(Database.ExecuteScalar<decimal>(query, Pid), 4);

            query = "SELECT COUNT(id), score / (time / 60.0) AS spm FROM player WHERE spm > " + Score.Value;
            Score.Position = Database.ExecuteScalar<int>(query) + 1;
            Score.CtrPosition = Database.ExecuteScalar<int>(query + " AND country=@P0", Model.Player["country"]) + 1;
            Score.PageNumber = GetPage(Score.Position);
            Score.CtrPageNumber = GetPage(Score.CtrPosition);
            Model.Rankings.Add(Score);

            // Win-Loss Ratio
            Score = new RankingPosition() { RankingType = "wlr" };
            query = "SELECT (wins * 1.0 / losses) AS wlr FROM player WHERE id=@P0";
            Score.Value = Math.Round(Database.ExecuteScalar<decimal>(query, Pid), 4);

            query = "SELECT COUNT(id), (wins * 1.0 / losses) AS wlr FROM player WHERE wlr > " + Score.Value;
            Score.Position = Database.ExecuteScalar<int>(query) + 1;
            Score.CtrPosition = Database.ExecuteScalar<int>(query + " AND country=@P0", Model.Player["country"]) + 1;
            Score.PageNumber = GetPage(Score.Position);
            Score.CtrPageNumber = GetPage(Score.CtrPosition);
            Model.Rankings.Add(Score);

            // K/D Ratio
            Score = new RankingPosition() { RankingType = "kdr" };
            query = "SELECT (kills * 1.0 / deaths) AS value FROM player WHERE id=@P0";
            Score.Value = Math.Round(Database.ExecuteScalar<decimal>(query, Pid), 4);

            query = "SELECT COUNT(id), (kills * 1.0 / deaths) AS value FROM player WHERE value > " + Score.Value;
            Score.Position = Database.ExecuteScalar<int>(query) + 1;
            Score.CtrPosition = Database.ExecuteScalar<int>(query + " AND country=@P0", Model.Player["country"]) + 1;
            Score.PageNumber = GetPage(Score.Position);
            Score.CtrPageNumber = GetPage(Score.CtrPosition);
            Model.Rankings.Add(Score);

            // Knife Ratio
            Score = new RankingPosition() { RankingType = "knife_kdr" };
            query = "SELECT (knifekills * 1.0 / knifedeaths) AS value FROM weapons WHERE id=@P0";
            Score.Value = Math.Round(Database.ExecuteScalar<decimal>(query, Pid), 4);

            query = "SELECT COUNT(id), (knifekills * 1.0 / knifedeaths) AS value FROM weapons WHERE value > " + Score.Value;
            Score.Position = Database.ExecuteScalar<int>(query) + 1;
            query = "SELECT COUNT(w.id), (w.knifekills * 1.0 / w.knifedeaths) AS value "
                + "FROM weapons AS w "
                + "JOIN player AS p "
                + "WHERE p.id = w.id AND p.country=@P0 AND value > " + Score.Value;
            Score.CtrPosition = Database.ExecuteScalar<int>(query, Model.Player["country"]) + 1;
            Score.PageNumber = GetPage(Score.Position);
            Score.CtrPageNumber = GetPage(Score.CtrPosition);
            Model.Rankings.Add(Score);

            // Sniper Accuracy
            Score = new RankingPosition() { RankingType = "sniper_acc" };
            query = "SELECT (hit4 * 1.0 / fired4) AS value FROM weapons WHERE id=@P0";
            Score.Value = Math.Round(Database.ExecuteScalar<decimal>(query, Pid), 4);

            query = "SELECT COUNT(id), (hit4 * 1.0 / fired4) AS value FROM weapons WHERE value > " + Score.Value;
            Score.Position = Database.ExecuteScalar<int>(query) + 1;
            query = "SELECT COUNT(w.id), (w.hit4 * 1.0 / w.fired4) AS value "
                + "FROM weapons AS w "
                + "JOIN player AS p "
                + "WHERE p.id = w.id AND p.country=@P0 AND value > " + Score.Value;
            Score.CtrPosition = Database.ExecuteScalar<int>(query, Model.Player["country"]) + 1;
            Score.PageNumber = GetPage(Score.Position);
            Score.CtrPageNumber = GetPage(Score.CtrPosition);
            Model.Rankings.Add(Score);

            // Hours Per Day 
            // Timeframe: ((Last Online timstamp - Joined Timestamp) / 1 day (86400 seconds)) Gets the timespan of days played
            // Divide Timeframe by: hours played (seconds played (`time` column) / 1 hr (3600 seconds))
            Score = new RankingPosition() { RankingType = "hpd" };
            query = "SELECT (time / 3600.0) / ((lastonline - joined) / 86400.0) AS value FROM player WHERE id=@P0";
            Score.Value = Math.Round(Database.ExecuteScalar<decimal>(query, Pid), 4);

            query = "SELECT COUNT(id), (time / 3600.0) / ((lastonline - joined) / 86400.0) AS value FROM player WHERE value > " + Score.Value;
            Rows = Database.Query(query);
            Score.Position = Database.ExecuteScalar<int>(query) + 1;
            Score.CtrPosition = Database.ExecuteScalar<int>(query + " AND country=@P0", Model.Player["country"]) + 1;
            Score.PageNumber = GetPage(Score.Position);
            Score.CtrPageNumber = GetPage(Score.CtrPosition);
            Model.Rankings.Add(Score);

            // Command Score
            Score = new RankingPosition() { RankingType = "command" };
            query = "SELECT COUNT(id) FROM player WHERE cmdscore > @P0";
            Score.Value = Model.FormatNumber(Int32.Parse(Model.Player["cmdscore"].ToString()));
            Score.Position = Database.ExecuteScalar<int>(query, Model.Player["cmdscore"]) + 1;
            Score.CtrPosition = Database.ExecuteScalar<int>(query + " AND country=@P1", Model.Player["cmdscore"], Model.Player["country"]) + 1;
            Score.PageNumber = GetPage(Score.Position);
            Score.CtrPageNumber = GetPage(Score.CtrPosition);
            Model.Rankings.Add(Score);

            // Relative Command Score
            Score = new RankingPosition() { RankingType = "rcmds" };
            query = "SELECT COALESCE((cmdscore * 1.0 / cmdtime), 0.0) AS value FROM player WHERE id=@P0";
            Score.Value = Math.Round(Database.ExecuteScalar<decimal>(query, Pid), 4);

            query = "SELECT COUNT(id), COALESCE((cmdscore * 1.0 / cmdtime), 0.0) AS value FROM player WHERE value > " + Score.Value;
            Score.Position = Database.ExecuteScalar<int>(query) + 1;
            Score.CtrPosition = Database.ExecuteScalar<int>(query + " AND country=@P0", Model.Player["country"]) + 1;
            Score.PageNumber = GetPage(Score.Position);
            Score.CtrPageNumber = GetPage(Score.CtrPosition);
            Model.Rankings.Add(Score);

            #endregion

            #region Player Stats

            // Define our play time
            TimeSpan PlayerTime = TimeSpan.FromSeconds(Int32.Parse(Model.Player["time"].ToString()));
            int Kills = Int32.Parse(Model.Player["kills"].ToString());
            int Deaths = Int32.Parse(Model.Player["deaths"].ToString());
            int Rounds = Int32.Parse(Model.Player["wins"].ToString()) + Int32.Parse(Model.Player["losses"].ToString());
            decimal Result;

            // Player Stats
            foreach (string Query in StatsQueries)
            {
                PlayerRankingStats Stats = new PlayerRankingStats();
                Result = Database.ExecuteScalar<decimal>(Query, Pid);
                Stats.PerMinute = (PlayerTime.TotalMinutes > 0) ? Math.Round(Result / (decimal)PlayerTime.TotalMinutes, 4) : Result;
                Stats.PerDay = (PlayerTime.TotalDays > 0) ? Math.Round(Result / (decimal)PlayerTime.TotalDays, 4) : Result;
                Stats.PerHour = (PlayerTime.TotalHours > 0) ? Math.Round(Result / (decimal)PlayerTime.TotalHours, 4) : Result;
                Stats.PerRound = (Rounds > 0) ? Math.Round(Result / Rounds, 4) : Result;
                Stats.PerKill = (Kills > 0) ? Math.Round(Result / Kills, 4) : Result;
                Stats.PerDeath = (Deaths > 0) ? Math.Round(Result / Deaths, 4) : Result;
                Model.Stats.Add(Stats);
            }

            #endregion

            // Send the response
            base.SendTemplateResponse("player_rankings", typeof(PlayerRankingsModel), Model, Pid + "_rankings");
        }

        /// <summary>
        /// Returns the page number that the players stat would appear
        /// </summary>
        /// <param name="index">The player position</param>
        /// <returns></returns>
        public int GetPage(int index)
        {
            return 1 + (index / RankingsController.PlayersPerPage);
        }

        /// <summary>
        /// Since secondary weapons (such as knife and c4) use column prefix's
        /// in the database, we use this method to return it based on weapons ID
        /// </summary>
        /// <param name="WeaponId"></param>
        /// <returns></returns>
        private static string GetWeaponTblPrefix(int WeaponId)
        {
            switch (WeaponId)
            {
                case 9:
                    return "knife";
                case 10:
                    return "c4";
                case 11:
                    return "claymore";
                case 12:
                    return "handgrenade";
                case 13:
                    return "shockpad";
                case 14:
                    return "atmine";
                default:
                    return "";
            }
        }
    }
}
