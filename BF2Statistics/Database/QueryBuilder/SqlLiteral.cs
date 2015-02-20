namespace BF2Statistics.Database.QueryBuilder
{
    /// <summary>
    /// This class represents a Literal value
    /// to be used in a query (no quotations)
    /// </summary>
    class SqlLiteral
    {
        public string Value { get; protected set; }

        public SqlLiteral(string Value)
        {
            this.Value = Value;
        }
    }
}
