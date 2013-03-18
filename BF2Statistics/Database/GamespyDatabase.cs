using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BF2Statistics.Database
{
    public class GamespyDatabase
    {
        /// <summary>
        /// Our database driver
        /// </summary>
        public DatabaseDriver Driver { get; protected set; }

        /// <summary>
        /// Our expected Database Version
        /// </summary>
        public static readonly int ExpectedVersion = 2;

        /// <summary>
        /// Constructor
        /// </summary>
        public GamespyDatabase()
        {
            CheckConnection();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~GamespyDatabase()
        {
            Close();
        }

        /// <summary>
        /// Fetches an account from the gamespy database
        /// </summary>
        /// <param name="Nick">The user's Nick</param>
        /// <returns></returns>
        public Dictionary<string, object> GetUser(string Nick)
        {
            // Fetch the user
            CheckConnection();
            var Rows = Driver.Query("SELECT * FROM accounts WHERE name='{0}'", Nick);
            return (Rows.Count == 0) ? null : Rows[0];
        }

        /// <summary>
        /// Fetches an account from the gamespy database
        /// </summary>
        /// <param name="Pid">The account player ID</param>
        /// <returns></returns>
        public Dictionary<string, object> GetUser(int Pid)
        {
            // Fetch the user
            CheckConnection();
            var Rows = Driver.Query("SELECT * FROM accounts WHERE id={0}", Pid);
            return (Rows.Count == 0) ? null : Rows[0];
        }

        /// <summary>
        /// Fetches an account from the gamespy database
        /// </summary>
        /// <param name="Email">Account email</param>
        /// <param name="Password">Account Password</param>
        /// <returns></returns>
        public Dictionary<string, object> GetUser(string Email, string Password)
        {
            CheckConnection();
            var Rows = Driver.Query("SELECT * FROM accounts WHERE email='{0}' AND password='{1}'", Email, Password);
            return (Rows.Count == 0) ? null : Rows[0];
        }

        /// <summary>
        /// Returns a list of player names that are similar to the passed parameter
        /// </summary>
        /// <param name="Nick"></param>
        /// <returns></returns>
        public List<string> GetUsersLike(string Nick)
        {
            CheckConnection();

            // Generate our return list
            List<string> List = new List<string>();
            var Rows = Driver.Query("SELECT name FROM accounts WHERE name LIKE '%{0}%'", Nick);
            foreach (Dictionary<string, object> Account in Rows)
                List.Add(Account["name"].ToString());

            return List;
        }

        /// <summary>
        /// Returns wether an account exists from the provided Nick
        /// </summary>
        /// <param name="Nick"></param>
        /// <returns></returns>
        public bool UserExists(string Nick)
        {
            // Fetch the user
            CheckConnection();
            var Rows = Driver.Query("SELECT id FROM accounts WHERE name='{0}'", Nick);
            return (Rows.Count != 0);
        }

        /// <summary>
        /// Returns wether an account exists from the provided Account Id
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        public bool UserExists(int PID)
        {
            // Fetch the user
            CheckConnection();
            var Rows = Driver.Query("SELECT name FROM accounts WHERE id='{0}'", PID);
            return (Rows.Count != 0);
        }

        /// <summary>
        /// Creates a new Gamespy Account
        /// </summary>
        /// <param name="Nick">The Account Name</param>
        /// <param name="Pass">The Account Password</param>
        /// <param name="Email">The Account Email Address</param>
        /// <param name="Country">The Country Code for this Account</param>
        /// <returns></returns>
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

            return (Rows != 0);
        }

        /// <summary>
        /// Creates a new Gamespy Account
        /// </summary>
        /// <param name="Nick">The Account Name</param>
        /// <param name="Pass">The Account Password</param>
        /// <param name="Email">The Account Email Address</param>
        /// <param name="Country">The Country Code for this Account</param>
        /// <returns></returns>
        public bool CreateUser(int Pid, string Nick, string Pass, string Email, string Country)
        {
            CheckConnection();

            // Make sure the user doesnt exist!
            var PidExists = Driver.Query("SELECT name FROM accounts WHERE id=" + Pid);
            var NameExists = Driver.Query("SELECT id FROM accounts WHERE name='{0}'", Nick);
            if (PidExists.Count == 1)
                throw new Exception("Account ID is already taken!");
            else if(NameExists.Count == 1)
                throw new Exception("Account username is already taken!");

            // Create the user in the database
            int Rows = Driver.Execute("INSERT INTO accounts(id, name, password, email, country) VALUES({0}, '{1}', '{2}', '{3}', '{4}')",
                Pid, Nick, Pass, Email, Country
            );

            return (Rows != 0);
        }

        /// <summary>
        /// Updates an Accounts Country Code
        /// </summary>
        /// <param name="Nick"></param>
        /// <param name="Country"></param>
        public void UpdateUser(string Nick, string Country)
        {
            CheckConnection();
            Driver.Execute("UPDATE accounts SET country='{0}' WHERE name='{1}'", Nick, Country);
        }

        /// <summary>
        /// Updates an Account's information by ID
        /// </summary>
        /// <param name="Id">The Current Account ID</param>
        /// <param name="NewPid">New Account ID</param>
        /// <param name="NewNick">New Account Name</param>
        /// <param name="NewPassword">New Account Password</param>
        /// <param name="NewEmail">New Account Email Address</param>
        public void UpdateUser(int Id, int NewPid, string NewNick, string NewPassword, string NewEmail)
        {
            CheckConnection();
            Driver.Execute("UPDATE accounts SET id='{0}', name='{1}', password='{2}', email='{3}' WHERE id='{4}'", 
                NewPid, NewNick, NewPassword, NewEmail, Id);
        }

        /// <summary>
        /// Deletes a Gamespy Account
        /// </summary>
        /// <param name="Nick"></param>
        /// <returns></returns>
        public int DeleteUser(string Nick)
        {
            CheckConnection();
            return Driver.Execute("DELETE FROM accounts WHERE name='{0}'", Nick);
        }

        /// <summary>
        /// Deletes a Gamespy Account
        /// </summary>
        /// <param name="Nick"></param>
        /// <returns></returns>
        public int DeleteUser(int Pid)
        {
            CheckConnection();
            return Driver.Execute("DELETE FROM accounts WHERE id={0}", Pid);
        }

        /// <summary>
        /// Deletes a Gamespy Account
        /// </summary>
        /// <param name="Nick"></param>
        /// <returns></returns>
        public int GetPID(string Nick)
        {
            CheckConnection();
            var Rows = Driver.Query("SELECT id FROM accounts WHERE name='{0}'", Nick);

            // If we have no result, we need to create a new Player :)
            if (Rows.Count == 0)
                return 0;

            int pid;
            Int32.TryParse(Rows[0]["id"].ToString(), out pid);
            return pid;
        }

        /// <summary>
        /// Sets the Account (Player) Id for an account by Name
        /// </summary>
        /// <param name="Nick">The account Nick we are setting the new Pid for</param>
        /// <param name="Pid">The new Pid</param>
        /// <returns></returns>
        public int SetPID(string Nick, int Pid)
        {
            CheckConnection();
            bool UserExists = Driver.Query("SELECT id FROM accounts WHERE name='{0}'", Nick).Count != 0;
            bool PidExists = Driver.Query("SELECT name FROM accounts WHERE id='{0}'", Pid).Count != 0;

            // If no user exists, return code -1
            if (UserExists)
                return -1;

            // If PID is false, the PID is not taken
            if (!PidExists)
            {
                int Success = Driver.Execute("UPDATE accounts SET id='{0}' WHERE name='{1}'", Pid, Nick);
                return (Success == 1) ? 1 : 0;
            }

            // PID exists already
            return -2;
        }

        /// <summary>
        /// Returns the number of accounts in the database
        /// </summary>
        /// <returns></returns>
        public int GetNumAccounts()
        {
            CheckConnection();
            List<Dictionary<string, object>> r = Driver.Query("SELECT COUNT(id) AS count FROM accounts");
            return Int32.Parse(r[0]["count"].ToString());
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
                        else
                        {
                            // Connect to DB
                            Driver.Connect();
                            CheckDatabase();
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
        /// Checks the Gamespy database, making sure the tables exist within,
        /// and that the database is up to date.
        /// </summary>
        private void CheckDatabase()
        {
            // Make sure our tables still exist
            List<Dictionary<string, object>> Rows = Driver.Query("SELECT COUNT(id) AS count FROM accounts");

            // Get Db Version
            int Version = 2;
            try {
                Rows = Driver.Query("SELECT dbver FROM _version");
                Version = Int32.Parse(Rows[0]["dbver"].ToString());
            }
            catch
            {
                // If an exception is thrown, table doesnt exist (Ver 1)
                if (Driver.DatabaseEngine == DatabaseEngine.Sqlite)
                    Driver.Execute(Utils.GetResourceString("BF2Statistics.SQL.Updates.SQLite.Gamespy.Update_1.sql"));
                else
                    Driver.Execute(Utils.GetResourceString("BF2Statistics.SQL.Updates.MySQL.Gamespy.Update_1.sql"));
            }

            // Process Updates
            while (Version < ExpectedVersion)
            {
                if (Driver.DatabaseEngine == DatabaseEngine.Sqlite)
                    Driver.Execute(Utils.GetResourceString("BF2Statistics.SQL.Updates.SQLite.Gamespy.Update_" + Version + ".sql"));
                else
                    Driver.Execute(Utils.GetResourceString("BF2Statistics.SQL.Updates.MySQL.Gamespy.Update_" + Version + ".sql"));
                Version++;
            }
        }

        /// <summary>
        /// Closes the Gamespy Database Connection
        /// </summary>
        public void Close()
        {
            if (Driver != null)
                Driver.Close();
        }
    }
}
