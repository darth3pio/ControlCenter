using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.IO;

namespace BF2Statistics.Database
{
    public class StatsDatabase
    {
        public DatabaseDriver Driver { get; protected set; }

        public StatsDatabase()
        {
            CheckConnection();
        }

        /// <summary>
        /// Returns a list of awards a player has earned
        /// </summary>
        /// <param name="Pid"></param>
        /// <returns></returns>
        public List<Dictionary<string, object>> GetPlayerAwards(int Pid)
        {
            CheckConnection();

            return Driver.Query("SELECT awd, level, earned, first FROM awards WHERE id = @P0 ORDER BY id", Pid);
        }

        /// <summary>
        /// Returns a list of all table names in the stats database
        /// </summary>
        /// <returns></returns>
        public static List<string> GetStatsTables()
        {
            return new List<string>()
            {
                { "army" },
                { "awards" },
                { "kills" },
                { "kits" },
                { "mapinfo" },
                { "maps" },
                { "player" },
                { "player_history" },
                { "round_history" },
                { "servers" },
                { "unlocks" },
                { "vehicles" },
                { "weapons" },
            };
        }

        /// <summary>
        /// Returns a list of player tables in the stats database
        /// </summary>
        /// <returns></returns>
        public static List<string> GetPlayerTables()
        {
            return new List<string>()
            {
                { "army" },
                { "awards" },
                { "kills" },
                { "kits" },
                { "maps" },
                { "player" },
                { "player_history" },
                { "unlocks" },
                { "vehicles" },
                { "weapons" },
            };
        }

        /// <summary>
        /// Clears all stats data from the stats database
        /// </summary>
        public void Truncate()
        {
            // Sqlite Database doesnt have a truncate method, so we will just recreate the database
            if (Driver.DatabaseEngine == DatabaseEngine.Sqlite)
            {
                // Stop the server to delete the file
                ASP.ASPServer.Stop();
                File.Delete(Path.Combine(MainForm.Root, MainForm.Config.StatsDBName + ".sqlite3"));
                System.Threading.Thread.Sleep(500); // Make sure the file deletes before the ASP server starts again!

                // Reset driver, start ASP again
                Driver = null;
                ASP.ASPServer.Start();
            }
            else
            {
                // Use MySQL's truncate method to clear the tables
                List<string> Tables = GetStatsTables();
                foreach (string Table in Tables)
                    Driver.Execute("TRUNCATE TABLE " + Table);
            }
        }

        /// <summary>
        /// Creates the connection to the database, and handles
        /// the excpetion (if any) that are thrown
        /// </summary>
        /// <summary>
        /// Creates the connection to the database, and handles
        /// the excpetion (if any) that are thrown
        /// </summary>
        public void CheckConnection()
        {
            if (Driver == null || !Driver.IsConnected)
            {
                try
                {
                    // First time connection
                    if (Driver == null)
                    {
                        // Create database connection
                        Driver = new DatabaseDriver(
                            MainForm.Config.StatsDBEngine,
                            MainForm.Config.StatsDBHost,
                            MainForm.Config.StatsDBPort,
                            MainForm.Config.StatsDBName,
                            MainForm.Config.StatsDBUser,
                            MainForm.Config.StatsDBPass
                        );
                        Driver.Connect();

                        // Create SQL tables on new SQLite DB's
                        if (Driver.IsNewDatabase)
                        {
                            CreateSqliteTables(Driver);
                            return;
                        }
                        else
                        {
                            // Try and get database version
                            try
                            {
                                var Rows = Driver.Query("SELECT dbver FROM _version LIMIT 1");
                                if (Rows.Count == 0)
                                    throw new Exception(); // Force insert of IP2Nation
                            }
                            catch
                            {
                                // Table doesnt contain a _version table, so run the createTables.sql
                                if (Driver.DatabaseEngine == DatabaseEngine.Sqlite)
                                    CreateSqliteTables(Driver);
                                else
                                    CreateMysqlTables(Driver);
                            }

                            return;
                        }
                    }

                    // Connect to DB
                    Driver.Connect();
                }
                catch (Exception E)
                {
                    throw new Exception(
                        "Database Connect Error: " +
                        Environment.NewLine +
                        E.Message +
                        Environment.NewLine +
                        "Forcing Server Shutdown..."
                    );
                }
            }
        }

        /// <summary>
        /// On a new Sqlite database, this method will create the default tables
        /// </summary>
        /// <param name="Driver"></param>
        private void CreateSqliteTables(DatabaseDriver Driver)
        {
            // Show Progress Form
            MainForm.Disable();
            bool TaskFormWasOpen = TaskForm.IsOpen;
            if(!TaskFormWasOpen)
                TaskForm.Show(MainForm.Instance, "Create Database", "Creating Bf2Stats SQLite Database...", false);

            // Create Tables
            TaskForm.UpdateStatus("Creating Stats Tables");
            string SQL = Utils.GetResourceAsString("BF2Statistics.SQL.SQLite.Stats.sql");
            Driver.Execute(SQL);

            // Insert Ip2Nation data
            TaskForm.UpdateStatus("Inserting Ip2Nation Data");
            SQL = Utils.GetResourceAsString("BF2Statistics.SQL.Ip2nation.sql");
            DbTransaction Transaction = Driver.BeginTransaction();
            Driver.Execute(SQL);

            // Attempt to do the transaction
            try
            {
                Transaction.Commit();
            }
            catch (Exception E)
            {
                Transaction.Rollback();
                if(!TaskFormWasOpen)
                    TaskForm.CloseForm();
                MainForm.Enable();
                throw E;
            }

            // Close update progress form
            if(!TaskFormWasOpen) TaskForm.CloseForm();
            MainForm.Enable();
        }

        /// <summary>
        /// On a new Mysql database, this method will create the default tables
        /// </summary>
        /// <param name="Driver"></param>
        private void CreateMysqlTables(DatabaseDriver Driver)
        {
            // Show Progress Form
            MainForm.Disable();
            bool TaskFormWasOpen = TaskForm.IsOpen;
            if (!TaskFormWasOpen)
                TaskForm.Show(MainForm.Instance, "Create Database", "Creating Bf2Stats Mysql Tables...", false);

            // To prevent packet size errors
            Driver.Execute("SET GLOBAL max_allowed_packet=51200");

            // Start Transaction
            DbTransaction Transaction = Driver.BeginTransaction();
            TaskForm.UpdateStatus("Creating Stats Tables");

            // Gets Table Queries
            string[] SQL = Utils.GetResourceFileLines("BF2Statistics.SQL.MySQL.Stats.sql");
            List<string> Queries = Utilities.Sql.ExtractQueries(SQL);

            // Attempt to do the transaction
            try
            {
                // Create Tables
                foreach (string Query in Queries)
                    Driver.Execute(Query);

                // Commit
                Transaction.Commit();
            }
            catch (Exception E)
            {
                Transaction.Rollback();
                if (!TaskFormWasOpen)
                    TaskForm.CloseForm();
                MainForm.Enable();
                throw E;
            }

            // Insert Ip2Nation data
            Transaction = Driver.BeginTransaction();
            TaskForm.UpdateStatus("Inserting Ip2Nation Data");
            SQL = Utils.GetResourceFileLines("BF2Statistics.SQL.Ip2nation.sql");
            Queries = Utilities.Sql.ExtractQueries(SQL);

            // Attempt to do the transaction
            try
            {
                // Insert rows
                foreach (string Query in Queries)
                    Driver.Execute(Query);

                // Commit
                Transaction.Commit();
            }
            catch (Exception E)
            {
                Transaction.Rollback();
                if(!TaskFormWasOpen)
                    TaskForm.CloseForm();
                MainForm.Enable();
                throw E;
            }

            // Close update progress form
            if (!TaskFormWasOpen) TaskForm.CloseForm();
            MainForm.Enable();
        }

        /// <summary>
        /// Closes the database connection
        /// </summary>
        public void Close()
        {
            if (Driver != null)
                Driver.Close();
        }

        /*
        private void ASPServer_OnStart()
        {
            ASP.ASPServer.OnStart -= new StartupEventHandler(ASPServer_OnStart);
            this.Driver = ASP.ASPServer.Database.Driver;
        }
         */
    }
}
