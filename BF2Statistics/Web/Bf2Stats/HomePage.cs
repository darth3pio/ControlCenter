using System.Collections.Generic;
using BF2Statistics.Database;

namespace BF2Statistics.Web.Bf2Stats
{
    partial class HomePage
    {
        /// <summary>
        /// The page title
        /// </summary>
        public string Title = Program.Config.BF2S_Title;

        /// <summary>
        /// The HttpClient Object
        /// </summary>
        public HttpClient Client;

        /// <summary>
        /// The Root URL used for links
        /// </summary>
        protected string Root;

        /// <summary>
        /// The array of 50 players used to display on the home page
        /// </summary>
        protected List<Dictionary<string, object>> Players;

        public HomePage(HttpClient Client, StatsDatabase Database)
        {
            this.Client = Client;
            this.Root = "http://" + this.Client.Request.Url.DnsSafeHost + "/bf2stats";

            Client.Response.ContentType = "text/html";
            Players = Database.Query(
                "SELECT id, name, rank, score, kills, country, time FROM player WHERE score > 0 ORDER BY score DESC LIMIT " 
                + Program.Config.BF2S_LeaderCount
            );
        }
    }
}
