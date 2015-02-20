namespace BF2Statistics.Database.QueryBuilder
{
    public class JoinClause
    {
        public JoinType JoinType { get; protected set; }
        public string FromTable { get; protected set; }
        public string FromColumn { get; protected set; }
        public Comparison ComparisonOperator { get; protected set; }
        public string ToTable { get; protected set; }
        public string ToColumn { get; protected set; }

        public JoinClause(JoinType join, string toTableName, string toColumnName, Comparison @operator, string fromTableName, string fromColumnName)
        {
            this.JoinType = join;
            this.FromTable = fromTableName;
            this.FromColumn = fromColumnName;
            this.ComparisonOperator = @operator;
            this.ToTable = toTableName;
            this.ToColumn = toColumnName;
        }
    }
}
