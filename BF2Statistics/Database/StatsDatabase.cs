using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

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

            return Driver.Query("SELECT awd, level, earned, first FROM awards WHERE id = {0} ORDER BY id", Pid);
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
            List<string> Tables = GetStatsTables();
            foreach (string Table in Tables)
            {
                if (Driver.DatabaseEngine == DatabaseEngine.Sqlite)
                    Driver.Execute("DELETE FROM " + Table);
                else
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
                    string Message = "Database Connect Error: " +
                        Environment.NewLine +
                        E.Message +
                        Environment.NewLine
                        + "Forcing Server Shutdown...";
                    throw new Exception(Message);
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
            UpdateProgressForm.ShowScreen("Creating Bf2Stats SQLite Database", MainForm.Instance);

            // Create Tables
            UpdateProgressForm.Status("Creating Tables...");
            string SQL = Utils.GetResourceString("BF2Statistics.SQL.SQLite.Stats.sql");
            Driver.Execute(SQL);

            // Insert Ip2Nation data
            UpdateProgressForm.Status("Inserting Ip2Nation Data...");
            SQL = Utils.GetResourceString("BF2Statistics.SQL.Ip2nation.sql");
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
                UpdateProgressForm.CloseForm();
                MainForm.Enable();
                throw E;
            }

            // Close update progress form
            UpdateProgressForm.CloseForm();
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
            UpdateProgressForm.ShowScreen("Creating Bf2Stats Mysql Tables", MainForm.Instance);

            // Create Tables
            UpdateProgressForm.Status("Creating Tables...");
            string SQL = Utils.GetResourceString("BF2Statistics.SQL.MySQL.Stats.sql");
            Driver.Execute(SQL);

            // Insert Ip2Nation data
            UpdateProgressForm.Status("Inserting Ip2Nation Data...");
            SQL = Utils.GetResourceString("BF2Statistics.SQL.Ip2nation.sql");

            // MySQL Will throw a packet size error here, so we need to increase this!
            Driver.Execute("SET GLOBAL max_allowed_packet=2048;");

            // Begin execution
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
                UpdateProgressForm.CloseForm();
                MainForm.Enable();
                throw E;
            }

            // Close update progress form
            UpdateProgressForm.CloseForm();
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
    }
}
