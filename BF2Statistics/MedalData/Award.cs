using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BF2Statistics.MedalData
{
    /// <summary>
    /// Award type enumeration
    /// </summary>
    public enum AwardType { Medal, Badge, Ribbon, Rank }

    /// <summary>
    /// Badgel level enumeration
    /// </summary>
    public enum BadgeLevel { Bronze, Silver, Gold }

    /// <summary>
    /// Represents an award found in the medal data file as an object
    /// </summary>
    public class Award : IAward
    {
        /// <summary>
        /// The Award ID
        /// </summary>
        public string Id { get; protected set; }

        /// <summary>
        /// The award string name
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// The Conition (or ConditionList) to get said award in-game
        /// </summary>
        public Condition Conditions { get; protected set; }

        /// <summary>
        /// The original Conition (or ConditionList) to get said award in-game
        /// </summary>
        private Condition OrigConditions;

        /// <summary>
        /// The string ID of the award
        /// </summary>
        protected string StrId;

        /// <summary>
        /// The award earn type
        /// </summary>
        protected int Type;

        #region Non-Static Members

        /// <summary>
        /// Class Constructor. Constructs a new award
        /// </summary>
        /// <param name="AwardId">The award id as a string</param>
        /// <param name="StrId">The short name of the award</param>
        /// <param name="Type">The award earn type. 0 = Purple heart, 1 = Earned only once per player, 2 = Earnable multiple times</param>
        /// <param name="Condition">The condition to earn this award</param>
        public Award(string AwardId, string StrId, string Type, Condition Condition)
        {
            // Throw an exception if the award is non-existant
            if (!Exists(AwardId))
                throw new Exception("Award Doesnt Exist: " + AwardId);

            // Set award vars
            this.Id = AwardId;
            this.StrId = StrId;
            this.Name = GetName(AwardId);
            this.Type = Int32.Parse(Type);
            this.Conditions = Condition;
            this.OrigConditions = (Condition)Condition.Clone();
        }

        /// <summary>
        /// Sets the Condition, or Condition list to earn the award
        /// </summary>
        /// <param name="C">The condition or condition list</param>
        public void SetCondition(Condition C) 
        {
            this.Conditions = C;
        }

        /// <summary>
        /// Returns the awards conditions
        /// </summary>
        /// <returns></returns>
        public Condition GetCondition()
        {
            return Conditions;
        }

        /// <summary>
        /// Restores any changes made to the conditions of this award
        /// </summary>
        /// <returns></returns>
        public void UndoConditionChanges()
        {
            Conditions = (Condition)OrigConditions.Clone();
        }

        /// <summary>
        /// Restores the condition of this award to the default (vanilla) state
        /// </summary>
        public void RestoreDefaultConditions()
        {
            Conditions = AwardCache.GetDefaultAwardCondition(this.Id);
        }

        /// <summary>
        /// Returns the name of the award
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Converts the medal data, and its conditions to python
        /// </summary>
        /// <returns></returns>
        public string ToPython()
        {
            return (Conditions == null) 
                ? null 
                : String.Format("('{0}', '{1}', {2}, {3}),#stop", Id, StrId, Type, Conditions.ToPython());
        }

        /// <summary>
        /// Converts the medal's conditions to a TreeView
        /// </summary>
        /// <returns></returns>
        public TreeNode ToTree()
        {
            return (Conditions == null) ? null : Conditions.ToTree();
        }

        #endregion Non-Static Members

        #region StaticMembers

        /// <summary>
        /// Returns the Award type of an award.
        /// </summary>
        /// <param name="AwardId">The award ID</param>
        /// <returns>AwardType enumeration</returns>
        public static AwardType GetType(string AwardId)
        {
            // Badges always have an underscore
            if (AwardId.Contains('_'))
                return AwardType.Badge;
            
            // Contert to int
            int id = Int32.Parse(AwardId);
            if (id < 22)
                return AwardType.Rank;

            return (id > 3000000) ? AwardType.Ribbon : AwardType.Medal;
        }

        /// <summary>
        /// Returns the badge level
        /// </summary>
        /// <param name="BadgeId">The award ID</param>
        /// <returns>BadgeLevel enumeration</returns>
        public static BadgeLevel GetBadgeLevel(string BadgeId)
        {
            // Badges always have an underscore
            if (!BadgeId.Contains('_'))
                throw new Exception("Award is not a badge");

            // Make sure the badge level is in range
            int id = Int32.Parse(BadgeId.Split('_')[1]);
            if (!id.InRange(1, 3))
                throw new Exception("Invalid badge level provided: " + id);

            // Now that we have validated this badge id/level, return its type
            switch (id)
            {
                default:
                case 1: return BadgeLevel.Bronze;
                case 2: return BadgeLevel.Silver;
                case 3: return BadgeLevel.Gold;
            }
        }

        /// <summary>
        /// Returns the full string name of an award
        /// </summary>
        /// <param name="AwardId">The award ID</param>
        /// <returns></returns>
        public static string GetName(string AwardId)
        {
            // Badges always have an underscore
            if (AwardId.Contains('_'))
            {
                // Make sure the award exists!
                string[] parts = AwardId.Split('_');
                if (!StatsConstants.Awards.ContainsKey(parts[0]))
                    throw new Exception("Not a valid badge ID");

                switch (parts[1])
                {
                    default:
                    case "1": return "Basic " + StatsConstants.Awards[parts[0]];
                    case "2": return "Veteran " + StatsConstants.Awards[parts[0]];
                    case "3": return "Expert " + StatsConstants.Awards[parts[0]];
                }
            }

            // Make sure the award exists
            if (!StatsConstants.Awards.ContainsKey(AwardId))
                throw new Exception("Invalid Award ID");

            return StatsConstants.Awards[AwardId];
        }

        /// <summary>
        /// Returns whether a specified award ID exists
        /// </summary>
        /// <param name="AwardId">The Award ID</param>
        /// <returns></returns>
        public static bool Exists(string AwardId)
        {
            if (AwardId.Contains('_'))
                AwardId = AwardId.Split('_')[0];

            return StatsConstants.Awards.ContainsKey(AwardId);
        }

        /// <summary>
        /// Indicates whether the award is a special forces award
        /// </summary>
        /// <param name="AwardId"></param>
        /// <returns></returns>
        public static bool IsSfAward(string AwardId)
        {
            // SF awards always go as x26xxxx
            return (AwardId[1] == '2' && AwardId[2] == '6');
        }

        #endregion StaticMembers
    }
}
