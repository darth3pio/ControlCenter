namespace BF2Statistics
{
    partial class SnapshotViewForm
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
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SnapshotView = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ImportBtn = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SelectAllBtn = new System.Windows.Forms.Button();
            this.SelectNoneBtn = new System.Windows.Forms.Button();
            this.MenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Details_MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MoveSnapshotMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewSelect = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.DeleteBtn = new System.Windows.Forms.Button();
            this.ServerOfflineWarning = new System.Windows.Forms.Label();
            this.linkLabelFilter = new System.Windows.Forms.LinkLabel();
            this.MenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Process";
            this.columnHeader1.Width = 75;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Map Name";
            this.columnHeader2.Width = 250;
            // 
            // SnapshotView
            // 
            this.SnapshotView.CheckBoxes = true;
            this.SnapshotView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.SnapshotView.FullRowSelect = true;
            this.SnapshotView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.SnapshotView.Location = new System.Drawing.Point(6, 127);
            this.SnapshotView.MultiSelect = false;
            this.SnapshotView.Name = "SnapshotView";
            this.SnapshotView.Size = new System.Drawing.Size(580, 250);
            this.SnapshotView.TabIndex = 0;
            this.SnapshotView.UseCompatibleStateImageBehavior = false;
            this.SnapshotView.View = System.Windows.Forms.View.Details;
            this.SnapshotView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SnapshotView_MouseClick);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Server Prefix";
            this.columnHeader3.Width = 83;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Date";
            this.columnHeader4.Width = 150;
            // 
            // ImportBtn
            // 
            this.ImportBtn.Location = new System.Drawing.Point(453, 393);
            this.ImportBtn.Name = "ImportBtn";
            this.ImportBtn.Size = new System.Drawing.Size(111, 30);
            this.ImportBtn.TabIndex = 1;
            this.ImportBtn.Text = "Process Selected";
            this.ImportBtn.UseVisualStyleBackColor = true;
            this.ImportBtn.Click += new System.EventHandler(this.ImportBtn_Click);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(10, 47);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(575, 35);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = "Below is a list of  snapshots that have not been imported into the database. You " +
    "can select which snapshots you wish to try and import below";
            // 
            // SelectAllBtn
            // 
            this.SelectAllBtn.Location = new System.Drawing.Point(30, 393);
            this.SelectAllBtn.Name = "SelectAllBtn";
            this.SelectAllBtn.Size = new System.Drawing.Size(111, 30);
            this.SelectAllBtn.TabIndex = 3;
            this.SelectAllBtn.Text = "Select All";
            this.SelectAllBtn.UseVisualStyleBackColor = true;
            this.SelectAllBtn.Click += new System.EventHandler(this.SelectAllBtn_Click);
            // 
            // SelectNoneBtn
            // 
            this.SelectNoneBtn.Location = new System.Drawing.Point(147, 393);
            this.SelectNoneBtn.Name = "SelectNoneBtn";
            this.SelectNoneBtn.Size = new System.Drawing.Size(111, 30);
            this.SelectNoneBtn.TabIndex = 4;
            this.SelectNoneBtn.Text = "Select None";
            this.SelectNoneBtn.UseVisualStyleBackColor = true;
            this.SelectNoneBtn.Click += new System.EventHandler(this.SelectNoneBtn_Click);
            // 
            // MenuStrip
            // 
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Details_MenuItem,
            this.MoveSnapshotMenuItem});
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(257, 48);
            // 
            // Details_MenuItem
            // 
            this.Details_MenuItem.Name = "Details_MenuItem";
            this.Details_MenuItem.Size = new System.Drawing.Size(256, 22);
            this.Details_MenuItem.Text = "View Game Details";
            this.Details_MenuItem.Click += new System.EventHandler(this.Details_MenuItem_Click);
            // 
            // MoveSnapshotMenuItem
            // 
            this.MoveSnapshotMenuItem.Name = "MoveSnapshotMenuItem";
            this.MoveSnapshotMenuItem.Size = new System.Drawing.Size(256, 22);
            this.MoveSnapshotMenuItem.Text = "Move To Processed / UnProcessed";
            this.MoveSnapshotMenuItem.Click += new System.EventHandler(this.MoveSnapshotMenuItem_Click);
            // 
            // ViewSelect
            // 
            this.ViewSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ViewSelect.FormattingEnabled = true;
            this.ViewSelect.Items.AddRange(new object[] {
            "Un-Processed (Incomplete) Snapshots",
            "Processed (Complete) Snapshots"});
            this.ViewSelect.Location = new System.Drawing.Point(236, 20);
            this.ViewSelect.Name = "ViewSelect";
            this.ViewSelect.Size = new System.Drawing.Size(210, 21);
            this.ViewSelect.TabIndex = 7;
            this.ViewSelect.SelectedIndexChanged += new System.EventHandler(this.ViewSelect_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(149, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Snapshot View:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(289, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "* Right click on a snaphot to view the Snapshot Game Data";
            // 
            // DeleteBtn
            // 
            this.DeleteBtn.Location = new System.Drawing.Point(335, 393);
            this.DeleteBtn.Name = "DeleteBtn";
            this.DeleteBtn.Size = new System.Drawing.Size(111, 30);
            this.DeleteBtn.TabIndex = 10;
            this.DeleteBtn.Text = "Delete Selected";
            this.DeleteBtn.UseVisualStyleBackColor = true;
            this.DeleteBtn.Click += new System.EventHandler(this.DeleteBtn_Click);
            // 
            // ServerOfflineWarning
            // 
            this.ServerOfflineWarning.AutoSize = true;
            this.ServerOfflineWarning.ForeColor = System.Drawing.Color.Red;
            this.ServerOfflineWarning.Location = new System.Drawing.Point(57, 83);
            this.ServerOfflineWarning.Name = "ServerOfflineWarning";
            this.ServerOfflineWarning.Size = new System.Drawing.Size(481, 13);
            this.ServerOfflineWarning.TabIndex = 11;
            this.ServerOfflineWarning.Text = "The ASP webserver is offline. You will be unable to import Snapshots until the AS" +
    "P server is enabled.";
            // 
            // linkLabelFilter
            // 
            this.linkLabelFilter.AutoSize = true;
            this.linkLabelFilter.Location = new System.Drawing.Point(494, 106);
            this.linkLabelFilter.Name = "linkLabelFilter";
            this.linkLabelFilter.Size = new System.Drawing.Size(88, 13);
            this.linkLabelFilter.TabIndex = 12;
            this.linkLabelFilter.TabStop = true;
            this.linkLabelFilter.Text = "<< Apply Filter >>";
            this.linkLabelFilter.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelFilter_LinkClicked);
            // 
            // SnapshotViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(594, 432);
            this.Controls.Add(this.linkLabelFilter);
            this.Controls.Add(this.ServerOfflineWarning);
            this.Controls.Add(this.DeleteBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ViewSelect);
            this.Controls.Add(this.SelectNoneBtn);
            this.Controls.Add(this.SelectAllBtn);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.ImportBtn);
            this.Controls.Add(this.SnapshotView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SnapshotViewForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Snapshots";
            this.MenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ListView SnapshotView;
        private System.Windows.Forms.Button ImportBtn;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button SelectAllBtn;
        private System.Windows.Forms.Button SelectNoneBtn;
        private System.Windows.Forms.ContextMenuStrip MenuStrip;
        private System.Windows.Forms.ToolStripMenuItem Details_MenuItem;
        private System.Windows.Forms.ComboBox ViewSelect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem MoveSnapshotMenuItem;
        private System.Windows.Forms.Button DeleteBtn;
        private System.Windows.Forms.Label ServerOfflineWarning;
        private System.Windows.Forms.LinkLabel linkLabelFilter;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;

    }
}