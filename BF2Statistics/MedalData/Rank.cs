using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BF2Statistics.MedalData
{
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

        protected static string[] Ranks = new String[22] {
            "Private",
            "Private First Class",
            "Lance Corporal",
            "Corporal",
            "Sergeant",
            "Staff Sergeant",
            "Gunnery Sergeant",
            "Master Sergeant",
            "First Sergeant",
            "Master Gunnery Sergeant",
            "Sergeant Major",
            "Sergeant Major of the Corps",
            "2nd Lieutenant",
            "1st Lieutenant",
            "Captain",
            "Major",
            "Lieutenant Colonel",
            "Colonel",
            "Brigadier General",
            "Major General",
            "Lieutenant General",
            "General"
        };

        public static bool Exists(int RankId)
        {
            return (RankId >= 0 && RankId < 22);
        }

        public static string GetName(int RankId)
        {
            if (!Exists(RankId))
                throw new IndexOutOfRangeException();

            return Ranks[RankId];
        }

        #endregion Static Members
    }
}
