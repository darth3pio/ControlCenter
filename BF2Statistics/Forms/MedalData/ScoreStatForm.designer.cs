namespace BF2Statistics.MedalData
{
    partial class ScoreStatForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.StatType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.StatName = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ConditionSelect = new System.Windows.Forms.ComboBox();
            this.FinishBtn = new System.Windows.Forms.Button();
            this.ValueBox = new System.Windows.Forms.NumericUpDown();
            this.Hms2SecBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ValueBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Stat Type";
            // 
            // StatType
            // 
            this.StatType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.StatType.FormattingEnabled = true;
            this.StatType.Items.AddRange(new object[] {
            "Global Stat",
            "Round Stat"});
            this.StatType.Location = new System.Drawing.Point(83, 24);
            this.StatType.Name = "StatType";
            this.StatType.Size = new System.Drawing.Size(170, 21);
            this.StatType.TabIndex = 2;
            this.StatType.SelectedIndexChanged += new System.EventHandler(this.StatType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Stat Name";
            // 
            // StatName
            // 
            this.StatName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.StatName.FormattingEnabled = true;
            this.StatName.Items.AddRange(new object[] {
            "Global Stat",
            "Round Stat"});
            this.StatName.Location = new System.Drawing.Point(83, 62);
            this.StatName.Name = "StatName";
            this.StatName.Size = new System.Drawing.Size(170, 21);
            this.StatName.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Condition";
            // 
            // ConditionSelect
            // 
            this.ConditionSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ConditionSelect.FormattingEnabled = true;
            this.ConditionSelect.Items.AddRange(new object[] {
            "Return Stat Value",
            "Equals or Greater Than"});
            this.ConditionSelect.Location = new System.Drawing.Point(83, 101);
            this.ConditionSelect.Name = "ConditionSelect";
            this.ConditionSelect.Size = new System.Drawing.Size(170, 21);
            this.ConditionSelect.TabIndex = 6;
            this.ConditionSelect.SelectedIndexChanged += new System.EventHandler(this.ConditionSelect_SelectedIndexChanged);
            // 
            // FinishBtn
            // 
            this.FinishBtn.Location = new System.Drawing.Point(62, 173);
            this.FinishBtn.Name = "FinishBtn";
            this.FinishBtn.Size = new System.Drawing.Size(160, 29);
            this.FinishBtn.TabIndex = 10;
            this.FinishBtn.Text = "Finish";
            this.FinishBtn.UseVisualStyleBackColor = true;
            this.FinishBtn.Click += new System.EventHandler(this.FinishBtn_Click);
            // 
            // ValueBox
            // 
            this.ValueBox.Location = new System.Drawing.Point(83, 133);
            this.ValueBox.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.ValueBox.Name = "ValueBox";
            this.ValueBox.Size = new System.Drawing.Size(120, 20);
            this.ValueBox.TabIndex = 11;
            // 
            // Hms2SecBtn
            // 
            this.Hms2SecBtn.Location = new System.Drawing.Point(217, 130);
            this.Hms2SecBtn.Name = "Hms2SecBtn";
            this.Hms2SecBtn.Size = new System.Drawing.Size(36, 23);
            this.Hms2SecBtn.TabIndex = 18;
            this.Hms2SecBtn.Text = "Hms";
            this.Hms2SecBtn.UseVisualStyleBackColor = true;
            this.Hms2SecBtn.Click += new System.EventHandler(this.Hms2SecBtn_Click);
            // 
            // ScoreStatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(279, 214);
            this.Controls.Add(this.Hms2SecBtn);
            this.Controls.Add(this.ValueBox);
            this.Controls.Add(this.FinishBtn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ConditionSelect);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.StatName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.StatType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScoreStatForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Player Stat";
            ((System.ComponentModel.ISupportInitialize)(this.ValueBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox StatType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox StatName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox ConditionSelect;
        private System.Windows.Forms.Button FinishBtn;
        private System.Windows.Forms.NumericUpDown ValueBox;
        private System.Windows.Forms.Button Hms2SecBtn;
    }
}