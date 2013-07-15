namespace BF2Statistics
{
    partial class S2hmsForm
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
            this.HoursBox = new System.Windows.Forms.NumericUpDown();
            this.MinutesBox = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.SecondsBox = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.ConvertBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.HoursBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinutesBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SecondsBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Hours";
            // 
            // HoursBox
            // 
            this.HoursBox.Location = new System.Drawing.Point(74, 18);
            this.HoursBox.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.HoursBox.Name = "HoursBox";
            this.HoursBox.Size = new System.Drawing.Size(80, 20);
            this.HoursBox.TabIndex = 1;
            // 
            // MinutesBox
            // 
            this.MinutesBox.Location = new System.Drawing.Point(74, 44);
            this.MinutesBox.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.MinutesBox.Name = "MinutesBox";
            this.MinutesBox.Size = new System.Drawing.Size(80, 20);
            this.MinutesBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Minutes";
            // 
            // SecondsBox
            // 
            this.SecondsBox.Location = new System.Drawing.Point(74, 70);
            this.SecondsBox.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.SecondsBox.Name = "SecondsBox";
            this.SecondsBox.Size = new System.Drawing.Size(80, 20);
            this.SecondsBox.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Seconds";
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(60, 131);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // ConvertBtn
            // 
            this.ConvertBtn.Location = new System.Drawing.Point(35, 102);
            this.ConvertBtn.Name = "ConvertBtn";
            this.ConvertBtn.Size = new System.Drawing.Size(125, 23);
            this.ConvertBtn.TabIndex = 7;
            this.ConvertBtn.Text = "Convert to Seconds";
            this.ConvertBtn.UseVisualStyleBackColor = true;
            this.ConvertBtn.Click += new System.EventHandler(this.ConvertBtn_Click);
            // 
            // S2hmsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(194, 162);
            this.Controls.Add(this.ConvertBtn);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.SecondsBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.MinutesBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.HoursBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "S2hmsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "H.M.S to Seconds";
            ((System.ComponentModel.ISupportInitialize)(this.HoursBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinutesBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SecondsBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown HoursBox;
        private System.Windows.Forms.NumericUpDown MinutesBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown SecondsBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button ConvertBtn;
    }
}