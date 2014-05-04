using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BF2Statistics.Database
{
    /// <summary>
    /// A class to provide common tasks against the Gamespy Login Database
    /// </summary>
    public class GamespyDatabase : DatabaseDriver, IDisposable
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
                    if (base.Query("SELECT dbver FROM _version LIMIT 1").Count == 0)
                        throw new Exception();
                }
                catch
                {
                    // If an exception is thrown, table doesnt exist... fresh install
                    if (DatabaseEngine == DatabaseEngine.Sqlite)
                        base.Execute(Utils.GetResourceAsString("BF2Statistics.SQL.SQLite.Gamespy.sql"));
                    else
                        base.Execute(Utils.GetResourceAsString("BF2Statistics.SQL.MySQL.Gamespy.sql"));
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
            if (!IsDisposed)
                base.Dispose();
        }

        /// <summary>
        /// Fetches an account from the gamespy database
        /// </summary>
        /// <param name="Nick">The user's Nick</param>
        /// <returns></returns>
        public Dictionary<string, object> GetUser(string Nick)
        {
            // Fetch the user
            var Rows = base.Query("SELECT * FROM accounts WHERE name=@P0", Nick);
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
            var Rows = base.Query("SELECT * FROM accounts WHERE id=@P0", Pid);
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
            var Rows = base.Query("SELECT * FROM accounts WHERE email=@P0 AND password=@P1", Email, Password);
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
            var Rows = base.Query("SELECT name FROM accounts WHERE name LIKE @P0", "%" + Nick + "%");
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
            return (base.Query("SELECT id FROM accounts WHERE name=@P0", Nick).Count != 0);
        }

        /// <summary>
        /// Returns wether an account exists from the provided Account Id
        /// </summary>
        /// <param name="PID"></param>
        /// <returns></returns>
        public bool UserExists(int PID)
        {
            // Fetch the user
            return (base.Query("SELECT name FROM accounts WHERE id=@P0", PID).Count != 0);
        }

        /// <summary>
        /// Creates a new Gamespy Account
        /// </summary>
        /// <remarks>Used by the login server when a create account request is made</remarks>
        /// <param name="Nick">The Account Name</param>
        /// <param name="Pass">The Account Password</param>
        /// <param name="Email">The Account Email Address</param>
        /// <param name="Country">The Country Code for this Account</param>
        /// <returns>A bool indicating whether the account was created sucessfully</returns>
        public bool CreateUser(string Nick, string Pass, string Email, string Country)
        {
            int Pid = 0;

            // Attempt to connect to stats database, and get a PID from there
            try
            {
                // try see if the player ID exists in the stats database
                using (StatsDatabase Db = new StatsDatabase())
                {
                    // NOTE: online account names in the stats DB start with a single space!
                    var Row = Db.Query("SELECT id FROM player WHERE upper(name) = upper(@P0)", " " + Nick);
                    Pid = (Row.Count == 0) ? GenerateAccountId() : Int32.Parse(Row[0]["id"].ToString());
                }
            }
            catch
            {
                Pid = GenerateAccountId();
            }

            // Create the user in the database
            int Rows = base.Execute("INSERT INTO accounts(id, name, password, email, country) VALUES(@P0, @P1, @P2, @P3, @P4)",
                Pid, Nick, Pass, Email, Country
            );

            return (Rows != 0);
        }

        /// <summary>
        /// Generates a new Account Id
        /// </summary>
        /// <returns></returns>
        private int GenerateAccountId()
        {
            var Row = base.Query("SELECT COALESCE(MAX(id), 500000000) AS max FROM accounts");
            int max = Int32.Parse(Row[0]["max"].ToString()) + 1;
            return (max < 500000000) ? 500000000 : max;
        }

        /// <summary>
        /// Creates a new Gamespy Account
        /// </summary>
        /// <remarks>Only used in the Gamespy Account Creation Form</remarks>
        /// <param name="Pid">The Profile Id to assign this account</param>
        /// <param name="Nick">The Account Name</param>
        /// <param name="Pass">The Account Password</param>
        /// <param name="Email">The Account Email Address</param>
        /// <param name="Country">The Country Code for this Account</param>
        /// <returns>A bool indicating whether the account was created sucessfully</returns>
        public bool CreateUser(int Pid, string Nick, string Pass, string Email, string Country)
        {
            // Make sure the user doesnt exist!
            if (UserExists(Pid))
                throw new Exception("Account ID is already taken!");
            else if(UserExists(Nick))
                throw new Exception("Account username is already taken!");

            // Create the user in the database
            int Rows = base.Execute("INSERT INTO accounts(id, name, password, email, country) VALUES(@P0, @P1, @P2, @P3, @P4)",
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
            base.Execute("UPDATE accounts SET country=@P0 WHERE name=@P1", Nick, Country);
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
            base.Execute("UPDATE accounts SET id=@P0, name=@P1, password=@P2, email=@P3 WHERE id=@P4", 
                NewPid, NewNick, NewPassword, NewEmail, Id);
        }

        /// <summary>
        /// Deletes a Gamespy Account
        /// </summary>
        /// <param name="Nick"></param>
        /// <returns></returns>
        public int DeleteUser(string Nick)
        {
            return base.Execute("DELETE FROM accounts WHERE name=@P0", Nick);
        }

        /// <summary>
        /// Deletes a Gamespy Account
        /// </summary>
        /// <param name="Nick"></param>
        /// <returns></returns>
        public int DeleteUser(int Pid)
        {
            return base.Execute("DELETE FROM accounts WHERE id=@P0", Pid);
        }

        /// <summary>
        /// Fetches a Gamespy Account id from an account name
        /// </summary>
        /// <param name="Nick"></param>
        /// <returns></returns>
        public int GetPID(string Nick)
        {
            var Rows = base.Query("SELECT id FROM accounts WHERE name=@P0", Nick);
            return (Rows.Count == 0) ? 0 : Int32.Parse(Rows[0]["id"].ToString());
        }

        /// <summary>
        /// Sets the Account (Player) Id for an account by Name
        /// </summary>
        /// <param name="Nick">The account Nick we are setting the new Pid for</param>
        /// <param name="Pid">The new Pid</param>
        /// <returns></returns>
        public int SetPID(string Nick, int Pid)
        {
            // If no user exists, return code -1
            if (!UserExists(Nick))
                return -1;

            // If the Pid already exists, return -2
            if (UserExists(Pid))
                return -2;

            // If PID is false, the PID is not taken
            int Success = base.Execute("UPDATE accounts SET id=@P0 WHERE name=@P1", Pid, Nick);
            return (Success > 0) ? 1 : 0;
        }

        /// <summary>
        /// Returns the number of accounts in the database
        /// </summary>
        /// <returns></returns>
        public int GetNumAccounts()
        {
            var Row = base.Query("SELECT COUNT(id) AS count FROM accounts");
            return Int32.Parse(Row[0]["count"].ToString());
        }
    }
}
