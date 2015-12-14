using System.Collections.Generic;

namespace BF2Statistics.Web.Bf2Stats
{
    public class RankingsModel : BF2PageModel
    {
        /// <summary>
        /// The array of parsed ranking objects
        /// </summary>
        public List<RankingStats> Stats = new List<RankingStats>();

        public RankingsModel(HttpClient Client) : base(Client) { }
    }

    public struct RankingStats
    {
        public string Name;
        public string Desc;
        public string UrlName;

        public List<Player> TopPlayers;
    }
}
