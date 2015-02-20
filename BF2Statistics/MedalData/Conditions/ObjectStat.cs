using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BF2Statistics.MedalData
{
    public class ObjectStat : Condition
    {
        /// <summary>
        /// List of parameters from the python function
        /// </summary>
        protected List<string> Params;

        public ObjectStat(List<string> Params) 
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
            return (Params.Count == 4) ? ReturnType.Number : ReturnType.Bool;
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
            // This can be returned 2 different ways based off of Value
            return (Params.Count == 4)
                ? String.Format("object_stat('{0}', '{1}', {2})", Params[1], Params[2], Params[3])
                : String.Format("object_stat('{0}', '{1}', {2}, {3})", Params[1], Params[2], Params[3], Params[4]);
        }

        /// <summary>
        /// Covnerts the conditions into a TreeNode
        /// </summary>
        /// <returns></returns>
        public override TreeNode ToTree()
        {
            string Name = "IAR: ";
            string Param = "";
            string AsOrWith = "with ";
            string[] parts = Params[3].Split('_');

            // If we have a 5th param, convert it to a timestamp for a round time request
            if (Params.Count == 5)
            {
                Name += (Params[2] == "rtime") 
                    ? Utils.Sec2hms(Int32.Parse(Params[4])) + " " 
                    : String.Format("{0:N0}", Int32.Parse(Params[4])) + " ";
            }

            // Get human readable version of object names
            switch (parts[0])
            {
                case "WEAPON":
                    Param = Data.WeaponNames[Params[3]];
                    break;
                case "VEHICLE":
                    Param = Data.VehicleNames[Params[3]];
                    break;
                case "KIT":
                    Param = Data.KitNames[Params[3]];
                    AsOrWith = "as ";
                    break;
            }

            // Get human readable version of decription
            switch (Params[2])
            {
                case "kills":
                    Name += "Kills " + AsOrWith + Param;
                    break;
                case "rtime":
                    Name += AsOrWith + Param;
                    break;
                case "roadKills":
                    Name += "RoadKills " + AsOrWith + Param;
                    break;
                case "deployed":
                    Name += "Deploys with " + Param;
                    break;
            }

            TreeNode Me = new TreeNode(Name);
            Me.Tag = this;
            return Me;
        }
    }
}
