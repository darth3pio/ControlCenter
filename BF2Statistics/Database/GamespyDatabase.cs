using System;
using System.Collections.Generic;
using System.Data.Common;
using BF2Statistics.ASP;
using BF2Statistics.Database.QueryBuilder;

namespace BF2Statistics.Database
{
    /// <summary>
    /// A class to provide common tasks against the Gamespy Login Database
    /// </summary>
    public class GamespyDatabase : DatabaseDriver, IDisposable
    {
        /// <summary>
        /// Indicates the most up to date database table version
        /// </summary>
        public static readonly Version LatestVersion = new Version(2, 1);

        /// <summary>
        /// Indicates the current Database tables version
        /// </summary>
        public Version Version { get; protected set; }

        /// <summary>
        /// Indicates whether the SQL tables exist in this database
        /// </summary>
        public bool TablesExist => Version.Major > 0;

        /// <summary>
        /// Indicates whether the user should be notified to update the database
        /// </summary>
        public bool NeedsUpdated => Version.CompareTo(LatestVersion) < 0;

        /// <summary>
        /// Creates a new connection to the Gamespy Database
        /// </summary>
        public GamespyDatabase() : base(Program.Config.GamespyDBEngine, Program.Config.GamespyDBConnectionString)
        {
            // Try and Reconnect
            try
            {
                Connect();

                // Try and get database version
                try
                {
                    // Correct the version string from older database installs
                    string ver = base.ExecuteScalar("SELECT dbver FROM _version LIMIT 1").ToString();
                    if (!ver.Contains(".")) ver += ".0";

                    // Set internals
                    Version = Version.Parse(ver);
                }
                catch
                {
                    // Table doesnt contain a _version table, then we arent installed
                    Version = new Version(0, 0);
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
        /// <param name="Email">The Account email</param>
        /// <param name="Password">the MD5 HASHED Account Password</param>
        /// <returns></returns>
        public List<Dictionary<string, object>> GetUsersByEmailPass(string Email, string Password)
        {
            return base.Query("SELECT * FROM accounts WHERE email=@P0 AND password=@P1", Email.ToLowerInvariant(), Password);
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
        /// <param name="Pass">The UN-HASHED Account Password</param>
        /// <param name="Email">The Account Email Address</param>
        /// <param name="Country">The Country Code for this Account</param>
        /// <returns>Returns the Player ID if sucessful, 0 otherwise</returns>
        public int CreateUser(string Nick, string Pass, string Email, string Country)
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
                    Pid = (Row.Count == 0) ? StatsManager.GenerateNewPlayerPid() : Int32.Parse(Row[0]["id"].ToString());
                }
            }
            catch
            {
                Pid = StatsManager.GenerateNewPlayerPid();
            }

            // Create the user in the database
            int Rows = base.Execute("INSERT INTO accounts(id, name, password, email, country) VALUES(@P0, @P1, @P2, @P3, @P4)",
                Pid, Nick, Pass.GetMD5Hash(false), Email.ToLowerInvariant(), Country
            );

            return (Rows != 0) ? Pid : 0;
        }

        /// <summary>
        /// Creates a new Gamespy Account
        /// </summary>
        /// <remarks>Only used in the Gamespy Account Creation Form</remarks>
        /// <param name="Pid">The Profile Id to assign this account</param>
        /// <param name="Nick">The Account Name</param>
        /// <param name="Pass">The UN-HASHED Account Password</param>
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
                Pid, Nick, Pass.GetMD5Hash(false), Email.ToLowerInvariant(), Country
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
        /// <param name="NewPassword">New Account Password, UN HASHED. Leave empty to not set a new password</param>
        /// <param name="NewEmail">New Account Email Address</param>
        public void UpdateUser(int Id, int NewPid, string NewNick, string NewPassword, string NewEmail)
        {
            UpdateQueryBuilder Query = new UpdateQueryBuilder("accounts", this);
            Query.SetField("id", NewPid);
            Query.SetField("name", NewNick);
            Query.SetField("email", NewEmail.ToLowerInvariant());
            Query.AddWhere("id", Comparison.Equals, Id);

            // Set new password if not empty
            if (!String.IsNullOrWhiteSpace(NewPassword))
                Query.SetField("password", NewPassword.GetMD5Hash(false));

            Query.Execute();
        }

        /// <summary>
        /// Deletes a Gamespy Account
        /// </summary>
        /// <param name="Nick">The Player's Nick, a.k.a Account name</param>
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
        public int GetPlayerId(string Nick)
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
        public int SetPlayerId(string Nick, int Pid)
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

        /// <summary>
        /// Tells the Database to install the Stats tables into the database
        /// </summary>
        public void CreateSqlTables()
        {
            if (TablesExist)
                return;

            // If an exception is thrown, table doesnt exist... fresh install
            if (DatabaseEngine == DatabaseEngine.Sqlite)
                base.Execute(Program.GetResourceAsString("BF2Statistics.SQL.SQLite.Gamespy.sql"));
            else
                base.Execute(Program.GetResourceAsString("BF2Statistics.SQL.MySQL.Gamespy.sql"));
        }

        /// <summary>
        /// If there is any table updates that need to be applied, calling this method will apply
        /// each update until the current database version is up to date
        /// </summary>
        public void MigrateTables()
        {
            // If we dont need updated, what are we doing here?
            if (!NeedsUpdated) return;

            // MD5 Hash Update, only update to date so i will make this better in the future
            if(Version.Major == 2 && Version.Minor == 0)
            {
                // In this update, we hash all of the passwords to MD5!
                using (DbTransaction Transaction = base.BeginTransaction())
                {
                    try
                    {
                        // Set the column length to 32, since this is the MD5 hash length
                        if (base.DatabaseEngine == DatabaseEngine.Mysql)
                            base.Execute("ALTER TABLE accounts MODIFY COLUMN password VARCHAR(32) NOT NULL");

                        // Here we use the QueryReader method to pull 1 row at a time, using memory efficiently
                        foreach (Dictionary<string, object> Row in base.Query("SELECT id, password FROM accounts"))
                        {
                            base.Execute("UPDATE accounts SET password=@P0 WHERE id=@P1",
                                Row["password"].ToString().GetMD5Hash(false),
                                Row["id"]
                            );
                        }

                        // Delete old version data
                        base.Execute("DELETE FROM _version");

                        // Run the alter table command on MySql, SQLite does not support this
                        if (base.DatabaseEngine == DatabaseEngine.Mysql)
                        {
                            base.Execute("ALTER TABLE _version MODIFY dbver VARCHAR(4)");
                        }
                        else
                        {
                            base.Execute("DROP TABLE IF EXISTS \"main\".\"_version\"");
                            base.Execute("CREATE TABLE \"main\".\"_version\"(\"dbver\" TEXT NOT NULL DEFAULT 0, \"dbdate\" INT NOT NULL DEFAULT 0, PRIMARY KEY (\"dbver\"));");
                        }

                        // Insert New Data
                        base.Execute("INSERT INTO _version(dbver, dbdate) VALUES (@P0, @P1)", "2.1", DateTime.UtcNow.ToUnixTimestamp());
                        Transaction.Commit();

                        // Set new version
                        Version = new Version(2, 1);
                    }
                    catch
                    {
                        Transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
