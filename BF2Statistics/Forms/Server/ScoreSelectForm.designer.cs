namespace BF2Statistics
{
    partial class ScoreSelectForm
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
            this.PlayerBtn = new System.Windows.Forms.Button();
            this.AIBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // PlayerBtn
            // 
            this.PlayerBtn.DialogResult = System.Windows.Forms.DialogResult.No;
            this.PlayerBtn.Location = new System.Drawing.Point(12, 48);
            this.PlayerBtn.Name = "PlayerBtn";
            this.PlayerBtn.Size = new System.Drawing.Size(110, 36);
            this.PlayerBtn.TabIndex = 0;
            this.PlayerBtn.Text = "Player Scoring";
            this.PlayerBtn.UseVisualStyleBackColor = true;
            // 
            // AIBtn
            // 
            this.AIBtn.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.AIBtn.Location = new System.Drawing.Point(136, 48);
            this.AIBtn.Name = "AIBtn";
            this.AIBtn.Size = new System.Drawing.Size(110, 36);
            this.AIBtn.TabIndex = 1;
            this.AIBtn.Text = "AI Bot Scoring";
            this.AIBtn.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Please select an option below:";
            // 
            // ScoreSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(258, 99);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AIBtn);
            this.Controls.Add(this.PlayerBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScoreSelectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Scoring Optoin";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button PlayerBtn;
        private System.Windows.Forms.Button AIBtn;
        private System.Windows.Forms.Label label1;
    }
}