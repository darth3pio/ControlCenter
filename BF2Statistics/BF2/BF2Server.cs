using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
        /// While loading the server's found mods, if there was any errors, the messages
        /// will be stored in this list
        /// </summary>
        public static List<string> ModLoadErrors { get; protected set; }

        /// <summary>
        /// Returns whether the server is currently running
        /// </summary>
        public static bool IsRunning
        {
            get { return ServerProcess != null; }
        }

        /// <summary>
        /// An event thats fired if the Bf2 server path is changed
        /// </summary>
        public static event ServerChangedEvent ServerPathChanged;

        /// <summary>
        /// An event fired when the BF2 Server is started
        /// </summary>
        public static event ServerChangedEvent Started;

        /// <summary>
        /// An event fired when the BF2 server exits
        /// </summary>
        public static event ServerChangedEvent Exited;

        /// <summary>
        /// The active (or null) server process
        /// </summary>
        protected static Process ServerProcess;

        /// <summary>
        /// Loads a battlefield 2 server into this object for use.
        /// </summary>
        /// <param name="ServerPath">The full root path to the server's executable file</param>
        public static void SetServerPath(string ServerPath)
        {
            // Make sure we have a valid server path
            if (!File.Exists(Path.Combine(ServerPath, "bf2_w32ded.exe")))
                throw new ArgumentException("Invalid server path");

            // Make sure we actually changed server paths before processing
            if (!String.IsNullOrEmpty(RootPath) && (new Uri(ServerPath)) == (new Uri(RootPath)))
            {
                // Same path is selected, just return
                return;
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
            ModLoadErrors = new List<string>();
            IEnumerable<string> ModList = from dir in Directory.GetDirectories(Modpath) select dir.Substring(Modpath.Length + 1);
            foreach (string Name in ModList)
            {
                try
                {
                    // Create a new instance of the mod, and store it for later
                    BF2Mod Mod = new BF2Mod(Modpath, Name);
                    TempMods.Add(Mod);
                }
                catch (InvalidModException E)
                {
                    ModLoadErrors.Add(E.Message);
                    continue;
                }
                catch (Exception E)
                {
                    ModLoadErrors.Add(E.Message);
                    Program.ErrorLog.Write(E.Message);
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
            if (ServerPathChanged != null)
                ServerPathChanged();
            
            // Recheck server process
            CheckServerProcess();
        }

        /// <summary>
        /// Assigns the Server Process if the process is running
        /// </summary>
        protected static void CheckServerProcess()
        {
            try
            {
                Process[] processCollection = Process.GetProcessesByName("bf2_w32ded");
                foreach (Process P in processCollection)
                {
                    if (Path.GetDirectoryName(P.MainModule.FileName) == RootPath)
                    {
                        // Hook into the proccess so we know when its running, and register a closing event
                        ServerProcess = P;
                        ServerProcess.EnableRaisingEvents = true;
                        ServerProcess.Exited += ServerProcess_Exited;

                        // Fire Event
                        if (Started != null)
                            Started();
                        break;
                    }
                }
            }
            catch { } // Who cares?
        }

        /// <summary>
        /// Starts the Battlefield 2 Server application
        /// </summary>
        /// <param name="Mod">The battlefield 2 mod that the server is to use</param>
        /// <param name="ExtraArgs">Any arguments to be past to the application on startup</param>
        /// <param name="ShowConsole">If false, a console will not be created</param>
        /// <param name="MinConsole">If <see cref="ShowConsole"/> is enabled, true will start the console minimized</param>
        public static void Start(BF2Mod Mod, string ExtraArgs, bool ShowConsole, bool MinConsole)
        {
            // Make sure the server isnt running already
            if (IsRunning)
                throw new Exception("The Battlefield 2 server is already running!");

            // Make sure the mod is supported!
            if (!Mods.Contains(Mod))
                throw new Exception("The battlefield 2 mod cannot be located in the mods folder");

            // Start new BF2 proccess
            ProcessStartInfo Info = new ProcessStartInfo();
            Info.Arguments = String.Format(" +modPath mods/{0}", Mod.Name.ToLower());
            if (!String.IsNullOrEmpty(ExtraArgs))
                Info.Arguments += " " + ExtraArgs;

            // Hide window if user specifies this...
            if (!ShowConsole)
                Info.WindowStyle = ProcessWindowStyle.Hidden;
            else if (MinConsole)
                Info.WindowStyle = ProcessWindowStyle.Minimized;

            // Start process. Set working directory so we dont get errors!
            Info.FileName = "bf2_w32ded.exe";
            Info.WorkingDirectory = RootPath;
            ServerProcess = Process.Start(Info);

            // Hook into the proccess so we know when its running, and register a closing event
            ServerProcess.EnableRaisingEvents = true;
            ServerProcess.Exited += ServerProcess_Exited;

            // Call event
            if (Started != null)
                Started();
        }

        /// <summary>
        /// Kills the Bf2 Server process
        /// </summary>
        public static void Stop()
        {
            if(IsRunning) 
                ServerProcess.Kill();
        }

        /// <summary>
        /// Event fired when the server closes down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected static void ServerProcess_Exited(object sender, EventArgs e)
        {
            if (Exited != null)
                Exited();

            ServerProcess.Close();
            ServerProcess = null;
        }
    }
}
