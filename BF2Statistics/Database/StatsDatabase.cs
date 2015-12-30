using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;
using BF2Statistics.Utilities;

namespace BF2Statistics.Database
{
    /// <summary>
    /// A class to provide common tasks against the Stats Database
    /// </summary>
    public class StatsDatabase : DatabaseDriver, IDisposable
    {
        /// <summary>
        /// Indicates the most up to date database table version
        /// </summary>
        public static readonly Version LatestVersion = new Version(2, 2, 0);

        /// <summary>
        /// Indicates the current Database tables version
        /// </summary>
        public Version Version { get; protected set; }

        /// <summary>
        /// An array of all table versions that we can migrate from
        /// if updating from an old database tables version
        /// </summary>
        public static readonly Version[] VersionList = {
                new Version(1, 3, 0),
                new Version(1, 3, 2),
                new Version(1, 3, 4),
                new Version(1, 4, 0),
                new Version(1, 4, 2),
                new Version(1, 4, 5),
                new Version(1, 5, 0),
                LatestVersion // Always last
            };

        /// <summary>
        /// Indicates whether the SQL tables exist in this database
        /// </summary>
        public bool TablesExist => Version.Major > 0;

        /// <summary>
        /// Indicates whether the user should be notified to update the database
        /// </summary>
        public bool NeedsUpdated => Version.CompareTo(LatestVersion) < 0;

        /// <summary>
        /// An array of all Stats specific table names
        /// </summary>
        public static readonly string[] StatsTables = {
                "army",
                "awards",
                "kills",
                "kits",
                "maps",
                "mapinfo",
                "player",
                "player_history",
                "round_history",
                "servers",
                "unlocks",
                "vehicles",
                "weapons"
            };

        /// <summary>
        /// An array of Table names that contain only player data
        /// </summary>
        public static readonly string[] PlayerTables = {
                "army",
                "awards",
                "kills",
                "kits",
                "maps",
                "player",
                "player_history",
                "unlocks",
                "vehicles",
                "weapons"
            };

        /// <summary>
        /// Creates a connection to the Stats Database
        /// </summary>
        public StatsDatabase() : base(Program.Config.StatsDBEngine, Program.Config.StatsDBConnectionString)
        {
            // Try and Reconnect
            try
            {
                base.Connect();
                GetTablesVersion();
            }
            catch (Exception)
            {
                if (Connection != null)
                    Connection.Dispose();

                throw;
            }

            // Set global packet size with MySql
            if (DatabaseEngine == DatabaseEngine.Mysql)
                base.Execute("SET GLOBAL max_allowed_packet=51200");
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~StatsDatabase()
        {
            if (!IsDisposed)
                base.Dispose();
        }

        /// <summary>
        /// Fetches the current tables version
        /// </summary>
        /// <remarks>
        ///     We have this in a seperate method because the table migration fetches the the 
        ///     version everytime an update is applied
        /// </remarks>
        protected void GetTablesVersion()
        {
            // Try and get database version
            try
            {
                string ver = base.ExecuteScalar("SELECT dbver FROM _version LIMIT 1").ToString();
                Version = Version.Parse(ver);
            }
            catch
            {
                // Table doesnt contain a _version table, then we arent installed
                Version = new Version(0, 0);
                //throw;
            }
        }

        #region Player Methods

        /// <summary>
        /// Returns whether or not a player exists in the "player" table
        /// </summary>
        /// <param name="Pid">The Player ID</param>
        /// <returns></returns>
        public bool PlayerExists(int Pid)
        {
            return (base.Query("SELECT name FROM player WHERE id=@P0", Pid).Count == 1);
        }

        /// <summary>
        /// Returns a list of awards a player has earned
        /// </summary>
        /// <param name="Pid">The Player ID</param>
        /// <returns></returns>
        public List<Dictionary<string, object>> GetPlayerAwards(int Pid)
        {
            return base.Query("SELECT awd, level, earned, first FROM awards WHERE id = @P0 ORDER BY id", Pid);
        }

        /// <summary>
        /// Removes a player, based on pid, from the stats database
        /// </summary>
        /// <param name="Pid">The players Id</param>
        /// <param name="TaskProgress">The progress object to report to.</param>
        public void DeletePlayer(int Pid, IProgress<TaskProgressUpdate> TaskProgress = null)
        {
            using (DbTransaction Transaction = BeginTransaction())
            {
                try
                {
                    // Remove the player from each player table
                    foreach (string Table in PlayerTables)
                    {
                        if (TaskProgress != null)
                            TaskProgress.Report(new TaskProgressUpdate("Removing player from \"" + Table + "\" table..."));

                        if (Table == "kills")
                            base.Execute(String.Format("DELETE FROM {0} WHERE attacker={1} OR victim={1}", Table, Pid));
                        else
                            base.Execute(String.Format("DELETE FROM {0} WHERE id={1}", Table, Pid));
                    }

                    // Commit Transaction
                    if (TaskProgress != null)
                        TaskProgress.Report(new TaskProgressUpdate("Committing Transaction"));
                    Transaction.Commit();
                }
                catch (Exception)
                {
                    // Rollback!
                    Transaction.Rollback();
                    throw;
                }
            }
        }

        #endregion Player Methods

        /// <summary>
        /// Clears all stats data from the stats database
        /// </summary>
        public void Truncate()
        {
            // Start a new transaction
            using (DbTransaction T = BeginTransaction())
            {
                // Sqlite Databases use an alternate method for clearing
                if (DatabaseEngine == DatabaseEngine.Sqlite)
                {
                    // Delete all records from each table
                    foreach (string Table in StatsTables)
                        base.Execute("DELETE FROM " + Table);

                    // Execute the VACUUM command to shrink the DB page size
                    T.Commit();
                    base.Execute("VACUUM;");
                }
                else
                {
                    // Use MySQL's truncate method to clear the tables.
                    foreach (string Table in StatsTables)
                        base.Execute("TRUNCATE TABLE " + Table);

                    T.Commit();
                }
            }
        }

        /// <summary>
        /// Tells the Database to install the Stats tables into the database
        /// </summary>
        public void CreateSqlTables(IProgress<TaskProgressUpdate> TaskProgress = null)
        {
            if (TablesExist)
                return;

            if (base.DatabaseEngine == DatabaseEngine.Mysql)
                CreateMysqlTables(TaskProgress);
            else
                CreateSqliteTables(TaskProgress);
        }

        /// <summary>
        /// On a new Sqlite database, this method will create the default tables
        /// </summary>
        private void CreateSqliteTables(IProgress<TaskProgressUpdate> TaskProgress)
        {
            // Show Progress Form
            if (TaskProgress != null)
                TaskProgress.Report(new TaskProgressUpdate("Creating Stats Tables", "Creating Bf2Stats SQLite Database..."));

            try
            {
                // Create Tables
                string SQL = Program.GetResourceAsString("BF2Statistics.SQL.SQLite.Stats.sql");
                base.Execute(SQL);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// On a new Mysql database, this method will create the default tables
        /// </summary>
        private void CreateMysqlTables(IProgress<TaskProgressUpdate> TaskProgress)
        {
            // Show Progress Form
            if (TaskProgress != null)
                TaskProgress.Report(new TaskProgressUpdate("Creating Stats Tables", "Creating Bf2Stats Mysql Tables..."));

            // Gets Table Queries
            string[] SQL = Program.GetResourceFileLines("BF2Statistics.SQL.MySQL.Stats.sql");
            List<string> Queries = SqlFile.ExtractQueries(SQL);

            // Start Transaction
            using (DbTransaction Transaction = BeginTransaction())
            {
                // Attempt to do the transaction
                try
                {
                    // Create Tables
                    foreach (string Query in Queries)
                        base.Execute(Query);

                    // Commit
                    Transaction.Commit();
                }
                catch
                {
                    Transaction.Rollback();
                    throw;
                }
            }

            // Update status
            if (TaskProgress != null)
                TaskProgress.Report(new TaskProgressUpdate("Inserting Ip2Nation Data"));

            // WE STILL INSTALL ip2Nation DATA to stay compatible with the web ASP
            SQL = Program.GetResourceFileLines("BF2Statistics.SQL.Ip2nation.sql");
            Queries = SqlFile.ExtractQueries(SQL);

            // Insert Ip2Nation data
            using (DbTransaction Transaction = BeginTransaction())
            {
                // Attempt to do the transaction
                try
                {
                    // Insert rows
                    foreach (string Query in Queries)
                        base.Execute(Query);

                    // Commit
                    Transaction.Commit();
                }
                catch
                {
                    Transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// If there is any table updates that need to be applied, calling this method will apply
        /// each update until the current database version is up to date
        /// </summary>
        public void MigrateTables()
        {
            MigrateTables(LatestVersion);
        }

        /// <summary>
        /// If there is any table updates that need to be applied, calling this method will apply
        /// each update until the specifed update version
        /// </summary>
        public void MigrateTables(Version ToVersion)
        {
            // If we dont need updated, what are we doing here?
            if (ToVersion.CompareTo(LatestVersion) < 0) return;

            // Make sure version is valid
            if (!VersionList.Contains(ToVersion))
                throw new ArgumentException("Supplied version does not exist as one of the migratable versions", "ToVersion");

            // Get our start and stop indexies
            int start = Array.IndexOf(VersionList, Version);
            int end = Array.IndexOf(VersionList, ToVersion);

            // Loop until we are at the specifed version
            for (int i = start; i <= end; i++)
            {
                // Apply updates till we reach the version we want
                using (DbTransaction Transaction = base.BeginTransaction())
                {
                    try
                    {
                        // Get our version string
                        Version nextVersion = VersionList[i];

                        // Gets Table Queries
                        string resource = $"BF2Statistics.SQL.Stats.{DatabaseEngine}_{nextVersion}_update.sql";
                        List<string> Queries = SqlFile.ExtractQueries(Program.GetResourceFileLines(resource));

                        // Delete old version data
                        base.Execute("DELETE FROM _version");

                        // Insert rows
                        foreach (string Query in Queries)
                            base.Execute(Query);

                        // Insert New Data
                        base.Execute("INSERT INTO _version(dbver, dbdate) VALUES (@P0, @P1)", nextVersion, DateTime.UtcNow.ToUnixTimestamp());
                        Transaction.Commit();
                    }
                    catch
                    {
                        Transaction.Rollback();
                        throw;
                    }
                }

                // Set new version
                GetTablesVersion();
            }
        }
    }
}
