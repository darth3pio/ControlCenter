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
    public partial class ScoreStatForm : ConditionForm
    {
        /// <summary>
        /// Will contain an ObjectStat condition during editing.
        /// </summary>
        protected Condition Obj;

        protected TreeNode Node;

        public ScoreStatForm(TreeNode Node)
        {
            InitializeComponent();

            this.Node = Node;
            this.Obj = (Condition)Node.Tag;
            List<String> Params = Obj.GetParams();

            if (Params[0] == "global_stat")
                StatType.SelectedIndex = 0;
            else
                StatType.SelectedIndex = 1;

            // Update fields
            UpdateFields();

            // Proccess 2nd param
            int j = 0;
            foreach (object A in StatName.Items)
            {
                SelectItem I = (SelectItem)A;
                if (I.Name == Params[1])
                {
                    StatName.SelectedIndex = j;
                    break;
                }
                j++;
            }

            // Check for 3rd param
            if (Params.Count == 3)
            {
                ConditionSelect.SelectedIndex = 1;
                ValueBox.Visible = true;
                ValueBox.Value = Int32.Parse(Params[2]);
            }
            else
                ConditionSelect.SelectedIndex = 0;
        }

        public ScoreStatForm()
        {
            InitializeComponent();
            this.Node = new TreeNode();

            // Set default selected indexies
            StatType.SelectedIndex = 0;
            StatName.SelectedIndex = 0;
            ConditionSelect.SelectedIndex = 0;
        }

        public override Condition GetCondition()
        {
            return this.Obj;
        }

        /// <summary>
        /// Converts the data in the form into a condition, and closes the form
        /// </summary>
        public void Save()
        {
            // Tell the base condition form that we modifed the condition
            base.Canceled = false;

            // First param is always the python method name
            List<string> Params = new List<string>();

            // 1st param
            Params.Add( (StatType.SelectedIndex == 1) ? "player_stat" : "global_stat" );

            // 2nd param
            SelectItem I = (SelectItem)StatName.SelectedItem;
            Params.Add(I.Name);

            // 3rd Param (optional)
            if (ValueBox.Visible)
                Params.Add(((int)ValueBox.Value).ToString());

            // Create the new Stat Object
            if (Obj == null)
                Obj = new PlayerStat(Params);
            else
                Obj.SetParams(Params);

            // Close the form
            this.Node.Tag = Obj;
            MedalDataEditor.ChangesMade = true;
            this.DialogResult = DialogResult.OK;
        }


        public void UpdateFields()
        {
            this.Enabled = false;

            Dictionary<string, string> Fields = (StatType.SelectedIndex == 0) 
                ? Data.GlobalStrings 
                : Data.PlayerStrings;

            // Clear out old junk!
            StatName.Items.Clear();

            foreach(KeyValuePair<string, string> I in Fields)
                StatName.Items.Add(new SelectItem(I.Key, I.Value));

            this.Enabled = true;
        }

        private void StatType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateFields();
        }

        /// <summary>
        /// Used to hide the value box if the selected condition doesnt
        /// support a value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConditionSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValueBox.Visible = (ConditionSelect.SelectedIndex == 1);
            Hms2SecBtn.Visible = (ConditionSelect.SelectedIndex == 1);
        }

        private void FinishBtn_Click(object sender, EventArgs e)
        {
            Save();
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
