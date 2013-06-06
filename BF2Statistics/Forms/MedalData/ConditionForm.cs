using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BF2Statistics.MedalData
{
    public partial class ConditionForm : Form
    {
        public bool Canceled = true;

        public virtual Condition GetCondition()
        {
            return null;
        }

        public virtual ConditionForm GetConditionForm()
        {
            return null;
        }
    }
}
