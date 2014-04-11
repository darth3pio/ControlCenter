using System;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Globalization;
using BF2Statistics.ASP;
using BF2Statistics.Database;
using BF2Statistics.Logging;
using BF2Statistics.Database.QueryBuilder;
using BF2Statistics.Utilities;

namespace BF2Statistics.ASP
{
    class Snapshot
    {
        /// <summary>
        /// Database driver
        /// </summary>
        private StatsDatabase Driver;

        /// <summary>
        /// Debug log file
        /// </summary>
        private static LogWritter DebugLog = new LogWritter(Path.Combine(MainForm.Root, "Logs", "StatsDebug.log"), 3000);

        /// <summary>
        /// Returns whether the snapshot data appears to be valid, and contain no obvious errors
        /// </summary>
        public bool IsValid { get; protected set; }

        /// <summary>
        /// Is this a central update snapshot?
        /// </summary>
        public bool IsCentralUpdate { get; protected set; }

        /// <summary>
        /// Snapshot Server prefix
        /// </summary>
        public string ServerPrefix { get; protected set; }

        /// <summary>
        /// Snapshot Server Name
        /// </summary>
        public string ServerName { get; protected set; }

        /// <summary>
        /// Snapshot Server Port
        /// </summary>
        public int ServerPort { get; protected set; }

        /// <summary>
        /// Snapshot Server Gamespy Query Port
        /// </summary>
        public int QueryPort { get; protected set; }

        /// <summary>
        /// Map ID Played this round
        /// </summary>
        public int MapId { get; protected set; }

        /// <summary>
        /// Is this a custom map?
        /// </summary>
        public bool IsCustomMap { get; protected set; }

        /// <summary>
        /// Map name played this round
        /// </summary>
        public string MapName { get; protected set; }

        /// <summary>
        /// Map Start time
        /// </summary>
        public int MapStart { get; protected set; }

        /// <summary>
        /// Map end time
        /// </summary>
        public int MapEnd { get; protected set; }

        /// <summary>
        /// The winning team ID
        /// </summary>
        public int WinningTeam { get; protected set; }

        /// <summary>
        /// The gamemode ID of this round (Cq, Coop, SP)
        /// </summary>
        public int GameMode { get; protected set; }

        /// <summary>
        /// Mod name that was played
        /// </summary>
        public string Mod { get; protected set; }

        /// <summary>
        /// Winning Army ID
        /// </summary>
        public int WinningArmy { get; protected set; }

        /// <summary>
        /// Team 1's Army Id
        /// </summary>
        public int Team1Army { get; protected set; }

        /// <summary>
        /// Team 2's Army Id
        /// </summary>
        public int Team2Army { get; protected set; }

        /// <summary>
        /// Remaining round tickets team 1
        /// </summary>
        public int Team1Tickets { get; protected set; }

        /// <summary>
        /// Remaining round tickets team 2
        /// </summary>
        public int Team2Tickets { get; protected set; }

        /// <summary>
        /// A Count of how many played connected
        /// </summary>
        public int PlayersConnected { get; protected set; }

        /// <summary>
        /// All of our players data
        /// </summary>
        private Dictionary<int, Dictionary<string, string>> PlayerData;

        /// <summary>
        /// All of player kill data (this can get quite huge)
        /// </summary>
        private Dictionary<int, Dictionary<string, string>> KillData;

        /// <summary>
        /// The snapshot Date
        /// </summary>
        private DateTime Date;

        /// <summary>
        /// Snapshot Timestamp
        /// </summary>
        private int TimeStamp;

        /// <summary>
        /// On Finish Event
        /// </summary>
        public static event SnapshotProccessed SnapshotProccessed;

        /// <summary>
        /// Initializes a new Snapshot, with the specified Date it was Posted
        /// </summary>
        /// <param name="Snapshot">The snapshot source</param>
        /// <param name="Date">The original date in which this snapshot was created</param>
        public Snapshot(string Snapshot, DateTime Date, StatsDatabase Database)
        {

            // Load out database connection
            this.Driver = Database;
            this.Date = Date;
            this.TimeStamp = Date.ToUnixTimestamp();

            // Get our snapshot key value pairs
            string[] Data = Snapshot.Split('\\');
            Snapshot = null;

            // Check for invalid snapshot string. All snapshots have at least 36 data pairs, 
            // and has an Even number of data sectors. We must also have an "End of File" Sector
            if (Data.Length < 36 || Data.Length % 2 != 0 || !Data.Contains("EOF"))
            {
                IsValid = false;
                return;
            }

            // Define if we are central update. the "cdb_update" variable must be the LAST sector in snapshot
            this.IsCentralUpdate = (Data[Data.Length - 2] == "cdb_update" && Data[Data.Length - 1] == "1");

            // Server data
            this.ServerPrefix = Data[0];
            this.ServerName = Data[1];
            this.ServerPort = int.Parse(Data[3].ToString());
            this.QueryPort = int.Parse(Data[5].ToString());

            // Map Data
            this.MapName = Data[7];
            this.MapId = int.Parse(Data[9]);
            this.MapStart = (int)Convert.ToDouble(Data[11], CultureInfo.InvariantCulture.NumberFormat);
            this.MapEnd = (int)Convert.ToDouble(Data[13], CultureInfo.InvariantCulture.NumberFormat);

            // Misc Data
            this.WinningTeam = int.Parse(Data[15]);
            this.GameMode = int.Parse(Data[17]);
            this.Mod = Data[21];
            this.PlayersConnected = int.Parse(Data[23]);

            // Army Data
            this.WinningArmy = int.Parse(Data[25]);
            this.Team1Army = int.Parse(Data[27]);
            this.Team1Tickets = int.Parse(Data[29]);
            this.Team2Army = int.Parse(Data[31]);
            this.Team2Tickets = int.Parse(Data[33]);

            // Setup snapshot variables
            PlayerData = new Dictionary<int, Dictionary<string, string>>();
            KillData = new Dictionary<int, Dictionary<string, string>>();

            // Check for custom map, with no ID
            if (MapId == 99)
            {
                IsCustomMap = true;

                // Check for existing map data
                List<Dictionary<string, object>> Rows = Driver.Query("SELECT id FROM mapinfo WHERE name=@P0", MapName);
                if (Rows.Count == 0)
                {
                    // Create new MapId. Id's 700 - 1000 are reserved for unknown maps in the Constants.py file
                    // There should never be more then 300 unknown map id's, considering 1001 is the start of KNOWN
                    // Custom mod map id's
                    Rows = Driver.Query("SELECT MAX(id) AS id FROM mapinfo WHERE id BETWEEN 700 AND 1000");
                    MapId = (Rows.Count == 0 || String.IsNullOrWhiteSpace(Rows[0]["id"].ToString()))
                        ? 700
                        : (Int32.Parse(Rows[0]["id"].ToString()) + 1);

                    // Insert map data, so we dont lose this mapid we generated
                    Driver.Execute("INSERT INTO mapinfo(id, name) VALUES (@P0, @P1)", MapId, MapName);
                }
                else
                    MapId = Int32.Parse(Rows[0]["id"].ToString());
            }
            else
                IsCustomMap = (MapId >= 700);

            // Do player snapshots, sector 36 is first player
            for (int i = 36; i < Data.Length; i += 2)
            {
                // Format: "DataKey_PlayerId". PlayerId is not the PID, but rather
                // the player INDEX
                string[] Parts = Data[i].Split('_');

                // Ignore uncomplete snapshots
                if (Parts.Length == 1)
                {
                    // Unless we are at the end of file, IF there is no PID
                    // Given for an item, the snapshot is invalid!
                    if (Parts[0] == "EOF")
                        break;
                    else
                        IsValid = false;
                    return;
                }

                // If the item key is "pID", then we have a new player record
                int id = int.Parse(Parts[1]);
                if (Parts[0] == "pID")
                {
                    PlayerData.Add(id, new Dictionary<string, string>());
                    KillData.Add(id, new Dictionary<string, string>());
                }

                // Kill and death data has its own array key
                if (Parts[0] == "mvks")
                    continue;

                if (Parts[0] == "mvns")
                    KillData[id].Add(Data[i + 1], Data[i + 3]);
                else
                    PlayerData[id].Add(Parts[0], Data[i + 1]);
            }

            // Set that we appear to be valid
            IsValid = true;
        }

        /// <summary>
        /// Processes the snapshot data, inserted and updating player data in the gamespy database
        /// </summary>
        /// <exception cref="InvalidDataException">Thrown if the snapshot data is invalid</exception>
        public void Process()
        {
            // Make sure we are valid, or throw exception!
            if (!IsValid)
                throw new InvalidDataException("Invalid Snapshot data!");

            // Begin Logging
            Log(String.Format("Begin Processing ({0})...", MapName), LogLevel.Notice);
            if (IsCustomMap)
                Log(String.Format("Custom Map ({0})...", MapId), LogLevel.Notice);
            else
                Log(String.Format("Standard Map ({0})...", MapId), LogLevel.Notice);

            Log("Found " + PlayerData.Count + " Player(s)...", LogLevel.Notice);

            // Make sure we meet the minimum player requirement
            if (PlayerData.Count < MainForm.Config.ASP_MinRoundPlayers)
            {
                Log("Minimum round Player count does not meet the ASP requirement... Aborting", LogLevel.Warning);
                throw new Exception("Minimum round Player count does not meet the ASP requirement");
            }

            // Start a timer!
            Stopwatch Clock = new Stopwatch();
            Clock.Start();

            // Setup some variables
            List<Dictionary<string, object>> Rows;
            InsertQueryBuilder InsertQuery;
            UpdateQueryBuilder UpdateQuery;
            WhereClause Where;

            // Temporary Map Data (For Round history and Map info tables)
            int MapScore = 0;
            int MapKills = 0;
            int MapDeaths = 0;
            int Team1Players = 0;
            int Team2Players = 0;
            int Team1PlayersEnd = 0;
            int Team2PlayersEnd = 0;

            // MySQL could throw a packet size error here, so we need will increase it!
            if (Driver.DatabaseEngine == DatabaseEngine.Mysql)
                Driver.Execute("SET GLOBAL max_allowed_packet=51200");

            // Begin Transaction
            DbTransaction Transaction = Driver.BeginTransaction();

            // To prevent half complete snapshots due to exceptions,
            // Put the whole thing in a try block, and rollback on error
            try
            {
                // Loop through each player, and process them
                foreach (KeyValuePair<int, Dictionary<string, string>> PlayerIndex in PlayerData)
                {
                    int PlayerPosition = PlayerIndex.Key;
                    Dictionary<string, string> Player = PlayerIndex.Value;

                    // Parse some player data
                    int Pid = Int32.Parse(Player["pID"]);
                    int Time = Int32.Parse(Player["ctime"]);
                    int SqlTime = Int32.Parse(Player["tsl"]);
                    int SqmTime = Int32.Parse(Player["tsm"]);
                    int LwTime = Int32.Parse(Player["tlw"]);
                    int Army = Int32.Parse(Player["a"]);
                    int RoundScore = Int32.Parse(Player["rs"]);
                    int CurRank = Int32.Parse(Player["rank"]);
                    int Kills = Int32.Parse(Player["kills"]);
                    int Deaths = Int32.Parse(Player["deaths"]);
                    int Ks = Int32.Parse(Player["ks"]);
                    int Ds = Int32.Parse(Player["ds"]);
                    bool IsAi = (Int32.Parse(Player["ai"]) != 0);
                    bool CompletedRound = (Int32.Parse(Player["c"]) == 1);
                    bool OnWinningTeam = (Int32.Parse(Player["t"]) == WinningTeam);
                    IPAddress PlayerIp = IPAddress.Loopback;

                    // Player meets min round time or are we ignoring AI?
                    if ((Time < MainForm.Config.ASP_MinRoundTime) || (MainForm.Config.ASP_IgnoreAI && IsAi))
                        continue;

                    // Add map data
                    MapScore += RoundScore;
                    MapKills += Kills;
                    MapDeaths += Deaths;

                    // Fix N/A Ip address
                    if (Player["ip"].ToUpper() == "N/A")
                        Player["ip"] = "127.0.0.1";

                    // Sometimes Squad times are negative.. idk why, but we need to fix that here
                    if (SqlTime < 0) SqlTime = 0;
                    if (SqmTime < 0) SqmTime = 0;
                    if (LwTime < 0) LwTime = 0;

                    // Log
                    Log(String.Format("Processing Player ({0})", Pid), LogLevel.Notice);

                    // Fetch the player
                    string Query;
                    Rows = Driver.Query("SELECT COUNT(id) AS count FROM player WHERE id=@P0", Pid);
                    if (int.Parse(Rows[0]["count"].ToString()) == 0)
                    {
                        // === New Player === //

                        // Log
                        Log(String.Format("Adding NEW Player ({0})", Pid), LogLevel.Notice);

                        // Get playres country code
                        IPAddress.TryParse(Player["ip"], out PlayerIp);
                        string CC = GetCountryCode(PlayerIp);

                        // Build insert data
                        InsertQuery = new InsertQueryBuilder("player", Driver);
                        InsertQuery.SetField("id", Pid);
                        InsertQuery.SetField("name", Player["name"]);
                        InsertQuery.SetField("country", CC);
                        InsertQuery.SetField("time", Time);
                        InsertQuery.SetField("rounds", Player["c"]);
                        InsertQuery.SetField("ip", Player["ip"]);
                        InsertQuery.SetField("score", Player["rs"]);
                        InsertQuery.SetField("cmdscore", Player["cs"]);
                        InsertQuery.SetField("skillscore", Player["ss"]);
                        InsertQuery.SetField("teamscore", Player["ts"]);
                        InsertQuery.SetField("kills", Player["kills"]);
                        InsertQuery.SetField("deaths", Player["deaths"]);
                        InsertQuery.SetField("captures", Player["cpc"]);
                        InsertQuery.SetField("captureassists", Player["cpa"]);
                        InsertQuery.SetField("defends", Player["cpd"]);
                        InsertQuery.SetField("damageassists", Player["ka"]);
                        InsertQuery.SetField("heals", Player["he"]);
                        InsertQuery.SetField("revives", Player["rev"]);
                        InsertQuery.SetField("ammos", Player["rsp"]);
                        InsertQuery.SetField("repairs", Player["rep"]);
                        InsertQuery.SetField("targetassists", Player["tre"]);
                        InsertQuery.SetField("driverspecials", Player["drs"]);
                        InsertQuery.SetField("teamkills", Player["tmkl"]);
                        InsertQuery.SetField("teamdamage", Player["tmdg"]);
                        InsertQuery.SetField("teamvehicledamage", Player["tmvd"]);
                        InsertQuery.SetField("suicides", Player["su"]);
                        InsertQuery.SetField("killstreak", Player["ks"]);
                        InsertQuery.SetField("deathstreak", Player["ds"]);
                        InsertQuery.SetField("rank", Player["rank"]);
                        InsertQuery.SetField("banned", Player["ban"]);
                        InsertQuery.SetField("kicked", Player["kck"]);
                        InsertQuery.SetField("cmdtime", Player["tco"]);
                        InsertQuery.SetField("sqltime", SqlTime);
                        InsertQuery.SetField("sqmtime", SqmTime);
                        InsertQuery.SetField("lwtime", LwTime);
                        InsertQuery.SetField("wins", OnWinningTeam);
                        InsertQuery.SetField("losses", !OnWinningTeam);
                        InsertQuery.SetField("availunlocks", 0);
                        InsertQuery.SetField("usedunlocks", 0);
                        InsertQuery.SetField("joined", TimeStamp);
                        InsertQuery.SetField("rndscore", Player["rs"]);
                        InsertQuery.SetField("lastonline", MapEnd);
                        InsertQuery.SetField("mode0", ((GameMode == 0) ? 1 : 0));
                        InsertQuery.SetField("mode1", ((GameMode == 1) ? 1 : 0));
                        InsertQuery.SetField("mode2", ((GameMode == 2) ? 1 : 0));
                        InsertQuery.SetField("isbot", Player["ai"]);

                        // Insert Player Data
                        InsertQuery.Execute();

                        // Create Player Unlock Data
                        Query = "INSERT INTO unlocks VALUES ";
                        for (int i = 11; i < 100; i += 11)
                            Query += String.Format("({0}, {1}, 'n'), ", Pid, i);
                        for (int i = 111; i < 556; i += 111)
                            Query += String.Format("({0}, {1}, 'n'), ", Pid, i);
                        Driver.Execute(Query.TrimEnd(new char[] { ',', ' ' }));
                    }
                    else
                    {
                        // Existing Player

                        // Log
                        Log(String.Format("Updating EXISTING Player ({0})", Pid), LogLevel.Notice);

                        // Fetch Player
                        Rows = Driver.Query("SELECT ip, country, rank, killstreak, deathstreak, rndscore FROM player WHERE id=@P0", Pid);
                        Dictionary<string, object> DataRow = Rows[0];

                        // Setup vars
                        string CC = DataRow["country"].ToString();
                        int DbRank = Int32.Parse(DataRow["rank"].ToString());

                        // Update country if the ip has changed
                        IPAddress.TryParse(Player["ip"], out PlayerIp);
                        if (DataRow["ip"].ToString() != Player["ip"])
                            CC = GetCountryCode(PlayerIp);

                        // Verify/Correct Rank
                        if (MainForm.Config.ASP_StatsRankCheck)
                        {
                            // Fail-safe in-case rank data was not obtained and reset to '0' in-game.
                            if (DbRank > CurRank)
                            {
                                Player["rank"] = DbRank.ToString();
                                DebugLog.Write("Rank Correction ({0}), Using database rank ({1})", Pid, DbRank);
                            }
                        }

                        // Calcuate best killstreak/deathstreak
                        int KillStreak = Int32.Parse(DataRow["killstreak"].ToString());
                        int DeathStreak = Int32.Parse(DataRow["deathstreak"].ToString());
                        if (Ks > KillStreak) KillStreak = Ks;
                        if (Ds > DeathStreak) DeathStreak = Ds;

                        // Calculate Best Round Score
                        int Brs = Int32.Parse(DataRow["rndscore"].ToString());
                        if (RoundScore > Brs) Brs = RoundScore;

                        // Calculate rank change
                        int chng = 0;
                        int decr = 0;
                        if (DbRank != CurRank)
                        {
                            if (CurRank > DbRank)
                                chng = 1;
                            else
                                decr = 1;
                        }

                        // Update Player Data
                        UpdateQuery = new UpdateQueryBuilder("player", Driver);
                        UpdateQuery.SetField("country", CC);
                        UpdateQuery.SetField("time", Time, ValueMode.Add);
                        UpdateQuery.SetField("rounds", Player["c"], ValueMode.Add);
                        UpdateQuery.SetField("ip", Player["ip"]);
                        UpdateQuery.SetField("score", Player["rs"], ValueMode.Add);
                        UpdateQuery.SetField("cmdscore", Player["cs"], ValueMode.Add);
                        UpdateQuery.SetField("skillscore", Player["ss"], ValueMode.Add);
                        UpdateQuery.SetField("teamscore", Player["ts"], ValueMode.Add);
                        UpdateQuery.SetField("kills", Player["kills"], ValueMode.Add);
                        UpdateQuery.SetField("deaths", Player["deaths"], ValueMode.Add);
                        UpdateQuery.SetField("captures", Player["cpc"], ValueMode.Add);
                        UpdateQuery.SetField("captureassists", Player["cpa"], ValueMode.Add);
                        UpdateQuery.SetField("defends", Player["cpd"], ValueMode.Add);
                        UpdateQuery.SetField("damageassists", Player["ks"], ValueMode.Add);
                        UpdateQuery.SetField("heals", Player["he"], ValueMode.Add);
                        UpdateQuery.SetField("revives", Player["rev"], ValueMode.Add);
                        UpdateQuery.SetField("ammos", Player["rsp"], ValueMode.Add);
                        UpdateQuery.SetField("repairs", Player["rep"], ValueMode.Add);
                        UpdateQuery.SetField("targetassists", Player["tre"], ValueMode.Add);
                        UpdateQuery.SetField("driverspecials", Player["drs"], ValueMode.Add);
                        UpdateQuery.SetField("teamkills", Player["tmkl"], ValueMode.Add);
                        UpdateQuery.SetField("teamdamage", Player["tmdg"], ValueMode.Add);
                        UpdateQuery.SetField("teamvehicledamage", Player["tmvd"], ValueMode.Add);
                        UpdateQuery.SetField("suicides", Player["su"], ValueMode.Add);
                        UpdateQuery.SetField("Killstreak", KillStreak, ValueMode.Set);
                        UpdateQuery.SetField("deathstreak", DeathStreak, ValueMode.Set);
                        UpdateQuery.SetField("rank", CurRank, ValueMode.Set);
                        UpdateQuery.SetField("banned", Player["ban"], ValueMode.Add);
                        UpdateQuery.SetField("kicked", Player["kck"], ValueMode.Add);
                        UpdateQuery.SetField("cmdtime", Player["tco"], ValueMode.Add);
                        UpdateQuery.SetField("sqltime", SqlTime, ValueMode.Add);
                        UpdateQuery.SetField("sqmtime", SqmTime, ValueMode.Add);
                        UpdateQuery.SetField("lwtime", LwTime, ValueMode.Add);
                        UpdateQuery.SetField("wins", ((OnWinningTeam) ? 1 : 0), ValueMode.Add);
                        UpdateQuery.SetField("losses", ((!OnWinningTeam) ? 1 : 0), ValueMode.Add);
                        UpdateQuery.SetField("rndscore", Brs, ValueMode.Set);
                        UpdateQuery.SetField("lastonline", TimeStamp, ValueMode.Set);
                        UpdateQuery.SetField("mode0", ((GameMode == 0) ? 1 : 0), ValueMode.Add);
                        UpdateQuery.SetField("mode1", ((GameMode == 1) ? 1 : 0), ValueMode.Add);
                        UpdateQuery.SetField("mode2", ((GameMode == 2) ? 1 : 0), ValueMode.Add);
                        UpdateQuery.SetField("chng", chng, ValueMode.Set);
                        UpdateQuery.SetField("decr", decr, ValueMode.Set);
                        UpdateQuery.SetField("isbot", Player["ai"], ValueMode.Set);
                        UpdateQuery.AddWhere("id", Comparison.Equals, Pid);
                        UpdateQuery.Execute();
                    }

                    // ********************************
                    // Insert Player history.
                    // ********************************
                    Driver.Execute(
                        "INSERT INTO player_history VALUES(@P0, @P1, @P2, @P3, @P4, @P5, @P6, @P7, @P8, @P9)",
                        Pid, TimeStamp, Time, RoundScore, Player["cs"], Player["ss"], Player["ts"],
                        Kills, Deaths, CurRank
                    );

                    // ********************************
                    // Process Player Army Data
                    // ********************************
                    Log(String.Format("Processing Army Data ({0})", Pid), LogLevel.Notice);

                    // DO team counts
                    if(Army == Team1Army)
                    {
                        Team1Players++;
                        if (CompletedRound) // Completed round?
                            Team1PlayersEnd++;
                    }
                    else
                    {
                        Team2Players++;
                        if (CompletedRound) // Completed round?
                            Team2PlayersEnd++;
                    }

                    // Update player army times
                    Rows = Driver.Query("SELECT * FROM army WHERE id=" + Pid);
                    if (Rows.Count == 0)
                    {
                        InsertQuery = new InsertQueryBuilder("army", Driver);
                        InsertQuery.SetField("id", Pid);
                        InsertQuery.SetField("time0", Player["ta0"]);
                        InsertQuery.SetField("time1", Player["ta1"]);
                        InsertQuery.SetField("time2", Player["ta2"]);
                        InsertQuery.SetField("time3", Player["ta3"]);
                        InsertQuery.SetField("time4", Player["ta4"]);
                        InsertQuery.SetField("time5", Player["ta5"]);
                        InsertQuery.SetField("time6", Player["ta6"]);
                        InsertQuery.SetField("time7", Player["ta7"]);
                        InsertQuery.SetField("time8", Player["ta8"]);
                        InsertQuery.SetField("time9", Player["ta9"]);
                        InsertQuery.SetField("time10", Player["ta10"]);
                        InsertQuery.SetField("time11", Player["ta11"]);
                        InsertQuery.SetField("time12", Player["ta12"]);
                        InsertQuery.SetField("time13", Player["ta13"]);

                        // Make sure we arent playing an unsupported army
                        if (Army < 14)
                        {
                            InsertQuery.SetField("win" + Army, ((OnWinningTeam) ? 1 : 0));
                            InsertQuery.SetField("loss" + Army, ((!OnWinningTeam) ? 1 : 0));
                            InsertQuery.SetField("score" + Army, Player["rs"]);
                            InsertQuery.SetField("best" + Army, Player["rs"]);
                            InsertQuery.SetField("worst" + Army, Player["rs"]);
                        }

                        InsertQuery.Execute();
                    }
                    else
                    {
                        UpdateQuery = new UpdateQueryBuilder("army", Driver);
                        UpdateQuery.AddWhere("id", Comparison.Equals, Pid);
                        UpdateQuery.SetField("time0", Player["ta0"], ValueMode.Add);
                        UpdateQuery.SetField("time1", Player["ta1"], ValueMode.Add);
                        UpdateQuery.SetField("time2", Player["ta2"], ValueMode.Add);
                        UpdateQuery.SetField("time3", Player["ta3"], ValueMode.Add);
                        UpdateQuery.SetField("time4", Player["ta4"], ValueMode.Add);
                        UpdateQuery.SetField("time5", Player["ta5"], ValueMode.Add);
                        UpdateQuery.SetField("time6", Player["ta6"], ValueMode.Add);
                        UpdateQuery.SetField("time7", Player["ta7"], ValueMode.Add);
                        UpdateQuery.SetField("time8", Player["ta8"], ValueMode.Add);
                        UpdateQuery.SetField("time9", Player["ta9"], ValueMode.Add);
                        UpdateQuery.SetField("time10", Player["ta10"], ValueMode.Add);
                        UpdateQuery.SetField("time11", Player["ta11"], ValueMode.Add);
                        UpdateQuery.SetField("time12", Player["ta12"], ValueMode.Add);
                        UpdateQuery.SetField("time13", Player["ta13"], ValueMode.Add);

                        // Prevent database errors with custom army IDs
                        if (Army < 14)
                        {
                            string Best = (Int32.Parse(Rows[0]["best" + Army].ToString()) > RoundScore) 
                                ? Rows[0]["best" + Army].ToString() 
                                : Player["rs"];
                            string Worst = (Int32.Parse(Rows[0]["worst" + Army].ToString()) > RoundScore)
                                ? Rows[0]["worst" + Army].ToString()
                                : Player["rs"];

                            UpdateQuery.SetField("win" + Army, OnWinningTeam, ValueMode.Add);
                            UpdateQuery.SetField("loss" + Army, !OnWinningTeam, ValueMode.Add);
                            UpdateQuery.SetField("score" + Army, Player["rs"], ValueMode.Add);
                            UpdateQuery.SetField("best" + Army, Best, ValueMode.Set);
                            UpdateQuery.SetField("worst" + Army, Worst, ValueMode.Set);
                        }

                        UpdateQuery.Execute();
                    }

                    // ********************************
                    // Process Player Kills
                    // ********************************
                    Log(String.Format("Processing Kills Data ({0})", Pid), LogLevel.Notice);

                    foreach (KeyValuePair<string, string> Kill in KillData[PlayerPosition])
                    {
                        string Victim = Kill.Key;
                        int KillCount = Int32.Parse(Kill.Value);
                        Rows = Driver.Query("SELECT count FROM kills WHERE attacker=@P0 AND victim=@P1", Pid, Victim);
                        if (Rows.Count == 0)
                        {
                            InsertQuery = new InsertQueryBuilder("kills", Driver);
                            InsertQuery.SetField("attacker", Pid);
                            InsertQuery.SetField("victim", Victim);
                            InsertQuery.SetField("count", KillCount);
                            InsertQuery.Execute();
                        }
                        else
                        {
                            UpdateQuery = new UpdateQueryBuilder("kills", Driver);
                            UpdateQuery.SetField("count", KillCount, ValueMode.Add);
                            Where = UpdateQuery.AddWhere("attacker", Comparison.Equals, Pid);
                            Where.AddClause(LogicOperator.And, "victim", Comparison.Equals, Victim);
                            UpdateQuery.Execute();
                        }
                    }


                    // ********************************
                    // Process Player Kit Data
                    // ********************************
                    Log(String.Format("Processing Kit Data ({0})", Pid), LogLevel.Notice);

                    Rows = Driver.Query("SELECT time0 FROM kits WHERE id=" + Pid);
                    if (Rows.Count == 0)
                    {
                        InsertQuery = new InsertQueryBuilder("kits", Driver);
                        InsertQuery.SetField("id", Pid);
                        for (int i = 0; i < 7; i++)
                        {
                            InsertQuery.SetField("time" + i, Player["tk" + i]);
                            InsertQuery.SetField("kills" + i, Player["kk" + i]);
                            InsertQuery.SetField("deaths" + i, Player["dk" + i]);
                        }
                        InsertQuery.Execute();
                    }
                    else
                    {
                        UpdateQuery = new UpdateQueryBuilder("kits", Driver);
                        UpdateQuery.AddWhere("id", Comparison.Equals, Pid);
                        for (int i = 0; i < 7; i++)
                        {
                            UpdateQuery.SetField("time" + i, Player["tk" + i], ValueMode.Add);
                            UpdateQuery.SetField("kills" + i, Player["kk" + i], ValueMode.Add);
                            UpdateQuery.SetField("deaths" + i, Player["dk" + i], ValueMode.Add);
                        }
                        UpdateQuery.Execute();
                    }


                    // ********************************
                    // Process Player Vehicle Data
                    // ********************************
                    Log(String.Format("Processing Vehicle Data ({0})", Pid), LogLevel.Notice);

                    Rows = Driver.Query("SELECT time0 FROM vehicles WHERE id=" + Pid);
                    if (Rows.Count == 0)
                    {
                        InsertQuery = new InsertQueryBuilder("vehicles", Driver);
                        InsertQuery.SetField("id", Pid);
                        for (int i = 0; i < 7; i++)
                        {
                            InsertQuery.SetField("time" + i, Player["tv" + i]);
                            InsertQuery.SetField("kills" + i, Player["kv" + i]);
                            InsertQuery.SetField("deaths" + i, Player["bv" + i]);
                            InsertQuery.SetField("rk" + i, Player["kvr" + i]);
                        }
                        InsertQuery.SetField("timepara", Player["tvp"]);
                        InsertQuery.Execute();
                    }
                    else
                    {
                        UpdateQuery = new UpdateQueryBuilder("vehicles", Driver);
                        UpdateQuery.AddWhere("id", Comparison.Equals, Pid);
                        for (int i = 0; i < 7; i++)
                        {
                            UpdateQuery.SetField("time" + i, Player["tv" + i], ValueMode.Add);
                            UpdateQuery.SetField("kills" + i, Player["kv" + i], ValueMode.Add);
                            UpdateQuery.SetField("deaths" + i, Player["bv" + i], ValueMode.Add);
                            UpdateQuery.SetField("rk" + i, Player["kvr" + i], ValueMode.Add);
                        }
                        UpdateQuery.SetField("timepara", Player["tvp"], ValueMode.Add);
                        UpdateQuery.Execute();
                    }


                    // ********************************
                    // Process Player Weapon Data
                    // ********************************
                    Log(String.Format("Processing Weapon Data ({0})", Pid), LogLevel.Notice);

                    Rows = Driver.Query("SELECT time0 FROM weapons WHERE id=" + Pid);
                    if (Rows.Count == 0)
                    {
                        // Prepare Query
                        InsertQuery = new InsertQueryBuilder("weapons", Driver);
                        InsertQuery.SetField("id", Pid);

                        // Basic Weapon Data
                        for (int i = 0; i < 9; i++)
                        {
                            InsertQuery.SetField("time" + i, Player["tw" + i]);
                            InsertQuery.SetField("kills" + i, Player["kw" + i]);
                            InsertQuery.SetField("deaths" + i, Player["bw" + i]);
                            InsertQuery.SetField("fired" + i, Player["sw" + i]);
                            InsertQuery.SetField("hit" + i, Player["hw" + i]);
                        }

                        // Knife Data
                        InsertQuery.SetField("knifetime", Player["te0"]);
                        InsertQuery.SetField("knifekills", Player["ke0"]);
                        InsertQuery.SetField("knifedeaths", Player["be0"]);
                        InsertQuery.SetField("knifefired", Player["se0"]);
                        InsertQuery.SetField("knifehit", Player["he0"]);

                        // C4 Data
                        InsertQuery.SetField("c4time", Player["te1"]);
                        InsertQuery.SetField("c4kills", Player["ke1"]);
                        InsertQuery.SetField("c4deaths", Player["be1"]);
                        InsertQuery.SetField("c4fired", Player["se1"]);
                        InsertQuery.SetField("c4hit", Player["he1"]);

                        // Handgrenade
                        InsertQuery.SetField("handgrenadetime", Player["te3"]);
                        InsertQuery.SetField("handgrenadekills", Player["ke3"]);
                        InsertQuery.SetField("handgrenadedeaths", Player["be3"]);
                        InsertQuery.SetField("handgrenadefired", Player["se3"]);
                        InsertQuery.SetField("handgrenadehit", Player["he3"]);

                        // Claymore
                        InsertQuery.SetField("claymoretime", Player["te2"]);
                        InsertQuery.SetField("claymorekills", Player["ke2"]);
                        InsertQuery.SetField("claymoredeaths", Player["be2"]);
                        InsertQuery.SetField("claymorefired", Player["se2"]);
                        InsertQuery.SetField("claymorehit", Player["he2"]);

                        // Shockpad
                        InsertQuery.SetField("shockpadtime", Player["te4"]);
                        InsertQuery.SetField("shockpadkills", Player["ke4"]);
                        InsertQuery.SetField("shockpaddeaths", Player["be4"]);
                        InsertQuery.SetField("shockpadfired", Player["se4"]);
                        InsertQuery.SetField("shockpadhit", Player["he4"]);

                        // At Mine
                        InsertQuery.SetField("atminetime", Player["te5"]);
                        InsertQuery.SetField("atminekills", Player["ke5"]);
                        InsertQuery.SetField("atminedeaths", Player["be5"]);
                        InsertQuery.SetField("atminefired", Player["se5"]);
                        InsertQuery.SetField("atminehit", Player["he5"]);

                        // Tactical
                        InsertQuery.SetField("tacticaltime", Player["te6"]);
                        InsertQuery.SetField("tacticaldeployed", Player["de6"]);

                        // Grappling Hook
                        InsertQuery.SetField("grapplinghooktime", Player["te7"]);
                        InsertQuery.SetField("grapplinghookdeployed", Player["de7"]);
                        InsertQuery.SetField("grapplinghookdeaths", Player["be9"]);

                        // Zipline
                        InsertQuery.SetField("ziplinetime", Player["te8"]);
                        InsertQuery.SetField("ziplinedeployed", Player["de8"]);
                        InsertQuery.SetField("ziplinedeaths", Player["be8"]);

                        // Do Query
                        InsertQuery.Execute();
                    }
                    else
                    {
                        // Prepare Query
                        UpdateQuery = new UpdateQueryBuilder("weapons", Driver);
                        UpdateQuery.AddWhere("id", Comparison.Equals, Pid);

                        // Basic Weapon Data
                        for (int i = 0; i < 9; i++)
                        {
                            UpdateQuery.SetField("time" + i, Player["tw" + i], ValueMode.Add);
                            UpdateQuery.SetField("kills" + i, Player["kw" + i], ValueMode.Add);
                            UpdateQuery.SetField("deaths" + i, Player["bw" + i], ValueMode.Add);
                            UpdateQuery.SetField("fired" + i, Player["sw" + i], ValueMode.Add);
                            UpdateQuery.SetField("hit" + i, Player["hw" + i], ValueMode.Add);
                        }

                        // Knife Data
                        UpdateQuery.SetField("knifetime", Player["te0"], ValueMode.Add);
                        UpdateQuery.SetField("knifekills", Player["ke0"], ValueMode.Add);
                        UpdateQuery.SetField("knifedeaths", Player["be0"], ValueMode.Add);
                        UpdateQuery.SetField("knifefired", Player["se0"], ValueMode.Add);
                        UpdateQuery.SetField("knifehit", Player["he0"], ValueMode.Add);

                        // C4 Data
                        UpdateQuery.SetField("c4time", Player["te1"], ValueMode.Add);
                        UpdateQuery.SetField("c4kills", Player["ke1"], ValueMode.Add);
                        UpdateQuery.SetField("c4deaths", Player["be1"], ValueMode.Add);
                        UpdateQuery.SetField("c4fired", Player["se1"], ValueMode.Add);
                        UpdateQuery.SetField("c4hit", Player["he1"], ValueMode.Add);

                        // Handgrenade
                        UpdateQuery.SetField("handgrenadetime", Player["te3"], ValueMode.Add);
                        UpdateQuery.SetField("handgrenadekills", Player["ke3"], ValueMode.Add);
                        UpdateQuery.SetField("handgrenadedeaths", Player["be3"], ValueMode.Add);
                        UpdateQuery.SetField("handgrenadefired", Player["se3"], ValueMode.Add);
                        UpdateQuery.SetField("handgrenadehit", Player["he3"], ValueMode.Add);

                        // Claymore
                        UpdateQuery.SetField("claymoretime", Player["te2"], ValueMode.Add);
                        UpdateQuery.SetField("claymorekills", Player["ke2"], ValueMode.Add);
                        UpdateQuery.SetField("claymoredeaths", Player["be2"], ValueMode.Add);
                        UpdateQuery.SetField("claymorefired", Player["se2"], ValueMode.Add);
                        UpdateQuery.SetField("claymorehit", Player["he2"], ValueMode.Add);

                        // Shockpad
                        UpdateQuery.SetField("shockpadtime", Player["te4"], ValueMode.Add);
                        UpdateQuery.SetField("shockpadkills", Player["ke4"], ValueMode.Add);
                        UpdateQuery.SetField("shockpaddeaths", Player["be4"], ValueMode.Add);
                        UpdateQuery.SetField("shockpadfired", Player["se4"], ValueMode.Add);
                        UpdateQuery.SetField("shockpadhit", Player["he4"], ValueMode.Add);

                        // At Mine
                        UpdateQuery.SetField("atminetime", Player["te5"], ValueMode.Add);
                        UpdateQuery.SetField("atminekills", Player["ke5"], ValueMode.Add);
                        UpdateQuery.SetField("atminedeaths", Player["be5"], ValueMode.Add);
                        UpdateQuery.SetField("atminefired", Player["se5"], ValueMode.Add);
                        UpdateQuery.SetField("atminehit", Player["he5"], ValueMode.Add);

                        // Tactical
                        UpdateQuery.SetField("tacticaltime", Player["te6"], ValueMode.Add);
                        UpdateQuery.SetField("tacticaldeployed", Player["de6"], ValueMode.Add);

                        // Grappling Hook
                        UpdateQuery.SetField("grapplinghooktime", Player["te7"], ValueMode.Add);
                        UpdateQuery.SetField("grapplinghookdeployed", Player["de7"], ValueMode.Add);
                        UpdateQuery.SetField("grapplinghookdeaths", Player["be9"], ValueMode.Add);

                        // Zipline
                        UpdateQuery.SetField("ziplinetime", Player["te8"], ValueMode.Add);
                        UpdateQuery.SetField("ziplinedeployed", Player["de8"], ValueMode.Add);
                        UpdateQuery.SetField("ziplinedeaths", Player["be8"], ValueMode.Add);

                        // Do Query
                        UpdateQuery.Execute();
                    }


                    // ********************************
                    // Process Player Map Data
                    // ********************************
                    Log(String.Format("Processing Map Data ({0})", Pid), LogLevel.Notice);

                    Rows = Driver.Query("SELECT best, worst FROM maps WHERE id=@P0 AND mapid=@P1", Pid, MapId);
                    if (Rows.Count == 0)
                    {
                        // Prepare Query
                        InsertQuery = new InsertQueryBuilder("maps", Driver);
                        InsertQuery.SetField("id", Pid);
                        InsertQuery.SetField("mapid", MapId);
                        InsertQuery.SetField("time", Time);
                        InsertQuery.SetField("win", ((OnWinningTeam) ? 1 : 0));
                        InsertQuery.SetField("loss", ((!OnWinningTeam) ? 1 : 0));
                        InsertQuery.SetField("best", RoundScore);
                        InsertQuery.SetField("worst", RoundScore);
                        InsertQuery.Execute();
                    }
                    else
                    {
                        // Get best and worst round scores
                        string Best = ((Int32.Parse(Rows[0]["best"].ToString()) > RoundScore) ? Rows[0]["best"].ToString() : RoundScore.ToString());
                        string Worst = ((Int32.Parse(Rows[0]["worst"].ToString()) > RoundScore) ? Rows[0]["worst"].ToString() : RoundScore.ToString());

                        // Prepare Query
                        UpdateQuery = new UpdateQueryBuilder("maps", Driver);
                        Where = UpdateQuery.AddWhere("id", Comparison.Equals, Pid);
                        Where.AddClause(LogicOperator.And, "mapid", Comparison.Equals, MapId);
                        UpdateQuery.SetField("time", Time, ValueMode.Add);
                        UpdateQuery.SetField("win", ((OnWinningTeam) ? 1 : 0), ValueMode.Add);
                        UpdateQuery.SetField("loss", ((!OnWinningTeam) ? 1 : 0), ValueMode.Add);
                        UpdateQuery.SetField("best", Best, ValueMode.Add);
                        UpdateQuery.SetField("worst", Worst, ValueMode.Add);
                        UpdateQuery.Execute();
                    }


                    // ********************************
                    // Process Player Awards Data
                    // ********************************
                    Log(String.Format("Processing Award Data ({0})", Pid), LogLevel.Notice);

                    // Do we require round completion for award processing?
                    if (CompletedRound || !MainForm.Config.ASP_AwardsReqComplete)
                    {
                        // Get our list of awards we earned in the round
                        Dictionary<int, int> Awards = GetRoundAwards(Pid, Player);
                        foreach (KeyValuePair<int, int> Award in Awards)
                        {
                            int First = 0;
                            int AwardId = Award.Key;
                            int Level = Award.Value;

                            // If isMedal
                            if (AwardId > 2000000 && AwardId < 3000000)
                                Query = String.Format("SELECT level FROM awards WHERE id={0} AND awd={1}", Pid, AwardId);
                            else
                                Query = String.Format("SELECT level FROM awards WHERE id={0} AND awd={1} AND level={2}", Pid, AwardId, Level);

                            // Check for prior awarding of award
                            Rows = Driver.Query(Query);
                            if (Rows.Count == 0)
                            {
                                // Medals
                                if (AwardId > 2000000 && AwardId < 3000000)
                                    First = TimeStamp;

                                // Badges
                                else if(AwardId < 2000000)
                                {
                                    // Need to do extra work for Badges as more than one badge per round may have been awarded
                                    for (int j = 1; j < Level; j++)
                                    {
                                        Rows = Driver.Query("SELECT level FROM awards WHERE id=@P0 AND awd=@P1 AND level=@P2", Pid, AwardId, j);
                                        if (Rows.Count == 0)
                                        {
                                            // Prepare Query
                                            InsertQuery = new InsertQueryBuilder("awards", Driver);
                                            InsertQuery.SetField("id", Pid);
                                            InsertQuery.SetField("awd", AwardId);
                                            InsertQuery.SetField("level", j);
                                            InsertQuery.SetField("earned", (TimeStamp - 5) + j);
                                            InsertQuery.SetField("first", First);
                                            InsertQuery.Execute();
                                        }
                                    }
                                }

                                // Add the players award
                                InsertQuery = new InsertQueryBuilder("awards", Driver);
                                InsertQuery.SetField("id", Pid);
                                InsertQuery.SetField("awd", AwardId);
                                InsertQuery.SetField("level", Level);
                                InsertQuery.SetField("earned", TimeStamp);
                                InsertQuery.SetField("first", First);
                                InsertQuery.Execute();

                            }
                            else
                            {
                                // Player has recived this award prior //

                                // If award if a medal (Because ribbons and badges are only awarded once ever!)
                                if (AwardId > 2000000 && AwardId < 3000000)
                                {
                                    // Prepare Query
                                    UpdateQuery = new UpdateQueryBuilder("awards", Driver);
                                    Where = UpdateQuery.AddWhere("id", Comparison.Equals, Pid);
                                    Where.AddClause(LogicOperator.And, "awd", Comparison.Equals, AwardId);
                                    UpdateQuery.SetField("level", 1, ValueMode.Add);
                                    UpdateQuery.SetField("earned", TimeStamp, ValueMode.Set);
                                    UpdateQuery.Execute();
                                }
                            }

                            // Add best round count if player earned best round medal
                            if (OnWinningTeam && AwardId == 2051907)
                            {
                                // Prepare Query
                                UpdateQuery = new UpdateQueryBuilder("army", Driver);
                                UpdateQuery.AddWhere("id", Comparison.Equals, Pid);
                                UpdateQuery.SetField("brnd" + Army, 1, ValueMode.Add);
                                UpdateQuery.Execute();
                            }

                        } // End Foreach Award
                    }
                } // End Foreach Player

                // Commit the transaction
                try
                {
                    Transaction.Commit();
                }
                catch (Exception E)
                {
                    // Log error
                    Log("An error occured while commiting player changes: " + E.Message, LogLevel.Error);
                    throw;
                }
            }
            catch(Exception E)
            {
                Log("An error occured while updating player stats: " + E.Message, LogLevel.Error);
                Transaction.Rollback();
                throw;
            }

            // ********************************
            // Process ServerInfo
            // ********************************
            //Log("Processing Game Server", LogLevel.Notice);

            // ********************************
            // Process MapInfo
            // ********************************
            Log(String.Format("Processing Map Info ({0}:{1})", MapName, MapId), LogLevel.Notice);

            TimeSpan Timer = new TimeSpan(Convert.ToInt64(MapEnd - MapStart));
            Rows = Driver.Query("SELECT COUNT(id) AS count FROM mapinfo WHERE id=" + MapId);
            if(Int32.Parse(Rows[0]["count"].ToString()) == 0)
            {
                // Prepare Query
                InsertQuery = new InsertQueryBuilder("mapinfo", Driver);
                InsertQuery.SetField("id", MapId);
                InsertQuery.SetField("name", MapName);
                InsertQuery.SetField("score", MapScore);
                InsertQuery.SetField("time", Timer.Seconds);
                InsertQuery.SetField("times", 1);
                InsertQuery.SetField("kills", MapKills);
                InsertQuery.SetField("deaths", MapDeaths);
                InsertQuery.SetField("custom", (IsCustomMap) ? 1 : 0);
                InsertQuery.Execute();
            }
            else
            {
                UpdateQuery = new UpdateQueryBuilder("mapinfo", Driver);
                UpdateQuery.AddWhere("id", Comparison.Equals, MapId);
                UpdateQuery.SetField("score", MapScore, ValueMode.Add);
                UpdateQuery.SetField("time", Timer.Seconds, ValueMode.Add);
                UpdateQuery.SetField("times", 1, ValueMode.Add);
                UpdateQuery.SetField("kills", MapKills, ValueMode.Add);
                UpdateQuery.SetField("deaths", MapDeaths, ValueMode.Add);
                UpdateQuery.Execute();
            }


            // ********************************
            // Process RoundInfo
            // ********************************
            Log("Processing Round Info", LogLevel.Notice);
            InsertQuery = new InsertQueryBuilder("round_history", Driver);
            InsertQuery.SetField("timestamp", MapStart);
            InsertQuery.SetField("mapid", MapId);
            InsertQuery.SetField("time", Timer.Seconds);
            InsertQuery.SetField("team1", Team1Army);
            InsertQuery.SetField("team2", Team2Army);
            InsertQuery.SetField("tickets1", Team1Tickets);
            InsertQuery.SetField("tickets2", Team2Tickets);
            InsertQuery.SetField("pids1", Team1Players);
            InsertQuery.SetField("pids1_end", Team1PlayersEnd);
            InsertQuery.SetField("pids2", Team2Players);
            InsertQuery.SetField("pids2_end", Team2PlayersEnd);
            InsertQuery.Execute();


            // ********************************
            // Process Smoc And General Ranks
            // ********************************
            if (MainForm.Config.ASP_SmocCheck) SmocCheck();
            if (MainForm.Config.ASP_GeneralCheck) GenCheck();

            // Call our Finished Event
            Timer = new TimeSpan(Clock.ElapsedTicks);
            Log(String.Format("Snapshot ({0}) processed in {1} milliseconds", MapName, Timer.Milliseconds), LogLevel.Info);
            SnapshotProccessed();
        }

        /// <summary>
        /// Gets the country code for a string IP address
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        private string GetCountryCode(IPAddress IP)
        {
            // Return default config Country Code
            if (IPAddress.IsLoopback(IP) || ASPServer.LocalIPs.Contains(IP))
                return MainForm.Config.ASP_LocalIpCountryCode;

            // Fetch country code from Ip2Nation
            List<Dictionary<string, object>> Rows = Driver.Query(
                "SELECT country FROM ip2nation WHERE ip < @P0 ORDER BY ip DESC LIMIT 1", 
                Networking.IP2Long(IP.ToString())
            );
            string CC = (Rows.Count == 0) ? "xx" : Rows[0]["country"].ToString();

            // Fix country!
            return (CC == "xx" || CC == "01") ? MainForm.Config.ASP_LocalIpCountryCode : CC;
        }

        /// <summary>
        /// This method gets the award id's of all awards earned by a player, in a round
        /// </summary>
        /// <param name="Player"></param>
        /// <returns></returns>
        private Dictionary<int, int> GetRoundAwards(int Pid, Dictionary<string, string> Player)
        {
            // Award format, Id => Level Earned
            Dictionary<int, int> Found = new Dictionary<int, int>();
            foreach (KeyValuePair<string, string> Item in Player)
            {
                if(AwardData.Awards.ContainsKey(Item.Key))
                    Found.Add(AwardData.Awards[Item.Key], Int32.Parse(Item.Value));
            }

            // Add Backend awards too
            foreach (BackendAward Award in AwardData.BackendAwards)
            {
                int Level;
                if (Award.CriteriaMet(Pid, Driver, out Level))
                    Found.Add(Award.AwardId, Level);
            }
            return Found;
        }

        /// <summary>
        /// Determines if a new player has has earned the new Smoc rank, and awards it
        /// </summary>
        private void SmocCheck()
        {
            Log("Processing SMOC Rank", LogLevel.Notice);

            // Vars
            List<Dictionary<string, object>> Rows;
            List<Dictionary<string, object>> Players;

            // Fetch all Sergeant Major's, Order by Score
            Players = Driver.Query("SELECT id, score FROM player WHERE rank = 10 ORDER BY score DESC LIMIT 1");
            if (Players.Count == 1)
            {
                int Id = Int32.Parse(Players[0]["id"].ToString());

                // Check for currently awarded Smoc
                Rows = Driver.Query("SELECT id, earned FROM awards WHERE awd = 6666666 LIMIT 1");
                if (Rows.Count > 0)
                {
                    // Check for same and determine if minimum tenure servred
                    int MinTenure = MainForm.Config.ASP_SpecialRankTenure * 86400;
                    int Sid = Int32.Parse(Rows[0]["id"].ToString());
                    int Earned = Int32.Parse(Rows[0]["earned"].ToString());

                    // Assign new Smoc If the old SMOC's tenure is up, and the current SMOC is not the highest scoring SGM
                    if (Id != Sid && (Earned + MinTenure) < TimeStamp)
                    {
                        // Delete old SMOC's award
                        Driver.Execute("DELETE FROM awards WHERE id=@P0 AND awd=6666666", Sid);

                        // Change current SMOC rank back to SGM
                        Driver.Execute("UPDATE player SET rank=10, chng=0, decr=1 WHERE id =" + Sid);

                        // Award new SMOC award
                        Driver.Execute("INSERT INTO awards(id,awd,earned) VALUES(@P0,@P1,@P2)", Id, 6666666, TimeStamp);

                        // Update new SMOC's rank
                        Driver.Execute("UPDATE player SET rank=11, chng=1, decr=0 WHERE id =" + Id);
                    }
                }
                else
                {
                    // Award SGMOC award
                    Driver.Execute("INSERT INTO awards(id,awd,earned) VALUES(@P0,@P1,@P2)", Id, 6666666, TimeStamp);

                    // Update SGMOC rank
                    Driver.Execute("UPDATE player SET rank=11, chng=1, decr=0 WHERE id =" + Id);
                }
            }
        }

        /// <summary>
        /// Checks the rank tenure, and assigns a new General
        /// </summary>
        private void GenCheck()
        {
            Log("Processing GENERAL Rank", LogLevel.Notice);

            // Vars
            List<Dictionary<string, object>> Rows;
            List<Dictionary<string, object>> Players;

            // Fetch all Sergeant Major's, Order by Score
            Players = Driver.Query("SELECT id, score FROM player WHERE rank = 20 ORDER BY score DESC LIMIT 1");
            if (Players.Count == 1)
            {
                int Id = Int32.Parse(Players[0]["id"].ToString());

                // Check for currently awarded Smoc
                Rows = Driver.Query("SELECT id, earned FROM awards WHERE awd = 6666667 LIMIT 1");
                if (Rows.Count > 0)
                {
                    // Check for same and determine if minimum tenure servred
                    int MinTenure = MainForm.Config.ASP_SpecialRankTenure * 86400;
                    int Sid = Int32.Parse(Rows[0]["id"].ToString());
                    int Earned = Int32.Parse(Rows[0]["earned"].ToString());

                    // Assign new Smoc If the old SMOC's tenure is up, and the current SMOC is not the highest scoring SGM
                    if (Id != Sid && (Earned + MinTenure) < TimeStamp)
                    {
                        // Delete the GENERAL award
                        Driver.Execute("DELETE FROM awards WHERE id=@P0 AND awd=6666667", Sid);

                        // Change current GENERAL rank back to 3 Star Gen
                        Driver.Execute("UPDATE player SET rank=20, chng=0, decr=1 WHERE id =" + Sid);

                        // Award new GENERAL award
                        Driver.Execute("INSERT INTO awards(id,awd,earned) VALUES(@P0,@P1,@P2)", Id, 6666667, TimeStamp);

                        // Update new GENERAL's rank
                        Driver.Execute("UPDATE player SET rank=21, chng=1, decr=0 WHERE id =" + Id);
                    }
                }
                else
                {
                    // Award GENERAL award
                    Driver.Execute("INSERT INTO awards(id,awd,earned) VALUES(@P0,@P1,@P2)", Id, 6666667, TimeStamp);

                    // Update GENERAL rank
                    Driver.Execute("UPDATE player SET rank=21, chng=1, decr=0 WHERE id =" + Id);
                }
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
