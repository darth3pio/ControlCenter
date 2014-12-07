using System;
using System.Threading;
using BF2Statistics.Database;

namespace BF2Statistics.ASP
{
    /// <summary>
    /// The PidManager class is used to safely generate unique, thread safe
    /// player ID numbers for the stats and gamespy services
    /// </summary>
    class PidManager
    {
        // Define Min/Max PID numbers for players
        const int DEFAULT_PID = 29000000;
        const int MAX_PID = 30000000;

        /// <summary>
        /// The current highest player ID value (Incremented)
        /// </summary>
        protected static int PlayerPid = 0;

        /// <summary>
        /// The current Lowest player ID value (Decremented)
        /// </summary>
        public static int AiPid = 0;

        /// <summary>
        /// Method to be called everytime the HttpStatsServer is started
        /// </summary>
        public static void Load(StatsDatabase Driver)
        {
            // Get the lowest Offline PID from the database
            var Rows = Driver.Query(
                String.Format(
                    "SELECT COALESCE(MIN(id), {0}) AS min, COALESCE(MAX(id), {0}) AS max FROM player WHERE id < {1}", 
                    DEFAULT_PID, MAX_PID
                )
            );

            int Lowest = Int32.Parse(Rows[0]["min"].ToString());
            int Highest = Int32.Parse(Rows[0]["max"].ToString());
            AiPid = (Lowest > DEFAULT_PID) ? DEFAULT_PID : Lowest;
            PlayerPid = (Highest < DEFAULT_PID) ? DEFAULT_PID : Highest;
        }

        /// <summary>
        /// Returns a new PID number for use when creating a new player
        /// for the stats database
        /// </summary>
        /// <returns></returns>
        public static int GenerateNewPlayerPid()
        {
            // Thread safe decrement
            return Interlocked.Increment(ref PlayerPid);
        }

        /// <summary>
        /// Returns a new PID number for use when creating a new AI Bot
        /// for the stats database
        /// </summary>
        /// <returns></returns>
        public static int GenerateNewAIPid()
        {
            // Thread safe decrement
            return Interlocked.Decrement(ref AiPid);
        }
    }
}
