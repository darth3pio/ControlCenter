using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using BF2Statistics.Database;

namespace BF2Statistics.Web.Bf2Stats
{
    partial class PlayerPage
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
        /// Our cache file object
        /// </summary>
        protected FileInfo CacheFile;

        /// <summary>
        /// Indicates whether we need to cache this file after its generated
        /// </summary>
        protected bool NeedsCached = false;

        /// <summary>
        /// Player image URL
        /// </summary>
        protected string PlayerImage;

        /// <summary>
        /// Player Data from the player Table
        /// </summary>
        protected Dictionary<string, object> Player;

        /// <summary>
        /// The player PID
        /// </summary>
        public int Pid = 0;

        #region Player Data

        protected List<WeaponStat> WeaponData = new List<WeaponStat>(18);
        protected List<WeaponStat> WeaponData2 = new List<WeaponStat>(3);
        protected WeaponSummary WeaponSummary = new WeaponSummary();
        protected WeaponSummary EquipmentSummary = new WeaponSummary();

        protected List<ObjectStat> KitData = new List<ObjectStat>(7);
        protected ObjectSummary KitSummary = new ObjectSummary();

        protected List<ObjectStat> VehicleData = new List<ObjectStat>(8);
        protected ObjectSummary VehicleSummary = new ObjectSummary();

        protected List<ArmyMapStat> ArmyData = new List<ArmyMapStat>(StatsData.Armies.Count);
        protected ArmyMapSummary ArmySummary = new ArmyMapSummary();

        protected List<ArmyMapStat> MapData = new List<ArmyMapStat>(StatsData.Maps.Count);
        protected ArmyMapSummary MapSummary = new ArmyMapSummary();

        protected List<TheaterStat> TheaterData = new List<TheaterStat>(StatsData.TheatreMapIds.Count);

        protected WeaponUnlock[] PlayerUnlocks = new WeaponUnlock[14];

        protected Dictionary<string, List<Award>> PlayerBadges = new Dictionary<string, List<Award>>();
        protected Dictionary<string, Award> PlayerMedals = new Dictionary<string, Award>();
        protected Dictionary<string, Award> PlayerRibbons = new Dictionary<string, Award>();

        protected Dictionary<string, List<Award>> PlayerSFBadges = new Dictionary<string, List<Award>>();
        protected Dictionary<string, Award> PlayerSFMedals = new Dictionary<string, Award>();
        protected Dictionary<string, Award> PlayerSFRibbons = new Dictionary<string, Award>();

        protected Dictionary<string, int> ExpackTime = new Dictionary<string, int>(4)
        {
            {"bf", 0},
            {"sf", 0},
            {"ef", 0},
            {"af", 0}
        };

        /// <summary>
        /// An list that contains the index of the player favorites in a catagory. these
        /// indexies are used to highlight player favorite rows
        /// </summary>
        protected Dictionary<string, int> Favorites = new Dictionary<string, int>();

        /// <summary>
        /// Favorite Map id differs from the index located in the Favorites map array, so the ID is defined here
        /// </summary>
        protected int FavoriteMapId = 0;

        /// <summary>
        /// A list of the Players this player has killed the most
        /// </summary>
        protected List<Player> TopVictims = new List<Player>();

        /// <summary>
        /// A list of players this player has been killed by the most
        /// </summary>
        protected List<Player> TopEnemies = new List<Player>();

        protected List<Rank> NextPlayerRanks = new List<Rank>();

        /// <summary>
        /// Returns the Kill / Death ratio for this player
        /// </summary>
        protected double KillDeathRatio
        {
            get
            {
                double Kills = Int32.Parse(Player["kills"].ToString());
                double Deaths = Int32.Parse(Player["deaths"].ToString());
                if (Deaths > 0)
                    return Math.Round(Kills / Deaths, 3);
                else
                    return Kills;
            }
        }

        /// <summary>
        /// Returns the Win Loss Ratio for this player
        /// </summary>
        protected double WinLossRatio
        {
            get
            {
                double Wins = Int32.Parse(Player["wins"].ToString());
                double Losses = Int32.Parse(Player["losses"].ToString());
                if (Losses > 0)
                    return Math.Round(Wins / Losses, 2);
                else
                    return Wins;
            }
        }

        /// <summary>
        /// Returns the amount of kill assists this player has
        /// </summary>
        protected int KillAssists
        {
            get
            {
                int da = Int32.Parse(Player["damageassists"].ToString());
                int ta = Int32.Parse(Player["targetassists"].ToString());
                return da + ta;
            }
        }

        /// <summary>
        /// Returns the score earned per minute
        /// </summary>
        protected double ScorePerMin
        {
            get
            {
                double Score = Int32.Parse(Player["score"].ToString());
                double Mins = Int32.Parse(Player["time"].ToString()) / 60;
                if (Mins > 0)
                    return Math.Round(Score / Mins, 4);
                else
                    return Score;
            }
        }

        /// <summary>
        /// Returns the kills earned per minute
        /// </summary>
        protected double KillsPerMin
        {
            get
            {
                double kills = Int32.Parse(Player["kills"].ToString());
                double time = Int32.Parse(Player["time"].ToString());
                if (kills > 0 && time > 0)
                {
                    time /= 60;
                    return Math.Round(kills / time, 3);
                }
                else
                    return 0.000;
            }
        }

        /// <summary>
        /// Returns the average kills per round
        /// </summary>
        protected double KillsPerRound
        {
            get
            {
                double kills = Int32.Parse(Player["kills"].ToString());
                int wins = Int32.Parse(Player["wins"].ToString());
                int losses = Int32.Parse(Player["losses"].ToString());
                double rounds = wins + losses;

                if (kills > 0 && rounds > 0)
                    return Math.Round(kills / rounds, 3);
                else
                    return 0.000;
            }
        }

        /// <summary>
        /// Returns the average deaths per minute
        /// </summary>
        protected double DeathsPerMin
        {
            get
            {
                double deaths = Int32.Parse(Player["deaths"].ToString());
                double time = Int32.Parse(Player["time"].ToString());
                if (deaths > 0 && time > 0)
                {
                    time /= 60;
                    return Math.Round(deaths / time, 3);
                }
                else
                    return 0.000;
            }
        }

        /// <summary>
        /// Returns the average deaths per round
        /// </summary>
        protected double DeathsPerRound
        {
            get
            {
                double deaths = Int32.Parse(Player["deaths"].ToString());
                int wins = Int32.Parse(Player["wins"].ToString());
                int losses = Int32.Parse(Player["losses"].ToString());
                double rounds = wins + losses;

                if (deaths > 0 && rounds > 0)
                    return Math.Round(deaths / rounds, 3);
                else
                    return 0.000;
            }
        }

        /// <summary>
        /// Returns the estimated cost per hour
        /// </summary>
        protected double CostPerHour
        {
            get
            {
                int T = Int32.Parse(Player["time"].ToString());
                if (T > 0)
                {
                    T /= 3600;
                    return Math.Round((double)50 / T, 4);
                }
                else
                    return 0.00;
            }
        }

        #endregion Player Data

        /// <summary>
        /// Constructor.. This is where the magic begins
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="Database"></param>
        public PlayerPage(HttpClient Client, StatsDatabase Database)
        {
            this.Client = Client;
            this.Root = "http://" + this.Client.Request.Url.DnsSafeHost + "/bf2stats";

            // Make sure we have a pid
            if (!Client.Request.QueryString.ContainsKey("pid"))
            {
                Client.Response.Redirect("/bf2stats");
                return;
            }

            // Fetch Player
            Int32.TryParse(Client.Request.QueryString["pid"], out Pid);
            List<Dictionary<string, object>> Rows = Database.Query("SELECT * FROM player WHERE id=@P0 AND score > 50", Pid);

            // Bad pid or player doesnt exist
            if (Pid == 0 || Rows.Count == 0)
            {
                Client.Response.Redirect("/bf2stats/search");
                return;
            }

            // Check cache
            if (MainForm.Config.BF2S_CacheEnabled)
            {
                CacheFile = new FileInfo(Path.Combine(Program.RootPath, "Web", "Bf2Stats", "Cache", Pid.ToString() + ".html"));
                if (!CacheFile.Exists || DateTime.Now.CompareTo(CacheFile.LastWriteTime.AddMinutes(30)) > 0)
                    this.NeedsCached = true;
                else
                    return; // We are reading from cache, nothing to do here
            }

            // Setup player variables
            Player = Rows[0];
            int PlayerKills = Int32.Parse(Player["kills"].ToString());
            double AcctsFor = 0;
            int j = 0;

            #region ArmyData
            // Fetch army data
            Rows = Database.Query("SELECT * FROM army WHERE id=@P0", Pid);
            foreach(KeyValuePair<int, string> Army in StatsData.Armies)
            {
                int Wins = Int32.Parse(Rows[0]["win" + Army.Key].ToString());
                int Losses = Int32.Parse(Rows[0]["loss" + Army.Key].ToString());
                ArmyData.Add(new ArmyMapStat
                {
                    Id = Army.Key,
                    Time = Int32.Parse(Rows[0]["time" + Army.Key].ToString()),
                    Wins = Wins,
                    Losses = Losses,
                    Best = Int32.Parse(Rows[0]["best" + Army.Key].ToString()),
                });

                ArmySummary.TotalWins += Wins;
                ArmySummary.TotalLosses += Losses;
                ArmySummary.TotalTime += ArmyData[j].Time;
                ArmySummary.TotalBest += ArmyData[j].Best;
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
                if (!StatsData.Maps.Keys.Contains(Int32.Parse(Row["mapid"].ToString())))
                    continue;

                int Wins = Int32.Parse(Row["win"].ToString());
                int Losses = Int32.Parse(Row["loss"].ToString());
                MapData.Add(new ArmyMapStat
                {
                    Id = Int32.Parse(Row["mapid"].ToString()),
                    Time = Int32.Parse(Row["time"].ToString()),
                    Wins = Wins,
                    Losses = Losses,
                    Best = Int32.Parse(Row["best"].ToString()),
                });

                MapSummary.TotalWins += Wins;
                MapSummary.TotalLosses += Losses;
                MapSummary.TotalTime += MapData[j].Time;
                MapSummary.TotalBest += MapData[j].Best;
                j++;
            }
            #endregion MapData

            #region TheaterData
            // Do Theater Data
            foreach(KeyValuePair<string, int[]> t in StatsData.TheatreMapIds)
            {
                j = 0;
                string inn = String.Join(",", t.Value);
                Rows = Database.Query(
                    "SELECT COALESCE(sum(time), 0) as time, COALESCE(sum(win), 0) as win, COALESCE(sum(loss), 0) as loss, COALESCE(max(best), 0) as best "
                    + "FROM maps WHERE mapid IN (" + inn + ") AND id=@P0", Pid
                );

                // 
                TheaterData.Add(new TheaterStat
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
                VehicleData.Add(new ObjectStat
                {
                    Time = Int32.Parse(Rows[0]["time" + i].ToString()),
                    Kills = Kills,
                    Deaths = Deaths,
                    RoadKills = RoadKills,
                    KillsAcctFor = (PlayerKills == 0 || Kills == 0) ? 0.00 : Math.Round(100 * ((double)(RoadKills + Kills) / PlayerKills), 2)
                });

                VehicleSummary.TotalKills += Kills;
                VehicleSummary.TotalDeaths += Deaths;
                VehicleSummary.TotalTime += VehicleData[i].Time;
                VehicleSummary.TotalRoadKills += VehicleData[i].RoadKills;
                AcctsFor += VehicleData[i].KillsAcctFor;
            }

            // Add para time
            VehicleData.Add(new ObjectStat { Time = Int32.Parse(Rows[0]["timepara"].ToString()) });
            VehicleSummary.KillsAcctFor = (AcctsFor > 0) ? Math.Round(AcctsFor / 7, 2) : 0.00;
            #endregion VehicleData

            #region XpackTime
            // Do Expansion time
            foreach (KeyValuePair<string, List<int>> t in StatsData.ModMapIds)
            {
                if (t.Value.Count > 0)
                {
                    string inn = String.Join(",", t.Value);
                    Rows = Database.Query("SELECT COALESCE(sum(time), 0) as time FROM maps WHERE mapid IN (" + inn + ") AND id=@P0", Pid);
                    if (ExpackTime.ContainsKey(t.Key))
                        ExpackTime[t.Key] = Int32.Parse(Rows[0]["time"].ToString());
                    else
                        ExpackTime.Add(t.Key, Int32.Parse(Rows[0]["time"].ToString()));
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
                KitData.Add(new ObjectStat
                {
                    Time = Int32.Parse(Rows[0]["time" + i].ToString()),
                    Kills = Kills,
                    Deaths = Deaths,
                    KillsAcctFor = (PlayerKills == 0 || Kills == 0) ? 0.00 : Math.Round(100 * ((double)Kills / PlayerKills), 2)
                });

                KitSummary.TotalKills += Kills;
                KitSummary.TotalDeaths += Deaths;
                KitSummary.TotalTime += KitData[i].Time;
                AcctsFor += KitData[i].KillsAcctFor;
            }
            KitSummary.KillsAcctFor = (AcctsFor > 0) ? Math.Round(AcctsFor / 7, 2) : 0.00;
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
                    WeaponData.Add(new WeaponStat
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
                    WeaponData.Add(new WeaponStat
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

            WeaponData.Add(new WeaponStat
            {
                Time = Int32.Parse(Rows[0]["tacticaltime"].ToString()),
                Fired = Int32.Parse(Rows[0]["tacticaldeployed"].ToString())
            });

            WeaponData.Add(new WeaponStat
            {
                Time = Int32.Parse(Rows[0]["grapplinghooktime"].ToString()),
                Deaths = Int32.Parse(Rows[0]["grapplinghookdeaths"].ToString()),
                Fired = Int32.Parse(Rows[0]["grapplinghookdeployed"].ToString())
            });

            WeaponData.Add(new WeaponStat
            {
                Time = Int32.Parse(Rows[0]["ziplinetime"].ToString()),
                Deaths = Int32.Parse(Rows[0]["ziplinedeaths"].ToString()),
                Fired = Int32.Parse(Rows[0]["ziplinedeployed"].ToString())
            });

            for (int i = 0; i < 17; i++)
            {
                WeaponSummary.TotalKills += WeaponData[i].Kills;
                WeaponSummary.TotalDeaths += WeaponData[i].Deaths;
                WeaponSummary.TotalTime += WeaponData[i].Time;
                WeaponSummary.TotalHits += WeaponData[i].Hits;
                WeaponSummary.TotalFired += WeaponData[i].Fired;
                AcctsFor += WeaponData[i].KillsAcctFor;

                if (i > 8)
                {
                    EquipmentSummary.TotalKills += WeaponData[i].Kills;
                    EquipmentSummary.TotalDeaths += WeaponData[i].Deaths;
                    EquipmentSummary.TotalTime += WeaponData[i].Time;
                    EquipmentSummary.TotalHits += WeaponData[i].Hits;
                    EquipmentSummary.TotalFired += WeaponData[i].Fired;
                    AcctsFor2 += WeaponData[i].KillsAcctFor;
                }
            }
            WeaponSummary.KillsAcctFor = (AcctsFor > 0) ? Math.Round(AcctsFor / 15, 2) : 0.00;
            EquipmentSummary.KillsAcctFor = (AcctsFor > 0) ? Math.Round(AcctsFor / 6, 2) : 0.00;

            // Extra weapon data DEFIB
            WeaponData2.Add(WeaponData[13]);

            // Extra Weapon data Explosives
            WeaponData2.Add(new WeaponStat
            {
                Time = WeaponData[10].Time + WeaponData[11].Time + WeaponData[14].Time,
                Kills = WeaponData[10].Kills + WeaponData[11].Kills + WeaponData[14].Kills,
                Deaths = WeaponData[10].Deaths + WeaponData[11].Deaths + WeaponData[14].Deaths,
                Hits = WeaponData[10].Hits + WeaponData[11].Hits + WeaponData[14].Hits,
                Fired = WeaponData[10].Fired + WeaponData[11].Fired + WeaponData[14].Fired,
                KillsAcctFor = WeaponData[10].KillsAcctFor + WeaponData[11].KillsAcctFor + WeaponData[14].KillsAcctFor
            });

            // Extra weapon data Grenade
            WeaponData2.Add(WeaponData[12]);

            #endregion WeaponData

            // Add Favorites
            Favorites.Add("army", (from x in ArmyData orderby x.Time select ArmyData.IndexOf(x)).Last());
            Favorites.Add("map", (from x in MapData orderby x.Time select MapData.IndexOf(x)).Last());
            Favorites.Add("theater", (from x in TheaterData orderby x.Time select TheaterData.IndexOf(x)).Last());
            Favorites.Add("vehicle", (from x in VehicleData orderby x.Time select VehicleData.IndexOf(x)).Last());
            Favorites.Add("kit", (from x in KitData orderby x.Time select KitData.IndexOf(x)).Last());
            Favorites.Add("weapon", (from x in WeaponData orderby x.Time select WeaponData.IndexOf(x)).Last());
            Favorites.Add("equipment", (from x in WeaponData orderby x.Time let index = WeaponData.IndexOf(x) where index > 8 select index).Last());
            FavoriteMapId = MapData[Favorites["map"]].Id;

            #region TopEnemy and Victim
            // Get top enemy's
            Rows = Database.Query(
                "SELECT attacker, count, t.name AS name, t.rank AS rank FROM kills JOIN player AS t WHERE t.id = attacker AND victim = @P0 ORDER BY count DESC LIMIT 11", 
                Pid
            );
            if (Rows.Count > 0)
            {
                TopEnemies.Add(new Player
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
                        TopEnemies.Add(new Player
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
                TopVictims.Add(new Player
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
                        TopVictims.Add(new Player
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
            foreach (Dictionary<string, object> Row in Rows)
            {
                PlayerUnlocks[j] = new WeaponUnlock
                {
                    Id = Int32.Parse(Row["kit"].ToString()),
                    Name = StatsData.Unlocks[Row["kit"].ToString()],
                    State = Row["state"].ToString()
                };
                j++;
            }
            #endregion Unlocks

            #region Badges
            // Fetch player badges
            foreach (KeyValuePair<string, string> Awd in StatsData.Badges)
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

                this.PlayerBadges.Add(Awd.Key, Badges);
            }

            // Fetch player badges
            foreach (KeyValuePair<string, string> Awd in StatsData.SfBadges)
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

                this.PlayerSFBadges.Add(Awd.Key, Badges);
            }
            #endregion Badges

            #region Medals

            // Fetch player medals
            foreach (KeyValuePair<string, string> Awd in StatsData.Medals)
            {
                int AwdId = Int32.Parse(Awd.Key);
                Rows = Database.Query("SELECT * FROM awards WHERE id=@P0 AND awd=@P1 LIMIT 1", Pid, AwdId);
                if (Rows.Count > 0)
                {
                    PlayerMedals.Add(Awd.Key, new Award
                    {
                        Id = AwdId,
                        Level = Int32.Parse(Rows[0]["level"].ToString()),
                        Earned = Int32.Parse(Rows[0]["earned"].ToString()),
                        First = Int32.Parse(Rows[0]["first"].ToString()),
                    });
                }
                else
                    PlayerMedals.Add(Awd.Key, new Award { Id = AwdId });

            }

            foreach (KeyValuePair<string, string> Awd in StatsData.SfMedals)
            {
                int AwdId = Int32.Parse(Awd.Key);
                Rows = Database.Query("SELECT * FROM awards WHERE id=@P0 AND awd=@P1 LIMIT 1", Pid, AwdId);
                if (Rows.Count > 0)
                {
                    PlayerSFMedals.Add(Awd.Key, new Award
                    {
                        Id = AwdId,
                        Level = Int32.Parse(Rows[0]["level"].ToString()),
                        Earned = Int32.Parse(Rows[0]["earned"].ToString()),
                        First = Int32.Parse(Rows[0]["first"].ToString()),
                    });
                }
                else
                    PlayerSFMedals.Add(Awd.Key, new Award { Id = AwdId });

            }

            #endregion Medals

            #region Ribbons

            // Fetch player ribbons
            foreach (KeyValuePair<string, string> Awd in StatsData.Ribbons)
            {
                int AwdId = Int32.Parse(Awd.Key);
                Rows = Database.Query("SELECT * FROM awards WHERE id=@P0 AND awd=@P1 LIMIT 1", Pid, AwdId);
                if (Rows.Count > 0)
                {
                    PlayerRibbons.Add(Awd.Key, new Award
                    {
                        Id = AwdId,
                        Level = Int32.Parse(Rows[0]["level"].ToString()),
                        Earned = Int32.Parse(Rows[0]["earned"].ToString()),
                        First = Int32.Parse(Rows[0]["first"].ToString()),
                    });
                }
                else
                    PlayerRibbons.Add(Awd.Key, new Award { Id = AwdId });

            }

            // Fetch SF Ribbons
            foreach (KeyValuePair<string, string> Awd in StatsData.SfRibbons)
            {
                int AwdId = Int32.Parse(Awd.Key);
                Rows = Database.Query("SELECT * FROM awards WHERE id=@P0 AND awd=@P1 LIMIT 1", Pid, AwdId);
                if (Rows.Count > 0)
                {
                    PlayerSFRibbons.Add(Awd.Key, new Award
                    {
                        Id = AwdId,
                        Level = Int32.Parse(Rows[0]["level"].ToString()),
                        Earned = Int32.Parse(Rows[0]["earned"].ToString()),
                        First = Int32.Parse(Rows[0]["first"].ToString()),
                    });
                }
                else
                    PlayerSFRibbons.Add(Awd.Key, new Award { Id = AwdId });

            }

            #endregion Ribbons

            #region Time To Advancement

            // Fetch all of our awards, so we can determine our qualified rank ups
            Rows = Database.Query("SELECT awd, level FROM awards WHERE id=@P0 ORDER BY level", Pid);
            Dictionary<string, int> Awds = new Dictionary<string, int>();
            foreach (Dictionary<string, object> Row in Rows)
            {
                if(!Awds.ContainsKey(Row["awd"].ToString()))
                    Awds.Add(Row["awd"].ToString(), Int32.Parse(Row["level"].ToString()));
            }

            // Get our next ranks
            int Score = Int32.Parse(Player["score"].ToString());
            NextPlayerRanks = RankCalculator.GetNext3Ranks(Score, Int32.Parse(Player["rank"].ToString()), Awds);
            foreach (Rank Rnk in NextPlayerRanks)
            {
                // Get Needed Points for this next rank
                int NP = Rnk.MinPoints - Score;
                Rnk.PointsNeeded = (NP > 0) ? NP : 0;

                // Get our percentage to this next rank based on needed points
                double Perc = Math.Round(((double)Score / Rnk.MinPoints) * 100, 0);
                Rnk.PercentComplete = (Perc > 100) ? 100 : Perc;

                // Get the time to completion, based on our score per minute
                double t = NP / (this.ScorePerMin / 60);
                if (t < 0) t = 0;
                Rnk.TimeToComplete = (int) t;

                // Get our days to completion time, based on our Join date, Last battle, and average Points per day
                TimeSpan Span = TimeSpan.FromSeconds(Int32.Parse(Player["lastonline"].ToString()) - Int32.Parse(Player["joined"].ToString()));
                double SPD = Math.Round(Score / Span.TotalDays, 0);
                Rnk.DaysToComplete = (int) Math.Round(NP / SPD, 0);
            }

            #endregion Time To Advancement

            // Set content type
            Client.Response.ContentType = "text/html";

            // Get player image
            PlayerImage = Path.Combine(
                Program.RootPath, "Web", "Bf2Stats", "Resources", "images", "soldiers", 
                Favorites["army"] + "_" + Favorites["kit"] + "_" + Favorites["weapon"] + ".jpg"
            );

            // Convert fav into a URL
            PlayerImage = (File.Exists(PlayerImage))
                ? this.Root + "/images/soldiers/" + Favorites["army"] + "_" + Favorites["kit"] + "_" + Favorites["weapon"] + ".jpg"
                : this.Root + "/images/soldiers/" + Favorites["army"] + "_" + Favorites["kit"] + "_5.jpg";
        }

        /// <summary>
        /// Using the Cache system, this method will return the Html contents of the generated page
        /// </summary>
        /// <returns></returns>
        public string TransformHtml()
        {
            if (MainForm.Config.BF2S_CacheEnabled)
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
                        Program.ErrorLog.Write("[PlayerPage.CreateCacheFile] " + e.Message);
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
                            return Reader.ReadToEnd();
                        }
                    }
                    catch (Exception e)
                    {
                        Program.ErrorLog.Write("[PlayerPage.ReadCacheFile] " + e.Message);
                        return "Cache Read Error";
                    }
                }
            }

            return TransformText();
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

        public string FormatTime(object Time)
        {
            TimeSpan Span = TimeSpan.FromSeconds(Int32.Parse(Time.ToString()));
            return String.Format("{0:00}:{1:00}:{2:00}", Span.TotalHours, Span.Minutes, Span.Seconds);
        }

        public string FormatTime(double Time)
        {
            TimeSpan Span = TimeSpan.FromSeconds(Time);
            return String.Format("{0:00}:{1:00}:{2:00}", Span.TotalHours, Span.Minutes, Span.Seconds);
        }

        public string FormatTime(int Time)
        {
            TimeSpan Span = TimeSpan.FromSeconds(Time);
            return String.Format("{0:00}:{1:00}:{2:00}", Span.TotalHours, Span.Minutes, Span.Seconds);
        }

        public string FormatNumber(object Num)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:n0}", Int32.Parse(Num.ToString()));
        }

        public string FormatNumber(int Num)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:n0}", Num);
        }

        public string FormatFloat(object Num, int Decimals)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:F" + Decimals + "}", Num);
        }

        public string FormatDate(object Time)
        {
            DateTime T = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (T.AddSeconds(Int32.Parse(Time.ToString()))).ToString("yyyy-MM-dd HH:mm:ss");
        }

        public string FormatAwardDate(int Time)
        {
            int Sec = Int32.Parse(Time.ToString());
            if (Sec > 0)
            {
                DateTime T = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return " (<i>" + T.AddSeconds(Sec).ToString("MMMM dd, yyyy") + "</i>)";
            }
            else
                return "";
        }
    }
}
