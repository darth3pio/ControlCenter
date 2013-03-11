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

        public List<Dictionary<string, object>> GetPlayerAwards(int Pid)
        {
            CheckConnection();

            return Driver.Query("SELECT awd, level, earned, first FROM awards WHERE id = {0} ORDER BY id", Pid);
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
                        Driver = new DatabaseDriver(
                            MainForm.Config.StatsDBEngine,
                            MainForm.Config.StatsDBHost,
                            MainForm.Config.StatsDBPort,
                            MainForm.Config.StatsDBName,
                            MainForm.Config.StatsDBUser,
                            MainForm.Config.StatsDBPass
                        );

                        // Create SQL tables on new SQLite DB's
                        if (Driver.IsNewDatabase)
                        {
                            // Connect to DB
                            Driver.Connect();
                            MainForm.Disable();
                            UpdateProgressForm.ShowScreen("Creating Bf2Stats SQLite Database");

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
                            try {
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

        public void Close()
        {
            if (Driver != null)
                Driver.Close();
        }
    }
}
