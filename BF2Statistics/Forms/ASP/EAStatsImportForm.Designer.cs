namespace BF2Statistics
{
    partial class EAStatsImportForm
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
            this.PanelAlert = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.InstructionText = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.PanelMain = new System.Windows.Forms.Panel();
            this.PidTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.PanelButtons = new System.Windows.Forms.Panel();
            this.ImportBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.PanelAlert.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.PanelMain.SuspendLayout();
            this.PanelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // PanelAlert
            // 
            this.PanelAlert.BackColor = System.Drawing.SystemColors.Window;
            this.PanelAlert.Controls.Add(this.label3);
            this.PanelAlert.Controls.Add(this.InstructionText);
            this.PanelAlert.Controls.Add(this.pictureBox1);
            this.PanelAlert.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelAlert.Location = new System.Drawing.Point(0, 0);
            this.PanelAlert.Name = "PanelAlert";
            this.PanelAlert.Size = new System.Drawing.Size(414, 50);
            this.PanelAlert.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(53, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(348, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Make sure you are redirecting bf2web.gamespy.com using the Hosts File";
            // 
            // InstructionText
            // 
            this.InstructionText.AutoSize = true;
            this.InstructionText.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InstructionText.ForeColor = System.Drawing.Color.MidnightBlue;
            this.InstructionText.Location = new System.Drawing.Point(50, 8);
            this.InstructionText.Name = "InstructionText";
            this.InstructionText.Size = new System.Drawing.Size(222, 19);
            this.InstructionText.TabIndex = 2;
            this.InstructionText.Text = "Gamespy Services Are Offline!";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::BF2Statistics.Properties.Resources.vistaWarning;
            this.pictureBox1.Location = new System.Drawing.Point(11, 9);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // PanelMain
            // 
            this.PanelMain.BackColor = System.Drawing.SystemColors.Window;
            this.PanelMain.Controls.Add(this.PidTextBox);
            this.PanelMain.Controls.Add(this.label2);
            this.PanelMain.Controls.Add(this.label1);
            this.PanelMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelMain.Location = new System.Drawing.Point(0, 50);
            this.PanelMain.Name = "PanelMain";
            this.PanelMain.Size = new System.Drawing.Size(414, 105);
            this.PanelMain.TabIndex = 7;
            // 
            // PidTextBox
            // 
            this.PidTextBox.Location = new System.Drawing.Point(125, 63);
            this.PidTextBox.Name = "PidTextBox";
            this.PidTextBox.Size = new System.Drawing.Size(260, 20);
            this.PidTextBox.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Enter Player\'s ID:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 14);
            this.label1.MaximumSize = new System.Drawing.Size(375, 0);
            this.label1.MinimumSize = new System.Drawing.Size(400, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(400, 26);
            this.label1.TabIndex = 3;
            this.label1.Text = "This area will allow you to Import player data from a different ASP server\'s data" +
                "base. You cannot import a player if the player ID exists already on your server." +
                "";
            // 
            // PanelButtons
            // 
            this.PanelButtons.Controls.Add(this.ImportBtn);
            this.PanelButtons.Controls.Add(this.CancelBtn);
            this.PanelButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelButtons.Location = new System.Drawing.Point(0, 155);
            this.PanelButtons.Name = "PanelButtons";
            this.PanelButtons.Size = new System.Drawing.Size(414, 50);
            this.PanelButtons.TabIndex = 0;
            // 
            // ImportBtn
            // 
            this.ImportBtn.Location = new System.Drawing.Point(299, 11);
            this.ImportBtn.Name = "ImportBtn";
            this.ImportBtn.Size = new System.Drawing.Size(100, 28);
            this.ImportBtn.TabIndex = 6;
            this.ImportBtn.Text = "Import Player";
            this.ImportBtn.UseVisualStyleBackColor = true;
            this.ImportBtn.Click += new System.EventHandler(this.ImportBtn_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(193, 11);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(100, 28);
            this.CancelBtn.TabIndex = 5;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // EAStatsImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(414, 202);
            this.Controls.Add(this.PanelButtons);
            this.Controls.Add(this.PanelMain);
            this.Controls.Add(this.PanelAlert);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(420, 180);
            this.Name = "EAStatsImportForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ASP Stats Import";
            this.PanelAlert.ResumeLayout(false);
            this.PanelAlert.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.PanelMain.ResumeLayout(false);
            this.PanelMain.PerformLayout();
            this.PanelButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel PanelAlert;
        private System.Windows.Forms.Panel PanelMain;
        private System.Windows.Forms.TextBox PidTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label InstructionText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel PanelButtons;
        private System.Windows.Forms.Button ImportBtn;
        private System.Windows.Forms.Button CancelBtn;
    }
}