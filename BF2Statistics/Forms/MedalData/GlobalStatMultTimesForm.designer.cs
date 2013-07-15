namespace BF2Statistics.MedalData
{
    partial class GlobalStatMultTimesForm
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
            this.ValueBox = new System.Windows.Forms.NumericUpDown();
            this.FinishBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.StatName = new System.Windows.Forms.ComboBox();
            this.Hms2SecBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ValueBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ValueBox
            // 
            this.ValueBox.Location = new System.Drawing.Point(108, 71);
            this.ValueBox.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.ValueBox.Name = "ValueBox";
            this.ValueBox.Size = new System.Drawing.Size(110, 20);
            this.ValueBox.TabIndex = 16;
            // 
            // FinishBtn
            // 
            this.FinishBtn.Location = new System.Drawing.Point(60, 120);
            this.FinishBtn.Name = "FinishBtn";
            this.FinishBtn.Size = new System.Drawing.Size(160, 29);
            this.FinishBtn.TabIndex = 15;
            this.FinishBtn.Text = "Finish";
            this.FinishBtn.UseVisualStyleBackColor = true;
            this.FinishBtn.Click += new System.EventHandler(this.FinishBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Criteria Value";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Global Stat Name";
            // 
            // StatName
            // 
            this.StatName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.StatName.FormattingEnabled = true;
            this.StatName.Location = new System.Drawing.Point(108, 28);
            this.StatName.Name = "StatName";
            this.StatName.Size = new System.Drawing.Size(150, 21);
            this.StatName.TabIndex = 11;
            // 
            // Hms2SecBtn
            // 
            this.Hms2SecBtn.Location = new System.Drawing.Point(222, 68);
            this.Hms2SecBtn.Name = "Hms2SecBtn";
            this.Hms2SecBtn.Size = new System.Drawing.Size(36, 23);
            this.Hms2SecBtn.TabIndex = 17;
            this.Hms2SecBtn.Text = "Hms";
            this.Hms2SecBtn.UseVisualStyleBackColor = true;
            this.Hms2SecBtn.Click += new System.EventHandler(this.Hms2SecBtn_Click);
            // 
            // GlobalStatMultTimesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(269, 172);
            this.Controls.Add(this.Hms2SecBtn);
            this.Controls.Add(this.ValueBox);
            this.Controls.Add(this.FinishBtn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.StatName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GlobalStatMultTimesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Global Stat Mult TimesForm";
            ((System.ComponentModel.ISupportInitialize)(this.ValueBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button FinishBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox StatName;
        private System.Windows.Forms.NumericUpDown ValueBox;
        private System.Windows.Forms.Button Hms2SecBtn;
    }
}