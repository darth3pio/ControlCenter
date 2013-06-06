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
    public partial class NewCriteriaForm : ConditionForm
    {
        protected ConditionForm ParentF = null;

        public NewCriteriaForm()
        {
            InitializeComponent();
        }

        public override ConditionForm GetConditionForm()
        {
            return ParentF;
        }

        #region Events

        /// <summary>
        /// Select Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnContinue_Click(object sender, EventArgs e)
        {
            if (GlobalStatRadio.Checked)
                this.ParentF = new ScoreStatForm();
            else if (ObjectStatRadio.Checked)
                this.ParentF = new ObjectStatForm();
            else if (AwardRadio.Checked)
                this.ParentF = new MedalConditionForm();
            else if (GlobalMultStatRadio.Checked)
                this.ParentF = new GlobalStatMultTimesForm();
            else if (GenericListRadio.Checked)
                this.ParentF = new ConditionListForm(ConditionType.And);
            else if (SumCriteriaList.Checked)
                this.ParentF = new ConditionListForm(ConditionType.Plus);
            else if (NotCriteriaList.Checked)
                this.ParentF = new ConditionListForm(ConditionType.Not);
            else if (DivCriteriaList.Checked)
                this.ParentF = new ConditionListForm(ConditionType.Div);
            else if (OrCriteriaList.Checked)
                this.ParentF = new ConditionListForm(ConditionType.Or);

            this.Close();
        }

        /// <summary>
        /// Cancel Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion Events

        #region Check Selection Methods

        private void UpdateCheckBoxesRight()
        {
            foreach (Control C in StatCriteriaBox.Controls)
            {
                RadioButton radioButton = C as RadioButton;
                if (radioButton != null)
                    radioButton.Checked = false;
            }
        }

        private void UpdateCheckBoxesLeft()
        {
            foreach (Control C in LogicalCriteriaBox.Controls)
            {
                RadioButton radioButton = C as RadioButton;
                if (radioButton != null)
                    radioButton.Checked = false;
            }
        }

        private void GlobalStatRadio_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckBoxesLeft();
            RadioButton Me = (RadioButton)sender;
            if (Me.Focused)
            {
                Me.Checked = true;
                DescTextBox.Text = "The player score criteria refers to score related criteria's, such as kills, deaths, "
                    + " global or round score, and global kit, weapon, and vehicle stats.";
                DescTextBox.SelectAll();
                DescTextBox.SelectionColor = Color.Black;
            }
        }

        private void ObjectStatRadio_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckBoxesLeft();
            RadioButton Me = (RadioButton)sender;
            if (Me.Focused)
            {
                Me.Checked = true;
                DescTextBox.Text = "The object stat criteria refers to the players weapon, kit, and vehicle related stats."
                    + " Object stats only reflects the current rounds stats for a given player. To get the global value, you will"
                    + " need to use the \"Player Global Score\" criteria. To get both the global value, and the round stat value, you will"
                    + " need to create a \"Sum\" Logical Criteria List, And add a Player Global Stat criteria, and Object Stat Criteria.";
                DescTextBox.SelectAll();
                DescTextBox.SelectionColor = Color.Black;
            }
        }

        private void AwardRadio_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckBoxesLeft();
            RadioButton Me = (RadioButton)sender;
            if (Me.Focused)
            {
                Me.Checked = true;
                DescTextBox.Text = "The Award / Rank criteria defines whether the player has met the requirments for an award or rank";
                DescTextBox.SelectAll();
                DescTextBox.SelectionColor = Color.Black;
            }
        }

        private void GlobalMultStatRadio_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckBoxesLeft();
            RadioButton Me = (RadioButton)sender;
            if (Me.Focused)
            {
                Me.Checked = true;
                DescTextBox.Text = "Global Stat Multiple Times is a special criteria. It defines that, each time the criteria is met, the player "
                    + "can recieve the award agian. An example would be, If you created a Global Stat Mult Times criteria, and set its criteria "
                    + "to 5,000 kills... Every time the player reaches 5,000 kills, he will recieve the award again.";
                DescTextBox.SelectAll();
                DescTextBox.SelectionColor = Color.Black;
            }
        }

        private void GenericListRadio_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckBoxesRight();
            RadioButton Me = (RadioButton)sender;
            if (Me.Focused)
            {
                Me.Checked = true;
                DescTextBox.Text = "The Generic Logical Criteria List takes 2 or more Stat Criteria definitions."
                    + " This criteria is only met if ALL sub critiera's are met. A Generic list is mostly used when "
                    + " there are multiple criteria's to be met to earn an award.";
                DescTextBox.SelectAll();
                DescTextBox.SelectionColor = Color.Black;
            }
        }

        private void SumCriteriaList_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckBoxesRight();
            RadioButton Me = (RadioButton)sender;
            if (Me.Focused)
            {
                Me.Checked = true;
                DescTextBox.Text = "The Sum Logical Criteria List takes 2 Stat Criteria definitions, and adds their values together."
                    + " An example of use would be getting the players global score, and round score totals. A third \"Value Condition\""
                    + " Can be defined. If a Value Condition is defined, this this criteria will be met if the 2 Stat Criteria's total value"
                    + " equals, or is greater than the condition value.";
                DescTextBox.SelectAll();
                DescTextBox.SelectionColor = Color.Black;
            }
        }

        private void NotCriteriaList_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckBoxesRight();
            RadioButton Me = (RadioButton)sender;
            if (Me.Focused)
            {
                Me.Checked = true;
                DescTextBox.Text = "The Zero Logical Criteria is met only if the sub criteria is NOT met,";
                DescTextBox.SelectAll();
                DescTextBox.SelectionColor = Color.Black;
            }
        }

        private void DivCriteriaList_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckBoxesRight();
            RadioButton Me = (RadioButton)sender;
            if (Me.Focused)
            {
                Me.Checked = true;
                DescTextBox.Text = "The Division Logical Criteria List takes 2 Stat Criteria definitions, and divides their values."
                    + " An example of use would be getting the players Kills to Deaths ratio. A third \"Value Condition\""
                    + " Can be defined. If a Value Condition is defined, this this criteria will be met if the 2 Stat Criteria's total value"
                    + " equals, or is greater than the condition value.";
                DescTextBox.SelectAll();
                DescTextBox.SelectionColor = Color.Black;
            }
        }

        private void OrCriteriaList_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckBoxesRight();
            RadioButton Me = (RadioButton)sender;
            if (Me.Focused)
            {
                Me.Checked = true;
                DescTextBox.Text = "The \"Or\" Logical Criteria List takes 2 or more Stat Criteria definitions."
                    + " This condition is met if any of the Stat Criteria's under it are met.";
                DescTextBox.SelectAll();
                DescTextBox.SelectionColor = Color.Black;
            }
        }

        #endregion Check Selection Methods
    }
}
