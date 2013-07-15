namespace BF2Statistics.MedalData
{
    partial class ConditionListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ConditionGrpBox = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ConditionTree = new System.Windows.Forms.TreeView();
            this.CriteriaRootMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.NewCriteriaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.UndoAllChangesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.ListTypeBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ValueBox = new System.Windows.Forms.NumericUpDown();
            this.EnableValue = new System.Windows.Forms.CheckBox();
            this.ConditionDescBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SaveBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.CriteriaItemMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.EditCritertiaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteCriteriaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ConditionGrpBox.SuspendLayout();
            this.CriteriaRootMenu.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ValueBox)).BeginInit();
            this.CriteriaItemMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConditionGrpBox
            // 
            this.ConditionGrpBox.Controls.Add(this.label5);
            this.ConditionGrpBox.Controls.Add(this.label4);
            this.ConditionGrpBox.Controls.Add(this.ConditionTree);
            this.ConditionGrpBox.Location = new System.Drawing.Point(12, 26);
            this.ConditionGrpBox.Name = "ConditionGrpBox";
            this.ConditionGrpBox.Size = new System.Drawing.Size(266, 262);
            this.ConditionGrpBox.TabIndex = 0;
            this.ConditionGrpBox.TabStop = false;
            this.ConditionGrpBox.Text = "Condition List";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 241);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(170, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "* Right click to open context menu";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 223);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(148, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "* Double click to edit a criteria";
            // 
            // ConditionTree
            // 
            this.ConditionTree.BackColor = System.Drawing.SystemColors.Window;
            this.ConditionTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ConditionTree.ContextMenuStrip = this.CriteriaRootMenu;
            this.ConditionTree.Location = new System.Drawing.Point(1, 27);
            this.ConditionTree.Name = "ConditionTree";
            this.ConditionTree.ShowPlusMinus = false;
            this.ConditionTree.Size = new System.Drawing.Size(263, 190);
            this.ConditionTree.TabIndex = 0;
            this.ConditionTree.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.ConditionTree_BeforeCollapse);
            this.ConditionTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ConditionTree_AfterSelect);
            this.ConditionTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ConditionTree_NodeMouseDoubleClick);
            this.ConditionTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ConditionTree_KeyDown);
            // 
            // CriteriaRootMenu
            // 
            this.CriteriaRootMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewCriteriaMenuItem,
            this.toolStripSeparator1,
            this.UndoAllChangesMenuItem});
            this.CriteriaRootMenu.Name = "CriteriaMenu";
            this.CriteriaRootMenu.Size = new System.Drawing.Size(165, 76);
            // 
            // NewCriteriaMenuItem
            // 
            this.NewCriteriaMenuItem.Name = "NewCriteriaMenuItem";
            this.NewCriteriaMenuItem.Size = new System.Drawing.Size(171, 22);
            this.NewCriteriaMenuItem.Text = "Add New Criteria";
            this.NewCriteriaMenuItem.Click += new System.EventHandler(this.NewCriteria_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(168, 6);
            // 
            // UndoAllChangesMenuItem
            // 
            this.UndoAllChangesMenuItem.Name = "UndoAllChangesMenuItem";
            this.UndoAllChangesMenuItem.Size = new System.Drawing.Size(171, 22);
            this.UndoAllChangesMenuItem.Text = "Undo Changes";
            this.UndoAllChangesMenuItem.Click += new System.EventHandler(this.UndoChanges_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "List Type:";
            // 
            // ListTypeBox
            // 
            this.ListTypeBox.BackColor = System.Drawing.SystemColors.Window;
            this.ListTypeBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ListTypeBox.Location = new System.Drawing.Point(76, 32);
            this.ListTypeBox.Name = "ListTypeBox";
            this.ListTypeBox.ReadOnly = true;
            this.ListTypeBox.Size = new System.Drawing.Size(150, 13);
            this.ListTypeBox.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ValueBox);
            this.groupBox1.Controls.Add(this.EnableValue);
            this.groupBox1.Controls.Add(this.ConditionDescBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.ListTypeBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(284, 26);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(246, 187);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Information";
            // 
            // ValueBox
            // 
            this.ValueBox.Enabled = false;
            this.ValueBox.Location = new System.Drawing.Point(151, 127);
            this.ValueBox.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.ValueBox.Name = "ValueBox";
            this.ValueBox.Size = new System.Drawing.Size(75, 20);
            this.ValueBox.TabIndex = 8;
            this.ValueBox.ValueChanged += new System.EventHandler(this.ValueBox_ValueChanged);
            // 
            // EnableValue
            // 
            this.EnableValue.AutoSize = true;
            this.EnableValue.Location = new System.Drawing.Point(20, 127);
            this.EnableValue.Name = "EnableValue";
            this.EnableValue.Size = new System.Drawing.Size(124, 17);
            this.EnableValue.TabIndex = 7;
            this.EnableValue.Text = "Enable Criteria Value";
            this.EnableValue.UseVisualStyleBackColor = true;
            this.EnableValue.CheckedChanged += new System.EventHandler(this.EnableValue_CheckedChanged);
            // 
            // ConditionDescBox
            // 
            this.ConditionDescBox.BackColor = System.Drawing.SystemColors.Window;
            this.ConditionDescBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ConditionDescBox.Location = new System.Drawing.Point(20, 76);
            this.ConditionDescBox.Multiline = true;
            this.ConditionDescBox.Name = "ConditionDescBox";
            this.ConditionDescBox.ReadOnly = true;
            this.ConditionDescBox.Size = new System.Drawing.Size(200, 45);
            this.ConditionDescBox.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Condition:";
            // 
            // SaveBtn
            // 
            this.SaveBtn.Location = new System.Drawing.Point(450, 244);
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Size = new System.Drawing.Size(80, 38);
            this.SaveBtn.TabIndex = 9;
            this.SaveBtn.Text = "Save";
            this.SaveBtn.UseVisualStyleBackColor = true;
            this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(360, 244);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(80, 38);
            this.CancelBtn.TabIndex = 10;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // CriteriaItemMenu
            // 
            this.CriteriaItemMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.EditCritertiaMenuItem,
            this.DeleteCriteriaMenuItem});
            this.CriteriaItemMenu.Name = "CriteriaItemMenu";
            this.CriteriaItemMenu.Size = new System.Drawing.Size(149, 48);
            // 
            // EditCritertiaMenuItem
            // 
            this.EditCritertiaMenuItem.Name = "EditCritertiaMenuItem";
            this.EditCritertiaMenuItem.Size = new System.Drawing.Size(148, 22);
            this.EditCritertiaMenuItem.Text = "Edit Criteria";
            this.EditCritertiaMenuItem.Click += new System.EventHandler(this.EditCriteria_Click);
            // 
            // DeleteCriteriaMenuItem
            // 
            this.DeleteCriteriaMenuItem.Name = "DeleteCriteriaMenuItem";
            this.DeleteCriteriaMenuItem.Size = new System.Drawing.Size(148, 22);
            this.DeleteCriteriaMenuItem.Text = "Delete Criteria";
            this.DeleteCriteriaMenuItem.Click += new System.EventHandler(this.DeleteCriteria_Click);
            // 
            // ConditionListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(543, 300);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.SaveBtn);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ConditionGrpBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConditionListForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Criteria List Editor";
            this.ConditionGrpBox.ResumeLayout(false);
            this.ConditionGrpBox.PerformLayout();
            this.CriteriaRootMenu.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ValueBox)).EndInit();
            this.CriteriaItemMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox ConditionGrpBox;
        private System.Windows.Forms.TreeView ConditionTree;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ListTypeBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox ConditionDescBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button SaveBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.CheckBox EnableValue;
        private System.Windows.Forms.NumericUpDown ValueBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ContextMenuStrip CriteriaItemMenu;
        private System.Windows.Forms.ToolStripMenuItem EditCritertiaMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DeleteCriteriaMenuItem;
        private System.Windows.Forms.ContextMenuStrip CriteriaRootMenu;
        private System.Windows.Forms.ToolStripMenuItem NewCriteriaMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem UndoAllChangesMenuItem;
    }
}