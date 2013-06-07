using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BF2Statistics.MedalData
{
    public abstract class Condition : ICloneable
    {
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
    }
}
