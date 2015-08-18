namespace BF2Statistics.ASP.StatsProcessor
{
    /// <summary>
    /// This enumeration provides 2 snapshot operation options for saving
    /// SNAPSHOT data to the stats database
    /// </summary>
    public enum SnapshotMode
    {
        /// <summary>
        /// Represents that this snapshot is to be copied as-is to the database 
        /// </summary>
        FullSync,

        /// <summary>
        /// Record everything, except Rank and Award data (typically use for LANs or 
        /// Tournaments where local db starts blank)
        /// </summary>
        Minimal
    }
}
