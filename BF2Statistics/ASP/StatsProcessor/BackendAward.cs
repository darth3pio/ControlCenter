using System;
using BF2Statistics.Database;

namespace BF2Statistics.ASP.StatsProcessor
{
    /// <summary>
    /// This object represents an award that is earned during
    /// the processing of a snapshot, and provides methods to
    /// check if a given player has met the criteria to earn it.
    /// </summary>
    public class BackendAward
    {
        /// <summary>
        /// The award ID
        /// </summary>
        public int AwardId { get; protected set; }

        /// <summary>
        /// The award criteria's to earn the award
        /// </summary>
        protected AwardCriteria[] Criterias;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="AwardId">The award id</param>
        /// <param name="Criterias">The criteria's needed to earn the award</param>
        public BackendAward(int AwardId, AwardCriteria[] Criterias)
        {
            this.AwardId = AwardId;
            this.Criterias = Criterias;
        }

        /// <summary>
        /// Returns a bool stating whether the criteria for this award is met for a givin player
        /// </summary>
        /// <param name="Pid">The player ID</param>
        /// <param name="Level">The award level if the criteria is met</param>
        /// <returns></returns>
        public bool CriteriaMet(int Pid, StatsDatabase Driver, ref int Level)
        {
            // Get the award count (or level for badges) for this award
            string Query = "SELECT COALESCE(max(level), 0) FROM awards WHERE id=@P0 AND awd=@P1";
            int AwardCount = Driver.ExecuteScalar<int>(Query, Pid, AwardId);
            bool IsRibbon = (AwardId > 3000000);

            // Can only recieve ribbons once in a lifetime, so return false if we have it already
            if (IsRibbon && AwardCount > 0)
                return false;

            // Medals and Badges can receive multiple times (Badges are award level, not count)
            if (!IsRibbon)
                Level = AwardCount + 1;

            // Loop through each criteria and see if we have met the criteria
            foreach (AwardCriteria Criteria in Criterias)
            {
                // Build the query. We always use a count() or sum() to return a sortof bool.
                string Where = Criteria.Where.Replace("###", Level.ToString());
                Query = String.Format("SELECT {0} FROM {1} WHERE id={2} AND {3}", Criteria.Field, Criteria.Table, Pid, Where);

                // If we dont meet the expected result, the criteria is unmet, no use continuing
                if (Driver.ExecuteScalar<int>(Query) < Criteria.ExpectedResult)
                    return false;
            }

            return true;
        }
    }
}
