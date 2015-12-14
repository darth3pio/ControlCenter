using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace BF2Statistics.MedalData
{
    /// <summary>
    /// Return types for python methods
    /// </summary>
    public enum ReturnType { Number, Bool }

    /// <summary>
    /// Provides a base object to define conditions required to receive an award in the medal_data.py
    /// </summary>
    public abstract class Condition : ICloneable
    {
        /// <summary>
        /// Returns the value return type of this condition
        /// </summary>
        public abstract ReturnType Returns();

        /// <summary>
        /// Returns the parameters that make up the python parts of the function
        /// </summary>
        /// <returns></returns>
        public abstract List<String> GetParams();

        /// <summary>
        /// Sets new parameters for the python function
        /// </summary>
        /// <param name="Params"></param>
        public abstract void SetParams(List<string> Params);

        /// <summary>
        /// Creates a Deep clone of the condition, and returns it
        /// </summary>
        /// <returns></returns>
        public abstract object Clone();

        /// <summary>
        /// Converts the condition object into python parsable code.
        /// </summary>
        /// <returns></returns>
        public abstract string ToPython();

        /// <summary>
        /// Converts the condition into a viewable TreeNode for the Criteria Editor
        /// </summary>
        /// <returns></returns>
        public virtual TreeNode ToTree() { return new TreeNode("empty"); }

        /// <summary>
        /// Converts a Timespan of seconds into Hours, Minutes, and Seconds
        /// </summary>
        /// <param name="seconds">Seconds to convert</param>
        /// <returns></returns>
        public static string Sec2hms(int seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            StringBuilder SB = new StringBuilder();
            char[] trim = new char[] { ',', ' ' };
            int Hours = t.Hours;

            // If we have more then 24 hours, then we need to
            // convert the days to hours
            if (t.Days > 0)
                Hours += t.Days * 24;

            // Format
            if (Hours > 0)
                SB.AppendFormat("{0} Hours, ", Hours);

            if (t.Minutes > 0)
                SB.AppendFormat("{0} Minutes, ", t.Minutes);

            if (t.Seconds > 0)
                SB.AppendFormat("{0} Seconds, ", t.Seconds);

            return SB.ToString().TrimEnd(trim);
        }
    }
}
