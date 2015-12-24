using System;
using System.Collections.Generic;
using BF2Statistics.Database;
using BF2Statistics.Database.QueryBuilder;

namespace BF2Statistics.Web.ASP
{
    /// <summary>
    /// /ASP/searchforplayers.aspx
    /// </summary>
    /// <queryParam name="nick">partial or full name of the player search</queryParam>
    /// <queryParam name="sort">"a" (alpha/numeric), "r" (reverse), if not set, sorted by ASCII worth</queryParam>
    /// <queryParam name="where">"a" (any), "b" (begin), "e" (end), "x" (exact)</queryParam>
    /// <seealso cref="http://bf2tech.org/index.php/BF2_Statistics#Function:_searchforplayers"/>
    public sealed class SearchForPlayers : ASPController
    {
        public SearchForPlayers(HttpClient Client) : base(Client) { }

        public override void HandleRequest()
        {
            // Setup Params
            if (!Request.QueryString.ContainsKey("nick"))
            {
                Response.WriteResponseStart(false);
                Response.WriteHeaderLine("asof", "err");
                Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp(), "Invalid Syntax!");
                Response.Send();
                return;
            }

            // NOTE: The HttpServer will handle the DbConnectException
            using (Database = new StatsDatabase())
            {
                // Setup local vars
                int i = 0;
                string Nick = Request.QueryString["nick"];
                string Sort = (Request.QueryString.ContainsKey("sort")) ? Request.QueryString["sort"] : "a";
                string Where = (Request.QueryString.ContainsKey("where")) ? Request.QueryString["where"] : "a";

                // Timestamp Header
                Response.WriteResponseStart();
                Response.WriteHeaderLine("asof");
                Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp());

                // Build our query builder
                SelectQueryBuilder builder = new SelectQueryBuilder(Database);
                builder.SelectColumns("id", "name", "score");
                builder.SelectFromTable("player");
                builder.Limit(20);

                // Where statement for our query
                switch (Where.ToLowerInvariant())
                {
                    default:
                    case "a": // Any
                        builder.AddWhere("name", Comparison.Like, "%" + Nick + "%");
                        break;
                    case "b": // Begins With
                        builder.AddWhere("name", Comparison.Like, "%" + Nick);
                        break;
                    case "e": // Ends With
                        builder.AddWhere("name", Comparison.Like, Nick + "%");
                        break;
                    case "x": // Exactly
                        builder.AddWhere("name", Comparison.Equals, Nick);
                        break;
                }

                // Add sorting (a = ascending, r = reverse (descending))
                if (Sort.Equals("r", StringComparison.InvariantCultureIgnoreCase))
                    builder.AddOrderBy("name", Sorting.Descending);
                else
                    builder.AddOrderBy("name", Sorting.Ascending);

                // Output status
                Response.WriteHeaderLine("n", "pid", "nick", "score");
                foreach (Dictionary<string, object> Player in builder.ExecuteQuery())
                {
                    Response.WriteDataLine(++i, Player["id"], Player["name"].ToString().Trim(), Player["score"]);
                }

                // Send Response
                Response.Send();
            }
        }
    }
}
