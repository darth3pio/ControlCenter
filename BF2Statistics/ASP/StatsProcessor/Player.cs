using System;
using System.Collections.Generic;
using System.Net;

namespace BF2Statistics.ASP.StatsProcessor
{
    /// <summary>
    /// This class is used to convert the Key => value data
    /// for a player in a Snapshot, into an object, as well
    /// as Validate that all the player data is valid.
    /// </summary>
    public class Player
    {
        #region Vars

        /// <summary>
        /// The players PID
        /// </summary>
        public int Pid { get; protected set; }

        /// <summary>
        /// The players Name
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// The players Rank at the end of the round
        /// </summary>
        public int Rank { get; protected set; }

        /// <summary>
        /// Specifies the IP address used by the player this round
        /// </summary>
        public IPAddress IpAddress { get; protected set; }

        /// <summary>
        /// Specifies if this player is an AI controlled player
        /// </summary>
        public bool IsAI { get; protected set; }

        /// <summary>
        /// Indicates the players overall score at the end of the round
        /// </summary>
        public int RoundScore { get; protected set; }

        /// <summary>
        /// Indicates the players round time played in seconds
        /// </summary>
        public int RoundTime { get; protected set; }

        /// <summary>
        /// Indicates the players command score at the end of the round
        /// </summary>
        public int CommandScore { get; protected set; }

        /// <summary>
        /// Indicates the players skill score at the end of the round
        /// </summary>
        public int SkillScore { get; protected set; }

        /// <summary>
        /// Indicates the players team score at the end of the round
        /// </summary>
        public int TeamScore { get; protected set; }

        /// <summary>
        /// Indicates the Army ID this player was playing as
        /// </summary>
        public int ArmyId { get; protected set; }

        /// <summary>
        /// Indicates the Team the player was on
        /// </summary>
        public int Team { get; protected set; }

        /// <summary>
        /// Returns whether or not the player was present at the end of the round
        /// </summary>
        public bool CompletedRound { get; protected set; }

        /// <summary>
        /// Returns the Time played as a Commander this round
        /// </summary>
        public int CmdTime { get; protected set; }

        /// <summary>
        /// Returns the Time played as a Squad Leader this round
        /// </summary>
        public int SqlTime { get; protected set; }

        /// <summary>
        /// Returns the Time played as a Squad Member this round
        /// </summary>
        public int SqmTime { get; protected set; }

        /// <summary>
        /// Returns the Time played as a Lone Wolf this round
        /// </summary>
        public int LwTime { get; protected set; }

        /// <summary>
        /// Indicates the number of times this player was kicked from the server
        /// </summary>
        public int TimesKicked { get; protected set; }

        /// <summary>
        /// Indicates the number of times this player was banned
        /// </summary>
        public int TimesBanned { get; protected set; }

        /// <summary>
        /// A list of players that were killed by this player [VictimPid => KillCount]
        /// </summary>
        public Dictionary<string, string> Victims { get; protected set; }

        /// <summary>
        /// An object containing the player's statistical round data
        /// </summary>
        public RoundStats Stats;

        /// <summary>
        /// Returns the players weapon data and scores
        /// </summary>
        public ObjectStat[] WeaponData = new ObjectStat[18];

        /// <summary>
        /// Returns the players kit data and scores
        /// </summary>
        public ObjectStat[] KitData = new ObjectStat[7];

        /// <summary>
        /// Returns the players vehicle data and scores
        /// </summary>
        public ObjectStat[] VehicleData = new ObjectStat[8];

        /// <summary>
        /// An array containing the seconds played as each army by ID
        /// </summary>
        public int[] TimeAsArmy { get; protected set; }

        /// <summary>
        /// A list of awards the player earned during the round [AwardId => AwardLevel]
        /// </summary>
        public Dictionary<int, int> EarnedAwards = new Dictionary<int, int>();

        #endregion Vars

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="PlayerData">The snapshot player data</param>
        /// <param name="PlayerKillData">The snapshot player kill data</param>
        public Player(Dictionary<string, string> PlayerData, Dictionary<string, string> PlayerKillData)
        {
            // Set internal dictionary data
            this.Victims = new Dictionary<string, string>(PlayerKillData);

            // Set Internal Variables
            this.Pid = Int32.Parse(PlayerData["pID"]);
            this.Name = PlayerData["name"];
            this.Rank = Int32.Parse(PlayerData["rank"]);
            this.RoundScore = Int32.Parse(PlayerData["rs"]);
            this.RoundTime = Int32.Parse(PlayerData["ctime"]);
            this.CommandScore = Int32.Parse(PlayerData["cs"]);
            this.SkillScore = Int32.Parse(PlayerData["ss"]);
            this.TeamScore = Int32.Parse(PlayerData["ts"]);
            this.ArmyId = Int32.Parse(PlayerData["a"]);
            this.Team = Int32.Parse(PlayerData["t"]);
            this.CompletedRound = (Int32.Parse(PlayerData["c"]) == 1);
            this.CmdTime = Int32.Parse(PlayerData["tco"]);
            this.SqlTime = Int32.Parse(PlayerData["tsl"]);
            this.SqmTime = Int32.Parse(PlayerData["tsm"]);
            this.LwTime = Int32.Parse(PlayerData["tlw"]);
            this.TimesKicked = Int32.Parse(PlayerData["kck"]);
            this.TimesBanned = Int32.Parse(PlayerData["ban"]);
            this.IsAI = (PlayerData["ai"] == "1");

            // Get Ip address
            IPAddress Addy = IPAddress.Loopback;
            IPAddress.TryParse(PlayerData["ip"], out Addy);
            this.IpAddress = Addy;

            // Sometimes Squad times are negative.. idk why, but we need to fix that here
            if (this.SqlTime < 0) this.SqlTime = 0;
            if (this.SqmTime < 0) this.SqmTime = 0;
            if (this.LwTime < 0) this.LwTime = 0;

            // Record players round stats
            this.Stats = new RoundStats()
            {
                Kills = Int32.Parse(PlayerData["kills"]),
                Deaths = Int32.Parse(PlayerData["deaths"]),
                Suicides = Int32.Parse(PlayerData["su"]),
                KillStreak = Int32.Parse(PlayerData["ks"]),
                DeathStreak = Int32.Parse(PlayerData["ds"]),
                Heals = Int32.Parse(PlayerData["he"]),
                Revives = Int32.Parse(PlayerData["rev"]),
                Repairs = Int32.Parse(PlayerData["rep"]),
                Ammos = Int32.Parse(PlayerData["rsp"]),
                FlagCaptures = Int32.Parse(PlayerData["cpc"]),
                FlagCaptureAssists = Int32.Parse(PlayerData["cpa"]),
                FlagDefends = Int32.Parse(PlayerData["cpd"]),
                DamageAssists = Int32.Parse(PlayerData["ka"]),
                TargetAssists = Int32.Parse(PlayerData["tre"]),
                DriverSpecials = Int32.Parse(PlayerData["drs"]),
                TeamKills = Int32.Parse(PlayerData["tmkl"]),
                TeamDamage = Int32.Parse(PlayerData["tmdg"]),
                TeamVehicleDamage = Int32.Parse(PlayerData["tmvd"])
            };

            // Extract Army Data
            this.TimeAsArmy = new int[14];
            for(int i = 0; i < 14; i++)
                this.TimeAsArmy[i] = Int32.Parse(PlayerData["ta" + i]);

            // Extract Kit Data
            for (int i = 0; i < 7; i++)
            {
                this.KitData[i] = new ObjectStat()
                {
                    Time = Int32.Parse(PlayerData["tk" + i]),
                    Kills = Int32.Parse(PlayerData["kk" + i]),
                    Deaths = Int32.Parse(PlayerData["dk" + i])
                };
            }

            // Extract Vehicle Data
            for (int i = 0; i < 7; i++)
            {
                this.VehicleData[i] = new ObjectStat()
                {
                    Time = Int32.Parse(PlayerData["tv" + i]),
                    Kills = Int32.Parse(PlayerData["kv" + i]),
                    Deaths = Int32.Parse(PlayerData["bv" + i]),
                    RoadKills = Int32.Parse(PlayerData["kvr" + i])
                };
            }

            // Add parachute
            this.VehicleData[7] = new ObjectStat() { Time = Int32.Parse(PlayerData["tvp"]) };

            // Extract Weapon Data
            for (int i = 0; i < 9; i++)
            {
                this.WeaponData[i] = new ObjectStat()
                {
                    Time = Int32.Parse(PlayerData["tw" + i]),
                    Kills = Int32.Parse(PlayerData["kw" + i]),
                    Deaths = Int32.Parse(PlayerData["bw" + i]),
                    Fired = Int32.Parse(PlayerData["sw" + i]),
                    Hits = Int32.Parse(PlayerData["hw" + i])
                };
            }

            // Extract more weapon data (Keys change)
            int j = 9;
            for (int i = 0; i < 9; i++)
            {
                if (i < 6)
                {
                    this.WeaponData[j] = new ObjectStat()
                    {
                        Time = Int32.Parse(PlayerData["te" + i]),
                        Kills = Int32.Parse(PlayerData["ke" + i]),
                        Deaths = Int32.Parse(PlayerData["be" + i]),
                        Fired = Int32.Parse(PlayerData["se" + i]),
                        Hits = Int32.Parse(PlayerData["he" + i])
                    };
                }
                else if (i == 6) // Tactical weapon
                {
                    this.WeaponData[j] = new ObjectStat()
                    {
                        Time = Int32.Parse(PlayerData["te6"]),
                        Deployed = Int32.Parse(PlayerData["de6"])
                    };
                }
                else // Grappling hook && Zipline
                {
                    int Be = (i == 7) ? 9 : 8; // Coding error in the python!
                    this.WeaponData[j] = new ObjectStat()
                    {
                        Time = Int32.Parse(PlayerData["te" + i]),
                        Deaths = Int32.Parse(PlayerData["be" + Be]),
                        Deployed = Int32.Parse(PlayerData["de" + i])
                    };
                }
                j++;
            }

            // Extract player awards
            foreach (KeyValuePair<string, string> Item in PlayerData)
            {
                if (AwardData.Awards.ContainsKey(Item.Key))
                    this.EarnedAwards.Add(AwardData.Awards[Item.Key], Int32.Parse(Item.Value));
            }
        }

        /// <summary>
        /// Sets the rank for this player
        /// </summary>
        /// <param name="Rank"></param>
        public void SetRank(int Rank)
        {
            this.Rank = Rank;
        }
    }

    public struct ObjectStat
    {
        public int Time;
        public int Kills;
        public int Deaths;
        public int Fired;
        public int Hits;
        public int RoadKills;
        public int Deployed;
    }

    public struct RoundStats
    {
        public int Kills;
        public int Deaths;
        public int Suicides;
        public int KillStreak;
        public int DeathStreak;

        public int Heals;
        public int Revives;
        public int Ammos;
        public int Repairs;

        public int FlagCaptures;
        public int FlagCaptureAssists;
        public int FlagDefends;

        public int DamageAssists;
        public int TargetAssists;
        public int DriverSpecials;

        public int TeamKills;
        public int TeamDamage;
        public int TeamVehicleDamage;
    }
}
