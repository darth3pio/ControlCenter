namespace BF2Statistics.MedalData
{
    partial class NewCriteriaForm
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
            this.BtnContinue = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.StatCriteriaBox = new System.Windows.Forms.GroupBox();
            this.GlobalMultStatRadio = new System.Windows.Forms.RadioButton();
            this.GlobalStatRadio = new System.Windows.Forms.RadioButton();
            this.AwardRadio = new System.Windows.Forms.RadioButton();
            this.ObjectStatRadio = new System.Windows.Forms.RadioButton();
            this.LogicalCriteriaBox = new System.Windows.Forms.GroupBox();
            this.SumCriteriaList = new System.Windows.Forms.RadioButton();
            this.DivCriteriaList = new System.Windows.Forms.RadioButton();
            this.NotCriteriaList = new System.Windows.Forms.RadioButton();
            this.GenericListRadio = new System.Windows.Forms.RadioButton();
            this.OrCriteriaList = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.DescTextBox = new System.Windows.Forms.RichTextBox();
            this.StatCriteriaBox.SuspendLayout();
            this.LogicalCriteriaBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnContinue
            // 
            this.BtnContinue.Location = new System.Drawing.Point(298, 271);
            this.BtnContinue.Name = "BtnContinue";
            this.BtnContinue.Size = new System.Drawing.Size(85, 30);
            this.BtnContinue.TabIndex = 14;
            this.BtnContinue.Text = "Select";
            this.BtnContinue.UseVisualStyleBackColor = true;
            this.BtnContinue.Click += new System.EventHandler(this.BtnContinue_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(203, 271);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(85, 30);
            this.button1.TabIndex = 13;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // StatCriteriaBox
            // 
            this.StatCriteriaBox.Controls.Add(this.GlobalMultStatRadio);
            this.StatCriteriaBox.Controls.Add(this.GlobalStatRadio);
            this.StatCriteriaBox.Controls.Add(this.AwardRadio);
            this.StatCriteriaBox.Controls.Add(this.ObjectStatRadio);
            this.StatCriteriaBox.Location = new System.Drawing.Point(203, 145);
            this.StatCriteriaBox.Name = "StatCriteriaBox";
            this.StatCriteriaBox.Size = new System.Drawing.Size(180, 120);
            this.StatCriteriaBox.TabIndex = 12;
            this.StatCriteriaBox.TabStop = false;
            this.StatCriteriaBox.Text = "Stat Criteria";
            // 
            // GlobalMultStatRadio
            // 
            this.GlobalMultStatRadio.AutoSize = true;
            this.GlobalMultStatRadio.Location = new System.Drawing.Point(13, 93);
            this.GlobalMultStatRadio.Name = "GlobalMultStatRadio";
            this.GlobalMultStatRadio.Size = new System.Drawing.Size(131, 17);
            this.GlobalMultStatRadio.TabIndex = 9;
            this.GlobalMultStatRadio.Text = "Global Stat Mult Times";
            this.GlobalMultStatRadio.UseVisualStyleBackColor = true;
            this.GlobalMultStatRadio.CheckedChanged += new System.EventHandler(this.GlobalMultStatRadio_CheckedChanged);
            // 
            // GlobalStatRadio
            // 
            this.GlobalStatRadio.AutoSize = true;
            this.GlobalStatRadio.Checked = true;
            this.GlobalStatRadio.Location = new System.Drawing.Point(13, 24);
            this.GlobalStatRadio.Name = "GlobalStatRadio";
            this.GlobalStatRadio.Size = new System.Drawing.Size(161, 17);
            this.GlobalStatRadio.TabIndex = 5;
            this.GlobalStatRadio.TabStop = true;
            this.GlobalStatRadio.Text = "Player Global / Round Score";
            this.GlobalStatRadio.UseVisualStyleBackColor = true;
            this.GlobalStatRadio.CheckedChanged += new System.EventHandler(this.GlobalStatRadio_CheckedChanged);
            // 
            // AwardRadio
            // 
            this.AwardRadio.AutoSize = true;
            this.AwardRadio.Location = new System.Drawing.Point(13, 70);
            this.AwardRadio.Name = "AwardRadio";
            this.AwardRadio.Size = new System.Drawing.Size(155, 17);
            this.AwardRadio.TabIndex = 8;
            this.AwardRadio.Text = "Award / Rank Requirement";
            this.AwardRadio.UseVisualStyleBackColor = true;
            this.AwardRadio.CheckedChanged += new System.EventHandler(this.AwardRadio_CheckedChanged);
            // 
            // ObjectStatRadio
            // 
            this.ObjectStatRadio.AutoSize = true;
            this.ObjectStatRadio.Location = new System.Drawing.Point(13, 47);
            this.ObjectStatRadio.Name = "ObjectStatRadio";
            this.ObjectStatRadio.Size = new System.Drawing.Size(110, 17);
            this.ObjectStatRadio.TabIndex = 7;
            this.ObjectStatRadio.Text = "Player Object Stat";
            this.ObjectStatRadio.UseVisualStyleBackColor = true;
            this.ObjectStatRadio.CheckedChanged += new System.EventHandler(this.ObjectStatRadio_CheckedChanged);
            // 
            // LogicalCriteriaBox
            // 
            this.LogicalCriteriaBox.Controls.Add(this.SumCriteriaList);
            this.LogicalCriteriaBox.Controls.Add(this.DivCriteriaList);
            this.LogicalCriteriaBox.Controls.Add(this.NotCriteriaList);
            this.LogicalCriteriaBox.Controls.Add(this.GenericListRadio);
            this.LogicalCriteriaBox.Controls.Add(this.OrCriteriaList);
            this.LogicalCriteriaBox.Location = new System.Drawing.Point(13, 145);
            this.LogicalCriteriaBox.Name = "LogicalCriteriaBox";
            this.LogicalCriteriaBox.Size = new System.Drawing.Size(180, 156);
            this.LogicalCriteriaBox.TabIndex = 11;
            this.LogicalCriteriaBox.TabStop = false;
            this.LogicalCriteriaBox.Text = "Logical Criteria Lists";
            // 
            // SumCriteriaList
            // 
            this.SumCriteriaList.AutoSize = true;
            this.SumCriteriaList.Location = new System.Drawing.Point(14, 47);
            this.SumCriteriaList.Name = "SumCriteriaList";
            this.SumCriteriaList.Size = new System.Drawing.Size(81, 17);
            this.SumCriteriaList.TabIndex = 0;
            this.SumCriteriaList.TabStop = true;
            this.SumCriteriaList.Text = "Sum Criteria";
            this.SumCriteriaList.UseVisualStyleBackColor = true;
            this.SumCriteriaList.CheckedChanged += new System.EventHandler(this.SumCriteriaList_CheckedChanged);
            // 
            // DivCriteriaList
            // 
            this.DivCriteriaList.AutoSize = true;
            this.DivCriteriaList.Location = new System.Drawing.Point(14, 93);
            this.DivCriteriaList.Name = "DivCriteriaList";
            this.DivCriteriaList.Size = new System.Drawing.Size(97, 17);
            this.DivCriteriaList.TabIndex = 1;
            this.DivCriteriaList.TabStop = true;
            this.DivCriteriaList.Text = "Division Criteria";
            this.DivCriteriaList.UseVisualStyleBackColor = true;
            this.DivCriteriaList.CheckedChanged += new System.EventHandler(this.DivCriteriaList_CheckedChanged);
            // 
            // NotCriteriaList
            // 
            this.NotCriteriaList.AutoSize = true;
            this.NotCriteriaList.Location = new System.Drawing.Point(14, 70);
            this.NotCriteriaList.Name = "NotCriteriaList";
            this.NotCriteriaList.Size = new System.Drawing.Size(122, 17);
            this.NotCriteriaList.TabIndex = 2;
            this.NotCriteriaList.TabStop = true;
            this.NotCriteriaList.Text = "Zero or False Criteria";
            this.NotCriteriaList.UseVisualStyleBackColor = true;
            this.NotCriteriaList.CheckedChanged += new System.EventHandler(this.NotCriteriaList_CheckedChanged);
            // 
            // GenericListRadio
            // 
            this.GenericListRadio.AutoSize = true;
            this.GenericListRadio.Location = new System.Drawing.Point(14, 24);
            this.GenericListRadio.Name = "GenericListRadio";
            this.GenericListRadio.Size = new System.Drawing.Size(116, 17);
            this.GenericListRadio.TabIndex = 3;
            this.GenericListRadio.TabStop = true;
            this.GenericListRadio.Text = "Generic Criteria List";
            this.GenericListRadio.UseVisualStyleBackColor = true;
            this.GenericListRadio.CheckedChanged += new System.EventHandler(this.GenericListRadio_CheckedChanged);
            // 
            // OrCriteriaList
            // 
            this.OrCriteriaList.AutoSize = true;
            this.OrCriteriaList.Location = new System.Drawing.Point(14, 116);
            this.OrCriteriaList.Name = "OrCriteriaList";
            this.OrCriteriaList.Size = new System.Drawing.Size(125, 17);
            this.OrCriteriaList.TabIndex = 4;
            this.OrCriteriaList.TabStop = true;
            this.OrCriteriaList.Text = "Any sub critera is met";
            this.OrCriteriaList.UseVisualStyleBackColor = true;
            this.OrCriteriaList.CheckedChanged += new System.EventHandler(this.OrCriteriaList_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.DescTextBox);
            this.groupBox1.Location = new System.Drawing.Point(13, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(370, 124);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Criteria Description";
            // 
            // DescTextBox
            // 
            this.DescTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.DescTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DescTextBox.Enabled = false;
            this.DescTextBox.Location = new System.Drawing.Point(12, 19);
            this.DescTextBox.Name = "DescTextBox";
            this.DescTextBox.ReadOnly = true;
            this.DescTextBox.Size = new System.Drawing.Size(345, 94);
            this.DescTextBox.TabIndex = 0;
            this.DescTextBox.Text = "";
            // 
            // NewCriteriaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(394, 307);
            this.Controls.Add(this.BtnContinue);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.StatCriteriaBox);
            this.Controls.Add(this.LogicalCriteriaBox);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewCriteriaForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Criteria";
            this.StatCriteriaBox.ResumeLayout(false);
            this.StatCriteriaBox.PerformLayout();
            this.LogicalCriteriaBox.ResumeLayout(false);
            this.LogicalCriteriaBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton SumCriteriaList;
        private System.Windows.Forms.RadioButton DivCriteriaList;
        private System.Windows.Forms.RadioButton NotCriteriaList;
        private System.Windows.Forms.RadioButton GenericListRadio;
        private System.Windows.Forms.RadioButton OrCriteriaList;
        private System.Windows.Forms.RadioButton GlobalStatRadio;
        private System.Windows.Forms.RadioButton ObjectStatRadio;
        private System.Windows.Forms.RadioButton AwardRadio;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox LogicalCriteriaBox;
        private System.Windows.Forms.GroupBox StatCriteriaBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button BtnContinue;
        private System.Windows.Forms.RichTextBox DescTextBox;
        private System.Windows.Forms.RadioButton GlobalMultStatRadio;
    }
}