using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace BF2Statistics.Web.Bf2Stats
{
    class StatsData
    {
        #region Static Variables

        /// <summary>
        /// ArmyId => Army Name. Data generated from ArmyData.xml
        /// </summary>
        public static Dictionary<int, string> Armies;

        /// <summary>
        /// MapId => Map Name. Data generated from MapData.xml
        /// </summary>
        public static Dictionary<int, string> Maps;

        /// <summary>
        /// ModName => List of MapIds. Data generated from MapData.xml
        /// </summary>
        public static Dictionary<string, List<int>> ModMapIds;

        /// <summary>
        /// TheaterName => List of MapIds. Data generated from TeaterData.xml
        /// </summary>
        public static Dictionary<string, int[]> TheatreMapIds;

        public static Dictionary<string, string> Badges = new Dictionary<string, string>()
        {
            {"1031105", "Engineer Combat Badge"},
            {"1031109", "Sniper Combat Badge"},
            {"1031113", "Medic Combat Badge"},
            {"1031115", "Spec-Ops Combat Badge"},
            {"1031119", "Assault Combat Badge"},
            {"1031120", "Anti-Tank Combat Badge"},
            {"1031121", "Support Combat Badge"},
            {"1031406", "Knife Combat Badge"},
            {"1031619", "Pistol Combat Badge"},
            {"1032415", "Explosives Ordinance Badge"},
            {"1190304", "Command Badge"},
            {"1190507", "Engineer Badge"},
            {"1190601", "First Aid Badge"},
            {"1191819", "Resupply Badge"},
            {"1031923", "Ground Defense Badge"},
            {"1220104", "Air Defense Badge"},
            {"1220118", "Armour Badge"},
            {"1220122", "Aviator Badge"},
            {"1220803", "Helicopter Badge"},
            {"1222016", "Transport Badge"}
        };

        public static Dictionary<string, string> SfBadges = new Dictionary<string, string>()
        {
            {"1261105", "Engineer Specialist Badge"},
            {"1261109", "Sniper Specialist Badge"},
            {"1261113", "Medic Specialist Badge"},
            {"1261115", "Spec-Ops Specialist Badge"},
            {"1261119", "Assault Specialist Badge"},
            {"1261120", "Anti-Tank Specialist Badge"},
            {"1261121", "Support Specialist Badge"},
            {"1260602", "Tactical Support Weaponry Badge"},
            {"1260708", "Grappling Hook Specialist Badge"},
            {"1262612", "Zip Line Specialist Badge"}
        };

        public static Dictionary<string, string> Medals = new Dictionary<string, string>()
        {
            {"2051902", "Bronze Star"},
            {"2051919", "Silver Star"},
            {"2051907", "Gold Star"},
            {"2020419", "Distinguished Service Medal"},
            {"2020903", "Combat Infantry Medal"},
            {"2020913", "Marksman Infantry Medal"},
            {"2020919", "Sharpshooter Infantry Medal"},
            {"2021322", "Medal of Valour"},
            {"2021403", "Navy Cross"},
            {"2020719", "Golden Scimitar"},
            {"2021613", "People's Medallion"},
            {"2190303", "Combat Action Medal"},
            {"2190308", "Helecopter Combat Medal"},
            {"2190309", "Air Combat Medal"},
            {"2190318", "Armour Combat Medal"},
            {"2190703", "Good Conduct Medal"},
            {"2191319", "Meritorious Service Medal"},
            {"2191608", "Purple Heart"},
            {"2270521", "European Union Special Service Medal"}
        };

        public static Dictionary<string, string> SfMedals = new Dictionary<string, string>()
        {
            {"2261913", "Navy Seal Special Service Medal"},
            {"2261919", "SAS Special Special Medal"},
            {"2261613", "SPETZ Special Service Medal"},
            {"2261303", "MECSF Special Service Medal"},
            {"2261802", "Rebel Special Service Medal"},
            {"2260914", "Insurgent Special Service Medal"}
        };

        public static Dictionary<string, string> Ribbons = new Dictionary<string, string>()
        {
            {"3040109", "Air Defense Ribbon"},
            {"3040718", "Ground Defense Ribbon"},
            {"3150914", "Infantry Officer Ribbon"},
            {"3151920", "Staff Officer Ribbon"},
            {"3190105", "Aerial Service Ribbon"},
            {"3190118", "Armoured Service Ribbon"},
            {"3190318", "Crew Service Ribbon"},
            {"3190409", "Distinguished Service Ribbon"},
            {"3190605", "Far East Service Ribbon"},
            {"3191305", "Middle East Service Ribbon"},
            {"3190803", "Helicopter Service Ribbon"},
            {"3211305", "Meritorious Unit Ribbon"},
            {"3212201", "Valorous Unit Ribbon"},
            {"3240102", "Airborne Ribbon"},
            {"3240301", "Combat Action Ribbon"},
            {"3240703", "Good Conduct Ribbon"},
            {"3241213", "Legion of Merit Ribbon"},
            {"3242303", "War College Ribbon"},
            {"3271401", "North America Service Ribbon"}
        };

        public static Dictionary<string, string> SfRibbons = new Dictionary<string, string>()
        {
            {"3261919", "U.S. Navy Seals Service Ribbon"},
            {"3261901", "British SAS Service Ribbon"},
            {"3261819", "Russian Spetsnaz Service Ribbon"},
            {"3261319", "MEC Special Forces Service Ribbon"},
            {"3261805", "Rebel Service Ribbon"},
            {"3260914", "Insurgent Forces Service Ribbon"},
            {"3260318", "Crew Specialist Ribbon"},
            {"3260118", "Armored Transport Specialist Ribbon"},
            {"3260803", "Helo Specialist Ribbon"}
        };

        public static Dictionary<string, string> Unlocks = new Dictionary<string, string>()
        {
            {"22", "G3"},
            {"33", "Jackhammer-Mk3A1"},
            {"44", "L85A1"},
            {"55", "G36C"},
            {"66", "PKM"},
            {"77", "M95"},
            {"11", "DAO-12"},
            {"88", "F2000"},
            {"99", "MP-7"},
            {"111", "G36E"},
            {"222", "SCAR-L"},
            {"333", "MG36"},
            {"555", "L96A1"},
            {"444", "P90"}
        };

        protected static string[] Ranks = new string[] 
        {
            "Private",
            "Private First Class",
            "Lance Corporal",
            "Corporal",
            "Sergeant",
            "Staff Sergeant",
            "Gunnery Sergeant",
            "Master Sergeant",
            "First Sergeant",
            "Master Gunnery Sergeant",
            "Sergeant Major",
            "Sergeant Major of the Corps",
            "2nd Lieutenant",
            "1st Lieutenant",
            "Captain",
            "Major",
            "Lieutenant Colonel",
            "Colonel",
            "Brigadier General",
            "Major General",
            "Lieutenant General",
            "General"
        };

        protected static string[] Vehicles = new string[] 
        {
            "Armor",
            "Aviator",
            "Air Defense",
            "Helicopter",
            "Transport",
            "Artillery",
            "Ground Defense"
        };

        protected static string[] Kits = new string[] 
        {
            "Anti-tank",
            "Assault",
            "Engineer",
            "Medic",
            "Special-Ops",
            "Support",
            "Sniper"
        };

        protected static string[] Weapons = new string[]
        {
            "Assault Rifles",
	        "Grenade Launcher Attachment",
	        "Carbines",
	        "Light Machine Guns",
	        "Sniper Rifles",
	        "Pistols",
	        "AT/AA",
	        "Submachine Guns",
	        "Shotguns",
            "Knife",
            "C4", 
            "Claymore",
            "Hand Grenade",
            "Shock Paddles",
            "AT Mine",
            "Tactical (Flash, Smoke)",
            "Grappling Hook",
            "Zip Line"
        };

        protected static string[] Weapons2 = new string[]
        {
            "Defibrillator",
            "Explosives (C4, Claymore, AT Mine)",
            "Hand Grenade"
        };

        #endregion Static Variables

        /// <summary>
        /// Loads the XML files from the "Web/Bf2Stats" folder.
        /// These XML files are used for stats processing
        /// </summary>
        public static void Load()
        {
            try
            {
                // Reset all incase of file changes
                Armies = new Dictionary<int, string>();
                Maps = new Dictionary<int, string>();
                ModMapIds = new Dictionary<string, List<int>>()
                {
                    { "bf", new List<int>() },
                    { "sf", new List<int>() },
                    { "ef", new List<int>() },
                    { "af", new List<int>() }
                };
                TheatreMapIds = new Dictionary<string, int[]>();

                // Enumerate through each .list file in the data directory
                string DataDir = Path.Combine(Program.RootPath, "Web", "Bf2Stats");
                XmlDocument Doc = new XmlDocument();

                // Load Army's
                Doc.Load(Path.Combine(DataDir, "ArmyData.xml"));
                foreach (XmlNode Node in Doc.GetElementsByTagName("army"))
                {
                    Armies.Add(Int32.Parse(Node.Attributes["id"].Value), Node.InnerText);
                }

                // Load Maps
                Doc.Load(Path.Combine(DataDir, "MapData.xml"));
                foreach (XmlNode Node in Doc.GetElementsByTagName("map"))
                {
                    int mid = Int32.Parse(Node.Attributes["id"].Value);
                    string mod = Node.Attributes["mod"].Value;
                    Maps.Add(mid, Node.InnerText);

                    // Add map to mod map ids if mod is not empty
                    if (!String.IsNullOrWhiteSpace(mod) && ModMapIds.ContainsKey(mod))
                        ModMapIds[mod].Add(mid);
                }

                // Load Theaters
                Doc.Load(Path.Combine(DataDir, "TheaterData.xml"));
                foreach (XmlNode Node in Doc.GetElementsByTagName("theater"))
                {
                    string name = Node.Attributes["name"].Value;
                    string[] arr = Node.Attributes["maps"].Value.Split(',');
                    TheatreMapIds.Add(name, Array.ConvertAll(arr, Int32.Parse));
                }

                // Load Rank Data
                Rank[] Ranks = new Rank[22];
                int i = 0;
                Doc.Load(Path.Combine(DataDir, "RankData.xml"));
                foreach (XmlNode Node in Doc.GetElementsByTagName("rank"))
                {
                    Dictionary<string, int> Awards = new Dictionary<string, int>();
                    XmlNode AwardsNode = Node.SelectSingleNode("reqAwards");
                    if (AwardsNode != null && AwardsNode.HasChildNodes)
                    {
                        foreach (XmlNode E in AwardsNode.ChildNodes)
                        {
                            Awards.Add(E.Attributes["id"].Value, Int32.Parse(E.Attributes["level"].Value));
                        }
                    }
                    string[] arr = Node.SelectSingleNode("reqRank").InnerText.Split(',');

                    Ranks[i] = new Rank
                    {
                        Id = i,
                        MinPoints = Int32.Parse(Node.SelectSingleNode("reqPoints").InnerText),
                        ReqRank = Array.ConvertAll(arr, Int32.Parse),
                        ReqAwards = Awards
                    };
                    i++;
                }

                RankCalculator.SetRankData(Ranks);
            }
            catch(Exception e)
            {
                ExceptionHandler.GenerateExceptionLog(e);
                throw;
            }
        }

        /// <summary>
        /// Returns the rank title based on the given rank id
        /// </summary>
        /// <param name="RankId"></param>
        /// <returns></returns>
        public static string GetRankName(int RankId)
        {
            if (RankId > 21)
                return "Unknown";

            return Ranks[RankId];
        }

        /// <summary>
        /// Returns the army title based on the given raarmy id
        /// </summary>
        /// <param name="ArmyId"></param>
        /// <returns></returns>
        public static string GetArmyName(int ArmyId)
        {
            if (ArmyId > Armies.Count)
                return "Unknown";

            return Armies[ArmyId];
        }

        /// <summary>
        /// Returns the map title based on the given map id
        /// </summary>
        /// <param name="MapId"></param>
        /// <returns></returns>
        public static string GetMapName(int MapId)
        {
            if (!Maps.ContainsKey(MapId))
                return "Unknown";

            return Maps[MapId];
        }

        /// <summary>
        /// Returns the vehicle title based on the given vehicle id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static string GetVehicleName(int Id)
        {
            if (Id > Vehicles.Length)
                return "Unknown";

            return Vehicles[Id];
        }

        /// <summary>
        /// Returns the kit title based on the given kit id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static string GetKitName(int Id)
        {
            if (Id > Kits.Length)
                return "Unknown";

            return Kits[Id];
        }

        /// <summary>
        /// Returns the weapon title based on the given weapon id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static string GetWeaponName(int Id)
        {
            if (Id > Weapons.Length)
                return "Unknown";

            return Weapons[Id];
        }

        public static string GetWeaponName2(int Id)
        {
            if (Id > Weapons2.Length)
                return "Unknown";

            return Weapons2[Id];
        }
    }
}
