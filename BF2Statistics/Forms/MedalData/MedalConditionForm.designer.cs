namespace BF2Statistics.MedalData
{
    partial class MedalConditionForm
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
            this.ObjectTypeLabel = new System.Windows.Forms.Label();
            this.AwardSelect = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TypeSelect = new System.Windows.Forms.ComboBox();
            this.FinishBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ObjectTypeLabel
            // 
            this.ObjectTypeLabel.AutoSize = true;
            this.ObjectTypeLabel.Location = new System.Drawing.Point(12, 68);
            this.ObjectTypeLabel.Name = "ObjectTypeLabel";
            this.ObjectTypeLabel.Size = new System.Drawing.Size(35, 13);
            this.ObjectTypeLabel.TabIndex = 14;
            this.ObjectTypeLabel.Text = "Name";
            // 
            // AwardSelect
            // 
            this.AwardSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AwardSelect.FormattingEnabled = true;
            this.AwardSelect.Location = new System.Drawing.Point(63, 65);
            this.AwardSelect.Name = "AwardSelect";
            this.AwardSelect.Size = new System.Drawing.Size(200, 21);
            this.AwardSelect.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Type";
            // 
            // TypeSelect
            // 
            this.TypeSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TypeSelect.FormattingEnabled = true;
            this.TypeSelect.Items.AddRange(new object[] {
            "Badge",
            "Medal",
            "Ribbon",
            "Rank"});
            this.TypeSelect.Location = new System.Drawing.Point(63, 34);
            this.TypeSelect.Name = "TypeSelect";
            this.TypeSelect.Size = new System.Drawing.Size(200, 21);
            this.TypeSelect.TabIndex = 11;
            // 
            // FinishBtn
            // 
            this.FinishBtn.Location = new System.Drawing.Point(63, 103);
            this.FinishBtn.Name = "FinishBtn";
            this.FinishBtn.Size = new System.Drawing.Size(160, 29);
            this.FinishBtn.TabIndex = 15;
            this.FinishBtn.Text = "Select";
            this.FinishBtn.UseVisualStyleBackColor = true;
            this.FinishBtn.Click += new System.EventHandler(this.FinishBtn_Click);
            // 
            // MedalConditionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(284, 145);
            this.Controls.Add(this.FinishBtn);
            this.Controls.Add(this.ObjectTypeLabel);
            this.Controls.Add(this.AwardSelect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TypeSelect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MedalConditionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Medal / Rank Condition";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ObjectTypeLabel;
        private System.Windows.Forms.ComboBox AwardSelect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox TypeSelect;
        private System.Windows.Forms.Button FinishBtn;
    }
}