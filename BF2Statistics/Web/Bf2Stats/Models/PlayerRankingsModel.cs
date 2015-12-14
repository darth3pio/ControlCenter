using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2Statistics.Web.Bf2Stats
{
    public class PlayerRankingsModel : BF2PageModel
    {
        /// <summary>
        /// Player Data from the player Table
        /// </summary>
        public Dictionary<string, object> Player;

        public List<RankingPosition> Rankings = new List<RankingPosition>();

        public List<PlayerRankingStats> Stats = new List<PlayerRankingStats>();

        public readonly string[] RankingNames = new string[]
        {
            "Global Score",
            //"Accuracy",
            "Score per Minute",
            "Win-Loss Ratio",
            "Kill-Death Ratio",
            "Knife KDR",
            "Sniper Accuracy",
            "Hours per Day",
            "Command Score",
            "Relative Command Score"
        };

        public readonly string[] RankingDescriptions = new string[]
        {
            "Players ranked by global score, lowest to highest",
            //"Players ranked by best accuracy",
            "Players ranked by highest score per minute ratio",
            "Players ordered by their win to loss standing",
            "Players sorted by who get the most kills per death",
            "Players ranked by their skill with a knife",
            "Players ranked by their accuracy with a Sniper Rifle",
            "Players ranked the number of hours per day they play",
            "Players ranked the their Commander score",
            "Players ranked by Commander Points / Command score"
        };

        public readonly string[] StatsNames = new string[]
        {
            "Gold, Silver, Bronze Stars",
            "Awards",
            "Kills",
            "Deaths",
            "Time",
            "Team Kills",
            "Flag points",
            "Team Points",
            "Combat Points",
            "Score",
        };


        public PlayerRankingsModel(HttpClient Client) : base(Client)  { }

        public string FormatNumber(decimal Num)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:n4}", Decimal.Parse(Num.ToString()));
        }
    }

    public struct RankingPosition
    {
        public string RankingType;

        public int PageNumber;

        public int CtrPageNumber;

        public int Position;

        public int CtrPosition;

        public object Value;
    }

    public struct PlayerRankingStats
    {
        public decimal PerRound;

        public decimal PerMinute;

        public decimal PerHour;

        public decimal PerDay;

        public decimal PerKill;

        public decimal PerDeath;
    }
}
