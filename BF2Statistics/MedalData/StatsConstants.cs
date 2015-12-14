using System;
using System.Collections.Generic;

namespace BF2Statistics.MedalData
{
    /// <summary>
    /// The data class holds all the different python variables and constants 
    /// related to statistics and awards
    /// </summary>
    public static class StatsConstants
    {
        /// <summary>
        /// Provides more human readable names based for the bf2.stats.player scores
        /// </summary>
        public static Dictionary<string, string> PythonPlayerVars = new Dictionary<string, string>()
        {
            {"driverSpecials", "Driver Specials"},
            {"driverAssists", "Driver Assists"},
            {"score", "Score"},
            {"skillScore", "Skill Score"},
            {"kills", "Kills"},
            {"deaths", "Deaths"},
            {"suicides", "Suicides"},
            {"heals", "Heals"},
            {"repairs", "Repair Points"},
            {"ammos", "Resupply Points"},
            {"revives", "Revives"},
            {"cmdScore", "Command Score"},
            {"TKs", "Team kills"},
            {"teamDamages", "Team Damages"},
            {"teamVehicleDamages", "Team Vehicle Damages"},
            {"rplScore", "Team Points"},
            {"timeAsCmd", "Time as Commander"},
            {"timePlayed", "Time in Round"},
            {"timeInSquad", "Time in Squad"},
            {"timeAsSql", "Time as Squad Leader"},
            {"cpCaptures", "Flag Captures"},
            {"cpAssists", "Flag Caputre Assists"},
            {"cpDefends", "Flag Defends"},
            {"cpNeutralizes", "Flag Neutralizes"},
            {"cpNeutralizeAssists", "Flag Neutralize Assists"}
        };

        /// <summary>
        /// Global Strings
        /// <remarks>http://bf2tech.org/index.php/Getplayerinfo_columns</remarks>
        /// </summary>
        public static Dictionary<string, string> PythonGlobalVars = new Dictionary<string, string>()
        {
            {"scor", "Score"},
            {"kill", "Kills"},
            //{"kila", "Kill Assists"},
            //{"deth", "Deaths"},
            //{"suic", "Suicides"},
            {"time", "Time"},
            {"bksk", "Best Kill Streak"},
            {"wdsk", "Worse Death Streak"},
            //{"tkil", "Team Kills"},
            //{"tdmg", "Team Damage"},
            //{"tvdm", "Team Vehicle Damage"},
            {"heal", "Heals"},
            //{"rviv", "Revivies"},
            {"rpar", "Repairs"},
            {"rsup", "Resupplies"},
            {"cdsc", "Command Score"},
            {"dsab", "Driver Special Ability Points"},
            {"tsql", "Time as Squad leader"},
            {"tsqm", "Time as Squad Member"},
            {"tcdr", "Time as Commander"},
            {"wins", "Wins"},
            {"loss", "Losses"},
            {"dfcp", "Flag Defends"},
            //{"cacp", "Capture Assits"},
            {"twsc", "Team Work Score"},

            // Kit Time
            {"ktm-0", "Time as Anti-Tank"},
            {"ktm-1", "Time as Assault"},
            {"ktm-2", "Time as Engineer"},
            {"ktm-3", "Time as Medic"},
            {"ktm-4", "Time as SpecOps"},
            {"ktm-5", "Time as Support"},
            {"ktm-6", "Time as Sniper"},

            // Vehicle Time
            {"vtm-0", "Time in Armor"},
            {"vtm-1", "Time in Aviator"},
            {"vtm-2", "Time in Air Defense"},
            {"vtm-3", "Time in Helicopter"},
            {"vtm-4", "Time in Transport"},
            {"vtm-5", "Time in Artillery"},
            {"vtm-6", "Time in Ground Defense"},

            // Vehcile Kills
            {"vkl-0", "Armor Kills"},
            {"vkl-1", "Aviator Kills"},
            {"vkl-2", "Air Defense Kills"},
            {"vkl-3", "Helicopter Kills"},
            {"vkl-4", "Transport Kills"},
            {"vkl-5", "Artillery Kills"},
            {"vkl-6", "Ground Defense Kills"},

            // Weapon Kills
            {"wkl-0", "Assault Rifle Kills"},
            {"wkl-1", "Assault Grenade Kills"},
            {"wkl-2", "Carbine Kills"},
            {"wkl-3", "Light Machine Gun Kills"},
            {"wkl-4", "Sniper Rifle Kills"},
            {"wkl-5", "Pistol Kills"},
            {"wkl-6", "Anit-Tank/Anti-Air Kills"},
            {"wkl-7", "SubMachine Gun Kills"},
            {"wkl-8", "Shotgun Kills"},
            {"wkl-9", "Knife Kills"},
            {"wkl-10", "Defibrillator Kills"},
            {"wkl-11", "Claymore Kills"},
            {"wkl-12", "Hand Grenade Kills"},
            {"wkl-13", "AT Mine Kills"},
            {"wkl-14", "C4 Kills"},

            // Special Forces
            {"de-6", "Flash Bang / Tear Gas Deploys"},
            {"de-7", "Grappling Hook Deploys"},
            {"de-8", "Zipline Deploys"},
        };

        /// <summary>
        /// A full list of awards [AwardId => Award Name]
        /// </summary>
        public static Dictionary<string, string> Awards = new Dictionary<string, string>()
        {
            // Badges
            {"1031119", "Assault Combat Badge"},
            {"1031120", "Anti-Tank Combat Badge"},
            {"1031109", "Sniper Combat Badge"},
            {"1031115", "Spec-Ops Combat Badge"},
            {"1031121", "Support Combat Badge"},
            {"1031105", "Engineer Combat Badge"},
            {"1031113", "Medic Combat Badge"},
            {"1031406", "Knife Combat Badge"},
            {"1031619", "Pistol Combat Badge"},
            {"1032415", "Explosives Ordinance Badge"},
            {"1190601", "First Aid Badge"},
            {"1190507", "Engineer Badge"},
            {"1191819", "Resupply Badge"},
            {"1190304", "Command Badge"},
            {"1220118", "Armour Badge"},
            {"1222016", "Transport Badge"},
            {"1220803", "Helicopter Badge"},
            {"1220122", "Aviator Badge"},
            {"1220104", "Air Defense Badge"},
            {"1031923", "Ground Defense Badge"},

            // SF Badges
            {"1261119", "Assault Specialist Badge"},
            {"1261120", "Anti-Tank Specialist Badge"},
            {"1261109", "Sniper Specialist Badge"},
            {"1261115", "Spec-Ops Specialist Badge"},
            {"1261121", "Support Specialist Badge"},
            {"1261105", "Engineer Specialist Badge"},
            {"1261113", "Medic Specialist Badge"},
            {"1260602", "Tactical Support Weaponry Badge"},
            {"1260708", "Grappling Hook Specialist Badge"},
            {"1262612", "Zip Line Specialist Badge"},

            // Medals
            {"2191608", "Purple Heart"},
            {"2191319", "Meritorious Service Medal"},
            {"2190303", "Combat Action Medal"},
            {"2190309", "Air Combat Medal"},
            {"2190318", "Armour Combat Medal"},
            {"2190308", "Helecopter Combat Medal"},
            {"2190703", "Good Conduct Medal"},
            {"2020903", "Combat Infantry Medal"},
            {"2020913", "Marksman Infantry Medal"},
            {"2020919", "Sharpshooter Infantry Medal"},
            {"2021322", "Medal of Valour"},
            {"2020419", "Distinguished Service Medal"},

            // Ribbons
            {"3240301", "Combat Action Ribbon"},
            {"3211305", "Meritorious Unit Ribbon"},
            {"3150914", "Infantry Officer Ribbon"},
            {"3151920", "Staff Officer Ribbon"},
            {"3190409", "Distinguished Service Ribbon"},
            {"3242303", "War College Ribbon"},
            {"3212201", "Valorous Unit Ribbon"},
            {"3241213", "Legion of Merit Ribbon"},
            {"3190318", "Crew Service Ribbon"},
            {"3190118", "Armoured Service Ribbon"},
            {"3190105", "Aerial Service Ribbon"},
            {"3190803", "Helicopter Service Ribbon"},
            {"3040109", "Air Defense Ribbon"},
            {"3040718", "Ground Defense Ribbon"},
            {"3240102", "Airborne Ribbon"},
            {"3240703", "Good Conduct Ribbon"},

            // SF Ribbons
            {"3260318", "Crew Specialist Ribbon"},
            {"3260118", "Armored Transport Specialist Ribbon"},
            {"3260105", "Airborne Specialist Service Ribbon"},
            {"3260803", "Helo Specialist Ribbon"},

            // Cant be earned
            {"6666666", "Smoc Award. Awarded From ASP"},
            {"6666667", "General Award. Awarded From ASP"}
        };

        /// <summary>
        /// A full list of Non-SF badges [AwardId => Award Name]
        /// </summary>
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

        /// <summary>
        /// A full list of SF only Badges [AwardId => Award Name]
        /// </summary>
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

        /// <summary>
        /// A full list of Non-SF Medals [AwardId => Award Name]
        /// </summary>
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

        /// <summary>
        /// A full list of SF only Medals [AwardId => Award Name]
        /// </summary>
        public static Dictionary<string, string> SfMedals = new Dictionary<string, string>()
        {
            {"2261913", "Navy Seal Special Service Medal"},
            {"2261919", "SAS Special Special Medal"},
            {"2261613", "SPETZ Special Service Medal"},
            {"2261303", "MECSF Special Service Medal"},
            {"2261802", "Rebel Special Service Medal"},
            {"2260914", "Insurgent Special Service Medal"}
        };

        /// <summary>
        /// A full list of Non-SF Ribbons [AwardId => Award Name]
        /// </summary>
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

        /// <summary>
        /// A full list of SF only Ribbons [AwardId => Award Name]
        /// </summary>
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

        /// <summary>
        /// Provides more human readable names for the bf2 stats ranks
        /// </summary>
        public static readonly string[] Ranks = new String[22] 
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

        /// <summary>
        /// Returns whether a parameter key's valueis a time related statistic
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static bool IsTimeStat(string Name)
        {
            return Name.StartsWith("time") 
                || Name.Contains("tm-")
                || Name == "time"
                || Name == "rtime"
                || Name == "tsqm"
                || Name == "tsql"
                || Name == "tcdr";
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
    }
}
