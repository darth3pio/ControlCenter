using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2Statistics.Web.Bf2Stats
{
    public class PlayerHistoryModel : BF2PageModel
    {
        /// <summary>
        /// Player Data from the player Table
        /// </summary>
        public Dictionary<string, object> Player;

        public List<PlayerHistory> History;

        public PlayerHistoryModel(HttpClient Client) : base(Client)  { }
    }

    public struct PlayerHistory
    {
        public int Rank;

        public DateTime Date;

        public string MapName;

        public int Score;

        public int CmdScore;

        public int SkillScore;

        public int TeamScore;

        public int Kills;

        public int Deaths;

        public int TimePlayed;
    }
}
