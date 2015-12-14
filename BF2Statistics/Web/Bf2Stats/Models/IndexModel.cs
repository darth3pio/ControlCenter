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

        public IndexModel(HttpClient Client) : base(Client) { }
    }
}
