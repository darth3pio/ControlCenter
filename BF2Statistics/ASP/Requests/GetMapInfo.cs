using System;
using System.Collections.Generic;
using BF2Statistics.Database;

namespace BF2Statistics.ASP.Requests
{
    class GetMapInfo
    {
        public GetMapInfo(ASPResponse Response, Dictionary<string, string> QueryString)
        {
            // Setup Variables
            int Pid = 0;
            int MapId = 0;
            int CustomOnly = 0;
            string MapName = "";
            string Query;
            List<Dictionary<string, object>> Rows;
            DatabaseDriver Driver = ASPServer.Database.Driver;
            FormattedOutput Output;

            // Setup Params
            if (QueryString.ContainsKey("pid"))
                Int32.TryParse(QueryString["pid"], out Pid);
            if (QueryString.ContainsKey("mapid"))
                Int32.TryParse(QueryString["mapid"], out MapId);
            if (QueryString.ContainsKey("customonly"))
                Int32.TryParse(QueryString["customonly"], out CustomOnly);
            if (QueryString.ContainsKey("mapname"))
                MapName = QueryString["mapname"];

            // Is this a Player Map Request?
            if (Pid != 0)
            {
                Query = "SELECT m.*, mi.name AS mapname "
                    + "FROM maps m JOIN mapinfo mi ON m.mapid = mi.id "
                    + "WHERE m.id = {0} "
                    + "ORDER BY mapid";
                Rows = Driver.Query(Query, Pid);

                Output = new FormattedOutput("mapid", "mapname", "time", "win", "loss", "best", "worst");
                foreach (Dictionary<string, object> Map in Rows)
                    Output.AddRow(Map["mapid"], Map["mapname"], Map["time"], Map["win"], Map["loss"], Map["best"], Map["worst"]);
            }
            else
            {
                // Build the Query
                string MapLimit = (CustomOnly == 1) ? " AND id >= " + MainForm.Config.ASP_CustomMapID : "";
                Query = "SELECT id, name, score, time, times, kills, deaths FROM mapinfo ";
                if (MapId > 0)
                    Query += String.Format("WHERE id = {0} {1}", MapId, MapLimit);
                else if (!String.IsNullOrWhiteSpace(MapName))
                    Query += String.Format("WHERE name = {0} {1}", MapName, MapLimit);
                else if (!String.IsNullOrWhiteSpace(MapLimit))
                    Query += String.Format("WHERE {0} ORDER BY id", MapLimit);

                // Get the list of maps
                Rows = Driver.Query(Query);

                Output = new FormattedOutput("mapid", "name", "score", "time", "times", "kills", "deaths");
                foreach (Dictionary<string, object> Map in Rows)
                    Output.AddRow(Map["id"], Map["name"], Map["score"], Map["time"], Map["times"], Map["kills"], Map["deaths"]);
            }

            // Send Response
            Response.AddData(Output);
            Response.Send();
        }
    }
}
