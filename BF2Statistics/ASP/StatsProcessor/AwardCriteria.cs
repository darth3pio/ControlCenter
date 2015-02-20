namespace BF2Statistics.ASP.StatsProcessor
{
    /// <summary>
    /// The award criteria struct is used to define the query
    /// details used when determining if a player has met the requirements
    /// to earn an award. This object is used by the "BackendAward" object.
    /// </summary>
    public struct AwardCriteria
    {
        /// <summary>
        /// The table to run the qyery
        /// </summary>
        public string Table;

        /// <summary>
        /// The field (or columns) to run the query on
        /// </summary>
        public string Field;

        /// <summary>
        /// The expected result of the query. If the result of the query matches this,
        /// or is greater, then the criteria is considered met.
        /// </summary>
        public int ExpectedResult;

        /// <summary>
        /// The where statement to use when running the query
        /// </summary>
        public string Where;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Table">The table name</param>
        /// <param name="Field">The field (column name)</param>
        /// <param name="ExpectedResult">
        ///     The expected result. If the result matches this result,
        ///     the criteria is considered met
        /// </param>
        /// <param name="Where">The where statement when running the query</param>
        public AwardCriteria(string Table, string Field, int ExpectedResult, string Where)
        {
            this.Table = Table;
            this.Field = Field;
            this.ExpectedResult = ExpectedResult;
            this.Where = Where;
        }
    }
}
