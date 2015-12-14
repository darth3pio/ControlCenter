using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BF2Statistics.MedalData;

namespace BF2Statistics.Web.Bf2Stats
{
    /// <summary>
    /// The RankCalulator object is used to programmatically determine the next
    /// ranks that can be earned by a player given its current stats and awards
    /// </summary>
    public static class RankCalculator
    {
        /// <summary>
        /// An Array of Rank objects that define the requirements to earn the Ranks
        /// in Battlefield 2
        /// </summary>
        private static Rank[] Ranks;

        /// <summary>
        /// Used when the RankData.xml is loaded by the StatsData object,
        /// to set the rank requirements
        /// </summary>
        /// <param name="RankData"></param>
        public static void SetRankData(Rank[] RankData)
        {
            Ranks = RankData;
        }

        /// <summary>
        /// Generates the next <paramref name="Count"/> ranks that can currently be obtained by the player
        /// </summary>
        /// <param name="Score">The players current score</param>
        /// <param name="Rank">The players current rank ID</param>
        /// <param name="Awards">All the players earned awards</param>
        /// <param name="Count">The number of next ranks to display</param>
        /// <returns></returns>
        public static List<Rank> GetNextRanks(int Score, int Rank, Dictionary<string, int> Awards, int Count = 3)
        {
            // do 1 rank at a time until we get 3 go's
            List<Rank> NextXRanks = new List<Rank>();
            int Avail = 21 - Rank; // Available promotions left
            StringBuilder Desc = new StringBuilder();

            // Make sure we dont get an index out of range exception
            if (Avail < Count) Count = Avail;

            // Loop through each promotion
            for (int i = 0; i < Count; i++)
            {
                // Update rank
                if (NextXRanks.Count > 0)
                    Rank = NextXRanks.Last().Id;

                // Generate a list of ranks we can jump to based on our current rank
                List<Rank> NextPromoRanks = GetNextRankUps(Rank, Awards);
                if (NextPromoRanks.Count == 0)
                    break;

                // Defines if we added a rank for the next promotion
                bool AddedARank = false;

                // We need to reverse the next rank array, so highest possible ranks are first, and
                // then we work our way down until we are just +1 rank
                foreach (Rank Rnk in NextPromoRanks.Reverse<Rank>())
                {
                    // First we loop through the required awards (if any), and see if we
                    // have the required awards and level to meet the promotion requirement
                    bool MeetsAwardReqs = true;
                    if (Rnk.ReqAwards.Count > 0)
                    {
                        foreach (KeyValuePair<string, int> Awd in Rnk.ReqAwards)
                        {
                            if (!Awards.ContainsKey(Awd.Key) || Awards[Awd.Key] < Awd.Value)
                            {
                                MeetsAwardReqs = false;
                                Rnk.MissingAwards.Add(Awd.Key, Awd.Value);
                            }
                        }
                    }

                    // If we meet the requirement for this rank, add it
                    if (MeetsAwardReqs)
                    {
                        // Set missing awards description
                        if (Desc.Length != 0)
                        {
                            Rnk.MissingDesc = Desc.ToString();
                            Desc.Clear();
                        }

                        NextXRanks.Add(Rnk);
                        AddedARank = true;
                        break;
                    }
                    else
                    {
                        // If we have multiple ranks for next promotion, and we havent cycled through all of them
                        if (NextPromoRanks.Count > 1 && Rnk != NextPromoRanks[0])
                        {
                            Desc.Clear();
                            Desc.Append(GenerateMissingDesc(Rnk, true));
                        }
                        else
                            Rnk.MissingDesc = GenerateMissingDesc(Rnk, false);
                    }
                }

                // Make sure we add at least the next rank, even if we dont qualify
                if (!AddedARank)
                {
                    NextXRanks.Add(NextPromoRanks[0]);
                }
            }

            return NextXRanks;
        }

        /// <summary>
        /// Returns a rank array of ranks that can be "jumped" to from the current rank
        /// Example: Gunnery Seargent -> Return would be Master Sergeant, and First Sergeant
        /// </summary>
        /// <param name="CurRank"></param>
        /// <param name="Awards"></param>
        private static List<Rank> GetNextRankUps(int CurRank, Dictionary<string, int> Awards)
        {
            List<Rank> rRanks = new List<Rank>();
            for (int i = CurRank + 1; i < 22; i++)
            {
                // Skip SMoC
                if (i == 11)
                    continue;

                // Make sure the next rank up allows a jump from the current rank
                if (Ranks[i].ReqRank.Contains(CurRank))
                    rRanks.Add((Rank)Ranks[i].Clone());
            }

            return rRanks;
        }

        /// <summary>
        /// Generates the Missing Awards description message, based on what awards are missing
        /// </summary>
        /// <param name="Rnk"></param>
        /// <param name="ForPrevRank"></param>
        /// <returns></returns>
        private static string GenerateMissingDesc(Rank Rnk, bool ForPrevRank)
        {
            StringBuilder Msg = new StringBuilder();

            // Prefix
            if (ForPrevRank)
            {
                Msg.AppendFormat(
                    "You are not yet eligible for the advanced rank of <strong>{0}</strong> because you are missing the awards: ",
                    MedalData.Rank.GetName(Rnk.Id)
                );
            }
            else
                Msg.Append("You are missing the awards: ");

            // Add missing award titles
            int i = 0;
            foreach (KeyValuePair<string, int> Awd in Rnk.MissingAwards)
            {
                // Increment missing award ID
                i++;

                // Check for Badge, and append the badge level if so...
                string AwdId = (Awd.Key[0] == '1') ? Awd.Key + "_" + Awd.Value : Awd.Key;

                // Fetch the award's full string name
                string name = StatsConstants.Awards.ContainsKey(Awd.Key) ? MedalData.Award.GetName(AwdId) : "Unknown";

                // Add the award
                Msg.Append(name + ((i == Rnk.MissingAwards.Count) ? "." : ",") + " ");
            }

            return Msg.ToString();
        }
    }
}
