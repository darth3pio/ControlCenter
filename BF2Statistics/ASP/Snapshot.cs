using System;
using System.IO;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using BF2Statistics.ASP;
using BF2Statistics.Database;
using BF2Statistics.Logging;

namespace BF2Statistics.ASP
{
    class Snapshot
    {
        /// <summary>
        /// Database driver
        /// </summary>
        private DatabaseDriver Driver;

        /// <summary>
        /// Debug log file
        /// </summary>
        private static LogWritter DebugLog = new LogWritter(Path.Combine(MainForm.Root, "Logs", "StatsDebug.log"), 3000);

        /// <summary>
        /// Returns whether the snapshot data appears to be valid, and contain no obvious errors
        /// </summary>
        public bool IsValidSnapshot { get; protected set; }

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
        public float MapStart { get; protected set; }

        /// <summary>
        /// Map end time
        /// </summary>
        public float MapEnd { get; protected set; }

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
        private List<Dictionary<string, string>> PlayerData;

        /// <summary>
        /// All of player kill data (this can get quite huge)
        /// </summary>
        private List<Dictionary<string, string>> KillData;

        /// <summary>
        /// On Finish Event
        /// </summary>
        public event ShutdownEventHandler OnFinish;

        public Snapshot(string Snapshot)
        {
            // Load out database connection
            this.Driver = ASPServer.Database.Driver;

            // Get our snapshot key value pairs
            string[] Data = Snapshot.Split('\\');
            Snapshot = null;

            // Check for invalid snapshot string
            if (Data.Length < 36 || Data.Length % 2 != 0 || Data[Data.Length - 2] != "EOF")
            {
                IsValidSnapshot = false;
                return;
            }

            // Server data
            this.ServerPrefix = Data[0];
            this.ServerName = Data[1];
            this.ServerPort = int.Parse(Data[3].ToString());
            this.QueryPort = int.Parse(Data[5].ToString());

            // Map Data
            this.MapName = Data[7];
            this.MapId = int.Parse(Data[9]);
            this.MapStart = float.Parse(Data[11]);
            this.MapEnd = float.Parse(Data[13]);

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
            PlayerData = new List<Dictionary<string, string>>();
            KillData = new List<Dictionary<string, string>>();

            // Check for custom map
            if (MapId == 99)
            {
                IsCustomMap = true;

                // Check for existing data
                List<Dictionary<string, object>> Rows = Driver.Query("SELECT id FROM mapinfo WHERE name='{0}'", MapName);
                if (Rows.Count == 0)
                {
                    // Create new MapId
                    Rows = Driver.Query("SELECT MAX(id) AS id FROM mapinfo WHERE id >= " + MainForm.Config.ASP_CustomMapID);
                    MapId = (Rows.Count == 0) ? MainForm.Config.ASP_CustomMapID : (Int32.Parse(Rows[0]["id"].ToString()) + 1);
                    if (MapId < MainForm.Config.ASP_CustomMapID)
                        MapId = MainForm.Config.ASP_CustomMapID;

                    // Insert map data, so we dont lose this mapid we generated
                    if (Rows.Count == 0 || MapId == MainForm.Config.ASP_CustomMapID)
                        Driver.Execute("INSERT INTO mapinfo(id, name) VALUES ({0}, {1})", MapId, MapName);
                }
                else
                    MapId = Int32.Parse(Rows[0]["id"].ToString());
            }
            else
                IsCustomMap = (MapId > MainForm.Config.ASP_CustomMapID);

            // Do player snapshots, 36 is first player
            for (int i = 36; i < Data.Length; i += 2)
            {
                string[] Parts = Data[i].Split('_');

                // Ignore uncomplete snapshots
                if (Parts.Length == 1)
                {
                    if (Parts[0] == "EOF")
                        break;
                    else
                        IsValidSnapshot = false;
                    return;
                }

                int id = int.Parse(Parts[1]);
                if (Parts[0] == "pID")
                {
                    PlayerData.Add(new Dictionary<string, string>());
                    KillData.Add(new Dictionary<string, string>());
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
            IsValidSnapshot = true;
        }

        /// <summary>
        /// Processes the snapshot data, inserted and updating player data.
        /// </summary>
        /// <exception cref="InvalidDataException">Thrown if the snapshot data is invalid</exception>
        public void Process()
        {
            // Make sure we are valid, or throw exception!
            if (!IsValidSnapshot)
                throw new InvalidDataException("Invalid Snapshot data!");

            // Start a timer!
            Stopwatch Clock = new Stopwatch();
            Clock.Start();

            // Setup some variables
            List<Dictionary<string, object>> Rows;
            Dictionary<string, object> IStmt;
            SqlUpdateDictionary UStmt;
            int TimeStamp = Utils.UnixTimestamp();

            // Temporary Map Data
            int MapScore = 0;
            int MapKills = 0;
            int MapDeaths = 0;
            int Team1Players = 0;
            int Team2Players = 0;
            int Team1PlayersEnd = 0;
            int Team2PlayersEnd = 0;

            // Begin Logging
            Log(String.Format("Begin Processing ({0})...", MapName), 3);
            if(IsCustomMap)
                Log(String.Format("Custom Map ({0})...", MapId), 3);
            else
                Log(String.Format("Standard Map ({0})...", MapId), 3);

            Log("Found " + PlayerData.Count + " Player(s)...", 3);

            // Begin Transaction
            DbTransaction Transaction = Driver.BeginTransaction();

            // To prevent half complete snapshots due to exceptions,
            // Put the whole thing in a try block, and rollback on error
            try
            {
                // Loop through each player, and process them
                int PlayerPosition = 0;
                foreach (Dictionary<string, string> Player in PlayerData)
                {
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

                    // Player meets min round time or are we ignoring AI?
                    if ((Time < MainForm.Config.ASP_MinRoundTime) || (MainForm.Config.ASP_IgnoreAI && IsAi))
                        continue;

                    // Add map data
                    MapScore += RoundScore;
                    MapKills += Kills;
                    MapDeaths += Deaths;

                    // Fix N/A Ip address
                    if (Player["ip"] == "N/A")
                        Player["ip"] = "127.0.0.1";

                    // Sometimes Squad times are negative.. idk why, but we need to fix that here
                    if (SqlTime < 0) SqlTime = 0;
                    if (SqmTime < 0) SqmTime = 0;
                    if (LwTime < 0) LwTime = 0;

                    // Log
                    Log(String.Format("Processing Player ({0})", Pid), 3);

                    // Fetch the player
                    string Query;
                    Rows = Driver.Query("SELECT COUNT(id) AS count FROM player WHERE id={0}", Pid);
                    if (int.Parse(Rows[0]["count"].ToString()) == 0)
                    {
                        // New Player

                        // Log
                        Log(String.Format("Adding NEW Player ({0})", Pid), 3);

                        // Get playres country code
                        string CC;
                        Rows = Driver.Query("SELECT country FROM ip2nation WHERE ip < {0} ORDER BY ip DESC LIMIT 1", Utils.IP2Long(Player["ip"]));
                        if (Rows.Count == 0)
                            CC = "xx";
                        else
                            CC = Rows[0]["country"].ToString();

                        // Build insert data
                        IStmt = new Dictionary<string, object>();
                        IStmt.Add("id", Pid);
                        IStmt.Add("name", Player["name"]);
                        IStmt.Add("country", CC);
                        IStmt.Add("time", Time);
                        IStmt.Add("rounds", Player["c"]);
                        IStmt.Add("ip", Player["ip"]);
                        IStmt.Add("score", Player["rs"]);
                        IStmt.Add("cmdscore", Player["cs"]);
                        IStmt.Add("skillscore", Player["ss"]);
                        IStmt.Add("teamscore", Player["ts"]);
                        IStmt.Add("kills", Player["kills"]);
                        IStmt.Add("deaths", Player["deaths"]);
                        IStmt.Add("captures", Player["cpc"]);
                        IStmt.Add("captureassists", Player["cpa"]);
                        IStmt.Add("defends", Player["cpd"]);
                        IStmt.Add("damageassists", Player["ka"]);
                        IStmt.Add("heals", Player["he"]);
                        IStmt.Add("revives", Player["rev"]);
                        IStmt.Add("ammos", Player["rsp"]);
                        IStmt.Add("repairs", Player["rep"]);
                        IStmt.Add("targetassists", Player["tre"]);
                        IStmt.Add("driverspecials", Player["drs"]);
                        IStmt.Add("teamkills", Player["tmkl"]);
                        IStmt.Add("teamdamage", Player["tmdg"]);
                        IStmt.Add("teamvehicledamage", Player["tmvd"]);
                        IStmt.Add("suicides", Player["su"]);
                        IStmt.Add("killstreak", Player["ks"]);
                        IStmt.Add("deathstreak", Player["ds"]);
                        IStmt.Add("rank", Player["rank"]);
                        IStmt.Add("banned", Player["ban"]);
                        IStmt.Add("kicked", Player["kck"]);
                        IStmt.Add("cmdtime", Player["tco"]);
                        IStmt.Add("sqltime", SqlTime);
                        IStmt.Add("sqmtime", SqmTime);
                        IStmt.Add("lwtime", LwTime);
                        IStmt.Add("wins", ((OnWinningTeam) ? 1: 0));
                        IStmt.Add("losses", ((!OnWinningTeam) ? 1 : 0));
                        IStmt.Add("availunlocks", 0);
                        IStmt.Add("usedunlocks", 0);
                        IStmt.Add("joined", TimeStamp);
                        IStmt.Add("rndscore", Player["rs"]);
                        IStmt.Add("lastonline", Utils.UnixTimestamp());
                        IStmt.Add("mode0", ((GameMode == 0) ? 1 : 0));
                        IStmt.Add("mode1", ((GameMode == 1) ? 1 : 0));
                        IStmt.Add("mode2", ((GameMode == 2) ? 1 : 0));
                        IStmt.Add("isbot", Player["ai"]);

                        // Insert Player Data
                        Driver.Insert("player", IStmt);

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
                        Log(String.Format("Updating EXISTING Player ({0})", Pid), 3);

                        // Fetch Player
                        Rows = Driver.Query("SELECT ip, country, rank, killstreak, deathstreak, rndscore FROM player WHERE id={0}", Pid);
                        Dictionary<string, object> DataRow = Rows[0];

                        // Setup vars
                        string CC = DataRow["country"].ToString();
                        int DbRank = Int32.Parse(DataRow["rank"].ToString());

                        // Update country if the ip has changed
                        if (DataRow["ip"].ToString() != Player["ip"] && Player["ip"] != "127.0.0.1")
                        {
                            Rows = Driver.Query("SELECT country FROM ip2nation WHERE ip < {0} ORDER BY ip DESC LIMIT 1", Utils.IP2Long(Player["ip"]));
                            if (Rows.Count != 0)
                               CC = Rows[0]["country"].ToString();
                        }

                        // Verify/Correct Rank
                        if (MainForm.Config.ASP_StatsRankCheck)
                        {
                            // Fail-safe in-case rank data was not obtained and reset to '0' in-game.
                            if (DbRank > CurRank)
                            {
                                Player["rank"] = DbRank.ToString();
                                DebugLog.Write("Rank Correction ({0}), Using database rank ({1})", Pid);
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
                        UStmt = new SqlUpdateDictionary();
                        UStmt.Add("country", CC, true);
                        UStmt.Add("time", Time, false, ValueMode.Add);
                        UStmt.Add("rounds", Player["c"], false, ValueMode.Add);
                        UStmt.Add("ip", Player["ip"], true);
                        UStmt.Add("score", Player["rs"], false, ValueMode.Add);
                        UStmt.Add("cmdscore", Player["cs"], false, ValueMode.Add);
                        UStmt.Add("skillscore", Player["ss"], false, ValueMode.Add);
                        UStmt.Add("teamscore", Player["ts"], false, ValueMode.Add);
                        UStmt.Add("kills", Player["kills"], false, ValueMode.Add);
                        UStmt.Add("deaths", Player["deaths"], false, ValueMode.Add);
                        UStmt.Add("captures", Player["cpc"], false, ValueMode.Add);
                        UStmt.Add("captureassists", Player["cpa"], false, ValueMode.Add);
                        UStmt.Add("defends", Player["cpd"], false, ValueMode.Add);
                        UStmt.Add("damageassists", Player["ks"], false, ValueMode.Add);
                        UStmt.Add("heals", Player["he"], false, ValueMode.Add);
                        UStmt.Add("revives", Player["rev"], false, ValueMode.Add);
                        UStmt.Add("ammos", Player["rsp"], false, ValueMode.Add);
                        UStmt.Add("repairs", Player["rep"], false, ValueMode.Add);
                        UStmt.Add("targetassists", Player["tre"], false, ValueMode.Add);
                        UStmt.Add("driverspecials", Player["drs"], false, ValueMode.Add);
                        UStmt.Add("teamkills", Player["tmkl"], false, ValueMode.Add);
                        UStmt.Add("teamdamage", Player["tmdg"], false, ValueMode.Add);
                        UStmt.Add("teamvehicledamage", Player["tmvd"], false, ValueMode.Add);
                        UStmt.Add("suicides", Player["su"], false, ValueMode.Add);
                        UStmt.Add("Killstreak", KillStreak, false, ValueMode.Set);
                        UStmt.Add("deathstreak", DeathStreak, false, ValueMode.Set);
                        UStmt.Add("rank", CurRank, false, ValueMode.Set);
                        UStmt.Add("banned", Player["ban"], false, ValueMode.Add);
                        UStmt.Add("kicked", Player["kck"], false, ValueMode.Add);
                        UStmt.Add("cmdtime", Player["tco"], false, ValueMode.Add);
                        UStmt.Add("sqltime", SqlTime, false, ValueMode.Add);
                        UStmt.Add("sqmtime", SqmTime, false, ValueMode.Add);
                        UStmt.Add("lwtime", LwTime, false, ValueMode.Add);
                        UStmt.Add("wins", ((OnWinningTeam) ? 1 : 0), false, ValueMode.Add);
                        UStmt.Add("losses", ((!OnWinningTeam) ? 1 : 0), false, ValueMode.Add);
                        UStmt.Add("rndscore", Brs, false, ValueMode.Set);
                        UStmt.Add("lastonline", TimeStamp, false, ValueMode.Set);
                        UStmt.Add("mode0", ((GameMode == 0) ? 1 : 0), false, ValueMode.Add);
                        UStmt.Add("mode1", ((GameMode == 1) ? 1 : 0), false, ValueMode.Add);
                        UStmt.Add("mode2", ((GameMode == 2) ? 1 : 0), false, ValueMode.Add);
                        UStmt.Add("chng", chng, false, ValueMode.Set);
                        UStmt.Add("decr", decr, false, ValueMode.Set);
                        UStmt.Add("isbot", Player["ai"], false, ValueMode.Set);
                        Driver.Update("player", UStmt, "id=" + Pid);
                    }

                    // ********************************
                    // Insert Player history.
                    // ********************************
                    Driver.Execute(
                        "INSERT INTO player_history VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9})",
                        Pid, TimeStamp, Time, RoundScore, Player["cs"], Player["ss"], Player["ts"],
                        Kills, Deaths, CurRank
                    );

                    // ********************************
                    // Process Player Army Data
                    // ********************************
                    Log(String.Format("Processing Army Data ({0})", Pid), 3);

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
                    Rows = Driver.Query("SELECT * FROM army WHERE id={0}", Pid);
                    if (Rows.Count == 0)
                    {
                        IStmt = new Dictionary<string, object>();
                        IStmt.Add("id", Pid);
                        IStmt.Add("time0", Player["ta0"]);
                        IStmt.Add("time1", Player["ta1"]);
                        IStmt.Add("time2", Player["ta2"]);
                        IStmt.Add("time3", Player["ta3"]);
                        IStmt.Add("time4", Player["ta4"]);
                        IStmt.Add("time5", Player["ta5"]);
                        IStmt.Add("time6", Player["ta6"]);
                        IStmt.Add("time7", Player["ta7"]);
                        IStmt.Add("time8", Player["ta8"]);
                        IStmt.Add("time9", Player["ta9"]);
                        IStmt.Add("time10", Player["ta10"]);
                        IStmt.Add("time11", Player["ta11"]);
                        IStmt.Add("time12", Player["ta12"]);
                        IStmt.Add("time13", Player["ta13"]);

                        // Make sure we arent playing an unsupported army
                        if (Army < 14)
                        {
                            IStmt.Add("win" + Army, ((OnWinningTeam) ? 1 : 0));
                            IStmt.Add("loss" + Army, ((!OnWinningTeam) ? 1 : 0));
                            IStmt.Add("score" + Army, Player["rs"]);
                            IStmt.Add("best" + Army, Player["rs"]);
                            IStmt.Add("worst" + Army, Player["rs"]);
                        }

                        Driver.Insert("army", IStmt);
                    }
                    else
                    {
                        UStmt = new SqlUpdateDictionary();
                        UStmt.Add("time0", Player["ta0"], false, ValueMode.Add);
                        UStmt.Add("time1", Player["ta1"], false, ValueMode.Add);
                        UStmt.Add("time2", Player["ta2"], false, ValueMode.Add);
                        UStmt.Add("time3", Player["ta3"], false, ValueMode.Add);
                        UStmt.Add("time4", Player["ta4"], false, ValueMode.Add);
                        UStmt.Add("time5", Player["ta5"], false, ValueMode.Add);
                        UStmt.Add("time6", Player["ta6"], false, ValueMode.Add);
                        UStmt.Add("time7", Player["ta7"], false, ValueMode.Add);
                        UStmt.Add("time8", Player["ta8"], false, ValueMode.Add);
                        UStmt.Add("time9", Player["ta9"], false, ValueMode.Add);
                        UStmt.Add("time10", Player["ta10"], false, ValueMode.Add);
                        UStmt.Add("time11", Player["ta11"], false, ValueMode.Add);
                        UStmt.Add("time12", Player["ta12"], false, ValueMode.Add);
                        UStmt.Add("time13", Player["ta13"], false, ValueMode.Add);

                        // Prevent database errors with custom army IDs
                        if (Army < 14)
                        {
                            string Best = (Int32.Parse(Rows[0]["best" + Army].ToString()) > RoundScore) 
                                ? Rows[0]["best" + Army].ToString() 
                                : Player["rs"];
                            string Worst = (Int32.Parse(Rows[0]["worst" + Army].ToString()) > RoundScore)
                                ? Rows[0]["worst" + Army].ToString()
                                : Player["rs"];

                            UStmt.Add("win" + Army, ((OnWinningTeam) ? 1 : 0), false, ValueMode.Add);
                            UStmt.Add("loss" + Army, ((!OnWinningTeam) ? 1 : 0), false, ValueMode.Add);
                            UStmt.Add("score" + Army, Player["rs"], false, ValueMode.Add);
                            UStmt.Add("best" + Army, Best, false, ValueMode.Set);
                            UStmt.Add("worst" + Army, Worst, false, ValueMode.Set);
                        }

                        Driver.Update("army", UStmt, "id=" + Pid);
                    }

                    // ********************************
                    // Process Player Kills
                    // ********************************
                    Log(String.Format("Processing Kills Data ({0})", Pid), 3);

                    foreach (KeyValuePair<string, string> Kill in KillData[PlayerPosition])
                    {
                        string Victim = Kill.Key;
                        int KillCount = Int32.Parse(Kill.Value);
                        Rows = Driver.Query("SELECT count FROM kills WHERE attacker={0} AND victim={1}", Pid, Victim);
                        if (Rows.Count == 0)
                        {
                            IStmt = new Dictionary<string, object>();
                            IStmt.Add("attacker", Pid);
                            IStmt.Add("victim", Victim);
                            IStmt.Add("count", KillCount);
                            Driver.Insert("kills", IStmt);
                        }
                        else
                        {
                            UStmt = new SqlUpdateDictionary();
                            UStmt.Add("count", KillCount, false, ValueMode.Add);
                            Driver.Update("kills", UStmt, String.Format("attacker={0} AND victim={1}", Pid, Victim));
                        }
                    }


                    // ********************************
                    // Process Player Kit Data
                    // ********************************
                    Log(String.Format("Processing Kit Data ({0})", Pid), 3);

                    Rows = Driver.Query("SELECT time0 FROM kits WHERE id=" + Pid);
                    if (Rows.Count == 0)
                    {
                        IStmt = new Dictionary<string, object>();
                        IStmt.Add("id", Pid);
                        for (int i = 0; i < 7; i++)
                        {
                            IStmt.Add("time" + i, Player["tk" + i]);
                            IStmt.Add("kills" + i, Player["kk" + i]);
                            IStmt.Add("deaths" + i, Player["dk" + i]);
                        }
                        Driver.Insert("kits", IStmt);
                    }
                    else
                    {
                        UStmt = new SqlUpdateDictionary();
                        for (int i = 0; i < 7; i++)
                        {
                            UStmt.Add("time" + i, Player["tk" + i], false, ValueMode.Add);
                            UStmt.Add("kills" + i, Player["kk" + i], false, ValueMode.Add);
                            UStmt.Add("deaths" + i, Player["dk" + i], false, ValueMode.Add);
                        }
                        Driver.Update("kits", UStmt, "id=" + Pid);
                    }


                    // ********************************
                    // Process Player Vehicle Data
                    // ********************************
                    Log(String.Format("Processing Vehicle Data ({0})", Pid), 3);

                    Rows = Driver.Query("SELECT time0 FROM vehicles WHERE id=" + Pid);
                    if (Rows.Count == 0)
                    {
                        IStmt = new Dictionary<string, object>();
                        IStmt.Add("id", Pid);
                        for (int i = 0; i < 7; i++)
                        {
                            IStmt.Add("time" + i, Player["tv" + i]);
                            IStmt.Add("kills" + i, Player["kv" + i]);
                            IStmt.Add("deaths" + i, Player["bv" + i]);
                            IStmt.Add("rk" + i, Player["kvr" + i]);
                        }
                        IStmt.Add("timepara", Player["tvp"]);
                        Driver.Insert("vehicles", IStmt);
                    }
                    else
                    {
                        UStmt = new SqlUpdateDictionary();
                        for (int i = 0; i < 7; i++)
                        {
                            UStmt.Add("time" + i, Player["tv" + i], false, ValueMode.Add);
                            UStmt.Add("kills" + i, Player["kv" + i], false, ValueMode.Add);
                            UStmt.Add("deaths" + i, Player["bv" + i], false, ValueMode.Add);
                            UStmt.Add("rk" + i, Player["kvr" + i], false, ValueMode.Add);
                        }
                        UStmt.Add("timepara", Player["tvp"], false, ValueMode.Add);
                        Driver.Update("vehicles", UStmt, "id=" + Pid);
                    }


                    // ********************************
                    // Process Player Weapon Data
                    // ********************************
                    Log(String.Format("Processing Weapon Data ({0})", Pid), 3);

                    Rows = Driver.Query("SELECT time0 FROM weapons WHERE id=" + Pid);
                    if (Rows.Count == 0)
                    {
                        IStmt = new Dictionary<string, object>();
                        IStmt.Add("id", Pid);

                        // Basic Weapon Data
                        for (int i = 0; i < 9; i++)
                        {
                            IStmt.Add("time" + i, Player["tw" + i]);
                            IStmt.Add("kills" + i, Player["kw" + i]);
                            IStmt.Add("deaths" + i, Player["bw" + i]);
                            IStmt.Add("fired" + i, Player["sw" + i]);
                            IStmt.Add("hit" + i, Player["hw" + i]);
                        }

                        // Knife Data
                        IStmt.Add("knifetime", Player["te0"]);
                        IStmt.Add("knifekills", Player["ke0"]);
                        IStmt.Add("knifedeaths", Player["be0"]);
                        IStmt.Add("knifefired", Player["se0"]);
                        IStmt.Add("knifehit", Player["he0"]);

                        // C4 Data
                        IStmt.Add("c4time", Player["te1"]);
                        IStmt.Add("c4kills", Player["ke1"]);
                        IStmt.Add("c4deaths", Player["be1"]);
                        IStmt.Add("c4fired", Player["se1"]);
                        IStmt.Add("c4hit", Player["he1"]);

                        // Handgrenade
                        IStmt.Add("handgrenadetime", Player["te3"]);
                        IStmt.Add("handgrenadekills", Player["ke3"]);
                        IStmt.Add("handgrenadedeaths", Player["be3"]);
                        IStmt.Add("handgrenadefired", Player["se3"]);
                        IStmt.Add("handgrenadehit", Player["he3"]);

                        // Claymore
                        IStmt.Add("claymoretime", Player["te2"]);
                        IStmt.Add("claymorekills", Player["ke2"]);
                        IStmt.Add("claymoredeaths", Player["be2"]);
                        IStmt.Add("claymorefired", Player["se2"]);
                        IStmt.Add("claymorehit", Player["he2"]);

                        // Shockpad
                        IStmt.Add("shockpadtime", Player["te4"]);
                        IStmt.Add("shockpadkills", Player["ke4"]);
                        IStmt.Add("shockpaddeaths", Player["be4"]);
                        IStmt.Add("shockpadfired", Player["se4"]);
                        IStmt.Add("shockpadhit", Player["he4"]);

                        // At Mine
                        IStmt.Add("atminetime", Player["te5"]);
                        IStmt.Add("atminekills", Player["ke5"]);
                        IStmt.Add("atminedeaths", Player["be5"]);
                        IStmt.Add("atminefired", Player["se5"]);
                        IStmt.Add("atminehit", Player["he5"]);

                        // Tactical
                        IStmt.Add("tacticaltime", Player["te6"]);
                        IStmt.Add("tacticaldeployed", Player["de6"]);

                        // Grappling Hook
                        IStmt.Add("grapplinghooktime", Player["te7"]);
                        IStmt.Add("grapplinghookdeployed", Player["de7"]);
                        IStmt.Add("grapplinghookdeaths", Player["be9"]);

                        // Zipline
                        IStmt.Add("ziplinetime", Player["te8"]);
                        IStmt.Add("ziplinedeployed", Player["de8"]);
                        IStmt.Add("ziplinedeaths", Player["be8"]);

                        // Do Query
                        Driver.Insert("weapons", IStmt);
                    }
                    else
                    {
                        UStmt = new SqlUpdateDictionary();

                        // Basic Weapon Data
                        for (int i = 0; i < 9; i++)
                        {
                            UStmt.Add("time" + i, Player["tw" + i], false, ValueMode.Add);
                            UStmt.Add("kills" + i, Player["kw" + i], false, ValueMode.Add);
                            UStmt.Add("deaths" + i, Player["bw" + i], false, ValueMode.Add);
                            UStmt.Add("fired" + i, Player["sw" + i], false, ValueMode.Add);
                            UStmt.Add("hit" + i, Player["hw" + i], false, ValueMode.Add);
                        }

                        // Knife Data
                        UStmt.Add("knifetime", Player["te0"], false, ValueMode.Add);
                        UStmt.Add("knifekills", Player["ke0"], false, ValueMode.Add);
                        UStmt.Add("knifedeaths", Player["be0"], false, ValueMode.Add);
                        UStmt.Add("knifefired", Player["se0"], false, ValueMode.Add);
                        UStmt.Add("knifehit", Player["he0"], false, ValueMode.Add);

                        // C4 Data
                        UStmt.Add("c4time", Player["te1"], false, ValueMode.Add);
                        UStmt.Add("c4kills", Player["ke1"], false, ValueMode.Add);
                        UStmt.Add("c4deaths", Player["be1"], false, ValueMode.Add);
                        UStmt.Add("c4fired", Player["se1"], false, ValueMode.Add);
                        UStmt.Add("c4hit", Player["he1"], false, ValueMode.Add);

                        // Handgrenade
                        UStmt.Add("handgrenadetime", Player["te3"], false, ValueMode.Add);
                        UStmt.Add("handgrenadekills", Player["ke3"], false, ValueMode.Add);
                        UStmt.Add("handgrenadedeaths", Player["be3"], false, ValueMode.Add);
                        UStmt.Add("handgrenadefired", Player["se3"], false, ValueMode.Add);
                        UStmt.Add("handgrenadehit", Player["he3"], false, ValueMode.Add);

                        // Claymore
                        UStmt.Add("claymoretime", Player["te2"], false, ValueMode.Add);
                        UStmt.Add("claymorekills", Player["ke2"], false, ValueMode.Add);
                        UStmt.Add("claymoredeaths", Player["be2"], false, ValueMode.Add);
                        UStmt.Add("claymorefired", Player["se2"], false, ValueMode.Add);
                        UStmt.Add("claymorehit", Player["he2"], false, ValueMode.Add);

                        // Shockpad
                        UStmt.Add("shockpadtime", Player["te4"], false, ValueMode.Add);
                        UStmt.Add("shockpadkills", Player["ke4"], false, ValueMode.Add);
                        UStmt.Add("shockpaddeaths", Player["be4"], false, ValueMode.Add);
                        UStmt.Add("shockpadfired", Player["se4"], false, ValueMode.Add);
                        UStmt.Add("shockpadhit", Player["he4"], false, ValueMode.Add);

                        // At Mine
                        UStmt.Add("atminetime", Player["te5"], false, ValueMode.Add);
                        UStmt.Add("atminekills", Player["ke5"], false, ValueMode.Add);
                        UStmt.Add("atminedeaths", Player["be5"], false, ValueMode.Add);
                        UStmt.Add("atminefired", Player["se5"], false, ValueMode.Add);
                        UStmt.Add("atminehit", Player["he5"], false, ValueMode.Add);

                        // Tactical
                        UStmt.Add("tacticaltime", Player["te6"], false, ValueMode.Add);
                        UStmt.Add("tacticaldeployed", Player["de6"], false, ValueMode.Add);

                        // Grappling Hook
                        UStmt.Add("grapplinghooktime", Player["te7"], false, ValueMode.Add);
                        UStmt.Add("grapplinghookdeployed", Player["de7"], false, ValueMode.Add);
                        UStmt.Add("grapplinghookdeaths", Player["be9"], false, ValueMode.Add);

                        // Zipline
                        UStmt.Add("ziplinetime", Player["te8"], false, ValueMode.Add);
                        UStmt.Add("ziplinedeployed", Player["de8"], false, ValueMode.Add);
                        UStmt.Add("ziplinedeaths", Player["be8"], false, ValueMode.Add);

                        // Do Query
                        Driver.Update("weapons", UStmt, "id=" + Pid);
                    }


                    // ********************************
                    // Process Player Map Data
                    // ********************************
                    Log(String.Format("Processing Map Data ({0})", Pid), 3);

                    Rows = Driver.Query("SELECT best, worst FROM maps WHERE id={0} AND mapid={1}", Pid, MapId);
                    if (Rows.Count == 0)
                    {
                        IStmt = new Dictionary<string, object>();
                        IStmt.Add("id", Pid);
                        IStmt.Add("mapid", MapId);
                        IStmt.Add("time", Time);
                        IStmt.Add("win", ((OnWinningTeam) ? 1 : 0));
                        IStmt.Add("loss", ((!OnWinningTeam) ? 1 : 0));
                        IStmt.Add("best", RoundScore);
                        IStmt.Add("worst", RoundScore);
                        Driver.Insert("maps", IStmt);
                    }
                    else
                    {
                        string Best = ((Int32.Parse(Rows[0]["best"].ToString()) > RoundScore) ? Rows[0]["best"].ToString() : RoundScore.ToString());
                        string Worst = ((Int32.Parse(Rows[0]["worst"].ToString()) > RoundScore) ? Rows[0]["worst"].ToString() : RoundScore.ToString());

                        UStmt = new SqlUpdateDictionary();
                        UStmt.Add("time", Time, false, ValueMode.Add);
                        UStmt.Add("win", ((OnWinningTeam) ? 1 : 0), false, ValueMode.Add);
                        UStmt.Add("loss", ((!OnWinningTeam) ? 1 : 0), false, ValueMode.Add);
                        UStmt.Add("best", Best, false, ValueMode.Add);
                        UStmt.Add("worst", Worst, false, ValueMode.Add);
                        Driver.Update("maps", UStmt, String.Format("id={0} AND mapid={1}", Pid, MapId));
                    }


                    // ********************************
                    // Process Player Awards Data
                    // ********************************
                    Log(String.Format("Processing Award Data ({0})", Pid), 3);

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
                                        Rows = Driver.Query("SELECT level FROM awards WHERE id={0} AND awd={1} AND level={2}", Pid, AwardId, j);
                                        if (Rows.Count == 0)
                                        {
                                            IStmt = new Dictionary<string, object>();
                                            IStmt.Add("id", Pid);
                                            IStmt.Add("awd", AwardId);
                                            IStmt.Add("level", j);
                                            IStmt.Add("earned", (TimeStamp - 5) + j);
                                            IStmt.Add("first", First);
                                            Driver.Insert("awards", IStmt);
                                        }
                                    }
                                }

                                // Add the players award
                                IStmt = new Dictionary<string, object>();
                                IStmt.Add("id", Pid);
                                IStmt.Add("awd", AwardId);
                                IStmt.Add("level", Level);
                                IStmt.Add("earned", TimeStamp);
                                IStmt.Add("first", First);
                                Driver.Insert("awards", IStmt);

                            }
                            else
                            {
                                // Player has recived this award prior //

                                // If award if a medal (Because ribbons and badges are only awarded once ever!)
                                if (AwardId > 2000000 && AwardId < 3000000)
                                {
                                    UStmt = new SqlUpdateDictionary();
                                    UStmt.Add("level", 1, false, ValueMode.Add);
                                    UStmt.Add("earned", TimeStamp, false, ValueMode.Set);
                                    Driver.Update("awards", UStmt, String.Format("id={0} AND awd={1}", Pid, AwardId));
                                }
                            }

                            // Add best round count if player earned best round medal
                            if (OnWinningTeam && AwardId == 2051907)
                            {
                                UStmt = new SqlUpdateDictionary();
                                UStmt.Add("brnd" + Army, 1, false, ValueMode.Add);
                                Driver.Update("army", UStmt, "id=" + Pid);
                            }

                        } // End Foreach Award
                    }


                    PlayerPosition++;
                } // End Foreach Player
            }
            catch(Exception E)
            {
                Log("An error occured while updating player stats: " + E.Message, 1);
                Transaction.Rollback();
            }

            try
            {
                Transaction.Commit();
            }
            catch(Exception E)
            {
                Log("An error occured while commiting player changes: " + E.Message, 1);
            }

            // ********************************
            // Process ServerInfo
            // ********************************
            //Log("Processing Game Server", 3);


            // ********************************
            // Process MapInfo
            // ********************************
            Log(String.Format("Processing Map Info ({0}:{1})", MapName, MapId), 3);

            TimeSpan Timer = new TimeSpan(Convert.ToInt64(MapEnd - MapStart));
            Rows = Driver.Query("SELECT COUNT(id) AS count FROM mapinfo WHERE id=" + MapId);
            if(Int32.Parse(Rows[0]["count"].ToString()) == 0)
            {
                IStmt = new Dictionary<string, object>();
                IStmt.Add("id", MapId);
                IStmt.Add("name", MapName);
                IStmt.Add("score", MapScore);
                IStmt.Add("time", Timer.Seconds);
                IStmt.Add("times", 1);
                IStmt.Add("kills", MapKills);
                IStmt.Add("deaths", MapDeaths);
                IStmt.Add("custom", (IsCustomMap) ? 1 : 0);
                Driver.Insert("mapinfo", IStmt);
            }
            else
            {
                UStmt = new SqlUpdateDictionary();
                UStmt.Add("score", MapScore, false, ValueMode.Add);
                UStmt.Add("time", Timer.Seconds, false, ValueMode.Add);
                UStmt.Add("times", 1, false, ValueMode.Add);
                UStmt.Add("kills", MapKills, false, ValueMode.Add);
                UStmt.Add("deaths", MapDeaths, false, ValueMode.Add);
                Driver.Update("mapinfo", UStmt, "id=" + MapId);
            }


            // ********************************
            // Process RoundInfo
            // ********************************
            Log("Processing Round Info", 3);
            IStmt = new Dictionary<string, object>();
            IStmt.Add("timestamp", MapStart);
            IStmt.Add("mapid", MapId);
            IStmt.Add("time", Timer.Seconds);
            IStmt.Add("team1", Team1Army);
            IStmt.Add("team2", Team2Army);
            IStmt.Add("tickets1", Team1Tickets);
            IStmt.Add("tickets2", Team2Tickets);
            IStmt.Add("pids1", Team1Players);
            IStmt.Add("pids1_end", Team1PlayersEnd);
            IStmt.Add("pids2", Team2Players);
            IStmt.Add("pids2_end", Team2PlayersEnd);
            Driver.Insert("round_history", IStmt);


            // ********************************
            // Process Smoc And General Ranks
            // ********************************
            if (MainForm.Config.ASP_SmocCheck) SmocCheck();
            if (MainForm.Config.ASP_GeneralCheck) GenCheck();

            // Call our Finished Event
            Timer = new TimeSpan(Clock.ElapsedTicks);
            Log(String.Format("Snapshot ({0}) processed in {1} milliseconds", MapName, Timer.Milliseconds), -1);
            OnCallFinish();
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
                if (Award.CriteriaMet(Pid, out Level))
                    Found.Add(Award.AwardId, Level);
            }
            return Found;
        }

        /// <summary>
        /// Determines if a new player has has earned the new Smoc rank, and awards it
        /// </summary>
        private void SmocCheck()
        {
            Log("Processing SMOC Rank", 3);

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

                    // Assign new Smoc If the old SMOC's tenure is up, and the current SMOC is not the highest scoring SGM
                    if (Id != Sid && Utils.UnixTimestamp() >= MinTenure)
                    {
                        // Delete old SMOC's award
                        Driver.Execute("DELETE FROM awards WHERE id={0} AND awd=6666666", Sid);

                        // Change current SMOC rank back to SGM
                        Driver.Execute("UPDATE player SET rank=10, chng=0, decr=1 WHERE id =" + Sid);

                        // Award new SGMOC award
                        Driver.Execute("INSERT INTO awards(id,awd,earned) VALUES({0},{1},{2})", Id, 6666666, Utils.UnixTimestamp());

                        // Update new SGMOC's rank
                        Driver.Execute("UPDATE player SET rank=11, chng=1, decr=0 WHERE id =" + Id);
                    }
                }
                else
                {
                    // Award SGMOC award
                    Driver.Execute("INSERT INTO awards(id,awd,earned) VALUES({0},{1},{2})", Id, 6666666, Utils.UnixTimestamp());

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
            Log("Processing GENERAL Rank", 3);

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

                    // Assign new Smoc If the old SMOC's tenure is up, and the current SMOC is not the highest scoring SGM
                    if (Id != Sid && Utils.UnixTimestamp() >= MinTenure)
                    {
                        // Delete the GENERAL award
                        Driver.Execute("DELETE FROM awards WHERE id={0} AND awd=6666667", Sid);

                        // Change current GENERAL rank back to 3 Star Gen
                        Driver.Execute("UPDATE player SET rank=20, chng=0, decr=1 WHERE id =" + Sid);

                        // Award new GENERAL award
                        Driver.Execute("INSERT INTO awards(id,awd,earned) VALUES({0},{1},{2})", Id, 6666667, Utils.UnixTimestamp());

                        // Update new GENERAL's rank
                        Driver.Execute("UPDATE player SET rank=21, chng=1, decr=0 WHERE id =" + Id);
                    }
                }
                else
                {
                    // Award GENERAL award
                    Driver.Execute("INSERT INTO awards(id,awd,earned) VALUES({0},{1},{2})", Id, 6666667, Utils.UnixTimestamp());

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
        private void Log(string Message, int Level)
        {
            string Lvl;
            switch (Level)
            {
                default:
                    Lvl = "INFO: ";
                    break;
                case 0:
                    Lvl = "SECURITY: ";
                    break;
                case 1:
                    Lvl = "ERROR: ";
                    break;
                case 2:
                    Lvl = "WARNING: ";
                    break;
                case 3:
                    Lvl = "NOTICE: ";
                    break;
            }

            DebugLog.Write(Lvl + Message);
        }

        /// <summary>
        /// A method to fire off the OnFinish event IF there is registered methods
        /// </summary>
        private void OnCallFinish()
        {
            ShutdownEventHandler tmp = OnFinish;
            if (tmp != null)
                tmp();
        }
    }
}
