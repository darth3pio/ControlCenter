using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BF2Statistics.MedalData
{
    public partial class GlobalStatMultTimesForm : ConditionForm
    {
        /// <summary>
        /// Will contain an ObjectStat condition during editing.
        /// </summary>
        protected GlobalStatMultTimes Obj;

        protected TreeNode Node;

        protected string AwardId;

        public GlobalStatMultTimesForm(TreeNode Node)
        {
            InitializeComponent();

            // Set our node and object
            this.Node = Node;
            this.Obj = (GlobalStatMultTimes)Node.Tag;
            List<String> Params = Obj.GetParams();

            int j = 0;

            // Add each global item to the list
            foreach (KeyValuePair<string, string> I in Data.GlobalStrings)
            {
                StatName.Items.Add(new SelectItem(I.Key, I.Value));
                if (I.Key == Params[1])
                    StatName.SelectedIndex = j;
                j++;
            }

            // set condition value
            ValueBox.Value = Int32.Parse(Params[2]);
            AwardId = Params[3];
        }

        public GlobalStatMultTimesForm()
        {
            InitializeComponent();
            this.Node = new TreeNode();

            // Add each global item to the list
            foreach (KeyValuePair<string, string> I in Data.GlobalStrings)
                StatName.Items.Add(new SelectItem(I.Key, I.Value));

            StatName.SelectedIndex = 0;

            // Get our award ID from the main form
            IAward A = MedalDataEditor.SelectedAward;
            if (A is Award)
                AwardId = ((Award)MedalDataEditor.SelectedAward).Id;
            else
                AwardId = ((Rank)MedalDataEditor.SelectedAward).Id.ToString();
        }

        public override Condition GetCondition()
        {
            return Obj;
        }

        private void FinishBtn_Click(object sender, EventArgs e)
        {
            // Tell the base condition form that we modifed the condition
            base.Canceled = false;

            // First param is always the python method name
            List<string> Params = new List<string>();

            // Always add the python function name first
            Params.Add("global_stat_mult_times");

            // 1st param
            SelectItem I = (SelectItem)StatName.SelectedItem;
            Params.Add(I.Name);

            //2nd Param
            Params.Add(((int)ValueBox.Value).ToString());

            // 3rd Param
            Params.Add(AwardId);

            // Create the new Stat Object?
            if (Obj == null)
                Obj = new GlobalStatMultTimes(Params);
            else
                Obj.SetParams(Params);

            // Close the form
            this.Node.Tag = Obj;
            MedalDataEditor.ChangesMade = true;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Shows the Hours/Minutes/Seconds to Seconds form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hms2SecBtn_Click(object sender, EventArgs e)
        {
            S2hmsForm Form = new S2hmsForm();
            Form.SetTime(Int32.Parse(ValueBox.Value.ToString()));
            if (Form.ShowDialog() == DialogResult.OK)
            {
                ValueBox.Value = S2hmsForm.LastValue;
            }
        }
    }
}
