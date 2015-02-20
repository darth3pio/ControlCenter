using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BF2Statistics.MedalData
{
    public class MedalOrRankCondition : Condition
    {
        /// <summary>
        /// List of parameters from the python function
        /// </summary>
        protected List<string> Params;

        public MedalOrRankCondition(List<string> Params)
        {
            if(Params[0] == "has_medal")
                if (!Award.Exists(Params[1]) && Params[1] != "6666666" && Params[1] != "6666667")
                    throw new Exception("[MedalCondition] Award ID: " + Params[1] + " Does not exist!");

            this.Params = Params;
        }

        /// <summary>
        /// Returns a list of parameters for this condition
        /// </summary>
        /// <returns></returns>
        public override List<string> GetParams()
        {
            return Params;
        }

        /// <summary>
        /// Sets the params for this condition
        /// </summary>
        /// <param name="Params"></param>
        public override void SetParams(List<string> Params)
        {
            this.Params = Params;
        }

        /// <summary>
        /// Returns the return value of this condition
        /// </summary>
        public override ReturnType Returns()
        {
            return ReturnType.Bool;
        }

        /// <summary>
        /// Returns a copy (clone) of this object
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Converts the Condition into python executable code
        /// </summary>
        /// <returns></returns>
        public override string ToPython()
        {
            return (Params[0] == "has_medal") 
                ? String.Format("has_medal('{0}')", Params[1]) 
                : String.Format("has_rank({0})", Params[1]);
        }

        /// <summary>
        /// Covnerts the conditions into a TreeNode
        /// </summary>
        /// <returns></returns>
        public override TreeNode ToTree()
        {
            string Name = (Params[0] == "has_medal")
                ? "Has Award \"" + Award.GetName(Params[1]) + "\""
                : "Current Rank Is \"" + Rank.GetName(Int32.Parse(Params[1])) + "\"";

            TreeNode Me = new TreeNode(Name);
            Me.Tag = this;
            return Me;
        }
    }
}
