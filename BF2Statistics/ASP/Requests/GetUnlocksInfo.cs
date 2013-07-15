using System;
using System.Collections.Generic;
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
                    Rows = Driver.Query("SELECT name, score, rank, usedunlocks FROM player WHERE id=@P0", Pid);
                    if (Rows.Count == 0)
                    {
                        Output = new FormattedOutput("pid", "nick", "asof");
                        Output.AddRow(Pid, "No_Player", DateTime.UtcNow.ToUnixTimestamp());
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
                    Output.AddRow(Pid, Rows[0]["name"].ToString().Trim(), DateTime.UtcNow.ToUnixTimestamp());
                    Response.AddData(Output);

                    // Get total number of unlocks player is allowed to have
                    Rank = Int32.Parse(Rows[0]["rank"].ToString());
                    Earned = Int32.Parse(Rows[0]["usedunlocks"].ToString());
                    Available = GetBonusUnlocks();

                    // Determine total unlocks available
                    Rows = Driver.Query("SELECT COUNT(id) AS count FROM unlocks WHERE id = @P0 AND state = 's'", Pid);
                    int Used = Int32.Parse(Rows[0]["count"].ToString());
                    if (Used > 0)
                    {
                        // Update unlocks data
                        Available -= Used;
                        Driver.Execute("UPDATE player SET availunlocks = @P0, usedunlocks = @P1 WHERE id = @P2", Available, Used, Pid);
                    }

                    // Output users unlocks
                    Output = new FormattedOutput("enlisted", "officer");
                    Output.AddRow(Available, "0");
                    Response.AddData(Output);
                    Output = new FormattedOutput("id", "state");

                    // Add each unlock's state
                    Dictionary<string, bool> Unlocks = new Dictionary<string, bool>();
                    Rows = Driver.Query("SELECT kit, state FROM unlocks WHERE id=@P0 ORDER BY kit ASC", Pid);
                    foreach (Dictionary<string, object> Unlock in Rows)
                    {
                        // Add unlock to output if its a base unlock
                        int Id = Int32.Parse(Unlock["kit"].ToString());
                        if (Id < 78)
                            Output.AddRow(Unlock["kit"], Unlock["state"]);

                        // Add Unlock to list
                        Unlocks.Add(Unlock["kit"].ToString(), (Unlock["state"].ToString() == "s"));
                    }

                    // Add SF Unlocks... We need the base class unlock unlocked first
                    CheckUnlock(88, 22, Unlocks, Output);
                    CheckUnlock(99, 33, Unlocks, Output);
                    CheckUnlock(111, 44, Unlocks, Output);
                    CheckUnlock(222, 55, Unlocks, Output);
                    CheckUnlock(333, 66, Unlocks, Output);
                    CheckUnlock(444, 11, Unlocks, Output);
                    CheckUnlock(555, 77, Unlocks, Output);

                    // Add unlock data
                    Response.AddData(Output);
                    break;

                // All Unlocked
                case 1:
                    Output = new FormattedOutput("pid", "nick", "asof");
                    Output.AddRow(Pid, "All_Unlocks", DateTime.UtcNow.ToUnixTimestamp());
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
                    Output.AddRow(Pid, "No_Unlocks", DateTime.UtcNow.ToUnixTimestamp());
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

        /// <summary>
        /// Gets the total unlocks a player can have based off of rank, and awards
        /// </summary>
        /// <returns></returns>
        private int GetBonusUnlocks()
        {
            // Start with Kit unlocks (veteran awards and above)
            string Query = String.Format(
                "SELECT COUNT(id) AS count FROM awards WHERE id = {0} AND awd IN ({1}) AND level > 1",
                Pid, 
                "1031119, 1031120, 1031109, 1031115, 1031121, 1031105, 1031113"
            );
            Rows = Driver.Query(Query);
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

        /// <summary>
        /// This method adds special forces unlocks to the output, only if the base
        /// class unlock is unlocked. We dont add the unlock if the base class unlock
        /// is NOT unlocked, because if we do, then the user will be able to choose
        /// the unlock, without earning the base unlock first
        /// </summary>
        /// <param name="Want">The SF unlock ID</param>
        /// <param name="Need">The base class unlock ID</param>
        /// <param name="Unlocks">All the unlocks, and status</param>
        /// <param name="Output">Current FormatedOutput</param>
        private void CheckUnlock(int Want, int Need, Dictionary<string, bool> Unlocks, FormattedOutput Output)
        {
            // If we have base unlock, add SF unlock to formatted output
            if (Unlocks.ContainsKey(Need.ToString()) && Unlocks[Need.ToString()] == true)
            {
                Output.AddRow(Want, (Unlocks.ContainsKey(Want.ToString()) && Unlocks[Want.ToString()]) ? "s" : "n");
            }
        }
    }
}
