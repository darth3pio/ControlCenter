using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BF2Statistics.Database
{
    public class GamespyDatabase : DatabaseDriver
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public GamespyDatabase() : 
            base(
                MainForm.Config.GamespyDBEngine,
                MainForm.Config.GamespyDBHost,
                MainForm.Config.GamespyDBPort,
                MainForm.Config.GamespyDBName,
                MainForm.Config.GamespyDBUser,
                MainForm.Config.GamespyDBPass
            )
        {
            // Try and Reconnect
            try
            {
                Connect();

                // Try to get the database version
                try
                {
                    if (Query("SELECT dbver FROM _version LIMIT 1").Count == 0)
                        throw new Exception();
                }
                catch
                {
                    // If an exception is thrown, table doesnt exist... fresh install
                    if (DatabaseEngine == DatabaseEngine.Sqlite)
                        Execute(Utils.GetResourceAsString("BF2Statistics.SQL.SQLite.Gamespy.sql"));
                    else
                        Execute(Utils.GetResourceAsString("BF2Statistics.SQL.MySQL.Gamespy.sql"));
                }
            }
            catch (Exception)
            {
                if (Connection != null)
                    Connection.Dispose();

                throw;
            }
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
            var Rows = this.Query("SELECT * FROM accounts WHERE name=@P0", Nick);
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
            var Rows = this.Query("SELECT * FROM accounts WHERE id=@P0", Pid);
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
            var Rows = this.Query("SELECT * FROM accounts WHERE email=@P0 AND password=@P1", Email, Password);
            return (Rows.Count == 0) ? null : Rows[0];
        }

        /// <summary>
        /// Returns a list of player names that are similar to the passed parameter
        /// </summary>
        /// <param name="Nick"></param>
        /// <returns></returns>
        public List<string> GetUsersLike(string Nick)
        {
            // Generate our return list
            List<string> List = new List<string>();
            var Rows = this.Query("SELECT name FROM accounts WHERE name LIKE @P0", "%" + Nick + "%");
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
            var Rows = this.Query("SELECT id FROM accounts WHERE name=@P0", Nick);
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
            var Rows = this.Query("SELECT name FROM accounts WHERE id=@P0", PID);
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
            // Generate PID
            int pid = 1;

            // User doesnt have a PID yet, Get the current max PID and increment
            var Max = this.Query("SELECT MAX(id) AS max FROM accounts");
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
            int Rows = this.Execute("INSERT INTO accounts(id, name, password, email, country) VALUES(@P0, @P1, @P2, @P3, @P4)",
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
            // Make sure the user doesnt exist!
            var PidExists = this.Query("SELECT name FROM accounts WHERE id=@P0", Pid);
            var NameExists = this.Query("SELECT id FROM accounts WHERE name=@P0", Nick);
            if (PidExists.Count == 1)
                throw new Exception("Account ID is already taken!");
            else if(NameExists.Count == 1)
                throw new Exception("Account username is already taken!");

            // Create the user in the database
            int Rows = this.Execute("INSERT INTO accounts(id, name, password, email, country) VALUES(@P0, @P1, @P2, @P3, @P4)",
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
            this.Execute("UPDATE accounts SET country=@P0 WHERE name=@P1", Nick, Country);
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
            this.Execute("UPDATE accounts SET id=@P0, name=@P1, password=@P2, email=@P3 WHERE id=@P4", 
                NewPid, NewNick, NewPassword, NewEmail, Id);
        }

        /// <summary>
        /// Deletes a Gamespy Account
        /// </summary>
        /// <param name="Nick"></param>
        /// <returns></returns>
        public int DeleteUser(string Nick)
        {
            return this.Execute("DELETE FROM accounts WHERE name=@P0", Nick);
        }

        /// <summary>
        /// Deletes a Gamespy Account
        /// </summary>
        /// <param name="Nick"></param>
        /// <returns></returns>
        public int DeleteUser(int Pid)
        {
            return this.Execute("DELETE FROM accounts WHERE id=@P0", Pid);
        }

        /// <summary>
        /// Deletes a Gamespy Account
        /// </summary>
        /// <param name="Nick"></param>
        /// <returns></returns>
        public int GetPID(string Nick)
        {
            var Rows = this.Query("SELECT id FROM accounts WHERE name=@P0", Nick);

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
            bool UserExists = this.Query("SELECT id FROM accounts WHERE name=@P0", Nick).Count != 0;
            bool PidExists = this.Query("SELECT name FROM accounts WHERE id=@P0", Pid).Count != 0;

            // If no user exists, return code -1
            if (UserExists)
                return -1;

            // If PID is false, the PID is not taken
            if (!PidExists)
            {
                int Success = this.Execute("UPDATE accounts SET id=@P0 WHERE name=@P1", Pid, Nick);
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
            List<Dictionary<string, object>> r = this.Query("SELECT COUNT(id) AS count FROM accounts");
            return Int32.Parse(r[0]["count"].ToString());
        }
    }
}
