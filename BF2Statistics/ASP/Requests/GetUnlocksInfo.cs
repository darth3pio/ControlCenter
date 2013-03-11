using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BF2Statistics.Database;

namespace BF2Statistics.ASP.Requests
{
    class GetUnlocksInfo
    {
        private int Pid = 0;
        private int Rank = 0;

        DatabaseDriver Driver = ASPServer.Database.Driver;
        List<Dictionary<string, object>> Rows;

        public GetUnlocksInfo(ASPResponse Response, Dictionary<string, string> QueryString)
        {
            int Earned = 0;
            int Available = 0;
            FormattedOutput Output;

            // Get player ID
            if (QueryString.ContainsKey("pid"))
                Int32.TryParse(QueryString["pid"], out Pid);

            switch(MainForm.Config.ASP_UnlocksMode)
            {
                // Player Based
                case 0:
                    // Make sure the player exists
                    Rows = Driver.Query("SELECT name, score, rank, usedunlocks FROM player WHERE id={0}", Pid);
                    if (Rows.Count == 0)
                    {
                        Output = new FormattedOutput("pid", "nick", "asof");
                        Output.AddRow(Pid, "No_Player", Utils.UnixTimestamp());
                        Response.AddData(Output);
                        Output = new FormattedOutput("enlisted", "officer");
                        Output.AddRow("0", "0");
                        Response.AddData(Output);
                        Output = new FormattedOutput("id", "state");
                        for (int i = 11; i < 100; i += 11)
                            Output.AddRow(i, "n");
                        for (int i = 111; i < 556; i += 111)
                            Output.AddRow(i, "n");
                        Response.AddData(Output);
                        break;
                    }

                    // Start Output
                    Output = new FormattedOutput("pid", "nick", "asof");
                    Output.AddRow(Pid, Rows[0]["name"].ToString().Trim(), Utils.UnixTimestamp());
                    Response.AddData(Output);

                    // Get total number of unlocks player is allowed to have
                    Rank = Int32.Parse(Rows[0]["rank"].ToString());
                    Earned = Int32.Parse(Rows[0]["usedunlocks"].ToString());
                    Available = GetBonusUnlocks();

                    // Determine total unlocks available
                    Rows = Driver.Query("SELECT COUNT(id) AS count FROM unlocks WHERE id = {0} AND state = 's'", Pid);
                    int Used = Int32.Parse(Rows[0]["count"].ToString());
                    if (Used > 0)
                    {
                        // Update unlocks data
                        Available -= Used;
                        Driver.Execute("UPDATE player SET availunlocks = {0}, usedunlocks = {1} WHERE id = {2}", Available, Used, Pid);
                    }

                    // Output users unlocks
                    Output = new FormattedOutput("enlisted", "officer");
                    Output.AddRow(Available, "0");
                    Response.AddData(Output);
                    Output = new FormattedOutput("id", "state");

                    // Add each unlock's state
                    Rows = Driver.Query("SELECT kit, state FROM unlocks WHERE id={0}", Pid);
                    foreach (Dictionary<string, object> Unlock in Rows)
                        Output.AddRow(Unlock["kit"], Unlock["state"]);

                    Response.AddData(Output);
                    break;

                // All Unlocked
                case 1:
                    Output = new FormattedOutput("pid", "nick", "asof");
                    Output.AddRow(Pid, "All_Unlocks", Utils.UnixTimestamp());
                    Response.AddData(Output);
                    Output = new FormattedOutput("enlisted", "officer");
                    Output.AddRow("0", "0");
                    Response.AddData(Output);
                    Output = new FormattedOutput("id", "state");
                    for (int i = 11; i < 100; i += 11)
                        Output.AddRow(i, "s");
                    for (int i = 111; i < 556; i += 111)
                        Output.AddRow(i, "s");
                    Response.AddData(Output);
                    break;

                // Unlocks Disabled
                default:
                    Output = new FormattedOutput("pid", "nick", "asof");
                    Output.AddRow(Pid, "No_Unlocks", Utils.UnixTimestamp());
                    Response.AddData(Output);
                    Output = new FormattedOutput("enlisted", "officer");
                    Output.AddRow("0", "0");
                    Response.AddData(Output);
                    Output = new FormattedOutput("id", "state");
                    for (int i = 11; i < 100; i += 11)
                        Output.AddRow(i, "n");
                    for (int i = 111; i < 556; i += 111)
                        Output.AddRow(i, "n");
                    Response.AddData(Output);
                    break;
            }

            // Send Response
            Response.Send();
        }

        private int GetBonusUnlocks()
        {
            // Start with Kit unlocks (veteran awards and above)
            string Query = "SELECT COUNT(id) AS count FROM awards WHERE id = {0} AND awd IN ({1}) AND level = 2";
            Rows = Driver.Query(Query, Pid, "1031119, 1031120, 1031109, 1031115, 1031121, 1031105, 1031113");
            int Unlocks = Int32.Parse(Rows[0]["count"].ToString());

            // And Rank Unlocks
            if (Rank >= 9) return Unlocks + 7;
            else if (Rank >= 7) return Unlocks + 6;
            else if (Rank >= 6) return Unlocks + 5;
            else if (Rank >= 5) return Unlocks + 4;
            else if (Rank >= 4) return Unlocks + 3;
            else if (Rank >= 3) return Unlocks + 2;
            else if (Rank >= 2) return Unlocks + 1;
            else return Unlocks;
        }
    }
}
