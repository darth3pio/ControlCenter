using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using BF2Statistics.Database;
using BF2Statistics.Database.QueryBuilder;

namespace BF2Statistics.Web.ASP
{
    class StatsParser
    {
        /// <summary>
        /// Converts an array of headers and data, and pairs them together in
        /// a dictionary.
        /// </summary>
        /// <param name="Headers">The Array of Headers</param>
        /// <param name="Data">The Array of Data</param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseHeaderData(string[] Headers, string[] Data)
        {
            Dictionary<string, string> Dic = new Dictionary<string, string>();
            int i = 0;
            foreach (string H in Headers)
            {
                Dic.Add(H, Data[i]);
                i++;
            }

            return Dic;
        }

        /// <summary>
        /// Parses awards output from the ASP into an organized list
        /// </summary>
        /// <param name="Lines">All the ASP lines from the output</param>
        /// <returns></returns>
        public static List<Dictionary<string, string>> ParseAwards(List<string[]> Lines)
        {
            List<Dictionary<string, string>> Awards = new List<Dictionary<string, string>>();
            bool FirstData = true;

            foreach (string[] Line in Lines)
            {
                if (Line[0] == "D")
                {
                    if (!FirstData)
                    {
                        // Ribbons on the EA server are level 0.. .they need to be level 1!
                        if (Line[2] == "0")
                            Line[2] = "1";

                        // Add Award to list
                        Awards.Add(new Dictionary<string, string>()
                        {
                            {"id", Line[1]},
                            {"level", Line[2]},
                            {"when", Line[3]},
                            {"first", Line[4]}
                        });
                    }
                    else
                    {
                        // Set first line to false
                        FirstData = false;
                    }
                }
            }

            return Awards;
        }
    }
}
