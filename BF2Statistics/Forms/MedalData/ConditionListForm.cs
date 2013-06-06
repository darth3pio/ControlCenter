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
    public partial class ConditionListForm : ConditionForm
    {
        /// <summary>
        /// The condition list being edited
        /// </summary>
        protected ConditionList List;

        /// <summary>
        /// A clone of the original Condition List
        /// </summary>
        protected ConditionList OrigList;

        /// <summary>
        /// Our child form if applicable
        /// </summary>
        protected ConditionForm Child;

        /// <summary>
        /// TreeNode holding variable
        /// </summary>
        protected TreeNode Node;

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Node"></param>
        public ConditionListForm(TreeNode Node)
        {
            InitializeComponent();

            // Set internal vars
            this.Node = Node;
            this.List = (ConditionList) Node.Tag;
            this.OrigList = (ConditionList)this.List.Clone();

            Initialize();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Type"></param>
        public ConditionListForm(ConditionType Type)
        {
            InitializeComponent();

            // Set internal vars
            this.List = new ConditionList(Type);
            this.OrigList = new ConditionList(Type);
            this.Node = new TreeNode();

            Initialize();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Intializes the condition tree
        /// </summary>
        private void Initialize()
        {
            // Add Conditions to tree view
            ConditionTree.BeginUpdate();
            ConditionTree.Nodes.Clear();
            ConditionTree.Nodes.Add(List.ToTreeNoCollapse());
            ConditionTree.EndUpdate();
            ConditionTree.ExpandAll();

            // Get the list name
            ListTypeBox.Text = ConditionList.Names[(int)List.Type];

            // Proccess list descritpion
            if (List.Type == ConditionType.And || List.Type == ConditionType.Or)
            {
                ConditionDescBox.Text = "Can contain unlimited number of sub criteria's";
            }
            else
            {
                ConditionDescBox.Text = "Must contain ";
                ConditionDescBox.Text += (List.Type == ConditionType.Not)
                    ? "1 Sub Criteria."
                    : "2 Sub Criteria's. An optinal 3rd \"Value\" Criteria can be applied.";
            }

            // Hide value box's for unsupporting condition lists
            if (List.Type != ConditionType.Plus && List.Type != ConditionType.Div)
            {
                EnableValue.Visible = false;
                ValueBox.Visible = false;
            }
            else
            {
                // Get our condition value, and add its value to the valuebox
                List<Condition> CL = List.GetConditions();
                if (CL.Count == 3)
                {
                    EnableValue.Checked = true;
                    ValueBox.Value = Int32.Parse(CL[2].ToString());
                }
                else
                    EnableValue.Checked = false;
            }
        }

        /// <summary>
        /// Returns the condition list in its current state
        /// </summary>
        /// <returns></returns>
        public override Condition GetCondition()
        {
            return this.List;
        }

        /// <summary>
        /// Adds a new criteria to an award from the Add Critera Dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewCriteria(object sender, FormClosingEventArgs e)
        {
            Condition Add = Child.GetCondition();
            ConditionTree.Nodes[0].Nodes.Add(Add.ToTree());
            UpdateRoot();
        }

        /// <summary>
        /// Brings up the Criteria Editor for an Award
        /// </summary>
        public void EditCriteria()
        {
            TreeNode SelectedNode = ConditionTree.SelectedNode;

            // Make sure we have a node selected
            if (Node == null)
            {
                MessageBox.Show("Please select a criteria to edit.");
                return;
            }

            // Make sure its a child node
            if (SelectedNode.Parent == null && SelectedNode.Nodes.Count != 0)
                return;

            this.Node = SelectedNode;

            // Open correct condition editor form
            if (Node.Tag is ObjectStat)
                Child = new ObjectStatForm(Node);
            else if (Node.Tag is PlayerStat)
                Child = new ScoreStatForm(Node);
            else if (Node.Tag is MedalOrRankCondition)
                Child = new MedalConditionForm(Node);
            else if (Node.Tag is GlobalStatMultTimes)
                Child = new GlobalStatMultTimesForm(Node);
            else if (Node.Tag is ConditionList)
                Child = new ConditionListForm(Node);
            else
                return;

            if (Child.ShowDialog() == DialogResult.OK)
            {
                ConditionList NN = new ConditionList(List.Type);
                NN = (ConditionList) MedalDataParser.ParseNodeConditions(ConditionTree.Nodes[0]);

                ConditionTree.Nodes.Clear();
                ConditionTree.Nodes.Add(NN.ToTreeNoCollapse());
                ConditionTree.Refresh();
                ConditionTree.ExpandAll();
            }
        }

        /// <summary>
        /// Deletes the selected criteria node
        /// </summary>
        public void DeleteCriteria()
        {
            TreeNode Node = ConditionTree.SelectedNode;

            // Make sure we have a node selected
            if (Node == null)
            {
                MessageBox.Show("Please select a criteria to edit.");
                return;
            }

            // Dont delete on Plus / Div Trees
            if (!(Node.Tag is ConditionList))
            {
                TreeNode Parent = Node.Parent;
                if (Parent == null)
                    return;

                // If we are in the root condition list
                if (Parent.Parent == null || Parent.Parent.Tag == null)
                {
                    ConditionTree.Nodes.Remove(Node);
                    return;
                }

                // Get the parents condition list
                ConditionList C = (ConditionList)Parent.Tag;

                // Remove the whole tree if its a not statement
                if (C.Type == ConditionType.Not)
                    ConditionTree.Nodes.Remove(Parent);

                // We donot handle nested condition lists in this form
                else if (C.Type == ConditionType.Plus || C.Type == ConditionType.Div)
                {
                    ConditionTree.SelectedNode = Parent;
                    EditCriteria();
                }
                else
                {
                    ConditionTree.Nodes.Remove(Node);
                }
            }
            else
                ConditionTree.Nodes.Remove(Node);

            ConditionTree.Refresh();
        }

        private void UpdateRoot()
        {
            ConditionList NList = new ConditionList(List.Type);

            // Add existing nodes to the new list
            foreach (TreeNode E in ConditionTree.Nodes[0].Nodes)
                NList.Add((Condition)E.Tag);

            // Add condition value if enabled
            if (ValueBox.Enabled)
                NList.Add(new ConditionValue(ValueBox.Value.ToString()));

            // update tree
            ConditionTree.BeginUpdate();
            ConditionTree.Nodes.Clear();
            ConditionTree.Nodes.Add(NList.ToTreeNoCollapse());
            ConditionTree.ExpandAll();
            ConditionTree.EndUpdate();
        }

        #endregion Methods

        #region Button Events

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            if (List.Type == ConditionType.Not)
            {
                if (ConditionTree.Nodes[0].Nodes.Count == 0)
                {
                    MessageBox.Show("You must define a criteria.");
                    return;
                }
            }
            else
            {
                if (ConditionTree.Nodes[0].Nodes.Count < 2)
                {
                    MessageBox.Show("You must define 2 criteria's.");
                    return;
                }
            }

            List = (ConditionList)MedalDataParser.ParseNodeConditions(ConditionTree.Nodes[0]);
            Node.Tag = List;
            MedalDataEditor.ChangesMade = true;
            this.DialogResult = DialogResult.OK;
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion Button Events

        #region Value Box Events

        private void EnableValue_CheckedChanged(object sender, EventArgs e)
        {
            ValueBox.Enabled = (EnableValue.Checked);
            UpdateRoot();
        }

        private void ValueBox_ValueChanged(object sender, EventArgs e)
        {
            UpdateRoot();
        }

        #endregion Value Box Events

        #region Context Menu

        /// <summary>
        /// New Criteria menu item click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewCriteria_Click(object sender, EventArgs e)
        {
            if (List.Type == ConditionType.Plus || List.Type == ConditionType.Div)
            {
                if (ConditionTree.Nodes[0].Nodes.Count > 1)
                {
                    MessageBox.Show("Only 2 criteria's allowed per List.");
                    return;
                }
            }
            else if (List.Type == ConditionType.Not)
            {
                if (ConditionTree.Nodes[0].Nodes.Count == 2)
                {
                    MessageBox.Show("Only 1 criteria allowed per List.");
                    return;
                }
            }

            // Make sure we are only adding to the root condition list

            Child = new NewCriteriaForm();
            Child.FormClosing += new FormClosingEventHandler(Child_FormClosing);
            Child.Show();
        }

        /// <summary>
        /// Delete Criteria menu item click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteCriteria_Click(object sender, EventArgs e)
        {
            DeleteCriteria();
        }

        /// <summary>
        /// Edit Criteria menu item click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditCriteria_Click(object sender, EventArgs e)
        {
            EditCriteria();
        }

        /// <summary>
        /// Undo Changes menu item click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UndoChanges_Click(object sender, EventArgs e)
        {
            List = (ConditionList)OrigList.Clone();
            Initialize();
        }

        #endregion Context Menu

        #region Condition Tree Events

        /// <summary>
        /// This method builds the correct context menu options for the selected node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConditionTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // By default, we want the "New Criteria" item enabled
            CriteriaRootMenu.Items[0].Enabled = true;

            // Get the current selected node
            TreeNode CNode = ConditionTree.SelectedNode;
            if (CNode == null)
            {
                if (ConditionTree.Nodes.Count == 0)
                    return;
                else
                    CNode = ConditionTree.Nodes[0];
            }

            // Are we the parent node?
            if (CNode.Parent == null)
            {
                CNode.ContextMenuStrip = CriteriaRootMenu;
                if (CNode.Tag is ConditionList)
                {
                    ConditionList L = (ConditionList)CNode.Tag;
                    int Items = L.GetConditions().Count;
                    if (Items > 1 && L.Type != ConditionType.And)
                        CNode.ContextMenuStrip.Items[0].Enabled = false;
                }
            }
            else
                CNode.ContextMenuStrip = (CNode.Tag is ConditionList && ((ConditionList)CNode.Tag).Type == ConditionType.And)
                    ? CriteriaRootMenu
                    : CriteriaItemMenu;
        }

        /// <summary>
        /// Disables collapsing of condition tree nodes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConditionTree_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
        }

        /// <summary>
        /// Double click on a node... opens the edit criter menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConditionTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            EditCriteria();
        }

        /// <summary>
        /// Keyboard events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConditionTree_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                EditCriteria();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                DeleteCriteria();
                e.Handled = true;
            }
        }

        #endregion Condition Tree Events

        private void Child_FormClosing(object sender, FormClosingEventArgs e)
        {
            ConditionForm C = Child.GetConditionForm();
            if (C == null)
                return;

            // Hide closing New Criteria form, because setting "Child" to a new window
            // will still display the old window below it until the new child closes.
            Child.Visible = false;
            Child = C;
            Child.FormClosing += new FormClosingEventHandler(AddNewCriteria);
            Child.ShowDialog();
        }
    }
}
