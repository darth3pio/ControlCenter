using System;
using System.Collections.Generic;

namespace BF2Statistics.MedalData
{
    /// <summary>
    /// The Award cache class is responsible for holding all the found
    /// awards and ranks from the Medal data file.
    /// </summary>
    public static class AwardCache
    {
        /// <summary>
        /// List of all found Medals
        /// </summary>
        private static List<Award> Medals = new List<Award>();

        /// <summary>
        /// List of all found Ribbons
        /// </summary>
        private static List<Award> Ribbons = new List<Award>();

        /// <summary>
        /// List of all found badges
        /// </summary>
        private static List<Award> Badges = new List<Award>();

        /// <summary>
        /// List of al found ranks
        /// </summary>
        private static List<Rank> Ranks = new List<Rank>();

        /// <summary>
        /// Complete list of all found awards (AwardId => Award)
        /// </summary>
        private static Dictionary<string, IAward> Awards = new Dictionary<string, IAward>();

        /// <summary>
        /// Complete list of all awards ORIGINAL conditions (vanilla)
        /// </summary>
        private static Dictionary<string, Condition> OrigConditions = new Dictionary<string, Condition>();

        /// <summary>
        /// Clears all awards. Usually only called when the MedalData file changes.
        /// </summary>
        public static void Clear()
        {
            Medals = new List<Award>();
            Ribbons = new List<Award>();
            Badges = new List<Award>();
            Ranks = new List<Rank>();
            Awards = new Dictionary<string, IAward>();
        }

        /// <summary>
        /// Used by the parser to add found awards to the list
        /// </summary>
        /// <param name="A">The Award Object To Add</param>
        public static void AddAward(Award A)
        {
            // Add award to individual award type collection
            switch (Award.GetType(A.Id))
            {
                case AwardType.Badge:
                    Badges.Add(A);
                    break;
                case AwardType.Medal:
                    Medals.Add(A);
                    break;
                case AwardType.Ribbon:
                    Ribbons.Add(A);
                    break;
            }

            // Add the award to the overall awards cache
            Awards.Add(A.Id, A);

            // Build the award tree here to Initially check for condition errors
            A.ToTree();
        }

        /// <summary>
        /// Used by the parser to add found ranks to the list
        /// </summary>
        /// <param name="A">The Rank Object To Add</param>
        public static void AddRank(Rank A)
        {
            // Add rank to indiviual rank collection
            Ranks.Add(A);

            // Add the award to the overall awards cache
            Awards.Add(A.Id.ToString(), A);

            // Build the award tree here to Initially check for condition errors
            A.ToTree();
        }

        /// <summary>
        /// Used by the Medal data parser to add the original award conditions
        /// </summary>
        /// <param name="Id">The Award ID found in the Medal Data file</param>
        /// <param name="C">The parsed condition to earn the award</param>
        public static void AddDefaultAwardCondition(string Id, Condition C)
        {
            if (!OrigConditions.ContainsKey(Id))
                OrigConditions.Add(Id, C);
        }

        /// <summary>
        /// Returns the original (vanilla) condition list to earn the specified award
        /// </summary>
        /// <param name="Id">The award's ID</param>
        public static Condition GetDefaultAwardCondition(string Id)
        {
            if (OrigConditions.Count == 0)
                throw new Exception("Original Conditions have not been set yet!");

            return OrigConditions[Id].Clone() as Condition;
        }

        /// <summary>
        /// Returns all found badges
        /// </summary>
        public static IEnumerable<Award> GetBadges()
        {
            foreach (Award A in Badges)
                yield return A;
        }

        /// <summary>
        /// Returns all found Medals
        /// </summary>
        public static IEnumerable<Award> GetMedals()
        {
            foreach (Award A in Medals)
                yield return A;
        }

        /// <summary>
        /// Returns all found ribbons
        /// </summary>
        public static IEnumerable<Award> GetRibbons()
        {
            foreach (Award A in Ribbons)
                yield return A;
        }

        /// <summary>
        /// Returns all found ranks
        /// </summary>
        public static IEnumerable<Rank> GetRanks()
        {
            foreach (Rank A in Ranks)
                yield return A;
        }

        /// <summary>
        /// This method is used to fetch a particular Award or rank by ID
        /// </summary>
        /// <param name="Id">The Award ID</param>
        public static IAward GetAward(string Id)
        {
            return Awards[Id];
        }
    }
}
