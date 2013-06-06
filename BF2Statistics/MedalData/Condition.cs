using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BF2Statistics.MedalData
{
    public abstract class Condition : ICloneable
    {
        public abstract List<String> GetParams();
        public abstract void SetParams(List<string> Params);
        public abstract object Clone();
        public abstract string ToPython();
        public virtual string ToPython(int level) { return ""; }
        public virtual TreeNode ToTree() { return new TreeNode("empty"); }
    }
}
