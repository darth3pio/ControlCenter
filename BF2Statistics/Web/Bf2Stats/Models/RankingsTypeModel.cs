using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2Statistics.Web.Bf2Stats
{
    public class RankingsTypeModel : BF2PageModel
    {
        /// <summary>
        /// Gets the scoring type selected for a URL
        /// </summary>
        public string UrlName;

        /// <summary>
        /// Gets the Table header name for the scoring type
        /// </summary>
        public string ScoreHeader;

        /// <summary>
        /// Gets the current page number in our player results
        /// </summary>
        public int CurrentPage = 1;

        /// <summary>
        /// Gets the country code if we are filtering by country
        /// </summary>
        public string Country = String.Empty;

        /// <summary>
        /// Gets a list of country codes from the player base
        /// </summary>
        public List<string> CountryList = new List<string>();

        /// <summary>
        /// Gets the list of player records
        /// </summary>
        public List<PlayerRow> Records;

        /// <summary>
        /// Gets the total number of records that match our filtering
        /// </summary>
        public int TotalRecords;

        /// <summary>
        /// Gets the total number of page results
        /// </summary>
        public int TotalPages;

        public RankingsTypeModel(HttpClient Client) : base(Client) { }

        public string FormatNumber(int Num)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:##}", Int32.Parse(Num.ToString()));
        }

        /// <summary>
        /// Represents a player result for the scoring table
        /// </summary>
        public struct PlayerRow
        {
            /// <summary>
            /// Gets the player's PID
            /// </summary>
            public int Pid;

            /// <summary>
            /// Gets the player's name
            /// </summary>
            public string Name;

            /// <summary>
            /// Gets the players rank index
            /// </summary>
            public int Rank;

            /// <summary>
            /// Gets the players country code
            /// </summary>
            public string Country;

            /// <summary>
            /// Gets the players score per minute
            /// </summary>
            public double ScorePerMin;

            /// <summary>
            /// Gets the players win-loss ratio
            /// </summary>
            public double WinLossRatio;

            /// <summary>
            /// Gets the players kill-death ratio
            /// </summary>
            public double KillDeathRatio;

            /// <summary>
            /// Gets the player's time played in seconds
            /// </summary>
            public int Time;

            /// <summary>
            /// Gets the player's score value, which is based on the scoring type
            /// </summary>
            public string ScoreValue;
        }
    }
}
