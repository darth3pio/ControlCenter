using System;
using System.Windows.Forms;

namespace BF2Statistics.MedalData
{
    /// <summary>
    /// Represents a rank found in the medal data file as an object
    /// </summary>
    public class Rank : IAward
    {
        /// <summary>
        /// The Award ID
        /// </summary>
        public int Id { get; protected set; }

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
        /// Constructor
        /// </summary>
        /// <param name="Id">The rank ID</param>
        /// <param name="Conditions">The conditions to earn the rank</param>
        public Rank(int Id, Condition Conditions)
        {
            this.Id = Id;
            this.Name = GetName(Id);
            this.Conditions = Conditions;
            this.OrigConditions = (Condition)Conditions.Clone();
        }

        /// <summary>
        /// Sets the Condition, or Condition list to earn the award
        /// </summary>
        /// <param name="C">The condition or condition list</param>
        public void SetCondition(Condition C)
        {
            Conditions = C;
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
            Conditions = AwardCache.GetDefaultAwardCondition(this.Id.ToString());
        }

        /// <summary>
        /// Returns the name of the rank
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Converts the rank data into python code
        /// </summary>
        /// <returns></returns>
        public string ToPython()
        {
            return String.Format("({0}, 'rank', {1}),#stop", Id, Conditions.ToPython());
        }

        /// <summary>
        /// Converts the condition into a viewable TreeNode for the Criteria Editor
        /// </summary>
        /// <returns></returns>
        public TreeNode ToTree()
        {
            if (Conditions == null)
                return null;
            return Conditions.ToTree();
        }


        #region Static Members

        /// <summary>
        /// Returns whether or not the Rank ID provided is a valid BF2 Rank index
        /// </summary>
        /// <param name="RankId"></param>
        /// <returns></returns>
        public static bool Exists(int RankId)
        {
            return RankId.InRange(0, 21);
        }

        /// <summary>
        /// Returns the Name of the Rank by Index
        /// </summary>
        /// <param name="RankId"></param>
        /// <returns></returns>
        public static string GetName(int RankId)
        {
            if (!Exists(RankId))
                throw new IndexOutOfRangeException();

            return StatsConstants.Ranks[RankId];
        }

        #endregion Static Members
    }
}
