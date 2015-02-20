using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BF2Statistics.MedalData
{
    public class GlobalStatMultTimes : Condition
    {
        /// <summary>
        /// List of parameters from the python function
        /// </summary>
        protected List<string> Params;

        public GlobalStatMultTimes(List<string> Params) 
        {
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
            return String.Format("global_stat_multiple_times('{0}', {1}, '{2}')", Params[1], Params[2], Params[3]);
        }

        /// <summary>
        /// Covnerts the conditions into a TreeNode
        /// </summary>
        /// <returns></returns>
        public override TreeNode ToTree()
        {
            // Achieve award less times then 
            string Type = Data.GlobalStrings[Params[1]];
            string Value = (Data.IsTimeStat(Params[1]))
                ? Utils.Sec2hms( Int32.Parse(Params[2]) )
                : String.Format("{0:N0}", Int32.Parse(Params[2]));

            TreeNode Me = new TreeNode("Achieved " + Value + " Global " + Type + " One or More Times");
            Me.Tag = this;
            return Me;
        }
    }
}
