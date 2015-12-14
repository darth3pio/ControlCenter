using System;
using System.Collections.Generic;

namespace BF2Statistics.Web.Bf2Stats
{
    public class SearchModel : BF2PageModel
    {
        /// <summary>
        /// The search value
        /// </summary>
        public string SearchValue = String.Empty;

        /// <summary>
        /// If we have a search string. The results are stored here
        /// </summary>
        public List<PlayerResult> SearchResults = new List<PlayerResult>();

        public SearchModel(HttpClient Client) : base(Client) { }
    }
}
