using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.IO;
using System.Net;
using System.ComponentModel;
using BF2Statistics.Web.ASP;
using BF2Statistics.Database.QueryBuilder;
using System.Data.SQLite;

namespace BF2Statistics.Database
{
    /// <summary>
    /// A class to provide common tasks against the Stats Database
    /// </summary>
    public class StatsDatabase : DatabaseDriver, IDisposable
    {
        /// <summary>
        /// An array of Stats specific table names
        /// </summary>
        public static readonly string[] StatsTables = new string[]
        {
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
            "weapons",
        };

        /// <summary>
        /// An array of Player Table names
        /// </summary>
        public static readonly string[] PlayerTables = new string[]
        {
            "army",
            "awards",
            "kills",
            "kits",
            "maps",
            "player",
            "player_history",
            "unlocks",
            "vehicles",
            "weapons",
        };

        /// <summary>
        /// Indicates whether the SQL tables exist in this database
        /// </summary>
        public bool IsInstalled { get; protected set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public StatsDatabase():  
            base(
                MainForm.Config.StatsDBEngine, 
                MainForm.Config.StatsDBConnectionString
            )
        {

            // Try and Reconnect
            try
            {
                Connect();

                // Try and get database version
                try
                {
                    IsInstalled = (base.Query("SELECT dbver FROM _version LIMIT 1").Count > 0);
                }
                catch
                {
                    // Table doesnt contain a _version table, then we arent installed
                    IsInstalled = false;
                }
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
        /// <param name="TaskFormOpen">
        ///     If true, the task form status message will be updated as progress is made.
        ///     You are still responsible for opening and closing the task form!
        /// </param>
        public void DeletePlayer(int Pid, bool TaskFormOpen)
        {
            using (DbTransaction Transaction = BeginTransaction())
            {
                try
                {
                    // Remove the player from each player table
                    foreach (string Table in PlayerTables)
                    {
                        if (TaskFormOpen)
                            TaskForm.UpdateStatus("Removing player from \"" + Table + "\" table...");

                        if (Table == "kills")
                            base.Execute(String.Format("DELETE FROM {0} WHERE attacker={1} OR victim={1}", Table, Pid));
                        else
                            base.Execute(String.Format("DELETE FROM {0} WHERE id={1}", Table, Pid));
                    }

                    // Commit Transaction
                    if (TaskFormOpen)
                        TaskForm.UpdateStatus("Committing Transaction");
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
        public void CreateSqlTables()
        {
            if (IsInstalled)
                return;

            if (base.DatabaseEngine == DatabaseEngine.Mysql)
                CreateMysqlTables();
            else
                CreateSqliteTables();
        }

        /// <summary>
        /// On a new Sqlite database, this method will create the default tables
        /// </summary>
        private void CreateSqliteTables()
        {
            // Show Progress Form
            bool TaskFormWasOpen = TaskForm.IsOpen;
            if(!TaskFormWasOpen)
                TaskForm.Show(MainForm.Instance, "Create Database", "Creating Bf2Stats SQLite Database...", false);

            try
            {
                // Create Tables
                TaskForm.UpdateStatus("Creating Stats Tables");
                string SQL = Utils.GetResourceAsString("BF2Statistics.SQL.SQLite.Stats.sql");
                base.Execute(SQL);
            }
            catch
            {
                throw;
            }
            finally
            {
                // Close update progress form
                if (!TaskFormWasOpen)
                    TaskForm.CloseForm();
            }
        }

        /// <summary>
        /// On a new Mysql database, this method will create the default tables
        /// </summary>
        private void CreateMysqlTables()
        {
            // Show Progress Form
            bool TaskFormWasOpen = TaskForm.IsOpen;
            if (!TaskFormWasOpen)
                TaskForm.Show(MainForm.Instance, "Create Database", "Creating Bf2Stats Mysql Tables...", false);

            // Update status
            TaskForm.UpdateStatus("Creating Stats Tables");

            // Gets Table Queries
            string[] SQL = Utils.GetResourceFileLines("BF2Statistics.SQL.MySQL.Stats.sql");
            List<string> Queries = Utilities.Sql.ExtractQueries(SQL);

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
                    if (!TaskFormWasOpen)
                        TaskForm.CloseForm();

                    throw;
                }
            }

            // Update status
            // WE STILL INSTALL ip2Nation DATA to stay compatible with the web ASP
            TaskForm.UpdateStatus("Inserting Ip2Nation Data");
            SQL = Utils.GetResourceFileLines("BF2Statistics.SQL.Ip2nation.sql");
            Queries = Utilities.Sql.ExtractQueries(SQL);

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
                finally
                {
                    // Close update progress form
                    if (!TaskFormWasOpen)
                        TaskForm.CloseForm();
                }
            }
        }
    }
}
