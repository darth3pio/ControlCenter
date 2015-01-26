using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BF2Statistics
{
    /// <summary>
    /// The Stats python class is used to Install and Restore the Server
    /// python files, that control stats, awards and ranks.
    /// </summary>
    class StatsPython
    {
        /// <summary>
        /// Defines the Backup Folder path
        /// </summary>
        protected static string BackupPath = Path.Combine(MainForm.Config.ServerPath, "python", "_backup_");

        /// <summary>
        /// Defines the Backup Folder path
        /// </summary>
        protected static string StatsBackupPath = Path.Combine(MainForm.Config.ServerPath, "python", "_bf2statistics_python_");

        /// <summary>
        /// Indicates whether the server has the ranked python files installed
        /// </summary>
        public static bool StatsEnabled
        {
            get 
            { 
                return File.Exists(Path.Combine(BF2Server.PythonPath, "BF2StatisticsConfig.py")); 
            }
        }

        /// <summary>
        /// The BF2StatisticsConfig.py object
        /// </summary>
        protected static StatsPythonConfig ConfigFile = null;

        /// <summary>
        /// Returns the BF2StatisticsConfig.py file as a configuration object
        /// </summary>
        public static StatsPythonConfig Config
        {
            get
            {
                // Make sure stats are enabled
                if (!StatsEnabled)
                    throw new Exception("Cannot load Bf2StatisticsConfig.py, Stats are not enabled!");

                // Make sure config has been Initiated
                if (ConfigFile == null)
                    ConfigFile = new StatsPythonConfig();

                // Return the private singleton object
                return ConfigFile;
            }
        }

        /// <summary>
        /// Backsup the current python files, and installs the ranked enabled ones
        /// </summary>
        public static void BackupAndInstall()
        {
            if (StatsEnabled)
                return;

            // Make sure we arent Ambiguous. If the backup folder exists, just leave it!!!
            // If we have both backup folders, start fresh install
            if (Directory.Exists(BackupPath) && Directory.Exists(StatsBackupPath))
            {
                Directory.Delete(StatsBackupPath, true);
                Directory.Delete(BF2Server.PythonPath, true);
                //System.Threading.Thread.Sleep(750);
            }
            else
            {
                // move the current "normal" files over to the backup path
                Directory.Move(BF2Server.PythonPath, BackupPath);
                //System.Threading.Thread.Sleep(750);
            }

            // Make sure we dont have an empty backup folder
            if(!Directory.Exists(StatsBackupPath))
                DirectoryExt.Copy(Paths.RankedPythonPath, BF2Server.PythonPath, true);
            else
                Directory.Move(StatsBackupPath, BF2Server.PythonPath);

            // Sleep
            System.Threading.Thread.Sleep(500);
        }

        /// <summary>
        /// Removes the rank enabled python files, and installs the originals back
        /// </summary>
        public static void RemoveAndRestore()
        {
            if (!StatsEnabled)
                return;

            // Make sure we dont have a pending error here
            if (Directory.Exists(StatsBackupPath))
            {
                Directory.Delete(StatsBackupPath, true);
                //System.Threading.Thread.Sleep(750);
            }

            // Backup the users new bf2s python files
            Directory.Move(BF2Server.PythonPath, StatsBackupPath);
            //System.Threading.Thread.Sleep(750);

            // Make sure we have a backup folder!!
            if (!Directory.Exists(BackupPath))
            {
                // Copy over the default python files
                DirectoryExt.Copy(Paths.DefaultPythonPath, BF2Server.PythonPath, true);
            }
            else
            {
                // Copy back the original contents
                Directory.Move(BackupPath, BF2Server.PythonPath);
            }

            // Stop for a breather
            System.Threading.Thread.Sleep(500);
        }

        /// <summary>
        /// Restores the ranked python files back to the original state
        /// </summary>
        public static void RestoreRankedPyFiles()
        {
            if (StatsEnabled)
            {
                Directory.Delete(BF2Server.PythonPath, true);
                //System.Threading.Thread.Sleep(750);
                DirectoryExt.Copy(Paths.RankedPythonPath, BF2Server.PythonPath, true);
            }
            else
            {
                Directory.Delete(StatsBackupPath, true);
                //System.Threading.Thread.Sleep(750);
                DirectoryExt.Copy(Paths.RankedPythonPath, StatsBackupPath, true);
            }
        }
    }
}
