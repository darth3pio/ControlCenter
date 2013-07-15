using System;
using System.Data.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BF2Statistics.Database.QueryBuilder
{
    class WhereStatement : List<WhereClause>
    {
        /// <summary>
        /// Gets or Sets the Logic Operator to use between Where Clauses
        /// </summary>
        public LogicOperator StatementOperator = LogicOperator.Or;

        /// <summary>
        /// Adds a new Where clause to the current Where Statement
        /// </summary>
        /// <param name="FieldName">The Column name</param>
        /// <param name="Operator">The Comparison Operator to use</param>
        /// <param name="Value">The Value object</param>
        /// <returns></returns>
        public WhereClause Add(string FieldName, Comparison @Operator, object Value)
        {
            WhereClause Clause = new WhereClause(FieldName, @Operator, Value);
            this.Add(Clause);
            return Clause;
        }

        /// <summary>
        /// Builds the Where statement to SQL format
        /// </summary>
        /// <returns></returns>
        public string BuildStatement()
        {
            DbCommand Command = null;
            return BuildStatement(false, ref Command);
        }

        /// <summary>
        /// Builds the Where statement to SQL format
        /// </summary>
        /// <param name="UseCommand">Determines whether or not to use a DbCommand object, and DbParameters</param>
        /// <param name="Command">The command object to use</param>
        /// <returns></returns>
        public string BuildStatement(bool UseCommand, ref DbCommand Command)
        {
            StringBuilder Statement = new StringBuilder();
            int Counter = 1;

            foreach (WhereClause ParentClause in this)
            {
                // Open Parent Clause
                Statement.Append("(");

                foreach(WhereClause.SubClause Clause in ParentClause)
                {
                    // SubClause Counter
                    int pCounter = 1;

                    // If we have more clauses, append operator
                    if ((pCounter < ParentClause.Count) && Clause.LogicOperator != null)
                        Statement.Append((Clause.LogicOperator == LogicOperator.Or) ? " OR " : " AND ");
                    pCounter++;

                    // If using a command, Convert values to Parameters
                    if (UseCommand && Command != null && Clause.Value != null && Clause.Value != DBNull.Value && !(Clause.Value is SqlLiteral))
                    {
                        if (Clause.ComparisonOperator == Comparison.Between || Clause.ComparisonOperator == Comparison.NotBetween)
                        {
                            // Add the between values to the command parameters
                            object[] Between = ((object[])Clause.Value);
                            DbParameter Param1 = Command.CreateParameter();
                            Param1.ParameterName = "@P" + Command.Parameters.Count;
                            Param1.Value = Between[0].ToString();
                            DbParameter Param2 = Command.CreateParameter();
                            Param2.ParameterName = "@P" + (Command.Parameters.Count + 1);
                            Param2.Value = Between[1].ToString();

                            // Add Params to command
                            Command.Parameters.Add(Param1);
                            Command.Parameters.Add(Param2);

                            // Add statement
                           Statement.Append( 
                               CreateComparisonClause(Clause.FieldName, Clause.ComparisonOperator, (object) new object[2]
                               {
                                    (object) new SqlLiteral(Param1.ParameterName),
                                    (object) new SqlLiteral(Param2.ParameterName)
                               })
                            );
                        }
                        else
                        {
                            // Create param for value
                            DbParameter Param = Command.CreateParameter();
                            Param.ParameterName = "@P" + Command.Parameters.Count;
                            Param.Value = Clause.Value;

                            // Add Params to command
                            Command.Parameters.Add(Param);

                            // Add statement
                            Statement.Append(CreateComparisonClause(Clause.FieldName, Clause.ComparisonOperator, new SqlLiteral(Param.ParameterName)));
                        }
                    }
                    else
                        Statement.Append(CreateComparisonClause(Clause.FieldName, Clause.ComparisonOperator, Clause.Value));
                }

                // Close Parent Clause
                Statement.Append(")");

                // If we have more clauses, append operator
                if (Counter < this.Count)
                    Statement.Append( (StatementOperator == LogicOperator.Or) ? " OR " : " AND " );
                Counter++;
            }

            return Statement.ToString();
        }

        /// <summary>
        /// Formats, using the correct Comparaison Operator, The clause to SQL.
        /// </summary>
        /// <param name="FieldName">The Clause Column name</param>
        /// <param name="ComparisonOperator">The Comparison Operator</param>
        /// <param name="Value">The Value object</param>
        /// <returns>Clause formatted to SQL</returns>
        public static string CreateComparisonClause(string FieldName, Comparison ComparisonOperator, object Value)
        {
            // Only 2 options for null values
            if (Value == null || Value == DBNull.Value)
            {
                switch (ComparisonOperator)
                {
                    case Comparison.Equals:
                        return FieldName + " IS NULL";
                    case Comparison.NotEqualTo:
                        return "NOT " + FieldName + " IS NULL";
                }
            }
            else
            {
                switch (ComparisonOperator)
                {
                    case Comparison.Equals:
                        return FieldName + " = " + FormatSQLValue(Value);
                    case Comparison.NotEqualTo:
                        return FieldName + " <> " + FormatSQLValue(Value);
                    case Comparison.Like:
                        return FieldName + " LIKE " + FormatSQLValue(Value);
                    case Comparison.NotLike:
                        return "NOT " + FieldName + " LIKE " + FormatSQLValue(Value);
                    case Comparison.GreaterThan:
                        return FieldName + " > " + FormatSQLValue(Value);
                    case Comparison.GreaterOrEquals:
                        return FieldName + " >= " + FormatSQLValue(Value);
                    case Comparison.LessThan:
                        return FieldName + " < " + FormatSQLValue(Value);
                    case Comparison.LessOrEquals:
                        return FieldName + " <= " + FormatSQLValue(Value);
                    case Comparison.In:
                    case Comparison.NotIn:
                        string str1 = (ComparisonOperator == Comparison.NotIn) ? "NOT " : "";
                        if (Value is Array)
                        {
                            Array array = (Array)Value;
                            string str2 = str1 + FieldName + " IN (";
                            foreach (object someValue in array)
                                str2 = str2 + FormatSQLValue(someValue) + ",";
                            return str2.TrimEnd(new char[1] { ',' }) + ")";
                        }
                        else if (Value is string)
                            return str1 + FieldName + " IN (" + Value.ToString() + ")";
                        else
                            return str1 + FieldName + " IN (" + FormatSQLValue(Value) + ")";
                    case Comparison.Between:
                    case Comparison.NotBetween:
                        object[] objArray = (object[])Value;
                        return String.Format(
                            "{0}{1} BETWEEN {2} AND {3}", 
                            ((ComparisonOperator == Comparison.NotBetween) ? "NOT " : ""), 
                            FieldName, 
                            FormatSQLValue(objArray[0]), 
                            FormatSQLValue(objArray[1])
                        );
                }
            }

            return "";
        }

        /// <summary>
        /// Formats and escapes a Value object, to the proper SQL format.
        /// </summary>
        /// <param name="someValue"></param>
        /// <returns></returns>
        public static string FormatSQLValue(object someValue)
        {
            if (someValue == null)
                return "NULL";

            switch (someValue.GetType().Name)
            {
                case "String":
                    return "'" + ((string)someValue).Replace("'", "''") + "'";
                case "DateTime":
                    return "'" + ((DateTime)someValue).ToString("yyyy/MM/dd HH:mm:ss") + "'";
                case "DBNull":
                    return "NULL";
                case "Boolean":
                    return (bool)someValue ? "1" : "0";
                case "Guid":
                    return "'" + ((Guid)someValue).ToString() + "'";
                case "SqlLiteral":
                    return ((SqlLiteral)someValue).Value;
                case "SelectQueryBuilder":
                    return ((SelectQueryBuilder)someValue).BuildQuery();
                default:
                    return someValue.ToString();
            }
        }
    }
}
