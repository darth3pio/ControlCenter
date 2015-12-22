using System;
using System.Collections.Generic;
using System.Linq;
using BF2Statistics.Database;

namespace BF2Statistics.Web.ASP
{
    /// <summary>
    /// /ASP/getplayerinfo.aspx
    /// </summary>
    public sealed class GetPlayerInfo : ASPController
    {
        /// <summary>
        /// Database Rows
        /// </summary>
        private List<Dictionary<string, object>> Rows;

        /// <summary>
        /// Player ID
        /// </summary>
        private int Pid = 0;

        /// <summary>
        /// Alternate Format Variable
        /// </summary>
        private int Transpose = 0;

        /// <summary>
        /// A list of each requested info querystring
        /// </summary>
        private List<string> Info = new List<string>();

        /// <summary>
        /// Preperation Output Variable
        /// </summary>
        private Dictionary<string, object> Out = new Dictionary<string, object>();

        /// <summary>
        /// The required Querystring for BF2HQ
        /// </summary>
        private static string RequiredKeys = "per*,cmb*,twsc,cpcp,cacp,dfcp,kila,heal,rviv,rsup,rpar,"
		    + "tgte,dkas,dsab,cdsc,rank,cmsc,kick,kill,deth,suic,ospm,"
		    + "klpm,klpr,dtpr,bksk,wdsk,bbrs,tcdr,ban,dtpm,lbtl,osaa,"
		    + "vrk,tsql,tsqm,tlwf,mvks,vmks,mvn*,vmr*,fkit,fmap,fveh,fwea,"
		    + "wtm-,wkl-,wdt-,wac-,wkd-,vtm-,vkl-,vdt-,vkd-,vkr-,"
		    + "atm-,awn-,alo-,abr-,ktm-,kkl-,kdt-,kkd-";

        //&info=per*,cmb*,twsc,cpcp,cacp,dfcp,kila,heal,rviv,rsup,rpar,tgte,dkas,dsab,cdsc,rank,cmsc,kick,kill,deth,suic,ospm,klpm,klpr,dtpr,bksk,wdsk,bbrs,tcdr,ban,dtpm,lbtl,osaa,vrk,tsql,tsqm,tlwf,mvks,vmks,mvn*,vmr*,fkit,fmap,fveh,fwea,wtm-,wkl-,wdt-,wac-,wkd-,vtm-,vkl-,vdt-,vkd-,vkr-,atm-,awn-,alo-,abr-,ktm-,kkl-,kdt-,kkd-

        /// <summary>
        /// This request provides details on a particular player
        /// </summary>
        /// <queryParam name="pid" type="int">The unique player ID</queryParam>
        /// <queryParam name="info" type="string">The requested player data keys, seperated by a comma</queryParam>
        /// <param name="Client">The HttpClient who made the request</param>
        public GetPlayerInfo(HttpClient Client) : base(Client) { }

        public override void HandleRequest()
        {
            // Setup Params
            if (Request.QueryString.ContainsKey("pid"))
                Int32.TryParse(Request.QueryString["pid"], out Pid);
            if (Request.QueryString.ContainsKey("transpose"))
                Int32.TryParse(Request.QueryString["transpose"], out Transpose);
            if (Request.QueryString.ContainsKey("info"))
                Info = Request.QueryString["info"].Split(',').ToList<string>();

            // Make sure our required params are indeed passed
            if (Pid == 0 || Info.Count == 0)
            {
                Response.WriteResponseStart(false);
                Response.WriteHeaderLine("asof", "err");
                Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp(), "Invalid Syntax!");
                Response.Send();
                return;
            }

            // NOTE: The HttpServer will handle the DbConnectException
            using (Database = new StatsDatabase())
            {
                // Get Missing keys for a standard request
                List<string> MissingKeys = RequiredKeys.Split(',').Except(Info).ToList();

                // Standard BF2HQ Request
                if (MissingKeys.Count == 0)
                    DoFullRequest();
                // Time Info
                else if (Request.QueryString["info"] == "ktm-,vtm-,wtm-,mtm-")
                    DoTimeRequest();
                // Map Info
                else if (Request.QueryString["info"].StartsWith("mtm-,mwn-,mls-"))
                    DoMapRequest();
                else if (Request.QueryString["info"].StartsWith("rank") && Request.QueryString["info"].EndsWith("vac-"))
                    DoServerRequest();
                else
                    Response.Send();
            }
        }

        /// <summary>
        /// Produces a FULL player response for BF2HQ
        /// </summary>
        public void DoFullRequest()
        {
            // Fetch Player
            Rows = Database.Query("SELECT * FROM player WHERE id=@P0", Pid);

            // If player doesnt exist then output default dataB
            if (Rows.Count == 0)
            {
                Response.WriteResponseStart(false);
                Response.WriteHeaderLine("asof", "err");
                Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp(), "Player Doesnt Exist");
                Response.Send();
                return;
            }
            else
            {
                Response.WriteResponseStart();
                Response.WriteHeaderLine("asof");
                Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp());
            }

            // Add Player Data
            Dictionary<string, object> Player = Rows[0];
            Rows = null;
            float Time = float.Parse(Player["time"].ToString());

            // Start adding Player Data
            Out.Add("pid", Player["id"]);
            Out.Add("nick", Player["name"].ToString().Trim());
            Out.Add("scor", Player["score"]);
            Out.Add("jond", Player["joined"]);
            Out.Add("wins", Player["wins"]);
            Out.Add("loss", Player["losses"]);
            Out.Add("mode0", Player["mode0"]);
            Out.Add("mode1", Player["mode1"]);
            Out.Add("mode2", Player["mode2"]);
            Out.Add("time", Player["time"]);
            Out.Add("smoc", (Int32.Parse(Player["rank"].ToString()) == 11) ? 1 : 0);
            Out.Add("cmsc", Player["skillscore"]);
            Out.Add("osaa", " "); // Overall small-arms accuracy, Filled Later
            Out.Add("kill", Player["kills"]);
            Out.Add("kila", Player["damageassists"]);
            Out.Add("deth", Player["deaths"]);
            Out.Add("suic", Player["suicides"]);
            Out.Add("bksk", Player["killstreak"]);
            Out.Add("wdsk", Player["deathstreak"]);
            Out.Add("tvcr", "0");    // Top Victim, Filled Later
            Out.Add("topr", "0");    // Top Oppenent, Filled Later
            Out.Add("klpm", Math.Round((60 * (float.Parse(Player["kills"].ToString()) / Time)), 2));    // Kills per minute
            Out.Add("dtpm", Math.Round((60 * (float.Parse(Player["deaths"].ToString()) / Time)), 2));   // Deaths per Minute
            Out.Add("ospm", Math.Round((60 * (float.Parse(Player["score"].ToString()) / Time)), 2));    // Score Per Minute
            Out.Add("klpr", Math.Round((float.Parse(Player["kills"].ToString()) / float.Parse(Player["rounds"].ToString())), 2));   // Kills Per Round
            Out.Add("dtpr", Math.Round((float.Parse(Player["deaths"].ToString()) / float.Parse(Player["rounds"].ToString())), 2));  // Deaths Per Round
            Out.Add("twsc", Player["teamscore"]);   // Teamwork Score
            Out.Add("cpcp", Player["captures"]);    // Capture Control Points
            Out.Add("cacp", Player["captureassists"]);    // Capture Assist Points
            Out.Add("dfcp", Player["defends"]);     // Capture Control Defend Points
            Out.Add("heal", Player["heals"]);       // Player Heals
            Out.Add("rviv", Player["revives"]);     // Player Revives
            Out.Add("rsup", Player["ammos"]);       // Player Resupplies
            Out.Add("rpar", Player["repairs"]);     // Player Repairs
            Out.Add("tgte", Player["targetassists"]);     // Times Targeted Enemy
            Out.Add("dkas", Player["driverassists"]);     // Kill assists as Database
            Out.Add("dsab", Player["driverspecials"]);    // Database special ability points
            Out.Add("cdsc", Player["cmdscore"]);    // Command Score
            Out.Add("rank", Player["rank"]);        // Player Rank
            Out.Add("kick", Player["kicked"]);      // Number of times Kicked from server the player has
            Out.Add("bbrs", Player["rndscore"]);    // Best Round Score
            Out.Add("tcdr", Player["cmdtime"]);     // Time As Commander
            Out.Add("ban", Player["banned"]);       // Times Banned
            Out.Add("lbtl", Player["lastonline"]);  // Player Last Battle
            Out.Add("vrk", "0");    // Vehicle Road Kills, Filled Later
            Out.Add("tsql", Player["sqltime"]);     // Time As Squad Leaders
            Out.Add("tsqm", Player["sqmtime"]);     // Time As Squad Members
            Out.Add("tlwf", Player["lwtime"]);      // Time As Lone Wolf
            Out.Add("mvks", "0");      // Top Victim Kills
            Out.Add("vmks", "0");      // Top Opponent Kills
            Out.Add("mvns", " ");      // Top Victim Name, Filled Later
            Out.Add("mvrs", " ");      // Top Victim Rank, Filled Later
            Out.Add("vmns", " ");      // Top Opponent Name, Filled Later
            Out.Add("vmrs", " ");      // Top Opponent Rank, Filled Later
            Out.Add("fkit", "");      // Favorite Kit, Filled Later
            Out.Add("fmap", "");      // Favorite Map, Filled Later
            Out.Add("fveh", "");      // Favorite Vehicle, Filled Later
            Out.Add("fwea", "");      // Favorite Weapon, Filled Later
            Out.Add("tnv", "0");      // NIGHT VISION GOGGLES Time - NOT USED
            Out.Add("tgm", "0");      // GAS MASK TIME - NOT USED

            // Proccess Weapons
            AddWeaponData();

            // Process Vehicles
            AddVehicleData();

            // Process Armies
            AddArmyData();

            // Process Kits
            AddKitData();

            // Get tactical data
            AddTacticalData();

            // Get Player Top Victim and Opponent
            GetPlayerTopVitcimAndOpp();

            // Get Favorite Map
            GetFavMap();

            // Do output
            Response.WriteHeaderDataPair(Out);
            Response.Send();
        }

        /// <summary>
        /// Fetches time info for player favorites
        /// </summary>
        private void DoTimeRequest()
        {
            int Kit = 0;
            int Vehicle = 0;
            int Weapon = 0;
            int Map = 0;
            string colName;

            // Get params
            if (Request.QueryString.ContainsKey("kit"))
                Int32.TryParse(Request.QueryString["kit"], out Kit);
            if (Request.QueryString.ContainsKey("vehicle"))
                Int32.TryParse(Request.QueryString["vehicle"], out Vehicle);
            if (Request.QueryString.ContainsKey("weapon"))
                Int32.TryParse(Request.QueryString["weapon"], out Weapon);
            if (Request.QueryString.ContainsKey("map"))
                Int32.TryParse(Request.QueryString["map"], out Map);

            // Check if the player exists
            Rows = Database.Query("SELECT name FROM player WHERE id=@P0", Pid);
            if (Rows.Count == 0)
            {
                Response.WriteResponseStart(false);
                Response.WriteHeaderLine("asof", "err");
                Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp(), "Player Doesnt Exist");
                Response.Send();
                return;
            }
            else
            {
                Response.WriteResponseStart();
                Response.WriteHeaderLine("asof");
                Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp());
            }

            // Prepare output
            Response.WriteHeaderLine("pid", "nick", "ktm-" + Kit.ToString(), "vtm-" + Vehicle.ToString(), "wtm-" + Weapon.ToString(), "mtm-" + Map.ToString());
            string Name = Rows[0]["name"].ToString().Trim();

            // Format weapon column name
            if(Weapon > 8)
            {
                switch(Weapon)
                {
                    default:
                        colName = "knifetime";
                        break;
                    case 10:
                        colName = "shockpadtime";
                        break;
                    case 11:
                        colName = "(c4time + claymoretime + atminetime)";
                        break;
                    case 12:
                        colName = "handgrenadetime";
                        break;
                }
            }
            else
                colName = "time" + Weapon;

            // Kit Time
            Rows = Database.Query(String.Format("SELECT time{0} AS time FROM kits WHERE id={1}", Kit, Pid));
            Kit = (Rows.Count == 0) ? 0 : Int32.Parse(Rows[0]["time"].ToString());

            // Vehicle Time
            Rows = Database.Query(String.Format("SELECT time{0} AS time FROM vehicles WHERE id={1}", Vehicle, Pid));
            Vehicle = (Rows.Count == 0) ? 0 : Int32.Parse(Rows[0]["time"].ToString());

            // Weapon Time
            Rows = Database.Query(String.Format("SELECT {0} AS time FROM weapons WHERE id={1}", colName, Pid));
            Weapon = (Rows.Count == 0) ? 0 : Int32.Parse(Rows[0]["time"].ToString());

            // Map Time
            Rows = Database.Query("SELECT time FROM maps WHERE id = @P0 AND mapid = @P1", Pid, Map);
            Map = (Rows.Count == 0) ? 0 : Int32.Parse(Rows[0]["time"].ToString());

            // Send Response
            Response.WriteDataLine(Pid, Name, Kit, Vehicle, Weapon, Map);
            Response.Send();
        }

        /// <summary>
        /// Fetches map info for player
        /// <remarks>
        ///     Originally this method was smaller and faster... BUT, good Ol'
        ///     BF2 wasnt having my new format... unfortunatly, if the headers
        ///     arent all grouped together, the mapinfo wont parse in the bf2
        ///     client, therefor this method is bigger and written differently
        ///     then it should be... just keep that in mind ;)
        /// </remarks>
        /// </summary>
        private void DoMapRequest()
        {
            bool Extended = Info.Contains("mbs-");
            int CustomMapId = 700;

            // Check if the player exists
            Rows = Database.Query("SELECT name FROM player WHERE id=@P0", Pid);
            if (Rows.Count == 0)
            {
                Response.WriteResponseStart(false);
                Response.WriteHeaderLine("asof", "err");
                Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp(), "Player Doesnt Exist");
                Response.Send();
                return;
            }
            else
            {
                Response.WriteResponseStart();
                Response.WriteHeaderLine("asof");
                Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp());
            }

            // Build individual headers, so they can group together in response
            Dictionary<int, string> Mtm = new Dictionary<int, string>();
            Dictionary<int, string> Mwn = new Dictionary<int, string>();
            Dictionary<int, string> Mls = new Dictionary<int, string>();
            Dictionary<int, string> Mbs = new Dictionary<int, string>();
            Dictionary<int, string> Mws = new Dictionary<int, string>();

            // Add player data
            List<string> Head = new List<string>();
            List<string> Body = new List<string>();
            Head.Add("pid");
            Head.Add("nick");
            Body.Add(Pid.ToString());
            Body.Add(Rows[0]["name"].ToString().Trim());

            // Begin headers
            for (int i = 0; i < 7; i++)
            {
                Mtm.Add(i, "0");
                Mwn.Add(i, "0");
                Mls.Add(i, "0");
                if (Extended)
                {
                    Mbs.Add(i, "0");
                    Mws.Add(i, "0");
                }
            }

            for (int i = 10; i < 13; i++)
            {
                Mtm.Add(i, "0");
                Mwn.Add(i, "0");
                Mls.Add(i, "0");
                if (Extended)
                {
                    Mbs.Add(i, "0");
                    Mws.Add(i, "0");
                }
            }

            for (int i = 100; i < 106; i++)
            {
                Mtm.Add(i, "0");
                Mwn.Add(i, "0");
                Mls.Add(i, "0");
                if (Extended)
                {
                    Mbs.Add(i, "0");
                    Mws.Add(i, "0");
                }
            }

            for (int i = 110; i < 130; i += 10)
            {
                Mtm.Add(i, "0");
                Mwn.Add(i, "0");
                Mls.Add(i, "0");
                if (Extended)
                {
                    Mbs.Add(i, "0");
                    Mws.Add(i, "0");
                }
            }

            for (int i = 200; i < 203; i++)
            {
                Mtm.Add(i, "0");
                Mwn.Add(i, "0");
                Mls.Add(i, "0");
                if (Extended)
                {
                    Mbs.Add(i, "0");
                    Mws.Add(i, "0");
                }
            }

            for (int i = 300; i < 308; i++)
            {
                Mtm.Add(i, "0");
                Mwn.Add(i, "0");
                Mls.Add(i, "0");
                if (Extended)
                {
                    Mbs.Add(i, "0");
                    Mws.Add(i, "0");
                }
            }

            for (int i = 601; i < 604; i++)
            {
                Mtm.Add(i, "0");
                Mwn.Add(i, "0");
                Mls.Add(i, "0");
                if (Extended)
                {
                    Mbs.Add(i, "0");
                    Mws.Add(i, "0");
                }
            }

            // Fetch all user map data
            string Where = (Info.Contains("cmap-")) 
                ? String.Format("WHERE id={0}", Pid)
                : String.Format("WHERE id={0} AND mapid < {1}", Pid, CustomMapId);
            foreach (Dictionary<string, object> Row in Database.QueryReader(String.Format("SELECT * FROM maps {0}", Where)))
            {
                int Id = Int32.Parse(Row["mapid"].ToString());
                if (Id > CustomMapId)
                {
                    Mtm.Add(Id, Row["time"].ToString());
                    Mwn.Add(Id, Row["win"].ToString());
                    Mls.Add(Id, Row["loss"].ToString());
                    if (Extended)
                    {
                        Mbs.Add(Id, Row["best"].ToString());
                        Mws.Add(Id, Row["worst"].ToString());
                    }
                }
                else
                {
                    Mtm[Id] = Row["time"].ToString();
                    Mwn[Id] = Row["win"].ToString();
                    Mls[Id] = Row["loss"].ToString();
                    if (Extended)
                    {
                        Mbs[Id] = Row["best"].ToString();
                        Mws[Id] = Row["worst"].ToString();
                    }
                }
            }

            // Send Response
            foreach (KeyValuePair<int, string> Item in Mtm)
            {
                Head.Add("mtm-" + Item.Key);
                Body.Add(Item.Value);
            }

            foreach (KeyValuePair<int, string> Item in Mwn)
            {
                Head.Add("mwn-" + Item.Key);
                Body.Add(Item.Value);
            }

            foreach (KeyValuePair<int, string> Item in Mls)
            {
                Head.Add("mls-" + Item.Key);
                Body.Add(Item.Value);
            }

            if (Extended)
            {
                foreach (KeyValuePair<int, string> Item in Mbs)
                {
                    Head.Add("mbs-" + Item.Key);
                    Body.Add(Item.Value);
                }

                foreach (KeyValuePair<int, string> Item in Mws)
                {
                    Head.Add("mws-" + Item.Key);
                    Body.Add(Item.Value);
                }
            }

            // Send Response
            Response.WriteHeaderLine(Head);
            Response.WriteDataLine(Body);
            Response.Send();
        }

        /// <summary>
        /// Produces the Server response
        /// </summary>
        private void DoServerRequest()
        {
            // Fetch Player
            Rows = Database.Query("SELECT * FROM player WHERE id=@P0", Pid);

            // If player doesnt exist then output default data
            if (Rows.Count == 0)
            {
                Response.WriteResponseStart(false);
                Response.WriteHeaderLine("asof", "err");
                Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp(), "Player Doesnt Exist");
                Response.Send();
                return;
            }
            else
            {
                Response.WriteResponseStart();
                Response.WriteHeaderLine("asof");
                Response.WriteDataLine(DateTime.UtcNow.ToUnixTimestamp());
            }

            // Add default player data
            Dictionary<string, object> Player = Rows[0];
            Out.Add("pid", Pid);
            Out.Add("name", Player["name"]);
            Out.Add("scor", Player["score"]);
            Out.Add("rank", Player["rank"]);
            Out.Add("dfcp", Player["defends"]);
            Out.Add("rpar", Player["repairs"]);
            Out.Add("heal", Player["heals"]);
            Out.Add("rsup", Player["ammos"]);
            Out.Add("dsab", Player["driverspecials"]);
            Out.Add("cdsc", Player["cmdscore"]);
            Out.Add("tcdr", Player["cmdtime"]);
            Out.Add("tsql", Player["sqltime"]);
            Out.Add("tsqm", Player["sqmtime"]);
            Out.Add("wins", Player["wins"]);
            Out.Add("loss", Player["losses"]);
            Out.Add("twsc", Player["teamscore"]);
            Out.Add("bksk", Player["killstreak"]);
            Out.Add("wdsk", Player["deathstreak"]);
            Out.Add("time", Player["time"]);
            Out.Add("kill", Player["kills"]);

            // Add Kit Times
            Rows = Database.Query("SELECT time0, time1, time2, time3, time4, time5, time6 FROM kits WHERE id=@P0", Pid);
            if (Rows.Count == 0)
            {
                for (int i = 0; i < 7; i++)
                    Out.Add("ktm-" + i, 0);
            }
            else
            {
                for (int i = 0; i < 7; i++)
                    Out.Add("ktm-" + i, Rows[0]["time" + i]);
            }

            // Add weapon data
            Rows = Database.Query("SELECT * FROM weapons WHERE id=@P0", Pid);
            if (Rows.Count == 0)
            {
                for (int i = 0; i < 14; i++)
                    Out.Add("wkl-" + i, 0);

                if (Info.Contains("de-"))
                {
                    Out.Add("de-6", 0);
                    Out.Add("de-7", 0);
                    Out.Add("de-8", 0);
                }
            }
            else
            {
                for (int i = 0; i < 9; i++)
                    Out.Add("wkl-" + i, Rows[0]["kills" + i]);

                Out.Add("wkl-9", Rows[0]["knifekills"]);
                Out.Add("wkl-10", Rows[0]["shockpadkills"]);
                Out.Add("wkl-11", 
                    int.Parse(Rows[0]["c4kills"].ToString()) +
                    int.Parse(Rows[0]["claymorekills"].ToString()) +
                    int.Parse(Rows[0]["atminekills"].ToString())
                );
                Out.Add("wkl-12", Rows[0]["handgrenadekills"]);
                Out.Add("wkl-13", 0);

                // Special Forces
                if (Info.Contains("de-"))
                {
                    Out.Add("de-6", Rows[0]["tacticaldeployed"]);
                    Out.Add("de-7", Rows[0]["grapplinghookdeployed"]);
                    Out.Add("de-8", Rows[0]["ziplinedeployed"]);
                }
            }

            // Add Vehicle Data
            Rows = Database.Query("SELECT * FROM vehicles WHERE id=@P0", Pid);
            if (Rows.Count == 0)
            {
                // Time
                for (int i = 0; i < 7; i++)
                    Out.Add("vtm-" + i, 0);

                // Kills
                for (int i = 0; i < 7; i++)
                    Out.Add("vkl-" + i, 0);
            }
            else
            {
                // Time
                for (int i = 0; i < 7; i++)
                    Out.Add("vtm-" + i, Rows[0]["time" + i]);

                // Kills
                for (int i = 0; i < 7; i++)
                    Out.Add("vkl-" + i, Rows[0]["kills" + i]);
            }

            // Add army data (Army medals are processed in the backend, but this MAY change)
            Rows = Database.Query("SELECT * FROM army WHERE id=@P0", Pid);
            if (Rows.Count == 0)
            {
                // Time
                for (int i = 0; i < 12; i++)
                    Out.Add("atm-" + i, 0);

                // Best Round Score
                for (int i = 0; i < 12; i++)
                    Out.Add("abr-" + i, 0);

                // Wins
                for (int i = 0; i < 12; i++)
                    Out.Add("awn-" + i, 0); 
            }
            else
            {
                // Time
                for (int i = 0; i < 12; i++)
                    Out.Add("atm-" + i, Rows[0]["time" + i]);

                // Best Round Score
                for (int i = 0; i < 12; i++)
                    Out.Add("abr-" + i, Rows[0]["best" + i]);

                // Wins
                for (int i = 0; i < 12; i++)
                    Out.Add("awn-" + i, Rows[0]["win" + i]); 
            }

            // Send Response
            Response.WriteHeaderDataPair(Out);
            Response.Send();
        }

        /// <summary>
        /// Adds Weapon Data to the Response
        /// </summary>
        private void AddWeaponData()
        {
            int time;
            int Fav = 0;
            int FavTime = 0;
            double tempAcc = 0;
            double Acc = 0;

            Rows = Database.Query("SELECT * FROM weapons WHERE id=@P0", Pid);
            if (Rows.Count == 0)
            {
                // Weapon Times
                for (int i = 0; i < 14; i++)
                    Out.Add("wtm-" + i, "0");

                // Weapon kills
                for (int i = 0; i < 14; i++)
                    Out.Add("wkl-" + i, "0");

                // Weapon Deaths
                for (int i = 0; i < 14; i++)
                    Out.Add("wdt-" + i, "0");

                // Weapon Accuracy
                for (int i = 0; i < 14; i++)
                    Out.Add("wac-" + i, "0");

                // Weapon kill/Death Ratio
                for (int i = 0; i < 14; i++)
                    Out.Add("wkd-" + i, "0");
            }
            else
            {
                // Weapon Times
                for (int i = 0; i < 9; i++)
                {
                    time = Int32.Parse(Rows[0]["time" + i].ToString());
                    if (time > FavTime)
                    {
                        Fav = i;
                        FavTime = time;
                    }
                    Out.Add("wtm-" + i, time);
                }

                // Knife
                time = Int32.Parse(Rows[0]["knifetime"].ToString());
                if (time > FavTime)
                {
                    Fav = 9;
                    FavTime = time;
                }
                Out.Add("wtm-9", time);

                // Shock Pads
                time = Int32.Parse(Rows[0]["shockpadtime"].ToString());
                if (time > FavTime)
                {
                    Fav = 10;
                    FavTime = time;
                }
                Out.Add("wtm-10", time);

                // Explosives
                time = (
                    Int32.Parse(Rows[0]["c4time"].ToString()) +
                    Int32.Parse(Rows[0]["claymoretime"].ToString()) +
                    Int32.Parse(Rows[0]["atminetime"].ToString())
                    );
                if (time > FavTime)
                {
                    Fav = 11;
                    FavTime = time;
                }
                Out.Add("wtm-11", time);

                // Hand grenade
                time = Int32.Parse(Rows[0]["handgrenadetime"].ToString());
                if (time > FavTime)
                {
                    Fav = 12;
                    FavTime = time;
                }
                Out.Add("wtm-12", time);

                Out.Add("wtm-13", "0");

                // Weapon kills
                for (int i = 0; i < 9; i++)
                    Out.Add("wkl-" + i, Rows[0]["kills" + i]);
                Out.Add("wkl-9", Rows[0]["knifekills"]);
                Out.Add("wkl-10", Rows[0]["shockpadkills"]);
                Out.Add("wkl-11", (
                    Int32.Parse(Rows[0]["c4kills"].ToString()) +
                    Int32.Parse(Rows[0]["claymorekills"].ToString()) +
                    Int32.Parse(Rows[0]["atminekills"].ToString())
                    )
                );
                Out.Add("wkl-12", Rows[0]["handgrenadekills"]);
                Out.Add("wkl-13", "0");

                // Weapon Deaths
                for (int i = 0; i < 9; i++)
                    Out.Add("wdt-" + i, Rows[0]["deaths" + i]);
                Out.Add("wdt-9", Rows[0]["knifedeaths"]);
                Out.Add("wdt-10", Rows[0]["shockpaddeaths"]);
                Out.Add("wdt-11", (
                    Int32.Parse(Rows[0]["c4deaths"].ToString()) +
                    Int32.Parse(Rows[0]["claymoredeaths"].ToString()) +
                    Int32.Parse(Rows[0]["atminedeaths"].ToString())
                    )
                );
                Out.Add("wdt-12", Rows[0]["handgrenadedeaths"]);
                Out.Add("wdt-13", "0");

                // Weapon Accuracy
                for (int i = 0; i < 9; i++)
                {
                    tempAcc = (Rows[0]["fired" + i].ToString() != "0")
                            ? (100 * (float.Parse(Rows[0]["hit" + i].ToString()) / float.Parse(Rows[0]["fired" + i].ToString())))
                            : 0;
                    Acc += tempAcc;
                    Out.Add("wac-" + i, Math.Round(tempAcc, 0));
                }

                tempAcc = (Rows[0]["knifefired"].ToString() != "0")
                            ? (100 * (float.Parse(Rows[0]["knifehit"].ToString()) / float.Parse(Rows[0]["knifefired"].ToString())))
                            : 0;
                Acc += tempAcc;
                Out.Add("wac-9", Math.Round(tempAcc, 0));

                tempAcc = (Rows[0]["shockpadfired"].ToString() != "0")
                            ? (100 * (float.Parse(Rows[0]["shockpadhit"].ToString()) / float.Parse(Rows[0]["shockpadfired"].ToString())))
                            : 0;
                Acc += tempAcc;
                Out.Add("wac-10", Math.Round(tempAcc));

                int fired = (
                    Int32.Parse(Rows[0]["c4fired"].ToString()) +
                    Int32.Parse(Rows[0]["claymorefired"].ToString()) +
                    Int32.Parse(Rows[0]["atminefired"].ToString())
                );
                if (fired != 0)
                {
                    int hits = (
                        Int32.Parse(Rows[0]["c4hit"].ToString()) +
                        Int32.Parse(Rows[0]["claymorehit"].ToString()) +
                        Int32.Parse(Rows[0]["atminehit"].ToString())
                    );
                    tempAcc = (100 * (float.Parse(hits.ToString()) / float.Parse(fired.ToString())));
                    Acc += tempAcc;
                    Out.Add("wac-11", Math.Round(tempAcc, 0));
                }
                else
                    Out.Add("wac-11", "0");


                tempAcc = (Rows[0]["handgrenadefired"].ToString() != "0")
                            ? (100 * (float.Parse(Rows[0]["handgrenadehit"].ToString()) / float.Parse(Rows[0]["handgrenadefired"].ToString())))
                            : 0;
                Acc += tempAcc;
                Out.Add("wac-12", Math.Round(tempAcc));
                Out.Add("wac-13", "0");

                // Add Overall Small Arms Acc.
                tempAcc = Math.Round((Acc / 12d), 2);
                Out["osaa"] = tempAcc;

                // Weapon kill/Death Ratio
                int kills;
                int deaths;
                int den;

                for (int i = 0; i < 9; i++)
                {
                    kills = Int32.Parse(Rows[0]["kills" + i].ToString());
                    deaths = Int32.Parse(Rows[0]["deaths" + i].ToString());
                    den = GetDenominator(kills, deaths);
                    if (kills == 0 && deaths == 0)
                        Out.Add("wkd-" + i, "0:0");
                    else if (deaths != 0)
                        Out.Add("wkd-" + i, kills / den + ":" + deaths / den);
                    else
                        Out.Add("wkd-" + i, kills + ":0");
                }

                // Knife
                kills = Int32.Parse(Rows[0]["knifekills"].ToString());
                deaths = Int32.Parse(Rows[0]["knifedeaths"].ToString());
                den = GetDenominator(kills, deaths);
                if (kills == 0 && deaths == 0)
                    Out.Add("wkd-9", "0:0");
                else if (deaths != 0)
                    Out.Add("wkd-9", kills / den + ":" + deaths / den);
                else
                    Out.Add("wkd-9", kills + ":0");

                // shockpad
                kills = Int32.Parse(Rows[0]["shockpadkills"].ToString());
                deaths = Int32.Parse(Rows[0]["shockpaddeaths"].ToString());
                den = GetDenominator(kills, deaths);
                if (kills == 0 && deaths == 0)
                    Out.Add("wkd-10", "0:0");
                else if (deaths != 0)
                    Out.Add("wkd-10", kills / den + ":" + deaths / den);
                else
                    Out.Add("wkd-10", kills + ":0");

                // explosives
                kills = (
                    Int32.Parse(Rows[0]["c4kills"].ToString()) + 
                    Int32.Parse(Rows[0]["claymorekills"].ToString()) + 
                    Int32.Parse(Rows[0]["atminekills"].ToString())
                );
                deaths = (
                    Int32.Parse(Rows[0]["c4deaths"].ToString()) + 
                    Int32.Parse(Rows[0]["claymoredeaths"].ToString()) + 
                    Int32.Parse(Rows[0]["atminedeaths"].ToString())
                );
                den = GetDenominator(kills, deaths);
                if (kills == 0 && deaths == 0)
                    Out.Add("wkd-11", "0:0");
                else if (deaths != 0)
                    Out.Add("wkd-11", kills / den + ":" + deaths / den);
                else
                    Out.Add("wkd-11", kills + ":0");

                // hand Grenade
                kills = Int32.Parse(Rows[0]["handgrenadekills"].ToString());
                deaths = Int32.Parse(Rows[0]["handgrenadedeaths"].ToString());
                den = GetDenominator(kills, deaths);
                if (kills == 0 && deaths == 0)
                    Out.Add("wkd-12", "0:0");
                else if (deaths != 0)
                    Out.Add("wkd-12", kills / den + ":" + deaths / den);
                else
                    Out.Add("wkd-12", kills + ":0");
                Out.Add("wkd-13", "0:0");
            }

            Out["fwea"] = Fav;
        }

        /// <summary>
        /// Adds tactical data to the Response
        /// </summary>
        private void AddTacticalData()
        {
            Rows = Database.Query("SELECT tacticaldeployed, grapplinghookdeployed, ziplinedeployed FROM weapons WHERE id=@P0", Pid);
            if (Rows.Count == 0)
            {
                // Weapon Times
                for (int i = 6; i < 9; i++)
                    Out.Add("de-" + i, "0");
            }
            else
            {
                // Add SF Data
                Out.Add("de-6", Rows[0]["tacticaldeployed"]);
                Out.Add("de-7", Rows[0]["grapplinghookdeployed"]);
                Out.Add("de-8", Rows[0]["ziplinedeployed"]);
            }
        }

        /// <summary>
        /// Adds Vehicle Data to the Response
        /// </summary>
        private void AddVehicleData()
        {
            int TotalRoadKills = 0;
            int Fav = 0;
            int FavTime = 0;

            Rows = Database.Query("SELECT * FROM vehicles WHERE id=@P0", Pid);
            if (Rows.Count == 0)
            {
                // Vehicle Times
                for (int i = 0; i < 7; i++)
                    Out.Add("vtm-" + i, "0");

                // Vehicle kills
                for (int i = 0; i < 7; i++)
                    Out.Add("vkl-" + i, "0");

                // Vehicle Deaths
                for (int i = 0; i < 7; i++)
                    Out.Add("vdt-" + i, "0");

                // Vehicle Kill Death Ratio
                for (int i = 0; i < 7; i++)
                    Out.Add("vkd-" + i, "0");

                // Vehicle Roadkills
                for (int i = 0; i < 7; i++)
                    Out.Add("vkr-" + i, "0");
            }
            else
            {
                // Vehicle Times
                for (int i = 0; i < 7; i++)
                {
                    int time = Int32.Parse(Rows[0]["time" + i].ToString());
                    if (time > FavTime)
                    {
                        Fav = i;
                        FavTime = time;
                    }
                    Out.Add("vtm-" + i, time);
                }

                // Vehicle kills
                for (int i = 0; i < 7; i++)
                    Out.Add("vkl-" + i, Rows[0]["kills" + i]);

                // Vehicle Deaths
                for (int i = 0; i < 7; i++)
                    Out.Add("vdt-" + i, Rows[0]["deaths" + i]);

                // Vehicle Kill Death Ratio
                for (int i = 0; i < 7; i++)
                {
                    int kills = Int32.Parse(Rows[0]["kills" + i].ToString());
                    int deaths = Int32.Parse(Rows[0]["deaths" + i].ToString());
                    int den = GetDenominator(kills, deaths);
                    if (kills == 0 && deaths == 0)
                        Out.Add("vkd-" + i, "0");
                    else if (deaths != 0)
                        Out.Add("vkd-" + i, kills / den + ":" + deaths / den);
                    else
                        Out.Add("vkd-" + i, kills + ":0");
                }

                // Vehicle Roadkills
                for (int i = 0; i < 7; i++)
                {
                    TotalRoadKills += int.Parse(Rows[0]["rk" + i].ToString());
                    Out.Add("vkr-" + i, Rows[0]["rk" + i]);
                } 
            }

            // Add total road kills
            Out["vrk"] = TotalRoadKills;
            Out["fveh"] = Fav;
        }

        /// <summary>
        /// Adds Kit Data to the Response
        /// </summary>
        private void AddKitData()
        {
            int Fav = 0;
            int FavTime = 0;

            Rows = Database.Query("SELECT * FROM kits WHERE id=@P0", Pid);
            if (Rows.Count == 0)
            {
                // Kit Times
                for (int i = 0; i < 7; i++)
                    Out.Add("ktm-" + i, "0");

                // Kit kills
                for (int i = 0; i < 7; i++)
                    Out.Add("kkl-" + i, "0");

                // Kit Deaths
                for (int i = 0; i < 7; i++)
                    Out.Add("kdt-" + i, "0");

                // Kit Kill Death Ratio
                for (int i = 0; i < 7; i++)
                    Out.Add("kkd-" + i, "0:0");
            }
            else
            {
                // Kit Times
                for (int i = 0; i < 7; i++)
                {
                    int time = Int32.Parse(Rows[0]["time" + i].ToString());
                    if (time > FavTime)
                    {
                        Fav = i;
                        FavTime = time;
                    }
                    Out.Add("ktm-" + i, time);
                }

                // Kit kills
                for (int i = 0; i < 7; i++)
                    Out.Add("kkl-" + i, Rows[0]["kills" + i]);

                // Kit Deaths
                for (int i = 0; i < 7; i++)
                    Out.Add("kdt-" + i, Rows[0]["deaths" + i]);

                // Kit Kill Death Ratio
                for (int i = 0; i < 7; i++)
                {
                    int kills = Int32.Parse(Rows[0]["kills" + i].ToString());
                    int deaths = Int32.Parse(Rows[0]["deaths" + i].ToString());
                    int den = GetDenominator(kills, deaths);
                    if (kills == 0 && deaths == 0)
                        Out.Add("kkd-" + i, "0:0");
                    else if (deaths != 0)
                        Out.Add("kkd-" + i, kills / den + ":" + deaths / den);
                    else
                        Out.Add("kkd-" + i, kills + ":0");
                }
            }

            Out["fkit"] = Fav;
        }

        /// <summary>
        /// Adds Army Data to the Response
        /// </summary>
        private void AddArmyData()
        {
            int Limit = (Info.Contains("mods-")) ? 14 : 10;
            Rows = Database.Query("SELECT * FROM army WHERE id=@P0", Pid);
            if (Rows.Count == 0)
            {
                // Army Times
                for (int i = 0; i < Limit; i++)
                    Out.Add("atm-" + i, "0");

                // Army Wins
                for (int i = 0; i < Limit; i++)
                    Out.Add("awn-" + i, "0");

                // Army Losses
                for (int i = 0; i < Limit; i++)
                    Out.Add("alo-" + i, "0");

                // Army Best Rounds
                for (int i = 0; i < Limit; i++)
                    Out.Add("abr-" + i, "0");
            }
            else
            {
                // Army Times
                for (int i = 0; i < Limit; i++)
                    Out.Add("atm-" + i, Rows[0]["time" + i]);

                // Army Wins
                for (int i = 0; i < Limit; i++)
                    Out.Add("awn-" + i, Rows[0]["win" + i]);

                // Army Losses
                for (int i = 0; i < Limit; i++)
                    Out.Add("alo-" + i, Rows[0]["loss" + i]);

                // Army Best Rounds
                for (int i = 0; i < Limit; i++)
                    Out.Add("abr-" + i, Rows[0]["best" + i]);
            }
        }

        /// <summary>
        /// Fills the Top Opponent and Victim Variables
        /// </summary>
        private void GetPlayerTopVitcimAndOpp()
        {
            // Create a new DB Row
            List<Dictionary<string, object>> Row;

            // Victim
            Rows = Database.Query("SELECT victim, count FROM kills WHERE attacker=@P0 ORDER BY count DESC LIMIT 1", Pid);
            if (Rows.Count != 0)
            {
                // Fetch Victim
                Row = Database.Query("SELECT name, rank FROM player WHERE id=@P0", Rows[0]["victim"]);
                if (Row.Count != 0)
                {
                    Out["tvcr"] = Rows[0]["victim"];
                    Out["mvks"] = Rows[0]["count"];
                    Out["mvns"] = Row[0]["name"];
                    Out["mvrs"] = Row[0]["rank"];
                }
            }

            // Opponent
            Rows = Database.Query("SELECT attacker, count FROM kills WHERE victim=@P0 ORDER BY count DESC LIMIT 1", Pid);
            if (Rows.Count != 0)
            {
                // Fetch Opponent
                Row = Database.Query("SELECT name, rank FROM player WHERE id=@P0", Rows[0]["attacker"]);
                if (Row.Count != 0)
                {
                    Out["topr"] = Rows[0]["attacker"];
                    Out["vmks"] = Rows[0]["count"];
                    Out["vmns"] = Row[0]["name"];
                    Out["vmrs"] = Row[0]["rank"];
                }
            }
        }

        /// <summary>
        /// Adds the players favorite map data to the response
        /// </summary>
        private void GetFavMap()
        {
            Rows = Database.Query("SELECT mapid FROM maps WHERE id=@P0 AND mapid < 700 ORDER BY time DESC LIMIT 1", Pid);
            if (Rows.Count == 0)
                Out["fmap"] = 0;
            else
                Out["fmap"] = Rows[0]["mapid"];
        }

        /// <summary>
        /// Returns the common denominator of 2 variables
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public int GetDenominator(int a, int b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a == 0 ? b : a;
        }
    }
}
