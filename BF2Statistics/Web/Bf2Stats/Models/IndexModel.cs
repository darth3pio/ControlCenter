using System.Collections.Generic;

namespace BF2Statistics.Web.Bf2Stats
{
    public class IndexModel : BF2PageModel
    {
        /// <summary>
        /// The array of X players used to display on the home page
        /// </summary>
        public List<Dictionary<string, object>> Players;

        /// <summary>
        /// List of My leaderboard players. Only filled if home style is set to BF2s
        /// </summary>
        public List<PlayerResult> MyLeaderboardPlayers = new List<PlayerResult>();

        /// <summary>
        /// A list of servers to display on the Index.cshtml page
        /// </summary>
        public List<Server> Servers = new List<Server>();

        public IndexModel(HttpClient Client) : base(Client) { }
    }
}
