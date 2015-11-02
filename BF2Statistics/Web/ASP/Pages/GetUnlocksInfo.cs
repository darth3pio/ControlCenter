using System;
using System.Collections.Generic;
using System.Text;
using BF2Statistics.Database;

namespace BF2Statistics.Web.ASP
{
    /// <summary>
    /// /ASP/getunlocksinfo.aspx
    /// </summary>
    public sealed class GetUnlocksInfo
    {
        /// <summary>
        /// Player's Unique Id
        /// </summary>
        private int Pid = 0;

        /// <summary>
        /// The Player's Rank
        /// </summary>
        private int Rank = 0;

        /// <summary>
        /// Our stats database driver
        /// </summary>
        DatabaseDriver Driver;

        /// <summary>
        /// Database Rows result
        /// </summary>
        List<Dictionary<string, object>> Rows;

        /// <summary>
        /// Our Http/Asp Response
        /// </summary>
        ASPResponse Response;

        /// <summary>
        /// This request provides details of the players unlocked weapons
        /// </summary>
        /// <queryParam name="pid" type="int">The unique player ID</queryParam>
        /// <queryParam name ="nick" type="string">Unique player nickname</queryParam>
        /// <param name="Client">The HttpClient who made the request</param>
        /// <param name="Driver">The Stats Database Driver. Connection errors are handled in the calling object</param>
        public GetUnlocksInfo(HttpClient Client, StatsDatabase Database)
        {
            // Load class Variables
            this.Response = Client.Response as ASPResponse;
            this.Driver = Database;

            // Get player ID
            if (Client.Request.QueryString.ContainsKey("pid"))
                Int32.TryParse(Client.Request.QueryString["pid"], out Pid);

            // Prepare Output
            Response.WriteResponseStart();
            Response.WriteHeaderLine("pid", "nick", "asof");

            // Our ourput changes based on the selected Unlocks config setting
            switch(Program.Config.ASP_UnlocksMode)
            {
                // Player Based - Unlocks are earned
                case 0:
                    // Make sure the player exists
                    Rows = Driver.Query("SELECT name, score, rank, availunlocks, usedunlocks FROM player WHERE id=@P0", Pid);
                    if(Rows.Count == 0)
                        goto case 2; // No Unlocks

                    // Start Output
                    Response.WriteDataLine(Pid, Rows[0]["name"].ToString().Trim(), DateTime.UtcNow.ToUnixTimestamp());

                    // Get total number of unlocks player is allowed to have givin his rank, and bonus unlocks
                    Rank = Int32.Parse(Rows[0]["rank"].ToString());
                    int HasUsed = Int32.Parse(Rows[0]["usedunlocks"].ToString());
                    int Available = Int32.Parse(Rows[0]["availunlocks"].ToString());
                    int Earned = GetBonusUnlocks();

                    // Determine total unlocks available, based on what he has earned, minus what he has used already
                    int Used = Driver.ExecuteScalar<int>("SELECT COUNT(*) FROM unlocks WHERE id=@P0 AND state='s'", Pid);
                    Earned -= Used;

                    // Update database if the database is off
                    if (Earned != Available || HasUsed != Used)
                        Driver.Execute("UPDATE player SET availunlocks=@P0, usedunlocks=@P1 WHERE id=@P2", Earned, Used, Pid);

                    // Output more
                    Response.WriteHeaderLine("enlisted", "officer");
                    Response.WriteDataLine(Earned, 0);
                    Response.WriteHeaderLine("id", "state");

                    // Add each unlock's state
                    Rows = Driver.Query("SELECT kit, state FROM unlocks WHERE id=@P0 ORDER BY kit ASC", Pid);
                    if (Rows.Count == 0)
                    {
                        // Create Player Unlock Data
                        StringBuilder Query = new StringBuilder("INSERT INTO unlocks VALUES ", 350);

                        // Normal unlocks
                        for (int i = 11; i < 100; i += 11)
                        {
                            // 88 and above are Special Forces unlocks, and wont display at all if the base unlocks are not earned
                            if (i < 78 ) 
                                Response.WriteDataLine(i, "n");
                            Query.AppendFormat("({0}, {1}, 'n'), ", Pid, i);
                        }

                        // Sf Unlocks, Dont display these because thats how Gamespy does it
                        for (int i = 111; i < 556; i += 111)
                        {
                            Query.AppendFormat("({0}, {1}, 'n')", Pid, i);
                            if (i != 555) 
                                Query.Append(", ");
                        }

                        // Do Insert
                        Driver.Execute(Query.ToString());
                    }
                    else
                    {
                        Dictionary<string, bool> Unlocks = new Dictionary<string, bool>(7);
                        foreach (Dictionary<string, object> Unlock in Rows)
                        {
                            // Add unlock to output if its a base unlock
                            int Id = Int32.Parse(Unlock["kit"].ToString());
                            if (Id < 78)
                                Response.WriteDataLine(Unlock["kit"], Unlock["state"]);

                            // Add Unlock to list
                            Unlocks.Add(Unlock["kit"].ToString(), (Unlock["state"].ToString() == "s"));
                        }

                        // Add SF Unlocks... We need the base class unlock unlocked first
                        CheckUnlock(88, 22, Unlocks);
                        CheckUnlock(99, 33, Unlocks);
                        CheckUnlock(111, 44, Unlocks);
                        CheckUnlock(222, 55, Unlocks);
                        CheckUnlock(333, 66, Unlocks);
                        CheckUnlock(444, 11, Unlocks);
                        CheckUnlock(555, 77, Unlocks);
                    }
                    break;

                // All Unlocked
                case 1:
                    Response.WriteDataLine(Pid, "All_Unlocks", DateTime.UtcNow.ToUnixTimestamp());
                    Response.WriteHeaderLine("enlisted", "officer");
                    Response.WriteDataLine(0, 0);
                    Response.WriteHeaderLine("id", "state");
                    for (int i = 11; i < 100; i += 11)
                        Response.WriteDataLine(i, "s");
                    for (int i = 111; i < 556; i += 111)
                        Response.WriteDataLine(i, "s");
                    break;

                // Unlocks Disabled
                case 2:
                default:
                    Response.WriteDataLine(Pid, "No_Unlocks", DateTime.UtcNow.ToUnixTimestamp());
                    Response.WriteHeaderLine("enlisted", "officer");
                    Response.WriteDataLine(0, 0);
                    Response.WriteHeaderLine("id", "state");
                    for (int i = 11; i < 78; i += 11)
                        Response.WriteDataLine(i, "n");
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
            Rows = Driver.Query(String.Format(
                "SELECT COUNT(id) AS count FROM awards WHERE id = {0} AND awd IN ({1}) AND level > 1",
                Pid, "1031119, 1031120, 1031109, 1031115, 1031121, 1031105, 1031113"
            ));
            int Unlocks = Int32.Parse(Rows[0]["count"].ToString());

            // And Rank Unlocks
            if (Rank >= 9) 
                Unlocks += 7;
            else if(Rank >= 7)
                Unlocks += 6;
            else if (Rank > 1)
                Unlocks += (Rank - 1);

            return Unlocks;
        }

        /// <summary>
        /// This method adds special forces unlocks to the output, only if the base
        /// class unlock is unlocked. We dont add the unlock if the base class unlock
        /// is NOT unlocked, because if we do, then the user will be able to choose
        /// the unlock, without earning the base unlock first
        /// </summary>
        /// <param name="Want">The Special Forces unlock ID</param>
        /// <param name="Need">The base class unlock ID</param>
        /// <param name="Unlocks">All the players unlocks, and status for each</param>
        private void CheckUnlock(int Want, int Need, Dictionary<string, bool> Unlocks)
        {
            // If we have base unlock, add SF unlock to formatted output
            if (Unlocks.ContainsKey(Need.ToString()) && Unlocks[Need.ToString()] == true)
            {
                Response.WriteDataLine(Want, (Unlocks.ContainsKey(Want.ToString()) && Unlocks[Want.ToString()]) ? "s" : "n");
            }
        }
    }
}
