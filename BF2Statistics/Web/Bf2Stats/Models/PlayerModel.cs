using System;
using System.Collections.Generic;
using System.Globalization;

namespace BF2Statistics.Web.Bf2Stats
{
    public class PlayerModel : BF2PageModel
    {
        /// <summary>
        /// Player Data from the player Table
        /// </summary>
        public Dictionary<string, object> Player;

        /// <summary>
        /// Indicates whether this player is on the MyLeaderboard for this client
        /// </summary>
        public bool OnMyLeaderboard = false;

        /// <summary>
        /// Player image URL
        /// </summary>
        public string PlayerImage;

        #region Player Data

        public List<WeaponStat> WeaponData = new List<WeaponStat>(18);
        public List<WeaponStat> WeaponData2 = new List<WeaponStat>(3);
        public WeaponSummary WeaponSummary = new WeaponSummary();
        public WeaponSummary EquipmentSummary = new WeaponSummary();

        public List<ObjectStat> KitData = new List<ObjectStat>(7);
        public ObjectSummary KitSummary = new ObjectSummary();

        public List<ObjectStat> VehicleData = new List<ObjectStat>(8);
        public ObjectSummary VehicleSummary = new ObjectSummary();

        public List<ArmyMapStat> ArmyData = new List<ArmyMapStat>(Bf2StatsData.Armies.Count);
        public ArmyMapSummary ArmySummary = new ArmyMapSummary();

        public List<ArmyMapStat> MapData = new List<ArmyMapStat>(Bf2StatsData.Maps.Count);
        public ArmyMapSummary MapSummary = new ArmyMapSummary();

        public List<TheaterStat> TheaterData = new List<TheaterStat>(Bf2StatsData.TheatreMapIds.Count);

        public WeaponUnlock[] PlayerUnlocks = new WeaponUnlock[14];

        public Dictionary<string, List<Award>> PlayerBadges = new Dictionary<string, List<Award>>();
        public Dictionary<string, Award> PlayerMedals = new Dictionary<string, Award>();
        public Dictionary<string, Award> PlayerRibbons = new Dictionary<string, Award>();

        public Dictionary<string, List<Award>> PlayerSFBadges = new Dictionary<string, List<Award>>();
        public Dictionary<string, Award> PlayerSFMedals = new Dictionary<string, Award>();
        public Dictionary<string, Award> PlayerSFRibbons = new Dictionary<string, Award>();

        public Dictionary<string, int> ExpackTime = new Dictionary<string, int>(4)
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
        public Dictionary<string, int> Favorites = new Dictionary<string, int>();

        /// <summary>
        /// Favorite Map id differs from the index located in the Favorites map array, so the ID is defined here
        /// </summary>
        public int FavoriteMapId = 0;

        /// <summary>
        /// A list of the Players this player has killed the most
        /// </summary>
        public List<Player> TopVictims = new List<Player>();

        /// <summary>
        /// A list of players this player has been killed by the most
        /// </summary>
        public List<Player> TopEnemies = new List<Player>();

        /// <summary>
        /// A list of the Next player ranks to be earned (For Time To Advancement section)
        /// </summary>
        public List<Rank> NextPlayerRanks = new List<Rank>();

        /// <summary>
        /// Returns the Kill / Death ratio for this player
        /// </summary>
        public double KillDeathRatio
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
        public double WinLossRatio
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
        public int KillAssists
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
        public double ScorePerMin
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
        public double KillsPerMin
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
        public double KillsPerRound
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
        public double DeathsPerMin
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
        public double DeathsPerRound
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
        public double CostPerHour
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

        public PlayerModel(HttpClient Client) : base(Client) { }

        public string FormatTime(object Time)
        {
            return base.FormatTime(Int32.Parse(Time.ToString()));
        }

        public string FormatNumber(int Num)
        {
            return String.Format(SpecificCulture, "{0:n0}", Num);
        }

        public string FormatFloat(object Num, int Decimals)
        {
            return String.Format(SpecificCulture, "{0:F" + Decimals + "}", Num);
        }

        public string FormatDate(object Time)
        {
            DateTime T = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (T.AddSeconds(Int32.Parse(Time.ToString()))).ToString("yyyy-MM-dd HH:mm:ss", SpecificCulture);
        }

        public string FormatAwardDate(int Time)
        {
            int Sec = Int32.Parse(Time.ToString());
            if (Sec > 0)
            {
                DateTime T = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return " (<i>" + T.AddSeconds(Sec).ToString("MMMM dd, yyyy", SpecificCulture) + "</i>)";
            }
            else
                return "";
        }
    }
}
