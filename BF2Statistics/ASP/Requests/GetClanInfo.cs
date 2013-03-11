using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BF2Statistics.ASP.Requests
{
    class GetClanInfo
    {
        public GetClanInfo(ASPResponse Response, Dictionary<string, string> QueryString)
        {
            int Type = 0;

            // make sure we have a valid player ID
            if (!QueryString.ContainsKey("type") || !Int32.TryParse(QueryString["type"], out Type))
            {
                FormattedOutput Output = new FormattedOutput("asof", "err");
                Output.AddRow(Utils.UnixTimestamp(), "Invalid Syntax!");
                Response.AddData(Output);
                Response.IsValidData(false);
                Response.Send();
                return;
            }

            // Filler Variables
            string Where = "";
            int I = 0;
            float F;
            string S;

            switch (Type)
            {
                // Blacklist
                case 0:
                    int BanLimit = (QueryString.ContainsKey("banned") && Int32.TryParse(QueryString["banned"], out I)) ? I : 100;
                    Where += String.Format(" AND (banned >= {0} OR permban = 1)", BanLimit);
                    break;
                // Whitelist
                case 1:
                    if (QueryString.ContainsKey("clantag"))
                        Where += String.Format(" AND clantag = '{0}'  AND permban = 0", Database.DatabaseDriver.Escape(QueryString["clantag"]));
                    break;
                // Greylist
                case 2:
                    // List of possible query's
                    string[] Params = new string[] { "score", "rank", "time", "kdratio", "country", "banned" };
                    foreach (string Param in Params)
                    {
                        if (QueryString.ContainsKey(Param))
                        {
                            switch (Param)
                            {
                                case "id":
                                    if (Int32.TryParse(QueryString["id"], out I))
                                        Where += String.Format(" AND id = {1}", I);
                                    break;
                                case "score":
                                case "time":
                                case "rank":
                                    if (Int32.TryParse(QueryString[Param], out I))
                                        Where += String.Format(" AND {0} >= {1}", Param, I);
                                    break;
                                case "kdratio":
                                    if (float.TryParse(QueryString["kdratio"], out F))
                                        Where += String.Format(" AND (kills / deaths) >= {0}", F);
                                    break;
                                case "country":
                                    S = QueryString["country"].Replace(",", "','");
                                    Where += String.Format(" AND {0} IN ('{1}')", Param, S);
                                    break;
                                case "banned":
                                    if (Int32.TryParse(QueryString["banned"], out I))
                                        Where += String.Format(" AND (banned < {0} AND permban = 0)", I);
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
            string Query = "SELECT id, name FROM player WHERE ip != '0.0.0.0' " + Where + " ORDER BY id ASC";
            List<Dictionary<string, object>> Players = ASPServer.Database.Driver.Query(Query);
            foreach (Dictionary<string, object> P in Players)
            {
                size++;
                Output2.AddRow(P["id"].ToString(), P["name"].ToString());
            }

            // Send Response
            Output1.AddRow(size, Utils.UnixTimestamp());
            Response.AddData(Output1);
            Response.AddData(Output2);
            Response.Send();
        }
    }
}
