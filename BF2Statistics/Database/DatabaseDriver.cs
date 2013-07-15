using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using MySql;
using MySql.Data.Common;
using MySql.Data.MySqlClient;

namespace BF2Statistics.Database
{
    public class DatabaseDriver
    {
        /// <summary>
        /// Current DB Engine
        /// </summary>
        public DatabaseEngine DatabaseEngine { get; protected set; }

        /// <summary>
        /// The database connection
        /// </summary>
        protected DbConnection Connection = null;

        /// <summary>
        /// Only applies to SQLite databases, used to determine whether or not
        /// the specified file already existed prior to attempting the connection.
        /// </summary>
        public bool IsNewDatabase { get; protected set; }

        /// <summary>
        /// Returns whether the Database connection is open
        /// </summary>
        public bool IsConnected
        {
            get { return (Connection.State == ConnectionState.Open); }
        }

        /// <summary>
        /// Event Fired if the DB connection goes offline
        /// </summary>
        public event StateChangeEventHandler ConnectionClosed;

        /// <summary>
        /// Event Fired if the DB connection is established
        /// </summary>
        public event StateChangeEventHandler ConnectionEstablshed;

        /// <summary>
        /// Random, yes... But its used here when building queries dynamically
        /// </summary>
        protected static char Comma = ',';

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Engine">The string name, from the GetDatabaseEngine() method</param>
        /// <param name="Host">The Database server IP Address</param>
        /// <param name="Port">The Database server Port Number</param>
        /// <param name="DatabaseName">The name of the database</param>
        /// <param name="User">A username, with database privliages</param>
        /// <param name="Pass">The password to the User</param>
        public DatabaseDriver(string Engine, string Host, int Port, string DatabaseName, string User, string Pass)
        {
            // Set class variables, and create a new connection builder
            this.DatabaseEngine = GetDatabaseEngine(Engine);
            DbConnectionStringBuilder Builder;

            if (this.DatabaseEngine == DatabaseEngine.Sqlite)
            {
                string FullPath = Path.Combine(MainForm.Root, DatabaseName + ".sqlite3");
                IsNewDatabase = !File.Exists(FullPath) || new FileInfo(FullPath).Length == 0;
                Builder = new SQLiteConnectionStringBuilder();
                Builder.Add("Data Source", FullPath);
                Connection = new SQLiteConnection(Builder.ConnectionString);
            }
            else if (this.DatabaseEngine == DatabaseEngine.Mysql)
            {
                IsNewDatabase = false;
                Builder = new MySqlConnectionStringBuilder();
                Builder.Add("Server", Host);
                Builder.Add("Port", Port);
                Builder.Add("User ID", User);
                Builder.Add("Password", Pass);
                Builder.Add("Database", DatabaseName);
                Connection = new MySqlConnection(Builder.ConnectionString);
            }
            else
            {
                throw new Exception("Invalid Database type.");
            }
        }

        /// <summary>
        /// Opens the database connection
        /// </summary>
        public void Connect()
        {
            Connection.StateChange += new StateChangeEventHandler(Connection_StateChange);
            Connection.Open();
        }

        /// <summary>
        /// Closes the connection to the database
        /// </summary>
        public void Close()
        {
            try
            {
                Connection.Close();

                // Yes, this below the close is intentional
                Connection.StateChange -= new StateChangeEventHandler(Connection_StateChange);
            }
            catch (ObjectDisposedException) { }
        }

        /// <summary>
        /// Creates a new command to be executed on the database
        /// </summary>
        /// <param name="QueryString"></param>
        public DbCommand CreateCommand(string QueryString)
        {
            if (DatabaseEngine == Database.DatabaseEngine.Sqlite)
                return new SQLiteCommand(QueryString, Connection as SQLiteConnection);
            else
                return new MySqlCommand(QueryString, Connection as MySqlConnection);
        }

        /// <summary>
        /// Creates a DbParameter using the current Database engine's Parameter object
        /// </summary>
        /// <returns></returns>
        public DbParameter CreateParam()
        {
            if (DatabaseEngine == Database.DatabaseEngine.Sqlite)
                return (new SQLiteParameter() as DbParameter);
            else
                return (new MySqlParameter() as DbParameter);
        }

        /// <summary>
        /// Queries the database, and returns a result set
        /// </summary>
        /// <param name="Sql">The SQL Statement to run on the database</param>
        /// <returns></returns>
        public List<Dictionary<string, object>> Query(string Sql)
        {
            return this.Query(Sql, new List<DbParameter>());
        }

        /// <summary>
        /// Queries the database, and returns a result set
        /// </summary>
        /// <param name="Sql">The SQL Statement to run on the database</param>
        /// <param name="Items">Additional parameters are parameter values for the query.
        /// The first parameter replaces @P0, second @P1 etc etc.
        /// </param>
        /// <returns></returns>
        public List<Dictionary<string, object>> Query(string Sql, params object[] Items)
        {
            List<DbParameter> Params = new List<DbParameter>(Items.Length);
            for (int i = 0; i < Items.Length; i++)
            {
                DbParameter Param = this.CreateParam();
                Param.ParameterName = "@P" + i;
                Param.Value = Items[i];
                Params.Add(Param);
            }

            return this.Query(Sql, Params);
        }

        /// <summary>
        /// Queries the database, and returns a result set
        /// </summary>
        /// <param name="Sql">The SQL Statement to run on the database</param>
        /// <param name="Params">A list of sql params to add to the command</param>
        /// <returns></returns>
        public List<Dictionary<string, object>> Query(string Sql, List<DbParameter> Params)
        {
            // Create the SQL Command
            DbCommand Command = this.CreateCommand(Sql);

            // Add params
            foreach (DbParameter Param in Params)
                Command.Parameters.Add(Param);

            // Execute the query
            List<Dictionary<string, object>> Rows = new List<Dictionary<string, object>>();
            DbDataReader Reader = Command.ExecuteReader();

            // If we have rows, add them to the list
            if (Reader.HasRows)
            {
                // Add each row to the rows list
                while (Reader.Read())
                {
                    Dictionary<string, object> Row = new Dictionary<string, object>(Reader.FieldCount);
                    for (int i = 0; i < Reader.FieldCount; ++i)
                        Row.Add(Reader.GetName(i), Reader.GetValue(i));

                    Rows.Add(Row);
                }
            }

            // Cleanup
            Reader.Close();
            Reader.Dispose();
            Command.Dispose();

            // Return Rows
            return Rows;
        }

        /// <summary>
        /// Queries the database, and returns 1 row at a time until all rows are returned
        /// </summary>
        /// <param name="Sql">The SQL Statement to run on the database</param>
        /// <returns></returns>
        public IEnumerable<Dictionary<string, object>> QueryReader(string Sql)
        {
            // Create the SQL Command
            DbCommand Command = this.CreateCommand(Sql);

            // Execute the query
            DbDataReader Reader = Command.ExecuteReader();

            // If we have rows, add them to the list
            if (Reader.HasRows)
            {
                // Add each row to the rows list
                while (Reader.Read())
                {
                    Dictionary<string, object> Row = new Dictionary<string, object>(Reader.FieldCount);
                    for (int i = 0; i < Reader.FieldCount; ++i)
                        Row.Add(Reader.GetName(i), Reader.GetValue(i));

                    yield return Row;
                }
            }

            // Cleanup
            Reader.Close();
            Reader.Dispose();
            Command.Dispose();
        }

        /// <summary>
        /// Executes a command, and returns 1 row at a time until all rows are returned
        /// </summary>
        /// <param name="Command">The database command to execute the reader on</param>
        /// <returns></returns>
        public IEnumerable<Dictionary<string, object>> QueryReader(DbCommand Command)
        {
            // Execute the query
            DbDataReader Reader = Command.ExecuteReader();

            // If we have rows, add them to the list
            if (Reader.HasRows)
            {
                // Add each row to the rows list
                while (Reader.Read())
                {
                    Dictionary<string, object> Row = new Dictionary<string, object>(Reader.FieldCount);
                    for (int i = 0; i < Reader.FieldCount; ++i)
                        Row.Add(Reader.GetName(i), Reader.GetValue(i));

                    yield return Row;
                }
            }

            // Cleanup
            Reader.Close();
            Reader.Dispose();
            Command.Dispose();
        }


        /// <summary>
        /// Executes a command, and returns the resulting rows
        /// </summary>
        /// <param name="Command">The database command to execute the reader on</param>
        /// <returns></returns>
        public List<Dictionary<string, object>> ExecuteReader(DbCommand Command)
        {
            // Execute the query
            List<Dictionary<string, object>> Rows = new List<Dictionary<string, object>>();
            DbDataReader Reader = Command.ExecuteReader();

            // If we have rows, add them to the list
            if (Reader.HasRows)
            {
                // Add each row to the rows list
                while (Reader.Read())
                {
                    Dictionary<string, object> Row = new Dictionary<string, object>(Reader.FieldCount);
                    for (int i = 0; i < Reader.FieldCount; ++i)
                        Row.Add(Reader.GetName(i), Reader.GetValue(i));

                    Rows.Add(Row);
                }
            }

            // Cleanup
            Reader.Close();
            Reader.Dispose();
            Command.Dispose();

            // Return Rows
            return Rows;
        }

        /// <summary>
        /// Executes a statement on the database (Update, Delete, Insert)
        /// </summary>
        /// <param name="Sql">The SQL statement to be executes</param>
        /// <returns>Returns the number of rows affected by the statement</returns>
        public int Execute(string Sql)
        {
            // Create the SQL Command
            DbCommand Command = this.CreateCommand(Sql);

            // Execute command, and dispose of the command
            int Result = Command.ExecuteNonQuery();
            Command.Dispose();

            return Result;
        }

        /// <summary>
        /// Executes a statement on the database (Update, Delete, Insert)
        /// </summary>
        /// <param name="Sql">The SQL statement to be executes</param>
        /// <param name="Params">A list of Sqlparameters</param>
        /// <returns>Returns the number of rows affected by the statement</returns>
        public int Execute(string Sql, List<DbParameter> Params)
        {
            // Create the SQL Command
            DbCommand Command = this.CreateCommand(Sql);

            // Add params
            foreach (DbParameter Param in Params)
                Command.Parameters.Add(Param);

            // Execute command, and dispose of the command
            int Result = Command.ExecuteNonQuery();
            Command.Dispose();

            return Result;
        }

        /// <summary>
        /// Executes a statement on the database (Update, Delete, Insert)
        /// </summary>
        /// <param name="Sql">The SQL statement to be executes</param>
        /// <param name="Items">Additional parameters are parameter values for the query.
        /// The first parameter replaces @P0, second @P1 etc etc.
        /// </param>
        /// <returns>Returns the number of rows affected by the statement</returns>
        public int Execute(string Sql, params object[] Items)
        {
            // Create the SQL Command
            DbCommand Command = this.CreateCommand(Sql);

            // Add params
            for (int i = 0; i < Items.Length; i++)
            {
                DbParameter Param = this.CreateParam();
                Param.ParameterName = "@P" + i;
                Param.Value = Items[i];
                Command.Parameters.Add(Param);
            }

            // Execute command, and dispose of the command
            int Result = Command.ExecuteNonQuery();
            Command.Dispose();

            return Result;
        }

        /// <summary>
        /// Begins a new database transaction
        /// </summary>
        /// <returns></returns>
        public DbTransaction BeginTransaction()
        {
            return Connection.BeginTransaction();
        }

        /// <summary>
        /// Begins a new database transaction
        /// </summary>
        /// <param name="Level"></param>
        /// <returns></returns>
        public DbTransaction BeginTransaction(IsolationLevel Level)
        {
            return Connection.BeginTransaction(Level);
        }

        /// <summary>
        /// Converts a database string name to a DatabaseEngine type.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static DatabaseEngine GetDatabaseEngine(string Name)
        {
            return ((DatabaseEngine)Enum.Parse(typeof(DatabaseEngine), Name, true));
        }

        /// <summary>
        /// Event fired when the connection is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connection_StateChange(object sender, StateChangeEventArgs e)
        {
            switch (Connection.State)
            {
                case ConnectionState.Closed:
                    if (ConnectionClosed != null)
                        ConnectionClosed(sender, e);
                    break;
                case ConnectionState.Open:
                    if (ConnectionEstablshed != null)
                        ConnectionEstablshed(sender, e);
                    break;
            }
        }
    }
}
