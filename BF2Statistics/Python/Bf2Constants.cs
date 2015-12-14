using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BF2Statistics.Python
{
    /// <summary>
    /// This object holds all of the Weapon, Vehicle, and Kit and unlocks id's and constants found
    /// within the python files.
    /// </summary>
    public static class Bf2Constants
    {
        /// <summary>
        /// Provides weapon names based on the bf2.stats.constants (ConstantId => StringName)
        /// </summary>
        public static Dictionary<string, string> WeaponTypes = new Dictionary<string, string>()
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

        /// <summary>
        /// Provides kit names based on the bf2.stats.constants (ConstantId => StringName)
        /// </summary>
        public static Dictionary<string, string> KitTypes = new Dictionary<string, string>()
        {
            {"KIT_TYPE_SPECOPS", "SpecOps"},
            {"KIT_TYPE_ASSAULT", "Assault"},
            {"KIT_TYPE_MEDIC", "Medic"},
            {"KIT_TYPE_ENGINEER", "Engineer"},
            {"KIT_TYPE_AT", "Anti Tank"},
            {"KIT_TYPE_SUPPORT", "Support"},
            {"KIT_TYPE_SNIPER", "Sniper"}
        };

        /// <summary>
        /// Provides vehicle names based on the bf2.stats.constants (ConstantId => StringName)
        /// </summary>
        public static Dictionary<string, string> VehicleTypes = new Dictionary<string, string>()
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

        /// <summary>
        /// A full list of BF2 unlocks [UnlockId => Unlock Name]
        /// </summary>
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

        /// <summary>
        /// A full list of BF2 Vehicle Types [TypeId => Vehicle Name]
        /// </summary>
        public static readonly string[] Vehicles = new string[] 
        {
            "Armor",
            "Aviator",
            "Air Defense",
            "Helicopter",
            "Transport",
            "Artillery",
            "Ground Defense"
        };

        /// <summary>
        /// A full list of BF2 Kit Types [TypeId => Kit Name]
        /// </summary>
        public static readonly string[] Kits = new string[] 
        {
            "Anti-tank",
            "Assault",
            "Engineer",
            "Medic",
            "Special-Ops",
            "Support",
            "Sniper"
        };

        /// <summary>
        /// A full list of BF2 Weapon Types [TypeId => Weapon Name]
        /// </summary>
        public static readonly string[] Weapons = new string[]
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

        /// <summary>
        /// A list of special BF2 Weapon Types [TypeId => Weapon Name]
        /// </summary>
        public static readonly string[] SpecialWeapons = new string[]
        {
            "Defibrillator",
            "Explosives (C4, Claymore, AT Mine)",
            "Hand Grenade"
        };

        /// <summary>
        /// Returns the vehicle title based on the given vehicle id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static string GetVehicleName(int Id)
        {
            if (Id >= Vehicles.Length)
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
            if (Id >= Kits.Length)
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
            if (Id >= Weapons.Length)
                return "Unknown";

            return Weapons[Id];
        }

        public static string GetSpecialWeaponName(int Id)
        {
            if (Id >= SpecialWeapons.Length)
                return "Unknown";

            return SpecialWeapons[Id];
        }
    }
}
