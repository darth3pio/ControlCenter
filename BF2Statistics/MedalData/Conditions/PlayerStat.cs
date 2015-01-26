using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BF2Statistics.MedalData
{
    public class PlayerStat : Condition
    {
        /// <summary>
        /// List of parameters from the python function
        /// </summary>
        protected List<string> Params;

        public PlayerStat(List<string> Params)
        {
            this.Params = Params;
            if (Params[0] == "player_stat" && !Data.IsTimeStat(Params[1]))
                Params[0] = "player_score";
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
            if (Params[0] == "player_stat" && !Data.IsTimeStat(Params[1]))
                Params[0] = "player_score";
        }

        /// <summary>
        /// Returns the return value of this condition
        /// </summary>
        public override ReturnType Returns()
        {
            return (Params.Count == 2) ? ReturnType.Number : ReturnType.Bool;
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
            if (Params.Count == 2)
                return String.Format("{0}('{1}')", Params[0], Params[1]);
            else
                return String.Format("{0}('{1}', {2})", Params[0], Params[1], Params[2]);
        }

        /// <summary>
        /// Covnerts the conditions into a TreeNode
        /// </summary>
        /// <returns></returns>
        public override TreeNode ToTree()
        {
            string Name;
            string P2 = "";

            // Define start of description
            if (Params[0] == "global_stat")
                Name = "Global " + Data.GlobalStrings[Params[1]];
            else
                Name = "Round " + Data.PlayerStrings[Params[1]];

            // If we have 3 params, parse the last paramenter
            if (Params.Count == 3)
            {
                if (Data.IsTimeStat(Params[1]))
                    P2 = Utils.Sec2hms(Int32.Parse(Params[2]));
                else
                    P2 = String.Format("{0:N0}", Int32.Parse(Params[2]));

                Name += " Equal to or Greater Than " + P2;
            }

            TreeNode Me = new TreeNode(Name);
            Me.Tag = this;
            return Me;
        }
    }
}
