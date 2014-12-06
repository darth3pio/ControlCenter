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
        /// The bf2 server python folder path
        /// </summary>
        public static string PythonPath { get; protected set; }

        /// <summary>
        /// Contains a list of all the found mod folders located in the "mods" directory
        /// </summary>
        public static List<BF2Mod> Mods { get; protected set; }

        /// <summary>
        /// An event thats fired if the Bf2 server path is changed
        /// </summary>
        public static event ServerChangedEvent ServerPathChanged;

        /// <summary>
        /// Loads a battlefield 2 server into this object for use.
        /// </summary>
        /// <param name="ServerPath">The full root path to the server's executable file</param>
        public static void Load(string ServerPath)
        {
            // Make sure we have a valid server path
            if (!File.Exists(Path.Combine(ServerPath, "bf2_w32ded.exe")))
                throw new ArgumentException("Invalid server path");

            // Defines if our path really did change
            bool Changed = false;

            // Do we need to fire a change event?
            if (!String.IsNullOrEmpty(RootPath))
            {
                // Same path is selected, just return
                if ((new Uri(ServerPath)) == (new Uri(RootPath)))
                    return;
                else
                    Changed = true;
            }

            // Temporary variables
            string Modpath = Path.Combine(ServerPath, "mods");
            string PyPath = Path.Combine(ServerPath, "python", "bf2");
            List<BF2Mod> TempMods = new List<BF2Mod>();

            // Make sure the server has the required folders
            if (!Directory.Exists(Modpath))
            {
                throw new Exception("Unable to locate the 'mods' folder. Please make sure you have selected a valid "
                    + "battlefield 2 installation path before proceeding.");

            }
            else if (!Directory.Exists(PyPath))
            {
                throw new Exception("Unable to locate the 'python/bf2' folder. Please make sure you have selected a valid "
                    + "battlefield 2 installation path before proceeding.");
            }

            // Load all found mods, discarding invalid mods
            IEnumerable<string> ModList = from dir in Directory.GetDirectories(Modpath) select dir.Substring(Modpath.Length + 1);
            foreach (string Name in ModList)
            {
                try
                {
                    // Create a new instance of the mod, and store it for later
                    BF2Mod Mod = new BF2Mod(Modpath, Name);
                    TempMods.Add(Mod);
                }
                catch (InvalidModException)
                {
                    continue;
                }
                catch (Exception e)
                {
                    Program.ErrorLog.Write(e.Message);
                }
            }

            // We need mods bro...
            if (TempMods.Count == 0)
                throw new Exception("No valid battlefield 2 mods could be found in the Bf2 Server mods folder!");

            // Define var values after we now know this server apears valid
            RootPath = ServerPath;
            PythonPath = PyPath;
            Mods = TempMods;

            // Fire change event
            if (Changed && ServerPathChanged != null)
                ServerPathChanged();
        }
    }
}
