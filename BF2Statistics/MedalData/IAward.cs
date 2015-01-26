using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BF2Statistics.MedalData
{
    public interface IAward
    {
        void SetCondition(Condition C);
        Condition GetCondition();
        void RestoreDefaultConditions();
        void UndoConditionChanges();
        TreeNode ToTree();
        String ToPython();
    }
}
