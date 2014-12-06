using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BF2Statistics.Web.Bf2Stats
{
    class RankCalculator
    {
        protected static Rank[] Ranks;

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
        /// Generates the next 3 ranks that can currently be obtained by the player
        /// </summary>
        /// <param name="Score">The players current score</param>
        /// <param name="Rank">The players current rank ID</param>
        /// <param name="Awards">All the players earned awards</param>
        /// <returns></returns>
        public static List<Rank> GetNext3Ranks(int Score, int Rank, Dictionary<string, int> Awards)
        {
            // do 1 rank at a time until we get 3 go's
            List<Rank> NextXRanks = new List<Rank>();
            int Count = 3; // Number of itterations to do
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
        protected static List<Rank> GetNextRankUps(int CurRank, Dictionary<string, int> Awards)
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
        protected static string GenerateMissingDesc(Rank Rnk, bool ForPrevRank)
        {
            StringBuilder Msg = new StringBuilder();

            // Prefix
            if (ForPrevRank)
                Msg.AppendFormat("You are not yet eligible for the advanced rank of <strong>{0}</strong> because you are missing the awards: ", StatsData.GetRankName(Rnk.Id));
            else
                Msg.Append("You are missing the awards: ");

            // Add missing award titles
            int i = 0;
            foreach (KeyValuePair<string, int> Award in Rnk.MissingAwards)
            {
                string name = "Unknown";
                i++;

                if (StatsData.Badges.ContainsKey(Award.Key))
                    name = GetBadgePrefix(Award.Value) + StatsData.Badges[Award.Key];
                else if (StatsData.Medals.ContainsKey(Award.Key))
                    name = StatsData.Medals[Award.Key];
                else if (StatsData.Ribbons.ContainsKey(Award.Key))
                    name = StatsData.Ribbons[Award.Key];
                else if (StatsData.SfBadges.ContainsKey(Award.Key))
                    name = GetBadgePrefix(Award.Value) + StatsData.SfBadges[Award.Key];
                else if (StatsData.SfMedals.ContainsKey(Award.Key))
                    name = StatsData.SfMedals[Award.Key];
                else if (StatsData.SfRibbons.ContainsKey(Award.Key))
                    name = StatsData.SfRibbons[Award.Key];

                // Add the award
                if(i == Rnk.MissingAwards.Count)
                    Msg.Append(name + ".");
                else
                    Msg.Append(name + ", ");
            }

            return Msg.ToString();
        }

        /// <summary>
        /// Returns a prefix for a badge level
        /// </summary>
        /// <param name="BadgeLevel"></param>
        /// <returns></returns>
        protected static string GetBadgePrefix(int BadgeLevel)
        {
            switch (BadgeLevel)
            {
                case 1:
                    return "Basic ";
                case 2:
                    return "Veteran ";
                case 3:
                    return "Expert ";
            }

            return "";
        }
    }
}
