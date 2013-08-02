using System;
using System.Collections.Generic;
using BF2Statistics.Database;

namespace BF2Statistics.ASP
{
    class BackendAward
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
        public BackendAward(int AwardId,  params AwardCriteria[] Criterias)
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
        public bool CriteriaMet(int Pid, out int Level)
        {
            // Prepare variables
            DatabaseDriver Driver = ASPServer.Database.Driver;
            List<Dictionary<string, object>> Rows;

            // See if the player has the award already
            Rows = Driver.Query("SELECT level FROM awards WHERE id=@P0 AND awd=@P1 ORDER BY level DESC", Pid, AwardId);
            int AwardCount = Rows.Count;
            bool MeetsCriteria = false;

            // If award is a medal, We Can receive multiple times. Badges also fall under this catagory
            // because there is different levels to the badges. Ribbons can only be awarded once!
            if (AwardId < 3000000)
                // Can receive multiple times (Medal | Badge)
                Level = ((AwardCount > 0) ? Int32.Parse(Rows[0]["level"].ToString()) + 1 : 1);
            else
                Level = 1;

            // Can only recieve ribbons once in a lifetime
            if (AwardId > 3000000 && AwardCount > 0)
                return false;

            // Loop through each criteria and see if we have met the criteria
            foreach (AwardCriteria Criteria in Criterias)
            {
                // Check to see if the player meets the requirments for the award
                string Where = Criteria.Where.Replace("###", Level.ToString());
                Rows = Driver.Query(String.Format("SELECT {0} AS checkval FROM {1} WHERE id={2} AND {3};", Criteria.Field, Criteria.Table, Pid, Where));
                if (Int32.Parse(Rows[0]["checkval"].ToString()) < Criteria.ExpectedResult)
                {
                    // Criteria not met... no use continuing
                    MeetsCriteria = false;
                    break;
                }
                else
                    MeetsCriteria = true;
            }

            return MeetsCriteria;
        }
    }
}
