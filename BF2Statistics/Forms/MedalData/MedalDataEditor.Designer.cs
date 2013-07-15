namespace BF2Statistics.MedalData
{
    partial class MedalDataEditor
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Badges");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Medals");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Ribbons");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Ranks");
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.AwardConditionsTree = new System.Windows.Forms.TreeView();
            this.CriteriaRootMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.NewCriteriaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.UndoAllChangesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RestoreCriteriaMenuBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.AwardPictureBox = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.AwardTree = new System.Windows.Forms.TreeView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.ActivateProfileBtn = new System.Windows.Forms.Button();
            this.SaveBtn = new System.Windows.Forms.Button();
            this.DelProfileBtn = new System.Windows.Forms.Button();
            this.NewProfileBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ProfileSelector = new System.Windows.Forms.ComboBox();
            this.Tipsy = new System.Windows.Forms.ToolTip(this.components);
            this.CriteriaItemMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.EditCritertiaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteCriteriaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AwardNameBox = new System.Windows.Forms.Label();
            this.AwardTypeBox = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.CriteriaRootMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AwardPictureBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.CriteriaItemMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.AwardTypeBox);
            this.groupBox2.Controls.Add(this.AwardNameBox);
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.AwardPictureBox);
            this.groupBox2.Location = new System.Drawing.Point(276, 14);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(490, 300);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Award Information";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.AwardConditionsTree);
            this.groupBox4.Location = new System.Drawing.Point(206, 16);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(271, 273);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Criteria";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 246);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(170, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "* Right click to open context menu";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 228);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(148, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "* Double click to edit a criteria";
            // 
            // AwardConditionsTree
            // 
            this.AwardConditionsTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.AwardConditionsTree.ContextMenuStrip = this.CriteriaRootMenu;
            this.AwardConditionsTree.Enabled = false;
            this.AwardConditionsTree.Location = new System.Drawing.Point(2, 19);
            this.AwardConditionsTree.Name = "AwardConditionsTree";
            this.AwardConditionsTree.ShowPlusMinus = false;
            this.AwardConditionsTree.Size = new System.Drawing.Size(267, 200);
            this.AwardConditionsTree.TabIndex = 8;
            this.AwardConditionsTree.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.AwardConditionsTree_BeforeCollapse);
            this.AwardConditionsTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.AwardConditionsTree_AfterSelect);
            this.AwardConditionsTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.AwardConditionsTree_NodeMouseDoubleClick);
            this.AwardConditionsTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AwardConditionsTree_KeyDown);
            // 
            // CriteriaRootMenu
            // 
            this.CriteriaRootMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewCriteriaMenuItem,
            this.toolStripSeparator1,
            this.UndoAllChangesMenuItem,
            this.RestoreCriteriaMenuBtn});
            this.CriteriaRootMenu.Name = "CriteriaMenu";
            this.CriteriaRootMenu.Size = new System.Drawing.Size(172, 76);
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
            this.UndoAllChangesMenuItem.Click += new System.EventHandler(this.UndoAllChangesMenuItem_Click);
            // 
            // RestoreCriteriaMenuBtn
            // 
            this.RestoreCriteriaMenuBtn.Name = "RestoreCriteriaMenuBtn";
            this.RestoreCriteriaMenuBtn.Size = new System.Drawing.Size(171, 22);
            this.RestoreCriteriaMenuBtn.Text = "Restore To Default";
            this.RestoreCriteriaMenuBtn.Click += new System.EventHandler(this.RestoreToDefaultMenuItem_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(13, 252);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Award Type:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 205);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Award Name:";
            // 
            // AwardPictureBox
            // 
            this.AwardPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.AwardPictureBox.Location = new System.Drawing.Point(16, 25);
            this.AwardPictureBox.Name = "AwardPictureBox";
            this.AwardPictureBox.Size = new System.Drawing.Size(175, 175);
            this.AwardPictureBox.TabIndex = 1;
            this.AwardPictureBox.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.AwardTree);
            this.groupBox1.Location = new System.Drawing.Point(8, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(250, 300);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Awards and Ranks";
            // 
            // AwardTree
            // 
            this.AwardTree.BackColor = System.Drawing.SystemColors.Window;
            this.AwardTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.AwardTree.Enabled = false;
            this.AwardTree.Location = new System.Drawing.Point(9, 20);
            this.AwardTree.Name = "AwardTree";
            treeNode1.Name = "BadgeNode";
            treeNode1.Text = "Badges";
            treeNode2.Name = "MedalNode";
            treeNode2.Text = "Medals";
            treeNode3.Name = "RibbonNode";
            treeNode3.Text = "Ribbons";
            treeNode4.Name = "RankNode";
            treeNode4.Text = "Ranks";
            this.AwardTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4});
            this.AwardTree.Size = new System.Drawing.Size(230, 260);
            this.AwardTree.TabIndex = 0;
            this.AwardTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.AwardTree_AfterSelect);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.ActivateProfileBtn);
            this.groupBox3.Controls.Add(this.SaveBtn);
            this.groupBox3.Controls.Add(this.DelProfileBtn);
            this.groupBox3.Controls.Add(this.NewProfileBtn);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.ProfileSelector);
            this.groupBox3.Location = new System.Drawing.Point(8, 316);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(757, 60);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            // 
            // ActivateProfileBtn
            // 
            this.ActivateProfileBtn.BackgroundImage = global::BF2Statistics.Properties.Resources.power;
            this.ActivateProfileBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ActivateProfileBtn.Enabled = false;
            this.ActivateProfileBtn.Location = new System.Drawing.Point(501, 20);
            this.ActivateProfileBtn.Name = "ActivateProfileBtn";
            this.ActivateProfileBtn.Padding = new System.Windows.Forms.Padding(7, 0, 0, 0);
            this.ActivateProfileBtn.Size = new System.Drawing.Size(150, 24);
            this.ActivateProfileBtn.TabIndex = 5;
            this.ActivateProfileBtn.Text = "Set as Server Profile";
            this.Tipsy.SetToolTip(this.ActivateProfileBtn, "Set current profile as the active medal data profile. The active profile\'s\r\nmedal" +
                    " data is used when the server is started.");
            this.ActivateProfileBtn.UseVisualStyleBackColor = true;
            this.ActivateProfileBtn.Click += new System.EventHandler(this.ActivateProfileBtn_Click);
            // 
            // SaveBtn
            // 
            this.SaveBtn.BackgroundImage = global::BF2Statistics.Properties.Resources.Save;
            this.SaveBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.SaveBtn.Enabled = false;
            this.SaveBtn.Location = new System.Drawing.Point(657, 19);
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Padding = new System.Windows.Forms.Padding(7, 0, 0, 0);
            this.SaveBtn.Size = new System.Drawing.Size(75, 24);
            this.SaveBtn.TabIndex = 4;
            this.SaveBtn.Text = "Save";
            this.SaveBtn.UseVisualStyleBackColor = true;
            this.SaveBtn.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // DelProfileBtn
            // 
            this.DelProfileBtn.BackgroundImage = global::BF2Statistics.Properties.Resources.cross;
            this.DelProfileBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.DelProfileBtn.Enabled = false;
            this.DelProfileBtn.Location = new System.Drawing.Point(369, 21);
            this.DelProfileBtn.Name = "DelProfileBtn";
            this.DelProfileBtn.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.DelProfileBtn.Size = new System.Drawing.Size(80, 24);
            this.DelProfileBtn.TabIndex = 3;
            this.DelProfileBtn.Text = "Delete";
            this.Tipsy.SetToolTip(this.DelProfileBtn, "Delete current medal data profile");
            this.DelProfileBtn.UseVisualStyleBackColor = true;
            this.DelProfileBtn.Click += new System.EventHandler(this.DelProfileBtn_Click);
            // 
            // NewProfileBtn
            // 
            this.NewProfileBtn.BackgroundImage = global::BF2Statistics.Properties.Resources.plus;
            this.NewProfileBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.NewProfileBtn.Location = new System.Drawing.Point(288, 21);
            this.NewProfileBtn.Name = "NewProfileBtn";
            this.NewProfileBtn.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.NewProfileBtn.Size = new System.Drawing.Size(75, 24);
            this.NewProfileBtn.TabIndex = 2;
            this.NewProfileBtn.Text = "New";
            this.Tipsy.SetToolTip(this.NewProfileBtn, "Create a new medal data profile");
            this.NewProfileBtn.UseVisualStyleBackColor = true;
            this.NewProfileBtn.Click += new System.EventHandler(this.NewProfileBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Medal Data Profile:";
            // 
            // ProfileSelector
            // 
            this.ProfileSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ProfileSelector.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProfileSelector.FormattingEnabled = true;
            this.ProfileSelector.Location = new System.Drawing.Point(120, 22);
            this.ProfileSelector.Name = "ProfileSelector";
            this.ProfileSelector.Size = new System.Drawing.Size(162, 24);
            this.ProfileSelector.TabIndex = 0;
            this.ProfileSelector.SelectedIndexChanged += new System.EventHandler(this.ProfileSelector_SelectedIndexChanged);
            // 
            // Tipsy
            // 
            this.Tipsy.AutoPopDelay = 8000;
            this.Tipsy.InitialDelay = 500;
            this.Tipsy.ReshowDelay = 100;
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
            // AwardNameBox
            // 
            this.AwardNameBox.AutoEllipsis = true;
            this.AwardNameBox.Location = new System.Drawing.Point(16, 225);
            this.AwardNameBox.MaximumSize = new System.Drawing.Size(180, 13);
            this.AwardNameBox.MinimumSize = new System.Drawing.Size(180, 13);
            this.AwardNameBox.Name = "AwardNameBox";
            this.AwardNameBox.Size = new System.Drawing.Size(180, 13);
            this.AwardNameBox.TabIndex = 7;
            // 
            // AwardTypeBox
            // 
            this.AwardTypeBox.AutoEllipsis = true;
            this.AwardTypeBox.Location = new System.Drawing.Point(16, 270);
            this.AwardTypeBox.MaximumSize = new System.Drawing.Size(180, 13);
            this.AwardTypeBox.MinimumSize = new System.Drawing.Size(180, 13);
            this.AwardTypeBox.Name = "AwardTypeBox";
            this.AwardTypeBox.Size = new System.Drawing.Size(180, 13);
            this.AwardTypeBox.TabIndex = 8;
            // 
            // MedalDataEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(774, 382);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MedalDataEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Medal Data Editor";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.CriteriaRootMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.AwardPictureBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.CriteriaItemMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TreeView AwardConditionsTree;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox AwardPictureBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TreeView AwardTree;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ProfileSelector;
        private System.Windows.Forms.Button DelProfileBtn;
        private System.Windows.Forms.Button NewProfileBtn;
        private System.Windows.Forms.Button SaveBtn;
        private System.Windows.Forms.Button ActivateProfileBtn;
        private System.Windows.Forms.ToolTip Tipsy;
        private System.Windows.Forms.ContextMenuStrip CriteriaRootMenu;
        private System.Windows.Forms.ToolStripMenuItem RestoreCriteriaMenuBtn;
        private System.Windows.Forms.ContextMenuStrip CriteriaItemMenu;
        private System.Windows.Forms.ToolStripMenuItem EditCritertiaMenuItem;
        private System.Windows.Forms.ToolStripMenuItem UndoAllChangesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem NewCriteriaMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem DeleteCriteriaMenuItem;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label AwardNameBox;
        private System.Windows.Forms.Label AwardTypeBox;
    }
}