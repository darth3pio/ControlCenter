using System;
using System.Collections.Generic;
using System.Text;

namespace BF2Statistics.Utilities
{
    class Sql
    {
        /// <summary>
        /// Takes an array of lines (most likely from an sql file), and extracts the
        /// queries (mulit or single line), and returns them
        /// </summary>
        /// <param name="Lines"></param>
        /// <returns></returns>
        public static List<string> ExtractQueries(string[] Lines)
        {
            List<string> Queries = new List<string>();
            StringBuilder Query = new StringBuilder();

            // Add each query to the Queries list
            foreach (string line in Lines)
            {
                // Trim line, and make sure its not a comment of any kind
                string TrimLine = line.Trim();
                if (!String.IsNullOrEmpty(TrimLine) && !TrimLine.StartsWith("#") && !TrimLine.StartsWith("--"))
                {
                    // Is the query complete?
                    if (TrimLine.EndsWith(";"))
                    {
                        Query.Append(TrimLine);
                        Queries.Add(Query.ToString());
                        Query = new StringBuilder();
                    }
                    else
                        Query.Append(TrimLine);
                }
            }

            return Queries;
        }
    }
}
