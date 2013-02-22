namespace BF2Statistics
{
    partial class InstallForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.ClientPath = new System.Windows.Forms.TextBox();
            this.ServerPath = new System.Windows.Forms.TextBox();
            this.ClientBtn = new System.Windows.Forms.Button();
            this.ServerBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.SaveBtn = new System.Windows.Forms.Button();
            this.IntroTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Battlefield 2 Client Path:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 119);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Battlefield 2 Server Path:";
            // 
            // ClientPath
            // 
            this.ClientPath.Location = new System.Drawing.Point(22, 80);
            this.ClientPath.Name = "ClientPath";
            this.ClientPath.ReadOnly = true;
            this.ClientPath.Size = new System.Drawing.Size(326, 20);
            this.ClientPath.TabIndex = 2;
            // 
            // ServerPath
            // 
            this.ServerPath.Location = new System.Drawing.Point(22, 145);
            this.ServerPath.Name = "ServerPath";
            this.ServerPath.ReadOnly = true;
            this.ServerPath.Size = new System.Drawing.Size(326, 20);
            this.ServerPath.TabIndex = 3;
            // 
            // ClientBtn
            // 
            this.ClientBtn.Location = new System.Drawing.Point(354, 77);
            this.ClientBtn.Name = "ClientBtn";
            this.ClientBtn.Size = new System.Drawing.Size(90, 25);
            this.ClientBtn.TabIndex = 4;
            this.ClientBtn.Text = "Select File";
            this.ClientBtn.UseVisualStyleBackColor = true;
            this.ClientBtn.Click += new System.EventHandler(this.ClientBtn_Click);
            // 
            // ServerBtn
            // 
            this.ServerBtn.Location = new System.Drawing.Point(354, 142);
            this.ServerBtn.Name = "ServerBtn";
            this.ServerBtn.Size = new System.Drawing.Size(90, 25);
            this.ServerBtn.TabIndex = 5;
            this.ServerBtn.Text = "Select File";
            this.ServerBtn.UseVisualStyleBackColor = true;
            this.ServerBtn.Click += new System.EventHandler(this.ServerBtn_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(134, 191);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(100, 32);
            this.CancelBtn.TabIndex = 6;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // SaveBtn
            // 
            this.SaveBtn.Location = new System.Drawing.Point(240, 191);
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Size = new System.Drawing.Size(100, 32);
            this.SaveBtn.TabIndex = 7;
            this.SaveBtn.Text = "Save";
            this.SaveBtn.UseVisualStyleBackColor = true;
            this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // IntroTextBox
            // 
            this.IntroTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.IntroTextBox.Enabled = false;
            this.IntroTextBox.Location = new System.Drawing.Point(23, 18);
            this.IntroTextBox.Name = "IntroTextBox";
            this.IntroTextBox.ReadOnly = true;
            this.IntroTextBox.Size = new System.Drawing.Size(420, 25);
            this.IntroTextBox.TabIndex = 8;
            this.IntroTextBox.Text = "This program requires paths to your BF2 Client, and Dedicated server to be saved." +
                "";
            // 
            // InstallForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 232);
            this.Controls.Add(this.IntroTextBox);
            this.Controls.Add(this.SaveBtn);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.ServerBtn);
            this.Controls.Add(this.ClientBtn);
            this.Controls.Add(this.ServerPath);
            this.Controls.Add(this.ClientPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InstallForm";
            this.Text = "Setup";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ClientPath;
        private System.Windows.Forms.TextBox ServerPath;
        private System.Windows.Forms.Button ClientBtn;
        private System.Windows.Forms.Button ServerBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button SaveBtn;
        private System.Windows.Forms.RichTextBox IntroTextBox;
    }
}