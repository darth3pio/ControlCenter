 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BF2Statistics
{
    public class BF2Server
    {
        /// <summary>
        /// Contains the BF2 Servers root path
        /// </summary>
        public static string RootPath { get; protected set; }

        /// <summary>
        /// Contains a list of all the found mod folders located in the "mods" directory
        /// </summary>
        public static List<string> Mods { get; protected set; }

        /// <summary>
        /// Once a mod is loaded into an object, it is stored here to prevent additional
        /// loading to be done later
        /// </summary>
        protected static Dictionary<string, BF2Mod> LoadedMods;

        /// <summary>
        /// Loads a battlefield 2 server into this object for use.
        /// </summary>
        /// <param name="ServerPath">The full root path to the server's executable file</param>
        public static void Load(string ServerPath)
        {
            // Make sure we have a valid server path
            if (!File.Exists(Path.Combine(ServerPath, "bf2_w32ded.exe")))
                throw new ArgumentException("Invalid server path");

            // Define var values
            RootPath = ServerPath;
            string path = Path.Combine(ServerPath, "mods");

            // Make sure the levels folder exists!
            if (!Directory.Exists(path))
            {
                throw new Exception("Unable to locate the 'mods' folder. Please make sure you have selected a valid "
                    + "battlefield 2 installation path before proceeding.");

            }

            // Get our mod directories
            Mods = new List<string>(from dir in Directory.GetDirectories(path) select dir.Substring(path.Length + 1));
            LoadedMods = new Dictionary<string, BF2Mod>();
        }

        /// <summary>
        /// Fetches the BF2 Mod into an object. If the Mod has already been loaded
        /// into an object previously, that object will be returned instead.
        /// </summary>
        /// <param name="Name">The mod's FOLDER name</param>
        /// <returns></returns>
        public static BF2Mod LoadMod(string Name)
        {
            // Check 2 things, 1, does the mod exist, and 2, if we have loaded it already
            if (!Mods.Contains(Name))
                throw new ArgumentException("Bf2 Mod " + Name + " does not exist!");
            else if (LoadedMods.ContainsKey(Name))
                return LoadedMods[Name];

            // Create a new instance of the mod, and store it for later
            BF2Mod Mod = new BF2Mod(Path.Combine(RootPath, "mods"), Name);
            LoadedMods.Add(Name, Mod);
            return Mod;
        }
    }
}
