using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BF2Statistics.MedalData
{
    public class ConditionValue : Condition
    {
        /// <summary>
        /// The literal Value
        /// </summary>
        public string Value;

        public ConditionValue(string val)
        {
            this.Value = val;
        }

        /// <summary>
        /// Returns a list of parameters for this condition
        /// </summary>
        /// <returns></returns>
        public override List<string> GetParams()
        {
            return new List<string>() { Value };
        }

        /// <summary>
        /// Sets the params for this condition
        /// </summary>
        /// <param name="Params"></param>
        public override void SetParams(List<string> Params)
        {
            this.Value = Params[0];
        }

        /// <summary>
        /// Returns the return value of this condition
        /// </summary>
        public override ReturnType Returns()
        {
            return ReturnType.Number;
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
        /// Returns the value of this condition
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Converts the Condition into python executable code
        /// </summary>
        /// <returns></returns>
        public override string ToPython()
        {
            return Value;
        }

        /// <summary>
        /// Covnerts the conditions into a TreeNode
        /// </summary>
        /// <returns></returns>
        public override TreeNode ToTree()
        {
            TreeNode Me = new TreeNode( String.Format("{0:N0}", Value) );
            Me.Tag = this;
            return Me;
        }
    }
}
