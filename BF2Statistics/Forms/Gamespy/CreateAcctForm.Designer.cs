namespace BF2Statistics
{
    partial class CreateAcctForm
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
            this.AccountName = new System.Windows.Forms.TextBox();
            this.AccountPass = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.AccountEmail = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.CreateBtn = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.PidSelect = new System.Windows.Forms.ComboBox();
            this.PidBox = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.PidBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Window;
            this.label1.Location = new System.Drawing.Point(21, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Account Nick:";
            // 
            // AccountName
            // 
            this.AccountName.Location = new System.Drawing.Point(126, 76);
            this.AccountName.Name = "AccountName";
            this.AccountName.Size = new System.Drawing.Size(166, 20);
            this.AccountName.TabIndex = 1;
            // 
            // AccountPass
            // 
            this.AccountPass.Location = new System.Drawing.Point(126, 106);
            this.AccountPass.Name = "AccountPass";
            this.AccountPass.Size = new System.Drawing.Size(166, 20);
            this.AccountPass.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.Window;
            this.label2.Location = new System.Drawing.Point(21, 109);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Account Password:";
            // 
            // AccountEmail
            // 
            this.AccountEmail.Location = new System.Drawing.Point(126, 137);
            this.AccountEmail.Name = "AccountEmail";
            this.AccountEmail.Size = new System.Drawing.Size(166, 20);
            this.AccountEmail.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.Window;
            this.label3.Location = new System.Drawing.Point(21, 140);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Account Email:";
            // 
            // CreateBtn
            // 
            this.CreateBtn.Location = new System.Drawing.Point(98, 178);
            this.CreateBtn.Name = "CreateBtn";
            this.CreateBtn.Size = new System.Drawing.Size(116, 28);
            this.CreateBtn.TabIndex = 6;
            this.CreateBtn.Text = "Create";
            this.CreateBtn.UseVisualStyleBackColor = true;
            this.CreateBtn.Click += new System.EventHandler(this.CreateBtn_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.SystemColors.Window;
            this.label4.Location = new System.Drawing.Point(21, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Battlefield 2 PID:";
            // 
            // PidSelect
            // 
            this.PidSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PidSelect.FormattingEnabled = true;
            this.PidSelect.Items.AddRange(new object[] {
            "Auto Generate",
            "Custom:"});
            this.PidSelect.Location = new System.Drawing.Point(126, 15);
            this.PidSelect.Name = "PidSelect";
            this.PidSelect.Size = new System.Drawing.Size(166, 21);
            this.PidSelect.TabIndex = 9;
            this.PidSelect.SelectedIndexChanged += new System.EventHandler(this.PidSelect_SelectedIndexChanged);
            // 
            // PidBox
            // 
            this.PidBox.Enabled = false;
            this.PidBox.Location = new System.Drawing.Point(126, 42);
            this.PidBox.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.PidBox.Name = "PidBox";
            this.PidBox.Size = new System.Drawing.Size(166, 20);
            this.PidBox.TabIndex = 10;
            // 
            // CreateAcctForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(312, 220);
            this.Controls.Add(this.PidBox);
            this.Controls.Add(this.PidSelect);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.CreateBtn);
            this.Controls.Add(this.AccountEmail);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.AccountPass);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.AccountName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateAcctForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Gamespy Account";
            ((System.ComponentModel.ISupportInitialize)(this.PidBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox AccountName;
        private System.Windows.Forms.TextBox AccountPass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox AccountEmail;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button CreateBtn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox PidSelect;
        private System.Windows.Forms.NumericUpDown PidBox;
    }
}