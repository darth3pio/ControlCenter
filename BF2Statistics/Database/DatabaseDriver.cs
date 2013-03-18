using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
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
        /// Current command running against the database
        /// </summary>
        protected DbCommand Command = null;

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
            get
            {
                return (Connection.State == ConnectionState.Open);
            }
        }

        /// <summary>
        /// Event Fired if the DB connection goes offline
        /// </summary>
        public event StateChangeEventHandler ConnectionClosed;

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
            Connection.Open();
            Connection.StateChange += new StateChangeEventHandler(Connection_StateChange);
        }

        /// <summary>
        /// Event fired when the connection is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Connection_StateChange(object sender, StateChangeEventArgs e)
        {
            if (Connection.State == ConnectionState.Closed)
            {
                try
                {
                    ConnectionClosed(sender, e);
                }
                catch { }
            }
        }

        /// <summary>
        /// Closes the connection to the database
        /// </summary>
        public void Close()
        {
            try {
                Connection.Close();
            }
            catch { }
        }

        /// <summary>
        /// Queries the database, and returns a result set
        /// </summary>
        /// <param name="Sql">The SQL Statement to run on the database</param>
        /// <returns></returns>
        public List<Dictionary<string, object>> Query(string Sql)
        {
            // Create the SQL Command
            this.CreateCommand(Sql);

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
        /// Queries the database, and returns a result set
        /// </summary>
        /// <param name="Sql">The SQL Statement to run on the database</param>
        /// <returns></returns>
        public List<Dictionary<string, object>> Query(string Sql, params object[] Items)
        {
            string Formatted = string.Format(Sql, Items);
            return this.Query(Formatted);
        }

        /// <summary>
        /// Executes a statement on the database (Update, Delete, Insert)
        /// </summary>
        /// <param name="Sql">The SQL statement to be executes</param>
        /// <returns>Returns the number of rows affected by the statement</returns>
        public int Execute(string Sql)
        {
            this.CreateCommand(Sql);
            int Result = Command.ExecuteNonQuery();
            Command.Dispose();

            return Result;
        }

        /// <summary>
        /// Executes a statement on the database (Update, Delete, Insert)
        /// </summary>
        /// <param name="Sql">The SQL statement to be executes</param>
        /// <returns>Returns the number of rows affected by the statement</returns>
        public int Execute(string Sql, params object[] Items)
        {
            string Formatted = string.Format(Sql, Items);
            return this.Execute(Formatted);
        }

        /// <summary>
        /// Inserts a dictionary or ColName => ColValue into a table
        /// </summary>
        /// <param name="Table">The Table Name to insert into</param>
        /// <param name="Items">List of ColName => Value</param>
        /// <returns></returns>
        public int Insert(string Table, Dictionary<string, object> Items)
        {
            char[] trim = new char[] { ',' };
            string Cols = "";
            string Values = "";

            foreach (KeyValuePair<string, object> I in Items)
            {
                Cols += I.Key + ",";
                Values += "'" + I.Value.ToString() + "',";
            }

            // Create out query command, and execute it
            string Query = String.Format("INSERT INTO {0} ({1}) VALUES ({2})", Table, Cols.TrimEnd(trim), Values.TrimEnd(trim));
            return this.Execute(Query);
        }

        /// <summary>
        /// Updates a set of rows in a table
        /// </summary>
        /// <param name="Table">The table name</param>
        /// <param name="Items">A SqlUpdateDictionary of columns => values</param>
        /// <param name="Where">The where statement (Excluding the "WHERE" word</param>
        /// <returns></returns>
        public int Update(string Table, SqlUpdateDictionary Items, string Where)
        {
            char[] trim = new char[] { ',' };
            string Cols = "";

            foreach (KeyValuePair<string, SqlUpdateItem> I in Items)
            {
                if (I.Value.Mode != ValueMode.Set)
                {
                    string Sign = "";
                    switch (I.Value.Mode)
                    {
                        case ValueMode.Add:
                            Sign = "+";
                            break;
                        case ValueMode.Divide:
                            Sign = "/";
                            break;
                        case ValueMode.Multiply:
                            Sign = "*";
                            break;
                        case ValueMode.Subtract:
                            Sign = "-";
                            break;
                    }

                    Cols += String.Format("{0}=`{0}`{1}{2}{3}{2},", I.Key, Sign, ((I.Value.Quote) ? "'" : ""), I.Value.Value.ToString());
                }
                else
                    Cols += String.Format("{0}={1}{2}{1},", I.Key, ((I.Value.Quote) ? "'" : ""), I.Value.Value.ToString());
            }

            string Query = String.Format("UPDATE {0} SET {1} WHERE {2}", Table, Cols.TrimEnd(trim), Where);
            return this.Execute(Query);
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
        /// Used to clean bad characters from a string
        /// </summary>
        /// <param name="QueryString">The string to be cleaned</param>
        /// <returns></returns>
        public static string Escape(string QueryString)
        {
            return QueryString.Replace("\x00", "\\x00").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\\", "\\\\").Replace("'", "\\'").Replace("\"", "\\\"").Replace("\x1A", "\\x1A");
        }

        /// <summary>
        /// Creates a new command to be executed on the database
        /// </summary>
        /// <param name="QueryString"></param>
        protected void CreateCommand(string QueryString)
        {
            if (DatabaseEngine == Database.DatabaseEngine.Sqlite)
                Command = new SQLiteCommand(QueryString, Connection as SQLiteConnection);
            else if (DatabaseEngine == Database.DatabaseEngine.Mysql)
                Command = new MySqlCommand(QueryString, Connection as MySqlConnection);
        }

        /// <summary>
        /// Converts a database string name to a DatabaseEngine type.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static DatabaseEngine GetDatabaseEngine(string Name)
        {
            Type EnumType = typeof(Database.DatabaseEngine);
            return ((Database.DatabaseEngine)Enum.Parse(EnumType, Name, true));
        }
    }
}
