using System;
using System.Collections.Generic;
using BF2Statistics.Database;
using BF2Statistics.Database.QueryBuilder;

namespace BF2Statistics.ASP.Requests
{
    class GetMapInfo
    {
        public GetMapInfo(HttpClient Client, StatsDatabase Driver)
        {
            // Setup Variables
            int Pid = 0, MapId = 0, CustomOnly = 0;
            string MapName = "";
            SelectQueryBuilder Query = new SelectQueryBuilder(Driver);

            // Setup QueryString Params
            if (Client.Request.QueryString.ContainsKey("pid"))
                Int32.TryParse(Client.Request.QueryString["pid"], out Pid);
            if (Client.Request.QueryString.ContainsKey("mapid"))
                Int32.TryParse(Client.Request.QueryString["mapid"], out MapId);
            if (Client.Request.QueryString.ContainsKey("customonly"))
                Int32.TryParse(Client.Request.QueryString["customonly"], out CustomOnly);
            if (Client.Request.QueryString.ContainsKey("mapname"))
                MapName = Client.Request.QueryString["mapname"].Trim();

            // Prepare Response
            Client.Response.WriteResponseStart();

            // Is this a Player Map Request?
            if (Pid != 0)
            {
                // Build our query statement
                Query.SelectFromTable("maps");
                Query.SelectColumns("maps.*", "mapinfo.name AS mapname");
                Query.AddJoin(JoinType.InnerJoin, "mapinfo", "id", Comparison.Equals, "maps", "mapid");
                Query.AddWhere("maps.id", Comparison.Equals, Pid);
                Query.AddOrderBy("mapid", Sorting.Ascending);

                // Execute the reader, and add each map to the output
                Client.Response.WriteHeaderLine("mapid", "mapname", "time", "win", "loss", "best", "worst");
                foreach (Dictionary<string, object> Map in Driver.QueryReader(Query.BuildCommand()))
                    Client.Response.WriteDataLine(Map["mapid"], Map["mapname"], Map["time"], Map["win"], Map["loss"], Map["best"], Map["worst"]);
            }
            else
            {
                // Build our query statement
                Query.SelectFromTable("mapinfo");
                Query.SelectColumns("id", "name", "score", "time", "times", "kills", "deaths");
                Query.AddOrderBy("id", Sorting.Ascending);

                // Select our where statement
                if (MapId > 0)
                    Query.AddWhere("id", Comparison.Equals, MapId);
                else if (!String.IsNullOrEmpty(MapName))
                    Query.AddWhere("name", Comparison.Equals, MapName);
                else if (CustomOnly == 1)
                    Query.AddWhere("id", Comparison.GreaterOrEquals, 700);

                // Execute the reader, and add each map to the output
                Client.Response.WriteHeaderLine("mapid", "name", "score", "time", "times", "kills", "deaths");
                foreach (Dictionary<string, object> Map in Driver.QueryReader(Query.BuildCommand()))
                    Client.Response.WriteDataLine(Map["id"], Map["name"], Map["score"], Map["time"], Map["times"], Map["kills"], Map["deaths"]);
            }

            // Send Response
            Client.Response.Send();
        }
    }
}
