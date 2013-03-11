using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BF2Statistics.Database
{
    public class GamespyDatabase
    {
        private DatabaseDriver Driver;

        public GamespyDatabase()
        {
            CheckConnection();
        }

        ~GamespyDatabase()
        {
            Close();
        }

        public Dictionary<string, object> GetUser(string Nick)
        {
            // Fetch the user
            CheckConnection();
            var Rows = Driver.Query("SELECT id, name, password, email, country, session FROM accounts WHERE name='{0}'", Nick);
            //Driver.Close();
            return (Rows.Count == 0) ? null : Rows[0];
        }

        public Dictionary<string, object> GetUser(string Email, string Password)
        {
            CheckConnection();
            var Rows = Driver.Query("SELECT id, name, password, country, session FROM accounts WHERE email='{0}' AND password='{1}'", Email, Password);
            //Driver.Close();
            return (Rows.Count == 0) ? null : Rows[0];
        }

        public List<string> GetUsersLike(string Nick)
        {
            // Generate our return list
            List<string> List = new List<string>();

            CheckConnection();
            var Rows = Driver.Query("SELECT name FROM accounts WHERE name LIKE '%{0}%'", Nick);
            //Driver.Close();

            if (Rows == null)
                return List;

            foreach (Dictionary<string, object> Account in Rows)
                List.Add(Account["name"].ToString());

            return List;
        }

        public bool UserExists(string Nick)
        {
            // Fetch the user
            CheckConnection();
            var Rows = Driver.Query("SELECT id FROM accounts WHERE name='{0}'", Nick);
            //Driver.Close();

            return (Rows.Count == 0) ? false : true;
        }

        public bool UserExists(int PID)
        {
            // Fetch the user
            CheckConnection();
            var Rows = Driver.Query("SELECT name FROM accounts WHERE id='{0}'", PID);
            //Driver.Close();

            return (Rows.Count == 0) ? false : true;
        }

        public bool CreateUser(string Nick, string Pass, string Email, string Country)
        {
            CheckConnection();

            // Generate PID
            int pid = 1;

            // User doesnt have a PID yet, Get the current max PID and increment
            var Max = Driver.Query("SELECT MAX(id) AS max FROM accounts");
            try
            {
                int max;
                Int32.TryParse(Max[0]["max"].ToString(), out max);
                pid = (max + 1);
                if (pid < 500000000)
                    pid = 500000000;
            }
            catch
            {
                pid = 500000000;
            }

            // Create the user in the database
            int Rows = Driver.Execute("INSERT INTO accounts(id, name, password, email, country) VALUES({0}, '{1}', '{2}', '{3}', '{4}')",
                pid, Nick, Pass, Email, Country
            );

            //Driver.Close();

            return (Rows != 0);
        }

        public void UpdateUser(string Nick, string Country)
        {
            CheckConnection();
            Driver.Execute("UPDATE accounts SET country='{0}' WHERE name='{1}'", Nick, Country);
            //Driver.Close();
        }

        public void UpdateUser(int Id, int NewPid, string NewNick, string NewPassword, string NewEmail)
        {
            CheckConnection();
            Driver.Execute("UPDATE accounts SET id='{0}', name='{1}', password='{2}', email='{3}' WHERE id='{4}'", 
                NewPid, NewNick, NewPassword, NewEmail, Id);
            //Driver.Close();
        }

        public int DeleteUser(string Nick)
        {
            CheckConnection();
            int result = Driver.Execute("DELETE FROM accounts WHERE name='{0}'", Nick);
            //Driver.Close();
            return result;
        }

        public int GetPID(string Nick)
        {
            CheckConnection();
            var Rows = Driver.Query("SELECT id FROM accounts WHERE name='{0}'", Nick);

            // If we have no result, we need to create a new Player :)
            if (Rows.Count == 0)
            {
                //Driver.Close();
                return 0;
            }

            //Driver.Close();

            int pid;
            Int32.TryParse(Rows[0]["id"].ToString(), out pid);
            return pid;
        }

        public int SetPID(string Nick, int Pid)
        {

            CheckConnection();

            var Rows = Driver.Query("SELECT id FROM accounts WHERE name='{0}'", Nick);
            var PidExists = Driver.Query("SELECT name FROM accounts WHERE id='{0}'", Pid);

            // If no user exists, return code -1
            if (Rows.Count == 0)
                return -1;

            // If PID is false, the PID is not taken
            if (PidExists == null)
            {
                int Success = Driver.Execute("UPDATE accounts SET id='{0}' WHERE name='{1}'", Pid, Nick);
                //Driver.Close();
                return (Success == 1) ? 1 : 0;
            }

            //Driver.Close();
            return -2; // PID exists already
        }

        public int GetNumAccounts()
        {
            CheckConnection();

            int result = 0;
            List<Dictionary<string, object>> r = Driver.Query("SELECT COUNT(id) AS count FROM accounts");
            Int32.TryParse(r[0]["count"].ToString(), out result);

            //Driver.Close();
            return result;
        }

        /// <summary>
        /// Creates the connection to the database, and handles
        /// the excpetion (if any) that are thrown
        /// </summary>
        public void CheckConnection()
        {
            if(Driver == null || !Driver.IsConnected)
            {
                try
                {
                    // First time connection
                    if (Driver == null)
                    {
                        Driver = new DatabaseDriver(
                            MainForm.Config.GamespyDBEngine,
                            MainForm.Config.GamespyDBHost,
                            MainForm.Config.GamespyDBPort,
                            MainForm.Config.GamespyDBName,
                            MainForm.Config.GamespyDBUser,
                            MainForm.Config.GamespyDBPass
                        );

                        // Create SQL tables on new SQLite DB's
                        if (Driver.IsNewDatabase)
                        {
                            // Connect to DB
                            Driver.Connect();
                            string SQL = Utils.GetResourceString("BF2Statistics.SQL.SQLite.Gamespy.sql");
                            Driver.Execute(SQL);
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
