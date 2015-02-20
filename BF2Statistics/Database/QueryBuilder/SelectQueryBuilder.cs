using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace BF2Statistics.Database.QueryBuilder
{
    class SelectQueryBuilder
    {
        #region Internal Properties

        protected List<string> _selectedColumns = new List<string>();
        protected List<string> _selectedTables = new List<string>();
        protected List<OrderByClause> OrderByStatements = new List<OrderByClause>();
        protected List<JoinClause> Joins = new List<JoinClause>();
        protected List<string> GroupByColumns = new List<string>();
        protected int[] LimitRecords = null;
        protected DatabaseDriver Driver;

        #endregion

        #region Public Properties

        public static readonly char[] CommaSpace = new char[] { ',', ' ' };

        /// <summary>
        /// Gets or Sets whether this Select statement will be distinct
        /// </summary>
        public bool Distinct = false;

        /// <summary>
        /// The selected columns for this query. We convert to an array,
        /// which un-references the original list, and prevents modifications
        /// </summary>
        public string[] SelectedColumns
        {
            get
            {
                return (_selectedColumns.Count > 0) ? _selectedColumns.ToArray() : new string[1] { "*" };
            }
        }

        /// <summary>
        /// The selected tables for this query. We convert to an array,
        /// which un-references the original list, and prevents modifications
        /// </summary>
        public string[] SelectedTables
        {
            get { return this._selectedTables.ToArray(); }
        }

        /// <summary>
        /// The Where statement for this query
        /// </summary>
        public WhereStatement WhereStatement = new WhereStatement();

        /// <summary>
        /// The Having statement for this query
        /// </summary>
        public WhereStatement HavingStatement = new WhereStatement();

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public SelectQueryBuilder() { }

        /// <summary>
        /// Sets the database driver
        /// </summary>
        /// <param name="factory"></param>
        public SelectQueryBuilder(DatabaseDriver Driver)
        {
            this.Driver = Driver;
        }

        /// <summary>
        /// Sets the database driver
        /// </summary>
        /// <param name="Driver"></param>
        public void SetDbDriver(DatabaseDriver Driver)
        {
            this.Driver = Driver;
        }

        #region Select Cols

        /// <summary>
        /// Selects all columns in the SQL Statement being built
        /// </summary>
        public void SelectAllColumns()
        {
            this._selectedColumns.Clear();
        }

        /// <summary>
        /// Selects the count of rows in the SQL Statement being built
        /// </summary>
        public void SelectCount()
        {
            this.SelectColumn("COUNT(1) as count");
        }

        /// <summary>
        /// Selects the distinct count of rows in the SQL Statement being built
        /// </summary>
        /// <param name="ColumnName">The Distinct column name</param>
        public void SelectDistinctCount(string ColumnName)
        {
            this.SelectColumn("COUNT(DISTINCT(" + ColumnName + ")) as count");
        }

        /// <summary>
        /// Selects a specified column in the SQL Statement being built. Calling this method
        /// clears all previous selected columns
        /// </summary>
        /// <param name="column">The Column name to select</param>
        public void SelectColumn(string column)
        {
            this._selectedColumns.Clear();
            this._selectedColumns.Add(column);
        }

        /// <summary>
        /// Selects the specified columns in the SQL Statement being built. Calling this method
        /// clears all previous selected columns
        /// </summary>
        /// <param name="columns">The column names to select</param>
        public void SelectColumns(params string[] columns)
        {
            this._selectedColumns.Clear();
            foreach (string str in columns)
                this._selectedColumns.Add(str);
        }

        #endregion Select Cols

        #region Select From


        /// <summary>
        /// Sets the table name to be used in this SQL Statement
        /// </summary>
        /// <param name="table">The table name</param>
        public void SelectFromTable(string table)
        {
            this._selectedTables.Clear();
            this._selectedTables.Add(table);
        }

        /// <summary>
        /// Sets the table names to be used in this SQL Statement
        /// </summary>
        /// <param name="tables">Each param passed is another table name</param>
        public void SelectFromTables(params string[] tables)
        {
            this._selectedTables.Clear();
            foreach (string str in tables)
                this._selectedTables.Add(str);
        }

        #endregion Select From

        #region Joins

        /// <summary>
        /// Adds a join clause to the current query object
        /// </summary>
        /// <param name="newJoin"></param>
        public void AddJoin(JoinClause newJoin)
        {
            this.Joins.Add(newJoin);
        }

        /// <summary>
        /// Creates a new Join clause statement fot the current query object
        /// </summary>
        /// <param name="join"></param>
        /// <param name="toTableName"></param>
        /// <param name="toColumnName"></param>
        /// <param name="operator"></param>
        /// <param name="fromTableName"></param>
        /// <param name="fromColumnName"></param>
        public void AddJoin(JoinType join, string toTableName, string toColumnName, Comparison @operator, string fromTableName, string fromColumnName)
        {
            this.Joins.Add(new JoinClause(join, toTableName, toColumnName, @operator, fromTableName, fromColumnName));
        }

        #endregion Joins

        #region Wheres

        /// <summary>
        /// Creates a where clause to add to the query's where statement
        /// </summary>
        /// <param name="field"></param>
        /// <param name="operator"></param>
        /// <param name="compareValue"></param>
        /// <returns></returns>
        public WhereClause AddWhere(string field, Comparison @operator, object compareValue)
        {
            WhereClause Clause = new WhereClause(field, @operator, compareValue);
            this.WhereStatement.Add(Clause);
            return Clause;
        }

        /// <summary>
        /// Adds a where clause to the current query statement
        /// </summary>
        /// <param name="Clause"></param>
        public void AddWhere(WhereClause Clause)
        {
            this.WhereStatement.Add(Clause);
        }

        /// <summary>
        /// Sets the Logic Operator for the WHERE statement
        /// </summary>
        /// <param name="Operator"></param>
        public void SetWhereOperator(LogicOperator @Operator)
        {
            this.WhereStatement.StatementOperator = @Operator;
        }

        #endregion Wheres

        #region Orderby

        /// <summary>
        /// Adds an OrderBy clause to the current query object
        /// </summary>
        /// <param name="Clause"></param>
        public void AddOrderBy(OrderByClause Clause)
        {
            OrderByStatements.Add(Clause);
        }

        /// <summary>
        /// Creates and adds a new Oderby clause to the current query object
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="Order"></param>
        public void AddOrderBy(string FieldName, Sorting Order)
        {
            OrderByStatements.Add(new OrderByClause(FieldName, Order));
        }


        #endregion Orderby

        #region Having

        public WhereClause AddHaving(string field, Comparison @operator, object compareValue)
        {
            WhereClause Clause = new WhereClause(field, @operator, compareValue);
            this.HavingStatement.Add(Clause);
            return Clause;
        }

        public void AddHaving(WhereClause Clause)
        {
            this.HavingStatement.Add(Clause);
        }

        /// <summary>
        /// Sets the Logic Operator for the WHERE statement
        /// </summary>
        /// <param name="Operator"></param>
        public void SetHavingOperator(LogicOperator @Operator)
        {
            this.HavingStatement.StatementOperator = @Operator;
        }

        #endregion Having

        /// <summary>
        /// Limit is used to limit your query results to those that fall within a specified range
        /// </summary>
        /// <param name="Records">The number if rows to be returned in the result set</param>
        public void Limit(int Records)
        {
            this.LimitRecords = new int[] { Records };
        }

        /// <summary>
        /// Limit is used to limit your query results to those that fall within a specified range
        /// </summary>
        /// <param name="Records">The number if rows to be returned in the result set</param>
        /// <param name="Start">The starting point or record (remember the first record is 0)</param>
        public void Limit(int Records, int Start)
        {
            this.LimitRecords = new int[] { Records, Start };
        }

        /// <summary>
        /// Builds the query string with the current SQL Statement, and returns
        /// the querystring.
        /// </summary>
        /// <returns></returns>
        public string BuildQuery()
        {
            return BuildQuery(false) as String;
        }

        /// <summary>
        /// Builds the query string with the current SQL Statement, and
        /// returns the DbCommand to be executed
        /// </summary>
        /// <returns></returns>
        public DbCommand BuildCommand()
        {
            return BuildQuery(true) as DbCommand;
        }

        /// <summary>
        /// Builds the query string or DbCommand
        /// </summary>
        /// <param name="BuildCommand"></param>
        /// <returns></returns>
        protected object BuildQuery(bool BuildCommand)
        {
            // Make sure we have a valid DB driver
            if (BuildCommand && Driver == null)
                throw new Exception("Cannot build a command when the Db Drvier hasn't been specified. Call SetDbDriver first.");

            // Create Command
            DbCommand Command = (BuildCommand) ? Driver.CreateCommand(null) : null;

            // Start Query
            StringBuilder Query = new StringBuilder("SELECT ");
            if (Distinct)
                Query.Append("DISTINCT ");

            // Append columns
            Query.Append(String.Join(", ", SelectedColumns).TrimEnd(CommaSpace));

            // Append Tables
            Query.Append(" FROM " + String.Join(", ", SelectedTables).TrimEnd(CommaSpace));

            // Append Joins
            if (Joins.Count > 0)
            {
                foreach (JoinClause Clause in Joins)
                {
                    // Convert join type to string
                    switch (Clause.JoinType)
                    {
                        case JoinType.InnerJoin:
                            Query.Append(" INNER JOIN ");
                            break;
                        case JoinType.OuterJoin:
                            Query.Append(" OUTER JOIN ");
                            break;
                        case JoinType.LeftJoin:
                            Query.Append(" LEFT JOIN ");
                            break;
                        case JoinType.RightJoin:
                            Query.Append(" RIGHT JOIN ");
                            break;
                    }

                    // Append the join statement
                    Query.Append(
                        Clause.ToTable + " ON " + 
                        WhereStatement.CreateComparisonClause(
                            Clause.FromTable + "." + Clause.FromColumn,
                            Clause.ComparisonOperator,
                            new SqlLiteral(Clause.ToTable + "." + Clause.ToColumn) as object
                        )
                    );
                }
            }


            // Append Where
            if(this.WhereStatement.Count != 0)
                Query.Append(" WHERE " + this.WhereStatement.BuildStatement(BuildCommand, ref Command));

            // Append GroupBy
            if (GroupByColumns.Count > 0)
                Query.Append(" GROUP BY " + String.Join(", ", GroupByColumns).TrimEnd(CommaSpace));

            // Append Having
            if (HavingStatement.Count > 0)
            {
                if (GroupByColumns.Count == 0)
                    throw new Exception("Having statement was set without Group By");

                Query.Append(" HAVING " + this.WhereStatement.BuildStatement(BuildCommand, ref Command));
            }

            // Append OrderBy
            if (OrderByStatements.Count > 0)
            {
                string Running = " ORDER BY ";
                foreach (OrderByClause Clause in OrderByStatements)
                    Running = String.Concat(Running, Clause.FieldName, ((Clause.SortOrder == Sorting.Ascending) ? " ASC, " : " DESC, "));
                
                // Add Running query
                Query.Append(Running.TrimEnd(CommaSpace));
            }

            // Append Limit
            if (LimitRecords is Array)
            {
                if (LimitRecords.Length == 1)
                    Query.Append(" LIMIT " + LimitRecords[0].ToString());
                else
                    Query.Append(" LIMIT " + LimitRecords[1].ToString() + ", " + LimitRecords[0].ToString());
            }

            // Set the command text
            if(BuildCommand)
                Command.CommandText = Query.ToString();

            // Return Result
            return (BuildCommand) ? Command as object : Query.ToString();
        }

    }
}
