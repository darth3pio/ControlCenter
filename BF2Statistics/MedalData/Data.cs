using System.Collections.Generic;

namespace BF2Statistics.MedalData
{
    /// <summary>
    /// Award type enumeration
    /// </summary>
    public enum AwardType { Medal, Badge, Ribbon, Rank }

    /// <summary>
    /// Badgel level enumeration
    /// </summary>
    public enum BadgeLevel { Bronze, Silver, Gold }

    /// <summary>
    /// Return types for python methods
    /// </summary>
    public enum ReturnType { Number, Bool }

    /// <summary>
    /// The data class holds all the different MedalData varaibles and constant ID's
    /// </summary>
    public static class Data
    {
        public static Dictionary<string, string> WeaponNames = new Dictionary<string, string>()
        {
            {"WEAPON_TYPE_KNIFE", "Knife"},
            {"WEAPON_TYPE_PISTOL", "Pistol"},
            {"WEAPON_TYPE_C4", "C4"},
            {"WEAPON_TYPE_ATMINE", "AtMine"},
            {"WEAPON_TYPE_CLAYMORE", "Claymore"},
            {"WEAPON_TYPE_ASSAULT", "Assault Rifles"},
            {"WEAPON_TYPE_CARBINE", "Carbines"},
            {"WEAPON_TYPE_LMG", "Light Machine Guns"},
            {"WEAPON_TYPE_SNIPER", "Sniper Rifles"},
            {"WEAPON_TYPE_SMG", "Sub Machine Guns"},
            {"WEAPON_TYPE_SHOTGUN", "Shotguns"},
            {"WEAPON_TYPE_HANDGRENADE", "Hand Grenades"},
            {"WEAPON_TYPE_SHOCKPAD", "Defibrillator"},
            {"WEAPON_TYPE_ATAA", "Anti-Tank / Anti-Air"},
            {"WEAPON_TYPE_TACTICAL", "Flash Bang / Tear Gas"},
            {"WEAPON_TYPE_GRAPPLINGHOOK", "Grappling Hook"},
            {"WEAPON_TYPE_ZIPLINE", "Zipline"},
        };

        public static Dictionary<string, string> KitNames = new Dictionary<string, string>()
        {
            {"KIT_TYPE_SPECOPS", "SpecOps"},
            {"KIT_TYPE_ASSAULT", "Assault"},
            {"KIT_TYPE_MEDIC", "Medic"},
            {"KIT_TYPE_ENGINEER", "Engineer"},
            {"KIT_TYPE_AT", "Anti Tank"},
            {"KIT_TYPE_SUPPORT", "Support"},
            {"KIT_TYPE_SNIPER", "Sniper"}
        };

        public static Dictionary<string, string> VehicleNames = new Dictionary<string, string>()
        {
            {"VEHICLE_TYPE_ARMOR", "Armor"},
            {"VEHICLE_TYPE_AVIATOR", "Aviator"},
            {"VEHICLE_TYPE_AIRDEFENSE", "Air Defense"},
            {"VEHICLE_TYPE_HELICOPTER", "Helicopter"},
            {"VEHICLE_TYPE_TRANSPORT", "Transport"},
            {"VEHICLE_TYPE_ARTILLERY", "Artillary"},
            {"VEHICLE_TYPE_GRNDDEFENSE", "Ground Defense"},
            {"VEHICLE_TYPE_PARACHUTE", "Parachute"},
            {"VEHICLE_TYPE_SOLDIER", "Soldier"},
            {"VEHICLE_TYPE_NIGHTVISION", "Night Vision"},
            {"VEHICLE_TYPE_GASMASK", "Gask Mask"},
        };

        public static Dictionary<string, string> PlayerStrings = new Dictionary<string, string>()
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
        public static Dictionary<string, string> GlobalStrings = new Dictionary<string, string>()
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
    }
}
