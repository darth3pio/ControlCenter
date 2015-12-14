using System;
using System.Collections.Generic;

namespace BF2Statistics.Web.Bf2Stats
{
    public class LeaderboardModel : BF2PageModel
    {
        /// <summary>
        /// List of leaderboard players
        /// </summary>
        public List<PlayerResult> Players = new List<PlayerResult>();

        /// <summary>
        /// The value of the cookie
        /// </summary>
        public string CookieValue = String.Empty;

        public LeaderboardModel(HttpClient Client) : base(Client) { }
    }
}
