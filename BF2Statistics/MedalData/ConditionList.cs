using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BF2Statistics.MedalData
{
    /// <summary>
    /// Different condition list types
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

        /// <summary>
        /// A list of "ConditionType" => "Name"
        /// </summary>
        public static Dictionary<int, string> Names = new Dictionary<int, string>()
        {
            {0, "Generic"},
            {1, "Is False or Zero"},
            {2, "Meets any sub requirement"},
            {3, "Sum"},
            {4, "Division"},
        };

        /// <summary>
        /// Returns whether the condition list has any condition errors (recursive). 
        /// Must use the <see cref="ToTree()"/> method to refresh this value
        /// </summary>
        public bool HasConditionErrors { get; protected set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Type">The list ConditionType</param>
        public ConditionList(ConditionType Type) 
        {
            this.Type = Type;
            this.HasConditionErrors = false;
        }

        /// <summary>
        ///  Adds a new sub condition under this list
        /// </summary>
        /// <param name="Condition"></param>
        public void Add(Condition Condition)
        {
            SubConditions.Add(Condition);
        }

        /// <summary>
        /// Removes all sub-conditions under this list
        /// </summary>
        public void Clear()
        {
            SubConditions.Clear();
        }

        /// <summary>
        /// Returns a list of sub-conditions under this list
        /// </summary>
        /// <returns></returns>
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
        /// Required by Condition abstract class. Donot use.
        /// </summary>
        /// <param name="Params">Not Used in a condition list!! Use <see cref="Add()"/></param>
        public override void SetParams(List<string> Params) { }

        /// <summary>
        /// Returns the return value of this condition
        /// </summary>
        public override ReturnType Returns()
        {
            switch (Type)
            {
                case ConditionType.And:
                    return (SubConditions.Count == 1) ? SubConditions[0].Returns() : ReturnType.Bool;
                case ConditionType.Plus:
                case ConditionType.Div:
                    return (SubConditions.Count == 3) ? ReturnType.Bool : ReturnType.Number;
                default:
                    return ReturnType.Bool;
            }
        }

        /// <summary>
        /// Returns a copy (clone) of this object
        /// </summary>
        public override object Clone()
        {
            ConditionList Clone = new ConditionList(this.Type);
            foreach (Condition Cond in SubConditions)
                Clone.Add(Cond.Clone() as Condition);

            return Clone as object;
        }

        /// <summary>
        /// Converts this list and all sub-conditions into a python string
        /// </summary>
        /// <returns>A python representation of this condition list</returns>
        public override string ToPython() 
        {
            // If there is no sub conditions, return true by default...
            if (SubConditions.Count == 0)
                return "true";
            // If there is only 1 sub condition in a AND list, just return it
            else if (Type == ConditionType.And && SubConditions.Count == 1)
                return SubConditions[0].ToPython();

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

            return SB.ToString().TrimEnd(new char[2] { ',', ' ' }) + ")"; 
        }

        /// <summary>
        /// Converts the list to tree view. If there is only 1 sub criteria
        /// on an "And" or "Or" type list, then the list will not collapse into the
        /// sub criteria. Invalid Sub condition nodes will be highlighted in Red.
        /// </summary>
        /// <returns>A TreeNode representation of the Conditions in this list</returns>
        public override TreeNode ToTree()
        {
            // Define vars
            string Name = "Meets All Requirements:";
            bool Trim = false;
            int i = 0;
            HasConditionErrors = false;

            // Build the name which will be displayed in the criteria view
            switch (this.Type)
            {
                case ConditionType.Div:
                    if (SubConditions.Count == 3)
                    {
                        // If a div condition has 3 conditions, the last is always a condition value
                        ConditionValue Cnd = (ConditionValue)SubConditions.Last();
                        Name = "Conditions Divided Equal to or Greater than " + Cnd.Value;
                        Trim = true;
                    }
                    else
                        Name = "Divided Value Of:";
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
                        // If a plus condition has 3 conditions, the last is always a condition value
                        ConditionValue Cnd = (ConditionValue)SubConditions.Last();
                        Name = "Condtions Equal to or Greater than " + String.Format("{0:N0}", Int32.Parse(Cnd.Value));
                        Trim = true;
                    }
                    else
                        Name = "The Sum Of:";
                    break;
            }

            // Start the TreeNode, and add this condition list in the tag
            TreeNode Me = new TreeNode(Name);
            Me.Tag = this;

            // Add sub conditions to the nodes of this condition's tree node
            foreach (Condition C in SubConditions)
            {
                // Obviously dont add null items
                if (C == null)
                    continue;

                // Make sure not to add the last element on a plus conditionlist
                if (Trim && C is ConditionValue)
                    break;

                // Convert sub-condition to tree
                TreeNode N = C.ToTree();
                if (N == null)
                    continue;

                // Nested errors
                if (!HasConditionErrors && C is ConditionList)
                    HasConditionErrors = (C as ConditionList).HasConditionErrors;

                // Validation
                if (!ValidateParam(i, C.Returns()))
                {
                    N.ForeColor = System.Drawing.Color.Red;
                    HasConditionErrors = true;
                    N.ToolTipText = "Invalid Return Type. ";
                    N.ToolTipText += (C.Returns() == ReturnType.Bool) 
                        ? "A criteria value is required" 
                        : "Criteria value must be disabled";
                }
                else
                {
                    // Always reset this
                    N.ToolTipText = "";
                }

                // Add the node
                Me.Nodes.Add(N);
                i++;
            }

            return Me;
        }

        /// <summary>
        /// Validates that the given parameter is a valid return type to make this
        /// condition list work properly
        /// </summary>
        /// <param name="ParamId">The current parameter index</param>
        /// <param name="RType">The return type if the current parameter</param>
        protected bool ValidateParam(int ParamId, ReturnType RType)
        {
            switch (Type)
            {
                case ConditionType.And:
                case ConditionType.Or:
                case ConditionType.Not:
                    return (RType == ReturnType.Bool);
                case ConditionType.Plus:
                case ConditionType.Div:
                    return (RType == ReturnType.Number);
            }

            return true;
        }
    }
}
