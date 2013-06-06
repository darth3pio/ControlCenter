using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BF2Statistics.MedalData
{
    /// <summary>
    /// Different condition types
    /// </summary>
    public enum ConditionType : int
    {
        And,
        Not,
        Or,
        Plus,
        Div
    }

    public class ConditionList : Condition
    {
        /// <summary>
        /// This objects condition type
        /// </summary>
        public ConditionType Type { get; protected set; }

        /// <summary>
        /// List of sub conditions
        /// </summary>
        protected List<Condition> SubConditions = new List<Condition>();

        public static Dictionary<int, string> Names = new Dictionary<int, string>()
        {
            {0, "Generic"},
            {1, "Is False or Zero"},
            {2, "Meets any sub requirement"},
            {3, "Sum"},
            {4, "Division"},
        };



        public ConditionList(ConditionType Type) 
        {
            this.Type = Type;
        }

        public void Add(Condition Condition)
        {
            SubConditions.Add(Condition);
        }

        public void Clear()
        {
            SubConditions.Clear();
        }

        public List<Condition> GetConditions()
        {
            return this.SubConditions;
        }

        /// <summary>
        /// Returns a list of parameters for this condition
        /// </summary>
        /// <returns></returns>
        public override List<string> GetParams()
        {
            return null;
        }

        /// <summary>
        /// Sets the params for this condition
        /// </summary>
        /// <param name="Params"></param>
        public override void SetParams(List<string> Params)
        {
            
        }

        /// <summary>
        /// Returns a copy (clone) of this object
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            ConditionList Clone = new ConditionList(this.Type);
            foreach (Condition Cond in SubConditions)
                Clone.Add(Cond.Clone() as Condition);

            return Clone as object;
        }

        public override string ToPython() 
        {
            if (SubConditions.Count == 0)
                return "true";
            else if (Type == ConditionType.And && SubConditions.Count == 1)
                return SubConditions[0].ToPython();

            char[] trim = new char[2] { ',', ' ' };
            StringBuilder SB = new StringBuilder();

            switch (Type)
            {
                case ConditionType.And:
                    SB.Append("f_and(");
                    break;
                case ConditionType.Div:
                    SB.Append("f_div(");
                    break;
                case ConditionType.Not:
                    SB.Append("f_not(");
                    break;
                case ConditionType.Or:
                    SB.Append("f_or(");
                    break;
                case ConditionType.Plus:
                    SB.Append("f_plus(");
                    break;
            }


            foreach (Condition C in SubConditions)
                SB.Append(C.ToPython() + ", ");

            return SB.ToString().TrimEnd(trim) + ")"; 
        }

        /// <summary>
        /// Converts the list to tree view. If there is only 1 sub criteria
        /// on an "And" or "Or" type list, then the list will not collapse into the
        /// sub criteria
        /// </summary>
        /// <returns></returns>
        public TreeNode ToTreeNoCollapse()
        {
            // Get Name
            string Name = "Meets All Requirements:";
            bool Trim = false;
            switch (this.Type)
            {
                case ConditionType.Div:
                    if (SubConditions.Count == 3)
                    {
                        ConditionValue Cnd = (ConditionValue)SubConditions.Last();
                        Name = "Conditions Divided >= " + Cnd.Value;
                        Trim = true;
                    }
                    else
                    {
                        Name = "Divided Value Of:";
                    }
                    break;
                case ConditionType.Not:
                    Name = "Does Not Meet Criteria:";
                    break;
                case ConditionType.Or:
                    Name = "Meets Any Criteria:";
                    break;
                case ConditionType.Plus:
                    if (SubConditions.Count == 3)
                    {
                        ConditionValue Cnd = (ConditionValue)SubConditions.Last();
                        Name = "Condtions Add Up To " + String.Format("{0:N0}", Int32.Parse(Cnd.Value));
                        Trim = true;
                    }
                    else
                    {
                        Name = "Sum Of:";
                    }
                    break;
            }

            TreeNode Me = new TreeNode(Name);
            Me.Tag = this;

            int i = 0;
            foreach (Condition C in SubConditions)
            {
                // Make sure not to add the last element on a plus
                if (Trim)
                {
                    if (C is ConditionValue)
                        break;
                }

                if (C == null)
                    continue;

                TreeNode N = C.ToTree();
                if (N == null)
                    continue;

                Me.Nodes.Add(N);
                i++;
            }

            return Me;
        }

        /// <summary>
        /// Converts the condition list to tree view
        /// </summary>
        /// <returns></returns>
        public override TreeNode ToTree()
        {
            // If we have just 1 sub condition, return it
            //if (Type != ConditionType.Not && SubConditions.Count == 1)
                //return SubConditions[0].ToTree();

            return ToTreeNoCollapse();
        }

        //public string override ToString() { return " ";  }
    }
}
