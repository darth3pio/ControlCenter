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
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SnapshotView = new System.Windows.Forms.ListView();
            this.ImportBtn = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SelectAllBtn = new System.Windows.Forms.Button();
            this.SelectNoneBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Process";
            this.columnHeader1.Width = 75;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Snapshot";
            this.columnHeader2.Width = 480;
            // 
            // SnapshotView
            // 
            this.SnapshotView.CheckBoxes = true;
            this.SnapshotView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.SnapshotView.FullRowSelect = true;
            this.SnapshotView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.SnapshotView.Location = new System.Drawing.Point(6, 61);
            this.SnapshotView.Name = "SnapshotView";
            this.SnapshotView.Size = new System.Drawing.Size(580, 250);
            this.SnapshotView.TabIndex = 0;
            this.SnapshotView.UseCompatibleStateImageBehavior = false;
            this.SnapshotView.View = System.Windows.Forms.View.Details;
            // 
            // ImportBtn
            // 
            this.ImportBtn.Location = new System.Drawing.Point(453, 327);
            this.ImportBtn.Name = "ImportBtn";
            this.ImportBtn.Size = new System.Drawing.Size(111, 30);
            this.ImportBtn.TabIndex = 1;
            this.ImportBtn.Text = "Import";
            this.ImportBtn.UseVisualStyleBackColor = true;
            this.ImportBtn.Click += new System.EventHandler(this.ImportBtn_Click);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(10, 20);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(575, 35);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = "Below is a list of  snapshots that have not been imported into the database. You " +
                "can select which snapshots you wish to try and import below";
            // 
            // SelectAllBtn
            // 
            this.SelectAllBtn.Location = new System.Drawing.Point(30, 327);
            this.SelectAllBtn.Name = "SelectAllBtn";
            this.SelectAllBtn.Size = new System.Drawing.Size(111, 30);
            this.SelectAllBtn.TabIndex = 3;
            this.SelectAllBtn.Text = "Select All";
            this.SelectAllBtn.UseVisualStyleBackColor = true;
            this.SelectAllBtn.Click += new System.EventHandler(this.SelectAllBtn_Click);
            // 
            // SelectNoneBtn
            // 
            this.SelectNoneBtn.Location = new System.Drawing.Point(147, 327);
            this.SelectNoneBtn.Name = "SelectNoneBtn";
            this.SelectNoneBtn.Size = new System.Drawing.Size(111, 30);
            this.SelectNoneBtn.TabIndex = 4;
            this.SelectNoneBtn.Text = "Select None";
            this.SelectNoneBtn.UseVisualStyleBackColor = true;
            this.SelectNoneBtn.Click += new System.EventHandler(this.SelectNoneBtn_Click);
            // 
            // SnapshotViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(594, 372);
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
            this.Text = "Unprocessed Snapshots";
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

    }
}