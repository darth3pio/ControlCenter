using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BF2Statistics
{
    public static class Paths
    {
        /// <summary>
        /// The bf2 server python folder path
        /// </summary>
        public static readonly string ServerPythonPath;

        /// <summary>
        /// Full path to the stats enabled python files
        /// </summary>
        public static readonly string RankedPythonPath;

        /// <summary>
        /// Full path to the Non-Ranked (default) python files
        /// </summary>
        public static readonly string NonRankedPythonPath;

        /// <summary>
        /// The Bf2Statistics folder path in "My documents"
        /// </summary>
        public static readonly string DocumentsFolder;

        /// <summary>
        /// Full path to where the Processed snapshots are stored
        /// </summary>
        public static readonly string SnapshotProcPath;

        /// <summary>
        /// Full path to where Temporary snapshots are stored
        /// </summary>
        public static readonly string SnapshotTempPath;

        static Paths()
        {
            // Define Documents Folder
            DocumentsFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "BF2Statistics"
            );

            // Define python paths
            ServerPythonPath = Path.Combine(MainForm.Config.ServerPath, "python", "bf2");
            NonRankedPythonPath = Path.Combine(Program.RootPath, "Python", "NonRanked");
            RankedPythonPath = Path.Combine(Program.RootPath, "Python", "Ranked", "Backup");

            // Define Snapshot Paths
            SnapshotTempPath = Path.Combine(Program.RootPath, "Snapshots", "Temp");
            SnapshotProcPath = Path.Combine(Program.RootPath, "Snapshots", "Processed");
        }
    }
}
