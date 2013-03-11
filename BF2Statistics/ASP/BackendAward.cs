using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BF2Statistics.Database;

namespace BF2Statistics.ASP
{
    class BackendAward
    {
        public int AwardId { get; protected set; }

        protected AwardCriteria[] Criterias;

        public BackendAward(int AwardId,  params AwardCriteria[] Criterias)
        {
            this.AwardId = AwardId;
            this.Criterias = Criterias;
        }

        public bool CriteriaMet(int Pid, out int Level)
        {
            DatabaseDriver Driver = ASPServer.Database.Driver;
            List<Dictionary<string, object>> Rows;

            // See if the player has the award already
            Rows = Driver.Query("SELECT awd, level FROM awards WHERE id={0} AND awd={1}", Pid, AwardId);
            int AwardCount = Rows.Count;
            bool MeetsCriteria = false;

            // If award is medal, We Can receive multiple times
            if (AwardId < 3000000)
                // Can receive multiple times (Medal)
                Level = ((AwardCount > 0) ? Int32.Parse(Rows[0]["level"].ToString()) + 1 : 1);
            else
                Level = 1;

            foreach (AwardCriteria Criteria in Criterias)
            {
                // Check to see if the player meets the requirments for the award
                string Where = Criteria.Where.Replace("###", Level.ToString());
                Rows = Driver.Query("SELECT {0} AS checkval FROM {1} WHERE id={2} AND {3};", Criteria.Field, Criteria.Table, Pid, Where);
                if (Int32.Parse(Rows[0]["checkval"].ToString()) < Criteria.ExpectedResult)
                {
                    MeetsCriteria = false;
                    break;
                }
                else
                    MeetsCriteria = true;
            }

            if (MeetsCriteria)
            {
                // Can only recieve ribbons once in a lifetime
                if (AwardId > 3000000 && AwardCount != 0)
                    return false;
            }

            return MeetsCriteria;
        }
    }

    class AwardCriteria
    {
        public string Table;

        public string Field;

        public int ExpectedResult;

        public string Where;

        public AwardCriteria(string Table, string Field, int ExpectedResult, string Where)
        {
            this.Table = Table;
            this.Field = Field;
            this.ExpectedResult = ExpectedResult;
            this.Where = Where;
        }
    }
}
