using System;
using System.IO;

namespace BF2Statistics
{
    /// <summary>
    /// Provides common file paths that this system will use
    /// </summary>
    public static class Paths
    {
        /// <summary>
        /// Full path to the stats enabled python files
        /// </summary>
        public static readonly string RankedPythonPath;

        /// <summary>
        /// Full path to the default python files
        /// </summary>
        public static readonly string DefaultPythonPath;

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
            RankedPythonPath = Path.Combine(Program.RootPath, "Python", "Ranked");
            DefaultPythonPath = Path.Combine(Program.RootPath, "Python", "NonRanked");

            // Define Snapshot Paths
            SnapshotTempPath = Path.Combine(Program.RootPath, "Snapshots", "Temp");
            SnapshotProcPath = Path.Combine(Program.RootPath, "Snapshots", "Processed");
        }
    }
}
