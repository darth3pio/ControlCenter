using System;
using System.Collections.Generic;
using System.Data.Common;
using BF2Statistics.Database;
using BF2Statistics.Database.QueryBuilder;

namespace BF2Statistics.ASP.Requests
{
    class GetClanInfo
    {
        protected DatabaseDriver Driver;

        /// <summary>
        /// This request provides claninfo to the bf2server, to set the filter rules
        /// for players joining the server.
        /// </summary>
        /// <queryParam name="type" type="int">The Listype (whitelist, blacklist)</queryParam>
        /// <queryParam name ="clantag" type="string">Specified the required clantag</queryParam>
        /// <queryParam name="score" type="int">The minimum required score</queryParam>
        /// <queryParam name="rank" type="int">The minimum required ranked</queryParam>
        /// <queryParam name="time" type="int">The minimum required global time</queryParam>
        /// <queryParam name="kdratio" type="float">The minimum required kill/death ratio</queryParam>
        /// <queryParam name="country" type="string">The country code (Ex: us, br, no) required, seperated by comma, that is required</queryParam>
        /// <queryParam name="banned" type="int">Specified the maximum ban count to be accepted into the list</queryParam>
        /// <param name="Client">The HttpClient who made the request</param>
        /// <param name="Driver">The Stats Database Driver. Connection errors are handled in the calling object</param>
        public GetClanInfo(HttpClient Client, StatsDatabase Database)
        {
            int Type = 0;
            Dictionary<string, string> QueryString = Client.Request.QueryString;
            Driver = Database;

            // make sure we have a valid player ID
            if (!QueryString.ContainsKey("type") || !Int32.TryParse(QueryString["type"], out Type))
            {
                Client.Response.WriteResponseStart(false);
                Client.Response.WriteHeaderLine("asof", "err");
                Client.Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp(), "Invalid Syntax!");
                Client.Response.Send();
                return;
            }

            // Filler Variables
            int I = 0;
            float F;
            string S;
            List<DbParameter> Params = new List<DbParameter>();

            // Prepare Query
            SelectQueryBuilder Query = new SelectQueryBuilder(Driver);
            Query.SelectColumns("id", "name");
            Query.SelectFromTable("player");
            Query.SetWhereOperator(LogicOperator.And);
            Query.AddWhere("ip", Comparison.NotEqualTo, "0.0.0.0");
            Query.AddOrderBy("id", Sorting.Ascending);
            WhereClause Where = null;

            switch (Type)
            {
                // Blacklist
                case 0:
                    int BanLimit = (QueryString.ContainsKey("banned") && Int32.TryParse(QueryString["banned"], out I)) ? I : 100;
                    Where = new WhereClause("banned", Comparison.GreaterOrEquals, BanLimit);
                    Where.AddClause(LogicOperator.Or, "permban", Comparison.Equals, 1);
                    break;
                // Whitelist
                case 1:
                    if (QueryString.ContainsKey("clantag"))
                    {
                        Where = new WhereClause("clantag", Comparison.Equals, QueryString["clantag"]);
                        Where.AddClause(LogicOperator.And, "permban", Comparison.Equals, 0);
                    }
                    break;
                // Greylist
                case 2:
                    // List of possible query's
                    string[] Queries = new string[] { "score", "rank", "time", "kdratio", "country", "banned" };
                    foreach (string Param in Queries)
                    {
                        if (QueryString.ContainsKey(Param))
                        {
                            switch (Param)
                            {
                                case "score":
                                case "time":
                                case "rank":
                                    if (Int32.TryParse(QueryString[Param], out I))
                                    {
                                        if (Where == null)
                                            Where = new WhereClause(Param, Comparison.GreaterOrEquals, I);
                                        else
                                            Where.AddClause(LogicOperator.And, Param, Comparison.GreaterOrEquals, I);
                                    }
                                    break;
                                case "kdratio":
                                    if (float.TryParse(QueryString["kdratio"], out F))
                                    {
                                        if (Where == null)
                                            Where = new WhereClause("(kills / deaths)", Comparison.GreaterOrEquals, F);
                                        else
                                            Where.AddClause(LogicOperator.And, "(kills / deaths)", Comparison.GreaterOrEquals, F);
                                    }
                                    break;
                                case "country":
                                    S = QueryString["country"].Replace(",", "','");
                                    if (Where == null)
                                        Where = new WhereClause(Param, Comparison.In, S.Split(','));
                                    else
                                        Where.AddClause(LogicOperator.And, Param, Comparison.In, S.Split(','));
                                    break;
                                case "banned":
                                    if (Int32.TryParse(QueryString["banned"], out I))
                                    {
                                        if(Where == null)
                                            Where = new WhereClause("banned", Comparison.LessThan, I);
                                        else
                                            Where.AddClause(LogicOperator.And,"banned", Comparison.LessThan, I);

                                        Where.AddClause(LogicOperator.And, "permban", Comparison.Equals, 0);
                                    }
                                    break;
                            }
                        }
                    }
                    break;
            }

            // Pepare 2 output headers
            int size = 0;
            FormattedOutput Output1 = new FormattedOutput("size", "asof");
            FormattedOutput Output2 = new FormattedOutput("pid", "nick");

            // Query the database, add each player to Output 2
            if(Where != null) Query.AddWhere(Where);
            List<Dictionary<string, object>> Players = Driver.ExecuteReader(Query.BuildCommand());
            foreach (Dictionary<string, object> P in Players)
            {
                size++;
                Output2.AddRow(P["id"].ToString(), P["name"].ToString());
            }

            // Send Response
            Output1.AddRow(size, DateTime.UtcNow.ToUnixTimestamp());
            Client.Response.AddData(Output1);
            Client.Response.AddData(Output2);
            Client.Response.Send();
        }
    }
}
