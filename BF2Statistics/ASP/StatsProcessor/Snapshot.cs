using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using BF2Statistics.Database;
using BF2Statistics.Database.QueryBuilder;
using BF2Statistics.Logging;

namespace BF2Statistics.ASP.StatsProcessor
{
    /// <summary>
    /// This class is used to Validate, and Process the
    /// Game data sent from the Battlefield 2 server at
    /// the end of a round (Aka: Snapshot data)
    /// </summary>
    class Snapshot : GameResult
    {
        /// <summary>
        /// Debug log file
        /// </summary>
        private static LogWriter DebugLog = new LogWriter(Path.Combine(Program.RootPath, "Logs", "StatsDebug.log"));

        /// <summary>
        /// Is this a central update snapshot?
        /// </summary>
        public bool IsCentralUpdate { get; protected set; }

        /// <summary>
        /// Indicates whether or not this game data is already processed into the database
        /// </summary>
        public bool IsProcessed { get; protected set; }

        /// <summary>
        /// Returns the Original Data string used to build this snapshot object
        /// </summary>
        public string DataString { get; protected set; }

        /// <summary>
        /// Returns the server IP that posted this snapshot. If no Server
        /// IP is provided (Ex: loading snapshot from a file), then the local
        /// loopback address will be returned instead
        /// </summary>
        public IPAddress ServerIp { get; protected set; }

        /// <summary>
        /// Initializes a new Snapshot, with the specified Date it was Posted
        /// </summary>
        /// <param name="Snapshot">The snapshot source</param>
        public Snapshot(string Snapshot, IPAddress ServerIp = null)
        {
            // Set some internal variables
            this.ServerIp = ServerIp ?? IPAddress.Loopback;
            this.Players = new List<Player>();
            this.DataString = Snapshot.Trim();
            string[] Data = DataString.Split('\\');
            Snapshot = null;

            // Check for invalid snapshot string. All snapshots have at least 36 data pairs, 
            // and has an Even number of data sectors.
            if (Data.Length < 36 || Data.Length % 2 != 0)
                throw new InvalidDataException("Snapshot does not contain at least 36 elements, or contains an odd number of elements");

            // Assign server name and prefix, and determine if we are central update. the "cdb_update" variable must be the LAST sector in snapshot
            this.ServerPrefix = Data[0];
            this.ServerName = Data[1];
            this.IsCentralUpdate = (Data[Data.Length - 2] == "cdb_update" && Data[Data.Length - 1] != "0");

            // Setup our data dictionary's. We use NiceDictionary so we can easily determine missing keys in the log file
            NiceDictionary<string, string> StandardData = new NiceDictionary<string, string>(16);
            NiceDictionary<string, string> PlayerData = new NiceDictionary<string, string>();
            Dictionary<int, int> KillData = new Dictionary<int, int>();

            // Wrap parsing Key/Value snapshot data in a try block!
            try
            {
                // Convert our standard data into key => value pairs
                for (int i = 2; i < Data.Length; i += 2)
                {
                    // Format: "DataKey_PlayerIndex". PlayerIndex is NOT the Player Id
                    string[] Parts = Data[i].Split('_');
                    if (Parts.Length == 1)
                    {
                        // Add to the Standard keys
                        StandardData.Add(Data[i], Data[i + 1]);

                        // Are we at the End of File? If so stop here
                        if (Parts[0] == "EOF")
                        {
                            // Make sure to save that last players stats!!
                            if (PlayerData.Count != 0)
                            {
                                AddPlayer(new Player(PlayerData, KillData));
                                PlayerData = null;
                                KillData = null;
                            }
                            break;
                        }
                    }

                    // If the item key is "pID", then we have a new player record
                    else if (Parts[0] == "pID")
                    {
                        // If we have data, complete this player and start anew
                        if (PlayerData.Count != 0)
                        {
                            AddPlayer(new Player(PlayerData, KillData));
                            PlayerData.Clear();
                            KillData.Clear();
                        }

                        // Add new PID
                        PlayerData.Add(Parts[0], Data[i + 1]);
                    }
                    else if (Parts[0] == "mvks") // Skip mvks... kill data only needs processed once (mvns)
                        continue;
                    else if (Parts[0] == "mvns") // Player kill data
                        KillData.Add(Int32.Parse(Data[i + 1]), Int32.Parse(Data[i + 3]));
                    else
                        PlayerData.Add(Parts[0], Data[i + 1]);
                }
            }
            catch (Exception e)
            {
                throw new InvalidDataException("Error assigning Key => value pairs. See InnerException", e);
            }

            // Make sure we have a completed snapshot
            if (!StandardData.ContainsKey("EOF"))
                throw new InvalidDataException("No End of File element was found, Snapshot assumed to be incomplete.");

            // Try and set internal GameResult variables
            try
            {
                // Server data
                this.ServerPort = Int32.Parse(StandardData["gameport"]);
                this.QueryPort = Int32.Parse(StandardData["queryport"]);

                // Map Data
                this.MapName = StandardData["mapname"];
                this.MapId = Int32.Parse(StandardData["mapid"]);
                this.RoundStartTime = (int)Convert.ToDouble(StandardData["mapstart"], CultureInfo.InvariantCulture.NumberFormat);
                this.RoundEndTime = (int)Convert.ToDouble(StandardData["mapend"], CultureInfo.InvariantCulture.NumberFormat);

                // Misc Data
                this.GameMode = Int32.Parse(StandardData["gm"]);
                this.Mod = StandardData["v"]; // bf2 mod replaced the version key, since we dont care the version anyways
                this.PlayersConnected = Int32.Parse(StandardData["pc"]);

                // Army Data... There is no RWA key if there was no winner...
                this.WinningTeam = Int32.Parse(StandardData["win"]); // Temp
                this.WinningArmyId = (StandardData.ContainsKey("rwa")) ? Int32.Parse(StandardData["rwa"]) : -1;
                this.Team1ArmyId = Int32.Parse(StandardData["ra1"]);
                this.Team1Tickets = Int32.Parse(StandardData["rs1"]);
                this.Team2ArmyId = Int32.Parse(StandardData["ra2"]);
                this.Team2Tickets = Int32.Parse(StandardData["rs2"]);
            }
            catch (Exception e)
            {
                throw new InvalidDataException("Error assigning GameResult variables. See InnerException", e);
            }

            // Wrap this in a try-catch block, because we want to be able to view GameResult
            // data, even if the database is offline
            try
            {
                // Dispose when done!
                using (StatsDatabase Driver = new StatsDatabase())
                {
                    // Check for custom map, with no ID (Not defined in Constants.py)
                    if (this.MapId == 99)
                    {
                        // Check for existing map data
                        var Rows = Driver.Query("SELECT id FROM mapinfo WHERE name=@P0", this.MapName);
                        if (Rows.Count == 0)
                        {
                            // Create new MapId. Id's 700 - 1000 are reserved for unknown maps in the Constants.py file
                            // There should never be more then 300 unknown map id's, considering 1001 is the start of KNOWN
                            // Custom mod map id's. If we are at 1000 now, then we are in trouble :S
                            this.MapId = Driver.ExecuteScalar<int>("SELECT COALESCE(MAX(id), 699) FROM mapinfo WHERE id BETWEEN 700 AND 999") + 1;
                            if (this.MapId == 1000)
                                throw new Exception("Maximum unknown custom mapid has been reached. Please add this map's mapid to the Constants.py");

                            // Insert map data, so we dont lose this mapid we generated
                            Driver.Execute("INSERT INTO mapinfo(id, name, custom) VALUES (@P0, @P1, @P2)", this.MapId, this.MapName, 1);
                        }
                        else
                            this.MapId = Int32.Parse(Rows[0]["id"].ToString());
                    }

                    // Set whether or not this data is already been processed. The OR condition is because i goofed in early updates
                    // and set the timestamp to the RoundStart instead of the RoundEnd like i should have
                    this.IsProcessed = Driver.ExecuteScalar<int>(
                        "SELECT COUNT(*) FROM round_history WHERE mapid=@P0 AND time=@P1 AND (timestamp=@P2 OR timestamp=@P3)",
                        this.MapId, this.RoundTime.Seconds, this.RoundEndTime, this.RoundStartTime
                    ) > 0;
                }
            }
            catch(DbConnectException)
            {
                this.IsProcessed = false;
            }

            // Indicate whether we are a custom map
            this.IsCustomMap = (this.MapId >= 700 || this.MapId == 99);
        }

        /// <summary>
        /// Processes the snapshot data, inserted and updating player data in the gamespy database
        /// </summary>
        /// <exception cref="InvalidDataException">Thrown if the snapshot data is invalid</exception>
        public void ProcessData()
        {
            // Make sure we are processing the same data again
            if (this.IsProcessed)
                throw new Exception("Round data has already been processed!");

            // Begin Logging
            Log(String.Format("Begin Processing ({0})...", this.MapName), LogLevel.Notice);
            Log(String.Format((this.IsCustomMap) ? "Custom Map ({0})..." : "Standard Map ({0})...", this.MapId), LogLevel.Notice);
            Log("Found " + this.Players.Count + " Player(s)...", LogLevel.Notice);

            // Make sure we meet the minimum player requirement
            if (this.Players.Count < MainForm.Config.ASP_MinRoundPlayers)
            {
                Log("Minimum round Player count does not meet the ASP requirement... Aborting", LogLevel.Warning);
                throw new Exception("Minimum round Player count does not meet the ASP requirement");
            }

            // Setup some variables
            Stopwatch Clock = new Stopwatch();
            List<Dictionary<string, object>> Rows;
            InsertQueryBuilder InsertQuery;
            UpdateQueryBuilder UpdateQuery;
            WhereClause Where;

            // Start the timer and Begin Transaction
            using (StatsDatabase Driver = new StatsDatabase())
            using (DbTransaction Transaction = Driver.BeginTransaction())
            {
                // Begin timer
                Clock.Start();

                // To prevent half complete snapshots due to exceptions,
                // Put the whole thing in a try block, and rollback on error
                try
                {
                    // Loop through each player, and process them
                    foreach (Player Player in this.Players)
                    {
                        // Player meets min round time or are we ignoring AI?
                        if ((Player.RoundTime < MainForm.Config.ASP_MinRoundTime) || (MainForm.Config.ASP_IgnoreAI && Player.IsAI))
                            continue;

                        // Parse some player data
                        bool OnWinningTeam = (Player.Team == WinningTeam);
                        string CountryCode = Ip2nation.GetCountryCode(Player.IpAddress);
                        int Best, Worst;

                        // Log
                        Log(String.Format("Processing Player ({0})", Player.Pid), LogLevel.Notice);

                        // Fetch the player
                        Rows = Driver.Query("SELECT ip, country, rank, killstreak, deathstreak, rndscore FROM player WHERE id=@P0", Player.Pid);
                        if (Rows.Count == 0)
                        {
                            // === New Player === //
                            Log(String.Format("Adding NEW Player ({0})", Player.Pid), LogLevel.Notice);

                            // Build insert data
                            InsertQuery = new InsertQueryBuilder("player", Driver);
                            InsertQuery.SetField("id", Player.Pid);
                            InsertQuery.SetField("name", Player.Name);
                            InsertQuery.SetField("country", CountryCode);
                            InsertQuery.SetField("time", Player.RoundTime);
                            InsertQuery.SetField("rounds", Player.CompletedRound);
                            InsertQuery.SetField("ip", Player.IpAddress);
                            InsertQuery.SetField("score", Player.RoundScore);
                            InsertQuery.SetField("cmdscore", Player.CommandScore);
                            InsertQuery.SetField("skillscore", Player.SkillScore);
                            InsertQuery.SetField("teamscore", Player.TeamScore);
                            InsertQuery.SetField("kills", Player.Stats.Kills);
                            InsertQuery.SetField("deaths", Player.Stats.Deaths);
                            InsertQuery.SetField("captures", Player.Stats.FlagCaptures);
                            InsertQuery.SetField("captureassists", Player.Stats.FlagCaptureAssists);
                            InsertQuery.SetField("defends", Player.Stats.FlagDefends);
                            InsertQuery.SetField("damageassists", Player.Stats.DamageAssists);
                            InsertQuery.SetField("heals", Player.Stats.Heals);
                            InsertQuery.SetField("revives", Player.Stats.Revives);
                            InsertQuery.SetField("ammos", Player.Stats.Ammos);
                            InsertQuery.SetField("repairs", Player.Stats.Repairs);
                            InsertQuery.SetField("targetassists", Player.Stats.TargetAssists);
                            InsertQuery.SetField("driverspecials", Player.Stats.DriverSpecials);
                            InsertQuery.SetField("teamkills", Player.Stats.TeamKills);
                            InsertQuery.SetField("teamdamage", Player.Stats.TeamDamage);
                            InsertQuery.SetField("teamvehicledamage", Player.Stats.TeamVehicleDamage);
                            InsertQuery.SetField("suicides", Player.Stats.Suicides);
                            InsertQuery.SetField("killstreak", Player.Stats.KillStreak);
                            InsertQuery.SetField("deathstreak", Player.Stats.DeathStreak);
                            InsertQuery.SetField("rank", Player.Rank);
                            InsertQuery.SetField("banned", Player.TimesBanned);
                            InsertQuery.SetField("kicked", Player.TimesKicked);
                            InsertQuery.SetField("cmdtime", Player.CmdTime);
                            InsertQuery.SetField("sqltime", Player.SqlTime);
                            InsertQuery.SetField("sqmtime", Player.SqmTime);
                            InsertQuery.SetField("lwtime", Player.LwTime);
                            InsertQuery.SetField("wins", OnWinningTeam);
                            InsertQuery.SetField("losses", !OnWinningTeam);
                            InsertQuery.SetField("availunlocks", 0);
                            InsertQuery.SetField("usedunlocks", 0);
                            InsertQuery.SetField("joined", this.RoundEndTime);
                            InsertQuery.SetField("rndscore", Player.RoundScore);
                            InsertQuery.SetField("lastonline", RoundEndTime);
                            InsertQuery.SetField("mode0", (GameMode == 0));
                            InsertQuery.SetField("mode1", (GameMode == 1));
                            InsertQuery.SetField("mode2", (GameMode == 2));
                            InsertQuery.SetField("isbot", Player.IsAI);

                            // Insert Player Data
                            InsertQuery.Execute();

                            // Double Check Unlocks
                            if (!Player.IsAI && Driver.ExecuteScalar<int>("SELECT COUNT(*) FROM unlocks WHERE id=@P0", Player.Pid) == 0)
                            {
                                // Create New Player Unlock Data
                                StringBuilder Q = new StringBuilder("INSERT INTO unlocks VALUES ", 350);

                                // Normal unlocks
                                for (int i = 11; i < 100; i += 11)
                                    Q.AppendFormat("({0}, {1}, 'n'), ", Player.Pid, i);

                                // Sf Unlocks
                                for (int i = 111; i < 556; i += 111)
                                {
                                    Q.AppendFormat("({0}, {1}, 'n')", Player.Pid, i);
                                    if (i != 555) Q.Append(", ");
                                }

                                // Execute query
                                Driver.Execute(Q.ToString());
                            }
                        }
                        else
                        {
                            // Existing Player
                            Log(String.Format("Updating EXISTING Player ({0})", Player.Pid), LogLevel.Notice);

                            // If rank is lower then the database rank, and the players old rank is not a special rank
                            // then we will be correct the rank here. We do this because sometimes stats are interupted
                            // while being fetched in the python (yay single threading!), and the players stats are reset
                            // during that round.
                            int DbRank = Int32.Parse(Rows[0]["rank"].ToString());
                            if (DbRank > Player.Rank && DbRank != 11 && DbRank != 21)
                            {
                                // Fail-safe in-case rank data was not obtained and reset to '0' in-game.
                                Player.SetRank(DbRank);
                                DebugLog.Write("Rank Correction ({0}), Using database rank ({1})", Player.Pid, DbRank);
                            }

                            // Calcuate best killstreak/deathstreak
                            int KillStreak = Int32.Parse(Rows[0]["killstreak"].ToString());
                            int DeathStreak = Int32.Parse(Rows[0]["deathstreak"].ToString());
                            if (Player.Stats.KillStreak > KillStreak) KillStreak = Player.Stats.KillStreak;
                            if (Player.Stats.DeathStreak > DeathStreak) DeathStreak = Player.Stats.DeathStreak;

                            // Calculate Best Round Score
                            int Brs = Int32.Parse(Rows[0]["rndscore"].ToString());
                            if (Player.RoundScore > Brs) Brs = Player.RoundScore;

                            // Update Player Data
                            UpdateQuery = new UpdateQueryBuilder("player", Driver);
                            UpdateQuery.SetField("country", CountryCode, ValueMode.Set);
                            UpdateQuery.SetField("time", Player.RoundTime, ValueMode.Add);
                            UpdateQuery.SetField("rounds", Player.CompletedRound, ValueMode.Add);
                            UpdateQuery.SetField("ip", Player.IpAddress, ValueMode.Set);
                            UpdateQuery.SetField("score", Player.RoundScore, ValueMode.Add);
                            UpdateQuery.SetField("cmdscore", Player.CommandScore, ValueMode.Add);
                            UpdateQuery.SetField("skillscore", Player.SkillScore, ValueMode.Add);
                            UpdateQuery.SetField("teamscore", Player.TeamScore, ValueMode.Add);
                            UpdateQuery.SetField("kills", Player.Stats.Kills, ValueMode.Add);
                            UpdateQuery.SetField("deaths", Player.Stats.Deaths, ValueMode.Add);
                            UpdateQuery.SetField("captures", Player.Stats.FlagCaptures, ValueMode.Add);
                            UpdateQuery.SetField("captureassists", Player.Stats.FlagCaptureAssists, ValueMode.Add);
                            UpdateQuery.SetField("defends", Player.Stats.FlagDefends, ValueMode.Add);
                            UpdateQuery.SetField("damageassists", Player.Stats.DamageAssists, ValueMode.Add);
                            UpdateQuery.SetField("heals", Player.Stats.Heals, ValueMode.Add);
                            UpdateQuery.SetField("revives", Player.Stats.Revives, ValueMode.Add);
                            UpdateQuery.SetField("ammos", Player.Stats.Ammos, ValueMode.Add);
                            UpdateQuery.SetField("repairs", Player.Stats.Repairs, ValueMode.Add);
                            UpdateQuery.SetField("targetassists", Player.Stats.TargetAssists, ValueMode.Add);
                            UpdateQuery.SetField("driverspecials", Player.Stats.DriverSpecials, ValueMode.Add);
                            UpdateQuery.SetField("teamkills", Player.Stats.TeamKills, ValueMode.Add);
                            UpdateQuery.SetField("teamdamage", Player.Stats.TeamDamage, ValueMode.Add);
                            UpdateQuery.SetField("teamvehicledamage", Player.Stats.TeamVehicleDamage, ValueMode.Add);
                            UpdateQuery.SetField("suicides", Player.Stats.Suicides, ValueMode.Add);
                            UpdateQuery.SetField("Killstreak", KillStreak, ValueMode.Set);
                            UpdateQuery.SetField("deathstreak", DeathStreak, ValueMode.Set);
                            UpdateQuery.SetField("rank", Player.Rank, ValueMode.Set);
                            UpdateQuery.SetField("banned", Player.TimesBanned, ValueMode.Add);
                            UpdateQuery.SetField("kicked", Player.TimesKicked, ValueMode.Add);
                            UpdateQuery.SetField("cmdtime", Player.CmdTime, ValueMode.Add);
                            UpdateQuery.SetField("sqltime", Player.SqlTime, ValueMode.Add);
                            UpdateQuery.SetField("sqmtime", Player.SqmTime, ValueMode.Add);
                            UpdateQuery.SetField("lwtime", Player.LwTime, ValueMode.Add);
                            UpdateQuery.SetField("wins", OnWinningTeam, ValueMode.Add);
                            UpdateQuery.SetField("losses", !OnWinningTeam, ValueMode.Add);
                            UpdateQuery.SetField("rndscore", Brs, ValueMode.Set);
                            UpdateQuery.SetField("lastonline", this.RoundEndTime, ValueMode.Set);
                            UpdateQuery.SetField("mode0", (GameMode == 0), ValueMode.Add);
                            UpdateQuery.SetField("mode1", (GameMode == 1), ValueMode.Add);
                            UpdateQuery.SetField("mode2", (GameMode == 2), ValueMode.Add);
                            UpdateQuery.SetField("chng", (Player.Rank > DbRank), ValueMode.Set);
                            UpdateQuery.SetField("decr", (Player.Rank < DbRank), ValueMode.Set);
                            UpdateQuery.SetField("isbot", Player.IsAI, ValueMode.Set);
                            UpdateQuery.AddWhere("id", Comparison.Equals, Player.Pid);
                            UpdateQuery.Execute();
                        }

                        // ********************************
                        // Insert Player history.
                        // ********************************
                        Driver.Execute(
                            "INSERT INTO player_history VALUES(@P0, @P1, @P2, @P3, @P4, @P5, @P6, @P7, @P8, @P9)",
                            Player.Pid, this.RoundEndTime, Player.RoundTime, Player.RoundScore, Player.CommandScore,
                            Player.SkillScore, Player.TeamScore, Player.Stats.Kills, Player.Stats.Deaths, Player.Rank
                        );

                        // ********************************
                        // Process Player Army Data
                        // ********************************
                        Log(String.Format("Processing Army Data ({0})", Player.Pid), LogLevel.Notice);

                        // Update player army times
                        Rows = Driver.Query("SELECT * FROM army WHERE id=" + Player.Pid);
                        if (Rows.Count == 0)
                        {
                            // Build query
                            InsertQuery = new InsertQueryBuilder("army", Driver);
                            InsertQuery.SetField("id", Player.Pid);
                            for (int i = 0; i < 14; i++)
                                InsertQuery.SetField("time" + i, Player.TimeAsArmy[i]);

                            // Make sure we arent playing an unsupported army
                            if (Player.ArmyId < 14)
                            {
                                InsertQuery.SetField("win" + Player.ArmyId, OnWinningTeam);
                                InsertQuery.SetField("loss" + Player.ArmyId, !OnWinningTeam);
                                InsertQuery.SetField("score" + Player.ArmyId, Player.RoundScore);
                                InsertQuery.SetField("best" + Player.ArmyId, Player.RoundScore);
                                InsertQuery.SetField("worst" + Player.ArmyId, Player.RoundScore);
                            }

                            InsertQuery.Execute();
                        }
                        else
                        {
                            // Build query
                            UpdateQuery = new UpdateQueryBuilder("army", Driver);
                            UpdateQuery.AddWhere("id", Comparison.Equals, Player.Pid);
                            for (int i = 0; i < 14; i++)
                                UpdateQuery.SetField("time" + i, Player.TimeAsArmy[i], ValueMode.Add);

                            // Prevent database errors with custom army IDs
                            if (Player.ArmyId < 14)
                            {
                                Best = Int32.Parse(Rows[0]["best" + Player.ArmyId].ToString());
                                Worst = Int32.Parse(Rows[0]["worst" + Player.ArmyId].ToString());
                                if (Player.RoundScore > Best) Best = Player.RoundScore;
                                if (Player.RoundScore < Worst) Worst = Player.RoundScore;

                                UpdateQuery.SetField("win" + Player.ArmyId, OnWinningTeam, ValueMode.Add);
                                UpdateQuery.SetField("loss" + Player.ArmyId, !OnWinningTeam, ValueMode.Add);
                                UpdateQuery.SetField("score" + Player.ArmyId, Player.RoundScore, ValueMode.Add);
                                UpdateQuery.SetField("best" + Player.ArmyId, Best, ValueMode.Set);
                                UpdateQuery.SetField("worst" + Player.ArmyId, Worst, ValueMode.Set);
                            }

                            UpdateQuery.Execute();
                        }

                        // ********************************
                        // Process Player Kills
                        // ********************************
                        Log(String.Format("Processing Kills Data ({0})", Player.Pid), LogLevel.Notice);
                        foreach (KeyValuePair<int, int> Kill in Player.Victims)
                        {
                            // Kill: VictimPid => KillCount
                            if (Driver.ExecuteScalar<int>("SELECT COUNT(*) FROM kills WHERE attacker=@P0 AND victim=@P1", Player.Pid, Kill.Key) == 0)
                            {
                                InsertQuery = new InsertQueryBuilder("kills", Driver);
                                InsertQuery.SetField("attacker", Player.Pid);
                                InsertQuery.SetField("victim", Kill.Key);
                                InsertQuery.SetField("count", Kill.Value);
                                InsertQuery.Execute();
                            }
                            else
                            {
                                UpdateQuery = new UpdateQueryBuilder("kills", Driver);
                                UpdateQuery.SetField("count", Kill.Value, ValueMode.Add);
                                Where = UpdateQuery.AddWhere("attacker", Comparison.Equals, Player.Pid);
                                Where.AddClause(LogicOperator.And, "victim", Comparison.Equals, Kill.Key);
                                UpdateQuery.Execute();
                            }
                        }


                        // ********************************
                        // Process Player Kit Data
                        // ********************************
                        Log(String.Format("Processing Kit Data ({0})", Player.Pid), LogLevel.Notice);

                        // Check for existing player data
                        if (Driver.ExecuteScalar<int>("SELECT COUNT(*) FROM kits WHERE id=" + Player.Pid) == 0)
                        {
                            InsertQuery = new InsertQueryBuilder("kits", Driver);
                            InsertQuery.SetField("id", Player.Pid);
                            for (int i = 0; i < 7; i++)
                            {
                                InsertQuery.SetField("time" + i, Player.KitData[i].Time);
                                InsertQuery.SetField("kills" + i, Player.KitData[i].Kills);
                                InsertQuery.SetField("deaths" + i, Player.KitData[i].Deaths);
                            }
                            InsertQuery.Execute();
                        }
                        else
                        {
                            UpdateQuery = new UpdateQueryBuilder("kits", Driver);
                            UpdateQuery.AddWhere("id", Comparison.Equals, Player.Pid);
                            for (int i = 0; i < 7; i++)
                            {
                                UpdateQuery.SetField("time" + i, Player.KitData[i].Time, ValueMode.Add);
                                UpdateQuery.SetField("kills" + i, Player.KitData[i].Kills, ValueMode.Add);
                                UpdateQuery.SetField("deaths" + i, Player.KitData[i].Deaths, ValueMode.Add);
                            }
                            UpdateQuery.Execute();
                        }


                        // ********************************
                        // Process Player Vehicle Data
                        // ********************************
                        Log(String.Format("Processing Vehicle Data ({0})", Player.Pid), LogLevel.Notice);

                        // Check for existing player data
                        if (Driver.ExecuteScalar<int>("SELECT COUNT(*) FROM vehicles WHERE id=" + Player.Pid) == 0)
                        {
                            InsertQuery = new InsertQueryBuilder("vehicles", Driver);
                            InsertQuery.SetField("id", Player.Pid);
                            for (int i = 0; i < 7; i++)
                            {
                                InsertQuery.SetField("time" + i, Player.VehicleData[i].Time);
                                InsertQuery.SetField("kills" + i, Player.VehicleData[i].Kills);
                                InsertQuery.SetField("deaths" + i, Player.VehicleData[i].Deaths);
                                InsertQuery.SetField("rk" + i, Player.VehicleData[i].RoadKills);
                            }
                            InsertQuery.SetField("timepara", Player.VehicleData[7].Time);
                            InsertQuery.Execute();
                        }
                        else
                        {
                            UpdateQuery = new UpdateQueryBuilder("vehicles", Driver);
                            UpdateQuery.AddWhere("id", Comparison.Equals, Player.Pid);
                            for (int i = 0; i < 7; i++)
                            {
                                UpdateQuery.SetField("time" + i, Player.VehicleData[i].Time, ValueMode.Add);
                                UpdateQuery.SetField("kills" + i, Player.VehicleData[i].Kills, ValueMode.Add);
                                UpdateQuery.SetField("deaths" + i, Player.VehicleData[i].Deaths, ValueMode.Add);
                                UpdateQuery.SetField("rk" + i, Player.VehicleData[i].RoadKills, ValueMode.Add);
                            }
                            UpdateQuery.SetField("timepara", Player.VehicleData[7].Time, ValueMode.Add);
                            UpdateQuery.Execute();
                        }


                        // ********************************
                        // Process Player Weapon Data
                        // ********************************
                        Log(String.Format("Processing Weapon Data ({0})", Player.Pid), LogLevel.Notice);

                        // Check for existing player data
                        if (Driver.ExecuteScalar<int>("SELECT COUNT(*) FROM weapons WHERE id=" + Player.Pid) == 0)
                        {
                            // Prepare Query
                            InsertQuery = new InsertQueryBuilder("weapons", Driver);
                            InsertQuery.SetField("id", Player.Pid);

                            // Basic Weapon Data
                            for (int i = 0; i < 15; i++)
                            {
                                if (i < 9)
                                {
                                    InsertQuery.SetField("time" + i, Player.WeaponData[i].Time);
                                    InsertQuery.SetField("kills" + i, Player.WeaponData[i].Kills);
                                    InsertQuery.SetField("deaths" + i, Player.WeaponData[i].Deaths);
                                    InsertQuery.SetField("fired" + i, Player.WeaponData[i].Fired);
                                    InsertQuery.SetField("hit" + i, Player.WeaponData[i].Hits);
                                }
                                else
                                {
                                    string Pfx = GetWeaponTblPrefix(i);
                                    InsertQuery.SetField(Pfx + "time", Player.WeaponData[i].Time);
                                    InsertQuery.SetField(Pfx + "kills", Player.WeaponData[i].Kills);
                                    InsertQuery.SetField(Pfx + "deaths", Player.WeaponData[i].Deaths);
                                    InsertQuery.SetField(Pfx + "fired", Player.WeaponData[i].Fired);
                                    InsertQuery.SetField(Pfx + "hit", Player.WeaponData[i].Hits);
                                }
                            }

                            // Tactical
                            InsertQuery.SetField("tacticaltime", Player.WeaponData[15].Time);
                            InsertQuery.SetField("tacticaldeployed", Player.WeaponData[15].Deployed);

                            // Grappling Hook
                            InsertQuery.SetField("grapplinghooktime", Player.WeaponData[16].Time);
                            InsertQuery.SetField("grapplinghookdeployed", Player.WeaponData[16].Deployed);
                            InsertQuery.SetField("grapplinghookdeaths", Player.WeaponData[16].Deaths);

                            // Zipline
                            InsertQuery.SetField("ziplinetime", Player.WeaponData[17].Time);
                            InsertQuery.SetField("ziplinedeployed", Player.WeaponData[17].Deployed);
                            InsertQuery.SetField("ziplinedeaths", Player.WeaponData[17].Deaths);

                            // Do Query
                            InsertQuery.Execute();
                        }
                        else
                        {
                            // Prepare Query
                            UpdateQuery = new UpdateQueryBuilder("weapons", Driver);
                            UpdateQuery.AddWhere("id", Comparison.Equals, Player.Pid);

                            // Basic Weapon Data
                            for (int i = 0; i < 15; i++)
                            {
                                if (i < 9)
                                {
                                    UpdateQuery.SetField("time" + i, Player.WeaponData[i].Time, ValueMode.Add);
                                    UpdateQuery.SetField("kills" + i, Player.WeaponData[i].Kills, ValueMode.Add);
                                    UpdateQuery.SetField("deaths" + i, Player.WeaponData[i].Deaths, ValueMode.Add);
                                    UpdateQuery.SetField("fired" + i, Player.WeaponData[i].Fired, ValueMode.Add);
                                    UpdateQuery.SetField("hit" + i, Player.WeaponData[i].Hits, ValueMode.Add);
                                }
                                else
                                {
                                    string Pfx = GetWeaponTblPrefix(i);
                                    UpdateQuery.SetField(Pfx + "time", Player.WeaponData[i].Time, ValueMode.Add);
                                    UpdateQuery.SetField(Pfx + "kills", Player.WeaponData[i].Kills, ValueMode.Add);
                                    UpdateQuery.SetField(Pfx + "deaths", Player.WeaponData[i].Deaths, ValueMode.Add);
                                    UpdateQuery.SetField(Pfx + "fired", Player.WeaponData[i].Fired, ValueMode.Add);
                                    UpdateQuery.SetField(Pfx + "hit", Player.WeaponData[i].Hits, ValueMode.Add);
                                }
                            }

                            // Tactical
                            UpdateQuery.SetField("tacticaltime", Player.WeaponData[15].Time, ValueMode.Add);
                            UpdateQuery.SetField("tacticaldeployed", Player.WeaponData[15].Deployed, ValueMode.Add);

                            // Grappling Hook
                            UpdateQuery.SetField("grapplinghooktime", Player.WeaponData[16].Time, ValueMode.Add);
                            UpdateQuery.SetField("grapplinghookdeployed", Player.WeaponData[16].Deployed, ValueMode.Add);
                            UpdateQuery.SetField("grapplinghookdeaths", Player.WeaponData[16].Deaths, ValueMode.Add);

                            // Zipline
                            UpdateQuery.SetField("ziplinetime", Player.WeaponData[17].Time, ValueMode.Add);
                            UpdateQuery.SetField("ziplinedeployed", Player.WeaponData[17].Deployed, ValueMode.Add);
                            UpdateQuery.SetField("ziplinedeaths", Player.WeaponData[17].Deaths, ValueMode.Add);

                            // Do Query
                            UpdateQuery.Execute();
                        }


                        // ********************************
                        // Process Player Map Data
                        // ********************************
                        Log(String.Format("Processing Map Data ({0})", Player.Pid), LogLevel.Notice);

                        Rows = Driver.Query("SELECT best, worst FROM maps WHERE id=@P0 AND mapid=@P1", Player.Pid, MapId);
                        if (Rows.Count == 0)
                        {
                            // Prepare Query
                            InsertQuery = new InsertQueryBuilder("maps", Driver);
                            InsertQuery.SetField("id", Player.Pid);
                            InsertQuery.SetField("mapid", this.MapId);
                            InsertQuery.SetField("time", Player.RoundTime);
                            InsertQuery.SetField("win", OnWinningTeam);
                            InsertQuery.SetField("loss", !OnWinningTeam);
                            InsertQuery.SetField("best", Player.RoundScore);
                            InsertQuery.SetField("worst", Player.RoundScore);
                            InsertQuery.Execute();
                        }
                        else
                        {
                            // Get best and worst round scores
                            Best = Int32.Parse(Rows[0]["best"].ToString());
                            Worst = Int32.Parse(Rows[0]["worst"].ToString());
                            if (Player.RoundScore > Best) Best = Player.RoundScore;
                            if (Player.RoundScore < Worst) Worst = Player.RoundScore;

                            // Prepare Query
                            UpdateQuery = new UpdateQueryBuilder("maps", Driver);
                            Where = UpdateQuery.AddWhere("id", Comparison.Equals, Player.Pid);
                            Where.AddClause(LogicOperator.And, "mapid", Comparison.Equals, this.MapId);
                            UpdateQuery.SetField("time", Player.RoundTime, ValueMode.Add);
                            UpdateQuery.SetField("win", OnWinningTeam, ValueMode.Add);
                            UpdateQuery.SetField("loss", !OnWinningTeam, ValueMode.Add);
                            UpdateQuery.SetField("best", Best, ValueMode.Set);
                            UpdateQuery.SetField("worst", Worst, ValueMode.Set);
                            UpdateQuery.Execute();
                        }


                        // ********************************
                        // Process Player Awards Data
                        // ********************************
                        Log(String.Format("Processing Award Data ({0})", Player.Pid), LogLevel.Notice);

                        // Do we require round completion for award processing?
                        if (Player.CompletedRound || !MainForm.Config.ASP_AwardsReqComplete)
                        {
                            // Prepare query
                            InsertQuery = new InsertQueryBuilder("awards", Driver);

                            // Add Backend awards too
                            foreach (BackendAward Award in BackendAwardData.BackendAwards)
                            {
                                int Level = 1;
                                if (Award.CriteriaMet(Player.Pid, Driver, ref Level))
                                    Player.EarnedAwards.Add(Award.AwardId, Level);
                            }

                            // Now we loop though each players earned award, and store them in the database
                            foreach (KeyValuePair<int, int> Award in Player.EarnedAwards)
                            {
                                // Get our award type. Award.Key is the ID, Award.Value is the level (or count)
                                bool IsMedal = Award.Key.InRange(2000000, 3000000);
                                bool IsBadge = (Award.Key < 2000000);

                                // Build our query
                                string Query = "SELECT COUNT(*) FROM awards WHERE id=@P0 AND awd=@P1";
                                if (IsBadge)
                                    Query += " AND level=" + Award.Value.ToString();

                                // Check for prior awarding of award
                                if (Driver.ExecuteScalar<int>(Query, Player.Pid, Award.Key) == 0)
                                {
                                    // Need to do extra work for Badges as more than one badge level may have been awarded.
                                    // The snapshot will only post the highest awarded level of a badge, so here we award
                                    // the lower level badges if the player does not have them.
                                    if (IsBadge)
                                    {
                                        // Check all prior badge levels, and make sure the player has them
                                        for (int j = 1; j < Award.Value; j++)
                                        {
                                            Query = "SELECT COUNT(*) FROM awards WHERE id=@P0 AND awd=@P1 AND level=@P2";
                                            if (Driver.ExecuteScalar<int>(Query, Player.Pid, Award.Key, j) == 0)
                                            {
                                                // Prepare Query
                                                InsertQuery.SetField("id", Player.Pid);
                                                InsertQuery.SetField("awd", Award.Key);
                                                InsertQuery.SetField("level", j);
                                                InsertQuery.SetField("earned", (this.RoundEndTime - 5) + j);
                                                InsertQuery.SetField("first", 0);
                                                InsertQuery.Execute();
                                            }
                                        }
                                    }

                                    // Add the players award
                                    InsertQuery.SetField("id", Player.Pid);
                                    InsertQuery.SetField("awd", Award.Key);
                                    InsertQuery.SetField("level", Award.Value);
                                    InsertQuery.SetField("earned", this.RoundEndTime);
                                    InsertQuery.SetField("first", ((IsMedal) ? this.RoundEndTime : 0));
                                    InsertQuery.Execute();

                                }
                                else // === Player has recived this award prior === //
                                {
                                    // Only update medals because ribbons and badges are only awarded once ever!
                                    if (IsMedal)
                                    {
                                        // Prepare Query
                                        UpdateQuery = new UpdateQueryBuilder("awards", Driver);
                                        Where = UpdateQuery.AddWhere("id", Comparison.Equals, Player.Pid);
                                        Where.AddClause(LogicOperator.And, "awd", Comparison.Equals, Award.Key);
                                        UpdateQuery.SetField("level", 1, ValueMode.Add);
                                        UpdateQuery.SetField("earned", this.RoundEndTime, ValueMode.Set);
                                        UpdateQuery.Execute();
                                    }
                                }

                                // Add best round count if player earned best round medal
                                if (Award.Key == 2051907 && Player.ArmyId < 14)
                                {
                                    // Prepare Query
                                    UpdateQuery = new UpdateQueryBuilder("army", Driver);
                                    UpdateQuery.AddWhere("id", Comparison.Equals, Player.Pid);
                                    UpdateQuery.SetField("brnd" + Player.ArmyId, 1, ValueMode.Add);
                                    UpdateQuery.Execute();
                                }

                            } // End Foreach Award
                        } // End Award Processing
                    } // End Foreach Player

                    // ********************************
                    // Process ServerInfo
                    // ********************************
                    //Log("Processing Game Server", LogLevel.Notice);

                    // ********************************
                    // Process MapInfo
                    // ********************************
                    Log(String.Format("Processing Map Info ({0}:{1})", this.MapName, this.MapId), LogLevel.Notice);
                    if (Driver.ExecuteScalar<int>("SELECT COUNT(*) FROM mapinfo WHERE id=" + this.MapId) == 0)
                    {
                        // Prepare Query
                        InsertQuery = new InsertQueryBuilder("mapinfo", Driver);
                        InsertQuery.SetField("id", this.MapId);
                        InsertQuery.SetField("name", this.MapName);
                        InsertQuery.SetField("score", this.MapScore);
                        InsertQuery.SetField("time", this.RoundTime.Seconds);
                        InsertQuery.SetField("times", 1);
                        InsertQuery.SetField("kills", this.MapKills);
                        InsertQuery.SetField("deaths", this.MapDeaths);
                        InsertQuery.SetField("custom", (this.IsCustomMap) ? 1 : 0);
                        InsertQuery.Execute();
                    }
                    else
                    {
                        UpdateQuery = new UpdateQueryBuilder("mapinfo", Driver);
                        UpdateQuery.AddWhere("id", Comparison.Equals, this.MapId);
                        UpdateQuery.SetField("score", this.MapScore, ValueMode.Add);
                        UpdateQuery.SetField("time", this.RoundTime.Seconds, ValueMode.Add);
                        UpdateQuery.SetField("times", 1, ValueMode.Add);
                        UpdateQuery.SetField("kills", this.MapKills, ValueMode.Add);
                        UpdateQuery.SetField("deaths", this.MapDeaths, ValueMode.Add);
                        UpdateQuery.Execute();
                    }


                    // ********************************
                    // Process RoundInfo
                    // ********************************
                    Log("Processing Round Info", LogLevel.Notice);
                    InsertQuery = new InsertQueryBuilder("round_history", Driver);
                    InsertQuery.SetField("timestamp", this.RoundEndTime);
                    InsertQuery.SetField("mapid", this.MapId);
                    InsertQuery.SetField("time", this.RoundTime.Seconds);
                    InsertQuery.SetField("team1", this.Team1ArmyId);
                    InsertQuery.SetField("team2", this.Team2ArmyId);
                    InsertQuery.SetField("tickets1", this.Team1Tickets);
                    InsertQuery.SetField("tickets2", this.Team2Tickets);
                    InsertQuery.SetField("pids1", this.Team1Players);
                    InsertQuery.SetField("pids1_end", this.Team1PlayersEnd);
                    InsertQuery.SetField("pids2", this.Team2Players);
                    InsertQuery.SetField("pids2_end", this.Team2PlayersEnd);
                    InsertQuery.Execute();

                    // ********************************
                    // Process Smoc And General Ranks
                    // ********************************
                    if (MainForm.Config.ASP_SmocCheck) SmocCheck(Driver);
                    if (MainForm.Config.ASP_GeneralCheck) GenCheck(Driver);

                    // ********************************
                    // Commit the Transaction and Log
                    // ********************************
                    Transaction.Commit();
                    Clock.Stop();

                    // Log in the stats debug log, and call it
                    this.IsProcessed = true;
                    string logText = String.Format(
                        "Snapshot ({0}) processed in {1} milliseconds [{2} Queries]", 
                        this.MapName, Clock.Elapsed.Milliseconds, Driver.NumQueries
                    );
                    Log(logText, LogLevel.Info);
                }
                catch (Exception E)
                {
                    Log("An error occured while updating player stats: " + E.Message, LogLevel.Error);
                    ExceptionHandler.GenerateExceptionLog(E);
                    Transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Adds a player to the player list, and adds his scores to the global
        /// GameResult scores
        /// </summary>
        /// <param name="Player"></param>
        private void AddPlayer(Player Player)
        {
            this.Players.Add(Player);

            // Add map data
            this.MapScore += Player.RoundScore;
            this.MapKills += Player.Stats.Kills;
            this.MapDeaths += Player.Stats.Deaths;

            // DO team counts
            if (Player.ArmyId == this.Team1ArmyId)
            {
                this.Team1Players++;
                if (Player.CompletedRound) // Completed round?
                    this.Team1PlayersEnd++;
            }
            else
            {
                this.Team2Players++;
                if (Player.CompletedRound) // Completed round?
                    this.Team2PlayersEnd++;
            }
        }

        /// <summary>
        /// Determines if a new player has has earned the new Smoc rank, and awards it
        /// </summary>
        private void SmocCheck(StatsDatabase Driver)
        {
            Log("Processing SMOC Rank", LogLevel.Notice);

            // Vars
            List<Dictionary<string, object>> Rows;
            List<Dictionary<string, object>> Players;

            // Fetch all Sergeant Major's, Order by Score
            Players = Driver.Query("SELECT id, score FROM player WHERE rank=10 OR rank=11 ORDER BY score DESC LIMIT 1");
            if (Players.Count == 1)
            {
                int Id = Int32.Parse(Players[0]["id"].ToString());

                // Check for currently awarded Smoc
                Rows = Driver.Query("SELECT id, earned FROM awards WHERE awd=6666666 LIMIT 1");
                if (Rows.Count > 0)
                {
                    // Check for same and determine if minimum tenure servred
                    int MinTenure = MainForm.Config.ASP_SpecialRankTenure * 86400;
                    int Sid = Int32.Parse(Rows[0]["id"].ToString());
                    int Earned = Int32.Parse(Rows[0]["earned"].ToString());

                    // Assign new Smoc If the old SMOC's tenure is up, and the current SMOC is not the highest scoring SGM
                    if (Id != Sid && (Earned + MinTenure) < this.RoundEndTime)
                    {
                        // Delete old SMOC's award
                        Driver.Execute("DELETE FROM awards WHERE id=@P0 AND awd=6666666", Sid);

                        // Change current SMOC rank back to SGM
                        Driver.Execute("UPDATE player SET rank=10, chng=0, decr=1 WHERE id =" + Sid);

                        // Award new SMOC award
                        Driver.Execute("INSERT INTO awards(id,awd,earned) VALUES(@P0,@P1,@P2)", Id, 6666666, this.RoundEndTime);

                        // Update new SMOC's rank
                        Driver.Execute("UPDATE player SET rank=11, chng=1, decr=0 WHERE id =" + Id);
                    }
                }
                else
                {
                    // Award SGMOC award
                    Driver.Execute("INSERT INTO awards(id,awd,earned) VALUES(@P0,@P1,@P2)", Id, 6666666, this.RoundEndTime);

                    // Update SGMOC rank
                    Driver.Execute("UPDATE player SET rank=11, chng=1, decr=0 WHERE id =" + Id);
                }
            }
        }

        /// <summary>
        /// Checks the rank tenure, and assigns a new General
        /// </summary>
        private void GenCheck(StatsDatabase Driver)
        {
            Log("Processing GENERAL Rank", LogLevel.Notice);

            // Vars
            List<Dictionary<string, object>> Rows;
            List<Dictionary<string, object>> Players;

            // Fetch all Sergeant Major's, Order by Score
            Players = Driver.Query("SELECT id, score FROM player WHERE rank=20 OR rank=21 ORDER BY score DESC LIMIT 1");
            if (Players.Count == 1)
            {
                int Id = Int32.Parse(Players[0]["id"].ToString());

                // Check for currently awarded Smoc
                Rows = Driver.Query("SELECT id, earned FROM awards WHERE awd=6666667 LIMIT 1");
                if (Rows.Count > 0)
                {
                    // Check for same and determine if minimum tenure servred
                    int MinTenure = MainForm.Config.ASP_SpecialRankTenure * 86400;
                    int Sid = Int32.Parse(Rows[0]["id"].ToString());
                    int Earned = Int32.Parse(Rows[0]["earned"].ToString());

                    // Assign new Smoc If the old SMOC's tenure is up, and the current SMOC is not the highest scoring SGM
                    if (Id != Sid && (Earned + MinTenure) < this.RoundEndTime)
                    {
                        // Delete the GENERAL award
                        Driver.Execute("DELETE FROM awards WHERE id=@P0 AND awd=6666667", Sid);

                        // Change current GENERAL rank back to 3 Star Gen
                        Driver.Execute("UPDATE player SET rank=20, chng=0, decr=1 WHERE id =" + Sid);

                        // Award new GENERAL award
                        Driver.Execute("INSERT INTO awards(id,awd,earned) VALUES(@P0,@P1,@P2)", Id, 6666667, this.RoundEndTime);

                        // Update new GENERAL's rank
                        Driver.Execute("UPDATE player SET rank=21, chng=1, decr=0 WHERE id =" + Id);
                    }
                }
                else
                {
                    // Award GENERAL award
                    Driver.Execute("INSERT INTO awards(id,awd,earned) VALUES(@P0,@P1,@P2)", Id, 6666667, this.RoundEndTime);

                    // Update GENERAL rank
                    Driver.Execute("UPDATE player SET rank=21, chng=1, decr=0 WHERE id =" + Id);
                }
            }
        }

        /// <summary>
        /// Since secondary weapons (such as knife and c4) use column prefix's
        /// in the database, we use this method to return it based on weapons ID
        /// </summary>
        /// <param name="WeaponId"></param>
        /// <returns></returns>
        private string GetWeaponTblPrefix(int WeaponId)
        {
            switch (WeaponId)
            {
                case 9:
                    return "knife";
                case 10:
                    return "c4";
                case 11:
                    return "claymore";
                case 12:
                    return "handgrenade";
                case 13:
                    return "shockpad";
                case 14:
                    return "atmine";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Logs a message in the stats debug, based on provided level
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Level"></param>
        private void Log(string Message, LogLevel Level)
        {
            if ((int)Level > MainForm.Config.ASP_DebugLevel)
                return;

            string Lvl;
            switch (Level)
            {
                default:
                    Lvl = "INFO: ";
                    break;
                case LogLevel.Security:
                    Lvl = "SECURITY: ";
                    break;
                case LogLevel.Error:
                    Lvl = "ERROR: ";
                    break;
                case LogLevel.Warning:
                    Lvl = "WARNING: ";
                    break;
                case LogLevel.Notice:
                    Lvl = "NOTICE: ";
                    break;
            }

            DebugLog.Write(Lvl + Message);
        }

        protected enum LogLevel : int
        {
            Info = -1,
            Security = 0,
            Error = 1,
            Warning = 2,
            Notice = 3,
        }
    }
}
